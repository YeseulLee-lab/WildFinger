using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{
    public class CallAnimationChannels : UIECAnimation
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
                        seq.AppendCallback(() => { a.OnCustomChannel(); });
                }

                if ((channels & UIECUseEvents.OnPointerEnter) == UIECUseEvents.OnPointerEnter)
                {
                    seq.AppendCallback(() => { a.OnPointerEnter(null); });
                }

                if ((channels & UIECUseEvents.OnPointerExit) == UIECUseEvents.OnPointerExit)
                {
                    seq.AppendCallback(() => { a.OnPointerExit(null); });
                }

                if ((channels & UIECUseEvents.OnPointerMove) == UIECUseEvents.OnPointerMove)
                {
                    seq.AppendCallback(() => { a.OnPointerMove(null); });
                }

                if ((channels & UIECUseEvents.OnPointerUp) == UIECUseEvents.OnPointerUp)
                {
                    seq.AppendCallback(() => { a.OnPointerUp(null); });
                }

                if ((channels & UIECUseEvents.OnPointerDown) == UIECUseEvents.OnPointerDown)
                {
                    seq.AppendCallback(() => { a.OnPointerDown(null); });
                }

                if ((channels & UIECUseEvents.OnPointerClick) == UIECUseEvents.OnPointerClick)
                {
                    seq.AppendCallback(() => { a.OnPointerClick(null); });
                }

                if ((channels & UIECUseEvents.OnInitializePotentialDrag) == UIECUseEvents.OnInitializePotentialDrag)
                {
                    seq.AppendCallback(() => { a.OnInitializePotentialDrag(null); });
                }

                if ((channels & UIECUseEvents.OnBeginDrag) == UIECUseEvents.OnBeginDrag)
                {
                    seq.AppendCallback(() => { a.OnBeginDrag(null); });
                }

                if ((channels & UIECUseEvents.OnDrag) == UIECUseEvents.OnDrag)
                {
                    seq.AppendCallback(() => { a.OnDrag(null); });
                }

                if ((channels & UIECUseEvents.OnEndDrag) == UIECUseEvents.OnEndDrag)
                {
                    seq.AppendCallback(() => { a.OnEndDrag(null); });
                }

                if ((channels & UIECUseEvents.OnDrop) == UIECUseEvents.OnDrop)
                {
                    seq.AppendCallback(() => { a.OnDrop(null); });
                }

                if ((channels & UIECUseEvents.OnScroll) == UIECUseEvents.OnScroll)
                {
                    seq.AppendCallback(() => { a.OnScroll(null); });
                }

                if ((channels & UIECUseEvents.OnUpdateSelected) == UIECUseEvents.OnUpdateSelected)
                {
                    seq.AppendCallback(() => { a.OnUpdateSelected(null); });
                }

                if ((channels & UIECUseEvents.OnSelect) == UIECUseEvents.OnSelect)
                {
                    seq.AppendCallback(() => { a.OnSelect(null); });
                }

                if ((channels & UIECUseEvents.OnDeselect) == UIECUseEvents.OnDeselect)
                {
                    seq.AppendCallback(() => { a.OnDeselect(null); });
                }

                if ((channels & UIECUseEvents.OnMove) == UIECUseEvents.OnMove)
                {
                    seq.AppendCallback(() => { a.OnMove(null); });
                }

                if ((channels & UIECUseEvents.OnSubmit) == UIECUseEvents.OnSubmit)
                {
                    seq.AppendCallback(() => { a.OnSubmit(null); });
                }

                if ((channels & UIECUseEvents.OnCancel) == UIECUseEvents.OnCancel)
                {
                    seq.AppendCallback(() => { a.OnCancel(null); });
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