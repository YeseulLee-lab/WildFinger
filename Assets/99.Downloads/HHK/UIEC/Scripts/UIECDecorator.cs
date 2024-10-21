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
    public abstract class UIECDecorator : UIECDescriptor
    {
        public abstract Sequence Do(Sequence seq, GameObject target);
    }
}