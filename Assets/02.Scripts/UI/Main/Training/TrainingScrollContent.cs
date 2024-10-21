using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InfiniteScroll))]
public class TrainingScrollContent : UIBehaviour, IInfiniteScrollSetup
{
    [SerializeField, Range(1, 999)]
    private int max;
    [SerializeField]
    private int subUnitCnt;
    [SerializeField]
    private TutorialPopupData _tutorialData;
    public TutorialPopupData tutorialData => _tutorialData;

    public TrainingSubUnit[] subUnits;

    private List<InGameTutorialPopupInfo> trainingInfos;

    protected override void Start()
    {
        base.Start();
        SetData();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    public void SetData()
    {
        trainingInfos = null;
        trainingInfos = new List<InGameTutorialPopupInfo>();

        for (int i = 0; i < tutorialData.tutorialPopupData.Length; i++)
        {
            if (tutorialData.tutorialPopupData[i].type == Define.InGameTutorialType.ItemHPPotion
                || tutorialData.tutorialPopupData[i].type == Define.InGameTutorialType.ItemIncreaseHP
                || tutorialData.tutorialPopupData[i].type == Define.InGameTutorialType.ItemShield)
            {
                DebugX.Log(tutorialData.tutorialPopupData[i].type.ToString());
            }
            else
            {
                trainingInfos.Add(tutorialData.tutorialPopupData[i]);
            }
        }
        max = trainingInfos.Count / subUnitCnt;
        if (trainingInfos.Count % subUnitCnt != 0)
        {
            max += 1;
        }

        InfiniteScroll infiniteScroll = GetComponent<InfiniteScroll>();
        infiniteScroll.Init();
    }

    public void OnPostSetupItems()
    {
        var infiniteScroll = GetComponent<InfiniteScroll>();
        infiniteScroll.onUpdateItem.AddListener(OnUpdateItem);
        GetComponentInParent<ScrollRect>().movementType = ScrollRect.MovementType.Elastic;

        var rectTransform = GetComponent<RectTransform>();
        var delta = rectTransform.sizeDelta;
        delta.y = infiniteScroll.itemScale * max;
        rectTransform.sizeDelta = delta;

        subUnits = new TrainingSubUnit[subUnitCnt * transform.childCount];
        //처음에 열때 첫번째 선택
        for (int i = 0; i < transform.childCount; i++)
        {
            for (int j = 0; j < transform.GetChild(i).GetChild(0).GetChild(0).childCount; j++)
            {
                subUnits[i * subUnitCnt + j] = transform.GetChild(i).GetChild(0).GetChild(0).GetChild(j).GetComponent<TrainingSubUnit>();
            }
        }
        MainUIManager.Instance.trainingCanvas.SetDescData(Define.InGameTutorialType.Rock, 0, tutorialData.tutorialPopupData[0].trainingDesc);
        subUnits[0].SelectUnit();
    }

    public void OnUpdateItem(int itemCount, GameObject obj)
    {
        if (itemCount < 0 || itemCount >= max)
        {
            obj.SetActive(false);
        }
        else
        {
            obj.SetActive(true);
            var item = obj.GetComponentInChildren<TrainingUnit>();
            List<InGameTutorialPopupInfo> infos = new List<InGameTutorialPopupInfo>();
            //마지막줄 데이터 추가
            if (itemCount == max - 1)
            {
                if (trainingInfos.Count % subUnitCnt == 0)
                {
                    for (int i = 0; i < subUnitCnt; i++)
                    {
                        infos.Add(trainingInfos[itemCount * subUnitCnt + i]);
                    }
                }
                else
                {
                    for (int i = 0; i < trainingInfos.Count % subUnitCnt; i++)
                    {
                        infos.Add(trainingInfos[itemCount * subUnitCnt + i]);
                    }
                }
                
            }
            else
            {
                for (int i = 0; i < subUnitCnt; i++)
                {
                    infos.Add(trainingInfos[itemCount * subUnitCnt + i]);
                }
            }
            item.SetData(itemCount, infos);
        }
    }
}
