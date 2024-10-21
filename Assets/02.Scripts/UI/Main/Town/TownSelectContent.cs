using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InfiniteScroll))]
public class TownSelectContent : UIBehaviour, IInfiniteScrollSetup
{
    [SerializeField, Range(1, 999)]
    private int max;
    [SerializeField]
    private TownData allTownDatas;

    private List<TownInfo> _towns = new List<TownInfo>();

    private void Start()
    {
        SetData();
    }

    public void SetData()
    {
        _towns.Clear();

        for (int i = 0; i < allTownDatas.townDatas.Length; i++)
        {
            _towns.Add(allTownDatas.townDatas[i]);
        }
        max = _towns.Count;

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
        if(MainUIManager.Instance.tutorialCanvas.IsTutorialDone(Define.MainTutorialType.LandPage1))
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, infiniteScroll.itemScale * (int)GamePlayData.Instance.maxTown);
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
            var item = obj.GetComponentInChildren<TownUnit>();
            item.UpdateItem(itemCount, _towns[itemCount]);
        }
    }
}
