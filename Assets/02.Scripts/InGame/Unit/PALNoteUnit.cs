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
/// 단짝몬, 비겨몬으로 처리됨, 없는 1개(inputIndexes[0]) 노트 누르기 
/// </summary>
public class PALNoteUnit : BaseNoteUnit
{
    [Header("------------------ Gimmick Area -----------------")]
    [SerializeField]
    private Image[] _palImgs; // Left Right
    [SerializeField]
    private UIECGroup _upDownAnim;

    #region Unity Life Cycle
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    #endregion

    #region Note Action
    public override void SetUnit(RectTransform[] notePlaceRects, int endRoadIndex, NoteInfo noteInfo)
    {
        base.SetUnit(notePlaceRects, endRoadIndex, noteInfo);

        _palImgs[0].sprite = logicRSPImgs[(int)Define.LogicType.Draw].rspImgs[(int)noteInfo.rspTypes[0]];
        _palImgs[1].sprite = logicRSPImgs[(int)Define.LogicType.Draw].rspImgs[(int)noteInfo.rspTypes[1]];
        _upDownAnim.Hide();
    }

    public override void MoveNote()
    {
        if (curRoadIndex == 1)
        {
            _upDownAnim.Show();
        }

        base.MoveNote();
    }
    #endregion
}
