using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using FMOD.Studio;
using HHK.UIEC;

public class MGMemorizationManager : MonoBehaviour
{
    public static MGMemorizationManager Instance { get; private set; }

    [Header("------------------- Others -------------------")]
    [SerializeField]
    private MGSequenceManager _sequenceManager;
    public static MGMemorizationRSPInputManager rspInputManager { get; private set; }

    [Header("------------------- GUI Settings -------------------")]
    [SerializeField]
    private Image _beatCntDownImg;
    [SerializeField]
    private UnityEngine.UI.Text _summarySeqText;
    [SerializeField]
    private UnityEngine.UI.Text _summaryMemText;
    [SerializeField]
    private Image _correctImg;
    [SerializeField]
    private MGMemorizationHPUIManager _hpManager;
    [SerializeField]
    private GameObject _timerLayout;
    [SerializeField]
    private Image _timerFillImg;
    [SerializeField]
    private GameObject _inputLayout;

    [Header("------------------- Addictive Anim -------------------")]
    [SerializeField]
    private UIECAnimator[] _ghostAnims;
    [SerializeField]
    private Sprite[] _ghostImgs; //Default1, Default2, Correct, Incorrect

    [Header("------------------- Popup Settings -------------------")]
    [SerializeField]
    private Button _pauseBtn;
    [SerializeField]
    private MGMemorizationGamePause _pausePopup;
    [SerializeField]
    private MGMemorizationResult _resultPopup;
    [SerializeField]
    private MGMemorizationGameRetry _retryPopup;

    [Header("------------------- FMOD Settings -------------------")]
    [SerializeField]
    private EventReference _cntDownBGM;
    private EventInstance _cntDownInstance;
    private EventInstance _bgmInstance;
    [SerializeField]
    private EventReference _correctBGM;
    private EventInstance _correctInstance;

    [Header("------------------- Settings -------------------")]
    [SerializeField]
    private Sprite[] _rspImgs; // RSP
    [SerializeField]
    private Sprite[] _cntDownImgs; // 1 2 3
    public Define.MiniGameState curState { get; private set; }
    private CancellationTokenSource _cts;
    private CancellationToken _ct;
    private int _curCnt = 0;
    private float _timerImgFullFillWidth = 0;
    private const int _showDelayMS = 1000;
    private const int _hideDelayMS = 500;
    private const int _enteringDelayMS = 3000;
    private bool _isEnteringComplete;
    public bool isEnteringComplete { get { return _isEnteringComplete; } 
        set {
            _isEnteringComplete = value;
        }
    }
    public Define.RSPType[] curCorrectTypes { get; private set; }
    private int _curEnterIndex;
    public int curEnterIndex { get { return _curEnterIndex; } 
        set {
            _curEnterIndex = value;
            _sequenceManager.SetSequence(_curEnterIndex);
        } }
    public int curMaxEnterIndex { get; private set; }
    public bool isEnteringState { get; private set; } = false;

    #region Unity Life Cycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        rspInputManager = this.GetComponent<MGMemorizationRSPInputManager>();
        _timerImgFullFillWidth = _timerFillImg.rectTransform.sizeDelta.x;

        SetGameState(Define.MiniGameState.Waiting);
        _pauseBtn?.gameObject.SetActive(false);
        _cntDownInstance = RuntimeManager.CreateInstance(_cntDownBGM);
        _correctInstance = RuntimeManager.CreateInstance(_correctBGM);
    }

    private void OnDestroy()
    {
        _bgmInstance.setUserData(IntPtr.Zero);
        _bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _bgmInstance.release();
        _cntDownInstance.setUserData(IntPtr.Zero);
        _cntDownInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _cntDownInstance.release();
        _correctInstance.setUserData(IntPtr.Zero);
        _correctInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _correctInstance.release();

        //TODO: 리소스 할당 초기화
        _sequenceManager = null;
        _beatCntDownImg = null;
        _summarySeqText = null;
        _summaryMemText = null;
        _correctImg = null;
        _hpManager = null;
        _timerLayout = null;
        _timerFillImg = null;
        _inputLayout = null;
        _ghostAnims = null;
        _pauseBtn = null;
        _pausePopup = null;
        _resultPopup = null;
        _retryPopup = null;
        _rspImgs = null;
        _cntDownImgs = null;
        _cts = null;
        Instance = null;
    }

    private async void Start()
    {
        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _correctInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _correctInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }

        StartCoroutine(RandomGhostDelayCoroutine());
        await UniTask.WaitUntil(() => TownDataLoader.isDone);
        StartMiniGame();
    }

    private void OnApplicationFocus(bool focus)
    {
        DebugX.Log("curState: " + curState);
        if (!focus && curState != Define.MiniGameState.End)
        {
            OnClickPauseBtn();
        }
    }
    #endregion

    #region MiniGame Main Action
    public void StartMiniGame()
    {
        SetGameState(Define.MiniGameState.Playing);
        _hpManager.SetHP(InGameKey.defaultIngameLife);
        _pauseBtn?.gameObject.SetActive(true);
        _pauseBtn?.onClick.RemoveAllListeners();
        _pauseBtn?.onClick.AddListener(OnClickPauseBtn);
        _timerLayout.SetActive(false);

        _bgmInstance = RuntimeManager.CreateInstance(TownDataLoader.curMusicInfo.music);
        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _bgmInstance.setVolume(GamePlayData.Instance.isCommonBGMOn ? 1f : 0f);
        }
        _bgmInstance.start();
        _curCnt = 0;
        _beatCntDownImg.gameObject.SetActive(false);
        _correctImg.gameObject.SetActive(false);
        _inputLayout.SetActive(false);

        SetGame(TownDataLoader.curMusicInfo.lineCnt);
    }

    private async void SetGame(int maxMemorizationCnt)
    {
        //DebugX.Log("SetGame: MaxLine: " + maxMemorizationCnt);
        curCorrectTypes = new Define.RSPType[maxMemorizationCnt];

        for (int i=0; i<maxMemorizationCnt; i++)
        {
            curCorrectTypes[i] = (Define.RSPType)UnityEngine.Random.Range(0, 3);
        }

        for(int i=1; i <= maxMemorizationCnt; i++)
        {
            await MemorizationGame(i);
        }
        //End
        ShowResult(true);
    }

    private async UniTask MemorizationGame(int curCnt)
    {
        _sequenceManager.InitUI(curCnt);
        isEnteringState = false;
        await ShowCntDown(3);
        await ShowSerializedRSP(curCnt);
    }

    private async UniTask ShowCntDown(int cntDown)
    {
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;

        //Init All Sequence UIs Not Selected
        _sequenceManager.SetSequence(-1);

        _beatCntDownImg.gameObject.SetActive(true);
        _correctImg.gameObject.SetActive(false);
        _summarySeqText.gameObject.SetActive(false);
        _summaryMemText.gameObject.SetActive(true);

        for (int i = cntDown; i > 0; i--)
        {
            _cntDownInstance.start();
            _beatCntDownImg.sprite = _cntDownImgs[i > 9? 9 : i - 1];

            await UniTask.WaitUntil(() => curState == Define.MiniGameState.Playing);

            try
            {
                await UniTask.Delay(1000, cancellationToken: _ct);
            }
            catch (OperationCanceledException)
            {
                DebugX.Log("ShowCntDown Cancel");
                return;
            }
        }

        _beatCntDownImg.gameObject.SetActive(false);
    }

    private async UniTask ShowSerializedRSP(int curCnt)
    {
        //DebugX.Log("ShowSerializedRSP: " + curCnt);
        //Set Sequence UI
        isEnteringComplete = false;
        curMaxEnterIndex = curCnt;
        for (int i=0; i < curCnt; i++)
        {
            _sequenceManager.SetSequence(i);
            _correctImg.gameObject.SetActive(true);
            _correctImg.sprite = _rspImgs[(int)curCorrectTypes[i]];
            _correctImg.GetComponent<UIECAnimator>().OnCustomChannel();
            await UniTask.WaitUntil(() => curState == Define.MiniGameState.Playing);
            try
            {
                await UniTask.Delay(_showDelayMS, cancellationToken: _ct);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            
            _correctImg.gameObject.SetActive(false);
            await UniTask.WaitUntil(() => curState == Define.MiniGameState.Playing);
            try
            {
                await UniTask.Delay(_hideDelayMS, cancellationToken: _ct);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
        await StartEntering(curCorrectTypes);
    }

    private async UniTask StartEntering(Define.RSPType[] correctRSPs)
    {
        isEnteringState = true;
        _sequenceManager.SetSequence(-1);
        this.curEnterIndex = 0;
        _inputLayout.SetActive(true);
        _summarySeqText.gameObject.SetActive(true);
        _summaryMemText.gameObject.SetActive(false);
        //DebugX.Log("StartEntering: " + correctRSPs.Length);

        bool timerCompleted = (await SetTimer());

        // SetTimer가 완료되면 다음 동작 수행
        if (timerCompleted)
        {
            // 여기에 다음 동작 추가
            _inputLayout.SetActive(false);
            if (!isEnteringComplete)
            {
                //인풋 다 아직 못 침
                _hpManager.ChangeHP(InGameKey.mgIncorrectPoint);
                CheckHP();
            }
        }
    }

    public void SetGameState(Define.MiniGameState state)
    {
        if(curState == Define.MiniGameState.End)
        {
            return;
        }

        curState = state;

        switch (state)
        {
            default:
                break;
            case Define.MiniGameState.Playing:
                rspInputManager.isEnteringReady = true;
                break;
            case Define.MiniGameState.Paused:
                rspInputManager.isEnteringReady = false;
                _bgmInstance.setPaused(true);
                break;
            case Define.MiniGameState.Resumed:
                rspInputManager.isEnteringReady = false;
                _bgmInstance.setPaused(false);
                SetGameState(Define.MiniGameState.Playing);
                break;
            case Define.MiniGameState.End:
                rspInputManager.isEnteringReady = false;
                _bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                break;
            case Define.MiniGameState.PausedCntDown:
                rspInputManager.isEnteringReady = false;
                //처리
                break;
        }
    }

    public async void SetCorrect(int index, bool isCorrect)
    {
        _sequenceManager.SetCorrectUI(index, isCorrect);
        _ghostAnims[0].OnCustomChannel();
        _ghostAnims[1].OnCustomChannel();
        if (isCorrect)
        {
            _correctInstance.start();
            _ghostAnims[0].GetComponent<Image>().sprite = _ghostImgs[2];
            _ghostAnims[1].GetComponent<Image>().sprite = _ghostImgs[2];
        }
        else
        {
            _hpManager.ChangeHP(InGameKey.mgIncorrectPoint);
            CheckHP();
            _ghostAnims[0].GetComponent<Image>().sprite = _ghostImgs[3];
            _ghostAnims[1].GetComponent<Image>().sprite = _ghostImgs[3];
        }

        await UniTask.Delay(300);
        _ghostAnims[0].GetComponent<Image>().sprite = _ghostImgs[0];
        _ghostAnims[1].GetComponent<Image>().sprite = _ghostImgs[1];
    }
    #endregion

    #region UI Action
    public void ShowResult(bool isCompleted)
    {
        isEnteringState = false;
        if (!isCompleted)
        {
            _retryPopup.ShowPopup();
        }
        else
        {
            _resultPopup.ShowPopup();
        }

        SetGameState(Define.MiniGameState.End);
    }

    public void OnClickPauseBtn()
    {
        SetGameState(Define.MiniGameState.Paused);
        _pausePopup.ShowPause();
    }
    #endregion

    #region Util
    private void CheckHP()
    {
        if (_hpManager.hp <= 0)
        {
            ShowResult(false);
        }
    }

    private async UniTask<bool> SetTimer(int ms = _enteringDelayMS)
    {
        _timerLayout.SetActive(true);
        _timerFillImg.rectTransform.sizeDelta = new Vector2(_timerImgFullFillWidth, _timerFillImg.rectTransform.sizeDelta.y);

        float duration = (float)ms / 1000f; // ms를 초 단위로 변환
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float progress = 1f - (Time.time - startTime) / duration;
            _timerFillImg.rectTransform.sizeDelta = new Vector2(_timerImgFullFillWidth * progress, _timerFillImg.rectTransform.sizeDelta.y);

            if (isEnteringComplete)
            {
                _timerFillImg.rectTransform.sizeDelta = new Vector2(0f, _timerFillImg.rectTransform.sizeDelta.y);
                _timerLayout.SetActive(false);
                return true; // isEnteringComplete가 true이면 바로 반환
            }
            await UniTask.WaitUntil(() => curState == Define.MiniGameState.Playing);
            try
            {
                await UniTask.Yield(); // 다음 프레임까지 대기
            }
            catch (OperationCanceledException)
            {
                // 취소됐을 때 처리
                DebugX.Log("SetTimer Cancel");
            }
        }

        //_timerFillImg.fillAmount = 0f;
        _timerFillImg.rectTransform.sizeDelta = new Vector2(0f, _timerFillImg.rectTransform.sizeDelta.y);
        _timerLayout.SetActive(false);
        return true;
    }
    #endregion

    #region Ghost Anim
    private IEnumerator RandomGhostDelayCoroutine()
    {
        int index = 0;
        while (true)
        {
            float randomDelay = UnityEngine.Random.Range(0.5f, 2f);
            yield return new WaitForSeconds(randomDelay);
            _ghostAnims[index++].OnCustomChannel();

            if (index >= _ghostAnims.Length)
            {
                index = 0;
            }
        }
    }
    #endregion
}
