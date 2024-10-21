using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

public class DebugModeManager : MonoBehaviour
{
    [Header("------------------ Button Setting -----------------")]
    [SerializeField]
    private Button _settingForcedLevelBtn;
    [SerializeField]
    private Button _addHeartOneBtn;
    [SerializeField]
    private Button _forcedLvSavingBtn;
    [SerializeField]
    private Button _forcedLvBackgroundBtn;
    [SerializeField]
    private Button _forcedLvCloseBtn;

    [Header("------------------ Layout Setting -----------------")]
    [SerializeField]
    private GameObject _forcedLvCanvas;
    [SerializeField]
    private GameObject _forcedLvPopup;
    [SerializeField]
    private InputField _forcedLvInputField;
    [SerializeField]
    private Text _forcedLvFailedText;
    public string forcedLv => _forcedLvInputField.text;

    private void OnEnable()
    {
        _forcedLvCanvas.SetActive(false);
#if DEBUGMODE
        _forcedLvCanvas.SetActive(true);
        Init();
#endif
        GamePlayData.Instance.admobManager.ShowBanner();
    }

    private void Start()
    {
#if DEBUGMODE
        _settingForcedLevelBtn?.onClick.AddListener(() => {
            GamePlayData.Instance.OnClickBtnEffect();
            _forcedLvPopup?.SetActive(true);
        });
        _forcedLvBackgroundBtn?.onClick.AddListener(() => {
            GamePlayData.Instance.OnClickBtnEffect();
            _forcedLvPopup?.SetActive(false);
        });
        _forcedLvCloseBtn?.onClick.AddListener(() => {
            GamePlayData.Instance.OnClickBtnEffect();
            _forcedLvPopup?.SetActive(false);
        });
        _forcedLvSavingBtn?.onClick.AddListener(OnClickForcedLvSavingBtn);
        _addHeartOneBtn?.onClick.AddListener(() => {
            GamePlayData.Instance.OnClickBtnEffect();
            if(GamePlayData.Instance.heartTimer.heartCnt >= OutGameInfo.maxHeartCnt)
            {
                GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.FullHeart, true);
                return;
            }
            GamePlayData.Instance.heartTimer.heartCnt++;
        });
        GamePlayData.Instance.isDebugMode = true;
#endif
        GamePlayData.Instance.admobManager.CheckInternetState(5);
    }

#if DEBUGMODE
    private void Init()
    {
        _forcedLvFailedText.text = "";
        _forcedLvPopup.SetActive(false);
    }

    private void OnClickForcedLvSavingBtn()
    {
        GamePlayData.Instance.OnClickBtnEffect();
        int forcedLv;

        if (int.TryParse(this.forcedLv, out forcedLv))
        {
            //TODO: 강제 레벨 세팅

            if(GamePlayData.Instance == null)
            {
                _forcedLvFailedText.text = "레벨 세팅 실패: GamePlayData.Instance NULL";
                return;
            }

            if(forcedLv > GamePlayData.Instance.maxDevelpedStage)
            {
                //자동으로 최대로 설정함
                forcedLv = GamePlayData.Instance.maxDevelpedStage + 1;
            }else if(forcedLv < 1)
            {
                //자동으로 최소로 설정함
                forcedLv = 1;
            }

            //레벨 설정
            GamePlayData.Instance.maxStage = forcedLv;
            Define.TownList beforeTown = GamePlayData.Instance.maxTown;//맥스타운 바뀌기전

            if(forcedLv <= 30)
            {
                //1 장난감
                GamePlayData.Instance.maxTown = Define.TownList.ToyTown;

                PlayerPrefs.SetInt(GamePlayData.Instance.maxTown.ToString() + MainTownKey.townLevelKey, forcedLv - 1);
            }
            else if(forcedLv <= 80)
            {
                //2 바이킹
                GamePlayData.Instance.maxTown = Define.TownList.Viking;

                PlayerPrefs.SetInt(GamePlayData.Instance.maxTown.ToString() + MainTownKey.townLevelKey, forcedLv - 30 - 1);
            }
            else if (forcedLv <= 140)
            {
                //3 우주
                GamePlayData.Instance.maxTown = Define.TownList.Space;

                PlayerPrefs.SetInt(GamePlayData.Instance.maxTown.ToString() + MainTownKey.townLevelKey, forcedLv - 80 - 1);
            }
            else if (forcedLv <= 200)
            {
                //4 네온EDM
                GamePlayData.Instance.maxTown = Define.TownList.NeonEDM;

                PlayerPrefs.SetInt(GamePlayData.Instance.maxTown.ToString() + MainTownKey.townLevelKey, forcedLv - 140 - 1);
            }
            else if (forcedLv <= 260)
            {
                //5 아라비안
                GamePlayData.Instance.maxTown = Define.TownList.ArabianDesert;

                PlayerPrefs.SetInt(GamePlayData.Instance.maxTown.ToString() + MainTownKey.townLevelKey, forcedLv - 200 - 1);
            }
            else if (forcedLv <= 320)
            {
                //6 트로피컬
                GamePlayData.Instance.maxTown = Define.TownList.TropicalBeach;

                PlayerPrefs.SetInt(GamePlayData.Instance.maxTown.ToString() + MainTownKey.townLevelKey, forcedLv - 260 - 1);
            }
            else if (forcedLv <= 380)
            {
                //7 재즈카페
                GamePlayData.Instance.maxTown = Define.TownList.JazzCafe;

                PlayerPrefs.SetInt(GamePlayData.Instance.maxTown.ToString() + MainTownKey.townLevelKey, forcedLv - 320 - 1);
            }
            else
            {
                //8 할로윈
                GamePlayData.Instance.maxTown = Define.TownList.Halloween;

                PlayerPrefs.SetInt(GamePlayData.Instance.maxTown.ToString() + MainTownKey.townLevelKey, forcedLv - 380 - 1);
            }

            // 다음 마을로 넘어갈시 애셋 데이터 초기화 필요
            if ((int)beforeTown < (int)GamePlayData.Instance.maxTown)
            {
                PlayerPrefs.SetInt("Tut" + Define.MainTutorialType.LandPage1, 1);
                PlayerPrefs.SetInt("Tut" + Define.MainTutorialType.LandPage2, 1);
                PlayerPrefs.SetInt("Tut" + Define.MainTutorialType.NextLand, 1);
                PlayerPrefs.SetInt("Tut" + Define.MainTutorialType.MainItemHPPotion, 1);
                PlayerPrefs.SetInt("Tut" + Define.MainTutorialType.MainItemIncreaseHP, 1);
                PlayerPrefs.SetInt("Tut" + Define.MainTutorialType.MainItemShield, 1);

                GamePlayData.Instance.maxAssetIdx = 0;
            }

            //이전 마을 누적
            if (GamePlayData.Instance.maxTown != Define.TownList.ToyTown)
            {
                for (int i = 0; i < (int)GamePlayData.Instance.maxTown; i++)
                {
                    PlayerPrefs.SetInt(((Define.TownList)i).ToString() + MainTownKey.townLevelKey, GamePlayData.Instance.GetTownInfo(((Define.TownList)i)).levelAmount);
                }
            }

            //음표 최대로 받은 것으로 설정
            for (int i=1; i< forcedLv; i++)
            {
                PlayerPrefs.SetInt(EncryptedKey.score + i.ToString(), 3);
                PlayerPrefs.SetInt(EncryptedKey.isAllPerfect + i.ToString(), 1);
            }
            GamePlayData.Instance.recordQuaverCnt = (forcedLv - 1) * 3;
            PlayerPrefs.SetInt(EncryptedKey.remainQuaverCnt, (forcedLv - 1) * 3);

            _forcedLvFailedText.text = forcedLv.ToString() + " 레벨 세팅 성공. 어플 종료 후 다시 실행합니다.";
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            Application.Quit();
#elif UNITY_EDITOR && (!UNITY_ANDROID || !UNITY_IOS)
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else
        {
            //실패
            _forcedLvFailedText.text = "레벨 세팅 실패: 잘못된 레벨 값(정수형)";
        }
    }
#endif
}
