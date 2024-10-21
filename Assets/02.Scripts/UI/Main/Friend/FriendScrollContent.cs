using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InfiniteScroll))]

public class FriendScrollContent : UIBehaviour, IInfiniteScrollSetup
{
    [SerializeField, Range(1, 999)]
    private int max;

    private string[] _friendUidArr;

    private int _requestsCnt;

    public void SetData(string[] friendUidArr, int requestsCnt)
    {
        _requestsCnt = requestsCnt;

        _friendUidArr = friendUidArr;
        max = _friendUidArr.Length;

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
            var item = obj.GetComponentInChildren<FriendUnit>();
            if (itemCount < _requestsCnt)
            {
                item.SetRequestsUnitData(_friendUidArr[itemCount]);
            }
            else if(itemCount == _requestsCnt)
            {
                item.SetToFriendListObject();
            }
            else if (itemCount == max - 1)
            {
                item.SetToFindFriendObject();
            }
            else if(itemCount > _requestsCnt)
            {
                item.SetFriendsUnitData(_friendUidArr[itemCount]);
            }
        }
    }
}
