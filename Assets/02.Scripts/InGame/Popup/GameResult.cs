using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using HHK.UIEC;
using TMPro;
using Sirenix.OdinInspector;


public class GameResult : InGameBasePopup
{
    [Header("------------------ Type Setting -----------------")]
    [SerializeField]
    private bool _isTraining = false;

    [Header("---------------- GUI ----------------")]
    [SerializeField]
    private GameObject _popupArea;

    [Header("---------------- Animation ----------------")]
    [SerializeField, DisableIf("_isTraining", true)]
    private UIECAnimator _rankTextAnim;
    private const float _rankAnimDelay = 2f;
    private WaitForSeconds _yieldRankAnimDelay;

    [Header("---------------- Buttons ----------------")]
    [SerializeField]
    private Button _closeBtn;
    [SerializeField]
    private Button _nextBtn;

    [Header("---------------- Result Info ----------------")]
    [SerializeField, DisableIf("_isTraining", true)]
    private GameObject[] _quavers; //1~3개
    [SerializeField, DisableIf("_isTraining", true)]
    private UnityEngine.UI.Text levelText;
    [SerializeField, DisableIf("_isTraining", true)]
    private UnityEngine.UI.Text comboText;

    //Result Data
    private bool _isAllPerfect = false;
    public bool isAllPerfect { get { return _isAllPerfect; } set {
            _isAllPerfect = value;

            if(PlayerPrefs.GetInt(EncryptedKey.isAllPerfect + TownDataLoader.level.ToString()) < 1)
            {
                //올퍼펙트가 아님
                PlayerPrefs.SetInt(EncryptedKey.isAllPerfect + TownDataLoader.level.ToString(), _isAllPerfect == true ? 1 : 0);
                FirestoreManager.Instance.SetIsAllPerfectLevels(TownDataLoader.level, _isAllPerfect);
            }
        } }
    private int _quaverCnt = 1;
    public int quaverCnt { get { return _quaverCnt; } set {
            _quaverCnt = value;
            if(PlayerPrefs.GetInt(EncryptedKey.score + TownDataLoader.level.ToString()) < _quaverCnt)
            {
                PlayerPrefs.SetInt(EncryptedKey.score + TownDataLoader.level.ToString(), _quaverCnt);
                FirestoreManager.Instance.SetScores(TownDataLoader.level, _quaverCnt);
            }
        } }
    private bool _isTownComplete = false;
    private Define.TownList _playedTown;
    private int _playedStage;

    #region Unity Life Cycle
    private void Awake()
    {
        _yieldRankAnimDelay = new WaitForSeconds(_rankAnimDelay);
    }

    public override void Start()
    {
        base.Start();

        _nextBtn?.onClick.AddListener(OnClickNextBtn);
        _closeBtn?.onClick.AddListener(OnClickNextBtn);
    }

    public virtual void OnDestroy()
    {
        StopAllCoroutines();
        _rankTextAnim = null;
        _yieldRankAnimDelay = null;
        _nextBtn = null;
        _quavers = null;
        levelText = null;
        comboText = null;
        _popupArea = null;
    }
    #endregion

    #region Result
    private IEnumerator RandomDelayCoroutine(UnityAction action)
    {
        while (true)
        {
            yield return _yieldRankAnimDelay;

            // TeddyStack 버튼 클릭 이벤트 호출
            action?.Invoke();
        }
    }

    public virtual void ShowPopup(bool isTraining = false)
    {
        base.ShowPopup();
        SetInteractable(true);
        _popupArea.SetActive(true);

        ShowScoreAndAnim();
        
        _playedTown = TownDataLoader.town;
        _playedStage = TownDataLoader.level;

        if (GamePlayData.Instance == null)
        {
            DebugX.Log("GamePlayData.Instance null");
            return;
        }

        GamePlayData.Instance.InitItem();

        if (TownDataLoader.IsLastStageOfTown())
        {
            _isTownComplete = true;
        }
        else
        {
            _isTownComplete = false;
        }

        if (!isTraining)
        {
            GamePlayData.Instance.curStage++;

            if (GamePlayData.Instance.inGameTryCnt <= 1)
            {
                GamePlayData.Instance.isSuccessfulOnFirstTryCnt++;
            }
        }
    }

    public virtual void ShowScoreAndAnim()
    {
        //Animation
        this.GetComponent<UIECAnimator>().OnCustomChannel();
        StartCoroutine(RandomDelayCoroutine(_rankTextAnim.OnCustomChannel));
        //음표 점수 계산
        SetQuaver(CalculateQuaverByHP(BeatGridTracker.Instance.judgeChecker.hpManager.hp));
        //코인 점수 계산
        SetCoin(BeatGridTracker.Instance.judgeChecker.maxCombo, BeatGridTracker.Instance.judgeChecker.judgeCnts[(int)Define.NoteJudge.Perfect]);
        comboText.text = BeatGridTracker.Instance.judgeChecker.maxCombo.ToString();
        levelText.text = TownDataLoader.level.ToString();
    }

    private int CalculateQuaverByHP(int curHP)
    {
        if(curHP >= BeatGridTracker.Instance.inGameMaxHP)
        {
            //올퍼펙 3
            isAllPerfect = true;
            return 3;
        }
        else if(curHP > (int)((float)BeatGridTracker.Instance.inGameMaxHP * (float)(Define.HPType.Enough) * 0.01f))
        {
            //3
            isAllPerfect = false;
            return 3;
        }
        else if (curHP > (int)((float)BeatGridTracker.Instance.inGameMaxHP * (float)(Define.HPType.Low) * 0.01f))
        {
            //2
            isAllPerfect = false;
            return 2;
        }
        else
        {
            //1
            isAllPerfect = false;
            return 1;
        }
    }

    private void SetQuaver(int quaverCnt)
    {
        if(GamePlayData.Instance == null)
        {
            return;
        }

        //음표 재화 누적
        if (quaverCnt >= PlayerPrefs.GetInt(EncryptedKey.score + TownDataLoader.level.ToString()))
        {
            //해당 스테이지에서 얻은 음표가 이전에 얻은 음표보다 커야 안얻은 음표를 얻을 수 있음
            GamePlayData.Instance.getQuaverCnt = quaverCnt - PlayerPrefs.GetInt(EncryptedKey.score + TownDataLoader.level.ToString());
           
            if (GamePlayData.Instance != null)
            {
                //기록용 음표
                GamePlayData.Instance.recordQuaverCnt += quaverCnt - PlayerPrefs.GetInt(EncryptedKey.score + TownDataLoader.level.ToString());
            }
        }

        this.quaverCnt = quaverCnt;
        
        for(int i=0; i< _quavers.Length; i++)
        {
            _quavers[i].SetActive(i < quaverCnt);
        }
        DebugX.Log($"Result - Quaver: {PlayerPrefs.GetInt(EncryptedKey.score + TownDataLoader.level.ToString())}, Allperfect: {PlayerPrefs.GetInt(EncryptedKey.isAllPerfect + TownDataLoader.level.ToString())}");
        
    }

    private void SetCoin(int maxCombo, int perfectCnt)
    {
        if(GamePlayData.Instance == null)
        {
            return;
        }

        GamePlayData.Instance.getCoinCnt = maxCombo + perfectCnt;
        DebugX.Log($"Result - Get Coin: { PlayerPrefs.GetInt(EncryptedKey.getCoinCnt)}");
    }
    #endregion

    #region Play
    public virtual void OnClickNextBtn()
    {
        SetInteractable(false);
        base.ShowBtnClickSFX();
        if (SceneSwitcher.Instance == null)
        {
            return;
        }
        SceneSwitcher.Instance.SwitchScene(Define.SceneName.Main);
    }

    public override void OnClickBlackPanelBtn()
    {
        SetInteractable(false);
        OnClickNextBtn();
    }

    public override void SetInteractable(bool active)
    {
        _closeBtn.interactable = active;
        _nextBtn.interactable = active;
    }
    #endregion
}
