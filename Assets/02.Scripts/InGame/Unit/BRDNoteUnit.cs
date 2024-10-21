using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HHK.UIEC;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using FMOD.Studio;
using DG.Tweening;

[Serializable]
public class BRDHitFrame
{
    public Sprite[] hitBeatImgs = new Sprite[5];
}

/// <summary>
/// 해당 버튼 gimmickParameters[0]박자 동안 해당 버튼만 4번 눌러서 연타폭탄, 밀가루반죽에서 가위바위보 모양이 됨
/// 중간에 다른 노트 누르면 good 처리
/// </summary>
public class BRDNoteUnit : BaseNoteUnit
{
    [Header("------------------ Gimmick Area -----------------")]
    [SerializeField]
    private BRDHitFrame[] _hitRSPImgs; 
    [SerializeField] 
    private Sprite[] _crushImgs; // Rock Scissor Paper
    [SerializeField]
    private UIECAnimator _hitAnim;
    [SerializeField]
    private UIECAnimator _crushAnim;
    [SerializeField]
    private UnityEngine.UI.Text _beatCntText;
    [SerializeField]
    private UIECGroup _upDownGroupAnim;
    private int[] _gimmickSizes = new int[] { 380, 480, 580, 680, 900};
    private int _curHitCnt;
    public int curHitCnt
    {
        get { return _curHitCnt; }
        set
        {
            _curHitCnt = value;
            if(_curHitCnt > InGameKey.noteBRDHitCnt)
            {
                _curHitCnt = InGameKey.noteBRDHitCnt - 1;
            }
            _hitAnim.GetComponent<Image>().sprite = _hitRSPImgs[(int)noteInfo.rspTypes[0]].hitBeatImgs[_curHitCnt];
            base.gimmickImg.rectTransform.DOSizeDelta(new Vector2(_gimmickSizes[_curHitCnt], _gimmickSizes[_curHitCnt]), BeatGridTracker.beatPerSec * 0.5f);
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
    private EventReference[] _eggSFXs;
    private EventInstance[] _eggInstances = new EventInstance[3];

    #region Unity Life Cycle
    public override void Awake()
    {
        base.Awake();
        
        for(int i=0; i< _eggSFXs.Length; i++)
        {
            _eggInstances[i] = RuntimeManager.CreateInstance(_eggSFXs[i]);
        }
    }

    public override void Start()
    {
        base.Start();

        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            for (int i = 0; i < _eggSFXs.Length; i++)
            {
                _eggInstances[i].setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        for (int i = 0; i < _eggSFXs.Length; i++)
        {
            _eggInstances[i].setUserData(IntPtr.Zero);
            _eggInstances[i].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _eggInstances[i].release();
        }
    }
    #endregion

    #region Note Action
    public override void SetUnit(RectTransform[] notePlaceRects, int endRoadIndex, NoteInfo noteInfo)
    {
        _upDownGroupAnim.Hide();
        _crushAnim.GetComponent<Image>().sprite = _crushImgs[(int)noteInfo.rspTypes[0]];
        _crushAnim.GetComponent<CanvasGroup>().alpha = 0f;
        curBeatCnt = noteInfo.gimmickParameters[0];
        base.SetUnit(notePlaceRects, endRoadIndex, noteInfo);
        curHitCnt = 0;
    }

    public override void MoveNote()
    {
        curRoadIndex--;
        base.ShowMoveingAnim();

        if (curRoadIndex == 0)
        {
            //DebugX.Log("판정 노트 타입: " + noteInfo.noteActionType);
            BeatGridTracker.Instance.judgeChecker.noteClickUnits.Add(this);
            BeatGridTracker.Instance.judgeChecker.judgeBeatTimer.StartTimer(curBeatCnt, BeatGridTracker.beatPerSec);
            _upDownGroupAnim.Show();
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
        curHitCnt++;
        _hitAnim.OnCustomChannel();

        if (judge == Define.NoteJudge.Miss || curHitCnt >= InGameKey.noteBRDHitCnt)
        {
            BeatGridTracker.Instance.ShakeCam(Define.InGameShakeScale.Small);
            _crushAnim.OnCustomChannel();
            base.ByeNote(noteUnits, judge);
            BeatGridTracker.Instance.judgeChecker.judgeBeatTimer.EndTimer();
            return;
        }

        _eggInstances[UnityEngine.Random.Range(0, _eggInstances.Length)].start();
    }
    #endregion
}