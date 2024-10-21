using DG.Tweening;
using HHK.UIEC;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayCurLevelPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject _popupArea;
    [SerializeField]
    private Sprite[] _noItemSp;
    [SerializeField]
    private Sprite[] _defaultItemSp;
    [SerializeField]
    private GameObject _bonusImage;
    [SerializeField]
    private GameObject _bonusCheckImage;

    [Header("-------------------- UI Area ---------------------")]
    [SerializeField]
    private Image _backPanel;
    [SerializeField]
    private GameObject _hardSubTitle;
    [SerializeField]
    private Sprite[] _panelSp;
    [SerializeField]
    private SpriteState[] _playBtnState;

    [Header("-------------------- Level Info ---------------------")]
    [SerializeField]
    private Button _playCurLevelExitButton;
    [SerializeField]
    private Button _playCurLevelButton;
    [SerializeField]
    private UnityEngine.UI.Text _playLevel;
    private LevelInfo _levelInfo;
    private TownInfo _townInfo;

    [Header("-------------------- Item Area ---------------------")]
    [SerializeField]
    private Toggle[] itemToggles;
    [SerializeField]
    private UnityEngine.UI.Text[] _itemCnts; //99개 이상이면 99로 표기
    [SerializeField]
    private GameObject[] _itemLockObjs; //shield, heart, recovery

    #region Unity Life Cycle
    private void Start()
    {
        _playCurLevelButton.onClick.AddListener(OnClickPlayLevelButton);
        _playCurLevelExitButton.onClick.AddListener(OnClickPlayExitButton);

        itemToggles[0].onValueChanged.AddListener((isOn) =>
        {
            if (GamePlayData.Instance.itemSheildCnt < 1 && isOn)
            {
                _popupArea.SetActive(false);
                GamePlayData.Instance.curTown = _townInfo.townType;
                GamePlayData.Instance.curStage = _levelInfo.level;
                GamePlayData.Instance.buyIngameItemPopup.ShowBuyInGameItemPopup(Define.UsingItemBeforeInGame.Shield, gameObject);
                itemToggles[0].isOn = false;
                return;
            }

            SetItem(Define.UsingItemBeforeInGame.Shield, isOn, 0);
        });

        itemToggles[1].onValueChanged.AddListener((isOn) =>
        {
            if (GamePlayData.Instance.itemIncreadeHPCnt < 1 && isOn)
            {
                _popupArea.SetActive(false);
                GamePlayData.Instance.curTown = _townInfo.townType;
                GamePlayData.Instance.curStage = _levelInfo.level;
                GamePlayData.Instance.buyIngameItemPopup.ShowBuyInGameItemPopup(Define.UsingItemBeforeInGame.IncreasedHP, gameObject);
                itemToggles[1].isOn = false;
                return;
            }

            SetItem(Define.UsingItemBeforeInGame.IncreasedHP, isOn, 1);
        });

        itemToggles[2].onValueChanged.AddListener((isOn) =>
        {
            if (GamePlayData.Instance.itemIncreasedHealingHPCnt < 1 && isOn)
            {
                _popupArea.SetActive(false);
                GamePlayData.Instance.curTown = _townInfo.townType;
                GamePlayData.Instance.curStage = _levelInfo.level;
                GamePlayData.Instance.buyIngameItemPopup.ShowBuyInGameItemPopup(Define.UsingItemBeforeInGame.IncreasedHealingHP, gameObject);
                itemToggles[2].isOn = false;
                return;
            }

            SetItem(Define.UsingItemBeforeInGame.IncreasedHealingHP, isOn, 2);
        });
    }
    #endregion

    #region UI Action
    public void ShowPopup(TownInfo townInfo, LevelInfo levelInfo)
    {
        _popupArea.SetActive(true);

        SelectLevel(townInfo, levelInfo);

        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }
        GamePlayData.Instance.InitItem();
        itemToggles[0].isOn = GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.Shield];
        itemToggles[1].isOn = GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.IncreasedHP];
        itemToggles[2].isOn = GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.IncreasedHealingHP];

        GetComponent<UIECAnimator>().OnCustomChannel();
    }

    public void HidePopup()
    {
        _popupArea.SetActive(false);
    }
    

    public void SelectLevel(TownInfo townInfo, LevelInfo levelInfo)
    {
        _levelInfo = levelInfo;
        _townInfo = townInfo;

        _playLevel.text = " " + _levelInfo.level.ToString();

        SetPanelContent(_levelInfo.level);

        //아이템 등장전까지는 선택못함
        if (GamePlayData.Instance.maxStage >= MainKey.increaseHPUnlockStage)
        {
            itemToggles[0].interactable = true;
            itemToggles[1].interactable = true;
            if (MainUIManager.Instance.tutorialCanvas.IsTutorialDone(Define.MainTutorialType.MainItemHPPotion))
            {
                itemToggles[2].interactable = true;
            }
            else
            {
                // 아이템 자물쇠 obj
                Sequence sequence = DOTween.Sequence();
                sequence.Join(_itemLockObjs[2].GetComponent<RectTransform>().DOShakeRotation(0.5f, 30f, 20).SetEase(Ease.OutCubic))
                    .Append(_itemLockObjs[2].GetComponent<RectTransform>().DOLocalMoveY(-50f, 0.2f).SetEase(Ease.InOutBack))
                    .Append(_itemLockObjs[2].GetComponent<Image>().DOFade(0, 0.5f));

                MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.MainItemHPPotion, () =>
                {
                    itemToggles[2].interactable = true;
                    _itemLockObjs[2].SetActive(false);

                    GamePlayData.Instance.itemIncreasedHealingHPCnt += 3;

                    SetItemCount();
                }, new RectTransform[] { itemToggles[2].GetComponent<RectTransform>() });
            }
        }
        else if (GamePlayData.Instance.maxStage >= MainKey.maxHealthUnlockStage)
        {
            itemToggles[0].interactable = true;
            if (MainUIManager.Instance.tutorialCanvas.IsTutorialDone(Define.MainTutorialType.MainItemIncreaseHP))
            {
                itemToggles[1].interactable = true;
            }
            else
            {
                // 아이템 자물쇠 obj
                Sequence sequence = DOTween.Sequence();
                sequence.Join(_itemLockObjs[1].GetComponent<RectTransform>().DOShakeRotation(0.5f, 30f, 20).SetEase(Ease.OutCubic))
                    .Append(_itemLockObjs[1].GetComponent<RectTransform>().DOLocalMoveY(-50f, 0.2f).SetEase(Ease.InOutBack))
                    .Append(_itemLockObjs[1].GetComponent<Image>().DOFade(0, 0.5f));

                MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.MainItemIncreaseHP, () =>
                {
                    itemToggles[1].interactable = true;
                    _itemLockObjs[1].SetActive(false);

                    GamePlayData.Instance.itemIncreadeHPCnt += 3;

                    SetItemCount();
                }, new RectTransform[] { itemToggles[1].GetComponent<RectTransform>() });
            }
        }
        else if (GamePlayData.Instance.maxStage >= MainKey.shieldUnlockStage)
        {
            if (MainUIManager.Instance.tutorialCanvas.IsTutorialDone(Define.MainTutorialType.MainItemShield))
            {
                itemToggles[0].interactable = true;
            }
            else
            {
                // 아이템 자물쇠 obj
                Sequence sequence = DOTween.Sequence();
                sequence.Join(_itemLockObjs[0].GetComponent<RectTransform>().DOShakeRotation(0.5f, 30f, 20).SetEase(Ease.OutCubic))
                    .Append(_itemLockObjs[0].GetComponent<RectTransform>().DOLocalMoveY(-50f, 0.2f).SetEase(Ease.InOutBack))
                    .Append(_itemLockObjs[0].GetComponent<Image>().DOFade(0, 0.5f));

                MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.MainItemShield, () =>
                {
                    itemToggles[0].interactable = true;
                    _itemLockObjs[0].SetActive(false);

                    GamePlayData.Instance.itemSheildCnt += 3;

                    SetItemCount();
                }, new RectTransform[] { itemToggles[0].GetComponent<RectTransform>() });
            }
        }

        // 아이템 자물쇠 obj
        _itemLockObjs[0].SetActive(!itemToggles[0].interactable);
        _itemLockObjs[1].SetActive(!itemToggles[1].interactable);
        _itemLockObjs[2].SetActive(!itemToggles[2].interactable);

        //수 표기
        SetItemCount();
    }

    private void SetItemCount()
    {
        if (GamePlayData.Instance.itemSheildCnt == 0)
        {
            itemToggles[0].targetGraphic.GetComponent<Image>().sprite = _noItemSp[0];
            _itemCnts[0].text = string.Empty;
        }
        else
        {
            itemToggles[0].targetGraphic.GetComponent<Image>().sprite = _defaultItemSp[0];
            _itemCnts[0].text = GamePlayData.Instance.itemSheildCnt > 99 ? "99+" : GamePlayData.Instance.itemSheildCnt.ToString();
        }


        if (GamePlayData.Instance.itemIncreadeHPCnt == 0)
        {
            itemToggles[1].targetGraphic.GetComponent<Image>().sprite = _noItemSp[1];
            _itemCnts[1].text = string.Empty;
        }
        else
        {
            itemToggles[1].targetGraphic.GetComponent<Image>().sprite = _defaultItemSp[1];
            _itemCnts[1].text = GamePlayData.Instance.itemIncreadeHPCnt > 99 ? "99+" : GamePlayData.Instance.itemIncreadeHPCnt.ToString();
        }


        if (GamePlayData.Instance.itemIncreasedHealingHPCnt == 0)
        {
            itemToggles[2].targetGraphic.GetComponent<Image>().sprite = _noItemSp[2];
            _itemCnts[2].text = string.Empty;
        }
        else
        {
            itemToggles[2].targetGraphic.GetComponent<Image>().sprite = _defaultItemSp[2];
            _itemCnts[2].text = GamePlayData.Instance.itemIncreasedHealingHPCnt > 99 ? "99+" : GamePlayData.Instance.itemIncreasedHealingHPCnt.ToString();
        }
    }

    private void SetPanelContent(int level)
    {
        int index = Convert.ToInt32(level != (GamePlayData.Instance.GetStackLevels(_townInfo) + GamePlayData.Instance.maxTownInfo.levelAmount));

        _hardSubTitle.SetActive(level == (GamePlayData.Instance.GetStackLevels(_townInfo) + GamePlayData.Instance.maxTownInfo.levelAmount));
        _backPanel.sprite = _panelSp[index];
        _playCurLevelButton.spriteState = _playBtnState[index];
        _playCurLevelButton.GetComponent<Image>().sprite = _playBtnState[index].selectedSprite;

        //Check Bonus Stage => 보너스에선 아이템 사용 불가능
        _bonusImage.SetActive(SceneSwitcher.IsBonusStage(level));
        if (SceneSwitcher.IsBonusStage(level))
        {
            _bonusCheckImage.SetActive(PlayerPrefs.GetInt(EncryptedKey.isAllPerfect + level.ToString()) == 1);
        }
    }
    #endregion

    #region OnClick
    private void OnClickPlayExitButton()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }
        HidePopup();
    }

    private void OnClickPlayLevelButton()
    {
        GamePlayData.Instance.OnClickBtnEffect();

        if (GamePlayData.Instance.heartTimer.heartCnt > 0)
        {
            //Check Item Usage State
            if (itemToggles[0].isOn)
            {
                GamePlayData.Instance.itemSheildCnt--;
            }
            if (itemToggles[1].isOn)
            {
                GamePlayData.Instance.itemIncreadeHPCnt--;
            }
            if (itemToggles[2].isOn)
            {
                GamePlayData.Instance.itemIncreasedHealingHPCnt--;
            }

            //Init Game Try State
            GamePlayData.Instance.inGameTryCnt = 1;

            if (_levelInfo != null)
            {

                SceneSwitcher.Instance.SwitchGameScene(SceneSwitcher.GetTownList(_levelInfo.level), _levelInfo.level);
            }
            else
            {
                SceneSwitcher.Instance.SwitchGameScene(SceneSwitcher.GetTownList(GamePlayData.Instance.maxStage), GamePlayData.Instance.maxStage);
            }
        }   
        else
        {
            //하트 충전 팝업
            GamePlayData.Instance.noHeartPopup.ShowPopup();
        }
    }

    private void SetItem(Define.UsingItemBeforeInGame usingItemBeforeInGame, bool isOn, int idx)
    {
        GamePlayData.Instance.OnClickBtnEffect();

        _itemCnts[idx].gameObject.SetActive(!isOn);
        GamePlayData.Instance.SetItemState(usingItemBeforeInGame, isOn);
    }
    #endregion
}
