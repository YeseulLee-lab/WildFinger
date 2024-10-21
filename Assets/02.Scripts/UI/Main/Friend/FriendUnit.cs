using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendUnit : MonoBehaviour
{
    [SerializeField]
    private Image _profileImg;
    [SerializeField]
    private Text _name;
    [SerializeField]
    private Button _removeBtn;
    [SerializeField]
    private Text _maxStage;
    [SerializeField]
    private Text _email;

    [Header("--------------- Requests --------------")]
    [SerializeField]
    private GameObject _requestUnitBtnGroup;
    [SerializeField]
    private Button _noBtn;
    [SerializeField]
    private Button _yesBtn;

    [Header("--------------- Objects --------------")]
    [SerializeField]
    private GameObject _friendListTitle;
    [SerializeField]
    private GameObject _findFriendBtn;
    [SerializeField]
    private GameObject _unitObject;

    private FriendInfo _friendInfo;

    private void Start()
    {
        _removeBtn.onClick.AddListener(OnClickRemoveBtn);

        _noBtn.onClick.AddListener(OnClickNobtn);
        _yesBtn.onClick.AddListener(OnClickYesBtn);

        _findFriendBtn.GetComponent<Button>().onClick.AddListener(OnClickFindFriendBtn);
    }

    public void SetFriendsUnitData(string uid)
    {
        _requestUnitBtnGroup.SetActive(false);
        _removeBtn.gameObject.SetActive(true);

        SetUnitData(uid);
    }

    public void SetRequestsUnitData(string uid)
    {
        _requestUnitBtnGroup.SetActive(true);
        _removeBtn.gameObject.SetActive(false);

        SetUnitData(uid);
    }

    private void SetUnitData(string uid)
    {
        _unitObject.SetActive(true);
        _friendListTitle.SetActive(false);
        _findFriendBtn.SetActive(false);

        FirestoreManager.Instance.SearchFriend(uid, (info) =>
        {
            _friendInfo = info;
            _name.text = _friendInfo.name;
        });

        FirestoreManager.Instance.GetMaxStage(uid, (stage) =>
        {
            _maxStage.text = "LV " + stage.ToString();
        });

        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.addressableGroupLoader.DownloadProfileImg(uid, (tex) =>
            {
                Rect rect = new Rect(0, 0, tex.width, tex.height);
                _profileImg.sprite = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));
            });
        }
    }

    public void SetToFriendListObject()
    {
        _friendListTitle.SetActive(true);
        _findFriendBtn.SetActive(false);
        _unitObject.SetActive(false);
    }

    public void SetToFindFriendObject()
    {
        _friendListTitle.SetActive(false);
        _findFriendBtn.SetActive(true);
        _unitObject.SetActive(false);
    }

    private void OnClickRemoveBtn()
    {
        GamePlayData.Instance.twoButtonPopup.Show(Define.PopupTitle.ConfirmRemoveFriend,
        () =>
        {
            GamePlayData.Instance.ShowLoading();
            FirestoreManager.Instance.RemoveFriend(_friendInfo.uid, () =>
            {
                GamePlayData.Instance.HideLoading();
                GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.RemoveFriend);
                MainUIManager.Instance.friendCanvas.GetFriendList();
            });
        }, 
        () =>
        {

        });
        
    }

    private void OnClickNobtn()
    {
        FirestoreManager.Instance.RemoveFriend(_friendInfo.uid, null, "Receive");
    }

    private void OnClickYesBtn()
    {
        GamePlayData.Instance.ShowLoading();
        FirestoreManager.Instance.RemoveFriend(_friendInfo.uid, null, "Receive");
        FirestoreManager.Instance.AcceptRequest(_friendInfo.uid,
            (arr) =>
            {
                GamePlayData.Instance.HideLoading();
                GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.AcceptFriend);
                MainUIManager.Instance.friendCanvas.GetFriendList();
            });
    }

    private void OnClickFindFriendBtn()
    {
        MainUIManager.Instance.friendCanvas.OnClickFindFriend();
    }
}

