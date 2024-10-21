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

    public abstract class UIECEventBase : SerializedMonoBehaviour
    {
        /// <summary>
        /// Do Animation
        /// </summary>
        /// <param name="animations"></param>
        protected void DoEvent(UnityEvent e)
        {
            e.Invoke();
        }
    }
}