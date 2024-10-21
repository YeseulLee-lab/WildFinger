using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Coffee.UIEffects;
using HHK.UIEC;

public class MGSequenceUI : MonoBehaviour
{
    [Header("------------------- Default -------------------")]
    [SerializeField]
    private Image _backgroundImg;
    [SerializeField]
    private Image _lineImg;
    [SerializeField]
    private UnityEngine.UI.Text _sequenceText;
    [SerializeField]
    private Color32[] _colors; //0: blackAlpha, 1: whiteAlpha

    [Header("------------------- Result -------------------")]
    [SerializeField]
    private Image _resultImg;
    [SerializeField]
    private Sprite[] _resultImgs; //0: correct, 1: incorrect
    public bool isResultShowed { get; set; }

    #region Unity Life Cycle
    private void OnDestroy()
    {
        _backgroundImg = null;
        _lineImg = null;
        _sequenceText = null;
        _colors = null;
        _resultImg = null;
        _resultImgs = null;
    }
    #endregion

    public void SelectedUI(bool isSelected)
    {
        InitUI(true);
        if (isSelected)
        {
            this.GetComponent<UIECAnimator>().OnCustomChannel();
            _backgroundImg.color = _colors[0];
            _lineImg.GetComponent<UIGradient>().enabled = true;
            _sequenceText.GetComponent<UIGradient>().enabled = true;
        }
        else
        {
            _backgroundImg.color = _colors[1];
            _lineImg.GetComponent<UIGradient>().enabled = false;
            _sequenceText.GetComponent<UIGradient>().enabled = false;
        }
    }

    public void SetCorrectUI(bool isCorrect)
    {
        InitUI(false);
        isResultShowed = true;
        _resultImg.sprite = _resultImgs[isCorrect ? 0 : 1];
        this.GetComponent<UIECAnimator>().OnCustomChannel();
    }

    public void InitUI(bool isInSequence)
    {
        if (isResultShowed)
        {
            return;
        }
        _backgroundImg.enabled = isInSequence;
        _lineImg.gameObject.SetActive(isInSequence);
        _sequenceText.gameObject.SetActive(isInSequence);
        _resultImg.gameObject.SetActive(!isInSequence);
    }
}
