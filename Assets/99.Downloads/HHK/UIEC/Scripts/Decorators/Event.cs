using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Sirenix.OdinInspector;



namespace HHK.UIEC
{
    // [TypeInfoBox("Invoke the callback functions.")]
    public class Event : UIECDecorator
    {
        public enum RunType
        {
            Follow,
            Join
        }


        [BoxGroup("Basic", false), GUIColor(0.6f, 0.8f, 0.6f), LabelText("")]
        public RunType runType = RunType.Follow;



        [BoxGroup("Settings", false)]
        [GUIColor(0.6f, 0.6f, 0.8f)]
        [DrawWithUnity]
        public UnityEvent callback = new UnityEvent();

        public override Sequence Do(Sequence seq, GameObject target)
        {
            if (runType == RunType.Follow)
                seq.AppendCallback(() => { callback.Invoke(); });
            else
                seq.Join(DOTween.Sequence().AppendCallback(() => { callback.Invoke(); }));
            return seq;
        }

        public override bool IsAllowPreview()
        {
            return NotAllowInPreview();
        }



    }
}