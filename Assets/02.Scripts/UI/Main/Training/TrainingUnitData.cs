using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TrainingUnitData : ScriptableObject
{
    [SerializeField]
    private TrainingInfo[] _trainingData;
    public TrainingInfo[] trainingData
    {
        get
        {
            return _trainingData;
        }
        set
        {
            _trainingData = value;
        }
    }
}
