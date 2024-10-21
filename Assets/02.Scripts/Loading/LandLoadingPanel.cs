using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using HHK.UIEC;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class LandLoadingPanel : BaseLoadingPanel
{
    [Header("------------------ Common -----------------")]
    [SerializeField]
    private GameObject _loadingPanel;
    private Image _bgImg;
    private const float _hideTermDelaySec = 0.25f;
    private const float _bgDelaySec = 0.65f;
    [SerializeField]
    private GameObject _shineVFX;

    [Header("------------------ Balloon -----------------")]
    [SerializeField]
    private Image _balloonImg;
    [SerializeField]
    private RectTransform[] _balloonRects; //0: start, 1: Stay, 2: Bye
    [SerializeField]
    private Sprite[] _balloonImgs;
    private const float _balloonMovingDelaySec = 0.5f;
    private const float _balloonImgDelaySec = 0.25f;
    private const float _balloonAnimDelaySec = 1.5f;

    [Header("-------------------- Cloud -------------------")]
    [SerializeField]
    private RectTransform[] _cloudRects;
    private Vector2[] _cloudsStartAnchoredPos;
    private Vector2[] _cloudsEndAnchoredPos;
    private const float _cloudGatheringDelaySec = 0.2f;
    private float[] _cloudsCircleRadius = new float[4] { 20f, 15f, 30f, 10f };
    private const float _cloudCircleDelaySec = 4f;

    [Header("------------------ Setting -----------------")]
    [SerializeField]
    private Color32[] _colors; //0: blue alpha, 1: blue, 2: white alpha, 3: white
    private CancellationTokenSource _cancellationTokenSource;
    private bool _isShowed = false;

    #region Unity Life Cycle
    private void Awake()
    {
        _bgImg = _loadingPanel.GetComponent<Image>();
        _cloudsStartAnchoredPos = new Vector2[_cloudRects.Length];
        _cloudsEndAnchoredPos = new Vector2[_cloudRects.Length];

        // Initialize cloud start and end positions
        InitializeCloudPositions();
        Hide();
    }

    private void InitializeCloudPositions()
    {
        _cloudsStartAnchoredPos[0] = new Vector2(_cloudRects[0].sizeDelta.x, -_cloudRects[0].sizeDelta.y);
        _cloudsEndAnchoredPos[0] = _cloudRects[0].anchoredPosition;

        _cloudsStartAnchoredPos[1] = new Vector2(-_cloudRects[1].sizeDelta.x, _cloudRects[1].sizeDelta.y);
        _cloudsEndAnchoredPos[1] = _cloudRects[1].anchoredPosition;

        _cloudsStartAnchoredPos[2] = new Vector2(_cloudRects[2].sizeDelta.x, _cloudRects[2].sizeDelta.y);
        _cloudsEndAnchoredPos[2] = _cloudRects[2].anchoredPosition;

        _cloudsStartAnchoredPos[3] = new Vector2(-_cloudRects[3].sizeDelta.x, -_cloudRects[3].sizeDelta.y);
        _cloudsEndAnchoredPos[3] = _cloudRects[3].anchoredPosition;
    }
    #endregion

    #region Show & Hide
    public override void Show(UnityAction<float> progress = null, UnityAction DownloadComplete = null)
    {
        if (_isShowed)
        {
            return;
        }

        _isShowed = true;
        _shineVFX.SetActive(false);
        _loadingPanel.SetActive(true);
        _balloonImg.gameObject.SetActive(true);
        _cancellationTokenSource = new CancellationTokenSource();

        // Initialize components
        InitializeComponents();

        // Change background color
        _bgImg.DOColor(_colors[1], _bgDelaySec).SetEase(Ease.Linear);

        // Start animations
        AnimateClouds(_cancellationTokenSource.Token).Forget();
        AnimateBalloon(_cancellationTokenSource.Token, progress, DownloadComplete).Forget();
    }

    private void InitializeComponents()
    {
        _balloonImg.color = _colors[3];
        _balloonImg.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
        _balloonImg.rectTransform.localScale = Vector3.one;
        _balloonImg.rectTransform.anchoredPosition = _balloonRects[0].anchoredPosition;
        for (int i = 0; i < _cloudRects.Length; i++)
        {
            _cloudRects[i].anchoredPosition = _cloudsStartAnchoredPos[i];
        }
    }

    public override async void Hide()
    {
        if (!_isShowed)
        {
            return;
        }

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        _balloonImg.rectTransform.DOScale(0.4f, 0.4f);
        _balloonImg.DOColor(_colors[2], 0.4f).SetEase(Ease.Linear);
        _balloonImg.rectTransform.DOAnchorPos(_balloonRects[2].anchoredPosition, 0.2f).SetEase(Ease.Linear).OnComplete(() => { _balloonImg.gameObject.SetActive(false); });
 

        // Animate clouds gathering
        for (int i = 0; i < _cloudRects.Length; i++)
        {
            _cloudRects[i].DOAnchorPos(_cloudsStartAnchoredPos[i], _cloudGatheringDelaySec * 3).SetEase(Ease.OutCubic);
        }

        _shineVFX.SetActive(true);
        await UniTask.Delay((int)(_hideTermDelaySec * 1000), cancellationToken: _cancellationTokenSource.Token);
        _bgImg.DOColor(_colors[0], _bgDelaySec).SetEase(Ease.Linear);

        await UniTask.Delay((int)(_bgDelaySec * 1000), cancellationToken: _cancellationTokenSource.Token);

        _loadingPanel.SetActive(false);
        _isShowed = false;
    }
    #endregion

    #region Animations
    private async UniTaskVoid AnimateClouds(CancellationToken token)
    {
        for (int i = 0; i < _cloudRects.Length; i++)
        {
            await _cloudRects[i].DOAnchorPos(_cloudsEndAnchoredPos[i], _cloudGatheringDelaySec).SetEase(Ease.OutCubic).AsyncWaitForCompletion();
            if (token.IsCancellationRequested) return;
            AnimateCloudCircle(_cloudRects[i], _cloudsCircleRadius[i], _cloudCircleDelaySec, token).Forget();
        }
    }

    private async UniTaskVoid AnimateCloudCircle(RectTransform cloud, float radius, float duration, CancellationToken token)
    {
        Vector2 initialPos = cloud.anchoredPosition;
        float angle = 0f;
        float angleStep = 1f; // 각도 증가 단위

        while (!token.IsCancellationRequested)
        {
            float startAngle = angle;
            float endAngle = angle + angleStep;

            if (endAngle >= 360f)
            {
                endAngle -= 360f;
            }

            float startTime = Time.time;

            while (Time.time - startTime < duration / 360f)
            {
                if (token.IsCancellationRequested) return;

                float t = (Time.time - startTime) / (duration / 360f);
                float currentAngle = Mathf.Lerp(startAngle, endAngle, t) * Mathf.Deg2Rad;
                cloud.anchoredPosition = initialPos + new Vector2(Mathf.Cos(currentAngle) * radius, Mathf.Sin(currentAngle) * radius);

                await UniTask.Yield(token);
            }

            angle = endAngle;
        }
    }

    private async UniTaskVoid AnimateBalloon(CancellationToken token, UnityAction<float> progress = null, UnityAction DownloadComplete = null)
    {
        await UniTask.Delay((int)(_cloudGatheringDelaySec * _cloudRects.Length * 1000), cancellationToken: _cancellationTokenSource.Token);
        await _balloonImg.rectTransform.DOAnchorPos(_balloonRects[1].anchoredPosition, _balloonMovingDelaySec).SetEase(Ease.OutCubic).AsyncWaitForCompletion();

        if (token.IsCancellationRequested) return;

        UniTask spriteTask = UpdateBalloonSprite(token);
        UniTask animTask = AnimateBalloonChannel(token);

        float progressValue = 0f;
        while (!token.IsCancellationRequested)
        {
            progressValue += Time.deltaTime / 3; // 3초 동안 진행 상태를 1로 증가
            progress?.Invoke(progressValue);

            if (progressValue >= 1f)
            {
                // 진행 상태가 1f가 되면 다운로드 완료 처리
                DownloadComplete?.Invoke();
                break;
            }

            await UniTask.Yield(token);
        }

        // 애니메이션과 스프라이트 업데이트 작업 취소
        _cancellationTokenSource.Cancel();

        // 다운로드 완료 처리
        Hide();
    }

    private async UniTask UpdateBalloonSprite(CancellationToken token)
    {
        int imgIndex = 0;
        while (!token.IsCancellationRequested)
        {
            _balloonImg.sprite = _balloonImgs[imgIndex];
            imgIndex = (imgIndex + 1) % _balloonImgs.Length;
            await UniTask.Delay((int)(_balloonImgDelaySec * 1000), cancellationToken: token);
        }
    }

    private async UniTask AnimateBalloonChannel(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            _balloonImg.GetComponent<UIECAnimator>().OnCustomChannel();
            await UniTask.Delay((int)(_balloonAnimDelaySec * 1000), cancellationToken: token);
        }
    }
    #endregion
}
