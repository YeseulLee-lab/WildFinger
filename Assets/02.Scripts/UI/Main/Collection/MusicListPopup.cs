using DG.Tweening;
using HHK.UIEC;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MusicListPopup : MonoBehaviour
{
    [SerializeField]
    private Button _closeButton;
    [SerializeField]
    private Button _background;
    [SerializeField]
    private UnityEngine.UI.Text _albumTitle;
    [SerializeField]
    private CollectMusicScrollContent _collectMusicContent;
    public CollectMusicScrollContent collectMusicContent => _collectMusicContent;
    [SerializeField]
    private GameObject _completeObject;
    [SerializeField]
    private RectTransform _progressImage;
    [SerializeField]
    private UnityEngine.UI.Text _quaverText;
    [SerializeField]
    private UnityEngine.UI.Text _progressText;
    [Header("---------------- Collecting Effect Area -------------")]
    [SerializeField]
    private RectTransform _quaverPopPos;
    [SerializeField]
    private RectTransform _quaverImage;
    [SerializeField]
    private RectTransform _unmaskPos;

    private AlbumInfo _albumInfo;

    public UnityAction _refresh;

    private void Start()
    {
        _closeButton.onClick.AddListener(() =>
        {
            if (GamePlayData.Instance != null)
            {
                GamePlayData.Instance.OnClickBtnEffect();
            }
            gameObject.SetActive(false);
            MainUIManager.Instance.collectionCanvas.SetAlbumProgress();
            _refresh.Invoke();
        });

        _background.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            MainUIManager.Instance.collectionCanvas.SetAlbumProgress();
            _refresh.Invoke();
        });
    }

    private void OnEnable()
    {
        //열릴때 애니메이션
        GetComponent<UIECAnimator>().OnCustomChannel();
        _quaverPopPos.position = _quaverImage.position;
        if(MainUIManager.Instance != null)
            _unmaskPos.position = MainUIManager.Instance.collectionCanvas._quaverArea.position;
    }

    private void OnDisable()
    {
        _progressImage.sizeDelta = new Vector2(0f, _progressImage.sizeDelta.y);
    }

    public void SetData(AlbumInfo info)
    {
        _albumInfo = info;

        _albumTitle.text = _albumTitle.GetComponent<LocalizationTextUI>().GetSummary(info.albumName);
        collectMusicContent.SetData(info);

        if (PlayerPrefs.GetInt(EncryptedKey.musicCollect + info.townList.ToString()) >= MainKey.musicCntEachLand)
        {
            //콜렉션 완성
            _completeObject.SetActive(true);
        }
        else
        {
            _completeObject.SetActive(false);

            SetProgress();
        }
    }

    public void UpdateQuaverText()
    {
        _quaverText.text = GamePlayData.Instance.remainQuaverCnt.ToString();
    }

    public void SetProgress()
    {
        float ratio = (float)(PlayerPrefs.GetInt(EncryptedKey.musicCollect + _albumInfo.townList.ToString())) / MainKey.musicCntEachLand;
        _progressImage.DOSizeDelta(new Vector2(ratio * _progressImage.transform.parent.GetComponent<RectTransform>().rect.width, _progressImage.sizeDelta.y), 0.3f, true)
            .OnComplete(() =>
            {
                if (ratio >= 1f)
                {
                    _completeObject.SetActive(true);
                }
            });

        _progressText.text = PlayerPrefs.GetInt(EncryptedKey.musicCollect + _albumInfo.townList.ToString()) + "/" + _albumInfo.collectMusics.Length;
    }
}
