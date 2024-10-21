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

    public class ZForDevOnly : UIECAnimation
    {
        protected override Sequence DoAnimation(GameObject target)
        {
            return DOTween.Sequence();
        }


#if UNITY_EDITOR
        void MarkPosition()
        {

        }
        void SetPosition()
        {

        }

#endif
    }
}