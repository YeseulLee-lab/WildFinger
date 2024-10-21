using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{
    public class Deactivate : UIECAnimation
    {
        protected override Sequence DoAnimation(GameObject target)
        {
            return DOTween.Sequence().AppendCallback(() => target.SetActive(false));
        }

        public override bool IsAllowPreview()
        {
            return NotAllowInPreview();
        }
    }
}
