using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InfiniteScroll))]
public class LevelSelectScrollContent : UIBehaviour, IInfiniteScrollSetup
{
    [SerializeField, Range(1, 999)]
    private int max;

    private List<LevelInfo> _levels = new List<LevelInfo>();

    [SerializeField]
    private float[] xPos; //0: 가운데, 1: 오른쪽, 2: 왼쪽

    private TownInfo _townInfo;

    private void OnDisable()
    {   
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0f, 0f);
    }

    public void SetData(TownInfo townInfo)
    {
        _townInfo = townInfo;
        _levels.Clear();

        for (int i = 0; i < townInfo.levelAmount + 1; i++) //1을 더해주는 이유는 UI 상 예쁘게 보여주기 위해서
        {
            LevelInfo level = new LevelInfo();

            if (townInfo.townType != Define.TownList.ToyTown)
            {
                level.level = i + 1 + GamePlayData.Instance.GetStackLevels(townInfo);
            }
            else
            {
                //첫번째 타운이면 1부터 표시
                level.level = i + 1;
            }
            
            level.completeQuaverCnt = PlayerPrefs.GetInt(EncryptedKey.score + (i + 1).ToString());
            _levels.Add(level);
        }
        max = townInfo.levelAmount + 1;

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
        delta.x = infiniteScroll.itemScaleX;
        delta.y = infiniteScroll.itemScale * max;
        rectTransform.sizeDelta = delta;

        //현재 랜드가 아니면(완료) => 맨 위 포커스
        if (_townInfo.townType != GamePlayData.Instance.maxTown)
        {
            rectTransform.anchoredPosition = new Vector2(0f, 0f);
        }
        else
        {
            //townunit에서 선택했는데 현재 랜드이면 현재 스테이지 포커스
            if (GamePlayData.Instance.maxTown != Define.TownList.ToyTown)
            {
                rectTransform.DOLocalMoveY(rectTransform.sizeDelta.y - (GamePlayData.Instance.maxStage - GamePlayData.Instance.GetStackLevels(_townInfo)) * infiniteScroll.itemScale - (Screen.height * 0.5f), 0.1f);
            }
            else
            {
                rectTransform.DOLocalMoveY(rectTransform.sizeDelta.y - GamePlayData.Instance.maxStage * infiniteScroll.itemScale - (Screen.height * 0.5f), 0.1f);
            }
        }   
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
            obj.GetComponent<RectTransform>().anchoredPosition 
                = new Vector2(xPos[itemCount % 4], obj.GetComponent<RectTransform>().anchoredPosition.y);
            var item = obj.GetComponentInChildren<LevelSelectUnit>();
            item.UpdateItem(max, itemCount, _levels[max - itemCount - 1], _townInfo);
        }
    }
}
