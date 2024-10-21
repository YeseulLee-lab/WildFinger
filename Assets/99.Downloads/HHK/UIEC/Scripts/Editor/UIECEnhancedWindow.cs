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

    public class UIECEnhancedWindow : OdinEditorWindow
    {

        void Remove(Object ob)
        {
            if (Selection.activeGameObject != null)
            {
                DestroyImmediate(ob);
            }
        }

        [InlineEditor]
        [GUIColor(0.8f, 0.7f, 0.8f, 0.8f)]
        [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, CustomAddFunction = "AddGroup", CustomRemoveElementFunction = "Remove", ListElementLabelName = "groupID")]
        [Searchable]
        public List<UIECGroup> groups = new List<UIECGroup>();

        void AddGroup()
        {
            if (Selection.activeGameObject != null)
            {
                Selection.activeGameObject.AddComponent<UIECGroup>();
            }
        }

        [PropertySpace]
        [InlineEditor]
        [GUIColor(0.5f, 0.7f, 0.7f, 0.8f)]
        [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, CustomAddFunction = "AddRunner", CustomRemoveElementFunction = "Remove", ListElementLabelName = "@channel")]
        [Searchable]
        public List<UIECRunner> runners = new List<UIECRunner>();

        void AddRunner()
        {
            if (Selection.activeGameObject != null)
            {
                Selection.activeGameObject.AddComponent<UIECRunner>();
            }
        }

        [PropertySpace]
        [InlineEditor]
        [GUIColor(0.8f, 0.8f, 0.5f, 0.8f)]
        [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, CustomAddFunction = "AddTrigger", CustomRemoveElementFunction = "Remove")]
        [Searchable]
        public List<UIECTrigger> triggers = new List<UIECTrigger>();

        void AddTrigger()
        {
            if (Selection.activeGameObject != null)
            {
                Selection.activeGameObject.AddComponent<UIECTrigger>();
            }
        }

        [PropertySpace]
        [InlineEditor]
        [GUIColor(0.6f, 0.6f, 0.5f, 0.8f)]
        [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, CustomAddFunction = "AddAnimator", CustomRemoveElementFunction = "Remove", ListElementLabelName = "@(useEvents == UIECUseEvents.Custom)?channel:useEvents.ToString()")]
        [Searchable]
        public List<UIECAnimator> animators = new List<UIECAnimator>();

        void AddAnimator()
        {
            if (Selection.activeGameObject != null)
            {
                Selection.activeGameObject.AddComponent<UIECAnimator>();
            }
        }

        [PropertySpace]
        [InlineEditor]
        [GUIColor(0.6f, 0.7f, 0.8f, 0.8f)]
        [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, CustomAddFunction = "AddEvent", CustomRemoveElementFunction = "Remove", ListElementLabelName = "@(useEvents == UIECUseEvents.Custom)?channel:useEvents.ToString()")]
        [Searchable]
        public List<UIECEvent> events = new List<UIECEvent>();


        void AddEvent()
        {
            if (Selection.activeGameObject != null)
            {
                Selection.activeGameObject.AddComponent<UIECEvent>();
            }
        }


        // [PropertySpace]
        // [InlineEditor]
        // [GUIColor(0.8f, 0.8f, 0.8f, 0.8f)]
        // [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, CustomAddFunction = "AddPreset", CustomRemoveElementFunction = "Remove", ListElementLabelName = "channel")]
        // [Searchable]
        // public List<UIECPreset> presets = new List<UIECPreset>();
        // void AddPreset()
        // {
        //     if (Selection.activeGameObject != null)
        //     {
        //         Selection.activeGameObject.AddComponent<UIECPreset>();
        //     }
        // }




        [MenuItem("Tools/HHK/UIEC Enhanced Window")]
        static void OpenWindow()
        {
            GetWindow<UIECEnhancedWindow>().Show();
        }

        string objName = "";
        string objPath = "";


        void Reflash()
        {
            objName = Selection.activeGameObject.name;
            // get the Game Object Path
            objPath = Selection.activeGameObject.name;
            var parent = Selection.activeGameObject;
            while (parent.transform.parent != null)
            {
                objPath = parent.transform.parent.name + "/" + objPath;
                parent = parent.transform.parent.gameObject;
            }


            groups = new List<UIECGroup>(Selection.activeGameObject.GetComponents<UIECGroup>());
            runners = new List<UIECRunner>(Selection.activeGameObject.GetComponents<UIECRunner>());
            triggers = new List<UIECTrigger>(Selection.activeGameObject.GetComponents<UIECTrigger>());
            animators = new List<UIECAnimator>(Selection.activeGameObject.GetComponents<UIECAnimator>());
            events = new List<UIECEvent>(Selection.activeGameObject.GetComponents<UIECEvent>());
            // presets = new List<UIECPreset>(Selection.activeGameObject.GetComponents<UIECPreset>());
        }


        protected override void OnGUI()
        {

            GUILayout.Label(Resources.Load<Texture>("UIEC/Icons/UIEC"), GUILayout.Height(32));
            if (GUILayout.Button("Online Documentations"))
            {
                Application.OpenURL("https://perfect-sauce-33c.notion.site/UI-Enhanced-Components-b9676cbe0ea542209ca335325f4884f0");
            }

            SirenixEditorGUI.Title(objName, objPath, TextAlignment.Center, false);

            if (Selection.activeGameObject != null)
            {
                Reflash();
            }

            base.OnImGUI();
        }


    }

}