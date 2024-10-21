using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;

public class StreamingAssetPather : MonoBehaviour
{
    private const string _saveNoteFileType = ".json";
    private const string _saveLocalizationFileType = ".csv";
    private const string _saveLocalizationImgType = ".png";

    public static string GetFilePathCommon(Define.TownList town, int stage)
    {
        StringBuilder sb = new StringBuilder(6);
        sb.Append(Application.streamingAssetsPath);
        sb.Append("/");
        sb.Append(town);
        sb.Append("/Stage");
        sb.Append(stage.ToString());
        sb.Append(_saveNoteFileType);

        return sb.ToString();
    }

    public static string GetFilePathIOS(Define.TownList town, int stage)
    {
        StringBuilder sb = new StringBuilder(7);
        sb.Append("file:/");
        sb.Append(Application.streamingAssetsPath);
        sb.Append("/");
        sb.Append(town);
        sb.Append("/Stage");
        sb.Append(stage.ToString());
        sb.Append(_saveNoteFileType);

        return sb.ToString();
    }

    public static string GetLocalizationFile()
    {
       return Path.Combine(Application.dataPath, "Resources", BasicKey.localizationDataTable + _saveLocalizationFileType);
    }

    public static string GetLocalizationImgFile(string key)
    {
        StringBuilder sb = new StringBuilder(7);
#if UNITY_IOS && !UNITY_EDITOR
        sb.Append("file:/");
#endif
        sb.Append(Application.streamingAssetsPath);
        sb.Append("/Localization/");
        sb.Append(key);
        sb.Append(_saveLocalizationImgType);

        return sb.ToString();
    }
}
