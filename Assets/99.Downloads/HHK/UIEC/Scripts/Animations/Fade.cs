using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{
    public class Fade : UIECAnimation
    {

        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [Range(0f, 1f)]
        public float value = 1.0f;

        [BoxGroup("Duration", false)]
        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public float duration = 0.5f;

        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public Ease ease = Ease.OutCubic;

        protected override Sequence DoAnimation(GameObject target)
        {

            Graphic g = target.GetComponent<Graphic>();
            if (g != null)
            {

                return DOTween.Sequence().Append(target.GetComponent<Graphic>().DOFade(value, duration).SetEase(ease));
            }
            else
            {
                return DOTween.Sequence();
            }

        }
    }
}