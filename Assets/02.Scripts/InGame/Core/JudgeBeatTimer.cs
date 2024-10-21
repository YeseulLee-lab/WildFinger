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

public class JudgeBeatTimer : MonoBehaviour
{
    [Header("------------------ GUI Setting -----------------")]
    [SerializeField]
    private Image _fillImg;
    [SerializeField]
    private Image _barImg;
    [SerializeField]
    private UIECAnimator _barGlowAnim; // UIEC Animator 있음(흔들리는 효과)
    private CanvasGroup _fillCg;

    [Header("------------------ Setting -----------------")]
    private Vector2 _barStartPos;
    private Vector2 _barEndPos;
    private CancellationTokenSource _cts;
    [SerializeField]
    private EventReference _timerSfx;
    private EventInstance _timerInstance;

    private float _elapsedTime = 0f;
    private float _remainingTime = 0f;
    private bool _isTimerOn = false;
    private bool _isPaused = false;

    #region Unity Life Cycle
    private void Awake()
    {
        _isTimerOn = false;
        _isPaused = false;
        _fillCg = _fillImg.GetComponent<CanvasGroup>();
        _barStartPos = new Vector2(_barImg.rectTransform.sizeDelta.x * 0.5f, 0f);
        _barEndPos = new Vector2(_barStartPos.x - _fillImg.rectTransform.sizeDelta.x, 0f);

        _fillCg.alpha = 0f;

        _timerInstance = RuntimeManager.CreateInstance(_timerSfx);
    }

    private void Start()
    {
        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _timerInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    private void OnDestroy()
    {
        _fillImg = null;
        _barImg = null;
        _barGlowAnim = null;

        _cts?.Cancel();
        _cts?.Dispose();

        _timerInstance.setUserData(IntPtr.Zero);
        _timerInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _timerInstance.release();
    }
    #endregion

    /// <summary>
    /// 타이머 시작
    /// </summary>
    /// <param name="maxBeat">최대 박자 수</param>
    /// <param name="beatPerSec">1박자 당 초</param>
    public async void StartTimer(int maxBeat, float beatPerSec)
    {
        _isTimerOn = true;
        _isPaused = false;
        _elapsedTime = 0f;
        float maxTime = maxBeat * beatPerSec;
        _remainingTime = maxTime;

        _fillImg.fillAmount = 1f;
        _barImg.rectTransform.anchoredPosition = _barStartPos;

        _fillCg.alpha = 1f;

        _cts = new CancellationTokenSource();
        _timerInstance.start();
        try
        {
            // UI 업데이트
            await UpdateUI(_remainingTime, _cts.Token);
        }
        catch (OperationCanceledException)
        {
            // DebugX.Log("타이머가 취소되었습니다.");
            _timerInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _isTimerOn = false;
        }
        catch (Exception ex)
        {
            // DebugX.LogError($"타이머 중 예외가 발생했습니다: {ex.Message}");
            _timerInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _isTimerOn = false;
        }
    }

    private async UniTask UpdateUI(float duration, CancellationToken token)
    {
        float startTime = Time.time;
        while (_elapsedTime < duration && !token.IsCancellationRequested)
        {
            if (!_isPaused)
            {
                _elapsedTime += Time.deltaTime;
                _remainingTime = duration - _elapsedTime;

                float fillAmount = _remainingTime / duration;
                _fillImg.fillAmount = fillAmount;
                float barPosX = Mathf.Lerp(_barStartPos.x, _barEndPos.x, 1 - fillAmount);
                _barImg.rectTransform.anchoredPosition = new Vector2(barPosX, _barStartPos.y);
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        //_fillCg.alpha = 0f;
    }

    /// <summary>
    /// 타이머를 일시 정지 or Resume 시킴
    /// </summary>
    public void SetJudgeBeatTimerState(bool isPause)
    {
        if (!_isTimerOn)
        {
            return;
        }

        _isPaused = isPause;
        _timerInstance.setPaused(isPause);

        if (_isPaused)
        {
            _fillImg.DOPause();
            _barImg.rectTransform.DOPause();
        }
        else
        {
            _fillImg.DOPlay();
            _barImg.rectTransform.DOPlay();
        }
    }

    /// <summary>
    /// 타이머를 강제로 종료 시킴
    /// </summary>
    public void EndTimer()
    {
        if (!_isTimerOn)
        {
            return;
        }

        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        _fillImg.DOKill();
        _barImg.rectTransform.DOKill();

        _fillCg.alpha = 0f;
        _timerInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _isTimerOn = false;
        _isPaused = false;
    }
}
