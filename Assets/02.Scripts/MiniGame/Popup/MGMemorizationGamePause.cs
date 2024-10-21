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

public class MGMemorizationGamePause : BaseGamePause
{
    [SerializeField]
    private MGMemorizationManager _gameManager;

    public override void OnDestroy()
    {
        base.OnDestroy();
        _gameManager = null;
    }

    public override void Start()
    {
        base.Start();
        base.resumeAction = () => _gameManager.SetGameState(Define.MiniGameState.Resumed);
        base.cntDownAction = () => _gameManager.SetGameState(Define.MiniGameState.PausedCntDown);
    }

    public override void CancelCountdown(UnityAction cancelCntDownAction = null)
    {
        base.CancelCountdown(() => OnClickResumeBtn());
    }
}