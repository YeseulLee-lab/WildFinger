using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{
    public class TriggerSwitch : UIECAnimation
    {
        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [OnValueChanged("TriggerTrim")]
        public string trigger = "Default";
        public void TriggerTrim()
        {
            trigger = trigger.Trim();
        }
        protected override Sequence DoAnimation(GameObject target)
        {

            var t = target.GetComponent<UIECTrigger>();
            var seq = DOTween.Sequence();
            if (t != null)
            {
                seq.AppendCallback(() =>
                {
                    if (t.HasTrigger(trigger))
                        t.UseTrigger(trigger);
                    else
                        t.SetTrigger(trigger);
                });
            }

            return seq;
        }
        public override bool IsAllowPreview()
        {
            return NotAllowInPreview();
        }
    }
}