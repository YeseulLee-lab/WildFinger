using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Text;
using System.IO;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.Video;
using DG.Tweening;

public class JsonNoteLoader : StreamingAssetPather
{
    public static GameRuleUIManager gameRuleUIManager { get; set; } = null;

    [Header("------------------- Others -------------------")]
    private InGameNoteRoadManager _noteRoadManager;

    [Header("------------------- Game Data -------------------")]
    private string _stageJson = null;
    public NoteInfo[] noteData { get; set; }
    public Queue<NoteInfo> noteQueue { get; set; }
    public StageInfo stageInfo { get; private set; } = null;

    [Header("------------------- Note UI -------------------")]
    public static float bpm = 0; //bpm
    public bool isDone { get; private set; } = false;

    #region Unity Life Cycle
    private void Awake()
    {
        gameRuleUIManager = this.GetComponent<GameRuleUIManager>();
        _noteRoadManager = this.GetComponent<InGameNoteRoadManager>();
    }

    private void OnDestroy()
    {
        gameRuleUIManager = null;
        _stageJson = null;
        noteData = null;
        noteQueue = null;
        stageInfo = null;
    }
    #endregion

    #region Note Actions
    private void LoadNotes(NoteInfo[] infos, UnityAction complete)
    {
        if (infos.Length < 1)
        {
            Debug.Log($"[Json] Null Note Data");
            return;
        }
        noteQueue = new Queue<NoteInfo>(stageInfo.noteCnt);

        for (int i = 0; i < infos.Length; i++)
        {
            RegisterNoteQueue(infos[i]);
        }

        bpm = infos[0].tempo;
        //DebugX.Log($"[Json] Complete - Note Data Setting");
        _noteRoadManager.noteRoad.SetLine(TownDataLoader.curMusicInfo.lineCnt);
        isDone = true;
        complete?.Invoke();
    }

    private void RegisterNoteQueue(NoteInfo info)
    {
        switch (info.noteType)
        {
            case Define.NoteType.N_:
                {
                    noteQueue.Enqueue(info);
                    //DebugX.Log("NoteQueue Enqueue: " + info.position);
                }
                break;
            case Define.NoteType.End:
            case Define.NoteType.Stop:
            case Define.NoteType.S_:
            case Define.NoteType.L_:
                break;
        }
    }

    public void ReleaseNote()
    {
        noteQueue.Clear();
        noteQueue = null;
    }

    #endregion

    #region Load Marker Json File
    public void LoadNoteData(UnityAction complete = null)
    {
        string filePath;
        DebugX.Log($"{Application.platform} 에서 Json 파일 로드.");
        stageInfo = new StageInfo();
        
        switch (Application.platform)
        {
            default:
            case RuntimePlatform.WindowsEditor:
                filePath = GetFilePathCommon(TownDataLoader.town, TownDataLoader.level);
                if (File.Exists(filePath))
                {
                    _stageJson = File.ReadAllText(filePath);
                    stageInfo = JsonUtility.FromJson<StageInfo>(_stageJson);
                    noteData = stageInfo.noteDatas.ToArray();
                    LoadNotes(noteData, complete);
                }
                else
                {
                    DebugX.LogError("File not found: " + filePath);
                }
                break;
            case RuntimePlatform.Android:
                {
                    filePath = GetFilePathCommon(TownDataLoader.town, TownDataLoader.level);
                    WWW reader = new WWW(filePath);
                    while (!reader.isDone) {}
                    _stageJson = reader.text;
                    stageInfo = JsonUtility.FromJson<StageInfo>(_stageJson);
                    noteData = stageInfo.noteDatas.ToArray();
                    LoadNotes(noteData, complete);
                    break;
                }
            case RuntimePlatform.IPhonePlayer:
                {
                    filePath = GetFilePathIOS(TownDataLoader.town, TownDataLoader.level);
                    DebugX.Log($"filePath: {filePath}");
                    WWW reader = new WWW(filePath);
                    while (!reader.isDone){}
                    _stageJson = reader.text;
                    stageInfo = JsonUtility.FromJson<StageInfo>(_stageJson);
                    noteData = stageInfo.noteDatas.ToArray();
                    LoadNotes(noteData, complete);
                    break;
                }
        }
    }
    #endregion

    #region Util
    public static Define.LogicType SetLogicType(string type)
    {
        if (type.Contains(Define.LogicType.Win.ToString()))
        {
            return Define.LogicType.Win;
        }
        else if (type.Contains(Define.LogicType.Draw.ToString()))
        {
            return Define.LogicType.Draw;
        }
        else if (type.Contains(Define.LogicType.Lose.ToString()))
        {
            return Define.LogicType.Lose;
        }
        else
        {
            return Define.LogicType.Win;
        }
    }

    public static Define.LogicType SetLogicTypeS(string type)
    {
        if (type.Contains(Define.NoteTypeS.S_W.ToString()))
        {
            return Define.LogicType.Win;
        }
        else if (type.Contains(Define.NoteTypeS.S_D.ToString()))
        {
            return Define.LogicType.Draw;
        }
        else if (type.Contains(Define.NoteTypeS.S_L.ToString()))
        {
            return Define.LogicType.Lose;
        }
        else
        {
            return Define.LogicType.Win;
        }
    }

    public static Define.RSPType SetRSPType(string type)
    {
        if (type.Contains(Define.RSPType.Rock.ToString()))
        {
            return Define.RSPType.Rock;
        }
        else if (type.Contains(Define.RSPType.Scissor.ToString()))
        {
            return Define.RSPType.Scissor;
        }
        else if (type.Contains(Define.RSPType.Paper.ToString()))
        {
            return Define.RSPType.Paper;
        }
        else
        {
            return Define.RSPType.Rock;
        }
    }

    public static Define.RSPType SetRSPTypeS(string type)
    {
        if (type.Contains(Define.NoteTypeS.S_R.ToString()))
        {
            return Define.RSPType.Rock;
        }
        else if (type.Contains(Define.NoteTypeS.S_R.ToString()))
        {
            return Define.RSPType.Scissor;
        }
        else if (type.Contains(Define.NoteTypeS.S_P.ToString()))
        {
            return Define.RSPType.Paper;
        }
        else
        {
            return Define.RSPType.Rock;
        }
    }

    public static Define.InGameTutorialType SetGameTutorialType(string type)
    {
        string onlyTutorial = RemovePattern(type, Define.NoteType.Stop.ToString() + "_");
        for (int i = Enum.GetValues(typeof(Define.InGameTutorialType)).Length - 2; i >= 0; i--)
        {
            if (onlyTutorial.Equals(((Define.InGameTutorialType)i).ToString()))
            {
                return (Define.InGameTutorialType)i;
            }
        }

        return Define.InGameTutorialType.None;
    }

    public static Define.NoteTypeS SetNoteTypeS(string type)
    {
        if (type.Contains(Define.NoteTypeS.S_W.ToString()))
        {
            return Define.NoteTypeS.S_W;
        }
        else if (type.Contains(Define.NoteTypeS.S_D.ToString()))
        {
            return Define.NoteTypeS.S_D;
        }
        else if (type.Contains(Define.NoteTypeS.S_L.ToString()))
        {
            return Define.NoteTypeS.S_L;
        }
        /*
        else if (type.Contains(Define.NoteTypeS.S_R.ToString()))
        {
            return Define.NoteTypeS.S_R;
        }
        else if (type.Contains(Define.NoteTypeS.S_S.ToString()))
        {
            return Define.NoteTypeS.S_S;
        }
        else if (type.Contains(Define.NoteTypeS.S_P.ToString()))
        {
            return Define.NoteTypeS.S_P;
        }
        */
        else
        {
            //Default
            return Define.NoteTypeS.S_W;
        }
    }

    public static Define.NoteTypeN SetNoteTypeN(string type)
    {
        for(int i =1; i< Enum.GetNames(typeof(Define.NoteTypeN)).Length; i++)
        {
            if (type.Contains(((Define.NoteTypeN)i).ToString()))
            {
                return (Define.NoteTypeN)i;
            }
        }
        return Define.NoteTypeN.N_;
    }

    public static Define.NoteTypeA SetNoteTypeA(string type)
    {
        if (type.Contains(Define.NoteTypeA.A_FLK.ToString()))
        {
            return Define.NoteTypeA.A_FLK;
        }
        else if (type.Contains(Define.NoteTypeA.A_HLD.ToString()))
        {
            return Define.NoteTypeA.A_HLD;
        }
        else
        {
            //Default
            return Define.NoteTypeA.A_CLK;
        }
    }

    public static int SetCorrectIndexStRSPRvs(Define.RSPType situationType, Define.RSPType noteType)
    {
        return ((((int)situationType * 2) + (int)noteType) + 1) % 3;
    }

    public static int SetCorrectIndex(int situationType, int noteType)
    {
        return (situationType + noteType + 2) % 3;
    }

    public static int SetCorrectIndex(Define.RSPType situationType, Define.RSPType noteType)
    {
        return ((int)situationType + (int)noteType + 1) % 3;
    }

    /// <summary>
    /// 패턴을 찾았을 때는 해당 부분을 제거하고 새로운 문자열을 반환, 패턴을 찾지 못했을 때는 원래 문자열을 반환.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static string RemovePattern(string original, string pattern)
    {
        int index = original.IndexOf(pattern);
        if (index != -1)
        {
            return original.Remove(index, pattern.Length);
        }
        else
        {
            // 패턴을 찾지 못했을 때, 원래 문자열을 반환
            return original;
        }
    }

    /// <summary>
    /// 만약 substring이 original 문자열에 있으면  substring 이전의 문자열을 제거하고 반환, 없으면 원래 문자열을 반환.
    /// </summary>
    /// <param name="original">기존 문자열</param>
    /// <param name="substring">지울 문자열을 나누는 패턴</param>
    /// <returns></returns>
    public static string RemoveBeforeSubstring(string original, string substring)
    {
        int index = original.IndexOf(substring);
        if (index == -1)
        {
            return original;
        }
        else
        {
            // substring 이전의 문자열을 제거하고 반환합니다.
            return original.Substring(index);
        }
    }

    /// <summary>
    /// 연타 기믹의 경우 필요한 파라미터 리턴, 버튼 상관 없이 n(배열[0])박자 동안 k(배열[1])번 연타
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static int[] SetGimmickHitParameters(string input)
    {
        string onlyParameter = RemovePattern(input, Define.NoteTypeN.N_HIT.ToString());

        if (onlyParameter.Contains("_"))
        {
            int underBarIndex = onlyParameter.IndexOf('_');
            if(onlyParameter[underBarIndex + 1].Equals(" "))
            {
                onlyParameter = onlyParameter.Remove(underBarIndex + 1, 1);
            }

            if (onlyParameter[underBarIndex-1].Equals(" "))
            {
                onlyParameter = onlyParameter.Remove(underBarIndex - 1, 1);
            }
        }

        if(onlyParameter.Contains(" "))
        {
            int whitespaceIndex = onlyParameter.IndexOf(' ');
            onlyParameter = onlyParameter.Substring(0, whitespaceIndex);
        }

        string[] parts = onlyParameter.Split('_', StringSplitOptions.RemoveEmptyEntries);
        int[] numbers = new int[2];

        for (int i = 0; i < 2; i++)
        {
            numbers[i] = int.Parse(parts[i]); // 숫자로 변환하여 배열에 저장
        }
        return numbers;
    }
    #endregion
}
