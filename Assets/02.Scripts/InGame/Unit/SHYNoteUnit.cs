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
/// 샤이: 2박자에 1번 씩 본인이 가진 로직을 보여줌
/// </summary>
public class SHYNoteUnit : BaseNoteUnit
{
    [Header("------------------ FMOD Area -----------------")]
    [SerializeField]
    private EventReference _noteSfx;
    private EventInstance _noteSfxInstance;

    [Header("------------------ Gimmick Area -----------------")]
    [SerializeField]
    private UIECGroup _shyGroupAnim;
    [SerializeField]
    private UIECGroup _shellOpenGroupAnim;

    #region Unity Life Cycle
    public override void Awake()
    {
        base.Awake();

        _noteSfxInstance = RuntimeManager.CreateInstance(_noteSfx);
    }

    public override void Start()
    {
        base.Start();

        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _noteSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _noteSfxInstance.setUserData(IntPtr.Zero);
        _noteSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _noteSfxInstance.release();

        _shyGroupAnim = null;
        _shellOpenGroupAnim = null;
    }
    #endregion

    #region Note Action
    public override void SetUnit(RectTransform[] notePlaceRects, int endRoadIndex, NoteInfo noteInfo)
    {
        _shellOpenGroupAnim.Show();
        base.SetUnit(notePlaceRects, endRoadIndex, noteInfo);
    }

    public override void MoveNote()
    {
        if(curRoadIndex > 0)
        {
            if (curRoadIndex % 2 == 0)
            {
                _shyGroupAnim.Show();
            }
            else
            {
                _shyGroupAnim.Hide();
            }
        }

        if(curRoadIndex == 1)
        {
            _noteSfxInstance.start();
            _shellOpenGroupAnim.Hide();
        }

        base.MoveNote();
    }

    public override void ByeNoteClickOrFlick(List<BaseNoteUnit> noteUnits, Define.NoteJudge judge)
    {
        base.ByeNoteClickOrFlick(noteUnits, judge);
    }

    public override void ByeNoteHold(List<BaseNoteUnit> noteUnits, Define.NoteJudge judge)
    {
        base.ByeNoteHold(noteUnits, judge);
    }
    #endregion
}
