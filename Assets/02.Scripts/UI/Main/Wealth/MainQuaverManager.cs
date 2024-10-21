using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using Cysharp.Threading.Tasks;
using HHK.UIEC;
using DG.Tweening;
using UnityEngine.Events;
using System;

public class MainQuaverManager : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Text _quaverText;
    [SerializeField]
    private GameObject _quaverImg;

    #region Unity Life Cycle
    #endregion

    public void SetQuaverData()
    {
        _quaverText.text = GamePlayData.Instance.remainQuaverCnt.ToString();
        
        //Collect Effect
        if (GamePlayData.Instance.getQuaverCnt > 0)
        {
            GetComponent<CollectingCoinManager>().RewardWealth(GamePlayData.Instance.getQuaverCnt, _quaverImg.GetComponent<RectTransform>(), _quaverText, () =>
            {

            });
            GamePlayData.Instance.remainQuaverCnt += GamePlayData.Instance.getQuaverCnt;
            GamePlayData.Instance.getQuaverCnt = 0;

            if (MainUIManager.Instance.tutorialCanvas.IsTutorialDone(Define.MainTutorialType.CollectionPage2)
                || MainUIManager.Instance.tutorialCanvas.IsTutorialDone(Define.MainTutorialType.CollectionPage3)
                || MainUIManager.Instance.tutorialCanvas.IsTutorialDone(Define.MainTutorialType.CollectionPage4))
                return;
            MainUIManager.Instance.collectionCanvas.StartCollectionTutorial();
        }
    }
}
