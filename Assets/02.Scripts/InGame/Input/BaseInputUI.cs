using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BaseInputUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private BaseRSPInputManager _rspInputManager;

    [field: SerializeField]
    public Define.InputType inputType { get; set; }
    [field: SerializeField]
    public Define.RSPType rspType { get; set; }
    [field: SerializeField]
    public Define.LogicType logicType { get; set; }

    [Header("-------------------- GUI ---------------------")]
    [SerializeField]
    private Image _btnBGImg;
    public Image btnBGImg => _btnBGImg;
    [SerializeField]
    private Color32[] _colors; //bg default, bg selected
    [SerializeField]
    private Sprite[] _btnImgs; //btn - Default, Pressed
    public Vector2 defaultPos { get; private set; }
    public Vector2 defaultBGImgPos { get; private set; }

    private CancellationTokenSource _cts;

    #region Unity Life Cycle
    public virtual void Awake()
    {
        defaultPos = this.GetComponent<RectTransform>().anchoredPosition;
        defaultBGImgPos = _btnBGImg.rectTransform.anchoredPosition;
    }

    public virtual void OnEnable()
    {
        _btnBGImg.color = _colors[0];
        _btnBGImg.sprite = _btnImgs[0];
        _cts = new CancellationTokenSource();
    }

    public virtual void OnDestroy()
    {
        _rspInputManager = null;
        _btnBGImg = null;
        _colors = null;
        _btnImgs = null;
        _cts.Cancel();
        _cts.Dispose();
    }

    #endregion

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (inputType == Define.InputType.RSP)
        {
            _rspInputManager.OnPointerDown((int)rspType, this);
        }
        else
        {
            _rspInputManager.OnPointerDown((int)logicType, this);
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (inputType == Define.InputType.RSP)
        {
            _rspInputManager.OnPointerUp((int)rspType, this);
        }
        else
        {
            _rspInputManager.OnPointerUp((int)logicType, this);
        }
    }

    public virtual void BtnPressAnim(bool isPressed)
    {
        if (isPressed)
        {
            if (GamePlayData.Instance != null)
            {
                GamePlayData.Instance.mobileVibrater.Vibrate();
            }
            _btnBGImg.color = _colors[1];
            _btnBGImg.sprite = _btnImgs[1];
        }
        else
        {
            _btnBGImg.color = _colors[0];
            _btnBGImg.sprite = _btnImgs[0];
        }
    }

    public virtual async void ResetBtnUI(int mSec, UnityAction resetAction = null)
    {
        try
        {
            // Wait for the specified milliseconds
            await UniTask.Delay(mSec, cancellationToken: _cts.Token);

            // Set the button's position to the default position
            this.GetComponent<RectTransform>().anchoredPosition = defaultPos;
            RectTransform rectTransformBg = _btnBGImg.rectTransform;
            float duration = 0.3f; // duration of the animation in seconds
            float elapsedTime = 0f;

            Vector2 startBGImgPos = rectTransformBg.anchoredPosition;

            // Animate the background image position over the specified duration
            while (elapsedTime < duration)
            {
                if (_cts.Token.IsCancellationRequested)
                    break;

                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                rectTransformBg.anchoredPosition = Vector2.Lerp(startBGImgPos, defaultBGImgPos, t);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            // Ensure the final position is set
            rectTransformBg.anchoredPosition = defaultBGImgPos;

            resetAction?.Invoke();
            //DebugX.Log($"{this.name} - rectTransform.anchoredPosition = {defaultPos}");
        }
        catch (OperationCanceledException)
        {
            // Handle the cancellation if needed
        }
    }

}
