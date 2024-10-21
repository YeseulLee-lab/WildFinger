using Coffee.UIExtensions;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class CollectMusicSubUnit : MonoBehaviour
{
    [SerializeField]
    private CollectMusicUnitImageSet[] collectMusicUnitImageSets; //0: 해금됨, 1: 잠김

    [Header("---------------- UI Area ----------------")]
    [SerializeField]
    private Image _background;
    [SerializeField]
    private Image _musicThumb;
    [SerializeField]
    private Image _bottomImg;
    [SerializeField]
    private GameObject _imageHolder;
    [SerializeField]
    private GameObject _needQuaver;
    [SerializeField]
    private UnityEngine.UI.Text _needQuaverCnt;
    [SerializeField]
    private UnityEngine.UI.Text _musicName;
    [SerializeField]
    private int _maxTextLength;

    [Header("---------------- Button ----------------")]
    [SerializeField]
    private Button _unlockBtn;
    public Button unlockBtn { get{ return _unlockBtn;} }

    [Header("---------------- Collect Effect ----------------")]
    [SerializeField]
    private GameObject _quaverPrefab;

    [Header("------------------ SFX Area -----------------")]
    [SerializeField]
    private EventReference _unlockSfx;
    private EventInstance _unlockSfxInstance;

    private PlayMusicPopup _playMusicPopup;
    private GameObject _musicListPopup;
    private CollectMusicInfo _info;

    #region Unity Life Cycle
    private void Awake()
    {
        _unlockSfxInstance = RuntimeManager.CreateInstance(_unlockSfx);
    }

    private void Start()
    {
        if (GamePlayData.Instance != null)
        {
            _unlockSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }

        GetComponent<Button>().onClick.AddListener(OnClickUnit);
        _unlockBtn.onClick.AddListener(OnClickUnlockButton);
    }

    private void OnDestroy()
    {
        _unlockSfxInstance.setUserData(IntPtr.Zero);
        _unlockSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _unlockSfxInstance.release();
    }
    #endregion

    public void UpdateItem(CollectMusicInfo info, GameObject playMusicPopup, GameObject musicListPopup)
    {
        _info = info;
        _playMusicPopup = playMusicPopup.GetComponent<PlayMusicPopup>();
        _musicListPopup = musicListPopup;
        if (_musicName.GetComponent<LocalizationTextUI>().GetSummary(info.musicId).Length >= _maxTextLength)
        {
            _musicName.text = _musicName.GetComponent<LocalizationTextUI>().GetSummary(info.musicId).Substring(0, _maxTextLength) + "...";
        }
        else
        {
            _musicName.text = _musicName.GetComponent<LocalizationTextUI>().GetSummary(info.musicId);
        }
        
        if (PlayerPrefs.GetInt(EncryptedKey.musicCollect + info.townList.ToString() + info.uuid) > 0)
        {
            //해금된 음악이면
            if (PlayerPrefs.GetInt(EncryptedKey.musicCollect + _info.townList.ToString() + _info.uuid + UnencryptedKey.hasPlayed) < 1)
            {
                _musicThumb.GetComponent<Animator>().enabled = true;
                _musicThumb.GetComponent<ShinyEffectForUGUI>().enabled = true;
            }
            else
            {
                _musicThumb.GetComponent<Animator>().enabled = false;
                _musicThumb.GetComponent<ShinyEffectForUGUI>().enabled = false;
            }
            
            _musicThumb.sprite = info.collectMusicImage;
            _imageHolder.SetActive(true);
            _needQuaver.SetActive(false);
            _unlockBtn.gameObject.SetActive(false);
            _unlockBtn.interactable = false;
            GetComponent<Button>().interactable = true;

            _background.sprite = collectMusicUnitImageSets[0]._backGroundSP;            
            _bottomImg.sprite = collectMusicUnitImageSets[0]._bottomSP;
            _musicName.color = collectMusicUnitImageSets[0]._textColor;
            _musicName.GetComponent<NicerOutline>().effectColor = collectMusicUnitImageSets[0]._outlineColor;
        }
        else
        {
            //잠겨있으면
            _imageHolder.SetActive(false);
            _needQuaver.SetActive(true);
            _needQuaverCnt.text = info.needQuaver.ToString();
            _unlockBtn.gameObject.SetActive(true);
            _unlockBtn.interactable = true;
            GetComponent<Button>().interactable = false;

            _background.sprite = collectMusicUnitImageSets[1]._backGroundSP;
            _bottomImg.sprite = collectMusicUnitImageSets[1]._bottomSP;
            _musicName.color = collectMusicUnitImageSets[1]._textColor;
            _musicName.GetComponent<NicerOutline>().effectColor = collectMusicUnitImageSets[1]._outlineColor;
        }
    }

    private void OnClickUnit()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }

        if (PlayerPrefs.GetInt(EncryptedKey.musicCollect + _info.townList.ToString() + _info.uuid) > 0)
        {
            //재생해봤는지 여부
            PlayerPrefs.SetInt(EncryptedKey.musicCollect + _info.townList.ToString() + _info.uuid + UnencryptedKey.hasPlayed, 1);
            //해금된 음악이면 음악 재생
            _musicThumb.GetComponent<Animator>().enabled = false;
            _musicThumb.GetComponent<ShinyEffectForUGUI>().enabled = false;
            _playMusicPopup.gameObject.SetActive(true);
            //_musicListPopup.gameObject.SetActive(false);
            _playMusicPopup.SetData(_info);
        }
        else
        {
            DebugX.Log("노래 잠김!!");
        }
    }

    private void OnClickUnlockButton()
    {
        //해금팝업, 음표 사용
        DebugX.Log("해금하기");
        if (GamePlayData.Instance.remainQuaverCnt >= _info.needQuaver)
        {
            //다른 클릭 막기
            MainUIManager.Instance.collectionCanvas._blockImage.SetActive(true);
            _unlockBtn.interactable = false;
            GetComponent<Button>().interactable = true;
            //메인 캔버스 음표 업데이트
            GamePlayData.Instance.remainQuaverCnt -= _info.needQuaver;
            //해금 여부
            PlayerPrefs.SetInt(EncryptedKey.musicCollect + _info.townList.ToString() + _info.uuid, 1);
            //해금된 앨범 개수
            PlayerPrefs.SetInt(EncryptedKey.musicCollect + _info.townList.ToString(), PlayerPrefs.GetInt(EncryptedKey.musicCollect + _info.townList.ToString()) + 1);
            if (PlayerPrefs.GetInt(EncryptedKey.musicCollect + _info.townList.ToString()) == GamePlayData.Instance.GetTownInfo(Define.TownList.ToyTown).albumInfo.collectMusics.Length)
            {
                GamePlayData.Instance.totalAlbumCnt++;
            }
            //모은 음악 총개수 업데이트
            GamePlayData.Instance.totalMusicCnt ++;

            MainUIManager.Instance.mainCanvas.quaverManager.SetQuaverData();

            MainUIManager.Instance.collectionCanvas.GetComponent<CollectingCoinManager>().UseWealth(_info.needQuaver, _needQuaver.transform.GetChild(0).GetComponent<RectTransform>(), _needQuaverCnt, () =>
            {
                MainUIManager.Instance.collectionCanvas._blockImage.SetActive(false);

                _musicListPopup.GetComponent<MusicListPopup>().SetProgress();
                _musicListPopup.GetComponent<MusicListPopup>().UpdateQuaverText();

                //ui update
                _imageHolder.SetActive(true);
                _musicThumb.sprite = _info.collectMusicImage;
                _musicThumb.color = new Color(1f, 1f, 1f, 0f);
                _unlockSfxInstance.start();
                _musicThumb.DOFade(1f, 0.5f).OnComplete(() =>
                {
                    _musicThumb.GetComponent<Animator>().enabled = true;
                    _musicThumb.GetComponent<ShinyEffectForUGUI>().enabled = true;
                });
                
                _needQuaver.SetActive(false);
                _unlockBtn.gameObject.SetActive(false);

                _background.sprite = collectMusicUnitImageSets[0]._backGroundSP;
                _bottomImg.sprite = collectMusicUnitImageSets[0]._bottomSP;
                _musicName.color = collectMusicUnitImageSets[0]._textColor;
                _musicName.GetComponent<NicerOutline>().effectColor = collectMusicUnitImageSets[0]._outlineColor;

                _needQuaverCnt.GetComponent<RectTransform>().localScale = Vector3.one;

                #region Tutorial CollectionPage2
                if (!MainUIManager.Instance.tutorialCanvas.IsTutorialDone(Define.MainTutorialType.CollectionPage2))
                {
                    _musicListPopup.GetComponent<MusicListPopup>().collectMusicContent.transform.parent.parent.GetComponent<ScrollRect>().vertical = false;

                    MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.CollectionPage2, () =>
                    {
                        _musicListPopup.GetComponent<MusicListPopup>().collectMusicContent.transform.parent.parent.GetComponent<ScrollRect>().vertical = true;
                    },
                    new RectTransform[] { GetComponent<RectTransform>() });
                }
                #endregion
            });
        }
        else
        {
            GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.NoQuaver, true);
        }
    }
}

[Serializable]
public class CollectMusicUnitImageSet
{
    public Color _textColor;
    public Color _outlineColor;
    public Sprite _backGroundSP;
    public Sprite _bottomSP;
}