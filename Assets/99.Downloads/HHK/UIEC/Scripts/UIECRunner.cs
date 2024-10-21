using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

namespace HHK.UIEC
{
    /// <summary>
    /// Auto run the animations 
    /// </summary>

    [AddComponentMenu("HHK/UIEC/UIEC Runner")]
    // [TypeInfoBox("You may set the Animations' RunType to make the animations run as a sequence or parallel.")]
    public class UIECRunner : UIECAnimatorBase
    {


        /// <summary>
        /// 
        /// </summary>
        [BoxGroup("Basic", false), GUIColor(0.8f, 0.8f, 0.8f, 0.8f)]
        public bool runAtStart = false;

        [PropertySpace]

        [GUIColor(0.8f, 0.8f, 0.8f, 0.8f)]
        [OnValueChanged("TrimChannel")]
        public string channel = "Default";



        void TrimChannel()
        {
            channel = channel.Trim();
        }

        [PropertySpace]

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UIECDescriptor"></typeparam>
        /// <returns></returns>
        [TypeFilter("GetFilteredTypeList")]
        [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = false)]
        public List<UIECDescriptor> animations = new List<UIECDescriptor>();


        private void OnEnable()
        {

        }

        void Start()
        {
            RunAtStart();
        }

        void RunAtStart()
        {
            if (runAtStart)
            {
                DoAnimation(animations);
            }
        }

        private void OnDisable()
        {
            StopImmediately();
        }

        //[Button("Run (Play Mode Only)"), HorizontalGroup, GUIColor(0.2f, 0.8f, 0.2f)]
        public void RunImmediately()
        {
            DoAnimation(animations);
        }

        //[Button("Stop (Play Mode Only)"), HorizontalGroup, GUIColor(0.8f, 0.2f, 0.2f)]
        public void StopImmediately()
        {
            StopAnimation();
        }


    }
}