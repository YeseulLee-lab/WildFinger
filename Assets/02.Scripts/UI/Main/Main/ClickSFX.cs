using System;
using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;
using FMOD.Studio;

public class ClickSFX : MonoBehaviour, IPointerClickHandler
{

    [SerializeField]
    private EventReference _clickSfx;
    private EventInstance _clickSfxInstance;

    private void Awake()
    {
        _clickSfxInstance = RuntimeManager.CreateInstance(_clickSfx);
    }

    private void Start()
    {
        if (GamePlayData.Instance != null)
        {
            _clickSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    private void OnDestroy()
    {
        _clickSfxInstance.setUserData(IntPtr.Zero);
        _clickSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _clickSfxInstance.release();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GamePlayData.Instance == null)
        {
            return;
        }

        _clickSfxInstance.start();
    }
}
