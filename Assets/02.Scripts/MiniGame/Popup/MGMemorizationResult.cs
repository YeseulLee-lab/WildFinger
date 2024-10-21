using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using HHK.UIEC;
using TMPro;
using FMODUnity;
using FMOD.Studio;
using System;

public class MGMemorizationResult : InGameBasePopup
{
    [SerializeField]
    private GameObject _popupArea;
    [Header("---------------- Animation ----------------")]
    [SerializeField]
    private UIECAnimator _rankTextAnim;
    private const float _rankAnimDelay = 2f;
    private WaitForSeconds _yieldRankAnimDelay;
    [SerializeField]
    private EventReference _resultBGM;
    private static EventInstance _resultInstance;

    [Header("---------------- Buttons ----------------")]
    [SerializeField]
    private Button _replayBtn;
    [SerializeField]
    private Button _nextBtn;

    [Header("---------------- Result Info ----------------")]
    [SerializeField]
    private UnityEngine.UI.Text levelText;

    //Result Data
    private bool _isAllPerfect = false;
    public bool isAllPerfect
    {
        get { return _isAllPerfect; }
        set
        {
            _isAllPerfect = value;

            if (PlayerPrefs.GetInt(EncryptedKey.isAllPerfect + TownDataLoader.level.ToString()) < 1)
            {
                //올퍼펙트가 아님
                PlayerPrefs.SetInt(EncryptedKey.isAllPerfect + TownDataLoader.level.ToString(), _isAllPerfect == true ? 1 : 0);
            }
        }
    }
    private int _quaverCnt = 1;
    public int quaverCnt
    {
        get { return _quaverCnt; }
        set
        {
            _quaverCnt = value;
            if (PlayerPrefs.GetInt(EncryptedKey.score + TownDataLoader.level.ToString()) < _quaverCnt)
            {
                PlayerPrefs.SetInt(EncryptedKey.score + TownDataLoader.level.ToString(), _quaverCnt);
            }
        }
    }
    private bool _isTownComplete = false;
    private Define.TownList _playedTown;
    private int _playedStage;

    [Header("----------------- Globalization Text -----------------")]
    [SerializeField]
    private UnityEngine.UI.Text _nextBtnText;

    #region Unity Life Cycle
    private void Awake()
    {
        _yieldRankAnimDelay = new WaitForSeconds(_rankAnimDelay);
        _resultInstance = RuntimeManager.CreateInstance(_resultBGM);
    }

    public override void Start()
    {
        base.Start();
        _replayBtn?.onClick.AddListener(() => PlayStage(false));

        if (GamePlayData.Instance != null)
        {
            _resultInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }

        if (SceneSwitcher.Instance == null)
        {
            return;
        }
        _nextBtn?.onClick.AddListener(() => SceneSwitcher.Instance.SwitchScene(Define.SceneName.Main));
    }

    private void OnDestroy()
    {
        _resultInstance.setUserData(IntPtr.Zero);
        _resultInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _resultInstance.release();
        StopAllCoroutines();
        _rankTextAnim = null;
        _yieldRankAnimDelay = null;
        _popupArea = null;
    }
    #endregion

    #region Judge
    private IEnumerator RandomDelayCoroutine(UnityAction action)
    {
        while (true)
        {
            yield return _yieldRankAnimDelay;

            // TeddyStack 버튼 클릭 이벤트 호출
            action?.Invoke();
        }
    }

    public override void ShowPopup()
    {
        base.ShowPopup();
        SetInteractable(true);
        _resultInstance.start();
        _popupArea.SetActive(true);
        this.GetComponent<UIECAnimator>().OnCustomChannel();
        StartCoroutine(RandomDelayCoroutine(_rankTextAnim.OnCustomChannel));

        if (GamePlayData.Instance == null)
        {
            DebugX.Log("GamePlayData.Instance null");
            return;
        }

        //TODO: 점수 계산법 수정
        SetCoin(InGameKey.mgMemorizationBonusCoinCnt);
        SetQuaver(3);

        levelText.text = GamePlayData.Instance.curStage.ToString();
        _playedTown = GamePlayData.Instance.curTown;
        _playedStage = GamePlayData.Instance.curStage;

        if (TownDataLoader.IsLastStageOfTown())
        {
            _isTownComplete = true;
        }
        else
        {
            _isTownComplete = false;
        }

        GamePlayData.Instance.curStage++;
        if (GamePlayData.Instance.inGameTryCnt <= 1)
        {
            GamePlayData.Instance.isSuccessfulOnFirstTryCnt++;
        }
    }

    private void SetQuaver(int quaverCnt)
    {
        if(GamePlayData.Instance == null)
        {
            return;
        }

        if (quaverCnt >= PlayerPrefs.GetInt(EncryptedKey.score + TownDataLoader.level.ToString()))
        {
            //해당 스테이지에서 얻은 음표가 이전에 얻은 음표보다 커야 안얻은 음표를 얻을 수 있음
            GamePlayData.Instance.getQuaverCnt = quaverCnt - PlayerPrefs.GetInt(EncryptedKey.score + TownDataLoader.level.ToString());

            if (GamePlayData.Instance != null)
            {
                //기록용 음표
                GamePlayData.Instance.recordQuaverCnt += quaverCnt - PlayerPrefs.GetInt(EncryptedKey.score + TownDataLoader.level.ToString());
                PlayerPrefs.SetInt(GamePlayData.Instance.curTown.ToString() + MainTownKey.townLevelKey, PlayerPrefs.GetInt(GamePlayData.Instance.curTown.ToString() + MainTownKey.townLevelKey) + 1);
                PlayerPrefs.SetInt(EncryptedKey.isAllPerfect + TownDataLoader.level.ToString(), 1);
            }
        }

        this.quaverCnt = quaverCnt;
        DebugX.Log($"Result - Quaver: {PlayerPrefs.GetInt(EncryptedKey.score + TownDataLoader.level.ToString())}, Allperfect: {PlayerPrefs.GetInt(EncryptedKey.isAllPerfect + TownDataLoader.level.ToString())}");
    }

    private void SetCoin(int bonusCoinCnt)
    {
        if (GamePlayData.Instance == null)
        {
            return;
        }

        if (PlayerPrefs.GetInt(EncryptedKey.isAllPerfect + TownDataLoader.level.ToString()) == 1)
        {
            DebugX.Log($"Result - Get Coin: 0 (Already gained)");
            return;
        }

        GamePlayData.Instance.getCoinCnt = bonusCoinCnt;
        DebugX.Log($"Result - Get Coin: { PlayerPrefs.GetInt(EncryptedKey.getCoinCnt)}");
    }
    #endregion

    #region Play
    private void PlayStage(bool isNextLevel)
    {
        SetInteractable(false);
        if (GamePlayData.Instance == null)
        {
            DebugX.Log("GamePlayData.Instance null");
            SetInteractable(true);
            return;
        }

        if (SceneSwitcher.Instance == null)
        {
            DebugX.Log("SceneSwitcher.Instance null");
            SetInteractable(true);
            return;
        }

        //TODO: 하트? 재화 소비
        if (!isNextLevel)
        {
            //리플레이
            SceneSwitcher.Instance.SwitchGameScene(_playedTown, _playedStage);
            return;
        }

        //다음레벨
        if (!_isTownComplete)
        {
            //타운은 그대로, 스테이지 업
            SceneSwitcher.Instance.SwitchGameScene(GamePlayData.Instance.curTown, GamePlayData.Instance.curStage);
        }
        else
        {
            //타운 이동해야 함. 일단 메인화면으로 무조건 내보내게 함.(이후 논의)
            GamePlayData.Instance.curTown++;
            SceneSwitcher.Instance.SwitchScene(Define.SceneName.Main);
        }
    }

    public override void OnClickBlackPanelBtn()
    {
        SetInteractable(false);
        SceneSwitcher.Instance.SwitchScene(Define.SceneName.Main);
    }

    public override void SetInteractable(bool active)
    {
        _replayBtn.interactable = active;
        _nextBtn.interactable = active;
    }
    #endregion
}
