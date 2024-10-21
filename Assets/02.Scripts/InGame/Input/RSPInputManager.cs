using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using TMPro;
using UnityEngine.Video;
using DG.Tweening;
using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HHK.UIEC;

public class RSPInputManager : BaseRSPInputManager
{
    [SerializeField]
    private UIECAnimator _deckShakeAnim;
    [SerializeField]
    private EventReference _holdSfx;
    private EventInstance _holdInstance;

    [Header("------------------ Flick -----------------")]
    private Vector3 _flickStartPos;
    private Vector3 _flickEndPos;
    private float _flickMinDistance = 10f; // 최소한의 드래그 거리
    private float _flickMinSpeed = 1000f; // 최소한의 속도

    [Header("------------------ Hold -----------------")]
    private bool isHolding = false;
    private CancellationTokenSource _cts;
    private CancellationToken _ct;
    private const int _holdDelayMS = 100;

    #region Unity Life Cycle
    public override void Awake()
    {
        _holdInstance = RuntimeManager.CreateInstance(_holdSfx);
        base.Awake();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _deckShakeAnim = null;
        _cts = null;
        _holdInstance.setUserData(IntPtr.Zero);
        _holdInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _holdInstance.release();
    }

    public override void Start()
    {
        base.Start();
        if (GamePlayData.Instance != null)
        {
            _holdInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }
    #endregion

    #region On Pointer Trigger
    public override void OnPointerDown(int type, BaseInputUI input)
    {
        base.OnPointerDown(type, input);

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            _flickStartPos = Input.GetTouch(0).position;
        }
        else
        {
            _flickStartPos = Input.mousePosition;
        }

        _deckShakeAnim.OnCustomChannel();
        BeatGridTracker.Instance.judgeChecker.JudgeNoteClick(type);
        StartHolding(type);

        _holdInstance.start();
    }

    public override void OnPointerUp(int type, BaseInputUI input)
    {
        //TODO: 추후 sustain note에서 사용
        base.OnPointerUp(type, input);

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            _flickEndPos = Input.GetTouch(0).position;
        }
        else
        {
            _flickEndPos = Input.mousePosition;
        }

        CheckFlicking(type);
        EndHolding(type);
        _holdInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
    #endregion

    #region Flick
    private void CheckFlicking(int type)
    {
        float flickDistance = Vector3.Distance(_flickStartPos, _flickEndPos);
        float flickSpeed = flickDistance / Time.deltaTime;

        if (flickDistance >= _flickMinDistance && flickSpeed >= _flickMinSpeed)
        {
            // Flick 감지됨
            //DebugX.Log("Flick Detected!");
            Vector3 flickDirection = (_flickEndPos - _flickStartPos).normalized;

            //TODO: Flick 판정 노트
            BeatGridTracker.Instance.judgeChecker.JudgeNoteFlick(type);
        }

        //DebugX.Log($"flickDistance: {flickDistance}, flickSpeed: {flickSpeed}");
    }
    #endregion

    #region Hold
    private async UniTask StartHolding(int type)
    {
        isHolding = true;
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;

        BeatGridTracker.Instance.judgeChecker.JudgeNoteHoldStarting(type);
        while (isHolding)
        {
            // Hold 동작이 지속되는 동안 실행할 작업
            //DebugX.Log("Holding...");
            //Holding 동작 
            await UniTask.Delay(_holdDelayMS, cancellationToken: _ct);
        }
    }

    private void EndHolding(int type)
    {
        isHolding = false;
        _cts?.Cancel(); // Hold 동작 취소

        //TODO: Holding Unit이 잘 사라졌는지 확인, 아직 남아있으면 Miss
        BeatGridTracker.Instance.judgeChecker.JudgeNoteHoldEnding(type);
    }
    #endregion
}
