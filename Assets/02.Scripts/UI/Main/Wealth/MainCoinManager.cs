using UnityEngine;

public class MainCoinManager : MonoBehaviour
{
    [SerializeField] 
    private UnityEngine.UI.Text _coinText;
    [SerializeField]
    private GameObject _coinImg;

    #region Unity Life Cycle
    private void OnDestroy()
    {
        
    }
    #endregion

    public void SetCoinData()
    {
        Debug.Log("GamePlayData.Instance.coinCnt: " + GamePlayData.Instance.coinCnt);
        _coinText.text = GamePlayData.Instance.coinCnt.ToString();
        //DebugX.Log("PlayerPrefs.GetInt(MainWealthKey.getCoinKey): " + PlayerPrefs.GetInt(MainWealthKey.getCoinKey));
        //Collect Effect
        if (GamePlayData.Instance.getCoinCnt > 0)
        {
            GetComponent<CollectingCoinManager>().RewardWealth(GamePlayData.Instance.getCoinCnt < 10 ? 1 : GamePlayData.Instance.getCoinCnt / 10, _coinImg.GetComponent<RectTransform>(), _coinText, () =>
            {
                _coinText.text = GamePlayData.Instance.coinCnt.ToString();
                _coinText.rectTransform.localScale = Vector3.one;
            });
            GamePlayData.Instance.coinCnt += GamePlayData.Instance.getCoinCnt;
            GamePlayData.Instance.getCoinCnt = 0;
        }
    }
}
