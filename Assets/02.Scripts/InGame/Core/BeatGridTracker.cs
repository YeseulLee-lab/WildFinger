using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Video;
using FMODUnity;
using FMOD.Studio;
using DG.Tweening;
using HHK.UIEC;

[StructLayout(LayoutKind.Sequential)]
public class TimelineInfo
{
    public int currentBeat = 0;
    public int currentBar = 0;
    public int beatPosition = 0;
    public float currentTempo = 0;
    public float lastTempo = 0;
    public int currentPos = 0;
    public double songLength = 0;
    public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
}

public class BeatGridTracker : MonoBehaviour
{
    public static BeatGridTracker Instance { get; private set; }
    [SerializeField]
    private InGameCutSceneManager _cutSceneManager;
    public static InGameCutSceneManager cutSceneManager;
    [field: SerializeField]
    public InGameTutorialManager tutorialManager { get; set; }
    public static InGameTutorialManager tutorialManagerStc;
    [field: SerializeField]
    public InGameFeverManager feverManager;
    [field: SerializeField]
    public MonsterManager monsterManager;

    [Header("------------------ Options -----------------")]
    [SerializeField]
    private UIECAnimator[] _damagedAnims;
    public float upBeatDivisor = 2f; // This value changes the offset of the up beats. Changing this value will "swing" the up beats.
    public static JsonNoteLoader noteLoader { get; private set; }
    public static InGameNoteRoadManager noteRoadManager { get; private set; }
    public static BaseRSPInputManager rspInputManager { get; private set; }
    [SerializeField]
    private JudgeChecker _judgeChecker;
    public JudgeChecker judgeChecker => _judgeChecker;
    private int _inGameMaxHP;
    public int inGameMaxHP { get { return _inGameMaxHP; } set {
            _inGameMaxHP = value;
            _judgeChecker.hpManager.SetHP(_inGameMaxHP);
        } }
    private int _inGameHealingHP;
    public int inGameHealingHP
    {
        get { return _inGameHealingHP; }
        set
        {
            _inGameHealingHP = value;
        }
    }

    [Header("------------------- Game Data -------------------")]
    public static int lastBeat = 0;
    public static string lastMarkerString = null;
    public static bool isStart { get; set; } = false;
    public static bool isStoped { get; set; } = false;
    private int masterSampleRate;
    private double currentSamples = 0;
    private double currentTime = 0f;
    private double _musicStartTime = 0f;
    public static Define.InGameState curState { get; private set; }
    public static float beatPerSec { get; set; }

    [Header("------------------- Beat Tracking -------------------")]
    private static bool justHitBeat = false;
    private double tempoTrackDSPStartTime;
    private static string markerString = "";
    private static bool justHitMarker = false;
    private static int markerTime;

    private ulong dspClock;
    private ulong parentDSP;

    public delegate void BeatEventDelegate();
    public static event BeatEventDelegate fixedBeatUpdate; // Subscribe any function you wan't to happen on the down beat to this event! DON'T FORGET TO UNSUBSCRIBE BEFORE DESTROYING YOU GAMEOBJECTS!

    private double lastFixedBeatTime = -2;
    private double lastFixedBeatDSPTime = -2;

    public static event BeatEventDelegate upBeatUpdate; // Subscribe any function you wan't to happen on the up beat to this event.

    private double lastUpBeatTime = -2;
    private double lastUpBeatDSPTime = -2;

    private bool hasDoneEnemyBeat = false;

    private static double beatInterval = 0f; // This is the time between each beat;
    private static double lastBeatInterval = 0f; // This is the previous time between each beat. It's what the "beatInterval" was before a tempo change.

    [Header("------------------ FMOD Setting -----------------")]
    private FMOD.ChannelGroup masterChannelGroup;
    public delegate void TempoUpdateDelegate(float beatInterval);
    public static event TempoUpdateDelegate tempoChanged;

    public delegate void MarkerListenerDelegate();
    public static event MarkerListenerDelegate markerUpdated;

    private FMOD.Studio.PLAYBACK_STATE musicPlayState;
    private FMOD.Studio.PLAYBACK_STATE lastMusicPlayState;
    [SerializeField]
    private EventReference _beatBGM;
    private static EventInstance _beatInstance;

    [Header("-------------------- Pause ---------------------")]
    [SerializeField]
    private Button _pauseBtn;
    [SerializeField]
    private GamePause _pauseManager;

    [Header("-------------------- Result ---------------------")]
    [SerializeField]
    private GameResult _resultPopup;
    public static GameResult resultPopup;
    [SerializeField]
    private EventReference _resultBGM;
    private static EventInstance _resultInstance;
    public TimelineInfo timelineInfo = null;

    [Header("------------------ FMOD Handler -----------------")]
    private static GCHandle timelineHandle;
    private FMOD.Studio.EVENT_CALLBACK beatCallback;
    private FMOD.Studio.EventDescription descriptionCallback;
    public static FMOD.Studio.EventInstance musicPlayEvent;

    #region Unity Life Cycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        noteLoader = this.GetComponent<JsonNoteLoader>();
        noteRoadManager = this.GetComponent<InGameNoteRoadManager>();
        resultPopup = _resultPopup;
        _resultInstance = RuntimeManager.CreateInstance(_resultBGM);
        _beatInstance = RuntimeManager.CreateInstance(_beatBGM);
        curState = Define.InGameState.Waiting;
        rspInputManager = this.GetComponent<BaseRSPInputManager>();
        cutSceneManager = _cutSceneManager;
        tutorialManagerStc = tutorialManager;

        BeatGridTracker.SetGameState(Define.InGameState.Waiting);
        SetPauseBtn(false);
    }

    private void Update()
    {
        if (!isStart || isStoped)
        {
            return;
        }

        musicPlayEvent.getPlaybackState(out musicPlayState);

        if (lastMusicPlayState != FMOD.Studio.PLAYBACK_STATE.PLAYING && musicPlayState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            SetTrackStartInfo();
        }

        lastMusicPlayState = musicPlayState;

        if (musicPlayState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            return;
        }

        musicPlayEvent.getTimelinePosition(out timelineInfo.currentPos);

        UpdateDSPClock();
        CheckTempoMarkers();

        if (beatInterval == 0f)
        {
            return;
        }

        if (justHitMarker)
        {
            justHitMarker = false;

            if (lastFixedBeatDSPTime < currentTime - (beatInterval / 2f))
            {
                DoFixedBeat(); // We trigger the beat immediately if we're far enough past the last beat. This will help correct the timing when we hit a marker;
            }

            musicPlayEvent.getTimelinePosition(out int currentTimelinePos);

            float offset = (currentTimelinePos - markerTime) * 0.001f;

            tempoTrackDSPStartTime = currentTime - offset;
            lastFixedBeatTime = 0f;
            lastFixedBeatDSPTime = tempoTrackDSPStartTime;

            lastUpBeatTime = 0f;
            lastUpBeatDSPTime = tempoTrackDSPStartTime;

            if (markerUpdated != null)
            {
                markerUpdated();
            }
        }
        CheckNextBeat();
    }

    private async void Start()
    {
        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _resultInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _beatInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }

        await UniTask.WaitUntil(() => TownDataLoader.isDone);
        monsterManager.SpawnMonster(TownDataLoader.town);
        await UniTask.WaitUntil(() => judgeChecker != null);
        await UniTask.WaitUntil(() => monsterManager.isReady);
        noteLoader.LoadNoteData(StartGame);
        _cutSceneManager.ShowStarting();
        judgeChecker.Init();
    }

    private void OnDestroy()
    {
        _resultInstance.setUserData(IntPtr.Zero);
        _resultInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _resultInstance.release();
        _beatInstance.setUserData(IntPtr.Zero);
        _beatInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _beatInstance.release();
        musicPlayEvent.setUserData(IntPtr.Zero);
        musicPlayEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicPlayEvent.release();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus && curState != Define.InGameState.End
            && curState != Define.InGameState.None
            && curState != Define.InGameState.Waiting) //!InGameTutorialManager.Instance.isTutorial
        {
            OnClickPauseBtn();
            judgeChecker.judgeBeatTimer.SetJudgeBeatTimerState(true);
            feverManager.SetFeverTimerState(true);
        }
    }
    #endregion

    #region UI Action
    public async void StartGame()
    {
        //DebugX.Log("StartMusic: " + currentTime);
        await UniTask.WaitUntil(() => noteLoader.isDone);
        await UniTask.WaitUntil(() => tutorialManager.isReady);
        await UniTask.WaitUntil(() => judgeChecker.itemManager.isItemInitDone);
        isStart = true;
        FMOD.Studio.EventDescription des;
        musicPlayEvent.getDescription(out des);
        des.loadSampleData();
        musicPlayEvent = RuntimeManager.CreateInstance(TownDataLoader.curMusicInfo.music);

        //볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            musicPlayEvent.setVolume(GamePlayData.Instance.isCommonBGMOn ? 1f : 0f);
        }

        musicPlayEvent.start();
        AssignMusicCallbacks();
        //playerBeatUpdate += DoPlayerBeat;
        SetPauseBtn(true);
        _pauseBtn?.onClick.RemoveAllListeners();
        _pauseBtn?.onClick.AddListener(OnClickPauseBtn);
        //Note
        noteRoadManager.StartNoteMoving(TownDataLoader.curMusicInfo, noteLoader.stageInfo.distancePerMarker, noteLoader.noteQueue);
        BeatGridTracker.SetGameState(Define.InGameState.Playing);
    }

    public static void EndGame()
    {
        SetGameState(Define.InGameState.End);
        noteRoadManager.EndNoteMoving();
        isStart = false;
        timelineHandle.Free();
        musicPlayEvent.setUserData(IntPtr.Zero);
        musicPlayEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicPlayEvent.release();
        noteLoader.ReleaseNote();
    }

    public void OnClickPauseBtn()
    {
        //DebugX.Log("OnClickPauseBtn curState: " + curState);
        if (curState == Define.InGameState.Paused)
        {
            return;
        }

        SetGameState(Define.InGameState.Paused);
        _pauseManager.ShowPause();
        
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }
    }

    public static async void ShowResult()
    {
        await cutSceneManager.ShowEnding();
        resultPopup.ShowPopup(TownDataLoader.isTraining);
        //_resultInstance.start();
        SetGameState(Define.InGameState.End);
        EndGame();
    }

    public void ShakeCam(Define.InGameShakeScale scale = Define.InGameShakeScale.Small)
    {
        _damagedAnims[(int)scale].OnCustomChannel();

        //TODO: 버튼 틀어짐 정상화
        switch (scale)
        {
            default:
            case Define.InGameShakeScale.Small:
                rspInputManager.ResetBtnUI(InGameKey.shakeSmallEffectMS);
                break;
            case Define.InGameShakeScale.Midium:
            case Define.InGameShakeScale.Large:
                rspInputManager.ResetBtnUI(InGameKey.shakeMidLargeEffectMS);
                break;
        }
    }

    public void SetPauseBtn(bool active)
    {
        _pauseBtn.gameObject.SetActive(active);
    }
    #endregion

    #region FMOD Setting
    private void AssignMusicCallbacks()
    {
        timelineInfo = new TimelineInfo();
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
        musicPlayEvent.setUserData(GCHandle.ToIntPtr(timelineHandle));
        musicPlayEvent.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        musicPlayEvent.getDescription(out descriptionCallback);
        descriptionCallback.getLength(out int length);

        timelineInfo.songLength = length;

        FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out masterChannelGroup);
        FMODUnity.RuntimeManager.CoreSystem.getSoftwareFormat(out masterSampleRate, out FMOD.SPEAKERMODE speakerMode, out int numRawSpeakers);
    }

    public static void SetGameState(Define.InGameState state)
    {
        curState = state;

        switch (state)
        {
            default:
                rspInputManager.isEnteringReady = false;
                break;
            case Define.InGameState.Playing:
            case Define.InGameState.FeverTime: //추가됨, 다른요소 확인
                rspInputManager.isEnteringReady = true;
                break;
            case Define.InGameState.Paused:
                isStoped = true;
                musicPlayEvent.setPaused(true);
                rspInputManager.isEnteringReady = false;
                break;
            case Define.InGameState.Resumed:
                isStoped = false;
                musicPlayEvent.setPaused(false);
                SetGameState(Define.InGameState.Playing);
                Instance.judgeChecker.judgeBeatTimer.SetJudgeBeatTimerState(false);
                Instance.feverManager.SetFeverTimerState(false);
                break;
            case Define.InGameState.PausedCntDown:
                //처리
                rspInputManager.isEnteringReady = false;
                break;
        }
    }

    private void SetTrackStartInfo()
    {
        UpdateDSPClock();

        tempoTrackDSPStartTime = currentTime;
        lastFixedBeatTime = 0f;
        lastFixedBeatDSPTime = currentTime;
    }

    private bool CheckTempoMarkers()
    {
        if (timelineInfo.currentTempo != timelineInfo.lastTempo)
        {
            SetTrackTempo();

            return true;
        }

        return false;
    }

    private void SetTrackTempo()
    {
        musicPlayEvent.getTimelinePosition(out int currentTimelinePos);

        float offset = (currentTimelinePos - timelineInfo.beatPosition) / 1000f;

        tempoTrackDSPStartTime = currentTime - offset;
        lastFixedBeatTime = 0f;
        lastFixedBeatDSPTime = tempoTrackDSPStartTime;

        lastUpBeatTime = 0f;
        lastUpBeatDSPTime = tempoTrackDSPStartTime;

        lastBeatInterval = beatInterval;
        timelineInfo.lastTempo = timelineInfo.currentTempo;

        beatInterval = 60f / timelineInfo.currentTempo;

        if (tempoChanged != null)
        {
            tempoChanged((float)beatInterval);
        }
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    private static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        FMOD.RESULT result = instance.getUserData(out IntPtr timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            DebugX.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        // There's more info about the callback in the "parameter" variable.
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentBar = parameter.bar;
                        timelineInfo.currentBeat = parameter.beat;
                        timelineInfo.beatPosition = parameter.position;
                        timelineInfo.currentTempo = parameter.tempo;
                        beatInterval = timelineInfo.currentTempo / 60f;
                        justHitBeat = true;
                        _beatInstance.start();
                        noteRoadManager.NextNoteMoving();
                        beatPerSec = 60f / timelineInfo.currentTempo;
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        // Same here.
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));

                        timelineInfo.lastMarker = parameter.name;
                        markerString = parameter.name;
                        markerTime = parameter.position;
                        justHitMarker = true;
                        beatInterval = timelineInfo.currentTempo / 60f;

                        if (((string)parameter.name).Contains(Define.NoteType.End.ToString()))
                        {
                            ShowResult();
                        }
                        else if (((string)parameter.name).Contains(Define.NoteType.S_.ToString()))
                        {
                            JsonNoteLoader.gameRuleUIManager.SetSituationUI((int)JsonNoteLoader.SetLogicTypeS((string)parameter.name));
                        }
                        else if (((string)parameter.name).Contains(Define.NoteType.Stop.ToString()))
                        {
                            //Tutorial, 임의로 일단 함
                            if(tutorialManagerStc != null)
                            {
                                tutorialManagerStc.StartTutorial(JsonNoteLoader.SetGameTutorialType((string)parameter.name));
                            }
                        }
                    }
                    break;
            }
        }
        return FMOD.RESULT.OK;
    }
    #endregion

    #region Beat Tracker
    private void UpdateDSPClock()
    {
        masterChannelGroup.getDSPClock(out dspClock, out parentDSP);

        currentSamples = dspClock;

        if (_musicStartTime <= 0)
        {
            _musicStartTime = currentTime;
            //DebugX.Log($"_musicStartTime: {_musicStartTime}");
        }
        currentTime = currentSamples / masterSampleRate;
        //DebugX.Log($"dspClock: {dspClock}, currentTime: {currentTime}");
    }

    private float UpBeatPosition()
    {
        return ((float)beatInterval / upBeatDivisor);
    }

    private void CheckNextBeat()
    {

        float fixedSongPosition = (float)(currentTime - tempoTrackDSPStartTime);
        float upBeatSongPosition = fixedSongPosition + UpBeatPosition();

        // FIXED BEAT (down beat)
        if (fixedSongPosition >= lastFixedBeatTime + beatInterval)
        {
            //DebugX.Log("FIXED BEAT (down beat)");
            float correctionAmount = Mathf.Repeat(fixedSongPosition, (float)beatInterval); // This is the amount of time that we're off from the beat...

            DoFixedBeat();

            lastFixedBeatTime = (fixedSongPosition - correctionAmount); // ... we subtract that time from the current time to correct the timing off the next beat.
            lastFixedBeatDSPTime = (currentTime - correctionAmount); // So if this beat is late by 0.1 seconds, the next beat will happen 0.1 seconds sooner.

        }

        // UP BEAT
        if (upBeatSongPosition >= lastUpBeatTime + beatInterval)
        {
            //DebugX.Log("UP BEAT");
            float correctionAmount = Mathf.Repeat(upBeatSongPosition, (float)beatInterval);

            DoUpBeat();

            lastUpBeatTime = (upBeatSongPosition - correctionAmount);
            lastUpBeatDSPTime = ((currentTime + UpBeatPosition()) - correctionAmount);
        }
    }

    private void DoFixedBeat()
    {
        if (fixedBeatUpdate != null)
        {
            fixedBeatUpdate();
        }
    }

    private void DoUpBeat()
    {

        if (upBeatUpdate != null)
        {
            upBeatUpdate();
        }
    }
    #endregion
}
