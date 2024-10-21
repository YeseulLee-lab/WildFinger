using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// UIECAnimation defines the animations
/// </summary>
namespace HHK.UIEC
{
    public abstract class UIECAnimation : UIECDescriptor
    {

        public enum ReTargeting
        {
            Inherit,
            Singel,
            Group,
            AllGroups
        }




        [BoxGroup("Basic", false), GUIColor(0.6f, 0.8f, 0.6f, 0.8f), LabelText("")]
        public RunType runType = RunType.Join;


        [BoxGroup("ReTargeting", false), GUIColor(0.8f, 0.8f, 0.6f, 0.8f), LabelText("")]
        public ReTargeting reTargeting = ReTargeting.Inherit;


        [BoxGroup("ReTargeting"), GUIColor(0.8f, 0.8f, 0.6f, 0.8f), LabelText("")]
        [ShowIf("@reTargeting == ReTargeting.Group")]
        [OnValueChanged("TrimGroupID")]
        public string groupID = "Default";

        void TrimGroupID()
        {
            groupID = groupID.Trim();
        }

        [BoxGroup("ReTargeting"), GUIColor(0.8f, 0.8f, 0.6f, 0.8f)]
        [ShowIf("@reTargeting == ReTargeting.Group || reTargeting == ReTargeting.AllGroups")]
        public bool includeInactivated = false;

        [BoxGroup("ReTargeting"), GUIColor(0.8f, 0.8f, 0.6f, 0.8f)]
        [ShowIf("@reTargeting == ReTargeting.Group || reTargeting == ReTargeting.AllGroups")]
        public float delayMin = 0f, delayMax = 0f;

        // [BoxGroup("ReTargeting"), GUIColor(0.6f, 0.8f, 0.6f), LabelText("")]
        // [ShowIf("@reTargeting == ReTargeting.Group")]
        // public RunType groupRunType = RunType.Join;


        [BoxGroup("ReTargeting"), GUIColor(0.8f, 0.8f, 0.6f, 0.8f), LabelText("")]
        [ShowIf("@reTargeting == ReTargeting.Singel")]
        [Required]
        public GameObject effectTarget = null;




        /// <summary>
        /// DO
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual Sequence Do(GameObject target)
        {
            switch (reTargeting)
            {
                case ReTargeting.Inherit:
                    if (target != null)
                        return DoAnimation(target);
                    break;


                case ReTargeting.Singel:
                    if (effectTarget != null)
                        return DoAnimation(effectTarget);
                    break;


                case ReTargeting.Group:
                    {
#if UNITY_2020_1_OR_NEWER
                        var ps = GameObject.FindObjectsOfType<UIECGroup>(includeInactivated);
#else
                        var ps = GameObject.FindObjectsOfType<UIECGroup>();
#endif
                        Sequence seq = DOTween.Sequence();
                        foreach (var p in ps)
                        {
                            if (p.groupID.Trim() == groupID.Trim())
                            {
                                seq.Join(DoAnimation(p.gameObject).SetDelay(UnityEngine.Random.Range(delayMin, delayMax)));
                            }
                        }

                        return seq;
                    }

                case ReTargeting.AllGroups:
                    {

#if UNITY_2020_1_OR_NEWER
                        var ps = GameObject.FindObjectsOfType<UIECGroup>(includeInactivated);
#else
                        var ps = GameObject.FindObjectsOfType<UIECGroup>();
#endif
                        Sequence seq = DOTween.Sequence();
                        foreach (var p in ps)
                        {
                            seq.Join(DoAnimation(p.gameObject).SetDelay(UnityEngine.Random.Range(delayMin, delayMax)));
                        }

                        return seq;
                    }
            }

            return DOTween.Sequence();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="eventData"></param>
        /// <returns></returns>
        protected abstract Sequence DoAnimation(GameObject target);


#if UNITY_EDITOR

        [FoldoutGroup("Editor Tools")]
        [Button]
        protected void EditorListEffectTargets()
        {
            var objs = EditorFindUs();
            foreach (var o in objs)
                Debug.Log($"[UIEC] Effect Target: {o.gameObject}");
        }

        [FoldoutGroup("Editor Tools")]
        [Button]
        protected void EditorSelectEffectTargets()
        {
            var objs = EditorFindUs();
            Selection.objects = objs.ToArray();
        }

        protected List<GameObject> EditorFindUs()
        {
            var ats = UnityEngine.GameObject.FindObjectsOfType<UIECAnimator>();
            List<GameObject> objs = new List<GameObject>();

            // Find my animator
            foreach (var at in ats)
            {
                foreach (var a in at.animations)
                {
                    if (a == this)
                    {
                        // Found!
                        switch (reTargeting)
                        {
                            case ReTargeting.Inherit:
                                objs.Add(at.gameObject);
                                break;


                            case ReTargeting.Singel:
                                if (effectTarget != null)
                                    objs.Add(effectTarget);
                                break;


                            case ReTargeting.Group:
                                {
#if UNITY_2020_1_OR_NEWER
                                    var ps = GameObject.FindObjectsOfType<UIECGroup>(includeInactivated);
#else
                                    var ps = GameObject.FindObjectsOfType<UIECGroup>();
#endif

                                    Sequence seq = DOTween.Sequence();
                                    foreach (var p in ps)
                                    {
                                        if (p.groupID.Trim() == groupID.Trim())
                                        {
                                            objs.Add(p.gameObject);
                                        }
                                    }

                                    break;
                                }

                            case ReTargeting.AllGroups:
                                {

#if UNITY_2020_1_OR_NEWER
                                    var ps = GameObject.FindObjectsOfType<UIECGroup>(includeInactivated);
#else
                                    var ps = GameObject.FindObjectsOfType<UIECGroup>();
#endif
                                    Sequence seq = DOTween.Sequence();
                                    foreach (var p in ps)
                                    {
                                        objs.Add(p.gameObject);

                                    }

                                    break;
                                }

                        }

                        break; // if (a == this)
                    }
                }
            }

            return objs;
        }

#endif
    }
}