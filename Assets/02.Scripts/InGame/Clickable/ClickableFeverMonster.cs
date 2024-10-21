using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableFeverMonster : ClickableObject
{
    public override void OnObjectClicked()
    {
        DebugX.Log(this.name + " OnObjectClicked");
        //TODO: 구현
        if (BeatGridTracker.Instance == null)
        {
            DebugX.Log("BeatGridTracker Null");
            return;
        }


        if (!BeatGridTracker.Instance.feverManager.isFever)
        {
            return;
        }

        if(BeatGridTracker.Instance.feverManager.feverType == Define.FeverType.Pinata)
        {
            DebugX.Log("피냐타-임!!");
            //TODO: 호출 위치 다시 생각해보기
            BeatGridTracker.Instance.monsterManager.SetMonsterDamaged(Define.NoteJudge.Perfect);
            //돈이 쏟아지고..
        }
    }
}
