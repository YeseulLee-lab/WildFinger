using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class MainHeartManager : MonoBehaviour
{
    [Header("----------------- GUI Setting -----------------")]
    [SerializeField]
    private Button _heartBtn;
    [SerializeField]
    private Image _heartImg;
    [SerializeField]
    private Sprite[] _heartImgs; //Default, infinite
    [SerializeField] 
    private Text _heartCntText; // 현재 하트 개수 
    [SerializeField] 
    private Text _heartTimeText; 

    #region Unity Life Cycle
    private void Start()
    {
        if(GamePlayData.Instance == null)
        {
            return;
        }


        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.heartTimer.heartCntUpdateAction += UpdateHeartUI;
            GamePlayData.Instance.heartTimer.heartFullAction += UpdateHeartFullUI;
            GamePlayData.Instance.heartTimer.infiniteHeartTimerModeStartAction += () => SetInfiniteHeartModeUI(true);
            GamePlayData.Instance.heartTimer.infiniteHeartTimerModeEndAction += () => SetInfiniteHeartModeUI(false);
        }

        _heartBtn?.onClick.AddListener(() =>
        {
            if (GamePlayData.Instance != null)
            {
                if (GamePlayData.Instance.heartTimer.heartCnt < 5)
                    MainUIManager.Instance.moreHeartPopup.ShowPopup();
                else
                {
                    GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.FullHeart);
                }
            }   
        });
    }

    private void OnDestroy()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.heartTimer.heartCntUpdateAction -= UpdateHeartUI;
            GamePlayData.Instance.heartTimer.heartFullAction -= UpdateHeartFullUI;
            GamePlayData.Instance.heartTimer.infiniteHeartTimerModeStartAction -= () => SetInfiniteHeartModeUI(true);
            GamePlayData.Instance.heartTimer.infiniteHeartTimerModeEndAction -= () => SetInfiniteHeartModeUI(false);
        }
    }
    #endregion

    #region UI Action
    private void UpdateHeartUI()
    {
        // 하트 개수 텍스트 업데이트
        _heartCntText.text = GamePlayData.Instance.heartTimer.heartCnt.ToString();

        // 하트 타이머 업데이트
        if (GamePlayData.Instance.heartTimer.totalRemainHeartTimerSec <= 0)
        {
            _heartTimeText.text = "00:00";
        }
        else
        {
            int totalSeconds = GamePlayData.Instance.heartTimer.totalRemainHeartTimerSec;
            StringBuilder sb = new StringBuilder();

            if (totalSeconds >= 3600)
            {
                int hours = totalSeconds / 3600;
                int minutes = (totalSeconds % 3600) / 60;
                int seconds = totalSeconds % 60;
                sb.Append(hours.ToString("D2")).Append(':')
                  .Append(minutes.ToString("D2")).Append(':')
                  .Append(seconds.ToString("D2"));
            }
            else
            {
                int minutes = totalSeconds / 60;
                int seconds = totalSeconds % 60;
                sb.Append(minutes.ToString("D2")).Append(':')
                  .Append(seconds.ToString("D2"));
            }

            _heartTimeText.text = sb.ToString();
        }
    }

    private void UpdateHeartFullUI()
    {
        //DebugX.Log("UpdateHeartFullUI");
        _heartCntText.text = OutGameInfo.maxHeartCnt.ToString();
        _heartTimeText.text = GamePlayData.Instance.heartTimer.GetHeartLocalizedFullText();
    }

    private void SetInfiniteHeartModeUI(bool isStart)
    {
        if(_heartImg == null)
        {
            return;
        }

        if (isStart)
        {
            _heartImg.sprite = _heartImgs[1];
            _heartCntText.gameObject.SetActive(false);
        }
        else
        {
            _heartImg.sprite = _heartImgs[0];
            _heartCntText.gameObject.SetActive(true);
        }
    }
    #endregion
}
