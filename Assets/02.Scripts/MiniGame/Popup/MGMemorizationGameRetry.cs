using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
using HHK.UIEC;
using FMODUnity;
using FMOD.Studio;

public class MGMemorizationGameRetry : BaseGameRetry
{
    [SerializeField]
    private MGMemorizationManager _gameManager;

    [Header("----------------- Globalization Text -----------------")]
    [SerializeField]
    private UnityEngine.UI.Text _itemSummaryText;
    [SerializeField]
    private UnityEngine.UI.Text _retryBtnText;

    public override void OnDestroy()
    {
        base.OnDestroy();
        _gameManager = null;
    }

    public override void ShowPopup()
    {
        base.ShowPopup();
        _gameManager.SetGameState(Define.MiniGameState.Paused);
    }
}
