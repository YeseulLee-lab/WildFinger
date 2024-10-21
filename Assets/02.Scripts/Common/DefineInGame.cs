using System.Collections.Generic;
using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Video;
using System.Diagnostics;
using Sirenix.OdinInspector;

public class DebugX
{
    [Conditional("DEBUGMODE")]
    public static void Log(string msg)
    {
#if DEBUGMODE
        UnityEngine.Debug.Log(msg);
#endif
    }

    [Conditional("DEBUGMODE")]
    public static void Log(int msg)
    {
#if DEBUGMODE
        UnityEngine.Debug.Log(msg);
#endif
    }

    [Conditional("DEBUGMODE")]
    public static void LogWarning(string msg)
    {
#if DEBUGMODE
        UnityEngine.Debug.LogWarning(msg);
#endif
    }

    [Conditional("DEBUGMODE")]
    public static void LogWarning(int msg)
    {
#if DEBUGMODE
        UnityEngine.Debug.LogWarning(msg);
#endif
    }

    [Conditional("DEBUGMODE")]
    public static void LogError(string msg)
    {
#if DEBUGMODE
        UnityEngine.Debug.LogError(msg);
#endif
    }

    [Conditional("DEBUGMODE")]
    public static void LogError(int msg)
    {
#if DEBUGMODE
        UnityEngine.Debug.LogError(msg);
#endif
    }
}

public partial class Define
{
    #region InGame Note
    public enum NoteType
    {
        None = -1,
        N_, //Note
        Bar,
        End,
        S_, //Situation
        L_, //Line
        Stop,
        MG_, //MiniGame
        A_,
        NotAvailable, //노트 등록 금지
    }

    public enum NoteTypeS
    {
        None = -1,
        S_W,
        S_D,
        S_L,
        S_R,
        S_S,
        S_P,
    }

    public enum NoteTypeN
    {
        N_,
        N_HIT, //연타폭탄
        N_SHY, //샤이(조개)
        N_SCT, //시크릿(박스)
        N_HTG, //인질(엄지척)
        N_JMP, //점프
        N_DSG, //변장술사(유령천)
        N_SCF, //시크릿플릭 1회
        N_BRD, //빵반죽 4번
        N_SCH, //시크릿홀드 4박자
        N_ILU, //알러뷰
        N_PAL, //단짝
        N_PCK, //공작새
    }

    public enum NoteTypeA
    {
        A_CLK, //디폴트
        A_HLD,
        A_FLK,
    }
#endregion

    #region InGame General
    public enum RSPType
    {
        None = -1,
        Rock,
        Scissor,
        Paper,
    }

    public enum LogicType
    {
        None = -1,
        Win,
        Draw,
        Lose,
    }

    public enum TownList
    {
        None = -1, //Training 상황
        ToyTown,
        Viking,
        Space,
        NeonEDM,
        ArabianDesert,
        TropicalBeach,
        JazzCafe,
        Halloween,
        TBC, //TO Be Continue
    }

    public enum NoteJudge
    {
        None = -1,
        Perfect,
        Good,
        Miss,
        Protected,
    }

    /// <summary>
    /// 사라질 예쩡
    /// </summary>
    public enum NoteJudgeTiming
    {
        None = -1,
        Perfect,
        Good,
        Miss,
    }

    /// <summary>
    /// 사라질 예쩡22
    /// </summary>
    public enum NoteJudgeLogic
    {
        None = -1,
        O,
        X, //Incorrect Logic
    }

    public enum InputType
    {
        None = -1,
        RSP,
        Logic, //Win Draw Lose
    }

    /// <summary>
    /// 레벨 시스템으로 분화 되면서 새로 추가됨. 현재는 StLogic 만 하는 것으로 처리함. 이후 추가해야함. 자세한 분류 내용은 마일스톤 기획서 참조 => 
    /// https://docs.google.com/spreadsheets/d/1SCkcocX7I35DRjqjrAPxAGCEnAFSbZoUBlR5MYqqmzA/edit#gid=123696234
    /// </summary>
    public enum RuleType
    {
        None = -1,
        StLogic,
        StRSP,
        StRSPRvs,
        LnRSPRvs,
        LnRSP,
        LnLogic,
    }

    public enum HPType
    {
        Full = 100,
        Enough = 70,
        Low = 40,
        Zero = 0,
    }

    public enum RetryCoin
    {
        None = 0,
        First = 900,
        Second = 1900,
        Third = 3900,
        Fourth = 6900,
        Others = 10900,
    }

    public enum InGameState
    {
       None = -1,
       Waiting,
       Playing,
       Paused,
       Resumed,
       End,
       PausedCntDown,
       FeverTime,
    }

    public enum InGameType
    {
        Default,
        MiniGame,
    }

    public enum InGameShakeScale
    {
        Small,
        Midium,
        Large,
    }

    public enum NoteLogicAppearLevel
    {
        Win = 1,
        Draw = 9,
        Lose = 201,
    }

    public enum FeverType
    {
        None = -1,
        Random,
        Pinata, //연타
        SuperShooting, //원거리 슈팅
        Clicker, //방치형
    }
    #endregion

    #region Monster
    public enum MonsterAnimType
    {
        Idle,
        Spawn,
        CastSpell,
        Damaged,
        Dash,
        Attack1,
        Attack2,
        Attack3,
        Attack4,
        Attack5,
        Die,
    }

    public enum MonsterPosType
    {
        Default,
        Front,
        VeryFront,
        FeverPinata,
    }
#endregion

    #region MiniGame
    public enum MiniGameState
    {
        None = -1,
        Waiting,
        Playing,
        Paused,
        Resumed,
        End,
        PausedCntDown,
    }
    #endregion

    #region Common
    public enum MusicGenre
    {
        None = -1,

    }

    public enum SceneName
    {
        None = -1,
        Init,
        Login,
        Loading,
        MusicList,
        Main,
        Game,
        MGMemorization,
        Training,
    }

    public enum SceneType
    {
        None = -1,
        Main,
        InGame,
    }

    public enum GameItem
    {
        Shield,
        PerfectDrink,
        EraseLogic,
        RecoveryLifePosion,
    }

    public enum LineUIType
    {
        Default,
        Correct,
        Incorrect,
    }

    public enum InGameTutorialType
    {
        None = -1,
        Rock, //Win도 설명해
        ScissorBtn,
        PaperBtn,
        Flick,
        Hold,
        AllTutorial,
        ItemShield,
        ItemIncreaseHP, //최대체력물약
        ItemHPPotion, //회복수치물약
        DrawMon,
        HitMon,
        DoubleWinMon,
        DoubleWinMon2,
        DoubleDrawMon,
        DoubleDrawMon2,
        ShyMon,
        DoubleShyMon,
        SecretMon,
        HTGMon,
        JMPMon,
        DSGMon,
        DoubleDSGMon,
        LoseMon, //
        SCFMon, //시크릿 플릭
        DoubleLoseMon,
        DoubleLoseMon2,
        BRDMon, //빵반죽
        SCHMon, //시크릿 홀드
        ILUMon, //알럽
        PALMon, //단짝
        PCKMon, //공작새
    }

    public enum LanguageType
    {
        Korean,
        English,
    }

    public enum TutorialUIType
    {
        None,
        Description,
        MaskDescription,
        MaskDescriptionAfterClick,
        Popup,
        MaskPopup,
    }

    public enum DescPosType
    {
        None,
        Top,
        Bottom,
    }

    public enum UsingItemBeforeInGame
    {
        Shield,
        IncreasedHP,
        IncreasedHealingHP,
    }

    public enum TownMaxLevel
    {
        ToyTown = 30,
        Viking = 80,
        Space = 140,
        NeonEDM = 200,
        ArabianDesert = 260,
        TropicalBeach = 320,
        JazzCafe = 380,
        Halloween = 440,
    }
#endregion
}

[Serializable]
public class LocalizationInfo
{
    public string key;
    public Define.TutorialUIType tutorialUIType;
    public Define.DescPosType descPosType;
    public string summary;
}

[Serializable]
public class LocalizationData
{
    public string key;
    public Define.TutorialUIType tutorialUIType { get; set; }
    public LanguageType languageType;
}

[Serializable]
public class LanguageType
{
    public string korean;
    public string english;
    public string spanish; //안씀
}

public class BasicKey
{
    public const float positionScale = 1f; //---임의로(칸 길이가 의미가 없어짐)
    public const string tagNote = "Note";
    public const string miniGame = "MG";
    public const string localizationDataTable = "LocalizationTable";
}

public class InGameKey
{
    public const int defaultIngameLife = 100;
    public const int itemIncreasedHPAmount = 50;
    public const int noteMissPoint = -20;
    public const int noteXPoint = -10;
    public const int noteTenComboPoint = 5;
    public const int resumeCntDown = 3;
    public const int maxNoteDuplicateCnt = 2;
    public const int itemShieldCnt = 3;
    public const int itemIncreasedHealingHPAmount = 10;
    public const int defaultVibrateMS = 10;
    public const int defaultVibrateAmplitude = 220;
    public const int hpMinusVibrateMS = 28;
    public const int hpMinusVibrateAmplitude = 255;
    public const int mgIncorrectPoint = -35;
    public const int maxNoteGimmickParameterCnt = 2;
    public const int defaultHoldingTime = 2;
    public const int mgMemorizationBonusCoinCnt = 900;
    public const int notePCKHitCnt = 4;
    public const int noteSCHBeatCnt = 4;
    public const int noteBRDHitCnt = 4;
    public const int judgeEffectMS = 405;
    public const int shakeSmallEffectMS = 350;
    public const int shakeMidLargeEffectMS = 550;
    public const int maxNoteRoadCnt = 6;
    public const int lastNoteSpaceBeatCnt = 4; // 끝나기 해당 박자 전에는 노트 안나옴
    public const float feverTime = 10f;
}

[Serializable]
public class NoteInfo
{
    public Define.NoteType noteType; // 0: Note, 1: Bar ..
    public Define.NoteTypeN noteGimmickType;
    public Define.NoteTypeA noteActionType;
    public Define.RSPType[] rspTypes;
    public Define.LogicType[] logicTypes;
    public Define.InputType[] inputTypes;
    public Define.InGameTutorialType tutorialType; //있으면?
    public int[] inputIndexes; //정답(RPS, Logic 일 때 int 값으로 저장)
    public int noteDuplicateCnt; //기본 1개
    public int barIndex;
    public float tempo;
    public int position;
    public int[] gimmickParameters;
    public int gimmickKey = -1; //자동 할당됨, 자동 생성기에서 사용함
}

[Serializable]
public class StageInfo
{
    //public Define.RuleType ruleType; //현재 StLogic 만 처리함
    //public int lineCnt; // TownDataLoader.curMusicInfo.lineCnt 에서 확인
    public int barCnt;
    public int noteCnt; //정확하지 않음(대략적으로 알기 위해)
    public int situationCnt;
    public int beatCnt; //임시로 n/4 박자에서 n박자 수, 비트가 바뀌는 경우는 불가능함
    public int totalBeatMarkerCnt;
    public float distancePerMarker;
    public float bpm;
    public bool isAutoGenerated = false;
    public List<NoteInfo> noteDatas = new List<NoteInfo>();
}

[Serializable]
public class MusicInfo
{
    public Define.InGameType gameType;
    //public Define.RuleType ruleType; //현재 StLogic 만 처리함
    public Define.InputType inputStyle;
    public InitInputActive initInputActive;
    public Define.InputType noteStyle;
    public int lineCnt = 3; // 최소 1줄, 미니게임에서는 외우는 숫자
    public int stage; // 1번부터 시작 => 타운이 달라져도 누적됨
    public EventReference music;
    //public Sprite[] backgroundImgs;
}

[Serializable]
public class InitInputActive
{
    public bool[] inputs = new bool[3] {true, true, true };
}

[Serializable]
public class InGameItemInfo
{
    //Anything else
    public Define.GameItem type;
    public string name;
    public int price; //Coin
    public string description;
}

[Serializable]
public class InGameTutorialPopupInfo
{
    public Define.InGameTutorialType type;
    public Define.TownList town;
    public int level;
    public Sprite[] imgs = new Sprite[1];
    public Sprite trainingImg;
    public string trainingDesc;
}

public partial class EncryptedKey
{
    public const string isSuccessfulOnFirstTryCnt = "SFT"; //한번에 성공한 수 Count
    public const string isAllPerfect = "AP"; //해당 레벨 올퍼펙트 여부(T/F)
    public const string score = "SC"; //해당 레벨 클리어 점수(음표 수 1~3개)
    public const string maxTown = "MT"; //최대로 진행한 마을 Index
    public const string maxLevel = "ML"; //최대 달성 레벨(플레이 해야하는 레벨) Index
    public const string recentDataUploadTime = "RUT"; //유저 데이터를 업로드한 최근 시간(yyyyMMddHHmmss)
    public const string recentDataDownloadTime = "RDT"; //유저 데이터를 다운로드한 최근 시간(yyyyMMddHHmmss)
    public const string firebaseStorageAddress = "gs://projectrsp-2ad9c.appspot.com";
}

public partial class UnencryptedKey
{
    public const string isFirst = "IF"; //설치 후 첫 접속 여부(T/F)
    public const string isVibOn = "CVO"; //전체 진동 OnOff 여부(T/F)
    public const string isSFXOn = "CSO"; //전체 SFX OnOff 여부(T/F)
    public const string isBGMOn = "CBO"; //전체 BGM OnOff 여부(T/F)
    public const string isNotiOn = "NTO"; //푸쉬 알람 OnOff 여부(T/F)
}

[Serializable]
public class NoteGeneratorUnitInfo
{
    [ReadOnly]
    public int gimmickKey; //자동 할당
    public Define.NoteTypeN gimmickType;
    public int appearLevel = 1;
    [ShowIf(nameof(IsActionTypeAvailable))]
    [OnValueChanged(nameof(UpdateTotalBeatCnt))]
    public Define.NoteTypeA actionType = Define.NoteTypeA.A_CLK;
    public Sprite gimmickImg;
    public string gimmickName;

    [ShowIf(nameof(IsActionTypeNotHold))]
    [OnValueChanged(nameof(UpdateTotalBeatCnt))]
    public bool isDouble = false;

    [ShowIf(nameof(isDouble))]
    [OnValueChanged(nameof(UpdateTotalBeatCnt))]
    public bool isSameDouble = true;

    [ShowIf(nameof(IsBeatCntAvailable))]
    [OnValueChanged(nameof(UpdateTotalBeatCnt))]
    public int beatCnt = 2;

    [ReadOnly]
    public int totalBeatCnt = 1;

    #region Setting
    public void UpdateTotalBeatCnt()
    {
        int total = 1;

        //TODO: 계산
        switch (gimmickType)
        {
            case Define.NoteTypeN.N_:
            case Define.NoteTypeN.N_JMP:
            case Define.NoteTypeN.N_DSG:
                if (isDouble)
                {
                    total++;
                }

                if(actionType == Define.NoteTypeA.A_HLD)
                {
                    total = beatCnt;
                }
                break;
            case Define.NoteTypeN.N_HIT:
            case Define.NoteTypeN.N_PCK:
            case Define.NoteTypeN.N_BRD:
                total = beatCnt;
                break;
            case Define.NoteTypeN.N_SHY:
            case Define.NoteTypeN.N_SCT:
            case Define.NoteTypeN.N_SCF:
                if (isDouble)
                {
                    total++;
                }
                break;
            case Define.NoteTypeN.N_HTG:
            case Define.NoteTypeN.N_ILU:
            case Define.NoteTypeN.N_PAL:
                break;
            case Define.NoteTypeN.N_SCH:
                total += 4;
                if (isDouble)
                {
                    total++;
                }
                break;
        }

        totalBeatCnt = total;
    }

    private bool IsActionTypeNotHold()
    {
        if(actionType == Define.NoteTypeA.A_HLD)
        {
            return false;
        }

        switch (gimmickType)
        {
            case Define.NoteTypeN.N_:
            case Define.NoteTypeN.N_SHY:
            case Define.NoteTypeN.N_SCT:
            case Define.NoteTypeN.N_JMP:
            case Define.NoteTypeN.N_DSG:
            case Define.NoteTypeN.N_SCF:
            case Define.NoteTypeN.N_SCH:
                return true;
            default:
                return false;
        }
    }

    private bool IsActionTypeAvailable()
    {
        switch (gimmickType)
        {
            case Define.NoteTypeN.N_HIT:
            case Define.NoteTypeN.N_SCT:
            case Define.NoteTypeN.N_HTG:
            case Define.NoteTypeN.N_SCF:
            case Define.NoteTypeN.N_BRD:
            case Define.NoteTypeN.N_SCH:
            case Define.NoteTypeN.N_ILU:
            case Define.NoteTypeN.N_PAL:
            case Define.NoteTypeN.N_PCK:
                return false;
            default:
                return true;
        }
    }

    private bool IsBeatCntAvailable()
    {
        if (actionType == Define.NoteTypeA.A_HLD)
        {
            return true;
        }

        switch (gimmickType)
        {
            case Define.NoteTypeN.N_HIT:
            case Define.NoteTypeN.N_BRD:
            case Define.NoteTypeN.N_PCK:
                return true;
            default:
                return false;
        }
    }
    #endregion
}