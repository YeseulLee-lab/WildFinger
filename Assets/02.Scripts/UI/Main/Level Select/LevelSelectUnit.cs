using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectUnit : UIBehaviour
{
    [SerializeField]
    private Text _levelNumber;

    [Header("------------ Quavers -----------")]
    [SerializeField]
    private Image[] _quavers;
    [SerializeField]
    private GameObject _quaverObject;
    [SerializeField]
    private Sprite[] quaverSpArr; //0: unclear, 1: clear

    [Header("------------ Images -----------")]
    [SerializeField]
    private Image[] _stateImages;//0: prev, 1: ing, 2: complete    
    [SerializeField]
    private Image[] _lineImages; //0: right, 1: right, 2: left, 3: left - 4개의 레벨 단위로 반복하는 패턴이기 때문에
    [SerializeField]
    private Image allPerfectImg;
    [SerializeField]
    private Sprite[] _bossSp;
    [SerializeField]
    private Sprite[] _bonusSp;
    [SerializeField]
    private Sprite[] _normalSp;

    private PlayCurLevelPanel _playCurLevelCanvas;

    private TownInfo _townInfo;

    //UI Recycle View

    #region Unity Life Cycle
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _playCurLevelCanvas = MainUIManager.Instance.playCurLevelPanel;
    }

    /*private void OnDisable()
    {
        _levelNumber = null;
        _quavers = null;
        _quaverObject = null;
        quaverSpArr = null;
        _stateImages = null;
        _lineImages = null;
        _lineImages = null;
        allPerfectImg = null;
        _bossSp = null;
        _bonusSp = null;
    }*/
    #endregion

    public void UpdateItem(int max, int itemCount, LevelInfo levelInfo, TownInfo townInfo)
    {
        _townInfo = townInfo;

        _levelNumber.text = levelInfo.level.ToString();

        for (int i = 0; i < _lineImages.Length; i++)
        {
            _lineImages[i].gameObject.SetActive(false);
        }

        //마지막은 라인 표시 안함
        if (itemCount != max - 1)
        {
            _lineImages[itemCount % 4].gameObject.SetActive(true);
        }

        //첫번째(마지막 레벨 + 1) 안보이게
        if (itemCount == 0)
        {
            gameObject.SetActive(false);
        }

        if (levelInfo.level < GamePlayData.Instance.maxStage)
        {
            _stateImages[0].gameObject.SetActive(false);
            _stateImages[1].gameObject.SetActive(false);
            _stateImages[2].gameObject.SetActive(true);
            _quaverObject.SetActive(true);

            if (SceneSwitcher.IsBonusStage(levelInfo.level))
            {
                _stateImages[2].sprite = _bonusSp[1];
                _levelNumber.text = string.Empty;
            }
            else if(levelInfo.level == townInfo.levelAmount)
            {
                _stateImages[2].sprite = _bossSp[1];
                _levelNumber.text = string.Empty;
            }
            else
            {
                _stateImages[2].sprite = _normalSp[1];
            }

            GetComponent<Button>().interactable = true;
            //올퍼펙트 아님
            if (PlayerPrefs.GetInt(EncryptedKey.isAllPerfect + levelInfo.level.ToString()) < 1)
            {
                //클리어한 레벨
                
                for (int i = 0; i < levelInfo.completeQuaverCnt; i++)
                {
                    _quavers[i].sprite = quaverSpArr[1];
                }

                for (int i = levelInfo.completeQuaverCnt; i < _quavers.Length; i++)
                {
                    _quavers[i].sprite = quaverSpArr[0];
                }

                allPerfectImg.gameObject.SetActive(false);
            }
            else
            {
                //올퍼펙트
                for (int i = 0; i < levelInfo.completeQuaverCnt; i++)
                {
                    _quavers[i].sprite = quaverSpArr[1];
                }

                allPerfectImg.gameObject.SetActive(true);
            }
        }
        else if (levelInfo.level == GamePlayData.Instance.maxStage)
        {
            //현재 레벨
            _stateImages[0].gameObject.SetActive(false);
            _stateImages[1].gameObject.SetActive(true);
            _stateImages[2].gameObject.SetActive(false);
            _quaverObject.SetActive(false);

            GetComponent<Button>().interactable = true;
            allPerfectImg.gameObject.SetActive(false);
            for (int i = 0; i < _quavers.Length; i++)
            {
                _quavers[i].sprite = quaverSpArr[0];
            }
        }
        else
        {
            //클리어안함

            _stateImages[0].gameObject.SetActive(true);
            _stateImages[1].gameObject.SetActive(false);
            _stateImages[2].gameObject.SetActive(false);
            _quaverObject.SetActive(false);
            if (SceneSwitcher.IsBonusStage(levelInfo.level))
            {
                _stateImages[0].sprite = _bonusSp[0];
                _levelNumber.text = string.Empty;
            }
            else if (levelInfo.level == townInfo.levelAmount)
            {
                _stateImages[0].sprite = _bossSp[0];
                _levelNumber.text = string.Empty;
            }
            else
            {
                _stateImages[0].sprite = _normalSp[0];
            }

            GetComponent<Button>().interactable = false;
            allPerfectImg.gameObject.SetActive(false);

            for (int i = 0; i < _quavers.Length; i++)
            {
                _quavers[i].sprite = quaverSpArr[0];
            }
        }

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(delegate {
            DebugX.Log(itemCount);
            _playCurLevelCanvas.ShowPopup(townInfo, levelInfo);
        });
    }
}
