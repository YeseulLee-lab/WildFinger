using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Gley.MobileAds;
using TMPro;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

public class AdmobManager : MonoBehaviour
{
    private int _adChargingCnt;
    public int adChargingCnt
    {
        get { return _adChargingCnt; }
        set
        {
            if (value > MainKey.adMaxChargingCnt)
            {
                value = MainKey.adMaxChargingCnt;
            }
            else if (value < 0)
            {
                value = 0;
            }
            _adChargingCnt = value;
            PlayerPrefs.SetInt(EncryptedKey.adCharingCnt, _adChargingCnt);
            DebugX.Log("adChargingCnt: " + adChargingCnt);
        }
    }
    private CancellationTokenSource _cts;
    private CancellationToken _ct;
    public UnityAction<DateTime> adTimerUpdateAction { get; set; } = null;
    public DateTime adFullChargingTime
    {
        get { return DateTime.ParseExact(PlayerPrefs.GetString(EncryptedKey.adFullChargingTime, DateTime.Now.ToString("yyyyMMddHHmmss")), "yyyyMMddHHmmss", null); }
        set
        {
            PlayerPrefs.SetString(EncryptedKey.adFullChargingTime, value.ToString("yyyyMMddHHmmss"));
            //DebugX.Log("adFullChargingTime: " + adFullChargingTime);
        }
    }

    #region Unity Life Cycle
    private void Start()
    {
        API.Initialize();
    }
    #endregion

    #region Advertisement API
    public void ShowBanner()
    {
        API.ShowBanner(BannerPosition.Bottom, BannerType.Adaptive);
    }

    public void HideBanner()
    {
        API.HideBanner();
    }

    public void ShowRewardVideo(UnityAction completeAction = null)
    {
        //Check?

        API.ShowRewardedInterstitial((isComplete) => { if (isComplete) completeAction.Invoke(); });
    }
    #endregion

    #region Actions
    public bool IsAvailable() => API.IsInitialized();

    public void InitFirst(int isFirst)
    {
        if (isFirst == 0)
        {
            adChargingCnt = MainKey.adMaxChargingCnt;
        }
        else
        {
            _adChargingCnt = PlayerPrefs.GetInt(EncryptedKey.adCharingCnt);
        }

        _ = StartAdTimerAsync();
    }

    public void SetAdChargingFullTime(int min = MainKey.adChargingCycleMin)
    {
        if(adChargingCnt <= 1)
        {
            adFullChargingTime = adFullChargingTime.AddMinutes(min);
        }
        else if(adChargingCnt == 2)
        {
            adFullChargingTime = DateTime.Now.AddMinutes(min);
        }
        adChargingCnt--;
    }

    /// <summary>
    /// 인터넷에 연결되어있는지 주기적으로 확인
    /// </summary>
    /// <param name="cycleSec">확인 하는 주기(초)</param>
    public async void CheckInternetState(int cycleSec)
    {
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;

        if (GamePlayData.Instance == null)
        {
            GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.UnexpectedError, true);
            return;
        }

        while (true)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.NoInternetConnection, true);
                await UniTask.Delay(2000, cancellationToken: _ct);
                GamePlayData.Instance.toastPopup.HideToastMessage();
                //TODO: 로그인 씬 이동
                SceneSwitcher.Instance.SwitchScene(Define.SceneName.Login);
            }
            try
            {
                await UniTask.Delay(cycleSec * 1000, cancellationToken: _ct);
            }
            catch (OperationCanceledException)
            {
                // 이전 작업이 취소되면 예외 발생, 무시
                DebugX.Log("이전 작업 취소됨");
            }
        }
    }

    public async UniTaskVoid StartAdTimerAsync()
    {
        //DebugX.Log("adChargingCnt: " + adChargingCnt);

        while (true)
        {
            adTimerUpdateAction?.Invoke(adFullChargingTime);
            await UniTask.WaitUntil(() => adChargingCnt < MainKey.adMaxChargingCnt);
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _ct); // 1초마다 실행
        }
    }
    #endregion
}
