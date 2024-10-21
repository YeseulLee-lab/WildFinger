// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using DG.Tweening;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
// using Sirenix.OdinInspector;

// namespace HHK
// {
//     // [TypeInfoBox("It changes the Image's color with an defined gradient color.")]
//     public class GradientColor : UIECAnimation
//     {

//         [BoxGroup("Settings", false)]
//         [BoxGroup("Settings/Gradient Color", false), GUIColor(1f, 1f, 1f)]
//         public UnityEngine.Gradient gradient = new UnityEngine.Gradient();

//         [BoxGroup("Settings/Gradient Color"), GUIColor(0.6f, 0.6f, 0.8f)]
//         public float duration = 0.5f;

//         [BoxGroup("Settings/Gradient Color"), GUIColor(0.6f, 0.6f, 0.8f)]
//         public Ease ease = Ease.OutCubic;


//         protected override Sequence DoAnimation(GameObject target)
//         {

//             Image img = target.GetComponent<Image>();
//             if (img != null)
//             {

//                 return DOTween.Sequence().Append(img.DOGradientColor(gradient, duration).SetEase(ease));
//             }
//             else
//             {
//                 return DOTween.Sequence();
//             }

//         }

//     }
// }