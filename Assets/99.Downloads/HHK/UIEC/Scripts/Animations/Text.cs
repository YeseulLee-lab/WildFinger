using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{

    public class Text : UIECAnimation
    {
        public enum ReferenceType
        {
            Custom,
            Text,
            InputField
        }

        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f)]
        public ReferenceType referenceType = ReferenceType.Custom;

        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f)]
        [Required]
        [ShowIf("@referenceType == ReferenceType.Text")]
#if UNITY_EDITOR
        [InlineButton("FindRef", "Self")]
#endif
        public UnityEngine.UI.Text referenceText;

        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f)]
        [Required]
        [ShowIf("@referenceType == ReferenceType.InputField")]
#if UNITY_EDITOR
        [InlineButton("FindRef", "Self")]
#endif
        public UnityEngine.UI.InputField referenceInputField;


        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [TextArea(3, 5)]
        [ShowIf("@referenceType == ReferenceType.Custom")]
        public string value = "";

        [BoxGroup("Settings"), GUIColor(0.6f, 0.6f, 0.8f)]
        public bool richTextEnabled = true;

        [BoxGroup("Settings"), GUIColor(0.6f, 0.6f, 0.8f)]
        public ScrambleMode scrambleMode = ScrambleMode.None;

        [BoxGroup("Settings"), GUIColor(0.6f, 0.6f, 0.8f)]
        public string scramblechars = null;

        [BoxGroup("Duration", false)]
        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public float duration = 0.5f;

        [HorizontalGroup("Duration/Sub"), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public Ease ease = Ease.OutCubic;

        protected override Sequence DoAnimation(GameObject target)
        {
            UnityEngine.UI.Text t = target.GetComponent<UnityEngine.UI.Text>();
            if (t != null)
            {
                switch (referenceType)
                {
                    case ReferenceType.Custom:
                        return DOTween.Sequence().Append(t.DOText(value, duration, richTextEnabled, scrambleMode, scramblechars).SetEase(ease));
                    case ReferenceType.Text:
                        if (referenceText != null)
                            return DOTween.Sequence().Append(t.DOText(referenceText.text, duration, richTextEnabled, scrambleMode, scramblechars).SetEase(ease));
                        break;
                    case ReferenceType.InputField:
                        if (referenceInputField != null)
                            return DOTween.Sequence().Append(t.DOText(referenceInputField.text, duration, richTextEnabled, scrambleMode, scramblechars).SetEase(ease));
                        break;
                    default:
                        return DOTween.Sequence();
                }
                return DOTween.Sequence();
            }
            else
            {
                return DOTween.Sequence();
            }
        }

#if UNITY_EDITOR
        public void FindRef()
        {
            referenceText = UnityEditor.Selection.activeGameObject.GetComponent<UnityEngine.UI.Text>();
            referenceInputField = UnityEditor.Selection.activeGameObject.GetComponent<UnityEngine.UI.InputField>();
        }
#endif
    }

}