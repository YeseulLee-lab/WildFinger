using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using HHK.UIEC;
using DG.Tweening;
using UnityEngine.Events;
using System;
using FMOD.Studio;
using FMODUnity;

public class MainCanvas : BaseMainCanvas
{
    [Header("----------------- MainCanvas Area -----------------")]
    [SerializeField]
    private QuestPanel _questPanel;

    [Header("----------------- Change Level Play Button -----------------")]
    [SerializeField]
    private ChangePlayButton _bossPlayBtn;
    [SerializeField]
    private ChangePlayButton _bonusPlayBtn;

    [Header("----------------- Main Buttons -----------------")]
    [SerializeField] 
    private Button _profileBtn;
    [SerializeField]
    private Button _settingBtn;
    [SerializeField]
    private Button _storeBtn;
    [SerializeField]
    private Button _playBtn;
    [SerializeField]
    private Button _selectLevelBtn;
    [SerializeField]
    private Button _trainingBtn;
    [SerializeField]
    private Button _townBtn;
    [SerializeField]
    private Button _collectionBtn;

    [Header("----------------- Event And Advertisement -----------------")]
    [SerializeField]
    private Button _adBtn;
    [SerializeField]
    private Button _eventBtn;
    [SerializeField]
    private Image[] _adChargingFillImgs;

    [Header("----------------- Wealth Manager -----------------")]
    [SerializeField]
    private MainCoinManager _coinManager;
    public MainCoinManager coinManager => _coinManager;
    [SerializeField]
    private MainHeartManager _heartManager;
    public MainHeartManager heartManager => _heartManager;
    [SerializeField]
    private MainQuaverManager _quaverManager;
    public MainQuaverManager quaverManager => _quaverManager;
    [SerializeField]
    private UIECAnimator[] _uiecAnimators;
    [SerializeField]
    private RectTransform _quaverPopPos;
    [SerializeField]
    private RectTransform _coinPopPos;

    #region Hide Show Obj Data
    [Header("----------------- Hide Main Object -----------------")]
    [SerializeField]
    private RectTransform headerObj;
    [SerializeField]
    private RectTransform questObj;
    [SerializeField]
    private RectTransform bottomObj;
    [SerializeField]
    private RectTransform eventObj;
    [SerializeField]
    private RectTransform adObj;
    private float trainingOriginX;

    private Ease hideEase = Ease.InBack;
    private Ease showEase = Ease.OutBack;
    float showHideDuration = 0.15f;
    #endregion

    [Header("------------------ SFX Area -----------------")]
    [SerializeField]
    private EventReference _hideSfx;
    private EventInstance _hideSfxInstance;
    [SerializeField]
    private EventReference _showSfx;
    private EventInstance _showSfxInstance;

    #region Unity Life Cycle
    private void Awake()
    {
        _hideSfxInstance = RuntimeManager.CreateInstance(_hideSfx);
        _showSfxInstance = RuntimeManager.CreateInstance(_showSfx);
    }

    private void Start()
    {
        OnClickInit();
        Init();

        if (GamePlayData.Instance != null)
        {
            _hideSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _showSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);

            GamePlayData.Instance.admobManager.adTimerUpdateAction += (fullAdTime) => SetAdTimerUpdateUI(fullAdTime);
        }

        _quaverPopPos.position = _playBtn.GetComponent<RectTransform>().position;
        _coinPopPos.position = _playBtn.GetComponent<RectTransform>().position;
    }

    private void OnDestroy()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.admobManager.adTimerUpdateAction -= (fullAdTime) => SetAdTimerUpdateUI(fullAdTime);
        }

        _hideSfxInstance.setUserData(IntPtr.Zero);
        _hideSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _hideSfxInstance.release();

        _showSfxInstance.setUserData(IntPtr.Zero);
        _showSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _showSfxInstance.release();
    }

    public void Init()
    {
        _questPanel.Init();
        trainingOriginX = _trainingBtn.GetComponent<RectTransform>().position.x;

        for (int i = 0; i < _uiecAnimators.Length; i++)
        {
            StartCoroutine(CoRandomAnim(_uiecAnimators[i]));
        }

        //첫 스테이지를 하고 메인화면으로 돌아오면 보여줌
        if (GamePlayData.Instance.maxStage == 1 && GamePlayData.Instance.maxTown == Define.TownList.ToyTown)
        {
            MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.PlayBtn, null, new RectTransform[] { _playBtn.GetComponent<RectTransform>() });
        }

        StartCoroutine(CoSetWealthData());

        #region play btn ui
        _playBtn.GetComponentInChildren<UnityEngine.UI.Text>().text = "Level " + GamePlayData.Instance.maxStage.ToString();
        _bossPlayBtn.levelText.text = "Level " + GamePlayData.Instance.maxStage.ToString();
        _bonusPlayBtn.levelText.text = "Level " + GamePlayData.Instance.maxStage.ToString();

        if (GamePlayData.Instance.maxStage == (GamePlayData.Instance.GetStackLevels(GamePlayData.Instance.maxTownInfo) + GamePlayData.Instance.maxTownInfo.levelAmount))
        {
            //보스레벨
            _playBtn.GetComponentInChildren<UnityEngine.UI.Text>().gameObject.SetActive(false);
            _bossPlayBtn.levelObj.SetActive(true);
            _playBtn.spriteState = _bossPlayBtn.playSpState;
            _playBtn.transform.GetChild(0).GetComponent<Image>().sprite = _bossPlayBtn.playSpState.selectedSprite;
        }
        else if (SceneSwitcher.IsBonusStage(GamePlayData.Instance.maxStage))
        {
            //보너스 레벨
            _playBtn.GetComponentInChildren<UnityEngine.UI.Text>().gameObject.SetActive(false);
            _bonusPlayBtn.levelObj.SetActive(true);
            _playBtn.spriteState = _bonusPlayBtn.playSpState;
            _playBtn.transform.GetChild(0).GetComponent<Image>().sprite = _bonusPlayBtn.playSpState.selectedSprite;
        }
        #endregion

        if (GamePlayData.Instance.maxStage > 1)
        {
            if (GamePlayData.Instance.isTrainingLocked)
            {
                MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.TrainingBtn, null, new RectTransform[] { _trainingBtn.GetComponent<RectTransform>() });
                _trainingBtn.GetComponent<RectTransform>().localScale = Vector2.zero;
                _trainingBtn.GetComponent<Animator>().enabled = true;

                GamePlayData.Instance.isTrainingLocked = false;
            }
            else
            {
                _trainingBtn.GetComponent<RectTransform>().localScale = Vector2.one;
                _trainingBtn.GetComponent<Animator>().enabled = false;
            }
        }
        else
        {
            _trainingBtn.GetComponent<RectTransform>().localScale = Vector2.zero;
            _trainingBtn.GetComponent<Animator>().enabled = false;
        }
    }
    #endregion

    #region OnClick
    private void OnClickInit()
    {
        _profileBtn.onClick.AddListener(OnClickProfile);
        _trainingBtn.onClick.AddListener(OnClickTraining);
        _settingBtn.onClick.AddListener(OnClickSetting);
        _storeBtn.onClick.AddListener(OnClickStore);
        _playBtn.onClick.AddListener(OnClickPlay);
        _selectLevelBtn.onClick.AddListener(() => 
        {
            _selectLevelBtn.interactable = false;
            HideMainObject(() =>
            {
                //메인화면에서 레벨 선택 맵 버튼 누름
                MainUIManager.Instance.SelectLevelCanvas.ShowCanvas(GamePlayData.Instance.maxTownInfo, false);
                _selectLevelBtn.interactable = true;
            });
        });
        _eventBtn?.onClick.AddListener(OnClickEventBtn);
        _adBtn?.onClick.AddListener(OnClickAdvertisementBtn);
    }

    private void OnClickProfile()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }
        //프로필
        MainUIManager.Instance.profileCanvas.ShowCanvas();
    }

    private void OnClickTraining()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }
        //프로필
        MainUIManager.Instance.trainingCanvas.ShowCanvas();
    }

    private void OnClickSetting()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }
        //설정
        MainUIManager.Instance.settingCanvas.ShowCanvas();
    }

    private void OnClickStore()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }
        //설정
        GamePlayData.Instance.storeCanvas.ShowCanvas();
    }

    private void OnClickPlay()
    {
        //플레이팝업
        LevelInfo levelInfo = new LevelInfo();
        levelInfo.level = GamePlayData.Instance.maxStage;
        MainUIManager.Instance.playCurLevelPanel.ShowPopup(GamePlayData.Instance.maxTownInfo, levelInfo);
    }
    #endregion

    #region Wealth
    IEnumerator CoSetWealthData()
    {
        yield return new WaitForEndOfFrame();
        SetWealthData();
    }

    public void SetWealthData()
    {
        quaverManager.SetQuaverData();
        coinManager.SetCoinData();
    }

    private IEnumerator CoRandomAnim(UIECAnimator uIECAnimator)
    {
        while (true)
        {
            float randomDelay = UnityEngine.Random.Range(1.5f, 2.0f);
            yield return new WaitForSeconds(randomDelay);
            uIECAnimator.OnCustomChannel();
        }
    }
    #endregion

    #region ShowHideObject
    public void HideMainObject(UnityAction complete)
    {
        Sequence sequence = DOTween.Sequence();
        if (!MainUIManager.Instance.townCanvas.isActive)
        {
            _hideSfxInstance.start();
            sequence.Append(_townBtn.GetComponent<RectTransform>().DOAnchorPosX(0f, showHideDuration).SetEase(hideEase))
            .Append(_collectionBtn.GetComponent<RectTransform>().DOAnchorPosX(0f, showHideDuration).SetEase(hideEase))
            .Append(_trainingBtn.GetComponent<RectTransform>().DOAnchorPosX(-_trainingBtn.GetComponent<RectTransform>().rect.width, showHideDuration).SetEase(hideEase))
            .Append(eventObj.GetComponent<RectTransform>().DOAnchorPosX(-eventObj.GetComponent<RectTransform>().rect.width, showHideDuration).SetEase(showEase))
            .Append(adObj.GetComponent<RectTransform>().DOAnchorPosX(adObj.GetComponent<RectTransform>().rect.width + Display.main.systemWidth, showHideDuration).SetEase(showEase))
            .Append(headerObj.DOAnchorPosY(headerObj.rect.height, showHideDuration).SetEase(hideEase))
            .Join(bottomObj.DOAnchorPosY(-bottomObj.rect.height, showHideDuration).SetEase(hideEase))
            .OnComplete(() => complete.Invoke());
        }
        else
        {
            sequence.Join(headerObj.DOAnchorPosY(headerObj.rect.height, showHideDuration).SetEase(hideEase))
            .Join(bottomObj.DOAnchorPosY(-bottomObj.rect.height, showHideDuration).SetEase(hideEase))
            .OnComplete(() => complete.Invoke());
        }
    }

    public void ShowMainObject(UnityAction endAction)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(eventObj.GetComponent<RectTransform>().DOMoveX(0f, showHideDuration).SetEase(hideEase))
                .Append(adObj.GetComponent<RectTransform>().DOMoveX(Display.main.systemWidth, showHideDuration).SetEase(hideEase))
                .Join(headerObj.DOAnchorPosY(0f, showHideDuration).SetEase(showEase))
                .Join(bottomObj.DOAnchorPosY(0f, showHideDuration).SetEase(showEase));

        if (!MainUIManager.Instance.townCanvas.isActive)
        {
            _showSfxInstance.start();
            Sequence btnSequence = DOTween.Sequence();
            btnSequence.Append(_trainingBtn.GetComponent<RectTransform>().DOAnchorPosX(trainingOriginX, showHideDuration).SetEase(hideEase))
                .Append(_townBtn.GetComponent<RectTransform>().DOAnchorPosX(_townBtn.GetComponent<RectTransform>().rect.width, showHideDuration).SetEase(showEase))
                .Append(_collectionBtn.GetComponent<RectTransform>().DOAnchorPosX(-_collectionBtn.GetComponent<RectTransform>().rect.width, showHideDuration).SetEase(showEase))
                .OnComplete(() =>
                {
                    if (endAction != null)
                        endAction.Invoke();
                });
        }


    }
    #endregion

    #region Event And Advertisement
    private void OnClickEventBtn()
    {
        if(GamePlayData.Instance == null)
        {
            return;
        }
        GamePlayData.Instance.OnClickBtnEffect();
        GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.ComingSoon);
    }

    private void OnClickAdvertisementBtn()
    {
        if (GamePlayData.Instance == null)
        {
            return;
        }

        //TODO: 광고 관련 팝업
        GamePlayData.Instance.OnClickBtnEffect();

        DebugX.Log("GamePlayData.Instance.admobManager.adChargingCnt: " + GamePlayData.Instance.admobManager.adChargingCnt);
        TimeSpan timeSinceFullAdCharging = GamePlayData.Instance.admobManager.adFullChargingTime - DateTime.Now;

        if((int)timeSinceFullAdCharging.TotalSeconds < 0)
        {
            GamePlayData.Instance.admobManager.adChargingCnt = MainKey.adMaxChargingCnt;
        }

        if (GamePlayData.Instance.admobManager.adChargingCnt < 1)
        {
            //현재 광고 충전 중
            StringBuilder sb = new StringBuilder();
            sb.Append(Define.ToastMessageType.AdCharging1);
            sb.Append((int)timeSinceFullAdCharging.TotalSeconds > (MainKey.adChargingCycleMin * 60)? (int)timeSinceFullAdCharging.TotalSeconds - MainKey.adChargingCycleMin * 60 : (int)timeSinceFullAdCharging.TotalSeconds);
            sb.Append(Define.ToastMessageType.AdCharging2);
            return;
        }

        GamePlayData.Instance.admobManager.ShowRewardVideo(() => {
            GamePlayData.Instance.heartTimer.SetInfiniteHeartTime();
            //TODO: 하트 충전 관련 애니메이션?
            GamePlayData.Instance.admobManager.SetAdChargingFullTime(MainKey.adChargingCycleMin);
        });
    }

    /// <summary>
    /// 광고 보기 충전되는 UI 업데이트
    /// </summary>
    private void SetAdTimerUpdateUI(DateTime fullAdTime)
    {
        TimeSpan adFullTimeSpan = fullAdTime - DateTime.Now;
        int oneChargingSec = (int)adFullTimeSpan.TotalSeconds;

        //DebugX.Log("oneChargingSec: " + oneChargingSec);
        if (oneChargingSec < 0)
        {
            //DebugX.Log("oneChargingSec 음수 오류");
            _adChargingFillImgs[0].fillAmount = 1f;
            _adChargingFillImgs[1].fillAmount = 1f;
            GamePlayData.Instance.admobManager.adFullChargingTime = DateTime.Now;
            GamePlayData.Instance.admobManager.adChargingCnt = MainKey.adMaxChargingCnt;
            return;
        }

        if (GamePlayData.Instance.admobManager.adChargingCnt < 1)
        {
            //2개 다 비어있는 케이스
            DebugX.Log("광고: 2개 다 비어있는 케이스");
            oneChargingSec -= (MainKey.adChargingCycleMin * 60);
            _adChargingFillImgs[0].fillAmount = 1f - (float)oneChargingSec / (float)(MainKey.adChargingCycleMin * 60);
            _adChargingFillImgs[1].fillAmount = 0f;
        }
        else
        {
            DebugX.Log("광고: 1개 비어있는 케이스");
            _adChargingFillImgs[0].fillAmount = 1f;
            _adChargingFillImgs[1].fillAmount = 1 - (float)oneChargingSec / (float)(MainKey.adChargingCycleMin * 60);
        }

        if(oneChargingSec == 0)
        {
            GamePlayData.Instance.admobManager.adChargingCnt++;
        }
    }
    #endregion
}

[Serializable]
public class ChangePlayButton
{
    public Sprite sprite;
    public UnityEngine.UI.Text levelText;
    public GameObject levelObj;
    public SpriteState playSpState;
}