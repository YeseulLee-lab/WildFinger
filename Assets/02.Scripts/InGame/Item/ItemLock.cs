using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // 이 부분 추가
using FMODUnity;
using FMOD.Studio;

public class ItemLock : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private EventReference _lockSfx;
    private EventInstance _lockInstance;

    private void Awake()
    {
        _lockInstance = RuntimeManager.CreateInstance(_lockSfx);
    }

    private void Start()
    {
        if (GamePlayData.Instance != null)
        {
            _lockInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    private void OnDestroy()
    {
        _lockInstance.setUserData(IntPtr.Zero);
        _lockInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _lockInstance.release();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(GamePlayData.Instance == null)
        {
            return;
        }

        _lockInstance.start();
        GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.BoostUnavailable, true);
    }
}
