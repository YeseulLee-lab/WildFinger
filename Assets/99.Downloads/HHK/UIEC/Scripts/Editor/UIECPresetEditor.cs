using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.DOTweenEditor;
using DG.Tweening;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;


namespace HHK.UIEC
{
    [CustomEditor(typeof(UIECPreset))]
    public class UIECPresetEditor : OdinEditor
    {

        public override void OnInspectorGUI()
        {
            var obj = this.target as UIECPreset;

            GUILayout.Label(Resources.Load<Texture>("UIEC/Icons/UIEC"), GUILayout.Height(32));

            SirenixEditorGUI.Title(obj.name, "Preset", TextAlignment.Center, false);

            SirenixEditorGUI.BeginHorizontalToolbar();

            SirenixEditorGUI.EndHorizontalToolbar();


            base.OnInspectorGUI();

        }




    }
}