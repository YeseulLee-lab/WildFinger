using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingUnit : MonoBehaviour
{
    [SerializeField]
    private TrainingSubUnit[] _subUnits;
    public TrainingSubUnit[] subUnits => _subUnits;

    public void SetData(int itemCount, List<InGameTutorialPopupInfo> trainingInfos)
    {
        if (trainingInfos.Count % subUnits.Length == 0)
        {
            for (int i = 0; i < subUnits.Length; i++)
            {
                subUnits[i].gameObject.SetActive(true);
                subUnits[i].UpdateItem(itemCount * subUnits.Length + i, trainingInfos[i]);
            }
        }
        else
        {
            for (int i = 0; i < trainingInfos.Count % subUnits.Length; i++)
            {
                subUnits[i].UpdateItem(itemCount * subUnits.Length + i, trainingInfos[i]);
            }

            for (int i = trainingInfos.Count % _subUnits.Length; i < subUnits.Length; i++)
            {
                subUnits[i].gameObject.SetActive(false);
            }
        }
    }
}
