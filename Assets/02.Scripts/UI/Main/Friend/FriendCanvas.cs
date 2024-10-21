using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendCanvas : BaseMainCanvas
{
    [Header("---------------- Friend Canvas Area ------------------")]
    [SerializeField]
    private GameObject _findUserPanel;
    [SerializeField]
    private FriendScrollContent _friendScrollContent;
    public FriendScrollContent friendScrollContent
    {
        get
        {
            return _friendScrollContent;
        }
    }
    [SerializeField]
    private GameObject _noFriendPanel;

    private int friendCnt;
    private int requestCnt;

    #region Unity Life Cycle
    public override void Start()
    {
        base.Start();
    }

    public override void ShowCanvas()
    {
        base.ShowCanvas();
        GetFriendList();
    }
    #endregion

    public void GetFriendList()
    {
        GamePlayData.Instance.ShowLoading();
        FirestoreManager.Instance.GetFriendList((friends) =>
        {
            friendCnt = friends.Length;
            FirestoreManager.Instance.GetReceivedRequestList((requests) =>
            {
                requestCnt = requests.Length;

                string[] arr = new string[friends.Length + requests.Length + 2];

                for (int i = 0; i < requests.Length; i++)
                {
                    arr[i] = requests[i];
                }
                for (int i = 0; i < friends.Length; i++)
                {
                    arr[i + requests.Length + 1] = friends[i];
                } 

                GamePlayData.Instance.HideLoading();
                if (arr.Length > 0)
                {
                    _noFriendPanel.SetActive(false);
                    _friendScrollContent.SetData(arr, requestCnt);
                }
                else
                {
                    _noFriendPanel.SetActive(true);
                }
            });
        });
    }

    public void OnClickFindFriend()
    {
        _findUserPanel.SetActive(true);
    }
}
