using DG.Tweening;
using HHK.UIEC;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using FMODUnity;
using FMOD.Studio;
using System;

public class QuestPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject mainCanvasPanel;
    [SerializeField]
    private Button nextTownBtn;
    [Header("----------------- Town -----------------")]
    [SerializeField]
    private UnityEngine.UI.Text _townNumberText;
    [SerializeField]
    private UnityEngine.UI.Text _townName;
    [SerializeField]
    private UnityEngine.UI.Text _townProgress;
    [SerializeField]
    private Image _trophyImg;
    [SerializeField]
    private Image _townProgressBar;
    [SerializeField]
    private Image _townImage;
    [SerializeField]
    private RawImage _townRawImage;

    [Header("----------------- Asset -----------------")]
    [SerializeField]
    private GameObject assetQuestObject;
    [SerializeField]
    private GameObject _sparklePrefab;
    [SerializeField]
    private UnityEngine.UI.Text _assetLevel;
    [SerializeField]
    private UnityEngine.UI.Text _assetName;
    [SerializeField]
    private Image _assetImage;
    [SerializeField]
    private Image _unlockAssetImage;
    [SerializeField]
    private Button _unlockAssetBtn;
    [SerializeField]
    private UnityEngine.UI.Text _unlockAssetText;
    [SerializeField]
    private Image lockImage;
    [SerializeField]
    private VideoPlayer townBackgroundVP;

    private TownInfo _maxTownInfo;

    [Header("------------------ SFX Area -----------------")]
    [SerializeField]
    private EventReference _coinCollectingSfx;
    private EventInstance _coinCollectingSfxInstance;
    [SerializeField]
    private EventReference _shineSfx;
    private EventInstance _shineSfxInstance;
    [SerializeField]
    private EventReference _getOneStarSfx;
    private EventInstance _getOneStarInstance;
    [SerializeField]
    private EventReference _assetPercentageSfx;
    private EventInstance _assetPercentageSfxInstance;
    [SerializeField]
    private EventReference _assetApearSfx;
    private EventInstance _assetApearSfxInstance;
    [SerializeField]
    private EventReference _nextLandSfx;
    private EventInstance _nextLandSfxInstance;
    [SerializeField]
    private EventReference _enableUnlockSfx;
    private EventInstance _enableUnlockSfxInstance;

    private Coroutine _progressCoroutine;

    #region Unity Life Cycle
    private void Awake()
    {
        _coinCollectingSfxInstance = RuntimeManager.CreateInstance(_coinCollectingSfx);
        _shineSfxInstance = RuntimeManager.CreateInstance(_shineSfx);
        _getOneStarInstance = RuntimeManager.CreateInstance(_getOneStarSfx);
        _assetPercentageSfxInstance = RuntimeManager.CreateInstance(_assetPercentageSfx);
        _assetApearSfxInstance = RuntimeManager.CreateInstance(_assetApearSfx);
        _nextLandSfxInstance = RuntimeManager.CreateInstance(_nextLandSfx);
        _enableUnlockSfxInstance = RuntimeManager.CreateInstance(_enableUnlockSfx);
    }

    private void Start()
    {
        if (GamePlayData.Instance != null)
        {
            _shineSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _coinCollectingSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _getOneStarInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _assetPercentageSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _assetApearSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _nextLandSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _enableUnlockSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }

        nextTownBtn.onClick.AddListener(OnClickNextTown);
        _unlockAssetBtn.onClick.AddListener(UnlockAsset);
        lockImage.GetComponent<Button>().onClick.AddListener(UnlockAsset);
    }

    private void OnDestroy()
    {
        _coinCollectingSfxInstance.setUserData(IntPtr.Zero);
        _coinCollectingSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _coinCollectingSfxInstance.release();

        _shineSfxInstance.setUserData(IntPtr.Zero);
        _shineSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _shineSfxInstance.release();

        _getOneStarInstance.setUserData(IntPtr.Zero);
        _getOneStarInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _getOneStarInstance.release();

        _assetPercentageSfxInstance.setUserData(IntPtr.Zero);
        _assetPercentageSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _assetPercentageSfxInstance.release();

        _assetApearSfxInstance.setUserData(IntPtr.Zero);
        _assetApearSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _assetApearSfxInstance.release();

        _nextLandSfxInstance.setUserData(IntPtr.Zero);
        _nextLandSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _nextLandSfxInstance.release();

        _enableUnlockSfxInstance.setUserData(IntPtr.Zero);
        _enableUnlockSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _enableUnlockSfxInstance.release();
    }

    private void UIReset()
    {
        nextTownBtn.gameObject.SetActive(false);
        assetQuestObject.transform.parent.gameObject.SetActive(true);
        _townImage.gameObject.SetActive(true);
        _townRawImage.GetComponent<ZoomInOut>().enabled = false;

        townBackgroundVP.loopPointReached -= TownVideoEnd;
    }

    public void Init()
    {
        UIReset();

        _townRawImage.rectTransform.sizeDelta = new Vector2(Display.main.renderingHeight, Display.main.renderingHeight);
        _townImage.rectTransform.sizeDelta = new Vector2(Display.main.renderingHeight, Display.main.renderingHeight);

        _maxTownInfo = GamePlayData.Instance.maxTownInfo;

        SetUI();
        if (GamePlayData.Instance.maxAssetIdx < _maxTownInfo.assetCnt)
        {
            SetAssetImage(false);
            EnableUnlockButton();
            townBackgroundVP.loopPointReached += TownVideoEnd;
        }
        else
        {
            SetAssetImage(true);
        }

        #region legacy
        /*//마지막애셋이면
        if (GamePlayData.Instance.maxAssetIdx == _maxTownInfo.assetCnt)
        {
            townBackgroundVP.clip = _maxTownInfo.quests.videoClips[GamePlayData.Instance.maxAssetIdx - 1];

            townBackgroundVP.Play(); //플레이 종료되어야 rawimage, image active 교체
            townBackgroundVP.isLooping = _maxTownInfo.quests.isLooping;
            _townImage.gameObject.SetActive(false);
            _townRawImage.GetComponent<ZoomInOut>().enabled = true;
            nextTownBtn.gameObject.SetActive(true);
            MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.NextLand, null, new RectTransform[] { nextTownBtn.GetComponent<RectTransform>() });

            //마지막 마을이면
            if ((int)GamePlayData.Instance.maxTown == GamePlayData.Instance.maxDevelpedTown - 1)
            {
                assetQuestObject.transform.parent.gameObject.SetActive(false);
            }

            SetTownData();
        }
        else
        {
            townBackgroundVP.loopPointReached += TownVideoEnd;
            
            SetAssetImage(false);
            SetUI();
            EnableUnlockButton();
        }*/
        #endregion
    }

    public void EnableUnlockButton()
    {
        //maxStage가 현재 애셋의 unlockStage와 같은지 확인
        //TODO: 마지막 마을의 마지막 레벨 예외처리
        int unlockStage = _maxTownInfo.quests.questDatas[GamePlayData.Instance.maxAssetIdx].unlockStage + 1;

        //마지막 마을이고 마지막 레벨 점수가 1이상이면
        if (GamePlayData.Instance.maxStage >= unlockStage)
        {
            _enableUnlockSfxInstance.start();
            Sequence sequence = DOTween.Sequence();
            sequence.Join(lockImage.rectTransform.DOShakeRotation(0.5f, 30f, 20).SetEase(Ease.OutCubic))
                .Append(lockImage.rectTransform.DOLocalMoveY(-50f, 0.2f).SetEase(Ease.InOutBack))
                .Append(lockImage.DOFade(0, 0.5f))
                .OnComplete(() =>
                {
                    _unlockAssetBtn.gameObject.SetActive(true);
                    if (GamePlayData.Instance.maxStage == GamePlayData.Instance.GetTownInfo(Define.TownList.ToyTown).quests.questDatas[0].unlockStage + 1 && GamePlayData.Instance.maxTown == Define.TownList.ToyTown && GamePlayData.Instance.maxAssetIdx == 0)
                    {
                        MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.OpenAsset1, null, new RectTransform[] { _unlockAssetBtn.GetComponent<RectTransform>() });
                    }
                    _unlockAssetBtn.GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
                    _unlockAssetBtn.interactable = true;
                });
        }
    }

    public void DisableUnlockButton()
    {
        _unlockAssetBtn.gameObject.SetActive(false);
        _unlockAssetBtn.interactable = true;
        _unlockAssetBtn.GetComponent<CanvasGroup>().alpha = 0f;
        lockImage.gameObject.SetActive(true);
        lockImage.color = new Color(1f, 1f, 1f, 1f);
        lockImage.rectTransform.anchoredPosition = new Vector2(0f, 0f);
    }

    #endregion

    private void UnlockAsset()
    {
        GamePlayData.Instance.mobileVibrater.Vibrate(MainKey.buttonVibrateMS, MainKey.buttonVibrateAmplitude);

        if (GamePlayData.Instance.maxStage < _maxTownInfo.quests.questDatas[GamePlayData.Instance.maxAssetIdx].unlockStage + 1)
        {
            //자물쇠 shake 애니메이션
            lockImage.GetComponentInChildren<UIECAnimator>().enabled = true;
            lockImage.GetComponentInChildren<UIECAnimator>().OnCustomChannel();
            return;
        }

        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }

        _unlockAssetBtn.interactable = false;
        //퀘스트 반짝이 애니메이션
        _shineSfxInstance.start();
        GameObject sparkle = Instantiate(_sparklePrefab, transform.GetChild(0).position, Quaternion.identity, transform.GetChild(0).transform) as GameObject;
        sparkle.transform.DOMove(_townImage.transform.position, 1).SetEase(Ease.OutBack).OnComplete(() =>
        {
            Destroy(sparkle.gameObject);
            MainUIManager.Instance.mainCanvas.HideMainObject(() => {
                GamePlayData.Instance.maxAssetIdx++;
                DisableUnlockButton();
                if (_maxTownInfo.quests.questDatas.Length - 1 >= GamePlayData.Instance.maxAssetIdx)
                    SetQuestData();
                SetAssetImage(true);
            });
        });
    }

    #region SetUI
    private void SetUI()
    {
        SetTownData();
        SetQuestData();
    }
    private void SetTownData()
    {
        DebugX.Log("TownName : " + _maxTownInfo.townName);

        float ratio = (float)GamePlayData.Instance.maxAssetIdx / _maxTownInfo.assetCnt;
        _townProgressBar.rectTransform.DOScaleX(ratio, 0.5f);
        
        if (GamePlayData.Instance.maxAssetIdx < 1)
        {
            if (gameObject.activeSelf)
            {
                _progressCoroutine = StartCoroutine(CoIncreaseNumber(_townProgress, 0, Mathf.FloorToInt((ratio * 100f))));
            }
            else
            {
                _townProgress.text = Mathf.FloorToInt((ratio * 100f)).ToString() + "%";
            }
        }
        else
        {
            float endRatio = (float)GamePlayData.Instance.maxAssetIdx / _maxTownInfo.assetCnt;
            if (gameObject.activeSelf)
                _progressCoroutine = StartCoroutine(CoIncreaseNumber(_townProgress, Mathf.FloorToInt(endRatio * 100f), Mathf.FloorToInt((ratio * 100f))));
            else
            {
                _townProgress.text = Mathf.FloorToInt((ratio * 100f)).ToString() + "%";
            }
        }

        _townName.text = _assetName.GetComponent<LocalizationTextUI>().GetSummary(_maxTownInfo.townName);
        _townNumberText.text = ((int)GamePlayData.Instance.maxTown + 1).ToString();
    }

    private void SetQuestData()
    {
        if (GamePlayData.Instance.maxAssetIdx == _maxTownInfo.assetCnt)
        {
            nextTownBtn.gameObject.SetActive(true);
            if ((int)GamePlayData.Instance.maxTown == GamePlayData.Instance.maxDevelpedTown - 1)
            {
                assetQuestObject.transform.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            DebugX.Log("현재 애셋 :" + _maxTownInfo.quests.questDatas[GamePlayData.Instance.maxAssetIdx].assetName);
            _unlockAssetText.text = _unlockAssetText.GetComponent<LocalizationTextUI>().GetSummary(_maxTownInfo.quests.questDatas[GamePlayData.Instance.maxAssetIdx].assetName);

            _assetLevel.text = "LV. " + _maxTownInfo.quests.questDatas[GamePlayData.Instance.maxAssetIdx].unlockStage;
            _assetName.text = _assetName.GetComponent<LocalizationTextUI>().GetSummary(_maxTownInfo.quests.questDatas[GamePlayData.Instance.maxAssetIdx].assetName);

            _assetImage.sprite = _maxTownInfo.quests.questDatas[GamePlayData.Instance.maxAssetIdx].lockAssetSprite;
            _unlockAssetImage.sprite = _maxTownInfo.quests.questDatas[GamePlayData.Instance.maxAssetIdx].assetSprite;
        }
    }

    private void SetAssetImage(bool isCompleted)
    {
        #region Tutorial OpenAsset2
        if (GamePlayData.Instance.maxAssetIdx == 1 && GamePlayData.Instance.maxTown == (int)Define.TownList.ToyTown)
        {
            //첫 마을의 첫번째 애셋을 완성했을때
            MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.OpenAsset2, null, null);
        }
        #endregion

        //아직 완료한 애셋이 0개면 첫비디오의 첫 프레임 보여줌
        if (GamePlayData.Instance.maxAssetIdx == 0)
        {
            townBackgroundVP.Stop();
            _townImage.gameObject.SetActive(true);
            _townRawImage.GetComponent<ZoomInOut>().enabled = false;
            _townImage.sprite = _maxTownInfo.quests.videoFirstFrame;
        }
        else
        {
            if (isCompleted)
            {
                //마지막 애셋 완성
                townBackgroundVP.clip = _maxTownInfo.quests.videoClips[GamePlayData.Instance.maxAssetIdx - 1];

                townBackgroundVP.Prepare();
                townBackgroundVP.prepareCompleted += TownBackgroundVP_prepareCompleted;
            }
            else
            {
                //비디오의 마지막 프레임 이미지 보여줌
                _townImage.gameObject.SetActive(true);
                _townRawImage.GetComponent<ZoomInOut>().enabled = false;
                _townImage.sprite = _maxTownInfo.quests.videoLastFrames[GamePlayData.Instance.maxAssetIdx - 1];
            }
        }
    }

    private void TownBackgroundVP_prepareCompleted(VideoPlayer source)
    {
        GamePlayData.Instance.mobileVibrater.Vibrate(MainKey.buttonVibrateMS, MainKey.buttonVibrateAmplitude);
        source.Play();

        _assetApearSfxInstance.start();
        _townImage.gameObject.SetActive(false);
        _townRawImage.GetComponent<ZoomInOut>().enabled = true;
        if (GamePlayData.Instance.maxAssetIdx >= _maxTownInfo.quests.questDatas.Length)
        {
            //마지막 애셋
            source.isLooping = _maxTownInfo.quests.isLooping;
        }
        else
        {
            source.isLooping = false;
        }
    }

    private void TownVideoEnd(VideoPlayer vp)
    {
        DebugX.Log("비디오 재생 끝남");
        
        MainUIManager.Instance.mainCanvas.ShowMainObject(() => 
            {
                GameObject sparkle = Instantiate(_sparklePrefab, _townImage.transform.position, Quaternion.identity, _townImage.transform) as GameObject;
                _getOneStarInstance.start();
                sparkle.transform.DOMove(new Vector2(_townProgressBar.transform.position.x + _townProgressBar.rectTransform.sizeDelta.x * 0.5f, _townProgressBar.transform.position.y), 1).OnComplete(() =>
                {
                    _trophyImg.rectTransform.DOPunchScale(new Vector3(1.05f, 1.05f, 1.05f), 0.2f);
                    _assetPercentageSfxInstance.start();
                    Destroy(sparkle.gameObject);
                    SetTownData();
                    
                    if (GamePlayData.Instance.maxAssetIdx == _maxTownInfo.quests.questDatas.Length)
                        MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.NextLand, null, new RectTransform[] { nextTownBtn.GetComponent<RectTransform>() });
                    else
                    {
                        EnableUnlockButton();
                    }
                });
            });
        lockImage.gameObject.SetActive(true);

        //마지막 애셋이면 루프, 이미지 안보여줌
        if (GamePlayData.Instance.maxAssetIdx == _maxTownInfo.quests.questDatas.Length)
        {
            nextTownBtn.gameObject.SetActive(true);
            vp.loopPointReached -= TownVideoEnd;
            //현재 개발된 마을이랑 현재 위치한 마을이랑 같으면 다음마을로 안감
            if ((int)GamePlayData.Instance.maxTown == GamePlayData.Instance.maxDevelpedTown - 1)
            {
                assetQuestObject.transform.parent.gameObject.SetActive(false);
                return;
            }
        }
        else if (GamePlayData.Instance.maxAssetIdx < _maxTownInfo.quests.questDatas.Length)
        {
            //마지막애셋이 아니면
            _townImage.sprite = _maxTownInfo.quests.videoLastFrames[GamePlayData.Instance.maxAssetIdx - 1];
            _townImage.gameObject.SetActive(true);
            _townRawImage.GetComponent<ZoomInOut>().enabled = false;
            vp.Stop();
        }
    }

    private void OnClickNextTown()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.mobileVibrater.Vibrate();
        }
        _nextLandSfxInstance.start();

        //다음 마을로 넘어가기 TODO: 버튼으로 처리
        nextTownBtn.gameObject.SetActive(false);
        //set data
        GamePlayData.Instance.maxTown++;
        GamePlayData.Instance.maxAssetIdx = 0;
        _maxTownInfo = GamePlayData.Instance.maxTownInfo;

        SetUI();
        DisableUnlockButton();
        SetAssetImage(false);
    }

    private IEnumerator CoIncreaseNumber(UnityEngine.UI.Text numberText, int curNum, int endNum)
    {
        while (curNum < endNum)
        {
            curNum ++;
            numberText.text= curNum.ToString() + "%";
            yield return new WaitForSeconds(0.01f);
        }
        numberText.text = endNum.ToString() + "%";
        yield return null;
    }
    #endregion
}
