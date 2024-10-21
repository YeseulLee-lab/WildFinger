using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HHK.UIEC;
using FMODUnity;
using FMOD.Studio;
using System;
using DG.Tweening;

public class ComboUIManager : MonoBehaviour
{
    [Header("--------------------- UI --------------------")]
    [SerializeField] 
    private GameObject _comboLayout;
    [SerializeField]
    private UIECAnimator _comboAnim;
    [SerializeField]
    private Image[] _comboImgs; //3자리 까지만, 이후는 999로 표기 예정
    [SerializeField]
    private Sprite[] _comboNums; // 0~9
    [SerializeField]
    private CanvasGroup _comboTenHealingCG;
    private const float _comboTenDelay = 0.5f;
    private int _curCombo = 0;
    public int curCombo { get { return _curCombo; } 
        set {
            _curCombo = value;
            SetComboUI(_curCombo);

            if(_curCombo == 11 && !TownDataLoader.isTraining)
            {
                BeatGridTracker.Instance.feverManager.StartFeverTime(Define.FeverType.Pinata);
            }
        } }
    private const int _maxComboLength = 999;

    [Header("-------------------- FMOD ---------------------")]
    [SerializeField]
    private EventReference _comboStartSFX;
    [SerializeField]
    private EventReference _comboTenSFX;
    private EventInstance _comboStartInstance;
    private EventInstance _comboTenInstance;

    private void Awake()
    {
        _comboStartInstance = RuntimeManager.CreateInstance(_comboStartSFX);
        _comboTenInstance = RuntimeManager.CreateInstance(_comboTenSFX);
    }

    private void OnDestroy()
    {
        _comboStartInstance.setUserData(IntPtr.Zero);
        _comboStartInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _comboStartInstance.release();
        _comboTenInstance.setUserData(IntPtr.Zero);
        _comboTenInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _comboTenInstance.release();
    }

    private void Start()
    {
        if (GamePlayData.Instance != null)
        {
            _comboStartInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _comboTenInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    private void SetComboUI(int combo)
    {
        if(combo < 2)
        {
            _comboLayout.SetActive(false);
            return;
        }
        _comboLayout.SetActive(true);

        int tempCombo = combo - 1; // 0부터 시작하도록 보정
        //DebugX.Log("tempCombo: " + tempCombo);
        if (tempCombo >= _maxComboLength)
        {
            _comboImgs[0].gameObject.SetActive(true);
            _comboImgs[0].sprite = _comboNums[9];
            _comboImgs[1].gameObject.SetActive(true);
            _comboImgs[1].sprite = _comboNums[9];
            _comboImgs[2].gameObject.SetActive(true);
            _comboImgs[2].sprite = _comboNums[9];
            return;
        }

        // 숫자를 자릿수별로 나누어 배열에 저장
        int[] digits = new int[3];
        for (int i = 0; i < digits.Length; i++)
        {
            digits[i] = tempCombo % 10;
            tempCombo /= 10;
            //DebugX.Log($"[Combo] digits[{i}]: {digits[i]}");
        }
        tempCombo = combo - 1;
        SetComboNumUI(digits[0], _comboImgs[0], tempCombo > 9); //1의자리
        SetComboNumUI(digits[1], _comboImgs[1], tempCombo > 99); //10의자리
        SetComboNumUI(digits[2], _comboImgs[2]);

        _comboAnim.OnCustomChannel();

        if (tempCombo == 1)
        {
            _comboStartInstance.start();
        }
        else if (tempCombo % 10 == 0)
        {
            ShowTenComboUI();
        }
    }

    public void ShowTenComboUI()
    {
        BeatGridTracker.Instance.judgeChecker.itemManager.ShowItemStateAnim(Define.UsingItemBeforeInGame.IncreasedHealingHP);
        _comboTenInstance.start();
        _comboTenHealingCG.DOFade(1f, _comboTenDelay).OnComplete(() =>
        {
            _comboTenHealingCG.DOFade(0f, 0.1f);
        });
    }

    /// <summary>
    /// 콤보 UI Sprite 이미지로 바꿔줌
    /// </summary>
    /// <param name="num">0~9</param>
    /// <param name="isNextExist">자기보다 높은 자리 숫자가 0이 아님</param>
    /// <param name="img"></param>
    private void SetComboNumUI(int num, Image img, bool isNextExist = false)
    {
        if(num < 1 && !isNextExist)
        {
            img.gameObject.SetActive(false);
            return;
        }

        img.gameObject.SetActive(true);
        img.sprite = _comboNums[num];
    }
}
