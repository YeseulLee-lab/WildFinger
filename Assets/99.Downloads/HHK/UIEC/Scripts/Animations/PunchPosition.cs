using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{

    public class PunchPosition : UIECAnimation
    {
        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f)]
        public Vector3 direction = new Vector3(0, 10, 0);

        [BoxGroup("Settings"), GUIColor(0.6f, 0.6f, 0.8f)]
        public int vibrato = 10;

        [BoxGroup("Settings"), GUIColor(0.6f, 0.6f, 0.8f)]
        [Range(0f, 1f)]
        public float elasticity = 0.5f;

        [BoxGroup("Duration", false)]
        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public float duration = 0.5f;

        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public Ease ease = Ease.OutCubic;


        protected override Sequence DoAnimation(GameObject target)
        {
            return DOTween.Sequence().Append(target.transform.DOPunchPosition(direction, duration, vibrato, elasticity).SetEase(ease));
        }
    }

}
