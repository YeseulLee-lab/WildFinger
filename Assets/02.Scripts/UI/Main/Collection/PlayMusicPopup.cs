using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using HHK.UIEC;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayMusicPopup : MonoBehaviour
{
    [Header("------------ Buttons ------------")]
    [SerializeField]
    private Button _background;
    [SerializeField]
    private Button _playBtn;
    [SerializeField]
    private Image _playImg;

    [Header("------------ Image Set ------------")]
    [SerializeField]
    private Sprite _playSP;//일시정지중일때
    [SerializeField]
    private Sprite _pauseSP;//플레이중일때

    [Header("------------ Music Data ------------")]
    [SerializeField]
    private Image _musicThumb;
    [SerializeField]
    private UnityEngine.UI.Text _musicName;

    [Header("------------ Popup ------------")]
    [SerializeField]
    private GameObject _musicListPopup;

    [Header("---------- Fmod Area ----------")]
    [SerializeField]
    private EventReference _collectMusicBGM;
    private EventInstance _collectMusicBGMInstance;
    [SerializeField]
    private EventReference _playMusicSfx;
    private EventInstance _playMusicSfxInstance;

    private bool isPlaying;

    private void Start()
    {
        _playMusicSfxInstance = RuntimeManager.CreateInstance(_playMusicSfx);

        _playBtn.onClick.AddListener(() => 
        {
            PlayMusic();
        });
        _background.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.MainTutorialType.CollectionPage3, null,
                        new RectTransform[] { _playBtn.GetComponent<RectTransform>() });
    }

    private void OnEnable()
    {
        MainUIManager.Instance.settingCanvas.PauseMainBGM();
        GetComponent<UIECAnimator>().OnCustomChannel();
    }

    private void OnDisable()
    {
        _collectMusicBGMInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _musicThumb.GetComponent<Animator>().speed = 0f;
        _musicListPopup.SetActive(true);
        MainUIManager.Instance.settingCanvas.PlayMainBGM();
    }

    private void OnDestroy()
    {
        _collectMusicBGMInstance.setUserData(IntPtr.Zero);
        _collectMusicBGMInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _collectMusicBGMInstance.release();

        _playMusicSfxInstance.setUserData(IntPtr.Zero);
        _playMusicSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _playMusicSfxInstance.release();
    }

    private void PlayMusic()
    {
        _playMusicSfxInstance.start();

        _collectMusicBGMInstance.setVolume(1f);
        _collectMusicBGMInstance.setPaused(isPlaying);
        isPlaying = !isPlaying;
        if (isPlaying)
        {
            _playImg.sprite = _pauseSP;
        }
        else
        {
            _playImg.sprite = _playSP;
        }
        
        _musicThumb.GetComponent<Animator>().speed = isPlaying? 1f : 0f;
    }

    public void SetData(CollectMusicInfo info)
    {
        //팝업 켜자마자 SetData
        isPlaying = true;
        _musicThumb.GetComponent<Animator>().speed = 1f;

        _collectMusicBGM = info.collectMusic;
        _musicThumb.sprite = info.collectMusicImage;
        _musicName.text = _musicName.GetComponent<LocalizationTextUI>().GetSummary(info.musicId);
        
        _collectMusicBGMInstance = RuntimeManager.CreateInstance(_collectMusicBGM);
        _collectMusicBGMInstance.start();
    }
}
