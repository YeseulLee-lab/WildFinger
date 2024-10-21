using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.Threading;

public partial class TableData : MonoBehaviour
{
    public Dictionary<string, LocalizationInfo> localizationDic { get; private set; }
    public Define.LanguageType languageType { get; private set; }

    private void InitLocalizationInfo(TextAsset textAsset)
    {
        if (textAsset == null)
        {
            Debug.LogError("TextAsset is null");
            return;
        }

        // 파싱할 CSV 데이터
        string csvData = textAsset.text;

        // CSV 데이터 파싱
        List<Dictionary<string, object>> data = CSVManager.ParseCSV(csvData);
        languageType = GetLanguage();
        localizationDic = new Dictionary<string, LocalizationInfo>(data.Count);

        for (int i = 0; i < data.Count; i++)
        {
            LocalizationInfo localizationInfo = new LocalizationInfo();
            localizationInfo.key = data[i]["Key"].ToString();
            localizationInfo.tutorialUIType = SetGameTutorialType(data[i]["TutorialUIType"].ToString());
            localizationInfo.descPosType = SetDescPosType(data[i]["TutorialUIType"].ToString());

            // 언어에 따른 summary 설정
            if (data[i].ContainsKey(languageType.ToString()))
            {
                localizationInfo.summary = data[i][languageType.ToString()].ToString();
            }
            else
            {
                DebugX.LogError($"Language type {languageType} not found for key {localizationInfo.key}");
                localizationInfo.summary = string.Empty;
            }

            if (!localizationDic.ContainsKey(localizationInfo.key))
            {
                localizationDic.Add(localizationInfo.key, localizationInfo);
                //DebugX.Log($"[다국어 지원] {localizationInfo.key} 등록");
            }
            else
            {
                DebugX.LogError($"[다국어 지원] 딕셔너리 key 겹쳐서 등록 불가능: {localizationInfo.key}");
            }
        }
    }

    private Define.LanguageType GetLanguage()
    {
        SystemLanguage systemLanguage = Application.systemLanguage;
        DebugX.Log("언어: " + systemLanguage.ToString());

        switch (systemLanguage)
        {
            default:
            case SystemLanguage.English:
                return Define.LanguageType.English;
            case SystemLanguage.Korean:
                return Define.LanguageType.Korean;
        }
    }

    public static Define.TutorialUIType SetGameTutorialType(string type)
    {
        for (int i = 0; i < Enum.GetValues(typeof(Define.InGameTutorialType)).Length; i++)
        {
            string[] words = type.Split("_");
            if (words[0].Equals(((Define.TutorialUIType)i).ToString()))
            {
                return (Define.TutorialUIType)i;
            }
        }

        return Define.TutorialUIType.None;
    }

    public static Define.DescPosType SetDescPosType(string type)
    {
        for (int i = 0; i < Enum.GetValues(typeof(Define.InGameTutorialType)).Length; i++)
        {
            string[] words = type.Split("_");
            if (words.Length > 1)
            {
                if (words[1].Equals(((Define.DescPosType)i).ToString()))
                {
                    return (Define.DescPosType)i;
                }
            }
            else
            {
                if (words[0].Equals(((Define.DescPosType)i).ToString()))
                {
                    return (Define.DescPosType)i;
                }
            }
        }

        return Define.DescPosType.None;
    }
}