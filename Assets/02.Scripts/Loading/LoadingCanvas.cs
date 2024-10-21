using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using HHK.UIEC;
using System.Threading;
using Cysharp.Threading.Tasks;

public class LoadingCanvas : MonoBehaviour
{
    [Header("------------------ GUI Setting -----------------")]
    [SerializeField]
    private UnityEngine.UI.Text _loadingText;
    [SerializeField]
    private Image loadingPanel;
    [SerializeField]
    private UIECAnimator[] _rspAnims; // Rock Scissor Paper
    [SerializeField]
    private LoadingArrowAnim[] _loadingArrowAnims; // 3개씩, RSP

    [Header("------------------ Setting -----------------")]
    private const int _rspAnimDelayMS = 350;
    private const int _arrowDelayMS = 150;
    private const int _loadingTextDelayMS = 300;
    private CancellationTokenSource _cts;
    private CancellationToken _ct;

    #region Unity Life Cycle
    private void Awake()
    {
    }

    private void OnEnable()
    {
        if(GamePlayData.Instance != null)
        {
            GamePlayData.Instance.HideLoading();
            GamePlayData.Instance.toastPopup.ForcedHideToastMessage();
        }

        StartLoadingImgAnim();
        StartLoadingTextAnim();
    }

    private void Start()
    {
        SceneSwitcher.Instance.SwitchScene();
    }

    private void OnDisable()
    {
    }

    private void OnDestroy()
    {
        if (_cts != null)
        {
            _cts.Cancel();
        }
    }
    #endregion

    private async void StartLoadingImgAnim()
    {
        int index = 0;
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;

        try
        {
            while (true)
            {
                if (index >= _rspAnims.Length)
                {
                    index = 0;
                }

                if(_rspAnims[index] != null)
                    _rspAnims[index].OnCustomChannel();
                await UniTask.Delay(_rspAnimDelayMS, cancellationToken: _ct);

                for (int i = 0; i < _loadingArrowAnims[index].arrowAnims.Length; i++)
                {
                    if(_loadingArrowAnims[index].arrowAnims[i] != null)
                        _loadingArrowAnims[index].arrowAnims[i].OnCustomChannel();
                    await UniTask.Delay(_arrowDelayMS, cancellationToken: _ct);
                }

                index++;
            }
        }
        catch (OperationCanceledException)
        {
            // Task was canceled, exit gracefully
        }
    }

    private async void StartLoadingTextAnim()
    {
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;
        string[] loadingStates = new string[] { "Loading", "Loading .", "Loading . .", "Loading . . ." };
        int stateIndex = 0;

        try
        {
            while (true)
            {
                _loadingText.text = loadingStates[stateIndex];
                stateIndex = (stateIndex + 1) % loadingStates.Length;
                await UniTask.Delay(_loadingTextDelayMS, cancellationToken: _ct);
            }
        }
        catch (OperationCanceledException)
        {
            // Task was canceled, exit gracefully
        }
    }
}

[Serializable]
public class LoadingArrowAnim
{
    public UIECAnimator[] arrowAnims = new UIECAnimator[3];
}