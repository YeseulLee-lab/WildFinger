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

    public class UIECOverallWindow : OdinEditorWindow
    {


        [InlineEditor]
        [GUIColor(0.8f, 0.7f, 0.8f, 0.8f)]
        [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, HideAddButton = true, HideRemoveButton = true, ListElementLabelName = "groupID")]
        [Searchable]
        public List<UIECGroup> groups = new List<UIECGroup>();



        [PropertySpace]
        [InlineEditor]
        [GUIColor(0.5f, 0.7f, 0.7f, 0.8f)]
        [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, HideAddButton = true, HideRemoveButton = true, ListElementLabelName = "@channel")]
        [Searchable]
        public List<UIECRunner> runners = new List<UIECRunner>();


        [PropertySpace]
        [InlineEditor]
        [GUIColor(0.8f, 0.8f, 0.5f, 0.8f)]
        [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, HideAddButton = true, HideRemoveButton = true)]
        [Searchable]
        public List<UIECTrigger> triggers = new List<UIECTrigger>();

        [PropertySpace]
        [InlineEditor]
        [GUIColor(0.6f, 0.6f, 0.5f, 0.8f)]
        [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, HideAddButton = true, HideRemoveButton = true, ListElementLabelName = "@(useEvents == UIECUseEvents.Custom)?channel:useEvents.ToString()")]
        [Searchable]
        public List<UIECAnimator> animators = new List<UIECAnimator>();

        [PropertySpace]
        [InlineEditor]
        [GUIColor(0.6f, 0.7f, 0.8f, 0.8f)]
        [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, HideAddButton = true, HideRemoveButton = true, ListElementLabelName = "@(useEvents == UIECUseEvents.Custom)?channel:useEvents.ToString()")]
        [Searchable]
        public List<UIECEvent> events = new List<UIECEvent>();



        // [PropertySpace]
        // [InlineEditor]
        // [GUIColor(0.8f, 0.8f, 0.8f, 0.8f)]
        // [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, HideAddButton = true, HideRemoveButton = true, ListElementLabelName = "@channel")]
        // [Searchable]
        // public List<UIECPreset> presets = new List<UIECPreset>();

        [MenuItem("Tools/HHK/UIEC Overall Window")]
        static void OpenWindow()
        {
            GetWindow<UIECOverallWindow>().Show();
        }



        void Reflash()
        {
#if UNITY_2020_1_OR_NEWER
            groups = new List<UIECGroup>(FindObjectsOfType<UIECGroup>(true));
            runners = new List<UIECRunner>(FindObjectsOfType<UIECRunner>(true));
            triggers = new List<UIECTrigger>(FindObjectsOfType<UIECTrigger>(true));
            animators = new List<UIECAnimator>(FindObjectsOfType<UIECAnimator>(true));
            events = new List<UIECEvent>(FindObjectsOfType<UIECEvent>(true));
            // presets = new List<UIECPreset>(FindObjectsOfType<UIECPreset>(true));
#else
            groups = new List<UIECGroup>(FindObjectsOfType<UIECGroup>());
            runners = new List<UIECRunner>(FindObjectsOfType<UIECRunner>());
            triggers = new List<UIECTrigger>(FindObjectsOfType<UIECTrigger>());
            animators = new List<UIECAnimator>(FindObjectsOfType<UIECAnimator>());
            events = new List<UIECEvent>(FindObjectsOfType<UIECEvent>());
#endif
        }


        protected override void OnGUI()
        {
            GUILayout.Label(Resources.Load<Texture>("UIEC/Icons/UIEC"), GUILayout.Height(32));
            if (GUILayout.Button("Online Documentations"))
            {
                Application.OpenURL("https://perfect-sauce-33c.notion.site/UI-Enhanced-Components-b9676cbe0ea542209ca335325f4884f0");
            }


            Reflash();
            base.OnImGUI();

        }

    }
}