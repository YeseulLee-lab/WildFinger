using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CollectingCoinManager : MonoBehaviour
{
    [Header("------------------ Effect Detail Area -----------------")]
    [SerializeField]
    private float _moveRate;
    [SerializeField]
    private float _incScaleRate;
    [SerializeField]
    private float _decScaleRate;
    [SerializeField]
    private float _moveDelayRate;
    [SerializeField]
    private float _scaleDelayRate;
    [SerializeField]
    private float _endDelayRate;

    [SerializeField]
    private GameObject _pileOfCoins;

    private Vector3[] _initialPos;
    private Quaternion[] _initialRotation;

    [Header("------------------ SFX Area -----------------")]
    [SerializeField]
    private EventReference _collectingSfx;
    private EventInstance _collectingSfxInstance;

    #region Unity Life Cycle
    private void Awake()
    {
        _collectingSfxInstance = RuntimeManager.CreateInstance(_collectingSfx);
    }
    private void Start()
    {
        if (GamePlayData.Instance != null)
        {
            _collectingSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }

        _initialPos = new Vector3[_pileOfCoins.transform.childCount];
        _initialRotation = new Quaternion[_pileOfCoins.transform.childCount];

        for (int i = 0; i < _pileOfCoins.transform.childCount; i++)
        {
            _initialPos[i] = _pileOfCoins.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
            _initialRotation[i] = _pileOfCoins.transform.GetChild(i).GetComponent<RectTransform>().rotation;
        }
    }

    private void OnDestroy()
    {
        _collectingSfxInstance.setUserData(IntPtr.Zero);
        _collectingSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _collectingSfxInstance.release();
    }

    private void Reset()
    {
        for (int i = 0; i < _pileOfCoins.transform.childCount; i++)
        {
            _pileOfCoins.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = _initialPos[i];
            _pileOfCoins.transform.GetChild(i).GetComponent<RectTransform>().rotation = _initialRotation[i];
        }
    }
    #endregion


    public void UseWealth(int coinNum, RectTransform desRect, Text targetText, UnityAction endAction)
    {
        Reset();

        float delay =  0f;

        _pileOfCoins.SetActive(true);

        int coinToMove = coinNum;

        if(coinNum > _pileOfCoins.transform.childCount)
            coinToMove = _pileOfCoins.transform.childCount;

        for (int i = 0; i < coinToMove; i++)
        {
            _pileOfCoins.transform.GetChild(i).DOScale(1f, _incScaleRate).SetDelay(delay).SetEase(Ease.OutBack);
            _pileOfCoins.transform.GetChild(i).DOMove(desRect.position, _moveRate).SetDelay(delay + _moveDelayRate).SetEase(Ease.InBack).OnComplete(() =>
            {
                targetText.rectTransform.DOPunchScale(new Vector3(1.01f, 1.01f, 1.01f), 0.1f);
                int tempNum = int.Parse(targetText.text) - 1;
                targetText.text = tempNum.ToString();
                PlayCollectingSound();
            });
            
            if (i == coinToMove - 1)
            {
                _pileOfCoins.transform.GetChild(i).DOScale(0f, _decScaleRate).SetDelay(delay + _scaleDelayRate).SetEase(Ease.InBack).OnComplete(() =>
                {
                    targetText.text = 0.ToString();
                    endAction.Invoke();
                });
            }
            else
            {
                _pileOfCoins.transform.GetChild(i).DOScale(0f, _decScaleRate).SetDelay(delay + _scaleDelayRate).SetEase(Ease.InBack);
            }

            delay += _endDelayRate;
        }
    }

    public void RewardWealth(int coinNum, RectTransform desRect, Text targetText, UnityAction endAction)
    {
        Reset();

        float delay = 0f;
        int num = coinNum;
        if (coinNum > _pileOfCoins.transform.childCount)
        {
            num = _pileOfCoins.transform.childCount;
        }

        _pileOfCoins.SetActive(true);

        for (int i = 0; i < num; i++)
        {
            _pileOfCoins.transform.GetChild(i).DOScale(1f, _incScaleRate).SetDelay(delay).SetEase(Ease.OutBack);
            _pileOfCoins.transform.GetChild(i).DOMove(desRect.position, _moveRate).SetDelay(delay + _moveDelayRate).SetEase(Ease.InBack).OnComplete(() =>
            {
                targetText.rectTransform.DOPunchScale(new Vector3(1.01f, 1.01f, 1.01f), 0.1f);
                int tempNum = int.Parse(targetText.text) + 1;
                targetText.text = tempNum.ToString();
                PlayCollectingSound();
            });
            
            if (i == num - 1)
            {
                _pileOfCoins.transform.GetChild(i).DOScale(0f, _decScaleRate).SetDelay(delay + _scaleDelayRate).SetEase(Ease.InBack).OnComplete(() =>
                {
                    endAction.Invoke();
                });
            }
            else
            {
                _pileOfCoins.transform.GetChild(i).DOScale(0f, _decScaleRate).SetDelay(delay + _scaleDelayRate).SetEase(Ease.InBack);
            }

            delay += _endDelayRate;
        }
    }

    private void PlayCollectingSound()
    {
        _collectingSfxInstance.start();
    }
}
