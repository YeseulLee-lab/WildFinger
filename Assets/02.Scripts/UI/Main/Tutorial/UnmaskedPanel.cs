using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnmaskedPanel : MonoBehaviour
{
    [SerializeField]
    private Unmask unmask;

    public void SetUnmaskedTarget(RectTransform target)
    {
        unmask.fitTarget = target;
    }
}
