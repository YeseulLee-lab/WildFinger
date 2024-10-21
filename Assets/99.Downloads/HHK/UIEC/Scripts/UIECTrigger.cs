using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;
using System;

namespace HHK.UIEC
{

    [AddComponentMenu("HHK/UIEC/UIEC Trigger")]
    public class UIECTrigger : SerializedMonoBehaviour
    {
        [GUIColor(0.8f, 0.8f, 0.8f, 0.8f)]
        [ListDrawerSettings(ShowIndexLabels = false)]

        public HashSet<string> triggers = new HashSet<string>();

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="t"></param>
        public void SetTrigger(string t)
        {
            triggers.Add(t);
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool HasTrigger(string t)
        {
            return triggers.Contains(t);
        }

        /// <summary>
        /// Remove the trigger
        /// </summary>
        /// <param name="t"></param>
        public void UseTrigger(string t)
        {
            triggers.Remove(t);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearAll()
        {
            triggers.Clear();
        }

    }
}