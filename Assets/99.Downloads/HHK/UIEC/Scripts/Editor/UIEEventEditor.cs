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
    [CustomEditor(typeof(UIECEvent))]
    public class UIEEventEditor : OdinEditor
    {

        public override void OnInspectorGUI()
        {
            var obj = this.target as UIECEvent;

            SirenixEditorGUI.Title(obj.name + ": " + obj.GetInspectorName(), "Event", TextAlignment.Center, false);
            SirenixEditorGUI.BeginHorizontalToolbar();


            SirenixEditorGUI.EndHorizontalToolbar();


            base.OnInspectorGUI();

        }




    }
}