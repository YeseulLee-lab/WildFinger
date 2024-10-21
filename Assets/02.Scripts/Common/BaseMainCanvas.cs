using HHK.UIEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseMainCanvas : MonoBehaviour
{
    [Header("------------ Common GUI ---------------")]
    [SerializeField]
    protected Button _closeButton;
    [SerializeField]
    protected GameObject _showArea;

    public virtual void Start()
    {
        _closeButton?.onClick.AddListener(() => 
        {
            if (GamePlayData.Instance != null)
            {
                GamePlayData.Instance.OnClickBtnEffect();
            }
            HideCanvas(); 
        });
    }

    public virtual void ShowCanvas()
    {
        _showArea.SetActive(true);
        GetComponent<UIECAnimator>()?.OnCustomChannel();
    }

    public virtual void HideCanvas()
    {
        _showArea.SetActive(false);
    }
}
