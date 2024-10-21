using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{
    public class Log : UIECDecorator
    {
        public enum Type
        {
            Log,
            Warning,
            Error
        }
        [BoxGroup("Settings", false)]
        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public Type type = Type.Log;

        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]

        public Color c = Color.white;


        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [TextArea(3, 5)]
        public string value = "";

        public override Sequence Do(Sequence seq, GameObject target)
        {
            string preFix = "";
            switch (type)
            {
                case Type.Log:
                    preFix = "<color=orange>[UIEC] Log:</color>";
                    seq.AppendCallback(() => Debug.Log($"{preFix} <color=#{ColorUtility.ToHtmlStringRGB(c)}>{value}</color>"));
                    break;

                case Type.Warning:
                    preFix = "<color=yellow>[UIEC] Warning:</color>";
                    seq.AppendCallback(() => Debug.LogWarning($"{preFix} <color=#{ColorUtility.ToHtmlStringRGB(c)}>{value}</color>"));
                    break;

                case Type.Error:
                    preFix = "<color=red>[UIEC] Error:</color>";
                    seq.AppendCallback(() => Debug.LogError($"{preFix} <color=#{ColorUtility.ToHtmlStringRGB(c)}>{value}</color>"));
                    break;

            }
            return seq;
        }
    }
}