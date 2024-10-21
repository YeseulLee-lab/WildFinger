using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class MainUIManager : MonoBehaviour
{
    public static MainUIManager Instance { get; private set; } = null;

    [Header("---------------- Canvas ----------------")]
    public MainCanvas mainCanvas;
    public ProfileCanvas profileCanvas;
    public FriendCanvas friendCanvas;
    public SettingCanvas settingCanvas;
    public SelectLevelCanvas SelectLevelCanvas;
    public PlayCurLevelPanel playCurLevelPanel;
    public TownCanvas townCanvas;
    public TrainingCanvas trainingCanvas;
    public CollectionCanvas collectionCanvas;
    public TutorialCanvas tutorialCanvas;
    public GameNoHeart moreHeartPopup;

    [Header("---------------- Touch Effect ----------------")]
    [SerializeField]
    private ParticleSystem touchEffect;
    [SerializeField]
    private RawImage touchEffectRawImage;

    #region Unity Life Cycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    #endregion
}
