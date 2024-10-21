using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{
    // [TypeInfoBox("It changes the Graphic's color.")]
    public class UIColor : UIECAnimation
    {

        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public RandomType rnd = RandomType.Fixed;

        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [ShowIf("@rnd == RandomType.Fixed")]
#if UNITY_EDITOR
        [InlineButton("Mark", "Mark")]
        [InlineButton("Mark", "Set")]
#endif
        public Color value = Color.white;

        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [ShowIf("@rnd == RandomType.Random")]
        [ListDrawerSettings(CustomAddFunction = "AddValue")]
        public List<Color> values = new List<Color>();

        void AddValue()
        {
            values.Add(Color.white);
        }


        [BoxGroup("Duration", false)]
        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public float duration = 0.5f;

        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public Ease ease = Ease.OutCubic;


        protected override Sequence DoAnimation(GameObject target)
        {
            Graphic g = target.GetComponent<Graphic>();
            if (g != null)
            {
                if (rnd == RandomType.Fixed)
                    return DOTween.Sequence().Join(g.DOColor(value, duration).SetEase(ease));
                else
                {
                    var c = value;
                    if (values.Count > 0)
                    {
                        c = values[UnityEngine.Random.Range(0, values.Count)];
                    }
                    return DOTween.Sequence().Join(g.DOColor(c, duration).SetEase(ease));
                }
            }
            else
            {
                return DOTween.Sequence();
            }

        }


#if UNITY_EDITOR
        void Mark()
        {
            var objs = EditorFindUs();
            foreach (var o in objs)
            {
                Graphic g = o.GetComponent<Graphic>();
                if (g != null)
                {
                    value = g.color;
                }
            }
        }
        void Set()
        {
            var objs = EditorFindUs();
            foreach (var o in objs)
            {
                Graphic g = o.GetComponent<Graphic>();
                if (g != null)
                {
                    g.color = value;
                }
            }
        }
#endif

    }
}