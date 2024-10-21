using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;

public class TownDataLoader : MonoBehaviour
{
    [Header("------------------ Type Setting -----------------")]
    [SerializeField]
    private bool _isTraining = false;
    public static bool isTraining;
    [SerializeField, ShowIf(nameof(_isTraining))]
    private Define.InGameTutorialType _tutorialType;
    private Define.TownList _town;
    [SerializeField, DisableIf(nameof(_isTraining), true)]
    private int _level = 1;

    [Header("------------------ Common Setting -----------------")]
    public static Define.TownList town;
    public static int level;
    public static Define.InGameTutorialType tutorialType;
    [SerializeField]
    private TownMusicData[] _townMusicDatas;
    public static TownMusicData[] townMusicDatas { get; private set; }
    public static Dictionary<int, MusicInfo> stageDic { get; private set; }
    public static MusicInfo curMusicInfo { get; private set; }
    public static TownMusicData curTownMusicData { get; private set; }
    private GameRuleUIManager _gameRuleUIManager;
    public static bool isDone { get; private set; }
    public static int curTownFirstLevel = 1;

    #region Unity Life Cycle
    private async void Awake()
    {
        isDone = false;

        if (GamePlayData.Instance == null)
        {
            isTraining = _isTraining;
            tutorialType = _tutorialType;
        }
        else
        {
            isTraining = (SceneSwitcher.Instance.curSceneName == Define.SceneName.Training);
            tutorialType = SceneSwitcher.Instance.trainingType;
            _level = GamePlayData.Instance.curStage;
        }

        _town = SceneSwitcher.GetTownList(_level);
        curMusicInfo = new MusicInfo();
        townMusicDatas = _townMusicDatas;
        _gameRuleUIManager = this.GetComponent<GameRuleUIManager>();

        
        if(SceneManager.GetActiveScene().ToString().Equals(Define.SceneName.Game.ToString()) || SceneManager.GetActiveScene().ToString().Equals(Define.SceneName.Training.ToString()))
        {
            await UniTask.WaitUntil(() => BeatGridTracker.Instance != null);
        }

        await SetTownData(_town, _level);

        if (_gameRuleUIManager == null)
        {
            return;
        }

        _gameRuleUIManager.SetInputUI(curMusicInfo.inputStyle, curMusicInfo.initInputActive);
        _gameRuleUIManager.SetCurStageNum(curMusicInfo.stage);
    }

    private void OnDestroy()
    {
        stageDic = null;
        curMusicInfo = null;
        _gameRuleUIManager = null;
        _townMusicDatas = null;
        townMusicDatas = null;
        curTownMusicData = null;
    }
    #endregion

    public static async UniTask SetTownData(Define.TownList loadedTown, int loadedStage)
    {
        if (isTraining)
        {
            if(BeatGridTracker.Instance == null)
            {
                await UniTask.WaitUntil(() => BeatGridTracker.Instance != null);
            }
            loadedTown = BeatGridTracker.Instance.tutorialManager.GetTutorialData(tutorialType).town;
            loadedStage = BeatGridTracker.Instance.tutorialManager.GetTutorialData(tutorialType).level;
            town = loadedTown;
            level = loadedStage;
        }
        else
        {
            town = loadedTown;
            level = loadedStage;
        }
        stageDic = new Dictionary<int, MusicInfo>(townMusicDatas[(int)loadedTown].musicDatas.Length);

        for (int i = 0; i < townMusicDatas[(int)loadedTown].musicDatas.Length; i++)
        {
            stageDic.Add(townMusicDatas[(int)loadedTown].musicDatas[i].stage, townMusicDatas[(int)loadedTown].musicDatas[i]);
        }

        DebugX.Log($"선택된 곡: {town} - Level{level}");
        curTownFirstLevel = townMusicDatas[(int)loadedTown].musicDatas[0].stage;
        curMusicInfo = stageDic[level];
        curTownMusicData = townMusicDatas[(int)loadedTown];
        isDone = true;
    }

    public static bool IsLastStageOfTown()
    {
        return curMusicInfo.stage == curTownMusicData.musicDatas[curTownMusicData.musicDatas.Length - 1].stage;
    }

    public static PoolingKeys GetNoteKey(Define.NoteTypeN gimmickType)
    {
        switch (gimmickType)
        {
            default:
            case Define.NoteTypeN.N_:
                return PoolingKeys.BaseNote;
            case Define.NoteTypeN.N_HIT:
                return PoolingKeys.NoteHIT;
            case Define.NoteTypeN.N_SHY:
                return PoolingKeys.NoteSHY;
            case Define.NoteTypeN.N_SCT:
                return PoolingKeys.NoteSCT;
            case Define.NoteTypeN.N_HTG:
                return PoolingKeys.NoteHTG;
            case Define.NoteTypeN.N_JMP:
                return PoolingKeys.NoteJMP;
            case Define.NoteTypeN.N_DSG:
                return PoolingKeys.NoteDSG;
            case Define.NoteTypeN.N_SCF:
                return PoolingKeys.NoteSCF;
            case Define.NoteTypeN.N_BRD:
                return PoolingKeys.NoteBRD;
            case Define.NoteTypeN.N_SCH:
                return PoolingKeys.NoteSCH;
            case Define.NoteTypeN.N_ILU:
                return PoolingKeys.NoteILU;
            case Define.NoteTypeN.N_PAL:
                return PoolingKeys.NotePAL;
            case Define.NoteTypeN.N_PCK:
                return PoolingKeys.NotePCK;
        }
    }
}
