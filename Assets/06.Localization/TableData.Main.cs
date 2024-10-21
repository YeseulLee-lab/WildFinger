using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TableData : MonoBehaviour
{
    private Dictionary<string, List<TutorialInfo>> _tutorialInfoDic = new Dictionary<string, List<TutorialInfo>>();
    public Dictionary<string, List<TutorialInfo>> tutorialInfoDic
    {
        get { return _tutorialInfoDic; }
        set { return;}
    }

    void TutorialInfoInit()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("MainTutorialTable");

        for (int i = 0; i < data.Count; i++)
        {
            TutorialInfo tutorialInfo = new TutorialInfo();

            string tutorial_name = data[i]["tutorial_name"].ToString();

            tutorialInfo.name_id = data[i]["name_id"].ToString();
            tutorialInfo.sentence_id = data[i]["sentence_id"].ToString();
            tutorialInfo.tutorial_type = (Define.TutorialType)Enum.Parse(typeof(Define.TutorialType), (data[i]["tutorial_type"].ToString()));


            if (!_tutorialInfoDic.ContainsKey(tutorial_name))
            {
                _tutorialInfoDic.Add(tutorial_name, new List<TutorialInfo>());
                _tutorialInfoDic[tutorial_name].Add(tutorialInfo);
            }
            else
            {
                _tutorialInfoDic[tutorial_name].Add(tutorialInfo);
            }
        }
    }
}
