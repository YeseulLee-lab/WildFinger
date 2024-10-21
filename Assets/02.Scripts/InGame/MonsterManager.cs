using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using TMPro;
using HHK.UIEC;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class MonsterManager : MonoBehaviour
{
    [Header("---------------Monster Field---------------")]
    [SerializeField]
    private Transform _monsterTran;
    [SerializeField]
    private GameObject[] _monsters = new GameObject[Enum.GetNames(typeof(Define.TownList)).Length - 2]; //Townlist만큼
    public bool isReady { get; private set; } = false;

    [Header("---------------Anim Field---------------")]
    private Animator _animator;
    private IEnumerator _bodyCoroutine;
    private int _randomIndex;
    private CancellationTokenSource _cts;
    private CancellationToken _ct;

    [Header("---------------VFX Field---------------")]
    [SerializeField]
    private GameObject _stunVFX;
    [SerializeField]
    private GameObject[] _gimmickNoteAttackMonsterVFXs;
    [SerializeField]
    private UIECAnimator _monsterDamagedAnim;
    private const int _monsterDamagedDelayMS = 1000;
    private CancellationTokenSource _ctsDamaged;
    private CancellationToken _ctDamaged;
    private Define.MonsterAnimType _curType;

    [Header("---------------Moving Field---------------")]
    [SerializeField]
    private Transform[] _movingPositions; //Back, Front
    private const float _movingDelay = 0.5f;
    private CancellationTokenSource _movingCts;
    private CancellationToken _movingCt;
    private const string _animKey = "selectedAnim";
    private Vector3 initPos { get; set; }
    private Quaternion initRot { get; set; }
    private const float _initMovingDelay = 0.3f;

    #region Unity Life Cycle
    private void Awake()
    {
        isReady = false;
    }

    private void OnDestroy()
    {
        if (_cts != null)
            _cts.Cancel();

        _animator = null;
        _cts = null;
    }
    #endregion

    #region Spawn & Despawn
    public void SpawnMonster(Define.TownList town)
    {
        GameObject monster = Instantiate(_monsters[((int)town < 0 || (int)town >= Enum.GetNames(typeof(Define.TownList)).Length - 2) ? 0: (int)town], _monsterTran);
        _animator = monster.GetComponent<Animator>();
        isReady = true;
        initPos = _animator.transform.localPosition; // 현재 위치 저장
        initRot = _animator.transform.localRotation;
    }
    #endregion

    #region Show Animation
    public void SetMonsterAnim(Define.MonsterAnimType type, float speed = 1f)
    {
        //DebugX.Log("Monster Anim: " + type);
        if (!isReady)
        {
            DebugX.Log("몬스터 스폰 전이라 애니메이션 불가능");
            return;
        }

        _curType = type;

        if (_animator != null)
            _animator.SetInteger(_animKey, (int)type);

        InitMonsterPosAndRot();

        WaitAnim(type.ToString(), speed,
           delegate
           {
               if (_animator != null)
                   _animator.SetInteger(_animKey, 0);

               InitMonsterPosAndRot();
           });
    }

    private async void WaitAnim(string clipName, float timePercentage, UnityAction callBack)
    {
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(GetClipLength(clipName) * timePercentage), cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {
            InitMonsterPosAndRot();
        }

        callBack?.Invoke();
    }

    /// <summary>
    /// 애니메이션이 시작될 때 초기 위치와 회전으로 되돌리기
    /// </summary>
    /// <param name="delay">이동 시간(초)</param>
    private void InitMonsterPosAndRot(float delay = _initMovingDelay)
    {
        if(_animator == null)
        {
            return;
        }

        _animator.transform.DOLocalMove(initPos, delay);
        _animator.transform.DOLocalRotateQuaternion(initRot, delay);
    }

    private float GetClipLength(string clipName)
    {
        RuntimeAnimatorController controller = _animator.runtimeAnimatorController;
        for (int i = 0; i < controller.animationClips.Length; i++)
        {
            if (controller.animationClips[i].name == clipName)
            {
                return controller.animationClips[i].length;
            }
        }
        return 0.0f;
    }
    #endregion

    #region Move
    public void MoveMonsterPos(Define.MonsterPosType type, bool isFrontAnim = false)
    {
        Vector3 targetPosition = _movingPositions[(int)type].localPosition;

        switch (type)
        {
            default:
                CancelRepeatAction(); // RepeatAction 취소
                break;
            case Define.MonsterPosType.Front:
                _movingCts?.Cancel(); // 이전 작업이 있을 경우 취소
                _movingCts = new CancellationTokenSource();
                if(isFrontAnim) RepeatAction(() => SetMonsterAnim(Define.MonsterAnimType.Damaged), 0.05f, _movingCts.Token);
                break;
        }

        _monsterTran.DOLocalMove(targetPosition, _movingDelay);
    }

    private async void RepeatAction(Action action, float interval, CancellationToken token)
    {
        _stunVFX.SetActive(true);
        try
        {
            while (!token.IsCancellationRequested)
            {
                action();
                await UniTask.Delay(TimeSpan.FromSeconds(interval), cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {
            //DebugX.Log("Canceled Monster Spin");
        }
    }

    private void CancelRepeatAction()
    {
        _stunVFX.SetActive(false);
        _movingCts?.Cancel();
    }

    #endregion

    #region Global VFX
    public void SetGimmickNoteAttackVFX(Define.NoteTypeN gimmickType)
    {
        if (_gimmickNoteAttackMonsterVFXs[(int)gimmickType].activeSelf)
        {
            _gimmickNoteAttackMonsterVFXs[(int)gimmickType].SetActive(false);
        }
        _gimmickNoteAttackMonsterVFXs[(int)gimmickType].SetActive(true);
    }

    public async void SetMonsterDamaged(Define.NoteJudge judgetype = Define.NoteJudge.None)
    {
        switch (judgetype)
        {
            case Define.NoteJudge.Perfect:
                _ctsDamaged = new CancellationTokenSource();
                _ctDamaged = _ctsDamaged.Token;
                _monsterDamagedAnim.OnCustomChannel();
                await UniTask.Delay(_monsterDamagedDelayMS, cancellationToken: _ctDamaged);
                break;
            case Define.NoteJudge.Good:
                _ctsDamaged = new CancellationTokenSource();
                _ctDamaged = _ctsDamaged.Token;
                _monsterDamagedAnim.OnCustomChannel();
                await UniTask.Delay(_monsterDamagedDelayMS, cancellationToken: _ctDamaged);
                break;
            case Define.NoteJudge.Miss:
                break;
        }
    }
    #endregion
}
