using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{

    public class TriggerCheck : UIECAnimation
    {
        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("Trigger")]
        [OnValueChanged("TriggerTrim")]
        public string trigger = "Default";
        public void TriggerTrim()
        {
            trigger = trigger.Trim();
        }


        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("Consume Trigger")]
        [Tooltip("If consume the trigger.")]
        public bool useTrigger = true;



        [BoxGroup("Success", true, true), LabelText("Animation"), GUIColor(0.4f, 0.8f, 0.4f)]
        [TypeFilter("GetTypeListCallAnimationChannels")]
        [InfoBox("If the Effect Target of this cell is Inherit, which means it will use the Effect Target of the TriggerCheck - no matter it is Inherit, Single, or Group.", "@successAnimation!=null")]
        public CallAnimationChannels successAnimation = null;

        [BoxGroup("Failure", true, true), LabelText("Animation"), GUIColor(0.8f, 0.4f, 0.4f)]
        [TypeFilter("GetTypeListCallAnimationChannels")]
        [InfoBox("If the Effect Target of this cell is Inherit, which means it will use the Effect Target of the TriggerCheck - no matter it is Inherit, Single, or Group.", "@failureAnimation!=null")]
        public CallAnimationChannels failureAnimation = null;

        public IEnumerable<System.Type> GetTypeListCallAnimationChannels()
        {
            var q = new List<System.Type>();
            q.Add(typeof(CallAnimationChannels));
            return q;
        }



        [BoxGroup("Success", true, true), LabelText("Event"), GUIColor(0.4f, 0.8f, 0.4f)]
        [TypeFilter("GetTypeListCallEventChannels")]
        [InfoBox("If the Effect Target of this cell is Inherit, which means it will use the Effect Target of the TriggerCheck - no matter it is Inherit, Single, or Group.", "@successEvent!=null")]
        public CallEventChannels successEvent = null;

        [BoxGroup("Failure", true, true), LabelText("Event"), GUIColor(0.8f, 0.4f, 0.4f)]
        [TypeFilter("GetTypeListCallEventChannels")]
        [InfoBox("If the Effect Target of this cell is Inherit, which means it will use the Effect Target of the TriggerCheck - no matter it is Inherit, Single, or Group.", "@failureEvent!=null")]
        public CallEventChannels failureEvent = null;


        public IEnumerable<System.Type> GetTypeListCallEventChannels()
        {
            var q = new List<System.Type>();
            q.Add(typeof(CallEventChannels));
            return q;
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
                    {
                        if (useTrigger)
                            t.UseTrigger(trigger);

                        if (successEvent != null)
                            successEvent.Do(target);


                        if (successAnimation != null)
                            successAnimation.Do(target);


                    }
                    else
                    {

                        if (failureEvent != null)
                            failureEvent.Do(target);


                        if (failureAnimation != null)
                            failureAnimation.Do(target);
                    }


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