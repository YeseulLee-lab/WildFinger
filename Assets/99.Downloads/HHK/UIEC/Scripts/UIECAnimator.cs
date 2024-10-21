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

    [AddComponentMenu("HHK/UIEC/UIEC Animator")]
    public class UIECAnimator : UIECAnimatorBase,
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



        [TypeFilter("GetFilteredTypeList")]
        [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = false)]
        public List<UIECDescriptor> animations = new List<UIECDescriptor>();


        public void OnCustomChannel()
        {
            DoAnimation(animations);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnPointerEnter) != UIECUseEvents.OnPointerEnter)
                return;




            DoAnimation(animations);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnPointerExit) != UIECUseEvents.OnPointerExit)
                return;



            DoAnimation(animations);
        }


        public void OnPointerMove(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnPointerMove) != UIECUseEvents.OnPointerMove)
                return;



            DoAnimation(animations);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnPointerUp) != UIECUseEvents.OnPointerUp)
                return;



            DoAnimation(animations);
        }



        public void OnPointerDown(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnPointerDown) != UIECUseEvents.OnPointerDown)
                return;



            DoAnimation(animations);
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnPointerClick) != UIECUseEvents.OnPointerClick)
                return;



            DoAnimation(animations);
        }


        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnInitializePotentialDrag) != UIECUseEvents.OnInitializePotentialDrag)
                return;



            DoAnimation(animations);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnBeginDrag) != UIECUseEvents.OnBeginDrag)
                return;



            DoAnimation(animations);
        }


        public void OnDrag(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnDrag) != UIECUseEvents.OnDrag)
                return;



            DoAnimation(animations);
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnEndDrag) != UIECUseEvents.OnEndDrag)
                return;



            DoAnimation(animations);
        }



        public void OnDrop(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnDrop) != UIECUseEvents.OnDrop)
                return;



            DoAnimation(animations);
        }


        public void OnScroll(PointerEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnScroll) != UIECUseEvents.OnScroll)
                return;



            DoAnimation(animations);
        }

        public void OnUpdateSelected(BaseEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnUpdateSelected) != UIECUseEvents.OnUpdateSelected)
                return;



            DoAnimation(animations);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnSelect) != UIECUseEvents.OnSelect)
                return;



            DoAnimation(animations);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnDeselect) != UIECUseEvents.OnDeselect)
                return;



            DoAnimation(animations);
        }


        public void OnMove(AxisEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnMove) != UIECUseEvents.OnMove)
                return;



            DoAnimation(animations);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnSubmit) != UIECUseEvents.OnSubmit)
                return;



            DoAnimation(animations);
        }


        public void OnCancel(BaseEventData eventData)
        {
            if ((useEvents & UIECUseEvents.OnCancel) != UIECUseEvents.OnCancel)
                return;



            DoAnimation(animations);
        }

    }
}