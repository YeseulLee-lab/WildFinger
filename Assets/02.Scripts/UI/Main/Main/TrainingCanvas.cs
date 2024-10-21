using UnityEngine;
using UnityEngine.UI;

public class TrainingCanvas : BaseMainCanvas
{
    [Header("-------------- Training Canvas Area --------------")]
    [SerializeField]
    private Text _trainingDesc;
    [SerializeField]
    private Button _playBtn;
    [SerializeField]
    private TrainingScrollContent trainingScrollContent;

    private Define.InGameTutorialType _selectedType;
    public Define.InGameTutorialType selectedType
    {
        get
        {
            return _selectedType;
        }
        set
        {
            _selectedType = value;
        }
    }

    private int _selectedIdx;
    public int selectedIdx
    {
        get
        {
            return _selectedIdx;
        }
        set
        {
            _selectedIdx = value;
        }
    }

    #region Unity Life Cycle
    public override void Start()
    {
        base.Start();
        _playBtn.onClick.AddListener(OnClickPlay);
    }
    #endregion

    public void SetDescData(Define.InGameTutorialType selectedType, int selectedIdx, string desc)
    {
        this.selectedIdx = selectedIdx;
        this.selectedType = selectedType;

        foreach (TrainingSubUnit unit in trainingScrollContent.subUnits)
        {
            if (unit.isSelected)
            {
                unit.UnselectUnit();

                break;
            }
        }

        _trainingDesc.text = _trainingDesc.GetComponent<LocalizationTextUI>().GetSummary(desc);
    }

    #region OnClick
    private void OnClickPlay()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }

        SceneSwitcher.Instance.SwitchGameScene(selectedType);
    }
    #endregion
}
