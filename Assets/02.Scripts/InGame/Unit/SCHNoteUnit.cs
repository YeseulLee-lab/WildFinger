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
using DG.Tweening;

/// <summary>
/// 4박자 홀드 액션 후 로직
/// </summary>
public class SCHNoteUnit : BaseNoteUnit
{
    [Header("------------------ Gimmick Area -----------------")]
    [SerializeField]
    private UIECGroup _schGroupAnim;
    [SerializeField]
    private UIECAnimator _fillingAnim;
    [SerializeField]
    private UIECGroup _upDownAnim;
    [SerializeField]
    private Sprite[] _holdImgs;
    private int _curHoldCnt;
    public int curHoldCnt
    {
        get { return _curHoldCnt; }
        set
        {
            _curHoldCnt = value;
            int index = _curHoldCnt > InGameKey.noteSCHBeatCnt ? InGameKey.noteSCHBeatCnt - 1 : _curHoldCnt;
            base.gimmickImg.sprite = _holdImgs[index];
            base.gimmickImg.rectTransform.sizeDelta = new Vector2(_gimmickSizes[index], _gimmickSizes[index]);
        }
    }
    public bool isCrashed { get; set; } = false;
    private List<BaseNoteUnit> _holdNoteUnits;
    private int[] _gimmickSizes = new int[] { 280, 320, 380, 480, 480};

    [SerializeField]
    private EventReference[] _blowUpSFXs;
    private EventInstance[] _blowUpInstances = new EventInstance[4];

    #region Unity Life Cycle
    public override void Awake()
    {
        base.Awake();

        for (int i = 0; i < _blowUpSFXs.Length; i++)
        {
            _blowUpInstances[i] = RuntimeManager.CreateInstance(_blowUpSFXs[i]);
        }
    }

    public override void Start()
    {
        base.Start();

        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            for (int i = 0; i < _blowUpSFXs.Length; i++)
            {
                _blowUpInstances[i].setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        for (int i = 0; i < _blowUpSFXs.Length; i++)
        {
            _blowUpInstances[i].setUserData(IntPtr.Zero);
            _blowUpInstances[i].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _blowUpInstances[i].release();
        }
    }
    #endregion

    #region Note Action
    public override void SetUnit(RectTransform[] notePlaceRects, int endRoadIndex, NoteInfo noteInfo)
    {
        _schGroupAnim.Show();
        isCrashed = false;
        curHoldCnt = 0;

        base.SetUnit(notePlaceRects, endRoadIndex, noteInfo);

        //일단 홀드로 처리해야해서 홀드로 저장(UI는 반영 X)
        base.holdingBeatRemainCnt = InGameKey.noteSCHBeatCnt;
        base.noteInfo.noteActionType = Define.NoteTypeA.A_HLD;
        _upDownAnim.Hide();
    }

    public override void ByeNoteHold(List<BaseNoteUnit> noteUnits, Define.NoteJudge judge)
    {
        _holdNoteUnits = noteUnits;

        if (!isHoldingFirstHit && judge != Define.NoteJudge.Miss)
        {
            //처음 홀드노트 친 상황
            //DebugX.Log("ByeNote Hold - First");
            _blowUpInstances[curHoldCnt++].start();
            isHoldingFirstHit = true;
            return;
        }

        base.ByeNote(noteUnits, judge);
    }

    public override void MoveNote()
    {
        if (curRoadIndex == 1)
        {
            _upDownAnim.Show();
        }
        else if (curRoadIndex < 1)
        {
            _fillingAnim.OnCustomChannel();
            _blowUpInstances[curHoldCnt++].start();

            if (curHoldCnt > InGameKey.noteSCHBeatCnt-1 && !isCrashed)
            {
                //터짐
                curRoadIndex = 2;
                SetNoteSizeFitInParent(notePlaceRects[curRoadIndex]);
                _holdNoteUnits.Remove(this);
                _schGroupAnim.Hide();
                isCrashed = true;
                base.noteInfo.noteActionType = Define.NoteTypeA.A_CLK;
                BeatGridTracker.Instance.ShakeCam(Define.InGameShakeScale.Small);
            }
        }
        base.MoveNote();
    }
    #endregion
}
