using HHK.UIEC;
using UnityEngine;
using UnityEngine.UI;

public class StoreCanvas : BaseMainCanvas
{
    [Header("----------------- Store Canvas Area -----------------")]
    [SerializeField]
    private UnityEngine.UI.Text _coinCnt;
    [SerializeField]
    private Button _moreBtn;

    [Header("----------------- Contents -----------------")]
    [SerializeField]
    private GameObject _basicContent;
    [SerializeField]
    private GameObject _moreContent;

    #region Unity Life Cycle
    public override void Start()
    {
        base.Start();

        _moreBtn.onClick.AddListener(() =>
        {
            if (GamePlayData.Instance != null)
            {
                GamePlayData.Instance.OnClickBtnEffect();
            }
            _basicContent.SetActive(false);
            _moreContent.SetActive(true);
            _moreContent.GetComponent<UIECAnimator>().OnCustomChannel();
        });

        SetCoinCount();
    }

    public new void ShowCanvas()
    {
        _showArea.SetActive(true);
        _basicContent.GetComponent<UIECAnimator>().OnCustomChannel();
    }

    public override void HideCanvas()
    {
        base.HideCanvas();
        _basicContent.SetActive(true);
        _moreContent.SetActive(false);
    }
    #endregion

    public void SetCoinCount()
    {
        _coinCnt.text = GamePlayData.Instance.coinCnt.ToString();
    }
}
