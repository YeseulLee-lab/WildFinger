using UnityEngine;

public class UISafeArea : MonoBehaviour
{
    private void Awake()
    {
        ApplySafeAreaPosition(GetComponent<RectTransform>());
    }

    public void ApplySafeAreaPosition(RectTransform rt)
    {
        if (Screen.safeArea.yMin <= 0f)
        {
            rt.sizeDelta = new Vector2(Screen.width, Screen.height - Screen.safeArea.height);
        }
        else
        {
            rt.sizeDelta = new Vector2(Screen.width, Screen.safeArea.yMin);
        }
    }
}