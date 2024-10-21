using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{
    public class CallEventChannels : UIECAnimation
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
            var c = UnityEditor.Selection.activeGameObject.GetComponentInParent<UIECEvent>();
            if (c != null)
                effectTarget = c.gameObject;
        }
#endif

        protected override Sequence DoAnimation(GameObject target)
        {
            var seq = DOTween.Sequence();
            var es = target.GetComponents<UIECEvent>();
            foreach (var e in es)
            {
                if ((channels & UIECUseEvents.Custom) == UIECUseEvents.Custom)
                {
                    if (e.channel.Trim() == channel.Trim())
                        seq.AppendCallback(() => { e.OnCustomChannel(); });
                }
                if ((channels & UIECUseEvents.OnPointerEnter) == UIECUseEvents.OnPointerEnter)
                {
                    seq.AppendCallback(() => { e.OnPointerEnter(null); });
                }

                if ((channels & UIECUseEvents.OnPointerExit) == UIECUseEvents.OnPointerExit)
                {
                    seq.AppendCallback(() => { e.OnPointerExit(null); });
                }

                if ((channels & UIECUseEvents.OnPointerMove) == UIECUseEvents.OnPointerMove)
                {
                    seq.AppendCallback(() => { e.OnPointerMove(null); });
                }

                if ((channels & UIECUseEvents.OnPointerUp) == UIECUseEvents.OnPointerUp)
                {
                    seq.AppendCallback(() => { e.OnPointerUp(null); });
                }

                if ((channels & UIECUseEvents.OnPointerDown) == UIECUseEvents.OnPointerDown)
                {
                    seq.AppendCallback(() => { e.OnPointerDown(null); });
                }

                if ((channels & UIECUseEvents.OnPointerClick) == UIECUseEvents.OnPointerClick)
                {
                    seq.AppendCallback(() => { e.OnPointerClick(null); });
                }

                if ((channels & UIECUseEvents.OnInitializePotentialDrag) == UIECUseEvents.OnInitializePotentialDrag)
                {
                    seq.AppendCallback(() => { e.OnInitializePotentialDrag(null); });
                }

                if ((channels & UIECUseEvents.OnBeginDrag) == UIECUseEvents.OnBeginDrag)
                {
                    seq.AppendCallback(() => { e.OnBeginDrag(null); });
                }

                if ((channels & UIECUseEvents.OnDrag) == UIECUseEvents.OnDrag)
                {
                    seq.AppendCallback(() => { e.OnDrag(null); });
                }

                if ((channels & UIECUseEvents.OnEndDrag) == UIECUseEvents.OnEndDrag)
                {
                    seq.AppendCallback(() => { e.OnEndDrag(null); });
                }

                if ((channels & UIECUseEvents.OnDrop) == UIECUseEvents.OnDrop)
                {
                    seq.AppendCallback(() => { e.OnDrop(null); });
                }

                if ((channels & UIECUseEvents.OnScroll) == UIECUseEvents.OnScroll)
                {
                    seq.AppendCallback(() => { e.OnScroll(null); });
                }

                if ((channels & UIECUseEvents.OnUpdateSelected) == UIECUseEvents.OnUpdateSelected)
                {
                    seq.AppendCallback(() => { e.OnUpdateSelected(null); });
                }

                if ((channels & UIECUseEvents.OnSelect) == UIECUseEvents.OnSelect)
                {
                    seq.AppendCallback(() => { e.OnSelect(null); });
                }

                if ((channels & UIECUseEvents.OnDeselect) == UIECUseEvents.OnDeselect)
                {
                    seq.AppendCallback(() => { e.OnDeselect(null); });
                }

                if ((channels & UIECUseEvents.OnMove) == UIECUseEvents.OnMove)
                {
                    seq.AppendCallback(() => { e.OnMove(null); });
                }

                if ((channels & UIECUseEvents.OnSubmit) == UIECUseEvents.OnSubmit)
                {
                    seq.AppendCallback(() => { e.OnSubmit(null); });
                }

                if ((channels & UIECUseEvents.OnCancel) == UIECUseEvents.OnCancel)
                {
                    seq.AppendCallback(() => { e.OnCancel(null); });
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