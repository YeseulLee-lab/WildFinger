using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SelectLevelCanvas : BaseMainCanvas
{
    [Header("----------------- Level Select Area -----------------")]
    [SerializeField]
    private Image _background;
    [SerializeField]
    private Image _townTitlePanel;
    [SerializeField]
    private Text _townTitle;
    [SerializeField]
    private LevelSelectScrollContent _levelSelectConent;

    [SerializeField]
    private Ease showEase;

    #region Set Data
    public void SetData(TownInfo townInfo, bool isShowingBack)
    {
        if (isShowingBack)
        {
            _background.DOFade(1f, 0.3f);
        }
        _levelSelectConent.SetData(townInfo);
        _townTitlePanel.sprite = townInfo.townTitlePanel;
        _townTitle.text = _townTitle.GetComponent<LocalizationTextUI>().GetSummary(townInfo.townName);
        _townTitle.GetComponent<Outline>().effectColor = townInfo.outline;
        _townTitle.GetComponent<Shadow>().effectColor = townInfo.shadow;
        _townTitle.GetComponent<Gradient2>().EffectGradient = townInfo._effectGradient;

        if (townInfo.townType != GamePlayData.Instance.maxTown)
        {
            _background.sprite = townInfo.quests.videoLastFrames[7];
        }
        else
        {
            if (GamePlayData.Instance.maxAssetIdx == 0)
            {
                _background.sprite = townInfo.quests.videoFirstFrame;
            }
            else
            {
                _background.sprite = townInfo.quests.videoLastFrames[GamePlayData.Instance.maxAssetIdx - 1];
            }
        }
        
        _townTitlePanel.rectTransform.DOScaleX(1f, 0.3f).SetEase(showEase);
    }
    #endregion

    #region Canvas Action
    public void ShowCanvas(TownInfo townInfo, bool isShowingBack)
    {
        _showArea.SetActive(true);
        SetData(townInfo, isShowingBack);
        MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.LevelPage);
    }

    public override void HideCanvas()
    {
        base.HideCanvas();
        _townTitlePanel.rectTransform.localScale = new Vector3(0f, 1f, 1f);
        _background.color = new Color(1f, 1f, 1f, 0f);
        MainUIManager.Instance.mainCanvas.ShowMainObject(null);
    }
    #endregion
}
