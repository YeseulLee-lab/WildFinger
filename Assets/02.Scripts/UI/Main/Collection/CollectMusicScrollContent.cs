using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InfiniteScroll))]
public class CollectMusicScrollContent : UIBehaviour, IInfiniteScrollSetup
{
    [SerializeField, Range(1, 999)]
    private int max;
    [SerializeField]
    private int subUnitCnt;
    [SerializeField]
    private GameObject playMusicPopup;
    [SerializeField]
    private GameObject musicListPopup;

    private List<CollectMusicInfo> _collectMusicInfos = new List<CollectMusicInfo>();

    public void SetData(AlbumInfo albumInfo)
    {
        _collectMusicInfos.Clear();

        for (int i = 0; i < albumInfo.collectMusics.Length; i++)
        {
            albumInfo.collectMusics[i].townList = albumInfo.townList;
            _collectMusicInfos.Add(albumInfo.collectMusics[i]);
        }
        max = _collectMusicInfos.Count / subUnitCnt;

        InfiniteScroll infiniteScroll = GetComponent<InfiniteScroll>();
        infiniteScroll.Init();
    }

    public void OnPostSetupItems()
    {
        var infiniteScroll = GetComponent<InfiniteScroll>();
        infiniteScroll.onUpdateItem.AddListener(OnUpdateItem);
        GetComponentInParent<ScrollRect>().movementType = ScrollRect.MovementType.Elastic;

        var rectTransform = GetComponent<RectTransform>();
        var delta = rectTransform.sizeDelta;
        rectTransform.anchoredPosition = new Vector2(0f, 0f);
        delta.y = infiniteScroll.itemScale * max;
        rectTransform.sizeDelta = delta;
    }

    public void OnUpdateItem(int itemCount, GameObject obj)
    {
        if (itemCount < 0 || itemCount >= max)
        {
            obj.SetActive(false);
        }
        else
        {
            obj.SetActive(true);
            var item = obj.GetComponentInChildren<CollectMusicUnit>();
            //서브 유닛이 아닌 앨범 유닛에서 setdata를 하고 유닛에서 서브 유닛에 updateItem
            //TODO: 홀수일 경우 생각해야함.
            List<CollectMusicInfo> infos = new List<CollectMusicInfo>();
            for (int i = 0; i < subUnitCnt; i++)
            {
                infos.Add(_collectMusicInfos[itemCount * subUnitCnt + i]);
            }
            item.SetData(infos, playMusicPopup, musicListPopup);
        }
    }
}
