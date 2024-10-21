using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
using HHK.UIEC;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Events;


/// <summary>
/// 인게임 연출 관련 처리
/// </summary>
public class InGameCutSceneManager : MonoBehaviour
{
    [SerializeField]
    private EventReference _monsterLaughingBGM;
    private EventInstance _monsterLaughingInstance;
    [SerializeField]
    private EventReference _beforeLaughingBGM;
    private EventInstance _beforeLaughingInstance;
    [SerializeField]
    private EventReference _monsterDiedBGM;
    private EventInstance _monsterDiedInstance;
    public bool isReady { get; private set; }
    private CancellationTokenSource _cts;
    private CancellationToken _ct;

    #region Unity Life Cycle
    private void Awake()
    {
        isReady = false;
        _monsterLaughingInstance = RuntimeManager.CreateInstance(_monsterLaughingBGM);
        _beforeLaughingInstance = RuntimeManager.CreateInstance(_beforeLaughingBGM);
        _monsterDiedInstance = RuntimeManager.CreateInstance(_monsterDiedBGM);
    }

    private void Start()
    {
        if (GamePlayData.Instance != null)
        {
            _monsterLaughingInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _beforeLaughingInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _monsterDiedInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    private void OnDestroy()
    {
        //리소스 할당 초기화
        _monsterLaughingInstance.setUserData(IntPtr.Zero);
        _monsterLaughingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _monsterLaughingInstance.release();
        _beforeLaughingInstance.setUserData(IntPtr.Zero);
        _beforeLaughingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _beforeLaughingInstance.release();
        _monsterDiedInstance.setUserData(IntPtr.Zero);
        _monsterDiedInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _monsterDiedInstance.release();
    }
    #endregion

    public async UniTask ShowStarting()
    {
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;
        _beforeLaughingInstance.start();
        BeatGridTracker.Instance.monsterManager.MoveMonsterPos(Define.MonsterPosType.Default);
        BeatGridTracker.Instance.monsterManager.SetMonsterAnim(Define.MonsterAnimType.Dash);
        try
        {
            await UniTask.Delay(500, cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {

        }

        BeatGridTracker.Instance.monsterManager.MoveMonsterPos(Define.MonsterPosType.VeryFront);
        BeatGridTracker.Instance.monsterManager.SetMonsterAnim(Define.MonsterAnimType.Attack1);
        try
        {
            await UniTask.Delay(500, cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {

        }

        _monsterLaughingInstance.start();
        BeatGridTracker.Instance.monsterManager.SetMonsterAnim(Define.MonsterAnimType.CastSpell);
        try
        {
            await UniTask.Delay(300, cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {

        }
        BeatGridTracker.Instance.monsterManager.SetMonsterAnim(Define.MonsterAnimType.Attack2);
        try
        {
            await UniTask.Delay(300, cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {

        }
        BeatGridTracker.Instance.monsterManager.SetMonsterAnim(Define.MonsterAnimType.Attack3);
        try
        {
            await UniTask.Delay(300, cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {

        }
        BeatGridTracker.Instance.monsterManager.SetMonsterAnim(Define.MonsterAnimType.Attack5);
        try
        {
            await UniTask.Delay(300, cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {

        }
        BeatGridTracker.Instance.monsterManager.SetMonsterAnim(Define.MonsterAnimType.CastSpell);
        try
        {
            await UniTask.Delay(700, cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {

        }
        BeatGridTracker.Instance.monsterManager.MoveMonsterPos(Define.MonsterPosType.Default);
        try
        {
            await UniTask.Delay(500, cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {

        }

        isReady = true;
    }

    public async UniTask ShowEnding()
    {
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;

        _monsterDiedInstance.start();
        BeatGridTracker.Instance.monsterManager.MoveMonsterPos(Define.MonsterPosType.VeryFront);
        BeatGridTracker.Instance.monsterManager.SetMonsterAnim(Define.MonsterAnimType.Die, 2f);
        try
        {
            await UniTask.Delay(1200, cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {

        }
        BeatGridTracker.Instance.monsterManager.MoveMonsterPos(Define.MonsterPosType.Default);
    }
}
