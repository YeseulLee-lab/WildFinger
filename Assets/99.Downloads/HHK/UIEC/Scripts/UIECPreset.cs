using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

namespace HHK.UIEC
{
    /// <summary>
    /// A special animation holder, it defines an animations but do not handle the sequence.
    /// </summary>
    [CreateAssetMenu(fileName = "UnnamedPreset", menuName = "HHK/UIEC/UIEC Preset")]
    public class UIECPreset : SerializedScriptableObject
    {

        [PropertySpace]

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UIECDescriptor"></typeparam>
        /// <returns></returns>
        [TypeFilter("GetFilteredTypeList")]
        [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = false)]
        public List<UIECDescriptor> animations = new List<UIECDescriptor>();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Sequence DoPreset(GameObject target)
        {

            var seq = DOTween.Sequence();

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
                    d.Do(seq, target);
                }
                else
                {
                    var anim = a as UIECAnimation;
                    if (anim.runType == RunType.Join)
                        seq.Join(anim.Do(target));
                    else
                        seq.Append(anim.Do(target));

                }

            }

            return seq;
        }

        protected virtual IEnumerable<System.Type> GetFilteredTypeList()
        {
            var q = new List<System.Type>(typeof(UIECDescriptor).Assembly.GetTypes());
            q.RemoveAll(x => x.IsAbstract || x.IsGenericTypeDefinition || !typeof(UIECDescriptor).IsAssignableFrom(x));
            q.RemoveAll(x => typeof(Loop).IsAssignableFrom(x)); // No loops
            return q;
        }

    }
}