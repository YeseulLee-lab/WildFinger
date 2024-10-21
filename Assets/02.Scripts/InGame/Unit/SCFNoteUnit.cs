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
/// 1회 아무키로 플릭 후 로직
/// 보물상자가 열리는 동작으로 처리
/// </summary>
public class SCFNoteUnit : BaseNoteUnit
{
    [SerializeField]
    private UIECGroup _scfGroupAnim;
    [SerializeField]
    private UIECGroup _upDownAnim;
    public bool isCrashed { get; set; } = false;

    [SerializeField]
    private EventReference _openSFX;
    private EventInstance _openInstance;

    #region Unity Life Cycle
    public override void Awake()
    {
        base.Awake();

        _openInstance = RuntimeManager.CreateInstance(_openSFX);
    }

    public override void Start()
    {
        base.Start();
        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _openInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _scfGroupAnim = null;

        _openInstance.setUserData(IntPtr.Zero);
        _openInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _openInstance.release();
    }
    #endregion

    #region Note Action
    public override void SetUnit(RectTransform[] notePlaceRects, int endRoadIndex, NoteInfo noteInfo)
    {
        _scfGroupAnim.Show();
        isCrashed = false;
        base.SetUnit(notePlaceRects, endRoadIndex, noteInfo);

        //일단 플릭으로 처리해야해서 플릭으로 저장(UI는 반영 X)
        base.noteInfo.noteActionType = Define.NoteTypeA.A_FLK;
        _upDownAnim.Hide();
    }

    /// <summary>
    /// 첫 처리는 플릭이 되어야하기때문에 flick queue에 등록하고 이후에 click queue 로 등록되어야 함
    /// </summary>
    /// <param name="noteUnits"></param>
    /// <param name="noDelay"></param>
    public override void ByeNoteClickOrFlick(List<BaseNoteUnit> noteUnits, Define.NoteJudge judge)
    {
        if (!isCrashed && judge != Define.NoteJudge.Miss)
        {
            //뒤로 이동
            _openInstance.start();
            ++curRoadIndex;
            SetNoteSizeFitInParent(notePlaceRects[curRoadIndex]);
            noteUnits.Remove(this);

            _upDownAnim.Hide();
            _scfGroupAnim.Hide();
            isCrashed = true;

            //TODO: 클릭 queue에 등록?
            base.noteInfo.noteActionType = Define.NoteTypeA.A_CLK;
            return;
        }

        base.ByeNoteClickOrFlick(noteUnits, judge);
    }

    public override void MoveNote()
    {
        if (curRoadIndex == 1)
        {
            _upDownAnim.Show();
        }

        base.MoveNote();
    }

    public override async void ShowJudgingAnim(Define.NoteJudge judge)
    {
        if (judge == Define.NoteJudge.None)
        {
            return;
        }
    }
    #endregion
}
