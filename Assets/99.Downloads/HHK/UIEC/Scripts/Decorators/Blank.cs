using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{
    [TypeInfoBox("This is an empty cell, nothing will happen.")]
    public class Blank : UIECDecorator
    {
        public override Sequence Do(Sequence seq, GameObject target)
        {
            return seq;
        }
    }
}
