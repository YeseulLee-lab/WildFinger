using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HHK.UIEC;

public class MGSequenceManager : MonoBehaviour
{
    [SerializeField]
    private MGSequenceUI[] _seqenceUIs;
    [SerializeField]
    private GameObject[] _spaces;
    private int _curSeqence;
    private int _maxSeqCnt;

    public void InitUI(int maxSeqCnt)
    {
        _maxSeqCnt = maxSeqCnt;
        _curSeqence = 0;

        for (int i=0; i<_seqenceUIs.Length; i++)
        {
            _seqenceUIs[i].gameObject.SetActive(i < maxSeqCnt);
            _seqenceUIs[i].SelectedUI(false);
            _seqenceUIs[i].isResultShowed = false;
            if (i > 0)
            {
                _spaces[i - 1].SetActive(i < maxSeqCnt);
                continue;
            }
        }
    }

    public void SetSequence(int curSequence)
    {
        if(curSequence < 0)
        {
            for (int i = 0; i < _maxSeqCnt; i++)
            {
                _seqenceUIs[i].SelectedUI(false);
            }
            return;
        }

        _seqenceUIs[_curSeqence].SelectedUI(false);
        _seqenceUIs[curSequence].SelectedUI(true);
        _curSeqence = curSequence;
        if (_curSeqence > 0)
        {
            _spaces[_curSeqence - 1].GetComponent<UIECAnimator>().OnCustomChannel();
        }
    }

    public void SetCorrectUI(int curSequence, bool isCorrect) 
    {
        _seqenceUIs[curSequence].SetCorrectUI(isCorrect);
    }
}
