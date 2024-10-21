using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISafeAreaElement : MonoBehaviour
{
    [SerializeField]
    private RectTransform headerArea;

    private void Start()
    {
        StartCoroutine(CoSetOffSet());
    }

    IEnumerator CoSetOffSet()
    {
        yield return new WaitForEndOfFrame();
        //Do your stuff
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMax = new Vector2(0f, -headerArea.sizeDelta.y);
    }
}
