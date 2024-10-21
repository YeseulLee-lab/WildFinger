using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace HHK.UIEC
{

    public class Preset : UIECAnimation
    {

        public enum PresetRunType
        {
            OneByOne,
            Parallel
        }


        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        [Required]
        public UIECPreset preset;


        // [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        // [OnValueChanged("TrimChannel")]
        // public string channel = "Default";

        [BoxGroup("Settings", false), GUIColor(0.6f, 0.6f, 0.8f), LabelText("")]
        public PresetRunType type = PresetRunType.OneByOne;

        // void TrimChannel()
        // {
        //     channel = channel.Trim();
        // }


        protected override Sequence DoAnimation(GameObject target)
        {
            var seq = DOTween.Sequence();
            if (preset != null)
            {
                if (type == PresetRunType.OneByOne)
                    seq.Append(preset.DoPreset(target));
                else
                    seq.Join(preset.DoPreset(target));
            }
            return seq;
        }
    }
}