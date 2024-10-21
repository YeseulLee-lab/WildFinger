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
/// 시크릿(실드): 해당 박자 때 2번 내야함, 1번에는 아무거나, 2번째는 로직
/// </summary>
public class SCTNoteUnit : BaseNoteUnit
{
    [Header("------------------ FMOD Area -----------------")]
    [SerializeField]
    private EventReference _noteSfx;
    private EventInstance _noteSfxInstance;

    [Header("------------------ Gimmick Area -----------------")]
    [SerializeField]
    private UIECGroup _sctGroupAnim;
    [SerializeField]
    private UIECGroup _upDownAnim;
    [SerializeField]
    private UIECGroup _upDownAnim2; //뭔가 이상함
    public bool isCrashed { get; set; } = false;

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

        _sctGroupAnim = null;
    }
    #endregion

    #region Note Action
    public override void SetUnit(RectTransform[] notePlaceRects, int endRoadIndex, NoteInfo noteInfo)
    {
        _sctGroupAnim.Show();
        _upDownAnim2.Hide();
        isCrashed = false;
        noteInfo.noteActionType = Define.NoteTypeA.A_CLK;
        base.SetUnit(notePlaceRects, endRoadIndex, noteInfo);
        _upDownAnim.Hide();
    }

    public override void ByeNoteClickOrFlick(List<BaseNoteUnit> noteUnits, Define.NoteJudge judge)
    {
        if (!isCrashed && judge != Define.NoteJudge.Miss)
        {
            //뒤로 이동
            ++curRoadIndex;
            SetNoteSizeFitInParent(notePlaceRects[curRoadIndex]);
            noteUnits.Remove(this);

            _upDownAnim2.Hide();
            _sctGroupAnim.Hide();
            _noteSfxInstance.start();
            isCrashed = true;
            return;
        }
        base.ByeNoteClickOrFlick(noteUnits, judge);
    }

    public override void MoveNote()
    {
        if (curRoadIndex == 1)
        {
            if (isCrashed)
            {
                _upDownAnim2.Show();
            }
            else
            {
                _upDownAnim.Show();
            }
        }

        base.MoveNote();
    }
    #endregion
}