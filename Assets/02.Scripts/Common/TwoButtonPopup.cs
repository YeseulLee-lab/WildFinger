using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TwoButtonPopup : MonoBehaviour
{
    [SerializeField]
    private Image _popupBack;
    [SerializeField]
    private Button _closeBtn;
    [SerializeField]
    private RectTransform _panel;
    [SerializeField]
    private Text _popupTitle;
    [SerializeField]
    private Button _noBtn;
    [SerializeField]
    private Button _yesBtn;

    [Header("================= FMOD =================")]
    [SerializeField]
    private EventReference _showSfx;
    private EventInstance _showInstance;

    private UnityAction _yesAction;
    private UnityAction _noAction;

    private void Start()
    {
        if (GamePlayData.Instance != null)
        {
            _showInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }

        _yesBtn.onClick.AddListener(() => 
        {
            if (GamePlayData.Instance != null)
            {
                GamePlayData.Instance.OnClickBtnEffect();
            }
            Hide();
        });
        _noBtn.onClick.AddListener(() =>
        {
            if (GamePlayData.Instance != null)
            {
                GamePlayData.Instance.OnClickBtnEffect();
            }
            Hide();
        });
        _closeBtn.onClick.AddListener(() => 
        {
            if (GamePlayData.Instance != null)
            {
                GamePlayData.Instance.OnClickBtnEffect();
            }
            Hide();
        });

        _showInstance = RuntimeManager.CreateInstance(_showSfx);
    }

    private void OnDestroy()
    {
        _showInstance.setUserData(IntPtr.Zero);
        _showInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _showInstance.release();
    }

    public void Show(Define.PopupTitle popupTitle, UnityAction yesAction, UnityAction noAction)
    {
        _yesAction = yesAction;
        _noAction = noAction;

        _showInstance.start();
        _popupTitle.text = _popupTitle.GetComponent<LocalizationTextUI>().GetSummary("Popup_" + popupTitle.ToString());

        _popupBack.gameObject.SetActive(true);
        _panel.DOAnchorPosY(-668f, 0.5f).SetEase(Ease.OutBack);

        _yesBtn.onClick.AddListener(yesAction);
        _noBtn.onClick.AddListener(noAction);
    }

    public void Hide()
    {
        _yesBtn.onClick.RemoveListener(_yesAction);
        _noBtn.onClick.RemoveListener(_noAction);

        _panel.DOAnchorPosY(0f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _popupBack.gameObject.SetActive(false);
        });
    }
}
