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
    [CustomEditor(typeof(UIECGroup))]
    public class UIECGroupEditor : OdinEditor
    {
        UIECGroup obj;
        public override void OnInspectorGUI()
        {
            obj = this.target as UIECGroup;

            if (!Application.isPlaying)
            {
                // Preview functions
                SirenixEditorGUI.Title(obj.name + ": " + obj.groupID, "Group", TextAlignment.Center, false);
                SirenixEditorGUI.BeginHorizontalToolbar(32);

                if (obj.stat != UIECGroup.Stat.Fixed)
                {


                    if (GUILayout.Button(Resources.Load<Texture>("UIEC/Icons/Show"), GUILayout.Width(32), GUILayout.Height(32)))
                    {
                        DG.DOTweenEditor.DOTweenEditorPreview.Stop(true);
                        obj.Show();
                        obj.seq.OnComplete(() => DG.DOTweenEditor.DOTweenEditorPreview.Stop(false)); // remian the stat
                        DG.DOTweenEditor.DOTweenEditorPreview.PrepareTweenForPreview(obj.seq, false);
                        DG.DOTweenEditor.DOTweenEditorPreview.Start(obj.DoProgress);
                    }

                    if (GUILayout.Button(Resources.Load<Texture>("UIEC/Icons/Hide"), GUILayout.Width(32), GUILayout.Height(32)))
                    {
                        DG.DOTweenEditor.DOTweenEditorPreview.Stop(true);
                        obj.Hide();
                        obj.seq.OnComplete(() => DG.DOTweenEditor.DOTweenEditorPreview.Stop(false));// remian the stat
                        DG.DOTweenEditor.DOTweenEditorPreview.PrepareTweenForPreview(obj.seq, false);
                        DG.DOTweenEditor.DOTweenEditorPreview.Start(obj.DoProgress);
                    }

                    if (GUILayout.Button(Resources.Load<Texture>("UIEC/Icons/Stop"), GUILayout.Width(32), GUILayout.Height(32)))
                    {

                        DG.DOTweenEditor.DOTweenEditorPreview.Stop(true);
                    }


                    if (obj.shows.Count == 0 && obj.hides.Count == 0)
                    {
                        AddDefaultAnimations();
                    }

                }

                SirenixEditorGUI.EndHorizontalToolbar();
            }
            else
            {
                SirenixEditorGUI.Title(obj.name + ": " + obj.groupID, "Group", TextAlignment.Center, false);
                SirenixEditorGUI.BeginHorizontalToolbar();
                if (GUILayout.Button("No preview function in Play Mode.", GUILayout.Height(32)))
                {

                }

                SirenixEditorGUI.EndHorizontalToolbar();
            }


            base.OnInspectorGUI();

        }

        void AddDefaultAnimations()
        {
            if (obj == null)
                return;


            if (GUILayout.Button("Add Default Animations", GUILayout.Height(32)))
            {

                // Add shows
                obj.shows.Clear();
                obj.shows.Add(new MoveTo() { to = obj.transform.position });
                obj.shows.Add(new ScaleToLocal() { to = obj.transform.lossyScale, runType = RunType.Join });

                // Add hides
                obj.hides.Clear();
                obj.hides.Add(new MoveTo() { to = obj.transform.position });
                obj.hides.Add(new ScaleToLocal() { to = obj.transform.lossyScale, runType = RunType.Join });


            }

        }


    }


}