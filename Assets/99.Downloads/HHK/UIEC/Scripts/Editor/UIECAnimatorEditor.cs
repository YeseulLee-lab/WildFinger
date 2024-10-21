using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.DOTweenEditor;
using DG.Tweening;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace HHK.UIEC
{
    [CustomEditor(typeof(UIECAnimator))]
    public class UIECAnimatorEditor : OdinEditor
    {

        UIECAnimator obj;
        public override void OnInspectorGUI()
        {
            obj = this.target as UIECAnimator;

            if (!Application.isPlaying)
            {
                // Preview functions
                SirenixEditorGUI.Title(obj.name + ": " + obj.GetInspectorName(), "Animator", TextAlignment.Center, false);

                SirenixEditorGUI.BeginHorizontalToolbar();
                if (GUILayout.Button(Resources.Load<Texture>("UIEC/Icons/Play"), GUILayout.Width(32), GUILayout.Height(32)))
                {
                    DG.DOTweenEditor.DOTweenEditorPreview.Stop(true);
                    obj.DoAnimation(obj.animations);
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
                SirenixEditorGUI.Title(obj.name + ": " + obj.GetInspectorName(), "Animator", TextAlignment.Center, false);
                SirenixEditorGUI.BeginHorizontalToolbar();
                if (GUILayout.Button("No preview function in Play Mode.", GUILayout.Height(32)))
                {

                }

                SirenixEditorGUI.EndHorizontalToolbar();
            }



            base.OnInspectorGUI();

            // Ending
            // SirenixEditorGUI.Title("", "", TextAlignment.Center, true);



        }

    }
}