using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using GooglePlayGames;
using UnityEngine.Events;

public class GamePlayData : MonoBehaviour
{
    public static GamePlayData Instance { get; private set; }

    [Header("--------------------- Loading GUI ---------------------")]
    [SerializeField]
    private CommonLoadingPanel _commonLoading;
    [SerializeField]
    private LandLoadingPanel _landLoading;

    [Header("---------------------- Popup GUI ---------------------")]
    [field: SerializeField]
    public GameNoHeart noHeartPopup;
    [field: SerializeField]
    public ToastPopup toastPopup;
    [field: SerializeField]
    public TwoButtonPopup twoButtonPopup;
    [field: SerializeField] 
    public BuyIngameItemPopup buyIngameItemPopup;
    [field: SerializeField] 
    public StoreCanvas storeCanvas;

    [Header("------------------------ Util -----------------------")]
    [field: SerializeField]
    public MobileVibrate mobileVibrater;
    public HeartTimer heartTimer { get; private set; }
    [field: SerializeField]
    public AdmobManager admobManager;
    [field: SerializeField]
    public AddressableGroupLoader addressableGroupLoader;
    [field: SerializeField]
    public TableData tableData;
    [SerializeField]
    private bool _isTrainingLocked;
    public bool isTrainingLocked
    {
        get
        {
            return _isTrainingLocked;
        }
        set
        {
            _isTrainingLocked = value;
            PlayerPrefs.SetInt(UnencryptedKey.trainingLocked, value ? 1 : 0);
        }
    }

    [Header("---------------------- FMOD Area ---------------------")]
    [SerializeField]
    private EventReference _btnSfx;
    private EventInstance _btnSfxInstance;
    [SerializeField]
    private EventReference _toggleSfx;
    private EventInstance _toggleSfxInstance;

    [Header("---------------------- Collection ---------------------")]
    private int _totalMusicCnt;
    public int totalMusicCnt
    {
        get
        {
            return _totalMusicCnt;
        }
        set
        {
            _totalMusicCnt = value;
            PlayerPrefs.SetInt(EncryptedKey.totalMusicCnt, value);
        }
    }

    private int _totalAlbumCnt;
    public int totalAlbumCnt
    {
        get
        {
            return _totalAlbumCnt;
        }
        set
        {
            _totalAlbumCnt = value;
            PlayerPrefs.SetInt(EncryptedKey.totalAlbumCnt, value);
        }
    }


    [Header("--------------------- Setting  ---------------------")]
    private bool _isCommonSFXOn;
    public bool isCommonSFXOn { get { return _isCommonSFXOn; } set {
            _isCommonSFXOn = value;
            PlayerPrefs.SetInt(UnencryptedKey.isSFXOn, value ? 1 : 0);
        } }
    private bool _isCommonBGMOn;
    public bool isCommonBGMOn { get { return _isCommonBGMOn; }
        set
        {
            _isCommonBGMOn = value;
            PlayerPrefs.SetInt(UnencryptedKey.isBGMOn, value ? 1 : 0);
        }
    }
    private bool _isNotiOn;
    public bool isNotiOn { get { return _isNotiOn; }
        set
        {
            _isNotiOn = value;
            PlayerPrefs.SetInt(UnencryptedKey.isNotiOn, value ? 1 : 0);
        }
    }
    private int _isFirst;
    public int isFirst
    {
        get { return _isFirst; }
        set
        {
            _isFirst = value;
            PlayerPrefs.SetInt(UnencryptedKey.isFirst, _isFirst);
        }
    }

    [Header("------------------ Wealth Data -----------------")]
    private int _coinCnt;
    public int coinCnt
    {
        get
        { return _coinCnt; }
        set
        {
            _coinCnt = value;
            PlayerPrefs.SetInt(EncryptedKey.coinCnt, _coinCnt);
            if (FirestoreManager.Instance != null)
                FirestoreManager.Instance.SetCoinCnt(_coinCnt);
        }
    }
    private int _getCoinCnt;
    public int getCoinCnt
    {
        get
        { return _getCoinCnt; }
        set
        {
            _getCoinCnt = value;
            PlayerPrefs.SetInt(EncryptedKey.getCoinCnt, _getCoinCnt);
        }
    }
    private int _recordQuaverCnt;
    public int recordQuaverCnt
    {
        get
        {
            return _recordQuaverCnt;
        }
        set
        {
            _recordQuaverCnt = value;
            PlayerPrefs.SetInt(EncryptedKey.recordQuaverCnt, _recordQuaverCnt);
            if (FirestoreManager.Instance != null)
                FirestoreManager.Instance.SetQuaverCnt(_recordQuaverCnt);
        }
    }
    private int _remainQuaverCnt;
    public int remainQuaverCnt
    {
        get
        {
            return _remainQuaverCnt;
        }
        set
        {
            _remainQuaverCnt = value;
            PlayerPrefs.SetInt(EncryptedKey.remainQuaverCnt, _remainQuaverCnt);
        }
    }
    private int _getQuaverCnt;
    public int getQuaverCnt
    {
        get
        {
            return _getQuaverCnt;
        }
        set
        {
            _getQuaverCnt = value;
            PlayerPrefs.SetInt(EncryptedKey.getQuaverCnt, _getQuaverCnt);
        }
    }

    [Header("------------------ Town and Level Data -----------------")]
    [SerializeField]
    private TownData allTownData;
    private TownInfo _maxTownInfo;
    public TownInfo maxTownInfo
    {
        get
        {
            _maxTownInfo = allTownData.townDatas[(int)_maxTown];
            return allTownData.townDatas[(int)_maxTown];
        }
        set
        {
            _maxTownInfo = value;
        }
    }
    private Define.TownList _maxTown;
    public Define.TownList maxTown
    {
        get
        { return _maxTown; }
        set
        {
            _maxTown = value;

            PlayerPrefs.SetInt(EncryptedKey.maxTown, (int)_maxTown);
        }
    }
    private Define.TownList _curTown;
    public Define.TownList curTown { get
        { return _curTown; }
        set {
            _curTown = value;
        }
    }
    private int _maxStage;
    public int maxStage
    {
        get { return _maxStage; }
        set
        {
            _maxStage = value;
            PlayerPrefs.SetInt(EncryptedKey.maxLevel, _maxStage);
            if(FirestoreManager.Instance != null)
                FirestoreManager.Instance.SetMaxStage(_maxStage);
        }
    }
    private int _curStage;
    public int curStage { get { return _curStage; }
        set {
            if (value > maxDevelpedStage)
            {
                _curStage = maxDevelpedStage + 1;
            }
            else
            {
                _curStage = value;
            }

            if (maxStage < _curStage)
            {
                maxStage = _curStage;
            }
        } }
    private int _maxAssetIdx;
    public int maxAssetIdx
    {
        get { return _maxAssetIdx; }
        set
        {
            _maxAssetIdx = value;
            PlayerPrefs.SetInt(EncryptedKey.maxAsset, value);
        }
    }

    //TODO: Setting 하는 부분 만들어야함, 지금은 강제로 값 세팅
    public int maxDevelpedStage 
    {
        get
        {
            int cnt = 0;
            for (int i = 0; i < allTownData.townDatas.Length; i++)
            {
                cnt += allTownData.townDatas[i].levelAmount;
            }
            return cnt;
        }
    }
    public int maxDevelpedTown
    {
        get
        {
            return allTownData.townDatas.Length;
        }
    }

    [Header("------------------ InGame Data -----------------")]
    private int _itemSheildCnt;
    public int itemSheildCnt { get { return _itemSheildCnt; }
        set {
            _itemSheildCnt = value;
            PlayerPrefs.SetInt(EncryptedKey.ItemShieldCnt, value < 1? 0 : value);
            if (FirestoreManager.Instance != null)
                FirestoreManager.Instance.SetItemsCnt(Define.UsingItemBeforeInGame.Shield, _itemSheildCnt);
        }
    }
    private int _itemMaxHealthCnt;
    public int itemIncreadeHPCnt
    {
        get { return _itemMaxHealthCnt; }
        set
        {
            _itemMaxHealthCnt = value;
            PlayerPrefs.SetInt(EncryptedKey.ItemMaxHealthCnt, value < 1 ? 0 : value);
            if (FirestoreManager.Instance != null)
                FirestoreManager.Instance.SetItemsCnt(Define.UsingItemBeforeInGame.IncreasedHP, _itemMaxHealthCnt);
        }
    }
    private int _itemIncreaseHPCnt;
    public int itemIncreasedHealingHPCnt
    {
        get { return _itemIncreaseHPCnt; }
        set
        {
            _itemIncreaseHPCnt = value;
            PlayerPrefs.SetInt(EncryptedKey.ItemIncreaseHPCnt, value < 1 ? 0 : value);
            if (FirestoreManager.Instance != null)
                FirestoreManager.Instance.SetItemsCnt(Define.UsingItemBeforeInGame.IncreasedHealingHP, _itemIncreaseHPCnt);
        }
    }
    public bool[] itemCnts { get; private set; }
    public int inGameMaxHP { get; set; } = InGameKey.defaultIngameLife; // 바뀔 수 있음(아이템)
    public int inGameTryCnt { get; set; } //메인 화면에서 시작할 때 0으로 초기화
    private int _isSuccessfulOnFirstTryCnt;
    public int isSuccessfulOnFirstTryCnt { get { return _isSuccessfulOnFirstTryCnt; } set
        {
            _isSuccessfulOnFirstTryCnt = value;
            PlayerPrefs.SetInt(EncryptedKey.isSuccessfulOnFirstTryCnt, value < 0 ? 0 : value);
        } }

    [Header("------------------ Application Quit -----------------")]
    private const float _appQuitDelay = 1f;
    private float _lastBackPressTime = 0f;
    private bool _isExitRequested = false;
    private WaitForSeconds _yieldAppQuitDelay;
    //TODO: iOS에서 빌드할 때는 Info.plist 파일에 백그라운드 모드 권한을 요구해야 함

    [Header("------------------ UserProfile -----------------")]
    private string _uid;
#if UNITY_EDITOR
    public string uid { get { return "PCTestAccount"; } set { _uid = value; } }
#elif UNITY_ANDROID
    public string uid
    {
        get
        {
            Debug.Log("_uid:" + _uid);
            return _uid;
        }
        set
        {
            _uid = value;
        }
    }
#endif

    private DateTime _joinDate;
    public DateTime joinDate
    {
        get
        {
            return _joinDate;
        }
        set
        {
            _joinDate = value;
            PlayerPrefs.SetString(EncryptedKey.joinDate, value.ToString("yyyyMMddHHmmss"));
        }
    }
    private DateTime _recentDataUploadTime;
    public DateTime recentDataUploadTime
    {
        get
        {
            return _recentDataUploadTime;
        }
        set
        {
            _recentDataUploadTime = value;
            PlayerPrefs.SetString(EncryptedKey.recentDataUploadTime, value.ToString("yyyyMMddHHmmss"));
        }
    }

    private DateTime _recentDataDownloadTime;
    public DateTime recentDataDownloadTime
    {
        get
        {
            return _recentDataDownloadTime;
        }
        set
        {
            _recentDataDownloadTime = value;
            PlayerPrefs.SetString(EncryptedKey.recentDataDownloadTime, value.ToString("yyyyMMddHHmmss"));
        }
    }
    public bool isDebugMode { get; set; } = false;

    #region Unity Life Cycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        heartTimer = this.GetComponent<HeartTimer>();
        InitData();
        _btnSfxInstance = RuntimeManager.CreateInstance(_btnSfx);
        _toggleSfxInstance = RuntimeManager.CreateInstance(_toggleSfx);
        _yieldAppQuitDelay = new WaitForSeconds(_appQuitDelay);
    }

    private void OnDestroy()
    {
        Instance = null;

        _btnSfxInstance.setUserData(IntPtr.Zero);
        _btnSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _btnSfxInstance.release();

        _toggleSfxInstance.setUserData(IntPtr.Zero);
        _toggleSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _toggleSfxInstance.release();
    }

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_isExitRequested)
            {
                _isExitRequested = true;
                _lastBackPressTime = Time.time;
                StartCoroutine(CheckExit());
            }
            else if (Time.time - _lastBackPressTime <= _appQuitDelay)
            {
                Application.Quit();
            }
        }
    }
#endif
    #endregion

    /// <summary>
    /// Forced Initialization
    /// </summary>
    private void InitData()
    {
        _isFirst = PlayerPrefs.GetInt(UnencryptedKey.isFirst);
        _maxTownInfo = allTownData.townDatas[(int)maxTown];
        //처음 접속 혹은 PlayerPref에 데이터 있음
        heartTimer.InitFirst(_isFirst);
        admobManager.InitFirst(_isFirst);
        if (_isFirst == 0)
        {
            DebugX.Log("로드할 데이터 없음. 첫 데이터 생성");

            maxTown = 0;
            curTown = 0;
            maxStage = 0;
            curStage = 1;
            maxAssetIdx = 0;
            coinCnt = 100000; //Tempt
            recordQuaverCnt = 0;

            remainQuaverCnt = 0;
            totalMusicCnt = 0;
            isTrainingLocked = true;
            //최초 접속 1로 변경해주는 부분 IntroVideoPlayer 스크립트

            isCommonBGMOn = true;
            isCommonSFXOn = true;
            isNotiOn = false; //추후 수정
            mobileVibrater.isOn = true;

            //item Setting
            itemSheildCnt = 0;
            itemIncreadeHPCnt = 0;
            itemIncreasedHealingHPCnt = 0;
            isSuccessfulOnFirstTryCnt = 0;
        }
        else
        {
            _maxTown = (Define.TownList)PlayerPrefs.GetInt(EncryptedKey.maxTown);
            _curTown = _maxTown;
            _maxStage = PlayerPrefs.GetInt(EncryptedKey.maxLevel);
            _curStage = _maxStage;

            _maxAssetIdx = PlayerPrefs.GetInt(EncryptedKey.maxAsset);
            heartTimer.heartCnt = PlayerPrefs.GetInt(EncryptedKey.heartCnt);
            _coinCnt = PlayerPrefs.GetInt(EncryptedKey.coinCnt);
            _recordQuaverCnt = PlayerPrefs.GetInt(EncryptedKey.recordQuaverCnt);
            _remainQuaverCnt = PlayerPrefs.GetInt(EncryptedKey.remainQuaverCnt);
            _totalMusicCnt = PlayerPrefs.GetInt(EncryptedKey.totalMusicCnt);
            _totalAlbumCnt = PlayerPrefs.GetInt(EncryptedKey.totalAlbumCnt);
            _isTrainingLocked = (PlayerPrefs.GetInt(UnencryptedKey.trainingLocked) == 0 ? false : true);

            _isCommonBGMOn = (PlayerPrefs.GetInt(UnencryptedKey.isBGMOn) == 0 ? false : true);
            _isCommonSFXOn = (PlayerPrefs.GetInt(UnencryptedKey.isSFXOn) == 0 ? false : true);
            _isNotiOn = (PlayerPrefs.GetInt(UnencryptedKey.isNotiOn) == 0 ? false : true);
            mobileVibrater.isOn = (PlayerPrefs.GetInt(UnencryptedKey.isVibOn) == 0 ? false : true);

            _itemSheildCnt = PlayerPrefs.GetInt(EncryptedKey.ItemShieldCnt); ;
            _itemMaxHealthCnt = PlayerPrefs.GetInt(EncryptedKey.ItemMaxHealthCnt);
            _itemIncreaseHPCnt = PlayerPrefs.GetInt(EncryptedKey.ItemIncreaseHPCnt);

            _joinDate = DateTime.ParseExact(PlayerPrefs.GetString(EncryptedKey.joinDate, DateTime.Now.ToString("yyyyMMddHHmmss")), "yyyyMMddHHmmss", null);
            _recentDataUploadTime = DateTime.ParseExact(PlayerPrefs.GetString(EncryptedKey.recentDataUploadTime, DateTime.Now.ToString("yyyyMMddHHmmss")), "yyyyMMddHHmmss", null);
            _recentDataDownloadTime = DateTime.ParseExact(PlayerPrefs.GetString(EncryptedKey.recentDataDownloadTime, DateTime.Now.ToString("yyyyMMddHHmmss")), "yyyyMMddHHmmss", null);

            _isSuccessfulOnFirstTryCnt = PlayerPrefs.GetInt(EncryptedKey.isSuccessfulOnFirstTryCnt);
        }

        InitItem();
    }

    public void GetFireStoreData()
    {
        //firestore에 저장이 되어 있으면 coin, qauver, 스테이지, 아이템 가져옴,
        // 아니면 새로 문서 데이터 만듦
        Debug.Log("유저 체크");
        FirestoreManager.Instance.CheckUserExist(
        () =>
        {
            FirestoreManager.Instance.GetCoinCnt((cnt) =>
            {
                Debug.Log("db에 저장되어 있는 코인 개수: "+ cnt);
                coinCnt = cnt;
                if (MainUIManager.Instance != null)
                {
                    MainUIManager.Instance.mainCanvas.coinManager.SetCoinData();
                }
            });

            FirestoreManager.Instance.GetQuaverCnt((cnt) =>
            {
                recordQuaverCnt = cnt;
                remainQuaverCnt = cnt;
                if (MainUIManager.Instance != null)
                {
                    MainUIManager.Instance.mainCanvas.quaverManager.SetQuaverData();
                }
            });

            FirestoreManager.Instance.GetMaxStage(uid, (stage) =>
            {
                maxStage = stage;
                maxTown = SceneSwitcher.GetTownList(maxStage);
                if (MainUIManager.Instance != null)
                {
                    MainUIManager.Instance.mainCanvas.Init();
                }
            });

            FirestoreManager.Instance.GetScores((scores) =>
            {
                for (int i = 0; i < scores.Count; i++)
                {
                    PlayerPrefs.SetInt(EncryptedKey.score + (i + 1).ToString(), scores[i]);
                }
            });

            FirestoreManager.Instance.GetIsAllPerfectLevels((allPerfects) =>
            {
                for (int i = 0; i < allPerfects.Count; i++)
                {
                    PlayerPrefs.SetInt(EncryptedKey.isAllPerfect + (i + 1).ToString(), allPerfects[i] == true ? 1 : 0);
                }
            });

            FirestoreManager.Instance.GetItemsCnt((items) =>
            {
                PlayerPrefs.SetInt(EncryptedKey.ItemIncreaseHPCnt, items[0]);
                PlayerPrefs.SetInt(EncryptedKey.ItemMaxHealthCnt, items[1]);
                PlayerPrefs.SetInt(EncryptedKey.ItemShieldCnt, items[2]);
            });

            FirestoreManager.Instance.GetJoinDate((date) =>
            {
                joinDate = date;
            });
        },
        () =>
        {
#if UNITY_EDITOR
            FirestoreManager.Instance.CreateUser(uid, "PCTestAccount", "test@gmail.com");

#elif UNITY_ANDROID
            FirestoreManager.Instance.CreateUser(uid, Social.localUser.userName, ((PlayGamesLocalUser)Social.localUser).Email);    
#endif
        });
    }

    public TownInfo GetTownInfo(Define.TownList town)
    {
        return allTownData.townDatas[(int)town];
    }

    public int GetStackLevels(TownInfo curTownInfo)
    {
        int stackLevel = 0;
        for (int i = 0; i < (int)curTownInfo.townType; i++)
        {
            stackLevel += GetTownInfo((Define.TownList)i).levelAmount;
        }
        return stackLevel;
    }

    #region Common SFX
    public void OnClickBtnEffect(long milliseconds = InGameKey.defaultVibrateMS, int amplitude = InGameKey.defaultVibrateAmplitude)
    {
        if(SceneSwitcher.Instance == null)
        {
            _btnSfxInstance.start();
            mobileVibrater.Vibrate(milliseconds, amplitude);
            return;
        }

        mobileVibrater.Vibrate(milliseconds, amplitude);

        if (isCommonSFXOn)
        {
            _btnSfxInstance.start();
        }
    }

    public void OnClickToggleEffect(long milliseconds = InGameKey.defaultVibrateMS, int amplitude = InGameKey.defaultVibrateAmplitude)
    {
        if (SceneSwitcher.Instance == null)
        {
            _toggleSfxInstance.start();
            mobileVibrater.Vibrate(milliseconds, amplitude);
            return;
        }

        mobileVibrater.Vibrate(milliseconds, amplitude);

        if (isCommonSFXOn)
        {
            _toggleSfxInstance.start();
        }
    }
    #endregion

    #region Item
    /// <summary>
    /// 모든 게임 입장 시, 아이템 사용 자동으로 전부 false 초기화
    ///  ***일단 슬롯 한 번 열리면, 갯수 상관 없이 사용할 수 있게 해주세요
    /// </summary>
    public void InitItem()
    {
        itemCnts = new bool[System.Enum.GetNames(typeof(Define.UsingItemBeforeInGame)).Length]; 

        for (int i = 0; i < itemCnts.Length; i++)
        {
            itemCnts[i] = false;
        }
    }

    /// <summary>
    /// 사용할 아이템 메인에서 게임 들어가기 전에 체크했을 때 호출
    /// </summary>
    /// <param name="item">사용할 아이템</param>
    /// <param name="active">사용할 지(true), 안 할 지(false)</param>
    public void SetItemState(Define.UsingItemBeforeInGame item, bool active)
    {
        DebugX.Log($"itemCnt {item}: " + active);
        itemCnts[(int)item] = active;

        if (item == Define.UsingItemBeforeInGame.IncreasedHP)
        {
            inGameMaxHP = active ? (InGameKey.defaultIngameLife + InGameKey.itemIncreasedHPAmount) : InGameKey.defaultIngameLife;
        }
    }
    #endregion

    #region Application Quit
    System.Collections.IEnumerator CheckExit()
    {
        yield return _yieldAppQuitDelay;
        _isExitRequested = false;
    }
    #endregion

    #region Loading
    private bool _isLanding = false;

    public void ShowLoading(bool isLanding = false, UnityAction<float> progress = null, UnityAction DownloadComplete = null)
    {
        _isLanding = isLanding;

        if (_isLanding)
        {
            _landLoading.Show(progress, DownloadComplete);
        }
        else
        {
            _commonLoading.Show(progress, DownloadComplete);
        }
    }

    public void HideLoading()
    {
        if (_isLanding)
        {
            _landLoading.Hide();
        }
        else
        {
            _commonLoading.Hide();
        }

        _isLanding = false;
    }
    #endregion
}
