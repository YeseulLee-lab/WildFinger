using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SettingToggle : MonoBehaviour
{
    public RectTransform toggleFocusRect;
    public Ease moveEase;
    public float margin;
    public float duration;

    private void Start()
    {
        GetComponent<Toggle>().onValueChanged.AddListener((isOn) =>
        {
            SwitchToggleButton(isOn);
        });
    }

    public void SwitchToggleButton(bool isOn)
    {
        if (!isOn)
        {
            toggleFocusRect.DOLocalMoveX(-toggleFocusRect.rect.width - margin, duration).SetEase(moveEase);
        }
        else
        {
            toggleFocusRect.DOLocalMoveX(margin, duration).SetEase(moveEase);
        }
    }
}
