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
/// 아무 노트나 눌러야 함, 동작 불가능
/// </summary>
public class ILUNoteUnit : BaseNoteUnit
{
    [SerializeField]
    private UIECGroup _upDownAnim;

    #region Note Action
    public override void SetUnit(RectTransform[] notePlaceRects, int endRoadIndex, NoteInfo noteInfo)
    {
        base.SetUnit(notePlaceRects, endRoadIndex, noteInfo);

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
