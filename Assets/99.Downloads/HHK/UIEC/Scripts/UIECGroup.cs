using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

namespace HHK.UIEC
{

    [AddComponentMenu("HHK/UIEC/UIEC Group")]
    public class UIECGroup : UIECAnimatorBase
    {

        public enum Stat
        {
            Fixed, // Can not be moved by Show and Hide action
            Show,
            Hide
        }

        [BoxGroup("Group", false), GUIColor(0.8f, 0.8f, 0.8f, 0.8f)]
        [OnValueChanged("TrimGroupID")]
        public string groupID = "Default";

        void TrimGroupID()
        {
            groupID = groupID.Trim();
        }

        [BoxGroup("Basic", false), GUIColor(0.8f, 0.8f, 0.8f, 0.8f), OnValueChanged("OnStatChanged")]
        public Stat stat = Stat.Fixed;


        //[BoxGroup("Actions", false)]
        [ShowIf("@stat!=Stat.Fixed")]
        [TypeFilter("GetFilteredTypeList")]
        [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = false), GUIColor(0.6f, 0.6f, 0.8f, 0.8f)]

        public List<UIECDescriptor> shows = new List<UIECDescriptor>();

        //[BoxGroup("Actions", false)]
        [ShowIf("@stat!=Stat.Fixed")]
        [TypeFilter("GetFilteredTypeList")]
        [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = false), GUIColor(0.9f, 0.7f, 0.7f, 0.8f)]
        public List<UIECDescriptor> hides = new List<UIECDescriptor>();

        protected override IEnumerable<System.Type> GetFilteredTypeList()
        {
            var q = new List<System.Type>(typeof(UIECDescriptor).Assembly.GetTypes());
            q.RemoveAll(x => x.IsAbstract || x.IsGenericTypeDefinition || !typeof(UIECDescriptor).IsAssignableFrom(x));
            q.RemoveAll(x => typeof(Loop).IsAssignableFrom(x)); // No loops
            return q;
        }


        void OnStatChanged()
        {
            if (Application.isPlaying)
            {
                if (stat == Stat.Show)
                {
                    Show();
                }
                if (stat == Stat.Hide)
                {
                    Hide();
                }

            }
        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            StopAnimation();
        }


        void Awake()
        {

        }

        public void Hide()
        {
            if (stat != Stat.Fixed)
            {
                stat = Stat.Hide;
                DoAnimation(hides);
            }

        }
        public void HideImmediately()
        {
            Hide();
        }

        public void Show()
        {
            if (stat != Stat.Fixed)
            {
                stat = Stat.Show;
                DoAnimation(shows);
            }
        }
        public void ShowImmediately()
        {
            Show();
        }

        public void Switch()
        {
            if (stat != Stat.Fixed)
            {
                if (stat == Stat.Show)
                    Hide();
                else if (stat == Stat.Hide)
                    Show();

            }
        }

        public void SwitchImmediately()
        {
            Switch();
        }
    }
}