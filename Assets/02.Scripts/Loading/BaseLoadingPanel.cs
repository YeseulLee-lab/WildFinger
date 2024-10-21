using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseLoadingPanel : MonoBehaviour
{
    public abstract void Show(UnityAction<float> progress = null, UnityAction DownloadComplete = null);
    public abstract void Hide();
}
