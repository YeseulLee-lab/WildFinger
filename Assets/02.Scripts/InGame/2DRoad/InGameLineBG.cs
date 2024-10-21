using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using UnityEngine.UI;
using HHK.UIEC;
using System.Threading;
using Cysharp.Threading.Tasks;

public class InGameLineBG : MonoBehaviour
{
    [SerializeField]
    private Image[] _lineBGImgs;
    [SerializeField]
    private LineBG[] _lineBGs; // 1줄, 2줄, 3줄 

    private void OnDestroy()
    {
        _lineBGImgs = null;
        _lineBGs = null;
    }

    public void SetLineBGPattern(int curMaxLineCnt, int curBeat)
    {
        for(int i=0; i<_lineBGImgs.Length; i++)
        {
            _lineBGImgs[i].sprite = ((i + curBeat) % 2 == 0) ? _lineBGs[curMaxLineCnt - 1].lineBGSprites[0] : _lineBGs[curMaxLineCnt - 1].lineBGSprites[1];
            _lineBGImgs[i].transform.GetChild(0).GetComponent<UIECAnimator>().OnCustomChannel();
        }
    }
}

[Serializable]
public class LineBG
{
    public Sprite[] lineBGSprites = new Sprite[2];
}
