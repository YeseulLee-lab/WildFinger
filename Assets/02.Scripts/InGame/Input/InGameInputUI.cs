using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public class InGameInputUI : BaseInputUI, IDragHandler
{
    [Header("----------------- VFX Setting -----------------")]
    [SerializeField]
    private GameObject _vfx;
    [SerializeField]
    private GameObject _vfxHold;

    [Header("----------------- Flick Action -----------------")]
    [SerializeField]
    private Image _btnLazerImg;
    [SerializeField]
    private Sprite[] _lazerImgs; //lazer - Default, Pressed, flick
    private RectTransform _btnImgRect;
    [SerializeField]
    private RectTransform _flickDownPos;
    [SerializeField]
    private RectTransform _flickUpPos;
    private const float _flickMovingDelay = 0.2f; 

    #region Unity Life Cycle
    public override void Awake()
    {
        base.Awake();
        _btnImgRect = base.btnBGImg.rectTransform;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        _btnImgRect = null;
        _btnLazerImg = null;
        _vfx = null;
        _vfxHold = null;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        _btnLazerImg.sprite = _lazerImgs[0];
    }
    #endregion

    #region Base Input Action
    public override void ResetBtnUI(int mSec, UnityAction resetAction = null)
    {
        resetAction += () => { };
        base.ResetBtnUI(mSec, resetAction);
    }

    public override void BtnPressAnim(bool isPressed)
    {
        base.BtnPressAnim(isPressed);

        if (isPressed)
        {
            _vfxHold.SetActive(true);
            _btnLazerImg.sprite = _lazerImgs[1];
        }
        else
        {
            _vfxHold.SetActive(false);
            _btnLazerImg.sprite = _lazerImgs[0];
        }

        if (_vfx.activeSelf)
        {
            _vfx.SetActive(false);
        }
        _vfx.SetActive(true);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!BeatGridTracker.rspInputManager.isEnteringReady)
        {
            return;
        }

        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!BeatGridTracker.rspInputManager.isEnteringReady)
        {
            return;
        }

        base.OnPointerUp(eventData);
        _btnLazerImg.sprite = _lazerImgs[2];
        _btnImgRect.DOMove(_flickDownPos.position, _flickMovingDelay).OnComplete(()=> {
            _btnLazerImg.sprite = _lazerImgs[0];
        });
    }
    #endregion

    #region Flick Action
    public void OnDrag(PointerEventData eventData)
    {
        if (!BeatGridTracker.rspInputManager.isEnteringReady)
        {
            return;
        }

        Vector2 flickPosition = Vector2.Lerp(_flickDownPos.position, _flickUpPos.position, Mathf.InverseLerp(_flickDownPos.position.y, _flickUpPos.position.y, eventData.position.y));
        _btnImgRect.position = flickPosition;
    }
    #endregion
}
