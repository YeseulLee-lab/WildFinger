using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;
using System;

namespace HHK.UIEC
{

    [AddComponentMenu("HHK/UIEC/UIEC Event")]
    public class UIECEvent : UIECEventBase,
        IPointerEnterHandler,
        IPointerExitHandler,
#if UNITY_2021_1_OR_NEWER
        IPointerMoveHandler,
#endif
        IPointerUpHandler,
        IPointerDownHandler,
        IPointerClickHandler,
        IInitializePotentialDragHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler,
        IScrollHandler,
        IUpdateSelectedHandler,
        ISelectHandler,
        IDeselectHandler,
        IMoveHandler,
        ISubmitHandler,
        ICancelHandler
    {

        [GUIColor(0.8f, 0.8f, 0.8f, 0.8f), LabelText("")]
        public UIECUseEvents useEvents = UIECUseEvents.Custom;


        [GUIColor(0.8f, 0.8f, 0.8f, 0.8f)]
        [ShowIf("@useEvents == UIECUseEvents.Custom")]
        [OnValueChanged("TrimChannel")]
        public string channel = "Default";
        void TrimChannel()
        {
            channel = channel.Trim();
        }

        public string GetInspectorName()
        {
            if (useEvents == UIECUseEvents.Custom)
                return channel;
            else
                return useEvents.ToString();
        }

        [PropertySpace]


        public UnityEvent events = new UnityEvent();

        public void OnCustomChannel()
        {
            DoEvent(events);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnPointerEnter) != UIECUseEvents.OnPointerEnter)
                return;

            DoEvent(events);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnPointerExit) != UIECUseEvents.OnPointerExit)
                return;
            DoEvent(events);
        }



        public void OnPointerMove(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnPointerMove) != UIECUseEvents.OnPointerMove)
                return;
            DoEvent(events);
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnPointerUp) != UIECUseEvents.OnPointerUp)
                return;
            DoEvent(events);
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnPointerDown) != UIECUseEvents.OnPointerDown)
                return;
            DoEvent(events);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnPointerClick) != UIECUseEvents.OnPointerClick)
                return;
            DoEvent(events);
        }



        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnInitializePotentialDrag) != UIECUseEvents.OnInitializePotentialDrag)
                return;
            DoEvent(events);
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnBeginDrag) != UIECUseEvents.OnBeginDrag)
                return;
            DoEvent(events);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnDrag) != UIECUseEvents.OnDrag)
                return;
            DoEvent(events);
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnEndDrag) != UIECUseEvents.OnEndDrag)
                return;
            DoEvent(events);
        }


        public void OnDrop(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnDrop) != UIECUseEvents.OnDrop)
                return;
            DoEvent(events);
        }


        public void OnScroll(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnScroll) != UIECUseEvents.OnScroll)
                return;
            DoEvent(events);
        }



        public void OnUpdateSelected(BaseEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnUpdateSelected) != UIECUseEvents.OnUpdateSelected)
                return;
            DoEvent(events);
        }


        public void OnSelect(BaseEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnSelect) != UIECUseEvents.OnSelect)
                return;
            DoEvent(events);
        }



        public void OnDeselect(BaseEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnDeselect) != UIECUseEvents.OnDeselect)
                return;
            DoEvent(events);
        }


        public void OnMove(AxisEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnMove) != UIECUseEvents.OnMove)
                return;
            DoEvent(events);
        }


        public void OnSubmit(BaseEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnSubmit) != UIECUseEvents.OnSubmit)
                return;
            DoEvent(events);
        }

        public void OnCancel(BaseEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnCancel) != UIECUseEvents.OnCancel)
                return;
            DoEvent(events);
        }




    }
}