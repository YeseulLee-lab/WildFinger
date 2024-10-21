using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using TMPro;
using UnityEngine.Video;
using DG.Tweening;
using System;

public class BaseRSPInputManager : MonoBehaviour
{
    [SerializeField]
    private EventReference _clickSfx;
    private EventInstance _clickInstance;
    [HideInInspector]
    public bool isARockActive = false;
    [HideInInspector]
    public bool isSScissorActive = false;
    [HideInInspector]
    public bool isDPaperActive = false;

    [Header("------------------ Input Setting -----------------")]
    [SerializeField]
    private BaseInputUI[] _inputRSPUIs;

    private float doubleClickThreshold = 0.3f;
    private Dictionary<KeyCode, float> lastKeyPressTime = new Dictionary<KeyCode, float>();
    private Dictionary<KeyCode, int> keyPressCount = new Dictionary<KeyCode, int>();
    public bool isEnteringReady { get; set; } = false;

    #region Unity Life Cycle
    public virtual void Awake()
    {
        _clickInstance = RuntimeManager.CreateInstance(_clickSfx);
    }

    public virtual void Update()
    {
        HandleKeyInput(KeyCode.A, KeyCode.LeftArrow, Define.RSPType.Rock, ref isARockActive);
        HandleKeyInput(KeyCode.S, KeyCode.DownArrow, Define.RSPType.Scissor, ref isSScissorActive);
        HandleKeyInput(KeyCode.D, KeyCode.RightArrow, Define.RSPType.Paper, ref isDPaperActive);
    }

    private void HandleKeyInput(KeyCode primaryKey, KeyCode secondaryKey, Define.RSPType rspType, ref bool isActive)
    {
        if (!isEnteringReady)
        {
            return;
        }

        if ((Input.GetKeyDown(primaryKey) || Input.GetKeyDown(secondaryKey)) && !isActive)
        {
            OnPointerDown((int)rspType, _inputRSPUIs[(int)rspType]);
            isActive = true;
            RegisterKeyPress(primaryKey);
            RegisterKeyPress(secondaryKey);
        }
        else if (!Input.GetKey(primaryKey) && !Input.GetKey(secondaryKey) && isActive)
        {
            OnPointerUp((int)rspType, _inputRSPUIs[(int)rspType]);
            isActive = false;
        }


        if(BeatGridTracker.Instance == null)
        {
            return;
        }

        if (CheckDoubleClick(primaryKey) || CheckDoubleClick(secondaryKey))
        {
            BeatGridTracker.Instance.judgeChecker.JudgeNoteFlick((int)rspType);
        }
    }

    private void RegisterKeyPress(KeyCode key)
    {
        if (!lastKeyPressTime.ContainsKey(key))
        {
            lastKeyPressTime[key] = Time.time;
            keyPressCount[key] = 1;
        }
        else
        {
            if (Time.time - lastKeyPressTime[key] <= doubleClickThreshold)
            {
                keyPressCount[key]++;
            }
            else
            {
                keyPressCount[key] = 1;
            }
            lastKeyPressTime[key] = Time.time;
        }
    }

    private bool CheckDoubleClick(KeyCode key)
    {
        if (keyPressCount.ContainsKey(key) && keyPressCount[key] >= 2)
        {
            keyPressCount[key] = 0;
            return true;
        }
        return false;
    }

    public virtual void OnDestroy()
    {
        _clickInstance.setUserData(IntPtr.Zero);
        _clickInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _clickInstance.release();
        _inputRSPUIs = null;
    }

    public virtual void Start()
    {
        if (GamePlayData.Instance != null)
        {
            _clickInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }
    #endregion

    #region UI Action
    public virtual void OnPointerDown(int type, BaseInputUI input)
    {
        //DebugX.Log($"OnPointerDown: {type}");
        _clickInstance.start();
        input.BtnPressAnim(true);
    }

    public virtual void OnPointerUp(int type, BaseInputUI input)
    {
        input.BtnPressAnim(false);
    }

    /// <summary>
    /// 폭발/동작 후 틀어짐 방지(버튼 원위치로)
    /// </summary>
    public void ResetBtnUI(int mSec = 0)
    {
        for(int i=0; i< _inputRSPUIs.Length; i++)
        {
            _inputRSPUIs[i].ResetBtnUI(mSec);
        }
    }
    #endregion

    #region InGame Tutorial
    public RectTransform GetInputBtnRect(Define.RSPType type)
    {
        return _inputRSPUIs[(int)type].GetComponent<RectTransform>();
    }
    #endregion
}