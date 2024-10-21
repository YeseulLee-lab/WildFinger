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

public class WavingCircleWithDirection : WavingCircle
{
    [Header("------------------ Direction GUI Setting -----------------")]
    [SerializeField]
    private Image _arrowImg;
    [SerializeField]
    private RectTransform _arrowStartRect;
    [SerializeField]
    private RectTransform _arrowEndRect;
    private const float _movingDelay = 1f;
    private CancellationTokenSource _cancellationTokenSource;

    #region Unity Life Cycle
    public override void OnEnable()
    {
        base.OnEnable();
        _cancellationTokenSource = new CancellationTokenSource();
        MoveArrowImageAsync(_arrowStartRect.anchoredPosition, _arrowEndRect.anchoredPosition, _cancellationTokenSource.Token).Forget(); // Example startPos and endPos
    }

    public override void OnDisable()
    {
        base.OnDisable();
        _cancellationTokenSource?.Cancel();
    }
    #endregion

    private async UniTaskVoid MoveArrowImageAsync(Vector2 startPos, Vector2 endPos, CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                // Initialize the position and alpha
                _arrowImg.rectTransform.anchoredPosition = startPos;
                _arrowImg.color = new Color(_arrowImg.color.r, _arrowImg.color.g, _arrowImg.color.b, 0);

                // Create tweens
                var moveTween = _arrowImg.rectTransform.DOAnchorPos(endPos, _movingDelay).SetEase(Ease.Linear);
                var fadeTween = _arrowImg.DOFade(1, _movingDelay).SetEase(Ease.Linear);

                // Await completion of tweens
                await UniTask.WhenAll(
                    moveTween.OnCompleteAsUniTask(cancellationToken: cancellationToken),
                    fadeTween.OnCompleteAsUniTask(cancellationToken: cancellationToken)
                );

                // Delay before the next iteration
                //await UniTask.Delay(TimeSpan.FromSeconds(_movingDelay), cancellationToken: cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Animation was canceled
        }
    }
}

public static class DOTweenExtensions
{
    public static UniTask OnCompleteAsUniTask(this Tween tween, CancellationToken cancellationToken)
    {
        var tcs = new UniTaskCompletionSource();
        tween.OnComplete(() => tcs.TrySetResult());
        cancellationToken.Register(() => {
            tween.Kill();
            tcs.TrySetCanceled();
        });
        return tcs.Task;
    }
}
