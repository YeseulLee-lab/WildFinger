using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
using HHK.UIEC;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Events;

public class BaseGamePause : InGameBasePopup
{
    [SerializeField]
    private Button _pauseBtn;

    [Header("------------------ GUI Setting -----------------")]
    [SerializeField]
    private GameObject _pausePanel;
    [SerializeField]
    private GameObject _pauseMenuPanel;
    [SerializeField]
    private Button _resumeBtn;
    [SerializeField]
    private Button _replayBtn;
    [SerializeField]
    private Button _backToMainBtn;

    [Header("------------------ Resume Setting -----------------")]
    [SerializeField]
    private TextMeshProUGUI _resumeCntDownText;
    private int _resumeCntDown;
    private CancellationTokenSource _cts;
    private CancellationToken _ct;

    [Header("-------------------- FMOD ---------------------")]
    [SerializeField]
    private EventReference _cntDownBGM;
    private EventInstance _cntDownInstance;
    private bool _isStart = false;
    public UnityAction resumeAction { get; set; } = null;
    public UnityAction cntDownAction { get; set; } = null;
    private bool _isCntDown = false;

    #region Unity Life Cycle
    public virtual void Awake()
    {
        _isStart = false;
        _isCntDown = false;
        _cntDownInstance = RuntimeManager.CreateInstance(_cntDownBGM);
    }

    public override void Start()
    {
        base.Start();

        _resumeBtn?.onClick.AddListener(() => OnClickResumeBtn());
        _replayBtn?.onClick.AddListener(OnClickRetryBtn);
        _backToMainBtn?.onClick.AddListener(OnClickBackToMainBtn);

        // SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _cntDownInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    public virtual void OnDestroy()
    {
        _pausePanel = null;
        _pauseMenuPanel = null;
        _resumeBtn = null;
        _replayBtn = null;
        _backToMainBtn = null;
        _resumeCntDownText = null;
        _cts = null;
        resumeAction = null;

        _cntDownInstance.setUserData(IntPtr.Zero);
        _cntDownInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _cntDownInstance.release();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (!_isStart)
        {
            _isStart = true;
            return;
        }
    }

    public virtual void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            CancelCountdown();
        }
    }
    #endregion

    #region UI Action
    public virtual void ShowPause()
    {
        base.ShowPopup();
        SetInteractable(true);

        _pauseBtn.interactable = false;
        _pausePanel?.SetActive(true);
        _resumeCntDownText.gameObject.SetActive(false);
        _pauseMenuPanel.SetActive(true);
    }

    public virtual void HidePause()
    {
        _pausePanel?.SetActive(false);
    }

    public virtual async void OnClickResumeBtn()
    {
        SetInteractable(false);
        base.VibrateBtnClick();
        CancelCountdown(); // 이전 카운트다운 취소
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;
        _pauseMenuPanel.SetActive(false);
        _resumeCntDownText.gameObject.SetActive(true);
        _isCntDown = true;
        cntDownAction?.Invoke();

        for (int i = InGameKey.resumeCntDown; i > 0; i--)
        {
            if (!_isCntDown)
            {
                return;
            }

            _resumeCntDownText.text = i.ToString();
            base.VibrateBtnClick();

            try
            {
                if (_cntDownInstance.isValid())
                {
                    _cntDownInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                }
                _cntDownInstance.start();
                await UniTask.Delay(1000, cancellationToken: _ct);
            }
            catch (OperationCanceledException)
            {
                // 취소됐을 때 처리
                DebugX.Log("Pause Timer 취소 됨");
                _isCntDown = false;
                _cntDownInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _resumeCntDownText.gameObject.SetActive(false);
                _pauseMenuPanel.SetActive(true);
                return;
            }
        }

        HidePause();

        // 게임 재개
        _pauseBtn.interactable = true;
        resumeAction?.Invoke();
        _isCntDown = false;
    }

    public virtual void OnClickRetryBtn()
    {
        SetInteractable(false);
        base.ShowBtnClickSFX();

        if (SceneSwitcher.Instance == null)
        {
            return;
        }
        SceneSwitcher.Instance.SwitchGameScene(GamePlayData.Instance.curTown, GamePlayData.Instance.curStage);
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

    public virtual void CancelCountdown(UnityAction cancelCntDownAction = null)
    {
        if (_cts != null && _isCntDown)
        {
            _cts.Cancel();
            _isCntDown = false;
            _cntDownInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            cancelCntDownAction?.Invoke();
        }
    }

    public override void OnClickBlackPanelBtn()
    {
        SetInteractable(false);
        base.OnClickBlackPanelBtn();
        OnClickResumeBtn();
    }

    public override void SetInteractable(bool active)
    {
        _resumeBtn.interactable = active;
        _replayBtn.interactable = active;
        _backToMainBtn.interactable = active;
    }
    #endregion
}
