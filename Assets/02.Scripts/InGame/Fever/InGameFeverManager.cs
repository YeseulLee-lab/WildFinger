using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using HHK.UIEC;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System;

public class InGameFeverManager : MonoBehaviour
{
    [Header("-------------------- Anim Setting ---------------------")]
    [SerializeField]
    private UIECGroup _feverTimerIconAnim;
    [SerializeField]
    private UIECGroup _feverTimerFillAnim;
    [SerializeField]
    private UIECGroup _deckAnim;

    [Header("-------------------- GUI Setting ---------------------")]
    [SerializeField]
    private Image _feverBackgroundBarImg;
    [SerializeField]
    private Image _feverImg;
    [SerializeField]
    private GameObject _feverTimeObj;
    [SerializeField]
    private CanvasGroup _IngameRoadCG;
    private float _feverImgFullFillheight = 451f; //여기부터
    private const float _feverFillDelay = 0.5f;

    [Header("-------------------- FMOD ---------------------")]
    [SerializeField]
    private EventReference _feverBGM;
    public EventInstance feverBGMInstance { get; private set; }
    [SerializeField]
    private EventReference _feverStartSFX;
    private EventInstance _feverStartSFXInstance;
    public Define.FeverType feverType { get; set; } = Define.FeverType.None;

    [Header("-------------------- Fever Setting ---------------------")]
    private float _feverTime;
    public float feverTime
    {
        get { return _feverTime; }
        set
        {
            _feverTime = value;
        }
    }
    private const float _feverVolumeDelay = 1.5f;
    public bool isFever { get; set; } = false;
    private bool _isPaused { get; set; } = false;
    private CancellationTokenSource _cts;
    private CancellationToken _ct;

    #region Unity Life Cycle
    private void Awake()
    {
        _isPaused = false;
        isFever = false;
        feverBGMInstance = RuntimeManager.CreateInstance(_feverBGM);
        _feverStartSFXInstance = RuntimeManager.CreateInstance(_feverStartSFX);
    }

    private void Start()
    {
        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            feverBGMInstance.setVolume(GamePlayData.Instance.isCommonBGMOn ? 1f : 0f);
            _feverStartSFXInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    private void OnDestroy()
    {
        feverBGMInstance.setUserData(IntPtr.Zero);
        feverBGMInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        feverBGMInstance.release();

        _feverStartSFXInstance.setUserData(IntPtr.Zero);
        _feverStartSFXInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _feverStartSFXInstance.release();
    }
    #endregion

    #region UI
    public void InitFeverBar()
    {
        feverTime = InGameKey.feverTime;

        float potionBarHeight = _feverBackgroundBarImg.rectTransform.sizeDelta.y;
        _feverBackgroundBarImg.rectTransform.DOSizeDelta(new Vector2(_feverBackgroundBarImg.rectTransform.sizeDelta.x, potionBarHeight), _feverFillDelay);

        float targetHeight = _feverImgFullFillheight;
        _feverImg.rectTransform.DOSizeDelta(new Vector2(_feverImg.rectTransform.sizeDelta.x, targetHeight), _feverFillDelay);
        _feverImgFullFillheight = targetHeight;
    }

    /// <summary>
    /// Fever Type별 Fever UI와 VFX 세팅
    /// </summary>
    /// <param name="type"></param>
    private void StartFeverUI(Define.FeverType type)
    {
        BeatGridTracker.noteRoadManager.SetBackground(true);
        _feverTimeObj.SetActive(true);
        switch (type)
        {
            case Define.FeverType.Pinata:
                _IngameRoadCG.alpha = 0f;
                BeatGridTracker.Instance.monsterManager.MoveMonsterPos(Define.MonsterPosType.FeverPinata);
                _deckAnim.Hide();
                break;
            case Define.FeverType.SuperShooting:
                break;
            case Define.FeverType.Clicker:
                break;
        }
    }

    private void EndFeverUI()
    {
        BeatGridTracker.noteRoadManager.SetBackground(false);
        _feverTimeObj.SetActive(false);
        switch (feverType)
        {
            case Define.FeverType.Pinata:
                _IngameRoadCG.alpha = 1f;
                BeatGridTracker.Instance.monsterManager.MoveMonsterPos(Define.MonsterPosType.Default);
                _deckAnim.Show();
                break;
            case Define.FeverType.SuperShooting:
                break;
            case Define.FeverType.Clicker:
                break;
        }
    }
    #endregion

    #region Fever Mode
    /// <summary>
    /// 피버타임 시작함, Training에서는 발동하지 않음 처리 추가해야 함.
    /// </summary>
    /// <param name="type">Random설정 시 아무거나로 나옴</param>
    public async void StartFeverTime(Define.FeverType type = Define.FeverType.Random)
    {
        if (isFever)
        {
            return;
        }

        isFever = true;
        BeatGridTracker.SetGameState(Define.InGameState.FeverTime);

        feverType = type;
        
        InitFeverBar();

        if (type == Define.FeverType.Random)
        {
            type = (Define.FeverType)UnityEngine.Random.Range(1, Enum.GetNames(typeof(Define.FeverType)).Length - 1);
        }

        _feverStartSFXInstance.start();
        feverBGMInstance.start();

        if (GamePlayData.Instance != null)
        {
            if (GamePlayData.Instance.isCommonBGMOn)
            {
                DOTween.To(() => GetMusicVolume(BeatGridTracker.musicPlayEvent), x => BeatGridTracker.musicPlayEvent.setVolume(x), 0f, _feverVolumeDelay);
                DOTween.To(() => 0f, x => feverBGMInstance.setVolume(x), 1f, _feverVolumeDelay);
            }
        }
        else
        {
            DOTween.To(() => GetMusicVolume(BeatGridTracker.musicPlayEvent), x => BeatGridTracker.musicPlayEvent.setVolume(x), 0f, _feverVolumeDelay);
            DOTween.To(() => 0f, x => feverBGMInstance.setVolume(x), 1f, _feverVolumeDelay);
        }

        _feverTimerIconAnim.Show();
        _feverTimerFillAnim.Show();

        StartFeverUI(type);

        // Fever Time countdown and UI update
        float elapsedTime = 0f;
        while (elapsedTime < feverTime)
        {
            switch (BeatGridTracker.curState)
            {
                case Define.InGameState.Paused:
                case Define.InGameState.PausedCntDown:
                    break;
                default:
                    elapsedTime += Time.deltaTime;
                    float remainingTime = feverTime - elapsedTime;
                    UpdateFeverUI(remainingTime);
                    break;
                case Define.InGameState.End:
                    EndFeverTime();
                    return;
            }

            await UniTask.Yield();
        }

        EndFeverTime();
    }

    private void UpdateFeverUI(float remainingTime)
    {
        float targetHeight = Mathf.Lerp(0f, _feverImgFullFillheight, remainingTime / feverTime);
        _feverImg.rectTransform.sizeDelta = new Vector2(_feverImg.rectTransform.sizeDelta.x, targetHeight);
    }

    public async void EndFeverTime()
    {
        if (!isFever)
        {
            return;
        }
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;
        isFever = false;

        if(GamePlayData.Instance != null)
        {
            if (GamePlayData.Instance.isCommonBGMOn)
            {
                DOTween.To(() => GetMusicVolume(BeatGridTracker.musicPlayEvent), x => BeatGridTracker.musicPlayEvent.setVolume(x), 1f, _feverVolumeDelay);
                DOTween.To(() => GetMusicVolume(feverBGMInstance), x => feverBGMInstance.setVolume(x), 0f, _feverVolumeDelay);
            }
        }
        else
        {
            DOTween.To(() => GetMusicVolume(BeatGridTracker.musicPlayEvent), x => BeatGridTracker.musicPlayEvent.setVolume(x), 1f, _feverVolumeDelay);
            DOTween.To(() => GetMusicVolume(feverBGMInstance), x => feverBGMInstance.setVolume(x), 0f, _feverVolumeDelay);
        }

        try
        {
            await UniTask.Delay((int)(1000 * _feverVolumeDelay), cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {
            // 이전 작업이 취소되면 예외 발생, 무시
            DebugX.Log("이전 작업 취소됨");
        }

        EndFeverUI();
        BeatGridTracker.SetGameState(Define.InGameState.Playing);
        _feverTimerIconAnim.Hide();
        _feverTimerFillAnim.Hide();
        feverType = Define.FeverType.None;
    }

    private float GetMusicVolume(EventInstance musicEvent)
    {
        float volume;
        musicEvent.getVolume(out volume);
        return volume;
    }

    public void SetFeverTimerState(bool isPause)
    {
        if (!isFever)
        {
            return;
        }

        _isPaused = isPause;
        feverBGMInstance.setPaused(isPause);
    }
    #endregion
}
