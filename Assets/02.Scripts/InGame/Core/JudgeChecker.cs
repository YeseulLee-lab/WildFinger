using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

public class JudgeChecker : MonoBehaviour
{
    [Header("------------------ Others -----------------")]
    [SerializeField]
    private JudgeUIManager _judgeManager;
    [SerializeField]
    private ComboUIManager _comboManager;
    public ComboUIManager comboManager => _comboManager;
    [SerializeField]
    private HPUIManager _hpManager;
    [SerializeField]
    private GameRetry _retryManager;
    [SerializeField]
    private InGameItemManager _itemManager;
    public InGameItemManager itemManager => _itemManager;
    public HPUIManager hpManager => _hpManager;
    [SerializeField]
    private JudgeBeatTimer _judgeBeatTimer;
    public JudgeBeatTimer judgeBeatTimer => _judgeBeatTimer;

    [Header("------------------ JudgeLine Field -----------------")]
    private CancellationTokenSource _cts;
    private CancellationToken _ct;

    [Header("------------------ Score Field -----------------")]
    private int _curCombo = 0;
    public int curCombo { get{ return _curCombo; } set {
            _curCombo = value;
            _comboManager.curCombo = _curCombo;
            if (_curCombo > maxCombo)
            {
                maxCombo = _curCombo;
            }

            if(_curCombo != 0 && _curCombo % 10 == 0)
            {
                _hpManager.ChangeHP(BeatGridTracker.Instance.inGameHealingHP);
            }
        } }
    public int maxCombo { get; private set; }
    public int[] judgeCnts { get; private set; } // 순서: Define.NoteJudge
    public List<BaseNoteUnit> noteClickUnits { get; set; }
    public List<BaseNoteUnit> noteFlickUnits { get; set; }
    public List<BaseNoteUnit> noteHoldUnits { get; set; }
    private bool _isHitVFX = false;

    #region Unity Life Cycle
    private void OnDestroy()
    {
        _judgeManager = null;
        _comboManager = null;
        _hpManager = null;
        _retryManager = null;
        _itemManager = null;
        noteClickUnits = null;
        noteFlickUnits = null;
        noteHoldUnits = null;
    }
    #endregion

    #region Judge
    public void JudgeNoteClick(int curInputType)
    {
        if (noteClickUnits.Count < 0)
        {
            DebugX.Log("[Check] 클릭 판정할 노트 없음");
            return;
        }

        int noteUnitCnt = noteClickUnits.Count;

        for (int i=0; i< noteUnitCnt; i++)
        {
            BaseNoteUnit noteUnit = noteClickUnits[i];

            switch (noteUnit.noteInfo.noteGimmickType)
            {
                case Define.NoteTypeN.N_HIT:
                case Define.NoteTypeN.N_ILU:
                    ShowJudgeEffect(Define.NoteJudge.Perfect, noteUnit);
                    break;
                case Define.NoteTypeN.N_HTG:
                    //데미지
                    ShowJudgeEffect(Define.NoteJudge.Miss, noteClickUnits[i]);
                    break;
                case Define.NoteTypeN.N_SCT:
                    if (!noteUnit.GetComponent<SCTNoteUnit>().isCrashed)
                    {
                        ShowJudgeEffect(Define.NoteJudge.Perfect, noteClickUnits[i]);
                    }
                    else
                    {
                        CheckDefaultNote(noteUnit, curInputType);
                    }
                    break;
                case Define.NoteTypeN.N_PCK:
                    ShowJudgeEffect((noteUnit.GetComponent<PCKNoteUnit>().GetCurInputType() == curInputType) ? Define.NoteJudge.Perfect : Define.NoteJudge.Good, noteUnit);
                    break;
                default:
                    CheckDefaultNote(noteUnit, curInputType);
                    break;
            }
        }
    }

    public void JudgeNoteFlick(int curInputType)
    {
        if (noteFlickUnits.Count < 0)
        {
            DebugX.Log("[Check] 플릭 판정할 노트 없음");
            return;
        }

        int noteUnitCnt = noteFlickUnits.Count;
        for (int i = 0; i < noteUnitCnt; i++)
        {
            BaseNoteUnit noteUnit = noteFlickUnits[i];

            switch (noteUnit.noteInfo.noteGimmickType)
            {
                case Define.NoteTypeN.N_SCF:
                    if (!noteUnit.GetComponent<SCFNoteUnit>().isCrashed)
                    {
                        ShowJudgeEffect(Define.NoteJudge.Perfect, noteFlickUnits[i]);
                    }
                    else
                    {
                        //해당 경우의 수는 존재하지 않음, 발생 시 예외처리해야 함
                        DebugX.Log("N_SCF 판정 오류 발생");
                        CheckDefaultNote(noteUnit, curInputType);
                    }
                    break;
                default:
                    CheckDefaultNote(noteUnit, curInputType);
                    break;
            }
        }
    }

    private void CheckDefaultNote(BaseNoteUnit noteUnit, int curInputType)
    {
        if (noteUnit.noteInfo.noteDuplicateCnt == 1)
        {
            ShowJudgeEffect((noteUnit.noteInfo.inputIndexes[0] == curInputType) ? Define.NoteJudge.Perfect : Define.NoteJudge.Good, noteUnit);
            return;
        }

        //더블몬
        if (!noteUnit.isDuplicateOneRemoved)
        {
            //첫 번째
            ShowJudgeEffect((noteUnit.noteInfo.inputIndexes[1] == curInputType) ? Define.NoteJudge.Perfect : Define.NoteJudge.Good, noteUnit);
        }
        else
        {
            //두 번째
            ShowJudgeEffect((noteUnit.noteInfo.inputIndexes[0] == curInputType) ? Define.NoteJudge.Perfect : Define.NoteJudge.Good, noteUnit);
        }
    }

    /// <summary>
    /// 홀드노트인지 확인하고 등록(홀드 시작), 판정은 하지 않음(끝날 때 판정함)
    /// </summary>
    /// <param name="curInputType"></param>
    public void JudgeNoteHoldStarting(int curInputType)
    {
        if (noteHoldUnits.Count < 0)
        {
            DebugX.Log("[Check] 홀드 판정할 노트 없음");
            return;
        }

        int noteUnitCnt = noteHoldUnits.Count;
        for (int i = 0; i < noteUnitCnt; i++)
        {
            BaseNoteUnit noteUnit = noteHoldUnits[i];
            noteUnit.isHolding = true;

            switch (noteUnit.noteInfo.noteGimmickType)
            {
                case Define.NoteTypeN.N_SCH:
                    if (!noteUnit.GetComponent<SCHNoteUnit>().isCrashed)
                    {
                        ShowJudgeEffect(Define.NoteJudge.Perfect, noteHoldUnits[i]);
                    }
                    else
                    {
                        //해당 경우의 수는 존재하지 않음, 발생 시 예외처리해야 함
                        DebugX.Log("N_SCH 판정 오류 발생");
                        ShowJudgeEffect(Define.NoteJudge.Miss, noteHoldUnits[i]);
                    }
                    break;
                default:
                    ShowJudgeEffect((noteUnit.noteInfo.inputIndexes[0] == curInputType) ? Define.NoteJudge.Perfect : Define.NoteJudge.Good, noteUnit);
                    break;
            }
        }
    }

    /// <summary>
    /// 누르고 있다가 뗐을 때 호출, 아직 판정할 노트가 남아있으면 Good처리, 아니면 그냥 넘어감(떼는 타이밍엔 판정이 없음)
    /// </summary>
    /// <param name="curInputType"></param>
    public void JudgeNoteHoldEnding(int curInputType)
    {
        if (noteHoldUnits.Count < 0)
        {
            DebugX.Log("[Check] 홀드 판정할 노트 없음");
            return;
        }

        int noteUnitCnt = noteHoldUnits.Count;
        for (int i = 0; i < noteUnitCnt; i++)
        {
            BaseNoteUnit noteUnit = noteHoldUnits[i];
            ShowJudgeEffect(Define.NoteJudge.Good, noteHoldUnits[i]);
        }
    }

    public void ShowJudgeEffect(Define.NoteJudge judge, BaseNoteUnit noteUnit)
    {
        if (judge == Define.NoteJudge.None)
        {
            DebugX.Log("[Judge] timing none");
            curCombo = 0;
            return;
        }

        if (BeatGridTracker.Instance.feverManager.isFever)
        {
            DebugX.Log("FeverMode 라서 안띄움");
            judge = Define.NoteJudge.None;
        }

        _judgeManager.gameObject.SetActive(true);
        if(judge != Define.NoteJudge.None)
        {
            judgeCnts[(int)judge]++;
        }
        else
        {
            //Perfect을 올려야하나?
        }
        BeatGridTracker.Instance.monsterManager.SetMonsterDamaged(judge);
        noteUnit.ShowJudgingAnim(judge);
        bool isProtected = false;
        switch (judge)
        {
            case Define.NoteJudge.Perfect:
                curCombo++;
                isProtected = false;
                break;
            case Define.NoteJudge.Good:
                if (_itemManager.shieldCnt > 0)
                {
                    curCombo++;
                    _itemManager.shieldCnt--;
                    isProtected = true;
                }
                else
                {
                    isProtected = false;
                    curCombo = 0;
                    _hpManager.ChangeHP(InGameKey.noteXPoint);
                }
                break;
            case Define.NoteJudge.Miss:
                if (_itemManager.shieldCnt > 0)
                {
                    isProtected = true;
                    curCombo++;
                    _itemManager.shieldCnt--;
                }
                else
                {
                    isProtected = false;
                    curCombo = 0;
                    _hpManager.ChangeHP(InGameKey.noteMissPoint);
                }
                break;
        }

        SetJudgeLineVFX(judge, isProtected);
        _judgeManager.SetJudgeUI(isProtected? Define.NoteJudge.Protected: judge, noteUnit.curRemainedBoomCnt);

        switch (noteUnit.noteInfo.noteActionType)
        {
            default:
            case Define.NoteTypeA.A_CLK:
                noteUnit.ByeNoteClickOrFlick(noteClickUnits, judge);
                break;
            case Define.NoteTypeA.A_HLD:
                noteUnit.ByeNoteHold(noteHoldUnits, judge);
                break;
            case Define.NoteTypeA.A_FLK:
                noteUnit.ByeNoteClickOrFlick(noteFlickUnits, judge);
                break;
        }

        if (_hpManager.hp <= 0)
        {
            if (!TownDataLoader.isTraining)
            {
                ShowRetry();
            }
            else
            {
                //애초 훈련도감은 체력이 안깎여서 올 수 없는 경우이지만, 예외처리
                DebugX.LogError("훈련도감에서 체력이 깎이는 상황(에러)");
                BeatGridTracker.ShowResult();
            }
        }

        if(GamePlayData.Instance != null)
        {
            GamePlayData.Instance.mobileVibrater.Vibrate();
        }
    }

    public async UniTask Init()
    {
        noteClickUnits = new List<BaseNoteUnit>();
        noteFlickUnits = new List<BaseNoteUnit>();
        noteHoldUnits = new List<BaseNoteUnit>();
        curCombo = 0;
        _judgeManager.SetJudgeUI();
        //_hpManager.SetHP(BeatGridTracker.Instance.inGameMaxHP);
        judgeCnts = new int[Enum.GetNames(typeof(Define.NoteJudge)).Length - 1]; //None 은 안 셈
        for (int i =0; i< judgeCnts.Length; i++)
        {
            judgeCnts[i] = 0;
        }
        await _itemManager.ShowInitItemAnim();
    }

    public void ShowRetry()
    {
        _retryManager.ShowPopup();
        BeatGridTracker.SetGameState(Define.InGameState.End);
    }
    #endregion

    #region Judge - Special Case
    /// <summary>
    /// 노트 판정선에서 그냥 지나감(안누름)
    /// </summary>
    /// <param name="noteUnit"></param>
    public void Miss(BaseNoteUnit noteUnit)
    {
        //DebugX.Log("[Check] noteUnit 삭제: " + noteUnit.noteInfo.inputType);
        for (int i = 0; i < noteClickUnits.Count; i++)
        {
            if (noteClickUnits[i].Equals(noteUnit))
            {
                ShowJudgeEffect(Define.NoteJudge.Miss, noteClickUnits[i]);
                return;
            }
        }

        for (int i = 0; i < noteFlickUnits.Count; i++)
        {
            if (noteFlickUnits[i].Equals(noteUnit))
            {
                ShowJudgeEffect(Define.NoteJudge.Miss, noteFlickUnits[i]);
                return;
            }
        }

        for (int i = 0; i < noteHoldUnits.Count; i++)
        {
            if (noteHoldUnits[i].Equals(noteUnit))
            {
                ShowJudgeEffect(Define.NoteJudge.Miss, noteHoldUnits[i]);
                return;
            }
        }
    }

    public void Perfect(BaseNoteUnit noteUnit)
    {
        //DebugX.Log("[Check] noteUnit 삭제: " + noteUnit.noteInfo.inputType);
        for (int i = 0; i < noteClickUnits.Count; i++)
        {
            if (noteClickUnits[i].Equals(noteUnit))
            {
                ShowJudgeEffect(Define.NoteJudge.Perfect, noteClickUnits[i]);
                return;
            }
        }

        for (int i = 0; i < noteFlickUnits.Count; i++)
        {
            if (noteFlickUnits[i].Equals(noteUnit))
            {
                ShowJudgeEffect(Define.NoteJudge.Perfect, noteFlickUnits[i]);
                return;
            }
        }

        for (int i = 0; i < noteHoldUnits.Count; i++)
        {
            if (noteHoldUnits[i].Equals(noteUnit))
            {
                ShowJudgeEffect(Define.NoteJudge.Perfect, noteHoldUnits[i]);
                return;
            }
        }
    }

    /// <summary>
    /// 홀드 성공 => Perfect
    /// </summary>
    /// <param name="noteUnit"></param>
    public void JudgeHoldingSuccess(BaseNoteUnit noteUnit)
    {
        for (int i = 0; i < noteHoldUnits.Count; i++)
        {
            if (noteHoldUnits[i].Equals(noteUnit))
            {
                ShowJudgeEffect(Define.NoteJudge.Perfect, noteHoldUnits[i]);
                return;
            }
        }
    }
    #endregion

    #region Hit VFX
    private async void SetJudgeLineVFX(Define.NoteJudge judge, bool isProtected)
    {
        // 기존의 CancellationTokenSource를 취소하고 새로 생성
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var _ct = _cts.Token;

        if (isProtected)
        {
            judge = Define.NoteJudge.Protected;
        }

        if (judge != Define.NoteJudge.None)
        {
            _isHitVFX = true;
            JsonNoteLoader.gameRuleUIManager.SelectLine(judge, isProtected);
            try
            {
                await UniTask.Delay(InGameKey.judgeEffectMS, cancellationToken: _ct);
                _isHitVFX = false;
            }
            catch (OperationCanceledException)
            {
                // 이전 작업이 취소되면 예외 발생, 무시
                _isHitVFX = true;
            }
        }

        if (!_isHitVFX)
        {
            JsonNoteLoader.gameRuleUIManager.SelectLine(); // Default
        }
    }
    #endregion
}
