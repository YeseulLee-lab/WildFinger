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

    public class ScaleToLocal : UIECAnimation
    {

        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f)]
#if UNITY_EDITOR
        [InlineButton("Mark", "Mark")]
        [InlineButton("Set", "Set")]
#endif
        public Vector3 to = Vector3.one;
        [BoxGroup("Settings"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public MoveType type = MoveType.Actual;


        [BoxGroup("Duration", false)]
        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public float duration = 0.5f;

        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public Ease ease = Ease.OutCubic;

        protected override Sequence DoAnimation(GameObject target)
        {
            if (type == MoveType.Actual)
                return DOTween.Sequence().Append(target.transform.DOScale(to, duration).SetEase(ease));
            else
                return DOTween.Sequence().Append(target.transform.DOScale(target.transform.localScale + to, duration).SetEase(ease));
        }


#if UNITY_EDITOR
        void Mark()
        {
            var objs = EditorFindUs();
            foreach (var o in objs)
                to = o.transform.localScale;
        }
        void Set()
        {
            var objs = EditorFindUs();
            foreach (var o in objs)
                o.transform.localScale = to;
        }
#endif

    }
}