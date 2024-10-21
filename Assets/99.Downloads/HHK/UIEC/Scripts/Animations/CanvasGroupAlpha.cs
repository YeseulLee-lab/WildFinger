using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{
    // [TypeInfoBox("It changes the Canvas Group Alpha.")]
    public class CanvasGroupAlpha : UIECAnimation
    {

        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [Range(0f, 1f)]
#if UNITY_EDITOR
        [InlineButton("Mark")]
        [InlineButton("Set")]
#endif
        public float to = 1.0f;

        [BoxGroup("Duration", false)]
        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public float duration = 0.5f;

        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public Ease ease = Ease.OutCubic;


#if UNITY_EDITOR
        [BoxGroup("ReTargeting"), GUIColor(0.6f, 0.8f, 0.6f), LabelText("")]
        [ShowIf("@reTargeting == ReTargeting.Singel && effectTarget == null")]
        [Button("Find In Parent")]
        void FindEffectTargetInParent()
        {
            effectTarget = null;
            var c = UnityEditor.Selection.activeGameObject.GetComponentInParent<CanvasGroup>();
            if (c != null)
                effectTarget = c.gameObject;
        }

        void Mark()
        {
            var cg = UnityEditor.Selection.activeGameObject.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                to = cg.alpha;
            }
        }

        void Set()
        {
            var cg = UnityEditor.Selection.activeGameObject.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = to;
            }
        }

#endif

        protected override Sequence DoAnimation(GameObject target)
        {

            CanvasGroup cg = target.GetComponent<CanvasGroup>();
            if (cg != null)
            {

                return DOTween.Sequence().Append(cg.DOFade(to, duration).SetEase(ease));
            }
            else
            {
                return DOTween.Sequence();
            }

        }


    }
}