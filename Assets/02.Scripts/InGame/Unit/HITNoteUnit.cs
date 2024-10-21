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
using UnityEngine.Events;
using DG.Tweening;

/// <summary>
/// 연타: 노트 로직에 상관없이 키를 n번 연타하여 제거 가능
/// </summary>
public class HITNoteUnit : BaseNoteUnit
{
    [Header("------------------ FMOD Area -----------------")]
    [SerializeField]
    private EventReference _noteSfx;
    private EventInstance _noteSfxInstance;

    [Header("------------------ Gimmick Area -----------------")]
    [SerializeField]
    private UIECAnimator _hitAnim;
    [SerializeField]
    private UIECGroup _upDownGroupAnim;
    [SerializeField]
    private UnityEngine.UI.Text _beatCntText;
    [SerializeField]
    private UnityEngine.UI.Text _hitCntText;
    [SerializeField]
    private Sprite[] _hitBeatImgs; // 3 2 1
    [SerializeField]
    private Sprite[] _hitNums; // 0~9
    [SerializeField]
    private Image[] _hitNumImgs; //2자리 까지만
    private const int _maxHitLength = 99;
    private int[] _gimmickSizes = new int[] {800, 600, 500};
    private int _curHitCnt;
    public int curHitCnt
    {
        get { return _curHitCnt; }
        set
        {
            _curHitCnt = value;
            _hitCntText.text = _curHitCnt.ToString();
            SetHitUI(_curHitCnt);
        }
    }
    private int _curBeatCnt;
    public int curBeatCnt
    {
        get { return _curBeatCnt; }
        set
        {
            _curBeatCnt = value;
            _beatCntText.text = _curBeatCnt.ToString();
            base.gimmickImg.rectTransform.sizeDelta = new Vector2(_gimmickSizes[2], _gimmickSizes[2]);
            if (_curBeatCnt < 4 && _curBeatCnt > 0)
            {
                base.gimmickImg.sprite = _hitBeatImgs[_curBeatCnt - 1];
                base.gimmickImg.rectTransform.DOSizeDelta(new Vector2(_gimmickSizes[_curBeatCnt - 1], _gimmickSizes[_curBeatCnt - 1]), BeatGridTracker.beatPerSec * 0.5f);
            }
        }
    }

    #region Unity Life Cycle
    public override void Awake()
    {
        base.Awake();

        _noteSfxInstance = RuntimeManager.CreateInstance(_noteSfx);
    }

    public override void Start()
    {
        base.Start();

        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _noteSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        _noteSfxInstance.setUserData(IntPtr.Zero);
        _noteSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _noteSfxInstance.release();

        _hitAnim = null;
        _beatCntText = null;
        _hitCntText = null;
    }
    #endregion

    #region Note Action
    public override void SetUnit(RectTransform[] notePlaceRects, int endRoadIndex, NoteInfo noteInfo)
    {
        _upDownGroupAnim.Hide();
        _hitAnim.GetComponent<Image>().sprite = _hitBeatImgs[2];
        curBeatCnt = noteInfo.gimmickParameters[0];
        curHitCnt = noteInfo.gimmickParameters[1];

        base.SetUnit(notePlaceRects, endRoadIndex, noteInfo);
    }

    public override void ByeNoteClickOrFlick(List<BaseNoteUnit> noteUnits, Define.NoteJudge judge)
    {
        if (judge == Define.NoteJudge.Miss || curHitCnt <= 1)
        {
            BeatGridTracker.Instance.ShakeCam(Define.InGameShakeScale.Large);
            base.ByeNote(noteUnits, judge);
            _noteSfxInstance.start();
            BeatGridTracker.Instance.judgeChecker.judgeBeatTimer.EndTimer();
            return;
        }

        curHitCnt--;
        _hitAnim.OnCustomChannel();
    }

    public override void MoveNote()
    {
        curRoadIndex--;
        base.ShowMoveingAnim();

       if (curRoadIndex == 0)
       {
            //DebugX.Log("판정 노트 타입: " + noteInfo.noteActionType);
            BeatGridTracker.Instance.judgeChecker.judgeBeatTimer.StartTimer(curBeatCnt, BeatGridTracker.beatPerSec);
            BeatGridTracker.Instance.judgeChecker.noteClickUnits.Add(this);
            _upDownGroupAnim.Show();
       }
       //판정 라인 나감
       else if (curRoadIndex < 0)
       {
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
    #endregion

    #region Set Num
    private void SetHitUI(int num)
    {
        if (num < 0)
        {
            return;
        }
        this.gameObject.SetActive(true);

        int tNum = num; // 0부터 시작하도록 보정
        //DebugX.Log("tempCombo: " + tempCombo);
        if (tNum >= _maxHitLength)
        {
            _hitNumImgs[0].gameObject.SetActive(true);
            _hitNumImgs[0].sprite = _hitNums[9];
            _hitNumImgs[1].gameObject.SetActive(true);
            _hitNumImgs[1].sprite = _hitNums[9];
            return;
        }

        // 숫자를 자릿수별로 나누어 배열에 저장
        int[] digits = new int[3];
        for (int i = 0; i < digits.Length; i++)
        {
            digits[i] = tNum % 10;
            tNum /= 10;
            //DebugX.Log($"[Combo] digits[{i}]: {digits[i]}");
        }

        SetHitNumUI(digits[0], _hitNumImgs[0], num > 9); //1의자리
        SetHitNumUI(digits[1], _hitNumImgs[1]); //10의자리
    }

    /// <summary>
    /// 히트 UI Sprite 이미지로 바꿔줌
    /// </summary>
    /// <param name="num">0~9</param>
    /// <param name="isNextExist">자기보다 높은 자리 숫자가 0이 아님</param>
    /// <param name="img"></param>
    private void SetHitNumUI(int num, Image img, bool isNextExist = false)
    {
        if (num < 1 && !isNextExist)
        {
            img.gameObject.SetActive(false);
            return;
        }

        img.gameObject.SetActive(true);
        img.sprite = _hitNums[num];
    }
    #endregion
}