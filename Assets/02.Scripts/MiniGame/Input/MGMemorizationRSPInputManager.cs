using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using TMPro;
using UnityEngine.Video;
using DG.Tweening;
using System;

public class MGMemorizationRSPInputManager : BaseRSPInputManager
{
    private MGMemorizationJudgeChecker _judgeChecker;

    #region Unity Life Cycle
    public override void Awake()
    {
        base.Awake();
        _judgeChecker = this.GetComponent<MGMemorizationJudgeChecker>();
    }

    public override void Update()
    {
        //Entering 일 때만 입력 가능하게
        if(MGMemorizationManager.Instance == null)
        {
            return;
        }

        if (!MGMemorizationManager.Instance.isEnteringState)
        {
            return;
        }

        base.Update();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _judgeChecker = null;
    }
    #endregion

    #region UI Action
    public override void OnPointerDown(int type, BaseInputUI input)
    {
        if (!isEnteringReady)
        {
            return;
        }

        base.OnPointerDown(type, input);
        _judgeChecker.JudgeSequence((Define.RSPType)type);
    }

    public override void OnPointerUp(int type, BaseInputUI input)
    {
        if (!isEnteringReady)
        {
            return;
        }

        //TODO: 추후 sustain note에서 사용
        base.OnPointerUp(type, input);
    }
    #endregion
}
