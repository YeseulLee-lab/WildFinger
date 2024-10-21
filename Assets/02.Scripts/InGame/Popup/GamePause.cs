using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
using HHK.UIEC;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Events;

public class GamePause : BaseGamePause
{
    public override void Start()
    {
        base.Start();
        base.resumeAction = () => BeatGridTracker.SetGameState(Define.InGameState.Resumed);
        base.cntDownAction = () => BeatGridTracker.SetGameState(Define.InGameState.PausedCntDown);
    }

    public override void OnApplicationFocus(bool focus)
    {
        base.OnApplicationFocus(focus);
    }

    #region UI Action
    public override void ShowPause()
    {
        BeatGridTracker.rspInputManager.ResetBtnUI();

        base.ShowPause();
    }

    public override void OnClickRetryBtn()
    {
        base.SetInteractable(false);
        base.ShowBtnClickSFX();

        if (GamePlayData.Instance == null)
        {
            return;
        }

        if (GamePlayData.Instance.heartTimer.heartCnt <= 1 && !TownDataLoader.isTraining)
        {
            GamePlayData.Instance.noHeartPopup.ShowPopup();
            return;
        }

        base.HidePause();

        if (!TownDataLoader.isTraining)
        {
            BeatGridTracker.Instance.judgeChecker.ShowRetry();
            GamePlayData.Instance.getQuaverCnt = 0;
        }
        else
        {
            BeatGridTracker.ShowResult();
        }
    }

    public override void OnClickBackToMainBtn()
    {
        base.SetInteractable(false);
        base.ShowBtnClickSFX();
     
        if (GamePlayData.Instance == null)
        {
            return;
        }

        if (!TownDataLoader.isTraining)
        {
            DebugX.Log("플레이 포기 (메인으로 돌아갈 시) 하트 차감");
            GamePlayData.Instance.heartTimer.heartCnt--;
            GamePlayData.Instance.getQuaverCnt = 0;
            GamePlayData.Instance.getCoinCnt = 0;
        }

        SceneSwitcher.Instance.SwitchScene(Define.SceneName.Main);
    }

    public override void CancelCountdown(UnityAction cancelCntDownAction = null)
    {
        base.CancelCountdown(() => OnClickResumeBtn()); ;
    }
    #endregion
}