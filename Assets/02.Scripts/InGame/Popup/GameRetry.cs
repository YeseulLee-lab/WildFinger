using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
using HHK.UIEC;
using FMODUnity;
using FMOD.Studio;

public class GameRetry : BaseGameRetry
{
    [Header("-------------------- Item Area ---------------------")]
    [SerializeField]
    private Toggle _itemShieldToggle;
    [SerializeField]
    private Toggle _itemHeartToggle;
    [SerializeField]
    private Toggle _itemRecoveryToggle;
    [SerializeField]
    private UnityEngine.UI.Text _itemShieldCntText;
    [SerializeField]
    private UnityEngine.UI.Text _itemHeartCntText;
    [SerializeField]
    private UnityEngine.UI.Text _itemRecoveryCntText;
    [SerializeField]
    private GameObject[] _itemLockObjs; //shield, heart, recovery

    #region Unity Life Cycle
    public override void OnEnable()
    {
        base.OnEnable();

        if(GamePlayData.Instance == null)
        {
            return;
        }
    }

    public override void Start()
    {
        base.Start();

        _itemShieldToggle.onValueChanged.AddListener((isOn) => {
            if (GamePlayData.Instance != null)
            {
                if (GamePlayData.Instance.itemSheildCnt < 1 && isOn)
                {
                    GamePlayData.Instance.buyIngameItemPopup.ShowBuyInGameItemPopup(Define.UsingItemBeforeInGame.Shield, gameObject);
                    _itemShieldToggle.isOn = false;
                    return;
                }
            }
            base.ShowBtnClickSFX();
            OnItemToggleStateChanged(isOn, _itemShieldCntText.gameObject, Define.UsingItemBeforeInGame.Shield);
        });
        _itemHeartToggle.onValueChanged.AddListener((isOn) => {
            if (GamePlayData.Instance != null)
            {
                if (GamePlayData.Instance.itemIncreadeHPCnt < 1 && isOn)
                {
                    GamePlayData.Instance.buyIngameItemPopup.ShowBuyInGameItemPopup(Define.UsingItemBeforeInGame.IncreasedHP, gameObject);
                    _itemHeartToggle.isOn = false;
                    return;
                }
            }
            base.ShowBtnClickSFX();
            OnItemToggleStateChanged(isOn, _itemHeartCntText.gameObject, Define.UsingItemBeforeInGame.IncreasedHP); });
        _itemRecoveryToggle.onValueChanged.AddListener((isOn) => {
            if (GamePlayData.Instance != null)
            {
                if (GamePlayData.Instance.itemIncreasedHealingHPCnt < 1 && isOn)
                {
                    GamePlayData.Instance.buyIngameItemPopup.ShowBuyInGameItemPopup(Define.UsingItemBeforeInGame.IncreasedHealingHP, gameObject);
                    _itemRecoveryToggle.isOn = false;
                    return;
                }
            }
            base.ShowBtnClickSFX();
            OnItemToggleStateChanged(isOn, _itemRecoveryCntText.gameObject, Define.UsingItemBeforeInGame.IncreasedHealingHP);
        });

        InitToggle(TownDataLoader.isTraining);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        //TODO: 리소스 할당 초기화
    }

    private void InitToggle(bool isTraining)
    {
        if (GamePlayData.Instance == null)
        {
            return;
        }

        if (isTraining)
        {
            _itemShieldToggle.interactable = false;
            _itemHeartToggle.interactable = false;
            _itemRecoveryToggle.interactable = false;
        }
        else
        {
            _itemShieldToggle.interactable = (GamePlayData.Instance.maxStage >= MainKey.shieldUnlockStage);
            _itemHeartToggle.interactable = (GamePlayData.Instance.maxStage >= MainKey.maxHealthUnlockStage);
            _itemRecoveryToggle.interactable = (GamePlayData.Instance.maxStage >= MainKey.increaseHPUnlockStage);
        }

        _itemLockObjs[0].SetActive(!_itemShieldToggle.interactable);
        _itemLockObjs[1].SetActive(!_itemHeartToggle.interactable);
        _itemLockObjs[2].SetActive(!_itemRecoveryToggle.interactable);
    }
    #endregion

    #region UI Action
    public override void ShowPopup()
    {
        base.ShowPopup();
        BeatGridTracker.SetGameState(Define.InGameState.Paused);

        if (GamePlayData.Instance == null)
        {
            return;
        }

        GamePlayData.Instance.getQuaverCnt = 0;

        _itemShieldToggle.isOn = GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.Shield];
        _itemHeartToggle.isOn = GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.IncreasedHP];
        _itemRecoveryToggle.isOn = GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.IncreasedHealingHP];
        OnItemToggleStateChanged(GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.Shield], _itemShieldCntText.gameObject, Define.UsingItemBeforeInGame.Shield);
        OnItemToggleStateChanged(GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.IncreasedHP], _itemShieldCntText.gameObject, Define.UsingItemBeforeInGame.IncreasedHP);
        OnItemToggleStateChanged(GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.IncreasedHealingHP], _itemShieldCntText.gameObject, Define.UsingItemBeforeInGame.IncreasedHealingHP);

        GamePlayData.Instance.heartTimer.heartCnt--;
        GamePlayData.Instance.getCoinCnt = 0;

        //TODO: 아이템 소지 수에 맞게 item Cnt Text 조정
        _itemShieldCntText.text = GamePlayData.Instance.itemSheildCnt > 99 ? "99+" : GamePlayData.Instance.itemSheildCnt.ToString();
        _itemHeartCntText.text = GamePlayData.Instance.itemIncreadeHPCnt > 99 ? "99+" : GamePlayData.Instance.itemIncreadeHPCnt.ToString();
        _itemRecoveryCntText.text = GamePlayData.Instance.itemIncreasedHealingHPCnt > 99 ? "99+" : GamePlayData.Instance.itemIncreasedHealingHPCnt.ToString();
    }

    public override void OnClickRetryBtn()
    {
        base.SetInteractable(false);

        if (GamePlayData.Instance == null)
        {
            return;
        }

        if (GamePlayData.Instance.heartTimer.heartCnt <= 0 && !TownDataLoader.isTraining)
        {
            GamePlayData.Instance.noHeartPopup.ShowPopup();
            return;
        }

        //Check Item Usage State
        if (_itemShieldToggle.isOn)
        {
            GamePlayData.Instance.itemSheildCnt--;
        }
        if (_itemHeartToggle.isOn)
        {
            GamePlayData.Instance.itemIncreadeHPCnt--;
        }
        if (_itemRecoveryToggle.isOn)
        {
            GamePlayData.Instance.itemIncreasedHealingHPCnt--;
        }

        base.OnClickRetryBtn();
    }

    private void OnItemToggleStateChanged(bool isOn, GameObject shieldCntTextObj, Define.UsingItemBeforeInGame item)
    {
        shieldCntTextObj.SetActive(!isOn);

        if(GamePlayData.Instance == null)
        {
            return;
        }

        GamePlayData.Instance.SetItemState(item, isOn);
    }
    #endregion
}
