using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using HHK.UIEC;
using TMPro;
using Sirenix.OdinInspector;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using FMOD.Studio;

public class TrainingResult : GameResult
{
    [Header("------------------ Training -----------------")]
    [SerializeField]
    private Button _replayBtn;
    [SerializeField]
    private UIECGroup _upDownAnim;
    private CancellationTokenSource _cts;
    private CancellationToken _ct;
    private const int _upDownAnimDelayMS = 305;
    [SerializeField]
    private EventReference _showBGM;
    private EventInstance _showInstance;
    [SerializeField]
    private EventReference _hideBGM;
    private EventInstance _hideInstance;

    #region Unity Life Cycle
    private void Awake()
    {
        _showInstance = RuntimeManager.CreateInstance(_showBGM);
        _hideInstance = RuntimeManager.CreateInstance(_hideBGM);
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Start()
    {
        base.Start();

        _replayBtn?.onClick.AddListener(OnClickReplayBtn);

        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _showInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _hideInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        _replayBtn = null;

        _showInstance.setUserData(IntPtr.Zero);
        _showInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _showInstance.release();
        _hideInstance.setUserData(IntPtr.Zero);
        _hideInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _hideInstance.release();
    }
    #endregion

    public override void ShowPopup(bool isTraining = false)
    {
        _showInstance.start();

        base.ShowPopup(isTraining);
    }
    public override void ShowScoreAndAnim() 
    {
        _upDownAnim.Show();
    }

    private async void OnClickReplayBtn()
    {
        SetInteractable(false);
        base.ShowBtnClickSFX();

        if (SceneSwitcher.Instance == null)
        {
            DebugX.Log("SceneSwitcher.Instance null");
            return;
        }

        _cts = new CancellationTokenSource();
        _ct = _cts.Token;

        _hideInstance.start();
        _upDownAnim.Hide();
        try
        {
            await UniTask.Delay(_upDownAnimDelayMS, cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {
            // 이전 작업이 취소되면 예외 발생, 무시
            DebugX.Log("이전 작업 취소됨");
        }
        SceneSwitcher.Instance.SwitchGameScene(TownDataLoader.tutorialType);
    }

    public async override void OnClickNextBtn()
    {
        SetInteractable(false);
        base.ShowBtnClickSFX();

        if (SceneSwitcher.Instance == null)
        {
            return;
        }

        _cts = new CancellationTokenSource();
        _ct = _cts.Token;

        _hideInstance.start();
        _upDownAnim.Hide();
        try
        {
            await UniTask.Delay(_upDownAnimDelayMS, cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {
            // 이전 작업이 취소되면 예외 발생, 무시
            DebugX.Log("이전 작업 취소됨");
        }
 
        SceneSwitcher.Instance.SwitchScene(Define.SceneName.Main);
    }

    public override void SetInteractable(bool active)
    {
        base.SetInteractable(active);
        _replayBtn.interactable = active;
    }
}
