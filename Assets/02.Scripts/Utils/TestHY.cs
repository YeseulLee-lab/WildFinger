using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using HHK.UIEC;
using System.Threading;
using Cysharp.Threading.Tasks;

public class TestHY : MonoBehaviour
{
    [SerializeField]
    private LandLoadingPanel _loadingPanel;
    [SerializeField]
    private Button _oneBtn;
    [SerializeField]
    private Button _twoBtn;

    [SerializeField]
    private Image _landImg;
    [SerializeField]
    private Sprite[] _landImgs;

    private void Start()
    {
        // _loadingPanel.Show(UnityAction<float> progress = null, UnityAction DownloadComplete = null)를 호출하려는데,
        _landImg.sprite = _landImgs[0];
        _oneBtn?.onClick.AddListener(() =>
        {
            _landImg.sprite = _landImgs[0];
            _loadingPanel.Show(progress: ProgressCallback, DownloadComplete);
            StartProgressUpdate(); // 3초 후에 progress를 1로 설정하는 작업 시작
        });

        _twoBtn?.onClick.AddListener(() =>
        {
            _loadingPanel.Hide();
        });
    }

    private async void StartProgressUpdate()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(5));
        ProgressCallback(1f); // 3초 후에 progress를 1로 설정
    }

    private void ProgressCallback(float progress)
    {
        // 이 메서드는 LandLoadingPanel의 Show 메서드에서 호출됩니다.
        Debug.Log($"Progress: {progress}");
    }

    private void DownloadComplete()
    {
        // 다운로드 완료 시 호출되는 콜백
        Debug.Log("다운로드 완료");
        _landImg.sprite = _landImgs[1];
    }
}
