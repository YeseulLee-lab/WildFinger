using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using HHK.UIEC;
using System.Threading;
using Cysharp.Threading.Tasks;

/// <summary>
/// 인질: 어떤 인풋이든 누르면 안됨, 그냥 보내줘야 함(때리면 체력 닳음)
/// </summary>
public class HTGNoteUnit : BaseNoteUnit
{
    [Header("------------------ Gimmick Area -----------------")]
    [SerializeField]
    private UIECAnimator _htgAnim;
    [SerializeField]
    private UIECGroup _htgJudgeGroupAnim;

    #region Unity Life Cycle
    public override void OnDestroy()
    {
        base.OnDestroy();
        _htgAnim = null;
        _htgJudgeGroupAnim = null;
    }
    #endregion

    #region Note Action
    public override void SetUnit(RectTransform[] notePlaceRects, int endRoadIndex, NoteInfo noteInfo)
    {
        _htgJudgeGroupAnim.Hide();
        noteInfo.noteActionType = Define.NoteTypeA.A_CLK;
        base.SetUnit(notePlaceRects, endRoadIndex, noteInfo);
    }

    public override void ByeNoteClickOrFlick(List<BaseNoteUnit> noteUnits, Define.NoteJudge judge)
    {
        _htgAnim.OnCustomChannel();
        ByeNote(noteUnits, judge);
    }

    public override void MoveNote()
    {
        if (notePlaceRects == null)
        {
            DebugX.Log("노트 위치 데이터 동기화 안되어있음. 오류");
        }

        curRoadIndex--;
        ShowMoveingAnim();

        //판정 라인 나감
        if (curRoadIndex < 0)
        {
            BeatGridTracker.Instance.judgeChecker.Perfect(this);
            return;
        }
        else if (curRoadIndex == 0)
        {
            //판정라인 임
            //DebugX.Log("판정 노트 타입: " + noteInfo.noteActionType);
            base.SetJudgeLineVFX();
            _htgJudgeGroupAnim.Show();
            if (noteInfo.noteActionType == Define.NoteTypeA.A_CLK)
            {
                BeatGridTracker.Instance.judgeChecker.noteClickUnits.Add(this);
            }
            else if (noteInfo.noteActionType == Define.NoteTypeA.A_FLK)
            {
                BeatGridTracker.Instance.judgeChecker.noteFlickUnits.Add(this);
            }
        }

        //DebugX.Log("Move Note: " + curRoadIndex);
        SetNoteSizeFitInParent(notePlaceRects[curRoadIndex]);
    }
    #endregion
}