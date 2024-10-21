using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using HHK.UIEC;
using TMPro;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using FMOD.Studio;

public class InGameItemManager : MonoBehaviour
{
    [Header("-------------------- Common GUI ---------------------")]
    [SerializeField]
    private Image[] _itemStateImgs; //Shield, Potion, Healing
    [SerializeField]
    private Image _bigItemImg;

    [Header("-------------------- Shield ---------------------")]
    [SerializeField]
    private Image[] _shieldEffectImgs; //2개
    [SerializeField]
    private Sprite[] _shieldSprites; //0: Default, 1,2: Broken
    private int _shieldCnt;
    public int shieldCnt { get { return _shieldCnt; }
        set {
            if(_shieldCnt > value)
            {
                //줄었음
                UseShield(value);
            }
            _shieldCnt = value < 0 ? 0: value;

            //DebugX.Log("Shield Cnt: " + _shieldCnt);
        } }

    [Header("-------------------- Potion ---------------------")]
    [SerializeField]
    private Image _hpHeartImg;
    [SerializeField]
    private Sprite[] _hpHeartSprites; // Default, PotionOn

    [Header("-------------------- Setting ---------------------")]
    [SerializeField]
    private Color32[] _colors; //alpha 0, 255
    [SerializeField]
    private Sprite[] _itemStateSprites; // Inactive Shield, Inactive Potion, Inactive Healing, Active 3 ...
    private CancellationTokenSource _cts;
    private CancellationToken _ct;
    private const int _shieldMS = 305;
    private const int _shieldBrokeMS = 27;
    private const int _itemAnimMS = 1500;
    [SerializeField]
    private UIECAnimator _shieldBrokeAnim;
    private bool[] _isItemUsing = new bool[3] { false, false, false };

    [Header("-------------------- FMOD ---------------------")]
    [SerializeField]
    private EventReference _shieldBGM;
    private EventInstance _shieldInstance;
    [SerializeField]
    private EventReference _shieldBrokeBGM;
    private EventInstance _shieldBrokeInstance;
    [SerializeField]
    private EventReference _itemAnimBGM;
    private EventInstance _itemAnimInstance;
    [SerializeField]
    private EventReference _potionHPBarBGM;
    private EventInstance _potionHPBarInstance;
    public bool isItemInitDone { get; set; } = false;

    #region Unity Life Cycle
    private void Awake()
    {
        _shieldInstance = RuntimeManager.CreateInstance(_shieldBGM);
        _shieldBrokeInstance = RuntimeManager.CreateInstance(_shieldBrokeBGM);
        _itemAnimInstance = RuntimeManager.CreateInstance(_itemAnimBGM);
        _potionHPBarInstance = RuntimeManager.CreateInstance(_potionHPBarBGM);
        isItemInitDone = false;

        shieldCnt = 0;
        _itemStateImgs[0].sprite = _itemStateSprites[(int)Define.UsingItemBeforeInGame.Shield];
        _itemStateImgs[1].sprite = _itemStateSprites[(int)Define.UsingItemBeforeInGame.IncreasedHP];
        _itemStateImgs[2].sprite = _itemStateSprites[(int)Define.UsingItemBeforeInGame.IncreasedHealingHP];
    }
    private void Start()
    {
        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _shieldInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _shieldBrokeInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _itemAnimInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
            _potionHPBarInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    private void OnDestroy()
    {
        _shieldInstance.setUserData(IntPtr.Zero);
        _shieldInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _shieldInstance.release();
        _shieldBrokeInstance.setUserData(IntPtr.Zero);
        _shieldBrokeInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _shieldBrokeInstance.release();
        _itemAnimInstance.setUserData(IntPtr.Zero);
        _itemAnimInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _itemAnimInstance.release();
        _potionHPBarInstance.setUserData(IntPtr.Zero);
        _potionHPBarInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _potionHPBarInstance.release();
        _cts = null;
    }
    #endregion

    #region Use Item
    private async void UseShield(int cnt)
    {
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;
        _shieldInstance.start();
        _itemStateImgs[0].GetComponent<UIECAnimator>().OnCustomChannel();

        if (cnt < 1)
        {
            _shieldBrokeInstance.start();
            _shieldBrokeAnim.OnCustomChannel();
            _shieldEffectImgs[0].sprite = _shieldSprites[1];
            _shieldEffectImgs[1].sprite = _shieldSprites[2];

            if (GamePlayData.Instance != null)
            {
                GamePlayData.Instance.mobileVibrater.Vibrate();
                await UniTask.Delay(_shieldBrokeMS, cancellationToken: _ct);
                GamePlayData.Instance.mobileVibrater.Vibrate();
                await UniTask.Delay(_shieldBrokeMS, cancellationToken: _ct);
                GamePlayData.Instance.mobileVibrater.Vibrate();
            }

            _itemStateImgs[0].sprite = _itemStateSprites[(int)Define.UsingItemBeforeInGame.Shield];
        }

        await UniTask.Delay(_shieldMS, cancellationToken: _ct);
    }
    #endregion

    #region Init Item
    /// <summary>
    /// 아이템 장착하는 애니메이션 재생
    /// </summary>
    /// <returns></returns>
    public async UniTask ShowInitItemAnim()
    {
        //Init
        _isItemUsing = new bool[3] { false, false, false };
        shieldCnt = 0;
        _bigItemImg.color = _colors[0];
        _hpHeartImg.sprite = _hpHeartSprites[0];

        BeatGridTracker.Instance.inGameMaxHP = InGameKey.defaultIngameLife;
        BeatGridTracker.Instance.inGameHealingHP = InGameKey.noteTenComboPoint;

        try
        {
            await UniTask.WaitUntil(() => BeatGridTracker.cutSceneManager.isReady);
            await UniTask.Delay(300, cancellationToken: _ct);
        }
        catch (OperationCanceledException)
        {
            // 취소됐을 때 처리
            DebugX.Log("Item Default Init Canceled");
        }

        if (GamePlayData.Instance == null)
        {
            isItemInitDone = true;
            return;
        }

        //TODO: Shield
        if (GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.Shield])
        {
            await ShowInitItemAnim(Define.UsingItemBeforeInGame.Shield);
        }

        //TODO: Potion
        if (GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.IncreasedHP])
        {
            await ShowInitItemAnim(Define.UsingItemBeforeInGame.IncreasedHP);
        }

        //TODO: HealingHP
        if (GamePlayData.Instance.itemCnts[(int)Define.UsingItemBeforeInGame.IncreasedHealingHP])
        {
            await ShowInitItemAnim(Define.UsingItemBeforeInGame.IncreasedHealingHP);
        }
        
        isItemInitDone = true;
        GamePlayData.Instance.InitItem();
    }

    public async UniTask ShowInitItemAnim(Define.UsingItemBeforeInGame item)
    {
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;
        _itemAnimInstance.start();
        _itemStateImgs[(int)item].sprite = _itemStateSprites[(int)item + 3];
        _itemStateImgs[(int)item].GetComponent<UIECAnimator>().OnCustomChannel();
        _bigItemImg.sprite = _itemStateSprites[(int)item + 3];
        _bigItemImg.GetComponent<UIECAnimator>().OnCustomChannel();

        switch (item)
        {
            case Define.UsingItemBeforeInGame.Shield:
                shieldCnt = InGameKey.itemShieldCnt;
                _shieldEffectImgs[0].sprite = _shieldSprites[0];
                _shieldEffectImgs[1].sprite = _shieldSprites[0];
                try
                {
                    await UniTask.Delay(_itemAnimMS, cancellationToken: _ct);
                }
                catch (OperationCanceledException)
                {
                    // 취소됐을 때 처리
                    DebugX.Log("[ItemManager] Shield Anim Canceled");
                }
                _isItemUsing[0] = true;
                break;
            case Define.UsingItemBeforeInGame.IncreasedHP:
                _potionHPBarInstance.start();
                _hpHeartImg.sprite = _hpHeartSprites[1];
                _hpHeartImg.GetComponent<UIECAnimator>().OnCustomChannel();
                BeatGridTracker.Instance.inGameMaxHP = GamePlayData.Instance.inGameMaxHP;
                BeatGridTracker.Instance.judgeChecker.hpManager.InitHPBar();
                try
                {
                    await UniTask.Delay(_itemAnimMS, cancellationToken: _ct);
                }
                catch (OperationCanceledException)
                {
                    // 취소됐을 때 처리
                    DebugX.Log("[ItemManager] Potion Anim Canceled");
                }
                _isItemUsing[1] = true;
                break;
            case Define.UsingItemBeforeInGame.IncreasedHealingHP:
                BeatGridTracker.Instance.inGameHealingHP = InGameKey.itemIncreasedHealingHPAmount;
                BeatGridTracker.Instance.judgeChecker.comboManager.ShowTenComboUI();
                try
                {
                    await UniTask.Delay(_itemAnimMS, cancellationToken: _ct);
                }
                catch (OperationCanceledException)
                {
                    // 취소됐을 때 처리
                    DebugX.Log("[ItemManager] HealingHP Anim Canceled");
                }
                _isItemUsing[2] = true;
                break;
        }
    }

    public void ShowItemStateAnim(Define.UsingItemBeforeInGame item)
    {
        if (_isItemUsing[(int)item])
        {
            _itemStateImgs[(int)item].GetComponent<UIECAnimator>().OnCustomChannel();
        }
    }

    #endregion
}
