using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{
    // [TypeInfoBox("It changes the Image's Fill Amount.\nThe Image Type must be filled.")]
    public class FillAmount : UIECAnimation
    {


        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [Range(0f, 1f)]
        public float to = 1.0f;

        [BoxGroup("Duration", false)]
        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public float duration = 0.5f;

        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public Ease ease = Ease.OutCubic;


        protected override Sequence DoAnimation(GameObject target)
        {

            Image img = target.GetComponent<Image>();
            if (img != null)
            {

                return DOTween.Sequence().Append(img.DOFillAmount(to, duration).SetEase(ease));
            }
            else
            {
                return DOTween.Sequence();
            }

        }

    }

}