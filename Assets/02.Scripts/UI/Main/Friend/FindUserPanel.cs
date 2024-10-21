using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindUserPanel : MonoBehaviour
{
    [SerializeField]
    private Button _background;
    [SerializeField]
    private Button _closeButton;
    [SerializeField]
    private InputField _uidInputField;
    [SerializeField]
    private Button _searchBtn;
    [SerializeField]
    private Text _myUid;
    [SerializeField]
    private Button _copyBtn;

    [Header("------------- Found User -------------")]
    [SerializeField]
    private GameObject _foundUserObj;
    [SerializeField]
    private Text _foundUserName;
    [SerializeField]
    private Text _foundUserEmail;
    [SerializeField]
    private Button _addBtn;

    private FriendInfo _friendInfo;

    private void Start()
    {
#if UNITY_EDITOR
        _myUid.text = "My UID: " + "PCTestAccount";
#elif UNITY_ANDROID
        _myUid.text = "My UID: " + Social.localUser.id;
#endif

        _background.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        _closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        _searchBtn.onClick.AddListener(OnClickSearchBtn);
        _addBtn.onClick.AddListener(OnClickFriendSendBtn);
        _copyBtn.onClick.AddListener(OnClickCopyBtn);
    }

    private void OnDisable()
    {
        _uidInputField.text = string.Empty;
        _foundUserName.text = string.Empty;
        _foundUserObj.SetActive(false);
    }

    private void OnClickSearchBtn()
    {
        if (_uidInputField.text.Equals(string.Empty))
        {
            GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.NoSearchText);
        }
        else
        {
            GamePlayData.Instance.ShowLoading();
            FirestoreManager.Instance.SearchFriend(_uidInputField.text, (info) =>
            {
                GamePlayData.Instance.HideLoading();
                if (info != null)
                    SetFoundUserData(info);
                _foundUserObj.SetActive(true);
            }, () =>
            {
                GamePlayData.Instance.HideLoading();
                GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.NoExistentUser);
            });
        }
    }

    private void SetFoundUserData(FriendInfo friendInfo)
    {
        _friendInfo = friendInfo;
        _foundUserName.text = friendInfo.name;
    }

    private void OnClickFriendSendBtn()
    {
        if (_friendInfo != null)
        {
            GamePlayData.Instance.ShowLoading();
            FirestoreManager.Instance.SendFriendRequest(_friendInfo.uid, () =>
            {
                GamePlayData.Instance.HideLoading();
                _foundUserObj.SetActive(false);
            });
        }
    }

    private void OnClickCopyBtn()
    {
        string copyText;
#if UNITY_EDITOR
        copyText = "PCTestAccount";
#elif UNITY_ANDROID
        copyText = Social.localUser.id;
#endif
        GUIUtility.systemCopyBuffer = copyText;
        GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.CopyToClipboard);
    }
}
