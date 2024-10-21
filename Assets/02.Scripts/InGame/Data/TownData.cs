using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TownData : ScriptableObject
{
    //Out Game
    [SerializeField]
    private TownInfo[] _townDatas;

    public TownInfo[] townDatas => _townDatas;
}
