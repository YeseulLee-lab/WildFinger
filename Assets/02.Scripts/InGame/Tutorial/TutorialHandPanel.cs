using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using HHK.UIEC;
using FMODUnity;
using FMOD.Studio;

public class TutorialHandPanel : MonoBehaviour
{
    [Header("------------------ GUI Setting -----------------")]
    [SerializeField]
    private GameObject _tutorialArea;
    [SerializeField]
    private GameObject[] _actionAreas; // Click, Hold, Flick
    [SerializeField]
    private RectTransform _rightFingerRect;
    [SerializeField]
    private RectTransform _flickStartRect;
    [SerializeField]
    private RectTransform _flickEndRect;
    [SerializeField]
    private TutorialHandTouchableArea _touchableArea;
    [SerializeField]
    private NoteHoldUI _holdUI;
    [SerializeField]
    private UnityEngine.UI.Text _summaryText;

    [Header("------------------ Setting -----------------")]
    private Define.NoteTypeA _curActionType;
    private CancellationTokenSource _cancellationTokenSource;
    private string[] _actionSummaryLocalizationKeys = { "ActionTutorial_Click", "ActionTutorial_Hold", "ActionTutorial_Flick" };
    private const float _clickSec = 0.15f;
    private const float _clickDelay = 0.7f;
    private const float _holdSec = 0.7f;
    private const float _holdDelay = 2f;
    private const float _flickSec = 0.25f;
    private const float _flickMoveDelay = 0.75f;
    private const int _holdBeatCnt = 5;
    private Vector2 _defaultPos;
    private float _defaultAngleZ;

    #region Unity Life Cycle
    private void Awake()
    {
        _defaultAngleZ = _rightFingerRect.localEulerAngles.z;
        _defaultPos = _rightFingerRect.anchoredPosition;
    }

    private void Start()
    {
        //Init UI
        for (int i = 0; i < _actionAreas.Length; i++)
        {
            _actionAreas[i].SetActive(false);
        }

        _tutorialArea.SetActive(false);
    }

    private void OnDestroy()
    {
        // Ensure to cancel any ongoing animations when the object is destroyed
        _cancellationTokenSource?.Cancel();
    }
    #endregion

    #region UI Action
    public void ShowTutorial(Define.NoteTypeA type, UnityAction completeAction = null)
    {
        _curActionType = type;
        _tutorialArea.SetActive(true);
        _actionAreas[(int)type].SetActive(true);

        if(GamePlayData.Instance != null)
        {
            if (GamePlayData.Instance.tableData.localizationDic.TryGetValue(_actionSummaryLocalizationKeys[(int)type], out LocalizationInfo info))
            {
                _summaryText.text = info.summary;
            }
        }

        _cancellationTokenSource?.Cancel(); // Cancel previous animation if any
        _cancellationTokenSource = new CancellationTokenSource();;
        completeAction += HideTutorial;

        switch (type)
        {
            case Define.NoteTypeA.A_CLK:
                _touchableArea.clickCompleteAction = completeAction;
                _rightFingerRect.anchoredPosition = _defaultPos;
                RotateZFinger(-2f, 1f, _clickSec, _clickDelay);
                break;
            case Define.NoteTypeA.A_HLD:
                completeAction += () => { 
                    _touchableArea.holdStartAction = null;
                    _touchableArea.holdFailAction = null;
                };
                _holdUI.InitUI(_holdBeatCnt);
                _touchableArea.holdStartAction = () => {
                    _holdUI.StartHolding(_holdBeatCnt, 0.5f, completeAction); 
                };
                _touchableArea.holdFailAction = () => {
                    _holdUI.EndHolding();
                };
                _rightFingerRect.anchoredPosition = _defaultPos;
                RotateZFinger(-4.5f, 2f, _holdSec, _holdDelay);
                break;
            case Define.NoteTypeA.A_FLK:
                _touchableArea.flickCompleteAction = completeAction;
                _rightFingerRect.localEulerAngles = new Vector3(0, 0, _defaultAngleZ);
                MoveAnchoredFinger(_flickStartRect.anchoredPosition, _flickEndRect.anchoredPosition, _flickSec, _flickMoveDelay);
                break;
        }
    }

    public void HideTutorial()
    {
        _tutorialArea.SetActive(false);
        _actionAreas[(int)_curActionType].SetActive(false);
    }
    #endregion

    #region Finger Animation
    /// <summary>
    /// _rightFingerRect가 sec초 동안 z축 각도가 startZAngle에서 endZAngle로 연속적으로 바뀜.
    /// 전부 회전한 후에는 delay초의 딜레이를 가지고 다시 해당 동작을 반복함
    /// </summary>
    /// <param name="startZAngle"></param>
    /// <param name="endZAngle"></param>
    /// <param name="sec"></param>
    /// <param name="delay"></param>
    private async void RotateZFinger(float startZAngle, float endZAngle, float sec, float delay)
    {
        try
        {
            while (true)
            {
                _rightFingerRect.localEulerAngles = new Vector3(0, 0, startZAngle);
                var tween = _rightFingerRect.DORotate(new Vector3(0, 0, endZAngle), sec).SetEase(Ease.Linear);
                await UniTask.WaitUntil(() => !tween.IsActive(), cancellationToken: _cancellationTokenSource.Token);
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: _cancellationTokenSource.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // 이전 작업이 취소되면 예외 발생, 무시
            DebugX.Log("이전 작업 취소됨");

            if(_rightFingerRect != null)
            {
                _rightFingerRect.localEulerAngles = new Vector3(0, 0, _defaultAngleZ);
                _rightFingerRect.anchoredPosition = _defaultPos;
            }
        }
    }

    /// <summary>
    /// _rightFingerRect가 sec초 동안 anchoredPosition이 sec초 동안 startPos에서 endPos로 연속적으로 이동함.
    /// 전부 이동한 후에는 delay초의 딜레이를 가지고 다시 해당 동작을 반복함.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="sec"></param>
    /// <param name="delay"></param>
    private async void MoveAnchoredFinger(Vector2 startPos, Vector2 endPos, float sec, float delay)
    {
        try
        {
            while (true)
            {
                _rightFingerRect.anchoredPosition = startPos;
                var tween = _rightFingerRect.DOAnchorPos(endPos, sec).SetEase(Ease.Linear);
                await UniTask.WaitUntil(() => !tween.IsActive(), cancellationToken: _cancellationTokenSource.Token);
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: _cancellationTokenSource.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // 이전 작업이 취소되면 예외 발생, 무시
            DebugX.Log("이전 작업 취소됨");

            if (_rightFingerRect != null)
            {
                _rightFingerRect.localEulerAngles = new Vector3(0, 0, _defaultAngleZ);
                _rightFingerRect.anchoredPosition = _defaultPos;
            }
        }
    }
    #endregion
}