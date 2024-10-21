using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{
    [TypeInfoBox("If the Effect Target does not have a Audio Source, then it will create one.")]
    public class PlayAudio : UIECAnimation
    {

        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [Required]
        public AudioClip clip;

        // [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        AudioSource source;

        protected override Sequence DoAnimation(GameObject target)
        {
            FindSelf(target);
            return DOTween.Sequence().AppendCallback(() => { source.PlayOneShot(clip); });
        }

        void FindSelf(GameObject target)
        {
            if (source == null)
            {
                source = target.GetComponent<AudioSource>();

                if (source == null)
                {
                    source = target.AddComponent<AudioSource>();
                }
            }
        }


    }

}