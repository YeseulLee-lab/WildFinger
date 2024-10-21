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
    [CustomEditor(typeof(UIECRunner))]
    public class UIECRunnerEditor : OdinEditor
    {

        public override void OnInspectorGUI()
        {
            var obj = this.target as UIECRunner;

            if (!Application.isPlaying)
            {
                // Preview functions

                SirenixEditorGUI.Title(obj.name + ": " + obj.channel, "Runner", TextAlignment.Center, false);

                SirenixEditorGUI.BeginHorizontalToolbar();

                if (GUILayout.Button(Resources.Load<Texture>("UIEC/Icons/Play"), GUILayout.Width(32), GUILayout.Height(32)))
                {
                    DG.DOTweenEditor.DOTweenEditorPreview.Stop(true);
                    obj.RunImmediately();
                    obj.seq.OnComplete(() => { obj.seq.Rewind(); DG.DOTweenEditor.DOTweenEditorPreview.Stop(true); });
                    DG.DOTweenEditor.DOTweenEditorPreview.PrepareTweenForPreview(obj.seq, false);
                    DG.DOTweenEditor.DOTweenEditorPreview.Start(obj.DoProgress);

                }
                if (GUILayout.Button(Resources.Load<Texture>("UIEC/Icons/Stop"), GUILayout.Width(32), GUILayout.Height(32)))
                {
                    DG.DOTweenEditor.DOTweenEditorPreview.Stop(true);
                }

                SirenixEditorGUI.EndHorizontalToolbar();
            }
            else
            {

                SirenixEditorGUI.Title(obj.name + ": " + obj.channel, "Runner", TextAlignment.Center, false);
                SirenixEditorGUI.BeginHorizontalToolbar();
                if (GUILayout.Button("No preview function in Play Mode.", GUILayout.Height(32)))
                {

                }

                SirenixEditorGUI.EndHorizontalToolbar();
            }


            base.OnInspectorGUI();

        }




    }
}