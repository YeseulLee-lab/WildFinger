using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using HHK.UIEC;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CollectionCanvas : BaseMainCanvas
{
    [Header("------------------ Collection Canvas Area ------------------")]
    [SerializeField]
    private Button _openButton;
    [SerializeField]
    private GameObject _musicListPopup;
    public GameObject _blockImage;

    [Header("---------------- Collection Progress ----------------")]
    [SerializeField]
    private UnityEngine.UI.Text _quaverCnt;
    [SerializeField]
    private UnityEngine.UI.Text _progressText;
    [SerializeField]
    private Image _progressImage;
    [SerializeField]
    private Image _albumIcon;

    [Header("---------------- Tutorial Area ----------------")]
    [SerializeField]
    private Transform _albumContent;
    [SerializeField]
    private Transform _musicContent;
    [SerializeField]
    private RectTransform _progressArea;
    public RectTransform _quaverArea;

    [Header("------------------ SFX Area -----------------")]
    [SerializeField]
    private EventReference _openSfx;
    private EventInstance _openSfxInstance;
    [SerializeField]
    private EventReference _collectPercentageSfx;
    private EventInstance _collectPercentageSfxInstance;

    private int _curAlbumCnt = 0;

    #region Unity Life Cycle
    private void Awake()
    {
        _openSfxInstance = RuntimeManager.CreateInstance(_openSfx);
        _collectPercentageSfxInstance = RuntimeManager.CreateInstance(_collectPercentageSfx);
    }

    public override void Start()
    {
        base.Start();
        Init();
    }

    private void Init()
    {
        if (GamePlayData.Instance != null)
        {
            _openSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _collectPercentageSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }

        _openButton.onClick.AddListener(ShowCanvas);

        if (MainUIManager.Instance.tutorialCanvas.IsTutorialDone(Define.MainTutorialType.CollectionPage2)
                || MainUIManager.Instance.tutorialCanvas.IsTutorialDone(Define.MainTutorialType.CollectionPage3)
                || MainUIManager.Instance.tutorialCanvas.IsTutorialDone(Define.MainTutorialType.CollectionPage4))
            return;
        MainUIManager.Instance.collectionCanvas.StartCollectionTutorial();
    }
    public void StartCollectionTutorial()
    {
        //Tutorial
        if (GamePlayData.Instance.remainQuaverCnt >= GamePlayData.Instance.GetTownInfo(Define.TownList.ToyTown).albumInfo.collectMusics[0].needQuaver)
        {
            //미리 musiclistpopup setdata해줘서 unit 접근할 수 있게 함.
            _musicContent.GetComponent<CollectMusicScrollContent>().SetData(GamePlayData.Instance.GetTownInfo(Define.TownList.ToyTown).albumInfo);

            if (!MainUIManager.Instance.tutorialCanvas.IsTutorialDone(Define.MainTutorialType.CollectionPage1))
            {
                _albumContent.parent.parent.GetComponent<ScrollRect>().vertical = false;
                _musicContent.parent.parent.GetComponent<ScrollRect>().vertical = false;
                MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.CollectionPage1,
                () =>
                {
                    _albumContent.parent.parent.GetComponent<ScrollRect>().vertical = true;
                    _musicContent.parent.parent.GetComponent<ScrollRect>().vertical = true;
                },
                new RectTransform[] { _openButton.GetComponent<RectTransform>(), _quaverArea.GetComponent<RectTransform>(),
                                        _albumContent.GetChild(0).GetChild(0).GetComponent<RectTransform>(),
                                        _musicContent.GetChild(0).GetChild(0).GetComponent<CollectMusicSubUnit>().unlockBtn.GetComponent<RectTransform>() });
            }
        }
    }
    private void OnDestroy()
    {
        _openSfxInstance.setUserData(IntPtr.Zero);
        _openSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _openSfxInstance.release();

        _collectPercentageSfxInstance.setUserData(IntPtr.Zero);
        _collectPercentageSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _collectPercentageSfxInstance.release();
    }
    #endregion

    #region SetData
    public void SetAlbumProgress()
    {
        //이전 튜토리얼이 끝났으면 CollectionPage5 시작
        if (MainUIManager.Instance.tutorialCanvas.IsTutorialDone(Define.MainTutorialType.CollectionPage3))
        {
            MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.CollectionPage4, null,
                        new RectTransform[] { _progressArea.GetComponent<RectTransform>() });
        }

        _progressText.text = GamePlayData.Instance.totalMusicCnt + "/" + (GamePlayData.Instance.maxDevelpedTown * MainKey.musicCntEachLand).ToString();

        float ratio = (float)(GamePlayData.Instance.totalMusicCnt) / (GamePlayData.Instance.maxDevelpedTown * MainKey.musicCntEachLand);

        if (ratio > 0)
        {
            if (_curAlbumCnt < GamePlayData.Instance.totalMusicCnt)
            {
                _albumIcon.GetComponent<UIECAnimator>().OnCustomChannel();
                _progressImage.rectTransform.DOSizeDelta(new Vector2(ratio * _progressImage.transform.parent.GetComponent<RectTransform>().sizeDelta.x, _progressImage.rectTransform.sizeDelta.y), 0.3f);
                _collectPercentageSfxInstance.start();
                _curAlbumCnt = GamePlayData.Instance.totalMusicCnt;
            }
            else
            {
                _progressImage.rectTransform.sizeDelta = new Vector2(ratio * _progressImage.transform.parent.GetComponent<RectTransform>().sizeDelta.x, _progressImage.rectTransform.sizeDelta.y);
            }
        }
    }
    #endregion

    #region Canvas Action
    public override void ShowCanvas()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.mobileVibrater.Vibrate();
        }
        GetComponent<Canvas>().sortingOrder++;
        _showArea.GetComponent<RectTransform>().DOMoveX(0f, 0.25f).SetEase(Ease.InBack);
        _openSfxInstance.start();

        _quaverCnt.text = GamePlayData.Instance.remainQuaverCnt.ToString();
        _curAlbumCnt = GamePlayData.Instance.totalMusicCnt;
        SetAlbumProgress();

        _albumContent.GetComponent<AlbumScrollContent>().SetData();
        if (_musicListPopup.activeSelf)
            _musicListPopup.SetActive(false);
    }

    public override void HideCanvas()
    {
        _showArea.GetComponent<RectTransform>().DOMoveX(Screen.width, 0.25f).SetEase(Ease.OutBack);
        GetComponent<Canvas>().sortingOrder--;
    }
    #endregion
}
