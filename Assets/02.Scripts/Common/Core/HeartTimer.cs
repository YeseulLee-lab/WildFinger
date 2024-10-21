using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HeartTimer : MonoBehaviour
{
    [Header("----------------- Heart Data -----------------")]
    private int _heartCnt;
    public int heartCnt
    {
        get { return _heartCnt; }
        set
        {
            if (isInfiniteHeartTimeMode)
            {
                //하트 소모x
                _heartCnt = OutGameInfo.maxHeartCnt;
                PlayerPrefs.SetInt(EncryptedKey.heartCnt, _heartCnt);
               DebugX.Log("무한 하트 모드");
                return;
            }

            if (value >= OutGameInfo.maxHeartCnt)
            {
                value = OutGameInfo.maxHeartCnt;

                heartFullAction?.Invoke();
                totalRemainHeartTimerSec = OutGameInfo.remainHeartSec;
            }
            else if (value < 0)
            {
                value = 0;
            }

            if(value > _heartCnt)
            {
                heartChargingAction?.Invoke();
            }

            _heartCnt = value;
            PlayerPrefs.SetInt(EncryptedKey.heartCnt, _heartCnt);
        }
    }
    private int _totalRemainHeartTimerSec;
    public int totalRemainHeartTimerSec { get { return _totalRemainHeartTimerSec; } set {
            _totalRemainHeartTimerSec = value;
            PlayerPrefs.SetInt(EncryptedKey.remainHeartSec, value);
            //DebugX.Log("_totalRemainHeartTimerSec: " + value);
        } }
    private DateTime _lastLoginTime;
    public DateTime lastLoginTime { get { return _lastLoginTime; } set {
            _lastLoginTime = value;
            PlayerPrefs.SetString(EncryptedKey.lastLoginTime, _lastLoginTime.ToString("yyyyMMddHHmmss"));
        } }
    public DateTime infiniteHeartTime
    {
        get { return DateTime.ParseExact(PlayerPrefs.GetString(EncryptedKey.infiniteHeartTime, DateTime.Now.ToString("yyyyMMddHHmmss")), "yyyyMMddHHmmss", null); }
        set
        {
            PlayerPrefs.SetString(EncryptedKey.infiniteHeartTime, value.ToString("yyyyMMddHHmmss"));
            //DebugX.Log("infiniteHeartTime: " + infiniteHeartTime);
        }
    }
    private bool _isInfiniteHeartTimeMode;
    public bool isInfiniteHeartTimeMode
    {
        get { return _isInfiniteHeartTimeMode; }
        set
        {
            _isInfiniteHeartTimeMode = value;
            PlayerPrefs.SetInt(EncryptedKey.isInfiniteHeartTimeMode, value? 1 : 0);
        }
    }

    [Header("----------------- Setting -----------------")]
    private CancellationTokenSource _cts;
    private CancellationToken _ct;
    public UnityAction heartFullAction { get; set; } = null;
    public UnityAction heartChargingAction { get; set; } = null;
    public UnityAction heartCntUpdateAction { get; set; } = null; // totalRemainHeartTimerSec가 업데이트 될 때마다 호출
    public UnityAction infiniteHeartTimerModeStartAction { get; set; } = null;
    public UnityAction infiniteHeartTimerModeEndAction { get; set; } = null;
    private bool _isAppFirstOn = false;

    #region Unity Life Cycle
    private void Awake()
    {
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    private void OnApplicationQuit()
    {
    }

    private void OnApplicationFocus(bool focus)
    {
        //DebugX.Log("OnApplicationFocus: " + focus);
        if (focus)
        {
            if (!_isAppFirstOn)
            {
                _isAppFirstOn = true;
                return;
            }

            CheckPrevHeartTimer();
        }
    }
    #endregion

    #region Heart Timer
    public async UniTaskVoid StartHeartTimerAsync()
    {
        infiniteHeartTimerModeEndAction?.Invoke();
        if (heartCnt >= OutGameInfo.maxHeartCnt)
        {
            totalRemainHeartTimerSec = 0;
        }

        while (true)
        {
            await UniTask.WaitUntil(() => heartCnt < OutGameInfo.maxHeartCnt);
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _ct); // 1초마다 실행
            totalRemainHeartTimerSec--; // 1초씩 감소
            lastLoginTime = DateTime.Now;

            // totalRemainHeartTimerSec 업데이트 이벤트 호출
            if (heartCnt < OutGameInfo.maxHeartCnt)
            {
                heartCntUpdateAction?.Invoke();
            }

            if (totalRemainHeartTimerSec <= 0)
            {
                heartCnt++; // 하트를 1개 추가
              totalRemainHeartTimerSec = OutGameInfo.remainHeartSec; // 다음 충전까지 남은 시간 초기화
            }
        }
    }

    public void InitFirst(int isFirst)
    {
        if (isFirst == 0)
        {
            // 하트 데이터 초기화, 어플 첫 실행
            heartCnt = OutGameInfo.maxHeartCnt;
            isInfiniteHeartTimeMode = false;
        }
        else
        {
            // 현재 하트 수 가져오기, 시간 반영되어야 함
            _isInfiniteHeartTimeMode = PlayerPrefs.GetInt(EncryptedKey.isInfiniteHeartTimeMode) != 0;
            LoadHeartData();
        }

        // 하트 타이머 시작
        _ = StartHeartTimerAsync();
       
        if (isInfiniteHeartTimeMode)
        {
            _ = StartInfiniteHeartTimerAsync();
        }
    }

    private void LoadHeartData()
    {
        // 저장된 하트 데이터 불러오기
        _heartCnt = PlayerPrefs.GetInt(EncryptedKey.heartCnt, OutGameInfo.maxHeartCnt);

        CheckPrevHeartTimer();
    }

    private void CheckPrevHeartTimer()
    {
        if(heartCnt >= OutGameInfo.maxHeartCnt)
        {
            return;
        }

        string lastLoginTimeStr = PlayerPrefs.GetString(EncryptedKey.lastLoginTime, DateTime.Now.ToString("yyyyMMddHHmmss"));
        lastLoginTime = DateTime.ParseExact(lastLoginTimeStr, "yyyyMMddHHmmss", null);
        TimeSpan timeSinceLastLogin = DateTime.Now - _lastLoginTime;
        totalRemainHeartTimerSec = PlayerPrefs.GetInt(EncryptedKey.remainHeartSec, 0);

        int heartToAdd = (int)(timeSinceLastLogin.TotalSeconds / OutGameInfo.remainHeartSec); // 600초(10분)마다 하트 추가
        heartCnt += heartToAdd; // 이전 접속 이후의 시간 동안 충전된 하트 추가

        // 전체 하트 충전에 남은 시간 업데이트
        totalRemainHeartTimerSec -= (int)timeSinceLastLogin.TotalSeconds;
        if (totalRemainHeartTimerSec < 0)
        {
            totalRemainHeartTimerSec = 0;
        }
        lastLoginTime = DateTime.Now;
    }
    
    public string GetHeartLocalizedFullText()
    {
        if (GamePlayData.Instance == null)
        {
            return "Full";
        }

        if (GamePlayData.Instance.tableData.localizationDic.TryGetValue("Full", out LocalizationInfo info))
        {
            return info.summary;
        }

        return string.Empty;
    }
    #endregion

    #region Inifinite Heart Mode
    /// <summary>
    /// TODO: 해당 시간(분) 만큼 하트 소모 안됨.
    /// 
    /// </summary>
    /// <param name="min"></param>
    public void SetInfiniteHeartTime(int min = 15)
    {
        if (!isInfiniteHeartTimeMode)
        {
            isInfiniteHeartTimeMode = true;
            infiniteHeartTime = DateTime.Now;
            _ = StartInfiniteHeartTimerAsync();
        }

        infiniteHeartTime = infiniteHeartTime.AddMinutes(min);
    }

    public async UniTaskVoid StartInfiniteHeartTimerAsync()
    {
        heartCnt = 5;
        while (true)
        {
            //DebugX.Log("무한하트 카운트: " + totalRemainHeartTimerSec);
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _ct); // 1초마다 실행
            infiniteHeartTimerModeStartAction?.Invoke();
            TimeSpan timeSinceInfiniteHeartTimeMode = infiniteHeartTime - DateTime.Now;
            totalRemainHeartTimerSec = (int)timeSinceInfiniteHeartTimeMode.TotalSeconds < 0 ? 0 : (int)timeSinceInfiniteHeartTimeMode.TotalSeconds;
            heartCntUpdateAction?.Invoke();

            if (totalRemainHeartTimerSec <= 0)
            {
                infiniteHeartTimerModeEndAction?.Invoke();
                totalRemainHeartTimerSec = 0;
                isInfiniteHeartTimeMode = false;
                heartCnt = 5;
                return;
            }
            
        }
    }
    #endregion
}
