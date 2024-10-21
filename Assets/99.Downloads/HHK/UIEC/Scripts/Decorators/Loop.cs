using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{

    // [InfoBox("It overrides the looping setting of the animation sequence. No matter this cell in which index of the animation list.")]
    public class Loop : UIECDecorator
    {
        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f)]
        public int loops = -1;

        [ShowIf("@loops!=0 && loops!=1")]
        [BoxGroup("Settings"), GUIColor(0.6f, 0.6f, 0.8f)]
        public LoopType loopType = LoopType.Restart;

        public override Sequence Do(Sequence seq, GameObject target)
        {
            return seq.SetLoops(loops, loopType);
        }
    }
}