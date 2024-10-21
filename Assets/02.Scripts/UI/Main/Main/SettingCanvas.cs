using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using GooglePlayGames;
using Cysharp.Threading.Tasks;
using System.Threading;

public class SettingCanvas : BaseMainCanvas
{
    [Header("----------------- Body GUI -----------------")]
    [SerializeField]
    private Toggle mainBgmToggle;
    [SerializeField]
    private Toggle mainSfxToggle;
    [SerializeField]
    private Toggle mainNotiToggle;
    [SerializeField]
    private Toggle mainVibToggle;

    [Header("----------------- Bottom Button -----------------")]
    [SerializeField]
    private Button _saveDataBtn;
    [SerializeField]
    private Button _loadDataBtn;
    [SerializeField]
    private Button _logoutBtn;
    [SerializeField]
    private Button _termOfUseBtn;

    [Header("---------------- Setting ----------------")]
    [SerializeField]
    private EventReference _mainBgm;
    private EventInstance _mainBgmInstance;
    [SerializeField]
    private Ease onOffEase;
    private FirebaseStorageManager _firebaseStorageManager;
    private CancellationTokenSource _cts;
    private CancellationToken _ct;

    #region Unity Life Cycle
    private void Awake()
    {
        _firebaseStorageManager = this.GetComponent<FirebaseStorageManager>();
        _mainBgmInstance = RuntimeManager.CreateInstance(_mainBgm);
    }

    private void OnEnable()
    {
        if(GamePlayData.Instance == null)
        {
            return;
        }

        mainBgmToggle.isOn = GamePlayData.Instance.isCommonBGMOn;
        mainSfxToggle.isOn = GamePlayData.Instance.isCommonSFXOn;
        mainNotiToggle.isOn = GamePlayData.Instance.isNotiOn;
        mainVibToggle.isOn = GamePlayData.Instance.mobileVibrater.isOn;

        mainBgmToggle.GetComponent<SettingToggle>().SwitchToggleButton(mainBgmToggle.isOn);
        mainSfxToggle.GetComponent<SettingToggle>().SwitchToggleButton(mainSfxToggle.isOn);
        mainNotiToggle.GetComponent<SettingToggle>().SwitchToggleButton(mainNotiToggle.isOn);
        mainVibToggle.GetComponent<SettingToggle>().SwitchToggleButton(mainVibToggle.isOn);
        
        _mainBgmInstance.setVolume(GamePlayData.Instance.isCommonBGMOn ? 1f : 0f);

        mainNotiToggle.interactable = false; //임시
    }

    public override void Start()
    {
        base.Start();

        if(GamePlayData.Instance != null)
        {
            mainBgmToggle.onValueChanged.AddListener((isOn) => {
                GamePlayData.Instance.isCommonBGMOn = isOn;
                GamePlayData.Instance.OnClickToggleEffect();
                _mainBgmInstance.setVolume(isOn ? 1f : 0f);
            });
            mainSfxToggle.onValueChanged.AddListener((isOn) => {
                GamePlayData.Instance.isCommonSFXOn = isOn;
                GamePlayData.Instance.OnClickToggleEffect();
            });
        }

        mainVibToggle.onValueChanged.AddListener((isOn) => OnVibrateValueChanged(isOn));
        mainNotiToggle.onValueChanged.AddListener((isOn) => OnNotiValueChanged(isOn));
        _saveDataBtn?.onClick.AddListener(OnClickSaveDataBtn);
        _loadDataBtn?.onClick.AddListener(OnClickLoadDataBtn);
        _logoutBtn?.onClick.AddListener(OnClickLogOutBtn);
        _termOfUseBtn?.onClick.AddListener(OnClickTermOfUseBtn);
        _mainBgmInstance.start();
    }

    private void OnDestroy()
    {
        _mainBgmInstance.setUserData(System.IntPtr.Zero);
        _mainBgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _mainBgmInstance.release();

        _saveDataBtn = null;
        _loadDataBtn = null;
        _logoutBtn = null;
        _termOfUseBtn = null;
        _firebaseStorageManager = null;
        mainBgmToggle = null;
        mainSfxToggle = null;
        mainNotiToggle = null;
        mainVibToggle = null;
    }

    public void PauseMainBGM()
    {
        _mainBgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void PlayMainBGM()
    {
        _mainBgmInstance.start();
    }
    #endregion

    #region OnClick
    private async void OnClickSaveDataBtn()
    {
        if(GamePlayData.Instance != null)
        {
            GamePlayData.Instance.ShowLoading();
            GamePlayData.Instance.OnClickBtnEffect();
        }

        _cts = new CancellationTokenSource();
        _ct = _cts.Token;

        UserData userData = UserData.GetUserData();

        //원래 이러면 안되는데 로딩창 너무 잠깐보이면 오류같아서...
        try
        {
            await UniTask.Delay(500, cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {
            // 이전 작업이 취소되면 예외 발생, 무시
            DebugX.Log("UniTask 작업 취소됨");
        }

        _firebaseStorageManager.UploadUserData(userData, Social.localUser.id, 
            //Complete
            () => {
                if (GamePlayData.Instance != null)
                {
                    GamePlayData.Instance.HideLoading();
                    GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.StorageUploadComplete, false);
                }
            },
            
            //Failed
            () => {
                if (GamePlayData.Instance != null)
                {
                    GamePlayData.Instance.HideLoading();
                    GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.StorageUploadFailed, true);
                }
            });
    }

    private async void OnClickLoadDataBtn()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.ShowLoading();
            GamePlayData.Instance.OnClickBtnEffect();
        }

        //원래 이러면 안되는데 로딩창 너무 잠깐보이면 오류같아서...
        try
        {
            await UniTask.Delay(500, cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {
            // 이전 작업이 취소되면 예외 발생, 무시
            DebugX.Log("UniTask 작업 취소됨");
        }

        UserData userData = await _firebaseStorageManager.DownloadUserData(Social.localUser.id,
              () => {
                  if (GamePlayData.Instance != null)
                  {
                      GamePlayData.Instance.HideLoading();
                      GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.StorageDownloadComplete, false);
                  }
              },
              () => {
                  if (GamePlayData.Instance != null)
                  {
                      GamePlayData.Instance.HideLoading();
                      GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.StorageDownloadFailedNoFile, true);
                  }
              }
            );

        if(userData != null)
        {
            UserData.SetUserData(userData, () => { 
                OnEnable();
                //YSYS: 예슬님! 여기에요! 로드성공 시 메인 Canvas 업데이트해서 에셋 상황 저장되게 해주세요!
                MainUIManager.Instance.mainCanvas.Init();
            });
        }
    }

    private void OnClickLogOutBtn()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }

#if UNITY_ANDROID
        ((PlayGamesPlatform)Social.Active).SignOut();
        DebugX.Log("Google LogOut");
#endif
        SceneSwitcher.Instance.SwitchScene(Define.SceneName.Login);
    }

    private void OnClickTermOfUseBtn()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }

        Application.OpenURL("https://www.tmaxmetaai.com/");
    }
    #endregion

    #region Toggle Action
    private void OnVibrateValueChanged(bool isOn)
    {
        if(GamePlayData.Instance == null)
        {
            DebugX.Log("GamePlayData.Instance NULL로 진동 제어 불가능");
            return;
        }
        GamePlayData.Instance.OnClickToggleEffect();
        GamePlayData.Instance.mobileVibrater.isOn = isOn;
    }

    private void OnNotiValueChanged(bool isOn)
    {
        DebugX.Log("현재 알림 관련 연동이 없음, 추후 구현 예정");

        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.isNotiOn = isOn;
            GamePlayData.Instance.OnClickToggleEffect();
        }
    }
    #endregion
}