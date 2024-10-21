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
using UnityEngine.UI.Extensions;

/// <summary>
/// gimmickParameters[0]박자 동안 빠르게 4개의 노트 누르기
/// rspTypes[0], rspTypes[1], logicTypes[0], logicTypes[1]
/// </summary>
public class PCKNoteUnit : BaseNoteUnit
{
    [SerializeField]
    private Image[] _sequenceImgs;
    [SerializeField]
    private GameObject[] _sequenceArrows;
    [SerializeField]
    private UnityEngine.UI.Text _beatCntText;
    [SerializeField]
    private UIECGroup _upDownAnim;
    [SerializeField]
    private Sprite[] _rspGrayImgs; //Rock Scissor Paper
    private int[] _correctInputs;
   private int _curHitCnt;
    public int curHitCnt
    {
        get { return _curHitCnt; }
        set
        {
            _sequenceArrows[_curHitCnt].SetActive(false); //이전 Arrow 비활성화
            _curHitCnt = value;
            int arrowIndex = _curHitCnt >= InGameKey.notePCKHitCnt ? InGameKey.notePCKHitCnt - 1 : _curHitCnt;
            _sequenceArrows[arrowIndex].SetActive(true); //현재 Arrow 활성화
            _sequenceArrows[arrowIndex].GetComponent<UIECAnimator>().OnCustomChannel();
        }
    }
    private int _curBeatCnt;
    public int curBeatCnt //줄어듬
    {
        get { return _curBeatCnt; }
        set
        {
            _curBeatCnt = value;
            _beatCntText.text = _curBeatCnt.ToString();
        }
    }
    [SerializeField]
    private EventReference _scanOnSFX;
    private EventInstance _scanOnInstance;
    [SerializeField]
    private EventReference _hitSFX;
    private EventInstance _hitInstance;

    #region Unity Life Cycle
    public override void Awake()
    {
        base.Awake();

        _scanOnInstance = RuntimeManager.CreateInstance(_scanOnSFX);
        _hitInstance = RuntimeManager.CreateInstance(_hitSFX);
    }

    public override void Start()
    {
        base.Start();
        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _scanOnInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _hitInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        _correctInputs = null;

        _scanOnInstance.setUserData(IntPtr.Zero);
        _scanOnInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _scanOnInstance.release();

        _hitInstance.setUserData(IntPtr.Zero);
        _hitInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _hitInstance.release();

    }
    #endregion

    #region Note Action
    public override void SetUnit(RectTransform[] notePlaceRects, int endRoadIndex, NoteInfo noteInfo)
    {
        _curHitCnt = 0;
        curBeatCnt = noteInfo.gimmickParameters[0];
        _correctInputs = new int[4] { (int)noteInfo.rspTypes[0], (int)noteInfo.rspTypes[1], (int)noteInfo.logicTypes[0], (int)noteInfo.logicTypes[1] };

        for(int i =0; i< _sequenceImgs.Length; i++)
        {
            _sequenceImgs[i].sprite = logicRSPImgs[(int)Define.LogicType.Draw].rspImgs[_correctInputs[i]];
            _sequenceArrows[i].SetActive(false);
        }
        base.SetUnit(notePlaceRects, endRoadIndex, noteInfo);
        _upDownAnim.Hide();
    }

    public override void MoveNote()
    {
        curRoadIndex--;
        base.ShowMoveingAnim();

        if (curRoadIndex == 0)
        {
            //DebugX.Log("판정 노트 타입: " + noteInfo.noteActionType);
            _scanOnInstance.start();
            curHitCnt = 0;
            BeatGridTracker.Instance.judgeChecker.noteClickUnits.Add(this);
            _upDownAnim.Show();
            BeatGridTracker.Instance.judgeChecker.judgeBeatTimer.StartTimer(curBeatCnt, BeatGridTracker.beatPerSec);
        }
        else if (curRoadIndex < 0)
        {
            //판정 라인 나감
            curBeatCnt--;
            if (curBeatCnt < 1)
            {
                BeatGridTracker.Instance.judgeChecker.Miss(this);
                BeatGridTracker.Instance.judgeChecker.judgeBeatTimer.EndTimer();
            }
            return;
        }

        base.SetNoteSizeFitInParent(notePlaceRects[curRoadIndex]);
    }

    public override void ByeNoteClickOrFlick(List<BaseNoteUnit> noteUnits, Define.NoteJudge judge)
    {
        SetHitAnim(curHitCnt++);

        if (judge == Define.NoteJudge.Miss || curHitCnt >= InGameKey.noteBRDHitCnt)
        {
            BeatGridTracker.Instance.ShakeCam(Define.InGameShakeScale.Midium);
            base.ByeNote(noteUnits, judge);
            BeatGridTracker.Instance.judgeChecker.judgeBeatTimer.EndTimer();
            return;
        }

        _hitInstance.start();
    }

    private void SetHitAnim(int index)
    {
        if(index > _sequenceImgs.Length - 1)
        {
            return;
        }

        _sequenceImgs[index].GetComponent<UIECAnimator>().OnCustomChannel();
        _sequenceImgs[index].sprite = _rspGrayImgs[_correctInputs[index]]; //g
    }

    public int GetCurInputType()
    {
        return _correctInputs[curHitCnt >= InGameKey.notePCKHitCnt? InGameKey.notePCKHitCnt -1 : curHitCnt];
    }
    #endregion
}
