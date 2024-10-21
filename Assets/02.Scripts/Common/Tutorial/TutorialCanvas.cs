using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HHK.UIEC;
using System.Text;
using UnityEngine.Events;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
using DG.Tweening;

public class TutorialCanvas : MonoBehaviour
{
    [Header("------------- Common Tutorials --------------")]
    [SerializeField]
    private GameObject _mainCanvas;
    [SerializeField]
    private UnmaskedPanel unmaskedPanel;
    [SerializeField]
    private Button _backgroundBtn;
    
    private Queue<(Define.MainTutorialType, UnityAction, RectTransform[])> _mainTutorialQueue = new Queue<(Define.MainTutorialType, UnityAction, RectTransform[])>();

    [Header("------------- Area - Description ---------------")]
    [SerializeField]
    private GameObject _descriptionArea;
    [SerializeField]
    private UnityEngine.UI.Text _descriptionSummaryText;

    [Header("------------- Area - Popup ---------------")]
    [SerializeField]
    private GameObject _popupArea;
    [SerializeField]
    private UnityEngine.UI.Text _popupTitleText;
    [SerializeField]
    private UnityEngine.UI.Text _popupSummaryText;
    [SerializeField]
    private Image _popupImg;
    [SerializeField]
    private Button _popupOKBtn;

    [Header("------------- Setting ---------------")]
    private int _curIndex;
    private Define.InGameTutorialType _curTutorialType;
    private Define.MainTutorialType _curMainTutType;
    private Sprite[] _popupImgs;
    private RectTransform[] _curMaskRects;
    private string _curKey = null;
    public UnityAction hideAction { get; set; } = null;

    private bool _isShowing;

    #region Unity Life Cycle
    private void Start()
    {
        _backgroundBtn.onClick.AddListener(() =>
        {
            if (GamePlayData.Instance != null)
            {
                GamePlayData.Instance.OnClickBtnEffect();
            }
            NextTutorial();
        });

        _popupOKBtn.onClick.AddListener(() =>
        {
            if (GamePlayData.Instance != null)
            {
                GamePlayData.Instance.OnClickBtnEffect();
            }
            NextTutorial();
        });

        _descriptionArea.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (GamePlayData.Instance != null)
            {
                GamePlayData.Instance.OnClickBtnEffect();
            }
            NextTutorial();
        });
    }

    private void OnDestroy()
    {
        _mainCanvas = null;
        unmaskedPanel = null;
        _backgroundBtn = null;
        _descriptionArea = null;
        _descriptionSummaryText = null;
        _popupTitleText = null;
        _popupSummaryText = null;
        _popupImg = null;
        _popupOKBtn = null;
        _popupImgs = null;
        _curMaskRects = null;
        hideAction = null;
    }
    #endregion

    #region UI Action
    public void StartTutorial(Define.MainTutorialType type, UnityAction hideAction = null, RectTransform[] maskRects = null)
    {
        if (IsTutorialDone(type))
        {
            return;
        }

        if (!IsTutorialInQueue(type))
        {
            _mainTutorialQueue.Enqueue((type, hideAction, maskRects));
        }

        if (_mainTutorialQueue.Count > 0)
        {
            if (_mainTutorialQueue.Peek().Item1 != type || _isShowing)
            {
                return;
            }
        }

        _isShowing = true;

        StringBuilder sb = new StringBuilder(3);
        _curIndex = 0;
        _curMainTutType = type;
        _curMaskRects = maskRects;
        this.hideAction = hideAction;
        sb.AppendJoin("", "T_", type.ToString(), "_", _curIndex.ToString());
        _curKey = sb.ToString();

        DebugX.Log("Tutorial KEY: " + _curKey);

        if (GamePlayData.Instance == null)
        {
            LocalizationInfo tempInfo =new LocalizationInfo();
            tempInfo.tutorialUIType = Define.TutorialUIType.Description;
            tempInfo.summary = type + "에 대한 튜토리얼이 업데이트 될 예정입니다.";
            SetTutorialUI(tempInfo, maskRects);
            return;
        }

        if (GamePlayData.Instance.tableData.localizationDic.TryGetValue(_curKey, out LocalizationInfo info))
        {
            SetTutorialUI(info, maskRects);
        }
        else
        {
            DebugX.Log($"{_curKey} 튜토리얼 등록 안 됨");
            LocalizationInfo tempInfo =new LocalizationInfo();
            tempInfo.tutorialUIType = Define.TutorialUIType.Description;
            tempInfo.summary = type + "에 대한 튜토리얼이 업데이트 될 예정입니다.";
            SetTutorialUI(tempInfo, maskRects);
        }
    }

    /// <summary>
    /// Popup 타입만 거의 사용하기 때문에 따로 씀
    /// </summary>
    /// <param name="type"></param>
    /// <param name="hideAction"></param>
    /// <param name="maskRects"></param>
    public void StartIngameTutorial(Define.InGameTutorialType type, Sprite[] popupImgs, UnityAction hideAction = null, RectTransform[] maskRects = null)
    {
        //InGame에서 확인(Pause때문에)
        StringBuilder sb = new StringBuilder(3);
        _curIndex = 0;
        _curTutorialType = type;
        _curMaskRects = maskRects;
        this.hideAction = hideAction;
        _popupImgs = popupImgs;
        sb.AppendJoin("", "T_", type.ToString(), "_", _curIndex.ToString());
        _curKey = sb.ToString();

        DebugX.Log("Tutorial KEY: " + _curKey);

        if (GamePlayData.Instance == null)
        {
            LocalizationInfo tempInfo =new LocalizationInfo();
            tempInfo.tutorialUIType = Define.TutorialUIType.Description;
            tempInfo.summary = type + "에 대한 튜토리얼이 업데이트 될 예정입니다.";
            SetTutorialUI(tempInfo, maskRects);
            return;
        }

        if (GamePlayData.Instance.tableData.localizationDic.TryGetValue(_curKey, out LocalizationInfo info))
        {
            SetTutorialUI(info, maskRects);
        }
        else
        {
            DebugX.Log($"{_curKey} 튜토리얼 등록 안 됨");
            LocalizationInfo tempInfo =new LocalizationInfo();
            tempInfo.tutorialUIType = Define.TutorialUIType.Description;
            tempInfo.summary = type + "에 대한 튜토리얼이 업데이트 될 예정입니다.";
            SetTutorialUI(tempInfo, maskRects);
        }
    }

    public void HideTutorial()
    {
        if (SceneSwitcher.Instance == null)
        {
            Hide();
            return;
        }

        if (SceneSwitcher.Instance.sceneType == Define.SceneType.InGame)
        {
            SetTutorialDone(_curTutorialType);
            _curTutorialType = Define.InGameTutorialType.None;
        }
        else
        {
            SetTutorialDone(_curMainTutType);
            _curMainTutType = Define.MainTutorialType.None;
            _isShowing = false;
        }

        Hide();
    }

    private void Hide()
    {
        _popupArea.SetActive(false);
        _descriptionArea.SetActive(false);
        _mainCanvas.SetActive(false);
        _curIndex = 0;
        _curKey = null;

        _curMaskRects = null;
        _popupImgs = null;

        this.hideAction?.Invoke();
        this.hideAction = null;

        if (_mainTutorialQueue.Count > 0)
        {
            StartTutorial(_mainTutorialQueue.Peek().Item1, _mainTutorialQueue.Peek().Item2, _mainTutorialQueue.Peek().Item3);
        }
    }

    public void NextTutorial(Button maskBtn = null)
    {
        if (SceneSwitcher.Instance == null)
        {
            HideTutorial();
            return;
        }

        if (SceneSwitcher.Instance.sceneType == Define.SceneType.Main)
        {
            if (_curMainTutType == Define.MainTutorialType.None)
            {
                return;
            }

            if(_curMaskRects != null)
                _curMaskRects[_curIndex].GetComponent<Canvas>().overrideSorting = false;

            _curIndex++;
            StringBuilder sb = new StringBuilder(3);
            sb.AppendJoin("", "T_", _curMainTutType.ToString(), "_", _curIndex.ToString());
            _curKey = sb.ToString();
            Next(maskBtn);
        }
        else if (SceneSwitcher.Instance.sceneType == Define.SceneType.InGame)
        {
            if (_curTutorialType == Define.InGameTutorialType.None)
            {
                return;
            }

            _curIndex++;
            StringBuilder sb = new StringBuilder(3);
            sb.AppendJoin("", "T_", _curTutorialType.ToString(), "_", _curIndex.ToString());
            _curKey = sb.ToString();
            Next(maskBtn);
        }
    }

    private void Next(Button maskBtn)
    {
        _backgroundBtn.GetComponent<Image>().raycastTarget = true;
        _descriptionArea.GetComponent<Image>().raycastTarget = true;
        unmaskedPanel.transform.GetChild(0).GetComponent<Image>().raycastTarget = true;

        if (GamePlayData.Instance.tableData.localizationDic.TryGetValue(_curKey, out LocalizationInfo info))
        {
            SetTutorialUI(info, _curMaskRects);
        }
        else
        {
            HideTutorial();
        }

        if (maskBtn != null)
        {
            maskBtn?.onClick.RemoveListener(() => Next(maskBtn));
        }
    }
    #endregion

    #region Action
    /// <summary>
    /// TutorialUIType에 따라서 분화
    /// </summary>
    /// <param name="summary"></param>
    private void SetTutorialUI(LocalizationInfo info, RectTransform[] maskRects)
    {
        DebugX.Log("uiType: " + info.tutorialUIType);
        switch (info.tutorialUIType)
        {
            case Define.TutorialUIType.None:
                return;
            case Define.TutorialUIType.Description:
                _backgroundBtn.interactable = true;
                unmaskedPanel.gameObject.SetActive(false);
                _popupArea.SetActive(false);
                _descriptionArea.SetActive(true);
                _descriptionSummaryText.text = info.summary;
                break;
            case Define.TutorialUIType.MaskDescription:
                unmaskedPanel.transform.GetChild(0).gameObject.SetActive(false);
                unmaskedPanel.GetComponent<Image>().raycastTarget = false;

                _popupArea.SetActive(false);
                _descriptionArea.SetActive(true);

                if(info.descPosType == Define.DescPosType.Top)
                    _descriptionArea.GetComponent<RectTransform>().DOMoveY(maskRects[_curIndex].position.y + maskRects[_curIndex].sizeDelta.y * (1 - maskRects[_curIndex].pivot.y) + _descriptionArea.GetComponent<RectTransform>().sizeDelta.y + MainKey.tutorialDescMargin, 0.3f);
                else if (info.descPosType == Define.DescPosType.Bottom)
                    _descriptionArea.GetComponent<RectTransform>().DOMoveY(maskRects[_curIndex].position.y - maskRects[_curIndex].sizeDelta.y * (maskRects[_curIndex].pivot.y) - MainKey.tutorialDescMargin, 0.3f);

                _descriptionSummaryText.text = info.summary;
                if (maskRects != null)
                {
                    maskRects[_curIndex].gameObject.AddComponent<Canvas>().overrideSorting = true;
                    maskRects[_curIndex].GetComponent<Canvas>().sortingOrder = MainKey.tutorialCanvasSortOrder;
                    maskRects[_curIndex].gameObject.AddComponent<GraphicRaycaster>();
                }
                _backgroundBtn.interactable = true;
                break;
            case Define.TutorialUIType.MaskDescriptionAfterClick:
                //다음으로 넘어가기 위한 raycastTarget set false
                _backgroundBtn.GetComponent<Image>().raycastTarget = false;
                _descriptionArea.GetComponent<Image>().raycastTarget = false;
                unmaskedPanel.gameObject.SetActive(true);
                unmaskedPanel.transform.GetChild(0).gameObject.SetActive(false);

                _popupArea.SetActive(false);
                _descriptionArea.SetActive(true);

                if (info.descPosType == Define.DescPosType.Top)
                    _descriptionArea.GetComponent<RectTransform>().DOMoveY(maskRects[_curIndex].position.y + maskRects[_curIndex].sizeDelta.y * (1 - maskRects[_curIndex].pivot.y) + _descriptionArea.GetComponent<RectTransform>().sizeDelta.y + MainKey.tutorialDescMargin, 0.3f);
                else if (info.descPosType == Define.DescPosType.Bottom)
                    _descriptionArea.GetComponent<RectTransform>().DOMoveY(maskRects[_curIndex].position.y - maskRects[_curIndex].sizeDelta.y * (maskRects[_curIndex].pivot.y) - MainKey.tutorialDescMargin, 0.3f);

                _descriptionSummaryText.text = info.summary;
                if (maskRects != null)
                {
                    maskRects[_curIndex].gameObject.AddComponent<Canvas>().overrideSorting = true;
                    maskRects[_curIndex].GetComponent<Canvas>().sortingOrder = MainKey.tutorialCanvasSortOrder;
                    maskRects[_curIndex].gameObject.AddComponent<GraphicRaycaster>();
                }
                if (maskRects[_curIndex].GetComponent<Button>() != null)
                {
                    _backgroundBtn.interactable = false;
                    maskRects[_curIndex].GetComponent<Button>()?.onClick.AddListener(() => NextTutorial(maskRects[_curIndex].GetComponent<Button>()));
                }
                else
                {
                    _backgroundBtn.interactable = true;
                }
                break;
            case Define.TutorialUIType.Popup:
                unmaskedPanel.gameObject.SetActive(true);
                unmaskedPanel.transform.GetChild(0).gameObject.SetActive(false);
                _popupArea.SetActive(true);
                _descriptionArea.SetActive(false);
                _backgroundBtn.interactable = false;
                SetPopupSprite(_curIndex);
                _popupSummaryText.text = info.summary;
                break;
            case Define.TutorialUIType.MaskPopup:
                unmaskedPanel.gameObject.SetActive(true);
                if (maskRects != null)
                {
                    unmaskedPanel.transform.GetChild(0).gameObject.SetActive(true);
                    unmaskedPanel.SetUnmaskedTarget(maskRects[_curIndex]);
                }
                else
                {
                    unmaskedPanel.transform.GetChild(0).gameObject.SetActive(false);
                }
                _popupArea.SetActive(true);
                _descriptionArea.SetActive(false);
                _backgroundBtn.interactable = false;
                SetPopupSprite(_curIndex);
                _popupSummaryText.text = info.summary;
                break;
        }

        _mainCanvas.SetActive(true);
        _mainCanvas.GetComponent<UIECAnimator>().OnCustomChannel();
    }

    private void SetPopupSprite(int index)
    {
        if (_popupImgs == null)
        {
            _popupImg.gameObject.SetActive(false);
            return;
        }
        _popupImg.sprite = _popupImgs[index];
        _popupImg.gameObject.SetActive(true);
    }
    #endregion

    #region Check 
    public bool IsTutorialDone(Define.InGameTutorialType type)
    {
        if(SceneSwitcher.Instance == null)
        {
            return false;
        }

        if(SceneSwitcher.Instance.curSceneName == Define.SceneName.Training) 
        { 
            return false;
        }

        return PlayerPrefs.GetInt("Tut" + type) == 0 ? false : true;
    }

    public bool IsTutorialDone(Define.MainTutorialType type)
    {
        return PlayerPrefs.GetInt("Tut" + type) == 0 ? false : true;
    }

    private void SetTutorialDone(Define.InGameTutorialType type)
    {
        PlayerPrefs.SetInt("Tut" + type, 1);
    }

    private void SetTutorialDone(Define.MainTutorialType type)
    {
        PlayerPrefs.SetInt("Tut" + type, 1);

        _mainTutorialQueue.Dequeue();
    }

    private bool IsTutorialInQueue(Define.MainTutorialType type)
    {
        foreach (var tut in _mainTutorialQueue)
        {
            if (tut.Item1 == type)
            {
                return true;
            }
        }

        return false;
    }
    #endregion
}
