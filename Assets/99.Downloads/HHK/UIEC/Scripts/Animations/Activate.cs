using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening.Plugins;

namespace HHK.UIEC
{
    public class Activate : UIECAnimation
    {
        protected override Sequence DoAnimation(GameObject target)
        {
            return DOTween.Sequence().AppendCallback(() => target.SetActive(true));
        }

        public override bool IsAllowPreview()
        {
            return NotAllowInPreview();
        }
    }
}
