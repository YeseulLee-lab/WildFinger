using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public partial class TableData : MonoBehaviour
{
    private Dictionary<string, StringInfo> _stringDic = new Dictionary<string, StringInfo>();
    public Dictionary<string, StringInfo> stringDic
    {
        get { return _stringDic; }
        set { return; }
    }

    void StringInit()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("StringTable");

        for (int i = 0; i < data.Count; i++)
        {
            StringInfo stringInfo = new StringInfo();

            string id = data[i]["Id"].ToString();

            stringInfo.sentence = data[i]["Sentence"].ToString();


            if (!_tutorialInfoDic.ContainsKey(id))
            {
                _stringDic.Add(id, stringInfo);
            }
            else
            {
                return;
            }
        }
    }
}

public class StringInfo
{
    public string sentence;
}