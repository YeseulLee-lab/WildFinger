using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AlbumUnit : MonoBehaviour
{
    [SerializeField]
    private AlbumSubUnit[] subUnits;

    public void SetData(List<AlbumInfo> albumInfos, GameObject musicListPopup, UnityAction _refrsh)
    {
        for (int i = 0; i < subUnits.Length; i++)
        {
            subUnits[i].UpdateItem(albumInfos[i], musicListPopup, _refrsh);
        }
    }
}
