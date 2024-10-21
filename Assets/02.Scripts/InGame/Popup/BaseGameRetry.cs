using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
using HHK.UIEC;
using FMODUnity;
using FMOD.Studio;

public class BaseGameRetry : InGameBasePopup
{
    [Header("-------------------- Retry Area ---------------------")]
    [SerializeField]
    private GameObject _popupArea;
    [SerializeField]
    private UnityEngine.UI.Text _retryLevelText;
    [SerializeField]
    private Button _retryBtn;
    [SerializeField]
    private Button _backToMainBtn;

    [Header("-------------------- FMOD ---------------------")]
    [SerializeField]
    private EventReference _retryBGM;
    private EventInstance _retryInstance;

    #region Unity Life Cycle
    public virtual void Awake()
    {
        _retryInstance = RuntimeManager.CreateInstance(_retryBGM);
    }

    public override void Start()
    {
        base.Start();

        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _retryInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }

        _retryBtn?.onClick.AddListener(() =>
        {
            OnClickRetryBtn();
        });

        _backToMainBtn?.onClick.AddListener(OnClickBackToMainBtn);

    }

    public new virtual void OnEnable()
    {
        base.OnEnable();
    }

    public virtual void OnDestroy()
    {
        _retryBtn = null;
        _backToMainBtn = null;
        _popupArea = null;
        _retryInstance.setUserData(IntPtr.Zero);
        _retryInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _retryInstance.release();
    }
    #endregion

    public virtual new void ShowPopup()
    {
        base.ShowPopup();
        SetInteractable(true);
        _popupArea.SetActive(true);
        _retryInstance.start();
        this.GetComponent<UIECAnimator>().OnCustomChannel();

        if (GamePlayData.Instance != null)
        {
            _retryLevelText.text = GamePlayData.Instance.curStage.ToString(); //Globalization
        }
    }

    public virtual void HidePopup()
    {
        _popupArea.SetActive(false);
    }

    /// <summary>
    /// 추후 해당 부분 유저 보유 재화(coin) 및 상점과 연동
    /// </summary>
    public virtual void OnClickRetryBtn()
    {
        SetInteractable(false);
        //TODO: 유저 보유 재화 확인 및 코인 사용

        //TODO: 재화가 사용해야하는 코인보다 적으면 코인 구매 유도를 위한 상점 페이지로 이동
        base.ShowBtnClickSFX();

        if (GamePlayData.Instance == null)
        {
            DebugX.Log("GamePlayData.Instance null");
            return;
        }

        if (!TownDataLoader.isTraining)
        {
            SceneSwitcher.Instance.SwitchGameScene(GamePlayData.Instance.curTown, GamePlayData.Instance.curStage);
        }
        else
        {
            SceneSwitcher.Instance.SwitchGameScene(TownDataLoader.tutorialType);
        }
    }

    public virtual void OnClickBackToMainBtn()
    {
        SetInteractable(false);
        base.ShowBtnClickSFX();
        if (SceneSwitcher.Instance == null)
        {
            return;
        }
        SceneSwitcher.Instance.SwitchScene(Define.SceneName.Main);
    }

    public override void OnClickBlackPanelBtn()
    {
        SetInteractable(false);
        base.OnClickBlackPanelBtn();
        OnClickBackToMainBtn();
    }

    public override void SetInteractable(bool active)
    {
        _retryBtn.interactable = active;
        _backToMainBtn.interactable = active;
    }
}
