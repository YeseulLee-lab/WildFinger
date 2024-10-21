using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public static SceneSwitcher Instance { get; private set; } = null;
    private Coroutine _loadingCor { get; set; } = null;
    public Define.SceneName curSceneName { get; set; } = Define.SceneName.None;
    private Define.SceneName _targetSceneName = Define.SceneName.None;
    public Define.SceneName targetSceneName
    {
        get
        {
            return _targetSceneName;
        }
        set
        {
            _targetSceneName = value;
            sceneType = GetSceneType(value);
        }
    }
    public Define.SceneType sceneType { get; private set; } = Define.SceneType.None;
    public Define.InGameTutorialType trainingType { get; private set; }

    #region Unity Life Cycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            curSceneName = Define.SceneName.Init;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        _loadingCor = null;
        Instance = null;
    }

    private void Start()
    {
        SwitchScene(Define.SceneName.Login);

        Invoke(nameof(SetFrame), 1f);
    }
    #endregion

    private void SetFrame()
    {
        Application.targetFrameRate = 60;
    }

    /// <summary>
    /// 게임 씬 제외 일반적인 씬 이동
    /// </summary>
    /// <param name="targetScene">이동할 씬 이름(게임씬 제외)</param>
    /// <param name="delay">forced delay 설정</param>
    public void SwitchScene(Define.SceneName targetScene, float delay = 0f)
    {
        targetSceneName = targetScene;

        if (curSceneName == Define.SceneName.Init)
        {
            //로딩 씬 없이
            _loadingCor = StartCoroutine(LoadSceneWithDelay(targetScene, delay));
            return;
        }

        _loadingCor = StartCoroutine(LoadSceneWithDelay(Define.SceneName.Loading, delay));
    }

    /// <summary>
    /// 게임 씬(미니게임, 일반게임 등)
    /// </summary>
    /// <param name="town"></param>
    /// <param name="stage"></param>
    /// <param name="delay"></param>
    public void SwitchGameScene(Define.TownList town, int stage, float delay = 0f)
    {
        if (curSceneName == Define.SceneName.Main)
        {
            GamePlayData.Instance.admobManager.HideBanner();
        }

        if (stage > GamePlayData.Instance.maxDevelpedStage)
        {
            stage = Random.Range(1, GamePlayData.Instance.maxDevelpedStage + 1);
            town = GetTownList(stage);
        }

        GamePlayData.Instance.curTown = town;
        GamePlayData.Instance.curStage = stage;

        if (IsBonusStage(stage))
        {
            targetSceneName = Define.SceneName.MGMemorization;
        }
        else
        {
            targetSceneName = Define.SceneName.Game;
        }

        DebugX.Log($"게임 실행: {town} - Level{stage}");
        _loadingCor = StartCoroutine(LoadSceneWithDelay(Define.SceneName.Loading, delay));
    }

    /// <summary>
    /// 훈련도감 씬
    /// </summary>
    /// <param name="type">훈련할 튜토리얼 타입</param>
    public void SwitchGameScene(Define.InGameTutorialType type, float delay = 0f)
    {
        if(curSceneName == Define.SceneName.Main)
        {
            GamePlayData.Instance.admobManager.HideBanner();
        }

        GamePlayData.Instance.curTown = Define.TownList.None;
        GamePlayData.Instance.curStage = 0;
        trainingType = type;
        targetSceneName = Define.SceneName.Training;

        _loadingCor = StartCoroutine(LoadSceneWithDelay(Define.SceneName.Loading, delay));
    }

    /// <summary>
    /// 이미 이동할 씬 위치가 설정되어 있는 경우 호출(ex 로딩씬) 
    /// </summary>
    /// <param name="delay"></param>
    public void SwitchScene(float delay = 0f)
    {
        if (targetSceneName == Define.SceneName.None)
        {
            DebugX.Log("이동 씬 설정 안 되어있음");
            return;
        }
        _loadingCor = StartCoroutine(LoadSceneWithDelay(targetSceneName, delay));
    }

    private IEnumerator LoadSceneWithDelay(Define.SceneName targetScene, float delay)
    {
        yield return new WaitForSeconds(delay);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene.ToString());

        // 씬 로드가 완료될 때까지 대기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        curSceneName = targetSceneName;
    }

    public static bool IsBonusStage(int stage)
    {
        if (IsMaxStage(stage))
        {
            return false;
        }
        return (stage % 10) == 0;
    }

    public static bool IsMaxStage(int stage)
    {
        Define.TownMaxLevel[] townMaxLevels = (Define.TownMaxLevel[])System.Enum.GetValues(typeof(Define.TownMaxLevel));
        for (int i = 0; i < System.Enum.GetNames(typeof(Define.TownMaxLevel)).Length; i++)
        {
            if (stage == (int)townMaxLevels[i])
            {
                return true;
            }
        }
        return false;
    }

    private Define.SceneType GetSceneType(Define.SceneName sceneName)
    {
        switch (sceneName)
        {
            default:
                return Define.SceneType.None;
            case Define.SceneName.Login:
            case Define.SceneName.Main:
                return Define.SceneType.Main;
            case Define.SceneName.Game:
            case Define.SceneName.MGMemorization:
            case Define.SceneName.Training:
                return Define.SceneType.InGame;
        }
    }

    public static Define.TownList GetTownList(int level)
    {
        Define.TownMaxLevel[] townMaxLevels = (Define.TownMaxLevel[])System.Enum.GetValues(typeof(Define.TownMaxLevel));
        for (int i=0; i < System.Enum.GetNames(typeof(Define.TownMaxLevel)).Length; i++)
        {
            if(level <= (int)townMaxLevels[i])
            {
                return (Define.TownList)i;
            }
        }
        return Define.TownList.Halloween;
    }
}
