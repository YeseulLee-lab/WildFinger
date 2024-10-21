using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class InGameBasePopup : MonoBehaviour
{
    [SerializeField]
    private Button _blackPanelBtn;

    public virtual void Start()
    {
        _blackPanelBtn?.onClick.AddListener(OnClickBlackPanelBtn);
    }

    public virtual void OnEnable()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.mobileVibrater.Vibrate();
        }
    }

    public virtual void ShowPopup()
    {
        _blackPanelBtn.interactable = true;
    }

    public void VibrateBtnClick()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.mobileVibrater.Vibrate();
        }
    }

    public void ShowBtnClickSFX()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
        }
    }

    public virtual void OnClickBlackPanelBtn()
    {
        _blackPanelBtn.interactable = false;
    }

    public abstract void SetInteractable(bool active);
}
