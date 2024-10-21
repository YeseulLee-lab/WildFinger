using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;


namespace HHK.UIEC
{
    // [TypeInfoBox("It blends the Graphic's color while its color is changing by other animations.")]
    public class BlendableColor : UIECAnimation
    {

        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public RandomType rnd = RandomType.Fixed;

        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [ShowIf("@rnd == RandomType.Fixed")]
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
                    return DOTween.Sequence().Join(g.DOBlendableColor(value, duration).SetEase(ease));
                else
                {
                    var c = value;
                    if (values.Count > 0)
                    {
                        c = values[UnityEngine.Random.Range(0, values.Count)];
                    }
                    return DOTween.Sequence().Join(g.DOBlendableColor(c, duration).SetEase(ease));
                }
            }
            else
            {
                return DOTween.Sequence();
            }
        }

    }

}