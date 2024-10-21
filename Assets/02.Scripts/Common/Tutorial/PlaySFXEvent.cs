using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFXEvent : MonoBehaviour
{
    [SerializeField]
    private EventReference _eventSfx;
    private EventInstance _eventSfxInstance;

    private void Start()
    {
        _eventSfxInstance = RuntimeManager.CreateInstance(_eventSfx);
    }

    private void PlaySFX()
    {
        _eventSfxInstance.start();
    }
}
