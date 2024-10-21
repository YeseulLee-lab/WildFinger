 using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntroVideoPlayer : MonoBehaviour
{
    [SerializeField]
    private Button _nextBtn;
    [SerializeField]
    private Text nextText;

    private VideoSwap videoSwap;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        videoSwap = GetComponent<VideoSwap>();

        videoSwap.OnVideoEnd = () =>
        {
            nextText.gameObject.SetActive(true);
        };
        videoSwap.OnLastVideo = () =>
        {
            //마지막 영상 - 인게임 진입
            SceneSwitcher.Instance.SwitchScene(Define.SceneName.Game);
        };

        _nextBtn.onClick.AddListener(OnClickNextBtn);
    }

    private void OnClickNextBtn()
    {
        nextText.gameObject.SetActive(false);
        videoSwap.NextVideo();
    }
}
