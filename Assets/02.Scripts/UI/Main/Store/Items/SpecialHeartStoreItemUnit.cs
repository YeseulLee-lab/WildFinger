using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialHeartStoreItemUnit : StoreItemUnit
{
    [Header("----------------- Special Heart Area -----------------")]
    [SerializeField]
    private Text _itemName;
    [SerializeField]
    private Text _shieldCnt;
    [SerializeField]
    private Text _potionCnt;
    [SerializeField]
    private Text _meatCnt;
    [SerializeField]
    private Text _infiniteHeartTime;

    [Header("----------------- Data Area -----------------")]
    public SpecialHeartStoreItem _itemData;

    public override void Start()
    {
        _storeItemID = _itemData._storeItemID;

        base.Start();

        SetData();
    }

    public void SetData()
    {
        _coinCnt.text = _itemData._coinCnt.ToString();
        _price.text = IAPManager.Instance.GetPrice(_itemData._storeItemID);

        _itemName.text = _itemName.GetComponent<LocalizationTextUI>().GetSummary(_itemData._storeItemID);
        _shieldCnt.text = _itemData._shieldCnt.ToString();
        _potionCnt.text = _itemData._potionCnt.ToString();
        _meatCnt.text = _itemData._meatCnt.ToString();
        _infiniteHeartTime.text = _itemData._infiniteHeartTime.ToString() + "시간";
    }

    public override void GetBoughtItems()
    {
        DebugX.Log("하트 스페셜 아이템 구매함");
        GamePlayData.Instance.coinCnt += _itemData._coinCnt;
        GamePlayData.Instance.itemSheildCnt += _itemData._shieldCnt;
        GamePlayData.Instance.itemIncreadeHPCnt += _itemData._potionCnt;
        GamePlayData.Instance.itemIncreasedHealingHPCnt += _itemData._meatCnt;
        //하트 무한 시간
        GamePlayData.Instance.heartTimer.SetInfiniteHeartTime(_itemData._infiniteHeartTime * 60);

        //storecanvas - SetCoinCount()
        GamePlayData.Instance.storeCanvas.SetCoinCount();
        MainUIManager.Instance.mainCanvas.SetWealthData();
    }
}
