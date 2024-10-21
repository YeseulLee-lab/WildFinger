using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{
    // [TypeInfoBox("Set a UIECGroup to the Hide stat.")]
    public class GroupStop : UIECAnimation
    {
        protected override Sequence DoAnimation(GameObject target)
        {
            UIECGroup g = target.GetComponent<UIECGroup>();
            var seq = DOTween.Sequence();
            if (g != null)
            {
                seq.AppendCallback(() => g.StopAnimation());
            }
            return seq;

        }

        public override bool IsAllowPreview()
        {
            return NotAllowInPreview();
        }


#if UNITY_EDITOR
        [BoxGroup("ReTargeting"), GUIColor(0.6f, 0.8f, 0.6f), LabelText("")]
        [ShowIf("@reTargeting == ReTargeting.Singel && effectTarget == null")]
        [Button("Find In Parent")]
        void FindEffectTargetInParent()
        {
            effectTarget = null;
            var c = UnityEditor.Selection.activeGameObject.GetComponentInParent<UIECGroup>();
            if (c != null)
                effectTarget = c.gameObject;
        }
#endif

    }




}
