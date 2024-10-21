using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TableData : MonoBehaviour
{
    [SerializeField]
    private TextAsset _localizationTextAsset;

    #region Singleton
    public static TableData instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        //TutorialInfoInit();
        //StringInit();
        InitLocalizationInfo(_localizationTextAsset);
    }
    #endregion
}