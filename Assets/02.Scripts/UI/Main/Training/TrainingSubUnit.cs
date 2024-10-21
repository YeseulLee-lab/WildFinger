using HHK.UIEC;
using UnityEngine;
using UnityEngine.UI;

public class TrainingSubUnit : MonoBehaviour
{
    [SerializeField]
    private Image _background;
    [SerializeField]
    private Sprite[] _backGroundSP;
    [SerializeField]
    private Image _trainingImage;

    private InGameTutorialPopupInfo _trainingInfo;
    public int _idx;

    public bool isSelected = false;

    #region Unity Life Cycle
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(SelectTraining);
    }
    #endregion

    public void UpdateItem(int idx, InGameTutorialPopupInfo info) 
    {
        //reset
        _idx = idx;
        if (idx == MainUIManager.Instance.trainingCanvas.selectedIdx)
        {
            SelectUnit();
        }
        else
        {
            UnselectUnit();
        }

        _trainingInfo = info;
        _trainingImage.sprite = info.trainingImg;
        _background.color = new Color(1f, 1f, 1f, 1f);
        _trainingImage.color = new Color(1f, 1f, 1f, 1f);
        GetComponent<Button>().interactable = true;
        
        //아직 수행할수 없는 튜토리얼
        if (GamePlayData.Instance.maxStage <= info.level)
        {
            _background.sprite = _backGroundSP[1];
            _background.color = new Color(1f, 1f, 1f, 0.3f);
            _trainingImage.color = new Color(1f, 1f, 1f, 0.3f);
            GetComponent<Button>().interactable = false;
        }
    }

    private void SelectTraining()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }
        _background.GetComponent<UIECAnimator>().OnCustomChannel();
        MainUIManager.Instance.trainingCanvas.SetDescData(_trainingInfo.type, _idx, _trainingInfo.trainingDesc);
        SelectUnit();
    }

    public void SelectUnit()
    {
        isSelected = true;
        _background.sprite = _backGroundSP[0];
    }

    public void UnselectUnit()
    {
        isSelected = false;
        _background.sprite = _backGroundSP[1];
    }
}
