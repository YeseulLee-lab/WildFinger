using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using HHK.UIEC;
using FMODUnity;
using FMOD.Studio;

public class WavingCircle : MonoBehaviour
{
    [Header("------------------ Circle GUI Setting -----------------")]
    [SerializeField]
    private Image _glowImg;
    [SerializeField]
    private Image[] _wavingCircles;

    [Header("------------------ Setting -----------------")]
    [SerializeField]
    private int _minSize = 120;
    [SerializeField]
    private int _maxSize = 312;
    [SerializeField]
    private float _sizeIncreaseRate = 50f; // 시간에 따른 증가율
    private CancellationTokenSource _cancellationTokenSource;
    private Coroutine _glowCoroutine;

    #region Unity Life Cycle
    public virtual void OnEnable()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        AnimateCircles(_cancellationTokenSource.Token).Forget();
        _glowCoroutine = StartCoroutine(AnimateGlowAlpha(1f));
    }

    public virtual void OnDisable()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();

        if (_glowCoroutine != null)
        {
            StopCoroutine(_glowCoroutine);
        }
    }
    #endregion

    private async UniTaskVoid AnimateCircles(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            for (int i = 0; i < _wavingCircles.Length; i++)
            {
                RectTransform rectTransform = _wavingCircles[i].rectTransform;
                Vector2 newSize = rectTransform.sizeDelta;

                newSize += Vector2.one * _sizeIncreaseRate * Time.deltaTime;

                if (newSize.x >= _maxSize)
                {
                    newSize = Vector2.one * _minSize;
                }

                float alpha = Mathf.Lerp(0.2f, 0f, (newSize.x - _minSize) / (_maxSize - _minSize));
                Color color = _wavingCircles[i].color;
                color.a = alpha;
                _wavingCircles[i].color = color;

                rectTransform.sizeDelta = newSize;
            }

            await UniTask.Yield(cancellationToken); // 다음 프레임까지 대기
        }
    }

    /// <summary>
    /// _glowImg 이미지 알파값이 interval 초 마다 0 => 1 => 0... 이 반복됨
    /// </summary>
    /// <param name="interval"></param>
    private IEnumerator AnimateGlowAlpha(float interval)
    {
        while (true)
        {
            // 알파값을 0에서 1로 변경
            _glowImg.DOFade(1f, interval).SetEase(Ease.Linear);
            yield return new WaitForSeconds(interval);

            // 알파값을 1에서 0으로 변경
            _glowImg.DOFade(0f, interval).SetEase(Ease.Linear);
            yield return new WaitForSeconds(interval);
        }
    }
}
