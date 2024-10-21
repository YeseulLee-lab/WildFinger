using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine.Events;

public class CommonLoadingPanel : BaseLoadingPanel
{
    [Header("------------------ GUI -----------------")]
    [SerializeField]
    private GameObject _loadingPanel;
    [SerializeField]
    private UnityEngine.UI.Text _summaryText;
    [SerializeField]
    private Image _downloadingFillImg;
    private float _downloadingBarFillGap = 4f;
    private float _downloadingFullFillWidth = 0;

    [Header("------------------ Circles -----------------")]
    [SerializeField]
    private RectTransform _circleRect;
    private const float _largeSize = 148f;
    private const float _smallSize = 100;
    private const float _rotationStep = -90f;
    private const float _rotationDuration = 0.45f;
    private const float _waitDelay = 0.25f;
    private CancellationTokenSource _cancellationTokenSource = null;

    #region Unity Life Cycle
    private void Awake()
    {
        Hide();
    }

    private void OnDestroy()
    {
        _circleRect = null;
        _cancellationTokenSource = null;
    }
    #endregion

    #region Show & Hide
    /// <summary>
    /// Black Alpha(Default) 로딩 호출
    /// </summary>
    /// <param name="isDownloading"></param>
    public override void Show(UnityAction<float> progress = null, UnityAction DownloadComplete = null)
    {
        _loadingPanel.SetActive(true);
        _cancellationTokenSource = new CancellationTokenSource();
        StartAnimation(_cancellationTokenSource.Token).Forget();
    }

    /// <summary>
    /// Black Alpha(Default) 로딩 호출
    /// </summary>
    public override void Hide()
    {
        if (!_loadingPanel.activeSelf)
        {
            return;
        }

        if(_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        _loadingPanel.SetActive(false);
    }
    #endregion

    #region UI Action
    private async UniTaskVoid StartAnimation(CancellationToken token)
    {
        int sequence = 1;
        float currentRotation = 0f;
        while (!token.IsCancellationRequested)
        {
            _circleRect.localRotation = Quaternion.Euler(new Vector3(0, 0, currentRotation));
            currentRotation = _rotationStep * sequence;
            _circleRect.DORotate(new Vector3(0, 0, currentRotation), _rotationDuration, RotateMode.Fast).SetEase(Ease.Linear).OnComplete(
                           () => {
                           });
            _circleRect.DOSizeDelta(new Vector2(_smallSize, _smallSize), _rotationDuration * 0.5f).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                _circleRect.DOSizeDelta(new Vector2(_largeSize, _largeSize), _rotationDuration * 0.5f).SetEase(Ease.Linear);
            });
            if (++sequence > 4)
            {
                sequence = 1;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(_rotationDuration + _waitDelay), cancellationToken: token);
        }
    }
    #endregion
}
