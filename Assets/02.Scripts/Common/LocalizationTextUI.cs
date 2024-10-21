using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

[ExecuteInEditMode]
public class LocalizationTextUI : MonoBehaviour
{
    [field: SerializeField]
    public LocalizationData localizationData = new LocalizationData();

    /// <summary>
    /// 인스펙터에 있는 해당 버튼 클릭시 키 존재 여부 확인
    /// </summary>
#if ODIN_INSPECTOR
    [Button("Check if Key Exists")]
#endif
    private void OnClickCheckKeyExists()
    {
        if (!string.IsNullOrEmpty(localizationData.key))
        {
            string filePath = StreamingAssetPather.GetLocalizationFile();
            TextAsset textAsset = Resources.Load<TextAsset>(BasicKey.localizationDataTable + ".csv");

            if (File.Exists(filePath))
            {
                var existingData = CSVManager.ReadCSVList(textAsset);
                foreach (var line in existingData)
                {
                    var values = line.Split(',');
                    if (values[0] == localizationData.key)
                    {
                        Debug.Log($"Key {localizationData.key} exists in the CSV file.");
                        return;
                    }
                }
                Debug.Log($"Key {localizationData.key} does not exist in the CSV file.");
            }
            else
            {
                Debug.LogError($"CSV file not found at path: {filePath}");
            }
        }
        else
        {
            Debug.LogError("Localization key is empty or null.");
        }
    }

    /// <summary>
    /// 인스펙터에 있는 해당 버튼 클릭시 키 존재할 때 키 삭제
    /// </summary>
#if ODIN_INSPECTOR
    [Button("Delete Key")]
#endif
    private void OnClickDeleteKey()
    {
        if (!string.IsNullOrEmpty(localizationData.key))
        {
            string filePath = StreamingAssetPather.GetLocalizationFile();
            TextAsset textAsset = Resources.Load<TextAsset>(BasicKey.localizationDataTable + ".csv");

            if (File.Exists(filePath))
            {
                var existingData = CSVManager.ReadCSVList(textAsset);
                var updatedData = new List<string>();

                bool keyFound = false;
                foreach (var line in existingData)
                {
                    var values = line.Split(',');
                    if (values[0] == localizationData.key)
                    {
                        keyFound = true;
                        continue; // Skip the line with the matching key
                    }
                    updatedData.Add(line);
                }

                if (keyFound)
                {
                    File.WriteAllLines(filePath, updatedData);
                    Debug.Log($"Key {localizationData.key} has been deleted from the CSV file.");
                }
                else
                {
                    Debug.Log($"Key {localizationData.key} does not exist in the CSV file.");
                }
            }
            else
            {
                Debug.LogError($"CSV file not found at path: {filePath}");
            }
        }
        else
        {
            Debug.LogError("Localization key is empty or null.");
        }
    }

#if UNITY_EDITOR
    private void Awake()
    {
        EnsureTextComponent();
    }

    private void EnsureTextComponent()
    {
        if (this.GetComponent<Text>() == null)
        {
            Debug.Log("Text component not found, adding Text component.");
            this.gameObject.AddComponent<Text>();
        }
    }

    private void OnValidate()
    {
        // This will ensure the text component is added even when changes are made in the inspector
        EnsureTextComponent();
    }

    /// <summary>
    /// 인스펙터에 있는 해당 버튼 클릭시 CSV 파일 저장 
    /// </summary>
#if ODIN_INSPECTOR
    [Button("Save CSV Data")]
#endif
    private void OnClickSaveCSVData()
    {
        if (!string.IsNullOrEmpty(localizationData.key))
        {
            localizationData.tutorialUIType = Define.TutorialUIType.None;
            string filePath = StreamingAssetPather.GetLocalizationFile();
            TextAsset textAsset = Resources.Load<TextAsset>(BasicKey.localizationDataTable + ".csv");

            CSVManager.WriteLocalizationData(textAsset, localizationData);
        }
    }
#endif

    private void Start()
    {
        //Set Localization Data
        this.gameObject.GetComponent<Text>().text = GetSummary(localizationData.key);
    }

    public string GetSummary(string localizationDataKey)
    {
        if (string.IsNullOrEmpty(localizationDataKey) || GamePlayData.Instance == null)
        {
            return this.gameObject.GetComponent<Text>().text;
        }

        if (GamePlayData.Instance.tableData.localizationDic.TryGetValue(localizationDataKey, out LocalizationInfo info))
        {
            return info.summary;
        }

        return string.Empty;
    }
}
