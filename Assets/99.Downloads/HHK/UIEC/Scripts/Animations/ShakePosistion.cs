using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{

    public class ShakePosition : UIECAnimation
    {
        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f)]
        public float strength = 10;

        [BoxGroup("Settings"), GUIColor(0.6f, 0.6f, 0.8f)]
        public float randomness = 90;

        [BoxGroup("Settings"), GUIColor(0.6f, 0.6f, 0.8f)]
        public bool snapping = false;

        [BoxGroup("Settings"), GUIColor(0.6f, 0.6f, 0.8f)]
        public bool fadeOut = true;

        [BoxGroup("Settings"), GUIColor(0.6f, 0.6f, 0.8f)]
        public int vibrato = 10;

        [BoxGroup("Duration", false)]
        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public float duration = 0.5f;

        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public Ease ease = Ease.OutCubic;


        protected override Sequence DoAnimation(GameObject target)
        {

            return DOTween.Sequence().Append(target.transform.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut).SetEase(ease));
        }
    }
}