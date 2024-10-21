using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class NoteGeneratorData : ScriptableObject
{
    [SerializeField, OnValueChanged(nameof(UpdateGimmickKeys))]
    private NoteGeneratorUnitInfo[] _unitInfo;
    public NoteGeneratorUnitInfo[] unitInfo => _unitInfo;

    private void UpdateGimmickKeys()
    {
        for (int i = 0; i < _unitInfo.Length; i++)
        {
            _unitInfo[i].gimmickKey = i;
        }
    }
}
