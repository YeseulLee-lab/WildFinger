using UnityEngine;
using UnityEngine.UI;
using HHK.UIEC;

public class GameNoHeart : InGameBasePopup
{
    [Header("------------------ NoHeart Area -----------------")]
    [SerializeField]
    private GameObject _popupArea;
    [SerializeField]
    private MainHeartManager _heartManager;
    [SerializeField]
    private UnityEngine.UI.Text _heartTimeText;
    [SerializeField]
    private Button _showAdBtn;
    [SerializeField]
    private Button _closeBtn;
    [SerializeField]
    private Button _refillBtn;
    [SerializeField]
    private UnityEngine.UI.Text _refillCoinText;

    #region Unity Life Cycle
    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Start()
    {
        base.Start();

        _showAdBtn?.onClick.AddListener(OnClickShowAdBtn);
        _closeBtn?.onClick.AddListener(OnClickCloseBtn); //닫으면 메인으로 돌아감
        _refillBtn?.onClick.AddListener(OnClickRefillBtn);

        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.heartTimer.heartChargingAction += AlertChargingHeartState;
        }
    }

    private void OnDestroy()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.heartTimer.heartChargingAction -= AlertChargingHeartState;
        }
    }
    #endregion

    #region UI Action
    public override void OnClickBlackPanelBtn()
    {
        OnClickCloseBtn();
    }

    public override void ShowPopup()
    {
        SetInteractable(true);
        base.ShowPopup();
        _popupArea.SetActive(true);
        this.GetComponent<UIECAnimator>().OnCustomChannel();

        //TODO: retry 횟수를 세어서 _refillCoinText.text 갯수 입력
        if(GamePlayData.Instance == null)
        {
            return;
        }
        _refillCoinText.text = GetRetryCoinCnt(GamePlayData.Instance.inGameTryCnt).ToString();
    }

    public void HidePopup()
    {
        _popupArea.SetActive(false);
    }

    private void OnClickCloseBtn()
    {
        SetInteractable(false);
        base.ShowBtnClickSFX();
        HidePopup();
    }

    private void OnClickShowAdBtn()
    {
        //TODO: 광고보기
        SetInteractable(false);
        base.ShowBtnClickSFX();

        if (GamePlayData.Instance == null)
        {
            SetInteractable(true);
            return;
        }

        if (!GamePlayData.Instance.admobManager.IsAvailable())
        {
            GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.AdUnavailable, true);
            SetInteractable(true);
            return;
        }

        if(GamePlayData.Instance.admobManager.adChargingCnt > 0)
        {
            GamePlayData.Instance.admobManager.ShowRewardVideo(() => {
                SetInteractable(true);
                HidePopup();
                GamePlayData.Instance.heartTimer.SetInfiniteHeartTime();
                //TODO: 하트 충전 관련 애니메이션?
                GamePlayData.Instance.admobManager.SetAdChargingFullTime(MainKey.adChargingCycleMin);

                if (SceneSwitcher.Instance.curSceneName == Define.SceneName.Game)
                {
                    SceneSwitcher.Instance.SwitchGameScene(GamePlayData.Instance.curTown, GamePlayData.Instance.curStage);
                }
            });
        }
        else
        {
            GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.AdUnavailable);
        }
    }

    private void OnClickRefillBtn()
    {
        SetInteractable(false);
        //TODO: 코인사용해서 하트 풀충전
        base.ShowBtnClickSFX();

        if (GamePlayData.Instance == null)
        {
            return;
        }

        //돈이 더 많음
        if(GamePlayData.Instance.coinCnt >= GetRetryCoinCnt(GamePlayData.Instance.inGameTryCnt))
        {
            GamePlayData.Instance.coinCnt -= GetRetryCoinCnt(GamePlayData.Instance.inGameTryCnt);
            GamePlayData.Instance.inGameTryCnt++;
            GamePlayData.Instance.heartTimer.heartCnt = OutGameInfo.maxHeartCnt;
            MainUIManager.Instance.mainCanvas.SetWealthData();
            HidePopup();
        }
        else
        {
            HidePopup();
            GamePlayData.Instance.storeCanvas.ShowCanvas();
        }
    }

    private int GetRetryCoinCnt(int cnt)
    {
        Define.RetryCoin coin = Define.RetryCoin.None;

        if(cnt < 1)
        {
            cnt = 1;
        }

        switch (cnt)
        {
            case 1:
                coin = Define.RetryCoin.First;
                break;
            case 2:
                coin = Define.RetryCoin.Second;
                break;
            case 3:
                coin = Define.RetryCoin.Third;
                break;
            case 4:
                coin = Define.RetryCoin.Fourth;
                break;
            default:
            case 5:
                coin = Define.RetryCoin.Others;
                break;
        }

        return (int)coin;
    }

    private void AlertChargingHeartState()
    {
        if (_popupArea.activeSelf)
        {
            HidePopup();
            GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.HeartCharged);
        }
    }

    public override void SetInteractable(bool active)
    {
        _showAdBtn.interactable = active;
        _closeBtn.interactable = active;
        _refillBtn.interactable = active;
    }
    #endregion
}
