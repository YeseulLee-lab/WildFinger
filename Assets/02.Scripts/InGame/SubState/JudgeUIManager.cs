using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using HHK.UIEC;
using System.Threading;
using Cysharp.Threading.Tasks;

public class JudgeUIManager : MonoBehaviour
{
    [Header("--------------------- UI --------------------")]
    [SerializeField]
    private Image _timingImg;
    [SerializeField]
    private Sprite[] _timingSprites; //Pefect, Good, Miss

    [Header("--------------------- Animation --------------------")]
    [SerializeField]
    private UIECAnimator[] _monsterMissBoomAnims;

    [Header("--------------------- VFX --------------------")]
    [SerializeField]
    private GameObject[] _shootVFXs;

    [Header("--------------------- Setting --------------------")]
    private CancellationTokenSource _cts;
    private CancellationToken _ct;
    private const int _boomDelayMS = 150;
    private bool _isHitVFX = false;

    private void OnDestroy()
    {
        _timingImg = null;
        _timingSprites = null;
        _cts = null;
        _monsterMissBoomAnims = null;
    }

    /// <summary>
    /// Judge 글씨 UI 표시
    /// </summary>
    /// <param name="judge"></param>
    /// <param name="curRemainedBoomCnt"></param>
    public async void SetJudgeUI(Define.NoteJudge judge = Define.NoteJudge.None, int curRemainedBoomCnt = 1)
    {
        if(judge == Define.NoteJudge.None)
        {
            this.gameObject.SetActive(false);
            return;
        }

        SetEffect(judge, true);
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;

        _isHitVFX = true;
        _monsterMissBoomAnims[1].gameObject.SetActive(false);
        _monsterMissBoomAnims[0].gameObject.SetActive(false);
        _timingImg.gameObject.SetActive(true);
        _timingImg.sprite = _timingSprites[(int)judge];
        _timingImg.GetComponent<UIECAnimator>().OnCustomChannel();

        switch (judge)
        {
            case Define.NoteJudge.None:
                break;
            case Define.NoteJudge.Miss:
                if (GamePlayData.Instance != null)
                {
                    GamePlayData.Instance.mobileVibrater.Vibrate(InGameKey.hpMinusVibrateMS, InGameKey.hpMinusVibrateAmplitude);
                }
                _monsterMissBoomAnims[0].gameObject.SetActive(true);
                _monsterMissBoomAnims[0].OnCustomChannel();

                try
                {
                    await UniTask.Delay(_boomDelayMS, cancellationToken: _ct);
                    _isHitVFX = false;
                }
                catch (OperationCanceledException)
                {
                    // 이전 작업이 취소되면 예외 발생, 무시
                    // DebugX.Log("[JudgeUIManager] Judge VFX 이전 작업 취소됨1");
                    _isHitVFX = true;
                }

                if (curRemainedBoomCnt == 2)
                {
                    if (GamePlayData.Instance != null)
                    {
                        GamePlayData.Instance.mobileVibrater.Vibrate(InGameKey.hpMinusVibrateMS, InGameKey.hpMinusVibrateAmplitude);
                    }
                    _monsterMissBoomAnims[1].gameObject.SetActive(true);
                    _monsterMissBoomAnims[1].OnCustomChannel();

                    try
                    {
                        await UniTask.Delay(InGameKey.judgeEffectMS, cancellationToken: _ct);
                        _isHitVFX = false;
                    }
                    catch (OperationCanceledException)
                    {
                        // 이전 작업이 취소되면 예외 발생, 무시
                        // DebugX.Log("[JudgeUIManager] Judge VFX 이전 작업 취소됨2");
                        _isHitVFX = true;
                    }
                    _monsterMissBoomAnims[1].gameObject.SetActive(false);
                    _monsterMissBoomAnims[0].gameObject.SetActive(false);
                }
                break;
            default:
                try
                {
                    await UniTask.Delay(InGameKey.judgeEffectMS, cancellationToken: _ct);
                    _isHitVFX = false;
                }
                catch (OperationCanceledException)
                {
                    // 이전 작업이 취소되면 예외 발생, 무시
                    // DebugX.Log("[JudgeUIManager] Judge VFX 이전 작업 취소됨3");
                    _isHitVFX = true;
                }
                break;
        }

        if (!_isHitVFX)
        {
            SetEffect(judge, false);
            this.gameObject.SetActive(false);
        }
    }

    #region Effect
    private void SetEffect(Define.NoteJudge judgeType, bool active)
    {
        if(judgeType == Define.NoteJudge.None)
        {
            return;
        }

        _shootVFXs[(int)judgeType].SetActive(active);
    }
    #endregion
}
