using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{
    public class Interval : UIECDecorator
    {


        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public RandomType rnd = RandomType.Fixed;

        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [ShowIf("@rnd == RandomType.Fixed")]
        public float interval = 0.5f;

        [HorizontalGroup("Settings/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [ShowIf("@rnd == RandomType.Random")]
        public float min = 0.5f, max = 1.0f;


        public override Sequence Do(Sequence seq, GameObject target)
        {
            if (rnd == RandomType.Random)
            {
                return seq.AppendInterval(Random.Range(min, max));
            }
            else
            {
                return seq.AppendInterval(interval);
            }
        }
    }

}
