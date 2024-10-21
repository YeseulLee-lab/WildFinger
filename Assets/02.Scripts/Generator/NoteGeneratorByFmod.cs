using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

public class NoteGeneratorByFmod : StreamingAssetPather
{
    [SerializeField]
    private bool _endlessGeneratingMode = false;
    public static bool endlessGeneratingMode { get; set; }

    [SerializeField, Range(0f, 1f)]
    private float _musicVolume;
    private static float musicVolume;

    [Header("------------------ GUI Setting -----------------")]
    [SerializeField]
    private GameObject _noteParent;
    public static GameObject noteParent;
    [SerializeField]
    private UnityEngine.UI.Text _villageText;
    public static UnityEngine.UI.Text villageText;
    [SerializeField]
    private UnityEngine.UI.Text _levelText;
    public static UnityEngine.UI.Text levelText;
    public static int preSituationChildIndex { get; set; } = 0;
    private static GameObject _note = null;

    [Header("------------------ FMOD Setting -----------------")]
    private static EventInstance musicInstance;
    private static FMOD.Studio.EVENT_CALLBACK beatCallback;

    public static TimelineInfo timelineInfo = null;
    private static GCHandle timeLineHandle;

    public static int lastBeat = 0;
    public static string lastMarkerString = null;
    public static float tempo = 0; //bpm
    public static bool isReady { get; set; } = false;
    public static int curRuleIndex { get; set; } = 0;
    public static int temptRuleIndex { get; set; } = 0;

    [Header("------------------ Check Duplicated Marker -----------------")]
    private static int _prevLockCnt;
    private static int _curLockCnt; //해당 박자동안은 노트 생성 불가능

    [Header("------------------ FileIO Field -----------------")]
    private static StreamWriter _sw;
    private static StageInfo _stageInfo;

    #region Unity Life Cycle
    private void Awake()
    {
#if UNITY_EDITOR
        Application.runInBackground = true;
#endif

        if(noteParent == null)
        {
            noteParent = _noteParent;
        }

        endlessGeneratingMode = _endlessGeneratingMode;
        musicVolume = _musicVolume;
        villageText = _villageText;
        levelText = _levelText;
    }

    private async void Start()
    {
        await UniTask.WaitUntil(() => TownDataLoader.isDone);
        StartGenerating();
    }
    #endregion

    #region UI Action
    public static void StartGenerating()
    {
        if (!TownDataLoader.curMusicInfo.music.IsNull)
        {
            _curLockCnt = 0;
            _prevLockCnt = _curLockCnt;
            _stageInfo = new StageInfo();
            //_stageData.ruleType = TownDataLoader.curMusicInfo.ruleType;
            _stageInfo.totalBeatMarkerCnt = 0;
            _stageInfo.beatCnt = 1;
            villageText.text = TownDataLoader.town.ToString();
            levelText.text = TownDataLoader.level.ToString();
            timelineInfo = new TimelineInfo();
            beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
            timeLineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
            musicInstance = RuntimeManager.CreateInstance(TownDataLoader.curMusicInfo.music);
            musicInstance.start();
            musicInstance.setUserData(GCHandle.ToIntPtr(timeLineHandle));
            musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
            musicInstance.setVolume(musicVolume);
        }
    }

    public static async void EndGenerating()
    {
        _stageInfo.bpm = tempo;
        WriteJson();
        musicInstance.setUserData(IntPtr.Zero);
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
        timeLineHandle.Free();

        BaseObjectPool.Instance.ClearPool(noteParent);

        if (!endlessGeneratingMode || ++TownDataLoader.level > (TownDataLoader.stageDic.Count + TownDataLoader.curTownFirstLevel - 1))
        {
            Debug.Log($"{TownDataLoader.town} 노트 생성을 전부 완료하였습니다.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            return;
        }

        await TownDataLoader.SetTownData(TownDataLoader.town, TownDataLoader.level++);
        StartGenerating();
    }
    #endregion

    #region File I/O
    private static void WriteJson()
    {
        string filePath = GetFilePathCommon(TownDataLoader.town, TownDataLoader.level);

        // 경로가 없으면 폴더 생성
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        // 파일 생성
        _sw = new StreamWriter(filePath);
        _sw.WriteLine(JsonHelper.ObjectToJson(_stageInfo));
        _sw.Flush();
        _sw.Close();
        Debug.Log($"{TownDataLoader.town}-{TownDataLoader.level}, {filePath} =>  총 마커수: {_stageInfo.totalBeatMarkerCnt}, NoteData 저장 완료 Json");
    }
    #endregion

    #region FMOD Note Setting
    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    private static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new EventInstance(instancePtr);
        IntPtr timeLineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timeLineInfoPtr);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("TimeLine Callback Error: " + result);
        }
        else if (timeLineInfoPtr != IntPtr.Zero)
        {
            GCHandle timelineHandle = GCHandle.FromIntPtr(timeLineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;
            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        --_prevLockCnt;
                        _stageInfo.totalBeatMarkerCnt++;
                           var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentBeat = parameter.beat;
                        if (_stageInfo.beatCnt < parameter.beat)
                        {
                            _stageInfo.beatCnt = parameter.beat;
                        }

                        if(parameter.tempo > 0)
                        {
                            tempo = parameter.tempo;
                        }
                        
                        NoteInfo info = new NoteInfo() {
                            noteType = Define.NoteType.Bar,
                            rspTypes = new Define.RSPType[1] { Define.RSPType .None },
                            logicTypes = new Define.LogicType[1] { Define.LogicType.None },
                            inputTypes = new Define.InputType[1] { Define.InputType.None },
                            inputIndexes = new int[] { (int)Define.RSPType.None },
                            barIndex = parameter.bar,
                            tempo = parameter.tempo,
                            position = parameter.position
                        };

                        if (timelineInfo.currentBar < parameter.bar)
                        {
                            timelineInfo.currentPos = parameter.position;
                            timelineInfo.currentBar = parameter.bar;
                            _stageInfo.noteDatas.Add(info);
                            _stageInfo.barCnt++;
                        }
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        NoteInfo info = new NoteInfo()
                        {
                            noteType = Define.NoteType.N_,
                            rspTypes = new Define.RSPType[InGameKey.maxNoteDuplicateCnt],
                            logicTypes = new Define.LogicType[InGameKey.maxNoteDuplicateCnt],
                            inputTypes = new Define.InputType[InGameKey.maxNoteDuplicateCnt],
                            inputIndexes = new int[InGameKey.maxNoteDuplicateCnt],
                            position = parameter.position,
                            tutorialType = Define.InGameTutorialType.None
                        };

                        //Situation Note
                        if (((string)parameter.name).Contains(Define.NoteType.S_.ToString()))
                        {
                            //RuleType StLogic 밖에 처리 안함
                            info.logicTypes[0] = JsonNoteLoader.SetLogicTypeS((string)parameter.name);
                            curRuleIndex = (int)info.logicTypes[0];
                            preSituationChildIndex = (int)info.logicTypes[0];

                            if (((string)parameter.name).Contains(Define.NoteType.N_.ToString()))
                            {
                                //노트에 기믹이 같이 붙은 경우
                                temptRuleIndex = (int)JsonNoteLoader.SetNoteTypeS((string)parameter.name);

                                info = SetNoteInfo(temptRuleIndex, parameter);
                                if(info.noteType != Define.NoteType.NotAvailable)
                                {
                                    _prevLockCnt = _curLockCnt;
                                    ShowNoteUI(info, noteParent);
                                }
                            }
                            else
                            {
                                info.noteType = Define.NoteType.S_;
                                curRuleIndex = (int)JsonNoteLoader.SetNoteTypeS((string)parameter.name);
                                _stageInfo.situationCnt++;
                            }

                        }
                        //End Note
                        else if (((string)parameter.name).Contains(Define.NoteType.End.ToString()))
                        {
                            info.noteType = Define.NoteType.End;
                            EndGenerating();
                        }
                        //Tutorial Note
                        else if (((string)parameter.name).Contains(Define.NoteType.Stop.ToString()))
                        {
                            info.noteType = Define.NoteType.Stop;
                            info.tutorialType = JsonNoteLoader.SetGameTutorialType((string)parameter.name);
                            Debug.Log("튜토리얼 모드: " + info.tutorialType);
                        }
                        else if (((string)parameter.name).Contains(Define.NoteType.MG_.ToString()))
                        {
                            info.noteType = Define.NoteType.MG_;
                            Debug.Log("미니게임 모드: " + (string)parameter.name);
                        }
                        else if(((string)parameter.name).Contains(Define.NoteType.N_.ToString()))
                        {
                            //일반 노트일 때
                            //더블 비겨몬, 트리플 때 리펙토링 예정(제너럴하게 처리)
                            info = SetNoteInfo(curRuleIndex, parameter);
                            if (info.noteType != Define.NoteType.NotAvailable)
                            {
                                _prevLockCnt = _curLockCnt;
                                ShowNoteUI(info, noteParent);
                            }
                        }

                        timelineInfo.lastMarker = parameter.name;
                        timelineInfo.currentPos = parameter.position;

                        if(info.noteType == Define.NoteType.NotAvailable)
                        {
                            Debug.Log($"노트가 중복되어 등록 불가능: {info.noteGimmickType}, 현재 남은 박자: {_prevLockCnt}");
                            return FMOD.RESULT.OK;
                        }

                        _stageInfo.noteDatas.Add(info);
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                    {
                        // Now the event has been destroyed, unpin the timeline memory so it can be garbage collected
                        timelineHandle.Free();
                        break;
                    }
                default:
                    break;
            }
        }

        return FMOD.RESULT.OK;
    }

    /// <summary>
    /// 일반 노트일 때 어떤 노트인지 데이터 parsing해서 저장
    /// </summary>
    /// <param name="stageInfo"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    private static NoteInfo SetNoteInfo(int ruleIndex, FMOD.Studio.TIMELINE_MARKER_PROPERTIES parameter)
    {
        NoteInfo info = new NoteInfo()
        {
            noteType = Define.NoteType.N_,
            noteActionType = JsonNoteLoader.SetNoteTypeA((string)parameter.name),
            noteGimmickType = JsonNoteLoader.SetNoteTypeN((string)parameter.name),
            noteDuplicateCnt = 1,
            rspTypes = new Define.RSPType[InGameKey.maxNoteDuplicateCnt],
            logicTypes = new Define.LogicType[InGameKey.maxNoteDuplicateCnt],
            inputTypes = new Define.InputType[InGameKey.maxNoteDuplicateCnt],
            inputIndexes = new int[InGameKey.maxNoteDuplicateCnt],
            position = parameter.position,
            tutorialType = Define.InGameTutorialType.None,
            gimmickParameters = new int[InGameKey.maxNoteGimmickParameterCnt]
        };

        if(_prevLockCnt > 0)
        {
            info.noteType = Define.NoteType.NotAvailable;
        }

        _curLockCnt = 1;
        string duplicateType = string.Empty;
        string noteName = JsonNoteLoader.RemoveBeforeSubstring((string)parameter.name, info.noteGimmickType.ToString());
        //1) 기믹 확인
        switch (info.noteGimmickType)
        {
            case Define.NoteTypeN.N_HIT:
                info.gimmickParameters = JsonNoteLoader.SetGimmickHitParameters(noteName);
                _stageInfo.noteCnt += info.gimmickParameters[1];
                _curLockCnt = info.gimmickParameters[0];
                Debug.Log($"노트 생성: {info.noteGimmickType} - {info.gimmickParameters[0]}박자 동안 {info.gimmickParameters[1]}번 때리기: {info.noteActionType}");
                return info;
            case Define.NoteTypeN.N_SCT:
            case Define.NoteTypeN.N_SCF:
                _stageInfo.noteCnt++;
                info.noteActionType = Define.NoteTypeA.A_CLK; //무조건 클릭
                _curLockCnt++;
                break;
            case Define.NoteTypeN.N_SCH:
                _stageInfo.noteCnt++;
                info.noteActionType = Define.NoteTypeA.A_CLK; //무조건 클릭
                _curLockCnt += 4;
                break;
            case Define.NoteTypeN.N_BRD:
                noteName = JsonNoteLoader.RemovePattern(noteName, info.noteGimmickType.ToString());
                if(int.TryParse(noteName[0].ToString(), out _))
                {
                    info.gimmickParameters[0] = Int32.Parse(noteName[0].ToString());
                    noteName = noteName.Substring(1);
                }
                else
                {
                    info.gimmickParameters[0] = 2; //Default로 2 지정
                }
                info.noteDuplicateCnt = 1;
                info.inputTypes[0] = Define.InputType.RSP;
                info.rspTypes[0] = JsonNoteLoader.SetRSPType(noteName);
                info.inputIndexes[0] = (int)info.rspTypes[0];
                _curLockCnt = info.gimmickParameters[0];
                Debug.Log($"노트 생성: {info.noteGimmickType} - {info.gimmickParameters[0]}박자 동안 {InGameKey.noteBRDHitCnt}번 때리기: {info.rspTypes[0]}");
                _stageInfo.noteCnt++;
                return info;
            case Define.NoteTypeN.N_SHY:
                if(info.noteActionType == Define.NoteTypeA.A_HLD)
                {
                    info.noteActionType = Define.NoteTypeA.A_CLK;
                }
                break;
            case Define.NoteTypeN.N_PAL:
                duplicateType = string.Empty;
                info.noteDuplicateCnt = 1;
                info.inputTypes[0] = Define.InputType.RSP;
                info.inputTypes[1] = Define.InputType.RSP;
                info.rspTypes[0] = JsonNoteLoader.SetRSPType(noteName);
                duplicateType = JsonNoteLoader.RemovePattern(noteName, info.rspTypes[0].ToString());
                info.rspTypes[1] = JsonNoteLoader.SetRSPType(duplicateType);
                info.inputIndexes[0] = 3 - (int)info.rspTypes[0] - (int)info.rspTypes[1];
                _stageInfo.noteCnt++;
                Debug.Log($"노트 생성: {info.noteGimmickType} - 단짝친구 {(Define.RSPType)info.inputIndexes[0]} 누르기");
                return info;
            case Define.NoteTypeN.N_PCK:
                //공작새 => 4개 연속 같은 것 내기 (rspType[0] [1], logicType[0] [1] 에 순서대로 4개 저장)
                noteName = JsonNoteLoader.RemovePattern(noteName, info.noteGimmickType.ToString());
                info.noteDuplicateCnt = 1;
                if (int.TryParse(noteName[0].ToString(), out _))
                {
                    info.gimmickParameters[0] = Int32.Parse(noteName[0].ToString());
                    noteName = noteName.Substring(1);
                }
                else
                {
                    info.gimmickParameters[0] = 2; //Default로 2 지정
                }
                info.inputTypes[0] = Define.InputType.RSP;
                info.inputTypes[1] = Define.InputType.RSP;
                noteName = noteName.Substring(1);

                char[] delimiterChars = { '_' };
                string[] result = noteName.Split(delimiterChars);

                if(result.Length != 4)
                {
                    info.rspTypes[0] = JsonNoteLoader.SetRSPType(noteName);
                    info.rspTypes[1] = JsonNoteLoader.SetRSPType(noteName);
                    info.logicTypes[0] = (Define.LogicType)JsonNoteLoader.SetRSPType(noteName);
                    info.logicTypes[1] = (Define.LogicType)JsonNoteLoader.SetRSPType(noteName);
                }
                else
                {
                    info.rspTypes[0] = JsonNoteLoader.SetRSPType(result[0]);
                    info.rspTypes[1] = JsonNoteLoader.SetRSPType(result[1]);
                    info.logicTypes[0] = (Define.LogicType)JsonNoteLoader.SetRSPType(result[2]);
                    info.logicTypes[1] = (Define.LogicType)JsonNoteLoader.SetRSPType(result[3]);
                }

                //안 씀
                info.inputIndexes[0] = (int)JsonNoteLoader.SetRSPType(noteName);
                info.inputIndexes[1] = (int)JsonNoteLoader.SetRSPType(noteName);
                _stageInfo.noteCnt += InGameKey.notePCKHitCnt;
                _curLockCnt = info.gimmickParameters[0];
                Debug.Log($"노트 생성: {info.noteGimmickType} - {info.gimmickParameters[0]}박자 동안 때리기: {info.rspTypes[0]} -> {info.rspTypes[1]} -> {(Define.RSPType)info.logicTypes[0]} -> {(Define.RSPType)info.logicTypes[1]}");
                return info;
        }

        info.logicTypes[0] = (Define.LogicType)ruleIndex;
        noteName = JsonNoteLoader.RemovePattern(noteName, info.noteGimmickType.ToString());

        //2) 중첩 확인
        if (noteName[0].Equals('2')) 
        {
            duplicateType = string.Empty;
            info.noteDuplicateCnt = 2;
            info.inputTypes[0] = Define.InputType.RSP;
            info.inputTypes[1] = Define.InputType.RSP;
            info.rspTypes[0] = JsonNoteLoader.SetRSPType(noteName);
            duplicateType = JsonNoteLoader.RemovePattern(noteName, info.rspTypes[0].ToString());
            info.rspTypes[1] = JsonNoteLoader.SetRSPType(duplicateType);
            info.inputIndexes[0] = JsonNoteLoader.SetCorrectIndex(ruleIndex, (int)info.rspTypes[0]);
            info.inputIndexes[1] = JsonNoteLoader.SetCorrectIndex(ruleIndex, (int)info.rspTypes[1]);
            _stageInfo.noteCnt++;
            _curLockCnt++;
        }
        else
        {
            info.noteDuplicateCnt = 1;
            info.inputTypes[0] = Define.InputType.RSP;
            info.rspTypes[0] = JsonNoteLoader.SetRSPType(noteName);
            info.inputIndexes[0] = JsonNoteLoader.SetCorrectIndex(ruleIndex, (int)info.rspTypes[0]);
        }

        //3) 동작 확인
        if(info.noteActionType == Define.NoteTypeA.A_HLD)
        {
            noteName = JsonNoteLoader.RemoveBeforeSubstring(noteName, info.noteActionType.ToString());
            noteName = JsonNoteLoader.RemovePattern(noteName, info.noteActionType.ToString());
            if(noteName.Contains(" "))
            {
                int whitespaceIndex = noteName.IndexOf(' ');
                noteName = noteName.Substring(0, whitespaceIndex);
            }

            if (noteName.Contains("_"))
            {
                int underBarIndex = noteName.IndexOf('_');
                noteName = noteName.Substring(0, underBarIndex);
            }
            info.gimmickParameters[0] = (string.IsNullOrEmpty(noteName) || string.IsNullOrWhiteSpace(noteName))? InGameKey.defaultHoldingTime: int.Parse(noteName);
            _stageInfo.noteCnt++; //홀드노트는 판정이 2개 이기 때문에(시작점, 유지기간), noteCnt가 2개임
            _curLockCnt = info.gimmickParameters[0];
        }

        Debug.Log($"노트 생성: {info.noteGimmickType} - Action: {info.noteActionType}");
        _stageInfo.noteCnt++;
        return info;
    }
    #endregion

    #region UI Action
    private static void ShowNoteUI(NoteInfo noteInfo, GameObject parent)
    {
        if(_note != null)
        {
            BaseObjectPool.Instance.ReturnObject(_note);
            _note = null;
        }
        _note = BaseObjectPool.Instance.Spawn(TownDataLoader.GetNoteKey(noteInfo.noteGimmickType), parent);
        _note.GetComponent<BaseNoteUnit>().SetUnit(noteInfo);
        _note.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200f);
        _note.GetComponent<RectTransform>().localRotation = parent.GetComponent<RectTransform>().localRotation;
        _note.GetComponent<RectTransform>().sizeDelta = parent.GetComponent<RectTransform>().sizeDelta;
    }
    #endregion
}
