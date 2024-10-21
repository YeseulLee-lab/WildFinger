using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HHK.UIEC;
using TMPro;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Events;

public class InGameTutorialManager : MonoBehaviour
{
    [Header("------------------ GUI Setting -----------------")]
    [SerializeField]
    private TutorialCanvas _tutorialCanvas;
    [SerializeField]
    private TutorialPopupData _tutorialPopupData; //Popup은 아직까지 인게임에서만 쓰기때문에
    [SerializeField]
    private TutorialHandPanel _handTutorial;
    private Dictionary<Define.InGameTutorialType, InGameTutorialPopupInfo> _tutorialPopupDic { get; set; }

    [Header("------------------ Resume Setting -----------------")]
    [SerializeField]
    private GameObject _resumeCntPanel;
    [SerializeField]
    private TextMeshProUGUI _resumeCntDownText;
    private CancellationTokenSource _cts;
    private CancellationToken _ct;

    [Header("-------------------- FMOD ---------------------")]
    [SerializeField]
    private EventReference _cntDownBGM;
    private EventInstance _cntDownInstance;
    public bool isReady { get; private set; } = false;
    public bool isTutorial { get; private set; } = false;
    private bool _actionTutorial = false;
    private Define.NoteTypeA _actionTutorialType;

    private void Awake()
    {
        _cntDownInstance = RuntimeManager.CreateInstance(_cntDownBGM);

        _tutorialPopupDic = new Dictionary<Define.InGameTutorialType, InGameTutorialPopupInfo>(_tutorialPopupData.tutorialPopupData.Length);

        for(int i =0; i< _tutorialPopupData.tutorialPopupData.Length; i++)
        {
            _tutorialPopupDic.Add(_tutorialPopupData.tutorialPopupData[i].type, _tutorialPopupData.tutorialPopupData[i]);
        }

        isReady = true;
    }

    private void Start()
    {
        if (GamePlayData.Instance != null)
        {
            _cntDownInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
        _resumeCntPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        _cntDownInstance.setUserData(IntPtr.Zero);
        _cntDownInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _cntDownInstance.release();

        _tutorialCanvas = null;
        _resumeCntPanel = null;
        _resumeCntDownText = null;
        _cts = null;
    }

    public void StartTutorial(Define.InGameTutorialType type, UnityAction hideAction = null)
    {
        if (_tutorialCanvas.IsTutorialDone(type))
        {
            return;
        }

        isTutorial = true;
        BeatGridTracker.Instance.SetPauseBtn(false);
        //Globalization 적용된 튜토리얼
        BeatGridTracker.SetGameState(Define.InGameState.Paused);
        RectTransform[] maskRects = null;

        switch (type)
        {
            case Define.InGameTutorialType.Rock:
                _actionTutorial = true;
                _actionTutorialType = Define.NoteTypeA.A_CLK;
                maskRects = new RectTransform[] { BeatGridTracker.rspInputManager.GetInputBtnRect(Define.RSPType.Rock) };
                break;
            case Define.InGameTutorialType.ScissorBtn:
                maskRects = new RectTransform[] { BeatGridTracker.rspInputManager.GetInputBtnRect(Define.RSPType.Scissor) };
                break;
            case Define.InGameTutorialType.PaperBtn:
                maskRects = new RectTransform[] { BeatGridTracker.rspInputManager.GetInputBtnRect(Define.RSPType.Paper) };
                break;
            case Define.InGameTutorialType.Hold:
                _actionTutorial = true;
                _actionTutorialType = Define.NoteTypeA.A_HLD;
                break;
            case Define.InGameTutorialType.Flick:
                _actionTutorial = true;
                _actionTutorialType = Define.NoteTypeA.A_FLK;
                break;
        }

        InGameTutorialPopupInfo info;
        if (_tutorialPopupDic.TryGetValue(type, out info))
        {
            _tutorialCanvas.StartIngameTutorial(type, info.imgs, () => HideTutorial(type), maskRects);
        }
        else
        {
            _tutorialCanvas.StartIngameTutorial(type, null, () => HideTutorial(type), maskRects);
        }
    }

    public async void HideTutorial(Define.InGameTutorialType type)
    {
        //TODO: 카운트 후 다시 시작
        if (_actionTutorial)
        {
            _actionTutorial = false;
            _handTutorial.ShowTutorial(_actionTutorialType, () => HideTutorial(type));
            return;
        }

        switch (type)
        {
            case Define.InGameTutorialType.Rock:
                JsonNoteLoader.gameRuleUIManager.SetRSPBtn(Define.RSPType.Rock, true);
                break;
            case Define.InGameTutorialType.ScissorBtn:
                JsonNoteLoader.gameRuleUIManager.SetRSPBtn(Define.RSPType.Scissor, true);
                break;
            case Define.InGameTutorialType.PaperBtn:
                JsonNoteLoader.gameRuleUIManager.SetRSPBtn(Define.RSPType.Paper, true);
                break;
            case Define.InGameTutorialType.ItemShield:
                if(GamePlayData.Instance != null)
                {
                    GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.Shield] = true;
                }
                BeatGridTracker.Instance.judgeChecker.itemManager.shieldCnt = InGameKey.itemShieldCnt;
                await BeatGridTracker.Instance.judgeChecker.itemManager.ShowInitItemAnim(Define.UsingItemBeforeInGame.Shield);
                break;
            case Define.InGameTutorialType.ItemIncreaseHP:
                if (GamePlayData.Instance != null)
                {
                    if (!GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.IncreasedHP])
                    {
                        GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.IncreasedHP] = true;
                        GamePlayData.Instance.inGameMaxHP = InGameKey.itemIncreasedHPAmount + InGameKey.defaultIngameLife;
                    }
                }
                BeatGridTracker.Instance.inGameMaxHP = GamePlayData.Instance.inGameMaxHP;
                await BeatGridTracker.Instance.judgeChecker.itemManager.ShowInitItemAnim(Define.UsingItemBeforeInGame.IncreasedHP);
                break;
            case Define.InGameTutorialType.ItemHPPotion:
                if (GamePlayData.Instance != null)
                {
                    if (!GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.IncreasedHealingHP])
                    {
                        GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.IncreasedHealingHP] = true;
                    }
                    BeatGridTracker.Instance.inGameHealingHP = InGameKey.itemIncreasedHealingHPAmount;
                    await BeatGridTracker.Instance.judgeChecker.itemManager.ShowInitItemAnim(Define.UsingItemBeforeInGame.IncreasedHealingHP);
                }
                break;
        }

        _cts = new CancellationTokenSource();
        _ct = _cts.Token;
        _resumeCntPanel.SetActive(true);

        for (int i = InGameKey.resumeCntDown; i > 0; i--)
        {
            _resumeCntDownText.text = i.ToString();
            _cntDownInstance.start();

            await UniTask.Delay(1000, cancellationToken: _ct);
        }

        isTutorial = false;
        //게임 재개
        BeatGridTracker.SetGameState(Define.InGameState.Resumed);
        _resumeCntPanel.SetActive(false);
        BeatGridTracker.Instance.SetPauseBtn(true);
        _actionTutorial = false;
    }

    public InGameTutorialPopupInfo GetTutorialData(Define.InGameTutorialType type)
    {
        InGameTutorialPopupInfo info = null;

        if (_tutorialPopupDic.ContainsKey(type))
        {
            info = _tutorialPopupDic[type];
        }
        return info;
    }
}
