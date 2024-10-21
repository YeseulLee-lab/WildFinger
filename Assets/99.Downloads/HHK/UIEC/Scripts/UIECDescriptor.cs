using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System;


/// <summary>
/// UIECAnimation defines the animations
/// </summary>
namespace HHK.UIEC
{
    public abstract class UIECDescriptor
    {
        /// <summary>
        /// If allow the cell run in preview mode
        /// </summary>
        public virtual bool IsAllowPreview()
        {
            return true;
        }

        protected bool NotAllowInPreview(string e = "")
        {
            UnityEngine.Debug.LogWarning($"[UIEC] Warning: {this.GetType()} do NOT support preview mode.\n{e}");
            return false;
        }


    }


    public enum RunType
    {
        Follow,
        Join
    }

    public enum RandomType
    {
        Fixed,
        Random
    }

    public enum MoveType
    {
        Actual,
        Relative,
    }
}