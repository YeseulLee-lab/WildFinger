using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarUnit : BaseObjectPoolUnit
{
    [field: SerializeField]
    public NoteInfo noteInfo { get; set; }

    public void SetUnit(NoteInfo noteInfo)
    {
        this.noteInfo = noteInfo;
        this.GetComponent<RectTransform>().localPosition = new Vector3(-this.GetComponent<RectTransform>().anchoredPosition.x, (int)(noteInfo.position * BasicKey.positionScale), 0f);
        this.GetComponent<RectTransform>().localScale = Vector3.one;

        //필요 시, Bar 길이 조정? (Line 수에 따라)
    }
}
