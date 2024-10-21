using Coffee.UIEffects;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class TownUnit : MonoBehaviour
{
    [Header("----------------- UI Sprite ------------------")]
    [SerializeField]
    private Image _panel;
    [SerializeField]
    private Image _ribbonFront;
    [SerializeField]
    private Image _ribbonBehind;
    [SerializeField]
    private UIShadow _titleShadow;
    [SerializeField]
    private Outline _titleOutline;
    [SerializeField]
    private Gradient2 _titleGradient;
    [SerializeField]
    private UIShadow _indexShadow;
    [SerializeField]
    private Outline _indexOutline;

    [Header("----------------- UI Data ------------------")]
    [SerializeField]
    private Text _townIdx;
    [SerializeField]
    private Text _townName;
    [SerializeField]
    private Image _townImage;
    [SerializeField]
    private Button _townButton;
    [SerializeField]
    private GameObject lockImage;
    [SerializeField]
    private Text _lockText;
    [SerializeField]
    private Button _showAnimationBtn;
    public Button showAnimationBtn { get { return _showAnimationBtn; } }
    [SerializeField]
    private TownUnitAsset[] townUnitAsset; //0: complete, 1: lock, 2: current

    [Header("------------ Quaver Progress ------------")]
    [SerializeField]
    private GameObject _levelProgress;
    [SerializeField]
    private Text _levelProgressText;
    [SerializeField]
    private RectTransform _levelProgressFill;

    private TownInfo _townInfo;

    private void Start()
    {
        _townButton.onClick.AddListener(OnClickTownUnit);
        _showAnimationBtn.onClick.AddListener(OnClickShowAnimation);
    }

    public void UpdateItem(int itemCount, TownInfo townInfo)
    {
        _townInfo = townInfo;
        _townName.text = _townName.GetComponent<LocalizationTextUI>().GetSummary(townInfo.townName);
        _townIdx.text = (itemCount + 1).ToString();
        _townImage.sprite = townInfo.townThumb;

        if (itemCount > (int)GamePlayData.Instance.maxTown)
        {
            //잠긴 마을
            SetAsset(1);
            _townButton.interactable = false;
            lockImage.gameObject.SetActive(true);

            int stackLevel = GamePlayData.Instance.GetStackLevels(townInfo);
            
            _lockText.text = "LV." + (stackLevel + 1).ToString() + " - " + (stackLevel + townInfo.levelAmount).ToString();

            _levelProgress.SetActive(false);
            _showAnimationBtn.gameObject.SetActive(false);
        }
        else
        {
            //진행중인 마을, 완료한 마을
            SetAsset(2);
            _townButton.interactable = true;
            lockImage.gameObject.SetActive(false);
            //레벨 진행상황 표시
            int completedLevel = (GamePlayData.Instance.maxStage - 1) - (GamePlayData.Instance.GetStackLevels(townInfo));
            if (GamePlayData.Instance.GetStackLevels(townInfo) <= 0)
                completedLevel = GamePlayData.Instance.maxStage - 1;
            _levelProgress.SetActive(true);
            _levelProgressText.text = completedLevel + "/" + townInfo.levelAmount;

            float ratio = (float)completedLevel / townInfo.levelAmount;
            if (completedLevel == 0 || ratio * _levelProgressFill.sizeDelta.x > 58f)
            {
                _levelProgressFill.sizeDelta = new Vector2(ratio * _levelProgressFill.sizeDelta.x, _levelProgressFill.sizeDelta.y);
            }
            else
            {
                _levelProgressFill.sizeDelta = new Vector2(58f, _levelProgressFill.sizeDelta.y);
            }
            _showAnimationBtn.gameObject.SetActive(false);

            if (completedLevel >= townInfo.levelAmount)
            {
                //완료한 마을 유닛
                SetAsset(0);
                _showAnimationBtn.gameObject.SetActive(true);
            }
        }
    }

    private void OnClickTownUnit()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }
        MainUIManager.Instance.mainCanvas.HideMainObject(() =>
        {
            MainUIManager.Instance.SelectLevelCanvas.ShowCanvas(_townInfo, true);
        });
    }

    private void OnClickShowAnimation()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }
        MainUIManager.Instance.townCanvas.ShowAnimations(_townInfo.quests.videoClips[_townInfo.quests.videoClips.Length - 1]);
    }

    private void SetAsset(int index)
    {
        for (int i = 0; i < 3; i++)
        {
            townUnitAsset[i].bottomBtn.SetActive(false);
        }

        _panel.sprite = townUnitAsset[index].panel;
        _ribbonFront.sprite = townUnitAsset[index].ribbonFront;
        _ribbonBehind.sprite = townUnitAsset[index].ribbonBehind;
        _titleShadow.effectColor = townUnitAsset[index].shadowOutlineColor;
        _titleOutline.effectColor = townUnitAsset[index].shadowOutlineColor;
        _titleGradient.EffectGradient = townUnitAsset[index]._effectGradient;
        _indexShadow.effectColor = townUnitAsset[index].shadowOutlineColor;
        _indexOutline.effectColor = townUnitAsset[index].shadowOutlineColor;
        townUnitAsset[index].bottomBtn.SetActive(true);
    }
}

[Serializable]
public class TownUnitAsset
{
    public Sprite panel;
    public Sprite ribbonFront;
    public Sprite ribbonBehind;
    public GameObject bottomBtn;
    public UnityEngine.Gradient _effectGradient = new UnityEngine.Gradient() { colorKeys = new GradientColorKey[] { new GradientColorKey(Color.black, 0), new GradientColorKey(Color.white, 1) } };
    public Color shadowOutlineColor;
}