using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{

    public class RunnerStart : UIECAnimation
    {

        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [OnValueChanged("TrimChannel")]
        public string channel = "Default";

        void TrimChannel()
        {
            channel = channel.Trim();
        }

        protected override Sequence DoAnimation(GameObject target)
        {
            var rs = target.GetComponents<UIECRunner>();
            var seq = DOTween.Sequence(); ;
            foreach (var r in rs)
            {
                if (r.channel.Trim() == channel.Trim())
                    seq.AppendCallback(() => r.RunImmediately());
            }
            return seq;

        }

        public override bool IsAllowPreview()
        {
            return NotAllowInPreview();
        }
    }
}