using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectMusicUnit : MonoBehaviour
{
    [SerializeField]
    private CollectMusicSubUnit[] subUnits;

    public void SetData(List<CollectMusicInfo> musicInfos, GameObject playMusicPopup, GameObject musicListPopup)
    {
        for (int i = 0; i < subUnits.Length; i++)
        {
            subUnits[i].UpdateItem(musicInfos[i], playMusicPopup, musicListPopup);
        }
    }
}
