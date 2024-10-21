using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using HHK.UIEC;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using FMOD.Studio;

/// <summary>
/// 변장술사: 2박자 전에 진짜로직보여줌
/// </summary>
public class DSGNoteUnit : BaseNoteUnit
{
    [Header("------------------ FMOD Area -----------------")]
    [SerializeField]
    private EventReference _openSfx;
    private EventInstance _openSfxInstance;

    [Header("------------------ Gimmick Area -----------------")]
    [SerializeField]
    private UIECGroup _dsgGroupAnim;
    private const int _dsgLimitIndex = 3;

    #region Unity Life Cycle
    public override void Awake()
    {
        base.Awake();

        _openSfxInstance = RuntimeManager.CreateInstance(_openSfx);
    }

    public override void Start()
    {
        base.Start();

        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _openSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        _openSfxInstance.setUserData(IntPtr.Zero);
        _openSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _openSfxInstance.release();

        _dsgGroupAnim = null;
    }
    #endregion

    #region Note Action
    public override void SetUnit(RectTransform[] notePlaceRects, int endRoadIndex, NoteInfo noteInfo)
    {
        _dsgGroupAnim.Show();

        base.SetUnit(notePlaceRects, endRoadIndex, noteInfo);
        //base.SetCaseImgs(true);
    }

    public override void ByeNoteClickOrFlick(List<BaseNoteUnit> noteUnits, Define.NoteJudge judge)
    {
        base.ByeNoteClickOrFlick(noteUnits, judge);
    }

    public override void ByeNoteHold(List<BaseNoteUnit> noteUnits, Define.NoteJudge judge)
    {
        base.ByeNoteHold(noteUnits, judge);
    }

    public override void MoveNote()
    {
        if(curRoadIndex == _dsgLimitIndex)
        {
            _openSfxInstance.start();
            _dsgGroupAnim.Hide();
        }

        base.MoveNote();
    }
    #endregion
}