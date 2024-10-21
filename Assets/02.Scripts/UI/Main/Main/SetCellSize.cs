using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetCellSize : MonoBehaviour
{
    [SerializeField]
    private GridLayoutGroup gridLayoutGroup;
    [SerializeField]
    private CanvasScaler canvasScaler;

    private void Start()
    {
        SetSize();
    }

    private void SetSize()
    {
        gridLayoutGroup.cellSize = new Vector2(Screen.width, Screen.height);
        canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
    }
}
