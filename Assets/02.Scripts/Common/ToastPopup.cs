using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using DG.Tweening;
using DG;
using FMODUnity;
using FMOD.Studio;

public class ToastPopup : MonoBehaviour
{
    [Header("================= Toast Property =================")]
    [SerializeField]
    private Image _toastBack;
    [SerializeField]
    private RectTransform _toastRect;
    [SerializeField]
    private Text _msgText;
    [SerializeField]
    private CanvasGroup _toastCG;
    [SerializeField]
    private Button _closeToastBtn;
    private Sequence _toastSequence = null;
    private const float _toastMovingDelay = 0.3f;
    private const float _toastAlphaDelay = 0.15f;

    [Header("================= Error Toast =================")]
    [SerializeField]
    private Sprite _defaultSp;
    [SerializeField]
    private Sprite _errorSp;
    [SerializeField]
    private Text _errorMsgText;
    private bool _isShowed = false;
    private bool _isHideCalled = false;
    private Queue<(Define.ToastMessageType, bool)> _toastQueue = new Queue<(Define.ToastMessageType, bool)>();
    private Define.ToastMessageType _currentMsgType;
    private bool _currentIsError;

    [Header("================= FMOD =================")]
    [SerializeField]
    private EventReference _defaultSfx;
    private EventInstance _defaultInstance;
    [SerializeField]
    private EventReference _errorSfx;
    private EventInstance _errorInstance;

    #region Unity Life Cycle
    private void Awake()
    {
        _toastSequence = DOTween.Sequence();

        _defaultInstance = RuntimeManager.CreateInstance(_defaultSfx);
        _errorInstance = RuntimeManager.CreateInstance(_errorSfx);
    }

    private void Start()
    {
        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _defaultInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _errorInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }

        _closeToastBtn?.onClick.AddListener(() => HideToastMessage());
    }

    private void OnDestroy()
    {
        _defaultInstance.setUserData(IntPtr.Zero);
        _defaultInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _defaultInstance.release();

        _errorInstance.setUserData(IntPtr.Zero);
        _errorInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _errorInstance.release();
    }
    #endregion

    #region Toast
    public void ShowToastMessage(Define.ToastMessageType msgKey, bool isError = false)
    {
        if (_isShowed)
        {
            if (!IsMessageAlreadyInQueue(msgKey, isError))
            {
                _toastQueue.Enqueue((msgKey, isError));
            }
            return;
        }

        DisplayToast(msgKey, isError);
    }

    private bool IsMessageAlreadyInQueue(Define.ToastMessageType msgKey, bool isError)
    {
        if (_currentMsgType == msgKey && _currentIsError == isError)
        {
            return true;
        }

        foreach (var toast in _toastQueue)
        {
            if (toast.Item1 == msgKey && toast.Item2 == isError)
            {
                return true;
            }
        }

        return false;
    }

    private void DisplayToast(Define.ToastMessageType msgKey, bool isError)
    {
        _isShowed = true;
        _currentMsgType = msgKey;
        _currentIsError = isError;

        if (isError)
        {
            _errorInstance.start();
            _toastBack.sprite = _errorSp;
            _msgText.gameObject.SetActive(false);
            _errorMsgText.gameObject.SetActive(true);
            _errorMsgText.text = _msgText.GetComponent<LocalizationTextUI>().GetSummary("Toast_" + msgKey.ToString());
        }
        else
        {
            _defaultInstance.start();
            _toastBack.sprite = _defaultSp;
            _msgText.gameObject.SetActive(true);
            _errorMsgText.gameObject.SetActive(false);
            _msgText.text = _msgText.GetComponent<LocalizationTextUI>().GetSummary("Toast_" + msgKey.ToString());
        }

        _toastRect.gameObject.SetActive(true);
        _toastSequence = DOTween.Sequence()
            .OnStart(() =>
            {
                _toastCG.alpha = 0f;
            })
            .Append(_toastRect.DOAnchorPosY(300, _toastMovingDelay))
            .Join(_toastCG.DOFade(1f, _toastAlphaDelay))
            .OnComplete(() =>
            {
                _toastCG.alpha = 1f;
                HideToastMessage(1.5f);
            });
    }

    public void HideToastMessage(float delay = 0.0f)
    {
        if (_isHideCalled)
        {
            return;
        }

        _isHideCalled = true;
        _toastSequence = DOTween.Sequence()
               .OnStart(() =>
               {
                   _toastCG.alpha = 1f;
               })
               .Append(_toastRect.DOAnchorPosY(152, _toastMovingDelay))
               .Join(_toastCG.DOFade(0f, _toastAlphaDelay))
               .SetDelay(delay)
               .OnComplete(() =>
               {
                   _toastCG.alpha = 0f;
                   _toastRect.gameObject.SetActive(false);
                   _isShowed = false;
                   _isHideCalled = false;

                   if (_toastQueue.Count > 0)
                   {
                       var nextToast = _toastQueue.Dequeue();
                       DisplayToast(nextToast.Item1, nextToast.Item2);
                   }
               });
    }

    public void ForcedHideToastMessage()
    {
        _toastCG.alpha = 0f;
        _toastRect.gameObject.SetActive(false);
        _isShowed = false;
        _isHideCalled = false;

        if (_toastQueue.Count > 0)
        {
            var nextToast = _toastQueue.Dequeue();
            DisplayToast(nextToast.Item1, nextToast.Item2);
        }
    }
    #endregion
}