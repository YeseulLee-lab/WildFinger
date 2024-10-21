using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GNBCanvas : MonoBehaviour
{
    [SerializeField]
    private Button homeButton;

    private void Start()
    {
        GetComponent<CanvasScaler>().referenceResolution = new Vector2(Screen.width, Screen.height);

        homeButton.onClick.AddListener(() =>
        {
            //MainUIManager.Instance.tutorialCanvas.StartTutorial(Define.TutorialName.ShowPlayButton);
        });
    }
}