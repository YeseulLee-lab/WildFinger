using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{
    public class StopAnimationChannels : UIECAnimation
    {
        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public UIECUseEvents channels = UIECUseEvents.Custom;

        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [ShowIf("@channels == UIECUseEvents.Custom")]
        [OnValueChanged("TrimChannel")]
        public string channel = "Default";

        void TrimChannel()
        {
            channel = channel.Trim();
        }



#if UNITY_EDITOR
        [BoxGroup("ReTargeting"), GUIColor(0.6f, 0.8f, 0.6f), LabelText("")]
        [ShowIf("@reTargeting == ReTargeting.Singel && effectTarget == null")]
        [Button("Find In Parent")]
        void FindEffectTargetInParent()
        {
            effectTarget = null;
            var c = UnityEditor.Selection.activeGameObject.GetComponentInParent<UIECAnimator>();
            if (c != null)
                effectTarget = c.gameObject;
        }
#endif

        protected override Sequence DoAnimation(GameObject target)
        {
            var seq = DOTween.Sequence();
            var ans = target.GetComponents<UIECAnimator>();
            foreach (var a in ans)
            {
                if ((channels & UIECUseEvents.Custom) == UIECUseEvents.Custom)
                {
                    if (a.channel.Trim() == channel.Trim())
                        seq.AppendCallback(() => { a.StopAnimation(); });
                }
                else
                {
                    seq.AppendCallback(() => { a.StopAnimation(); });
                }

            }

            return seq;
        }

        public override bool IsAllowPreview()
        {
            return NotAllowInPreview();
        }
    }
}