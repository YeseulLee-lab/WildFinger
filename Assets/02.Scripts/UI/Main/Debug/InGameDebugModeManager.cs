using DG.Tweening;
using HHK.UIEC;
using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

public class InGameDebugModeManager : MonoBehaviour
{
    [SerializeField]
    private Button _skipBtn;

    private int _clickCount = 0;
    private float _clickThreshold = 0.5f; // Max time interval between clicks to count as a sequence
    private float _lastClickTime = 0f;
    private CancellationTokenSource _cts;
    private const int _debugModeTriggerMaxCnt = 3;

    private void Start()
    {
        _skipBtn?.onClick.AddListener(OnClickSkipBtn);
    }

    private void OnDestroy()
    {
        _skipBtn?.onClick.RemoveListener(OnClickSkipBtn);
        _cts?.Cancel();
        _cts?.Dispose();
    }

    private void OnClickSkipBtn()
    {
        if(GamePlayData.Instance == null)
        {
            return;
        }

        if (!GamePlayData.Instance.isDebugMode)
        {
            return;
        }

        float timeSinceLastClick = Time.time - _lastClickTime;
        _lastClickTime = Time.time;

        if (timeSinceLastClick > _clickThreshold)
        {
            _clickCount = 0; // Reset if the time between clicks is too long
        }
        else
        {
            _clickCount++;
        }

        if (_clickCount >= _debugModeTriggerMaxCnt)
        {
            _clickCount = 0; // Reset click count
            _skipBtn.interactable = false;
            TriggerDebugMode();
        }
    }

    private void TriggerDebugMode()
    {
        DebugX.Log("Debug mode triggered!");

        switch (SceneSwitcher.Instance.curSceneName)
        {
            case Define.SceneName.Game:
            case Define.SceneName.Training:
                if(BeatGridTracker.curState == Define.InGameState.Playing)
                    BeatGridTracker.ShowResult();
                break;
            case Define.SceneName.MGMemorization:
                if(MGMemorizationManager.Instance.curState == Define.MiniGameState.Playing)
                    MGMemorizationManager.Instance.ShowResult(true);
                break;
        }
    }
}
