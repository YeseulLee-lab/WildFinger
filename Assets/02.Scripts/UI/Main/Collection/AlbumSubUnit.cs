using Coffee.UIEffects;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class AlbumSubUnit : MonoBehaviour
{
    [SerializeField]
    private AlbumUnitImageSet[] albumUnitImageSets; //0: 완료 1: 진행중 2: 잠김

    [Header("------------- UI Area -------------")]
    [SerializeField]
    private Image _background;
    [SerializeField]
    private Image _cdFront;
    [SerializeField]
    private Image _nameBackground;
    [SerializeField]
    private Text _albumName;
    [SerializeField]
    private UIShadow _albumNameShadow;
    [SerializeField]
    private Text _albumLevel;
    [SerializeField]
    private GameObject _imageHolder;
    [SerializeField]
    private Image _albumImage;

    [Header("------------- Progressing -------------")]
    [SerializeField]
    private RectTransform _progressImage;
    [SerializeField]
    private Text _progressText;

    private MusicListPopup _musicListPopup;
    private AlbumInfo _info;
    private UnityAction _albumScrollRefresh;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickAlbum);
    }

    public void UpdateItem(AlbumInfo info, GameObject musicListPopup, UnityAction _refresh)
    {
        _musicListPopup = musicListPopup.GetComponent<MusicListPopup>();
        _info = info;
        _albumScrollRefresh = _refresh;

        //아직 해금 안됨
        if (info.townList > GamePlayData.Instance.maxTown)
        {
            GetComponent<Button>().interactable = false;
            SetUI(2);
            _albumLevel.text = "LV." + GamePlayData.Instance.GetStackLevels(GamePlayData.Instance.GetTownInfo(info.townList)) + "-" + (GamePlayData.Instance.GetStackLevels(GamePlayData.Instance.GetTownInfo(info.townList)) + info.unlockLevel);
        }
        //진행중, 완성
        else
        {
            GetComponent<Button>().interactable = true;
            //완성하였으면
            //YSYS
            if (PlayerPrefs.GetInt(EncryptedKey.musicCollect + info.townList.ToString()) >= MainKey.musicCntEachLand)
            {
                SetUI(0);
            }
            //진행중
            else
            {
                float ratio = (float)(PlayerPrefs.GetInt(EncryptedKey.musicCollect + info.townList.ToString())) / MainKey.musicCntEachLand;
                _progressImage.sizeDelta = new Vector2(ratio * _progressImage.transform.parent.GetComponent<RectTransform>().rect.width, _progressImage.sizeDelta.y);

                _progressText.text = PlayerPrefs.GetInt(EncryptedKey.musicCollect + info.townList.ToString()) + "/" + info.collectMusics.Length;

                SetUI(1);
            }
        }
        _albumName.text = _albumName.GetComponent<LocalizationTextUI>().GetSummary(info.albumName);
        _albumImage.sprite = info.albumSp;
    }

    private void SetUI(int idx)
    {
        #region reset
        //reset
        for (int i = 0; i < 3; i++)
        {
            albumUnitImageSets[i]._bottomObject.SetActive(false);
        }
        _imageHolder.SetActive(true);
        _cdFront.gameObject.SetActive(false);
        #endregion

        if (albumUnitImageSets[idx]._frontSP != null)
        {
            _cdFront.gameObject.SetActive(true);
            _cdFront.sprite = albumUnitImageSets[idx]._frontSP;
        }
        else
        {
            _imageHolder.SetActive(false);
        }
            
        _background.sprite = albumUnitImageSets[idx]._backgroundSP;
        _nameBackground.sprite = albumUnitImageSets[idx]._nameBackgroundSP;
        _albumName.GetComponent<Outline>().effectColor = albumUnitImageSets[idx]._textOutlineColor;
        _albumNameShadow.effectColor = albumUnitImageSets[idx]._textShadowColor;
        _albumName.GetComponent<Gradient2>().EffectGradient = albumUnitImageSets[idx]._textEffectGradient;
        albumUnitImageSets[idx]._bottomObject.SetActive(true);
    }

    private void OnClickAlbum()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }
        //음악리스트 패널 활성화
        _musicListPopup.gameObject.SetActive(true);
        _musicListPopup.SetData(_info);
        
        _musicListPopup._refresh = _albumScrollRefresh;
        _musicListPopup._refresh += () => UpdateItem(_info, _musicListPopup.gameObject, null);
    }
}

[Serializable]
public class AlbumUnitImageSet
{
    public Sprite _backgroundSP;
    public Sprite _frontSP;
    public Sprite _nameBackgroundSP;
    public Color _textOutlineColor;
    public Color _textShadowColor;
    public UnityEngine.Gradient _textEffectGradient = new UnityEngine.Gradient() { colorKeys = new GradientColorKey[] { new GradientColorKey(Color.black, 0), new GradientColorKey(Color.white, 1) } };
    public GameObject _bottomObject;
}