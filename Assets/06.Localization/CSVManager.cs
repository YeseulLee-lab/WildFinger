using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using UnityEngine;
using System.Text.RegularExpressions;

public class CSVManager : MonoBehaviour
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    /// <summary>
    /// Writes localization data to the CSV file. If the key already exists, it add the row.
    /// </summary>
    /// <param name="data">Localization data to write or update. The type is assumed to be None.</param>
    public static void WriteLocalizationData(TextAsset textAsset, LocalizationData data)
    {
        var existingData = ReadCSVList(textAsset);
        bool keyExists = false;

        for (int i = 0; i < existingData.Count; i++)
        {
            var values = existingData[i].Split(',');
            if (values[0] == data.key)
            {
                // Update the existing line
                existingData[i] = CreateCSVLine(data);
                keyExists = true;
                break;
            }
        }

        if (!keyExists)
        {
            // Add new line if key does not exist
            existingData.Add(CreateCSVLine(data));
        }

        // Append data to the CSV file
        string filePath = Path.Combine(Application.dataPath, "Resources", "LocalizationTable.csv");
        using (StreamWriter writer = new StreamWriter(filePath, true)) // 'true' to append
        {
            if (!File.Exists(filePath))
            {
                // Write header if the file does not exist
                string[] header = new string[2 + Enum.GetValues(typeof(Define.LanguageType)).Length];
                header[0] = "Key";
                header[1] = "TutorialUIType";
                for (int i = 2; i < header.Length; i++)
                {
                    header[i] = ((Define.LanguageType)(i - 2)).ToString();
                }

                writer.WriteLine(string.Join(",", header));
            }

            // Write only the new/updated line to the file
            writer.WriteLine(existingData[existingData.Count - 1]);
        }

        Debug.Log($"{data.key} - CSV file updated at {filePath}");
    }


    private static string CreateCSVLine(LocalizationData data)
    {
        string[] contents = new string[2 + Enum.GetValues(typeof(Define.LanguageType)).Length];
        contents[0] = data.key;
        contents[1] = data.tutorialUIType.ToString();
        contents[2] = EscapeSpecialCharacters(data.languageType.korean);
        contents[3] = EscapeSpecialCharacters(data.languageType.english);

        return string.Join(",", contents);
    }

    private static string EscapeSpecialCharacters(string field)
    {
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
        {
            field = field.Replace("\"", "\"\"");
            field = $"\"{field}\"";
        }
        return field;
    }

    public static List<string> ReadCSVList(TextAsset textAsset)
    {
        var lines = new List<string>();
        if (textAsset == null)
        {
            //Debug.LogError("TextAsset is not assigned.");
            return lines;
        }

        using (StringReader reader = new StringReader(textAsset.text))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }
        }

        return lines;
    }

    public List<Dictionary<string, object>> ReadLocalizationCSV(TextAsset textAsset)
    {
        var list = new List<Dictionary<string, object>>();

        if (textAsset == null)
        {
            DebugX.LogError("TextAsset is not assigned.");
            return list;
        }

        var lines = textAsset.text.Split(new[] { '\n' }, StringSplitOptions.None);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }

    public static List<Dictionary<string, object>> ParseCSV(string csvData)
    {
        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
        string[] lines = csvData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length <= 1) return data;

        // First line is the header
        string[] headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            Dictionary<string, object> entry = new Dictionary<string, object>();
            for (int j = 0; j < headers.Length; j++)
            {
                entry[headers[j]] = values[j];
            }
            data.Add(entry);
        }

        return data;
    }
}