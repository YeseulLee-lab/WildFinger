using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class MGMemorizationJudgeChecker : MonoBehaviour
{
    public async void JudgeSequence(Define.RSPType type)
    {
        //TODO: Check 정답
        if(MGMemorizationManager.Instance.curCorrectTypes == null)
        {
            DebugX.Log("_gameManager.curCorrectTypes NULL");
            return;
        }

        if(MGMemorizationManager.Instance.curCorrectTypes[MGMemorizationManager.Instance.curEnterIndex] != type)
        {
            // 잘못 누름, 체력 깎이자
            DebugX.Log("Judge: 틀림 => " + MGMemorizationManager.Instance.curEnterIndex);
            MGMemorizationManager.Instance.SetCorrect(MGMemorizationManager.Instance.curEnterIndex, false);
        }
        else
        {
            DebugX.Log("Judge: 맞음 => " + MGMemorizationManager.Instance.curEnterIndex);
            MGMemorizationManager.Instance.SetCorrect(MGMemorizationManager.Instance.curEnterIndex, true);
        }

        if (MGMemorizationManager.Instance.curEnterIndex >= MGMemorizationManager.Instance.curMaxEnterIndex - 1)
        {
            MGMemorizationManager.Instance.isEnteringComplete = true;
        }
        else
        {
            ++MGMemorizationManager.Instance.curEnterIndex;
        }

        try
        {
            await UniTask.Delay(300);
        }
        catch (OperationCanceledException)
        {
            // 이전 작업이 취소되면 예외 발생, 무시
            DebugX.Log("이전 작업 취소됨");
        }

    }
}
