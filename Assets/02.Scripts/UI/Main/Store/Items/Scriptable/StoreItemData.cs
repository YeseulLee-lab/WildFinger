using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StoreItemData : ScriptableObject
{
    [SerializeField]
    private CoinStoreItem[] _coinStoreItems;
    [SerializeField]
    private SpecialStoreItem[] _specialStoreItems;
    [SerializeField]
    private SpecialHeartStoreItem[] _specialHeartStoreItems;

    public CoinStoreItem[] coinStoreItems => _coinStoreItems;
    public SpecialStoreItem[] specialStoreItems => _specialStoreItems;
    public SpecialHeartStoreItem[] specialHeartStoreItems => _specialHeartStoreItems;
}
