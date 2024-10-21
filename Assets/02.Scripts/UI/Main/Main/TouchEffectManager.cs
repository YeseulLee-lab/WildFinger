using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UI;

public class TouchEffectManager : MonoBehaviour
{
    [SerializeField]
    private RawImage touchEffectRawImage;
    [SerializeField]
    private ParticleSystem touchEffect;

    [Header("-------------------- FMOD ---------------------")]
    [SerializeField]
    private EventReference _touchSFX;
    private EventInstance _touchInstance;

    private void Awake()
    {
        _touchInstance = RuntimeManager.CreateInstance(_touchSFX);
        GetComponent<CanvasScaler>().referenceResolution = new Vector2(Screen.width, Screen.height);
    }

    private void Start()
    {
        if (GamePlayData.Instance != null)
        {
            _touchInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    private void OnDestroy()
    {
        _touchInstance.setUserData(IntPtr.Zero);
        _touchInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _touchInstance.release();

        touchEffectRawImage = null;
        touchEffect = null;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchEffect.Play();
            touchEffectRawImage.rectTransform.anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    public void PlayTouchEffect()
    {
        _touchInstance.start();
    }
}
