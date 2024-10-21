using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TownMusicData : ScriptableObject
{
    //In Game
    [SerializeField]
    private Define.TownList _town;
    public Define.TownList town => _town;
    [SerializeField]
    private MusicInfo[] _musicDatas;
    public MusicInfo[] musicDatas => _musicDatas;
}
