using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreStoreContent : MonoBehaviour
{
    [Header("----------------- Store Unit Prefabs -----------------")]
    [SerializeField]
    private RectTransform _coinStoreItem;
    [SerializeField]
    private RectTransform _specialHeartStoreItem;
    [SerializeField]
    private RectTransform _specialStoreItem;
    [SerializeField]
    private RectTransform _normalStoreItem;

    [SerializeField]
    private StoreItemData _storeItemData;

    private void Start()
    {
        for (int i = 0; i < _storeItemData.specialStoreItems.Length; i++)
        {
            if (_storeItemData.specialStoreItems[i].isSpecialist)
            {
                _specialStoreItem.GetComponent<SpecialStoreItemUnit>()._itemData = _storeItemData.specialStoreItems[i];
                Instantiate(_specialStoreItem, transform);
            }
            else
            {
                _normalStoreItem.GetComponent<SpecialStoreItemUnit>()._itemData = _storeItemData.specialStoreItems[i];
                Instantiate(_normalStoreItem, transform);
            }
        }

        for (int i = 0; i < _storeItemData.specialHeartStoreItems.Length; i++)
        {
            _specialHeartStoreItem.GetComponent<SpecialHeartStoreItemUnit>()._itemData = _storeItemData.specialHeartStoreItems[i];
            Instantiate(_specialHeartStoreItem, transform);
        }

        for (int i = 0; i < _storeItemData.coinStoreItems.Length; i++)
        {
            _coinStoreItem.GetComponent<CoinStoreItemUnit>()._itemData = _storeItemData.coinStoreItems[i];
            Instantiate(_coinStoreItem, transform);
        }

        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x,
            _specialHeartStoreItem.sizeDelta.y * _storeItemData.specialStoreItems.Length
            + _specialStoreItem.sizeDelta.y * _storeItemData.specialHeartStoreItems.Length
            + _coinStoreItem.sizeDelta.y * _storeItemData.coinStoreItems.Length);
    }
}
