using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class StoreItemUnit : MonoBehaviour
{
    [Header("----------------- UI -----------------")]
    [SerializeField]
    private Button _buyButton;

    [SerializeField]
    protected Text _coinCnt;
    [SerializeField]
    protected Text _price;
    [SerializeField]
    protected Text _priceUnit;

    protected string _storeItemID;

    public virtual void Start()
    {
        _buyButton.GetComponent<CodelessIAPButton>().productId = "";
        _buyButton.GetComponent<CodelessIAPButton>().productId = _storeItemID;
        _buyButton.onClick.AddListener(() =>
        {
            if (GamePlayData.Instance != null)
            {
                GamePlayData.Instance.OnClickBtnEffect();
            }

            IAPManager.Instance.endAction = () =>
            {
                GetBoughtItems();
            };

            DebugX.Log("_storeItemID: " + _storeItemID + " 구매");
        });

        _priceUnit.text = IAPManager.Instance.GetPriceUnit(_storeItemID);
    }

    public virtual void GetBoughtItems()
    {

    }
}
