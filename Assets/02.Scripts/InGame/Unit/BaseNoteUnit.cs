using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using HHK.UIEC;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using DG.Tweening;
using FMODUnity;
using FMOD.Studio;

[Serializable]
public class NoteLogicRSPImg
{
    public Sprite[] rspImgs;
}

public class BaseNoteUnit : BaseObjectPoolUnit
{
    [Header("------------------ GUI Areas -----------------")]
    [SerializeField]
    private Image[] _caseImgs;
    [SerializeField]
    private Image[] _actionImgs; // Click, Hold, Flick
    private NoteHoldUI _holdUI;

    [Header("------------------ Animation Areas -----------------")]
    [SerializeField]
    private Image _gimmickImg;
    public Image gimmickImg => _gimmickImg;
    [SerializeField]
    private Sprite[] _flickImgs; // R S P
    [SerializeField]
    private UIECAnimator _judgeAnim;
    [SerializeField]
    private GameObject _judgelineVFX;
    public bool isHolding { get { 
            if(_holdUI == null)
            {
                _holdUI = _actionImgs[1].GetComponent<NoteHoldUI>();
            }
            return _holdUI.isHolding; } set {
            if (_holdUI == null)
            {
                _holdUI = _actionImgs[1].GetComponent<NoteHoldUI>();
            }
            _holdUI.isHolding = value;
        } }
    public bool isHoldingFirstHit { get {
            if (_holdUI == null)
            {
                _holdUI = _actionImgs[1].GetComponent<NoteHoldUI>();
            }
            return _holdUI.isHoldingFirstHit; } set
        {
            if (_holdUI == null)
            {
                _holdUI = _actionImgs[1].GetComponent<NoteHoldUI>();
            }
            _holdUI.isHoldingFirstHit = value;
        } }
    public int holdingBeatRemainCnt { get {
            if (_holdUI == null)
            {
                _holdUI = _actionImgs[1].GetComponent<NoteHoldUI>();
            }
            return _holdUI.holdingBeatRemainCnt; } 
        set
        {
            if (_holdUI == null)
            {
                _holdUI = _actionImgs[1].GetComponent<NoteHoldUI>();
            }
            _holdUI.holdingBeatRemainCnt = value;
            //DebugX.Log("_holdingBeatRemainCnt: " + _holdingBeatRemainCnt);
            //오류 잡아야합니다
            if (_holdUI.holdingBeatRemainCnt < 1)
            {
                //판정 완료!
                BeatGridTracker.Instance.judgeChecker.JudgeHoldingSuccess(this);
            }
        }
    }

    [Header("------------------ FMOD Areas -----------------")]
    [SerializeField]
    private EventReference _perfectSFX;
    private EventInstance _perfectInstance;

    [Header("------------------ Setting Areas -----------------")]
    [SerializeField]
    private NoteLogicRSPImg[] _logicRSPImgs;
    public NoteLogicRSPImg[] logicRSPImgs => _logicRSPImgs;
    private CancellationTokenSource _cts;
    private CancellationToken _ct;
    private const int _byeNoteMS = 405;
    public NoteInfo noteInfo { get; set; }
    public int lineIndex { get; private set; } = 0;
    public int curRoadIndex { get; set; } //Road 의 칸 수에 따라 index 설정, 0이 판정존임
    public RectTransform[] notePlaceRects { get; set; }
    public bool isDuplicateOneRemoved { get; private set; } = false; //더블몬 처리를 위해 강제로
    public int curRemainedBoomCnt { get; private set; } = 1;
    private const float _holdBarForcedSize = -150f;
    private const float _holdBarJudgeForcedSize = 300f;

    #region Unity Life Cycle
    public virtual void Awake()
    {
        _perfectInstance = RuntimeManager.CreateInstance(_perfectSFX);
    }

    public virtual void Start()
    {
        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _perfectInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    public virtual void OnDisable()
    {
        if(_cts != null)
            _cts.Cancel();
    }

    public virtual void OnDestroy()
    {
        _perfectInstance.setUserData(IntPtr.Zero);
        _perfectInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _perfectInstance.release();

        noteInfo = null;
        _caseImgs = null;
        _gimmickImg = null;
        _judgeAnim = null;
        _cts = null;
        notePlaceRects = null;
        _actionImgs = null;
    }
    #endregion

    #region Note Action
    /// <summary>
    ///  최초 세팅, set parent 하여 처리함
    /// </summary>
    /// <param name="notePlaceRects"></param>
    /// <param name="endRoadIndex"></param>
    /// <param name="noteInfo"></param>
    public virtual void SetUnit(RectTransform[] notePlaceRects, int endRoadIndex, NoteInfo noteInfo)
    {
        _judgelineVFX.SetActive(false);
        
        if (_holdUI == null)
        {
            _holdUI = _actionImgs[1].GetComponent<NoteHoldUI>();
        }
        isHoldingFirstHit = false;
        isHolding = false;
        curRemainedBoomCnt = noteInfo.noteDuplicateCnt;
        isDuplicateOneRemoved = false;
        this.notePlaceRects = notePlaceRects;
        this.curRoadIndex = endRoadIndex;
        this.noteInfo = noteInfo;

        //현재 STLogic만 처리함
        _caseImgs[0].gameObject.SetActive(true);
        _caseImgs[0].sprite = _logicRSPImgs[(int)noteInfo.logicTypes[0]].rspImgs[(int)noteInfo.rspTypes[0]];

        if (noteInfo.noteDuplicateCnt == 2)
        {
            _actionImgs[2].sprite = _flickImgs[(int)noteInfo.rspTypes[1]]; //Flick
            _caseImgs[1].gameObject.SetActive(true);
            _caseImgs[1].sprite = _logicRSPImgs[(int)noteInfo.logicTypes[0]].rspImgs[(int)noteInfo.rspTypes[1]];
        }
        else
        {
            _actionImgs[2].sprite = _flickImgs[(int)noteInfo.rspTypes[0]]; //Flick
            _caseImgs[1].gameObject.SetActive(false);
        }

        SetNoteSizeFitInParent(this.notePlaceRects[curRoadIndex]);
        InitActionState(noteInfo);

        BeatGridTracker.Instance.monsterManager.SetMonsterAnim(Define.MonsterAnimType.CastSpell);
        _actionImgs[2].GetComponent<UIECGroup>().Hide();
    }

    /// <summary>
    /// Note Generator에서 쓸래용 힛
    /// </summary>
    /// <param name="noteInfo"></param>
    public void SetUnit(NoteInfo noteInfo)
    {
        _judgelineVFX.SetActive(false);
        
        if (_holdUI == null)
        {
            _holdUI = _actionImgs[1].GetComponent<NoteHoldUI>();
        }
        _holdUI.isHoldingFirstHit = false;
        _holdUI.isHolding = false;
        curRemainedBoomCnt = noteInfo.noteDuplicateCnt;
        isDuplicateOneRemoved = false;
        this.notePlaceRects = notePlaceRects;
        this.curRoadIndex = 0;
        this.noteInfo = noteInfo;

        //현재 STLogic만 처리함
        _caseImgs[0].gameObject.SetActive(true);
        _caseImgs[0].sprite = _logicRSPImgs[(int)noteInfo.logicTypes[0]].rspImgs[(int)noteInfo.rspTypes[0]];

        if (noteInfo.noteDuplicateCnt == 2)
        {
            _actionImgs[2].sprite = _flickImgs[(int)noteInfo.rspTypes[1]]; //Flick
            _caseImgs[1].gameObject.SetActive(true);
            _caseImgs[1].sprite = _logicRSPImgs[(int)noteInfo.logicTypes[0]].rspImgs[(int)noteInfo.rspTypes[1]];
        }
        else
        {
            _actionImgs[2].sprite = _flickImgs[(int)noteInfo.rspTypes[0]]; //Flick
            _caseImgs[1].gameObject.SetActive(false);
        }

        InitActionState(noteInfo);
    }

    public virtual void MoveNote()
    {
        if (noteInfo.noteActionType == Define.NoteTypeA.A_HLD)
        {
            curRoadIndex--;
            ShowMoveingAnim();

            if (_holdUI.isHolding)
            {
                if (curRoadIndex <= 0)
                {
                    //SetHoldingBar(--holdingBeatRemainCnt);
                    return;
                }
            }
            else
            {
                if (curRoadIndex < 0)
                {
                    BeatGridTracker.Instance.judgeChecker.Miss(this);
                    return;
                }
                else if(curRoadIndex == 0)
                {
                    SetJudgeLineVFX();
                    _actionImgs[1].GetComponent<UIECGroup>().Show();
                    BeatGridTracker.Instance.judgeChecker.noteHoldUnits.Add(this);
                }
            }
        }
        else
        {
            curRoadIndex--;
            ShowMoveingAnim();

            //판정 라인 나감
            if (curRoadIndex < 0)
            {
                BeatGridTracker.Instance.judgeChecker.Miss(this);
                return;
            }
            else if (curRoadIndex == 0)
            {
                //판정라인 임
                //DebugX.Log("판정 노트 타입: " + noteInfo.noteActionType);
                SetJudgeLineVFX();
                if (noteInfo.noteActionType == Define.NoteTypeA.A_CLK)
                {
                    BeatGridTracker.Instance.judgeChecker.noteClickUnits.Add(this);
                }
                else if (noteInfo.noteActionType == Define.NoteTypeA.A_FLK)
                {
                    _actionImgs[2].GetComponent<UIECGroup>().Show();
                    BeatGridTracker.Instance.judgeChecker.noteFlickUnits.Add(this);
                }
            }
        }

        //DebugX.Log("Move Note: " + curRoadIndex);
        SetNoteSizeFitInParent(notePlaceRects[curRoadIndex]);
    }

    public void SetJudgeLineVFX()
    {
        _caseImgs[0].transform.parent.GetComponent<UIECAnimator>().OnCustomChannel();
    }

    public virtual void SetNoteSizeFitInParent(RectTransform parentRect)
    {
        if (curRoadIndex == 0)
        {
            parentRect.anchoredPosition3D = new Vector3(parentRect.sizeDelta.x * 0.5f, parentRect.anchoredPosition3D.y, parentRect.anchoredPosition3D.z);
        }

        //TODO: 샤샤샥 이동하는 애니메이션
        this.GetComponent<RectTransform>().DOMove(new Vector3(parentRect.position.x, parentRect.position.y, parentRect.position.z), 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            // 애니메이션 완료 후 부모 설정

            if(curRoadIndex == 0)
            {
                parentRect.anchoredPosition3D = new Vector3(parentRect.sizeDelta.x * 0.5f + parentRect.sizeDelta.x * 0.25f, parentRect.anchoredPosition3D.y, parentRect.anchoredPosition3D.z);
            }

            this.GetComponent<RectTransform>().SetParent(parentRect, false);
            this.GetComponent<RectTransform>().sizeDelta = parentRect.sizeDelta;
            this.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            this.GetComponent<RectTransform>().localScale = Vector3.one;
            this.GetComponent<RectTransform>().localRotation = Quaternion.Euler(-55.5f, 0, 0);
        });
    }

    public virtual void ByeNoteClickOrFlick(List<BaseNoteUnit> noteUnits, Define.NoteJudge judge)
    {
        if (noteInfo.noteDuplicateCnt == 1)
        {
            ByeNote(noteUnits, judge);
            return;
        }

        //더블몬
        if (!isDuplicateOneRemoved)
        {
            curRemainedBoomCnt--;
            //처음 처리
            _caseImgs[0].rectTransform.localScale = Vector3.one;
            _caseImgs[1].gameObject.SetActive(false);
            _actionImgs[2].sprite = _flickImgs[(int)noteInfo.rspTypes[0]];
            isDuplicateOneRemoved = true;

            //뒤로 이동
            curRoadIndex++;
            SetNoteSizeFitInParent(notePlaceRects[curRoadIndex]);
            noteUnits.Remove(this);
            if (judge == Define.NoteJudge.Miss)
            {
                //일단 죽이자
                ByeNote(noteUnits, judge);
            }
        }
        else
        {
            //두 번째 처리
            ByeNote(noteUnits, judge);
        }
    }

    public virtual void ByeNoteHold(List<BaseNoteUnit> noteUnits, Define.NoteJudge judge)
    {
        if (!_holdUI.isHoldingFirstHit && judge != Define.NoteJudge.Miss)
        {
            //처음 홀드노트 친 상황
            //DebugX.Log("ByeNote Hold - First");
            StartHolding(holdingBeatRemainCnt, BeatGridTracker.beatPerSec);
            _holdUI.isHoldingFirstHit = true;
            return;
        }

        ByeNote(noteUnits, judge);
    }

    /// <summary>
    /// 노트 종료, 할 동작이 있으면 byAction에 추가
    /// </summary>
    /// <param name="noteUnits"></param>
    /// <param name="noDelay"></param>
    /// <param name="byeAction">오브젝트 disable하고 나서 나오는 동작</param>
    public async void ByeNote(List<BaseNoteUnit> noteUnits, Define.NoteJudge judge, UnityAction byeAction = null)
    {
        //DebugX.Log("Bye note");
        if(noteInfo.noteActionType == Define.NoteTypeA.A_HLD)
        {
            EndHolding();
        }

        this.gameObject.SetActive(false);
        noteUnits.Remove(this);
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;

        BeatGridTracker.noteRoadManager.noteRoad.ReserveDeletingFromMovingNoteList(this);

        switch (judge)
        {
            case Define.NoteJudge.None:
                break;
            case Define.NoteJudge.Miss:
                ShowEffect(judge);
                break;
            default:
                ShowEffect(judge);
                _perfectInstance.start();
                BeatGridTracker.Instance.monsterManager.SetMonsterAnim(Define.MonsterAnimType.Damaged);
                await UniTask.Delay(_byeNoteMS, cancellationToken: _ct);
                break;
        }

        BaseObjectPool.Instance.ReturnObject(this.gameObject);
        byeAction?.Invoke();
    }
    #endregion

    #region Effects
    public virtual async void ShowMoveingAnim()
    {
        /*
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;

        if (_gimmickImg.sprite == null)
        {
            //기믹이 있을 때에만 애니메이션 바꿔줌
            return;
        }

        for (int i=0; i< _movingImgs.Length; i++)
        {
            if (_isKilled || _isDamaged)
            {
                return;
            }
            _gimmickImg.sprite = _movingImgs[i];
            await UniTask.Delay(_movingNoteMS, cancellationToken: _ct);
        }
        */
    }

    public virtual async void ShowJudgingAnim(Define.NoteJudge judge)
    {
        if(judge == Define.NoteJudge.None)
        {
            return;
        }
        _judgeAnim.OnCustomChannel();
    }

    public virtual void ShowEffect(Define.NoteJudge judge)
    {
        if (judge == Define.NoteJudge.Miss)
        {
            //Miss Effect
            BeatGridTracker.Instance.monsterManager.SetMonsterAnim((Define.MonsterAnimType)UnityEngine.Random.Range((int)Define.MonsterAnimType.Attack1, (int)Define.MonsterAnimType.Attack5 + 1));
        }
        else
        {
            //Success Effect
            BeatGridTracker.Instance.monsterManager.SetGimmickNoteAttackVFX(noteInfo.noteGimmickType);
        }
    }
    #endregion

    #region Action
    private void InitActionState(NoteInfo noteInfo)
    {
        if(noteInfo == null || this.notePlaceRects == null)
        {
            //DebugX.Log("Action State 초기화 불가능");
            return;
        }

        for (int i = 0; i < _actionImgs.Length; i++)
        {
            _actionImgs[i].gameObject.SetActive((Define.NoteTypeA)i == noteInfo.noteActionType);
        }

        if (noteInfo.noteActionType == Define.NoteTypeA.A_HLD)
        {
            _holdUI.InitUI(noteInfo.gimmickParameters[0]);
            _actionImgs[1].GetComponent<UIECGroup>().Hide();
        }else if (noteInfo.noteActionType == Define.NoteTypeA.A_FLK)
        {
            _actionImgs[2].GetComponent<UIECGroup>().Hide();
        }
    }

    public void StartHolding(int curBeatRemain, float beatPerSec)
    {
        BeatGridTracker.Instance.monsterManager.MoveMonsterPos(Define.MonsterPosType.Front, true);
        _holdUI.StartHolding(curBeatRemain, beatPerSec, () =>
            BeatGridTracker.Instance.judgeChecker.Perfect(this));
    }

    public void EndHolding()
    {
        BeatGridTracker.Instance.monsterManager.MoveMonsterPos(Define.MonsterPosType.Default);
        _holdUI.EndHolding();
    }
    #endregion
}