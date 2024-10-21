using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinStoreItemUnit : StoreItemUnit
{
    [Header("----------------- Data Area -----------------")]
    public CoinStoreItem _itemData;

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
    }

    public override void GetBoughtItems()
    {
        DebugX.Log("코인 구매함");
        GamePlayData.Instance.coinCnt += _itemData._coinCnt;

        //storecanvas - SetCoinCount()
        GamePlayData.Instance.storeCanvas.SetCoinCount();
        MainUIManager.Instance.mainCanvas.SetWealthData();
    }
}
