using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceivedUnit : MonoBehaviour
{
    [SerializeField]
    private Text _username;
    [SerializeField]
    private Button _noBtn;
    [SerializeField]
    private Button _yesBtn;

    private FriendInfo _userInfo;
    private ReceiveScrollContent _parentPanel;

    private void Start()
    {
        _noBtn.onClick.AddListener(OnClickNobtn);
        _yesBtn.onClick.AddListener(OnClickYesBtn);
    }

    private void OnDisable()
    {
        _noBtn.interactable = false;
        _yesBtn.interactable = false;
    }

    private void OnClickNobtn()
    {
        FirestoreManager.Instance.RemoveFriend(_userInfo.uid, null, "Receive");
    }

    private void OnClickYesBtn()
    {
        GamePlayData.Instance.ShowLoading();
        FirestoreManager.Instance.RemoveFriend(_userInfo.uid, null, "Receive");
        FirestoreManager.Instance.AcceptRequest(_userInfo.uid, 
            (arr) =>
            {
                GamePlayData.Instance.HideLoading();
                GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.AcceptFriend);
                MainUIManager.Instance.friendCanvas.GetFriendList();
            });
    }

    public void SetUnitData(string id, ReceiveScrollContent parent)
    {
        _parentPanel = parent;

        FirestoreManager.Instance.SearchFriend(id, (info) =>
        {
            _userInfo = info;
            _username.text = info.name;

            _noBtn.interactable = true;
            _yesBtn.interactable = true;
        });
    }
}
