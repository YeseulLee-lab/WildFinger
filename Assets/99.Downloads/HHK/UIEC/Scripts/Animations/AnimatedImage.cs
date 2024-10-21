using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{
    [TypeInfoBox("@\"Duration: \"+ evaluteDuration + \" sec\"")]
    public class AnimatedImage : UIECAnimation
    {

        [BoxGroup("FPS", false)]
        [BoxGroup("FPS", false), GUIColor(0.6f, 0.6f, 0.8f)]
        [Tooltip("If FPS <=0, then no interval will be added. Which means, the image will be set immediately.")]
        [OnValueChanged("EvaluateDuration")]
        public float FPS = 60;

        [BoxGroup("FPS", false)]
        [BoxGroup("FPS"), GUIColor(0.6f, 0.6f, 0.8f)]
        [Tooltip("When finished, reset the image to the original one.")]
        public bool resetToOriginal = false;

        [BoxGroup("FPS", false)]
        [BoxGroup("FPS"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [OnValueChanged("EvaluateDuration")]
        public List<Sprite> sprites = new List<Sprite>();

        private float evaluteDuration = float.PositiveInfinity;

        [BoxGroup("FPS"), GUIColor(0.6f, 0.6f, 0.8f)]
        [Button("Evaluate Duration")]
        public void EvaluateDuration()
        {
            float interval = 0;
            evaluteDuration = 0;

            if (FPS > 0)
            {
                interval = 1.0f / FPS;
                evaluteDuration = interval * sprites.Count;
            }

        }

        protected override Sequence DoAnimation(GameObject target)
        {
            Image img = target.GetComponent<Image>();
            if (img != null && sprites.Count > 0)
            {

                Sprite ori = img.sprite;

                float interval = 0;

                if (FPS > 0)
                {
                    interval = 1.0f / FPS;
                }

                var seq = DOTween.Sequence();
                foreach (var s in sprites)
                {
                    seq.AppendCallback(() => { img.sprite = s; });
                    if (FPS > 0)
                    {
                        seq.AppendInterval(interval);
                    }
                }

                if (resetToOriginal)
                {
                    seq.AppendCallback(() => { img.sprite = ori; });
                }

                return seq;
            }
            else
            {
                return DOTween.Sequence();
            }
        }
    }
}