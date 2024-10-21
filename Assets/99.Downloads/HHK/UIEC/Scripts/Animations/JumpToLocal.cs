using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HHK.UIEC
{

    public class JumpToLocal : UIECAnimation
    {
        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f)]
#if UNITY_EDITOR
        [InlineButton("Mark", "Mark")]
        [InlineButton("Set", "Set")]
#endif
        public Vector3 to;

        [BoxGroup("Settings"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public MoveType type = MoveType.Actual;

        [BoxGroup("Settings"), GUIColor(0.6f, 0.6f, 0.8f)]
        public float jumpPower = 10;

        [BoxGroup("Settings"), GUIColor(0.6f, 0.6f, 0.8f)]
        public int numJumps = 3;

        [BoxGroup("Settings"), GUIColor(0.6f, 0.6f, 0.8f)]
        public bool snapping = false;


        [BoxGroup("Duration", false)]
        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public float duration = 0.5f;

        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public Ease ease = Ease.OutCubic;

        protected override Sequence DoAnimation(GameObject target)
        {
            if (type == MoveType.Actual)
                return target.transform.DOLocalJump(to, jumpPower, numJumps, duration, snapping).SetEase(ease);
            else
                return target.transform.DOLocalJump(target.transform.localPosition + to, jumpPower, numJumps, duration, snapping).SetEase(ease);
        }

#if UNITY_EDITOR

        void Mark()
        {
            var objs = EditorFindUs();
            foreach (var o in objs)
                to = o.transform.localPosition;
        }
        void Set()
        {
            var objs = EditorFindUs();
            foreach (var o in objs)
                o.transform.localPosition = to;
        }
#endif
    }
}