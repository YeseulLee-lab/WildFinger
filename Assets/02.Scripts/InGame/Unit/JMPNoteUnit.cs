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
/// 점프: 길을 왔다갔다 함
/// </summary>
public class JMPNoteUnit : BaseNoteUnit
{
    [Header("------------------ Gimmick Area -----------------")]
    private int _rootLineIndex = 0;
    private RectTransform[] _jumpingNotePlaceRects1 { get; set; }
    private RectTransform[] _jumpingNotePlaceRects2 { get; set; }
    [SerializeField]
    private EventReference _jumpSFX;
    private EventInstance _jumpInstance;

    #region Unity Life Cycle
    public override void Awake()
    {
        base.Awake();

        _jumpInstance = RuntimeManager.CreateInstance(_jumpSFX);
    }

    public override void Start()
    {
        base.Start();

        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _jumpInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        _jumpInstance.setUserData(IntPtr.Zero);
        _jumpInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _jumpInstance.release();

        _jumpingNotePlaceRects1 = null;
        _jumpingNotePlaceRects2 = null;
    }
    #endregion

    #region Note Action
    public override void SetUnit(RectTransform[] notePlaceRects, int endRoadIndex, NoteInfo noteInfo)
    {
        _rootLineIndex = GetChildIndex(notePlaceRects[1].parent.GetComponent<RectTransform>()); //0번은 judge임
        _jumpingNotePlaceRects1 = new RectTransform[notePlaceRects.Length];
        _jumpingNotePlaceRects2 = new RectTransform[notePlaceRects.Length];
        Array.Copy(notePlaceRects, _jumpingNotePlaceRects1, notePlaceRects.Length);
        Array.Copy(notePlaceRects, _jumpingNotePlaceRects2, notePlaceRects.Length);

        if (TownDataLoader.curMusicInfo.lineCnt != 1)
        {
            int length;
            switch (_rootLineIndex)
            {
                default:
                case 0:
                case 2:
                    length = notePlaceRects[1].parent.parent.GetChild(1).childCount;
                    for (int i = 1; i < length; i++)
                    {
                        _jumpingNotePlaceRects2[i] = notePlaceRects[1].parent.parent.GetChild(1).GetChild(length - i).GetComponent<RectTransform>();
                    }
                    break;
                case 1:
                    length = notePlaceRects[1].parent.parent.GetChild(2).childCount;
                    for (int i = 1; i < notePlaceRects[1].parent.parent.GetChild(2).childCount; i++)
                    {
                        _jumpingNotePlaceRects2[i] = notePlaceRects[1].parent.parent.GetChild(0).GetChild(length - i).GetComponent<RectTransform>();
                    }
                    break;
            }
        }

        base.SetUnit(notePlaceRects, endRoadIndex, noteInfo);
    }

    public override void SetNoteSizeFitInParent(RectTransform parentRect)
    {
        if(curRoadIndex != 0)
        {
            if (curRoadIndex % 2 == 0)
            {
                parentRect = _jumpingNotePlaceRects1[curRoadIndex];
            }
            else
            {
                parentRect = _jumpingNotePlaceRects2[curRoadIndex];
            }
        }
        else
        {
            _jumpInstance.start();
        }

       base.SetNoteSizeFitInParent(parentRect);
    }

    private int GetChildIndex(RectTransform child)
    {
        int childCount = child.parent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            if (child.parent.GetChild(i) == child)
            {
                return i;
            }
        }

        return 0;
    }
    #endregion
}