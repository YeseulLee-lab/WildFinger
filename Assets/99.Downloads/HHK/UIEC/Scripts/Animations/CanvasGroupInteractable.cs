using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{

    public class CanvasGroupInteractable : UIECAnimation
    {

        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f)]
        public bool interactable = false;


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
#endif

        protected override Sequence DoAnimation(GameObject target)
        {

            CanvasGroup cg = target.GetComponent<CanvasGroup>();
            if (cg != null)
            {

                return DOTween.Sequence().AppendCallback(() => cg.interactable = interactable);
            }
            else
            {
                return DOTween.Sequence();
            }

        }
        public override bool IsAllowPreview()
        {
            return NotAllowInPreview();
        }


    }
}