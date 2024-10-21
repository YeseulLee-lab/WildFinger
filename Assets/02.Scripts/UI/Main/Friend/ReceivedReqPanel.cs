using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceivedReqPanel : MonoBehaviour
{
    [SerializeField]
    private Button _background;
    [SerializeField]
    private ReceiveScrollContent _receiveScrollContent;

    private void Start()
    {
        _background.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void OnEnable()
    {
        GamePlayData.Instance.ShowLoading();
        FirestoreManager.Instance.GetReceivedRequestList((arr) =>
        {
            GamePlayData.Instance.HideLoading();
            _receiveScrollContent.SetData(arr);
        });
    }
}
