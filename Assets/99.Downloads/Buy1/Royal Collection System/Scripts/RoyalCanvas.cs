using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoyalCanvas : MonoBehaviour
{
    [SerializeField]
    private Button addCoinBtn;
    [SerializeField]
    private Button addDiamondBtn;

    private void Start()
    {
        addCoinBtn.onClick.AddListener(() => RoyalCollectingController._instance.CollectItem(7));
        addDiamondBtn.onClick.AddListener(() => RoyalCollectingController._instance.CollectItem(25));
    }
}
