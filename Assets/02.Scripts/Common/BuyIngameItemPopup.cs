using Coffee.UIExtensions;
using FMOD.Studio;
using FMODUnity;
using HHK.UIEC;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BuyIngameItemPopup : MonoBehaviour
{
    [SerializeField]
    private Button _background;
    [SerializeField]
    private GameObject _popupArea;
    [SerializeField]
    private Button _buyButton;
    [SerializeField]
    private Unmask _unmask;
    [SerializeField]
    private MainCoinManager _coinWealthObject;
    
    [Header("--------- Set UI --------")]
    [SerializeField]
    private UnityEngine.UI.Text _itemTitle;
    [SerializeField]
    private UnityEngine.UI.Text _itemDesc;
    [SerializeField]
    private Image _itemImg;
    [SerializeField]
    private UnityEngine.UI.Text _itemPrice;
    [SerializeField]
    private Sprite[] _itemSprites;

    [Header("------------------ SFX Area -----------------")]
    [SerializeField]
    private EventReference _buySfx;
    private EventInstance _buySfxInstance;

    private Define.UsingItemBeforeInGame _selectedItem;
    private GameObject _callPopup;

    #region Unity Life Cycle
    private void Awake()
    {
        _buySfxInstance = RuntimeManager.CreateInstance(_buySfx);
    }

    private void Start()
    {
        if (GamePlayData.Instance != null)
        {
            _buySfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }

        _buyButton.onClick.AddListener(OnClickBuy);

        _itemPrice.text = MainKey.inGameItemPrice.ToString();
    }

    private void OnDestroy()
    {
        _buySfxInstance.setUserData(IntPtr.Zero);
        _buySfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _buySfxInstance.release();
    }
    #endregion

    #region Buy In Game Item
    public void ShowBuyInGameItemPopup(Define.UsingItemBeforeInGame gameItem, GameObject callPopup)
    {
        _buySfxInstance.start();
        
        if (callPopup.GetComponent<GameRetry>())
        {
            //RETRY
            _callPopup = callPopup;

            _coinWealthObject.gameObject.SetActive(true);
            _coinWealthObject.SetCoinData();

            _background.onClick.AddListener(() =>
            {
                HideBuyInGameItemPopup();
                callPopup.GetComponent<GameRetry>().HidePopup();
            });
        }
        else if(callPopup.GetComponent<PlayCurLevelPanel>())
        {
            //MAIN
            _callPopup = callPopup;

            if (MainUIManager.Instance != null)
            {
                //unmask 위치
                if (MainUIManager.Instance.SelectLevelCanvas.gameObject.activeSelf)
                {
                    _unmask.fitTarget = MainUIManager.Instance.mainCanvas.coinManager.GetComponent<RectTransform>();
                }
            }

            _background.onClick.AddListener(() =>
            {
                HideBuyInGameItemPopup();
                LevelInfo levelInfo = new LevelInfo();
                levelInfo.level = GamePlayData.Instance.curStage;
                callPopup.GetComponent<PlayCurLevelPanel>().ShowPopup(GamePlayData.Instance.GetTownInfo(GamePlayData.Instance.curTown), levelInfo);
            });
        }

        _selectedItem = gameItem;
        //setdata
        _itemTitle.text = _itemTitle.GetComponent<LocalizationTextUI>().GetSummary(gameItem.ToString());
        _itemDesc.text = _itemTitle.GetComponent<LocalizationTextUI>().GetSummary(gameItem.ToString() + "Desc");
        _itemImg.sprite = _itemSprites[(int)gameItem];

        _coinWealthObject.SetCoinData();

        _popupArea.SetActive(true);

        GetComponent<UIECAnimator>().OnCustomChannel();
    }

    public void HideBuyInGameItemPopup()
    {
        _popupArea.SetActive(false);
        _background.GetComponent<Button>().onClick.RemoveAllListeners();
    }
    #endregion

    private void OnClickBuy()
    {
        if (GamePlayData.Instance.coinCnt >= MainKey.inGameItemPrice)
        {
            _buySfxInstance.start();
            switch(_selectedItem)
            {
                case Define.UsingItemBeforeInGame.Shield:
                    GamePlayData.Instance.itemSheildCnt += 3;
                    break;
                case Define.UsingItemBeforeInGame.IncreasedHP:
                    GamePlayData.Instance.itemIncreadeHPCnt += 3;
                    break;
                case Define.UsingItemBeforeInGame.IncreasedHealingHP:
                    GamePlayData.Instance.itemIncreasedHealingHPCnt += 3;
                    break;
            }
            
            GamePlayData.Instance.coinCnt -= MainKey.inGameItemPrice;
            if (MainUIManager.Instance != null)
            {
                MainUIManager.Instance.mainCanvas.SetWealthData();

                LevelInfo levelInfo = new LevelInfo();
                levelInfo.level = GamePlayData.Instance.curStage;
                _callPopup.GetComponent<PlayCurLevelPanel>().ShowPopup(GamePlayData.Instance.GetTownInfo(GamePlayData.Instance.curTown), levelInfo);
            }
            else
            {
                _coinWealthObject.GetComponent<MainCoinManager>().SetCoinData();
                _callPopup.GetComponent<GameRetry>().ShowPopup();
            }
                
            HideBuyInGameItemPopup();
        }
        else
        {
            GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.NoCoinTitle, true);
            HideBuyInGameItemPopup();
            GamePlayData.Instance.storeCanvas.ShowCanvas();
        }
    }
}
