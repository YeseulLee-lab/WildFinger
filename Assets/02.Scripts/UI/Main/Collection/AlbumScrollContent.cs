using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InfiniteScroll))]
public class AlbumScrollContent : UIBehaviour, IInfiniteScrollSetup
{
    [SerializeField, Range(1, 999)]
    private int max;
    [SerializeField]
    private int subUnitCnt;
    [SerializeField]
    private TownData allTownDatas;
    [SerializeField]
    private GameObject musicListPopup;

    private List<AlbumInfo> _albumInfos = new List<AlbumInfo>();

    private void Start()
    {
        SetData();
    }

    public void SetData()
    {
        _albumInfos.Clear();

        for (int i = 0; i < allTownDatas.townDatas.Length; i++)
        {
            _albumInfos.Add(allTownDatas.townDatas[i].albumInfo);
            _albumInfos[i].albumName = allTownDatas.townDatas[i].townName;
            _albumInfos[i].townList = allTownDatas.townDatas[i].townType;
            _albumInfos[i].unlockLevel = allTownDatas.townDatas[i].levelAmount;
        }
        max = _albumInfos.Count / subUnitCnt;

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
            var item = obj.GetComponentInChildren<AlbumUnit>();
            //서브 유닛이 아닌 앨범 유닛에서 setdata를 하고 유닛에서 서브 유닛에 updateItem
            //TODO: 홀수일 경우 생각해야함.
            List<AlbumInfo> infos = new List<AlbumInfo>();
            for (int i = 0; i < subUnitCnt; i++)
            {
                infos.Add(_albumInfos[itemCount * subUnitCnt + i]);
            }
            item.SetData(infos, musicListPopup, SetData);
        }
    }
}