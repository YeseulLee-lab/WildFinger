using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using HHK.UIEC;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using DG.Tweening;
using FMODUnity;
using FMOD.Studio;

public class NoteHoldUI : MonoBehaviour
{
    private Image _holdingFillImg;
    private RectTransform _holdingCircleRect;
    private Vector2 _startHoldingCirclePos = Vector2.zero;
    private float _fillCircleRadius = 183f;
    private CancellationTokenSource _fillAnimCts;

    public bool isHolding { get; set; }
    public bool isHoldingFirstHit { get; set; }
    public int holdingBeatRemainCnt { get; set; } = 0;

    public void InitUI(int totalHoldingBeatCnt)
    {
        holdingBeatRemainCnt = totalHoldingBeatCnt - 1;
        _holdingFillImg = this.GetComponent<RectTransform>().GetChild(0).GetComponent<Image>();
        _holdingCircleRect = this.GetComponent<RectTransform>().GetChild(1).GetComponent<RectTransform>();
        _fillCircleRadius = this.GetComponent<RectTransform>().sizeDelta.x * 0.5f;
        _startHoldingCirclePos = new Vector2(0f, _fillCircleRadius - _holdingCircleRect.sizeDelta.x * 0.125f);
        _holdingFillImg.fillAmount = 0f;
    }

    public void StartHolding(int curBeatRemain, float beatPerSec, UnityAction perfectAction = null)
    {
        _fillAnimCts?.Cancel();
        _fillAnimCts = new CancellationTokenSource();
        StartHoldingFillAnimAsync(curBeatRemain, beatPerSec, _fillAnimCts.Token, perfectAction).Forget();
    }

    public void EndHolding()
    {
        _fillAnimCts?.Cancel();
    }

    private async UniTask StartHoldingFillAnimAsync(int curBeatRemain, float beatPerSec, CancellationToken cancellationToken, UnityAction perfectAction)
    {
        _holdingCircleRect.anchoredPosition = _startHoldingCirclePos;
        _holdingFillImg.fillAmount = 0f;

        float startTime = Time.time;
        float endTime = startTime + curBeatRemain * beatPerSec;

        try
        {
            while (Time.time < endTime)
            {
                cancellationToken.ThrowIfCancellationRequested();

                float elapsedTime = Time.time - startTime;
                float normalizedTime = elapsedTime / (curBeatRemain * beatPerSec);
                _holdingFillImg.fillAmount = normalizedTime;

                float angle = Mathf.Lerp(0f, 360f, normalizedTime);
                Vector2 circlePosition = Quaternion.Euler(0, 0, -angle) * _startHoldingCirclePos;
                _holdingCircleRect.anchoredPosition = circlePosition;

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }

            //TODO: Perfect
            _holdingCircleRect.anchoredPosition = _startHoldingCirclePos;
            _holdingFillImg.fillAmount = 1f;
            perfectAction?.Invoke();

        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때 처리할 내용
            _holdingCircleRect.anchoredPosition = _startHoldingCirclePos;
            _holdingFillImg.fillAmount = 0f;
        }
    }
}