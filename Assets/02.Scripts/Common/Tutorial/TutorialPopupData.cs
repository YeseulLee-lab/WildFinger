using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TutorialPopupData : ScriptableObject
{
    [SerializeField]
    private InGameTutorialPopupInfo[] _tutorialPopupData;
    public InGameTutorialPopupInfo[] tutorialPopupData => _tutorialPopupData;
}
