using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;
using System;

namespace HHK.UIEC
{

    public abstract class UIECAnimatorBase : SerializedMonoBehaviour
    {

        // [BoxGroup("Progress", false)]
        [ProgressBar(0, "fullDuration"), LabelText("")]
        [ShowInInspector]
        [ReadOnly]
        protected float progress = 0f;
        protected float fullDuration = 1f;


        public enum CompleteType
        {
            CompleteWhenForceStop,
            NoCompleteWhenForceStop

        }

        [GUIColor(0.8f, 0.8f, 0.8f, 0.8f), LabelText("")]
        [PropertySpace(0, 8)]
        public CompleteType completeType = CompleteType.CompleteWhenForceStop;

        public void StopAnimation()
        {

            // Stop all the animator bases that attached to this game object.
            var bs = gameObject.GetComponents<UIECAnimatorBase>();
            foreach (var b in bs)
            {
                if (b.seq.IsActive())
                {
                    if (b.completeType == CompleteType.CompleteWhenForceStop)
                    {
                        b.seq.Complete(true);

                        if (b.seq.hasLoops && b.seq.Loops() == -1)
                        {
                            b.seq.Rewind();
                            b.seq.Kill(true);
                        }
                    }
                    else
                    {
                        b.seq.Kill();
                    }
                }

            }

        }


        private void OnDisable()
        {
            StopAnimation();
        }

        protected virtual void LateUpdate()
        {
            DoProgress();
        }

        public void DoProgress()
        {
            if (seq.IsActive())
            {
                progress = seq.fullPosition;
                if (seq.hasLoops && seq.Loops() == -1)
                {
                    fullDuration = seq.fullPosition;
                }
                else
                {
                    fullDuration = seq.Duration(true);
                }
            }
            else
            {
                progress = 0;
                fullDuration = 1f;
            }

        }



        [HideInInspector]
        public Sequence seq = null;



        /// <summary>
        /// Do Animation
        /// </summary>
        /// <param name="animations"></param>
        public void DoAnimation(List<UIECDescriptor> animations)
        {
            StopAnimation();

            seq = DOTween.Sequence();

            foreach (var a in animations)
            {

                if (a == null)
                    continue;

                // if it supports preview
                if (!Application.isPlaying)
                    if (!a.IsAllowPreview())
                        continue;

                if (a is UIECDecorator)
                {
                    var d = a as UIECDecorator;
                    d.Do(seq, gameObject);
                }
                else
                {
                    var anim = a as UIECAnimation;
                    if (anim.runType == RunType.Join)
                        seq.Join(anim.Do(gameObject));
                    else
                        seq.Append(anim.Do(gameObject));

                }

            }
        }

        protected virtual IEnumerable<Type> GetFilteredTypeList()
        {
            var q = new List<Type>(typeof(UIECDescriptor).Assembly.GetTypes());
            q.RemoveAll(x => x.IsAbstract || x.IsGenericTypeDefinition || !typeof(UIECDescriptor).IsAssignableFrom(x));

            return q;
        }
    }
}
