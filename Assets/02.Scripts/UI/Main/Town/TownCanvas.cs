using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TownCanvas : BaseMainCanvas
{
    [Header("------------------ Town Canvas Area ------------------")]
    [SerializeField]
    private TownSelectContent _townSelectContent;
    [SerializeField]
    private Button _openButton;

    [Header("------------------ Video Swap ------------------")]
    [SerializeField]
    private GameObject _fullVideoPanel;
    [SerializeField]
    private VideoPlayer _fullVP;
    [SerializeField]
    private RawImage _videoImage;
    [SerializeField]
    private Button _videoExitButton;

    [Header("------------------ SFX Area -----------------")]
    [SerializeField]
    private EventReference _openSfx;
    private EventInstance _openSfxInstance;

    public bool isActive;

    #region Unity Life Cycle
    private void Awake()
    {
        _openSfxInstance = RuntimeManager.CreateInstance(_openSfx);
    }

    public override void Start()
    {
        if (GamePlayData.Instance != null)
        {
            _openSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }

        //tutorial
        if (GamePlayData.Instance.maxTown == Define.TownList.Viking)
        {
            _townSelectContent.SetData();
            if(!MainUIManager.Instance.tutorialCanvas.IsTutorialDone(Define.MainTutorialType.LandPage1))
                _townSelectContent.transform.parent.parent.GetComponent<ScrollRect>().vertical = false;

            MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.LandPage1, () =>
            {
                _townSelectContent.transform.parent.parent.GetComponent<ScrollRect>().vertical = true;
            },
            new RectTransform[] { _openButton.GetComponent<RectTransform>(), _openButton.GetComponent<RectTransform>(), _townSelectContent.transform.GetChild(0).GetComponent<TownUnit>().showAnimationBtn.GetComponent<RectTransform>() });
        }

        //set video image
        _videoImage.rectTransform.sizeDelta = new Vector2(Display.main.renderingHeight, Display.main.renderingHeight);
        _fullVP.prepareCompleted += VideoPrepareComplete;

        #region Button
        _openButton.onClick.AddListener(ShowCanvas);
        _closeButton.onClick.AddListener(HideCanvas);
        _videoExitButton.onClick.AddListener(ExitAnimationVideo);
        #endregion
    }

    private void OnDestroy()
    {
        _openSfxInstance.setUserData(IntPtr.Zero);
        _openSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _openSfxInstance.release();
    }
    #endregion

    #region Video Action
    private void VideoPrepareComplete(VideoPlayer source)
    {
        _videoImage.DOFade(1f, 0.3f).OnComplete(() =>
        {
            source.Play();
        });
    }

    public void ShowAnimations(VideoClip videoClip)
    {
        _fullVideoPanel.SetActive(true);
        _fullVP.clip = videoClip;
        _fullVP.Prepare();
        _fullVP.frame = 1;
    }

    public void ExitAnimationVideo()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }
        _videoImage.color = new Color(1f, 1f, 1f, 0f);

        _fullVideoPanel.SetActive(false);
        _fullVP.Stop();

        if (!MainUIManager.Instance.tutorialCanvas.IsTutorialDone(Define.MainTutorialType.LandPage2))
        {
            _townSelectContent.transform.GetChild(0).GetComponent<TownUnit>().showAnimationBtn.interactable = false;
            _townSelectContent.transform.parent.parent.GetComponent<ScrollRect>().vertical = false;
        }   

        MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.LandPage2, () =>
        {
            _townSelectContent.transform.GetChild(0).GetComponent<TownUnit>().showAnimationBtn.interactable = true;
            _townSelectContent.transform.parent.parent.GetComponent<ScrollRect>().vertical = true;
        },
        new RectTransform[] { _townSelectContent.transform.GetChild(0).GetComponent<RectTransform>() });
    }
    #endregion

    #region Canvas Action
    public override void ShowCanvas()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.mobileVibrater.Vibrate();
        }
        isActive = true;
        GetComponent<Canvas>().sortingOrder++;
        _showArea.GetComponent<RectTransform>().DOMoveX(Screen.width, 0.25f).SetEase(Ease.InBack);
        _openSfxInstance.start();
        _townSelectContent.SetData();
    }

    public override void HideCanvas()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }
        isActive = false;
        _showArea.GetComponent<RectTransform>().DOMoveX(0f, 0.25f).SetEase(Ease.OutBack);
        GetComponent<Canvas>().sortingOrder--;
    }
    #endregion
}
