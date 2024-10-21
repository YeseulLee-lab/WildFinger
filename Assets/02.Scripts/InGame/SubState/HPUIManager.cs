using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
using FMODUnity;
using FMOD.Studio;
using HHK.UIEC;
using DG.Tweening;
using System.Threading;
using Cysharp.Threading.Tasks;

public class HPUIManager : MonoBehaviour
{
    [Header("------------------ HPBar Setting -----------------")]
    [SerializeField]
    private Image _hpImg;
    [SerializeField]
    private Image _hpBackgroundBarImg;

    [Header("------------------ Dangerous Setting -----------------")]
    [SerializeField]
    private CanvasGroup _dangerousCG;
    [SerializeField]
    private GameObject _damagedPanel;
    private const float _dangerousBlinkDelay = 0.8f;
    private bool _isDangerous { get; set; }

    [Header("------------------ Setting -----------------")]
    [SerializeField]
    private Sprite[] _colorImgs; //0: Full, 1: Orange, 2: Red, 3: Potion
    private float _hpImgFullFillheight = 451f; //여기부터
    private int _hp;
    public int hp { get { return _hp; }
        set {
            if(value > BeatGridTracker.Instance.inGameMaxHP)
            {
                _hp = BeatGridTracker.Instance.inGameMaxHP;
            }
            else if(value < 0)
            {
                _hp = 0;
            }
            else
            {
                _hp = value;
            }
            ShowHPEffect(_hp);
            //DebugX.Log("HP Changed: " + _hp);
        } }
    private CancellationTokenSource _cts;
    private CancellationToken _ct;

    [Header("-------------------- FMOD ---------------------")]
    [SerializeField]
    private EventReference _damagedSFX;
    private EventInstance _damagedInstance;

    #region Unity Life Cycle
    private void Awake()
    {
        _damagedInstance = RuntimeManager.CreateInstance(_damagedSFX);
        _dangerousCG.alpha = 0;
        _hpImgFullFillheight = _hpImg.rectTransform.sizeDelta.y;
    }

    private void OnDestroy()
    {
        _damagedInstance.setUserData(IntPtr.Zero);
        _damagedInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _damagedInstance.release();

        _hpImg = null;
        _colorImgs = null;
    }

    private void Start()
    {
        //SFX 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _damagedInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }
    #endregion

    /// <summary>
    /// 해당 값으로 HP 세팅함.
    /// </summary>
    /// <param name="hp">세팅할 hp값.</param>
    public void SetHP(int hp)
    {
        this.hp = hp;
    }

    public void InitHPBar()
    {
        float potionBarHeight = _hpBackgroundBarImg.rectTransform.sizeDelta.y + (_hpImgFullFillheight * (hp / (float)InGameKey.defaultIngameLife)) - _hpImgFullFillheight;
        _hpBackgroundBarImg.rectTransform.DOSizeDelta(new Vector2(_hpBackgroundBarImg.rectTransform.sizeDelta.x, potionBarHeight), _dangerousBlinkDelay);
        
        float targetHeight = _hpImgFullFillheight * (hp / (float)InGameKey.defaultIngameLife);
        _hpImg.rectTransform.DOSizeDelta(new Vector2(_hpImg.rectTransform.sizeDelta.x, targetHeight), _dangerousBlinkDelay);
        _hpImgFullFillheight = targetHeight;
    }

    /// <summary>
    /// 현재 HP에 값을 더함
    /// </summary>
    /// <param name="chageHP">현재 HP에 더할 값</param>
    public async void ChangeHP(int chageHP)
    {
        if (chageHP < 0)
        {
            //공격 받음
            BeatGridTracker.Instance.monsterManager.SetMonsterAnim((Define.MonsterAnimType)UnityEngine.Random.Range(5, 9));
            BeatGridTracker.Instance.judgeChecker.itemManager.ShowItemStateAnim(Define.UsingItemBeforeInGame.IncreasedHP);

            _damagedInstance.start();
            _hpImg.GetComponent<UIECAnimator>().OnCustomChannel();
            _cts = new CancellationTokenSource();
            _ct = _cts.Token;
            _damagedPanel.SetActive(true);
            await UniTask.Delay(InGameKey.judgeEffectMS, cancellationToken: _ct);
            _damagedPanel.SetActive(false);
            BeatGridTracker.Instance.ShakeCam(chageHP <= InGameKey.noteMissPoint ? Define.InGameShakeScale.Midium : Define.InGameShakeScale.Small);

            if (TownDataLoader.isTraining)
            {
                //체력 안닳음
                return;
            }
        }

        this.hp += chageHP;
        //DebugX.Log("changed Hp: " + hp);
    }

    /// <summary>
    /// 현재 HP 값이 얼마인지에 따라 변화하는 상태를 보여줌
    /// </summary>
    /// <param name="curHP">0~100 사이 값</param>
    private void ShowHPEffect(float curHP)
    {
        //길이가 _damagedMS 밀리세컨드 동안 변화함
        float targetHeight = _hpImgFullFillheight * (curHP / (float)BeatGridTracker.Instance.inGameMaxHP);
        _hpImg.rectTransform.DOSizeDelta(new Vector2(_hpImg.rectTransform.sizeDelta.x, targetHeight), InGameKey.judgeEffectMS * 0.001f);

        if (curHP > (float)InGameKey.defaultIngameLife)
        {
            _hpImg.sprite = _colorImgs[3];
            DangerousEffect(false);
        }
        else if (curHP > (float)Define.HPType.Enough)
        {
            _hpImg.sprite = _colorImgs[0];
            DangerousEffect(false);
        }
        else if(curHP > (float)Define.HPType.Low)
        {
            _hpImg.sprite = _colorImgs[1];
            DangerousEffect(false);
        }
        else if(curHP > (float)Define.HPType.Zero)
        {
            _hpImg.sprite = _colorImgs[2];
            DangerousEffect(true);
        }
    }

    #region Dangerous
    private void DangerousEffect(bool isDangerous)
    {
        if (_isDangerous == isDangerous)
        {
            return;
        }

        _isDangerous = isDangerous;

        if (_isDangerous)
        {
            // DOTween을 사용하여 _dangerousCG의 alpha 값을 0과 1 사이에서 깜빡이도록 설정
            _dangerousCG.DOFade(1f, _dangerousBlinkDelay / 2f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            // 깜빡이는 애니메이션 중지하고 alpha 값을 0으로 설정
            _dangerousCG.DOKill(); // 애니메이션 중지
            _dangerousCG.alpha = 0f; // alpha 값을 0으로 설정
        }
    }
    #endregion
}
