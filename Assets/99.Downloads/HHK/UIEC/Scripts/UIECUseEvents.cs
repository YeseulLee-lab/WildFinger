using System;

namespace HHK.UIEC
{

    // Flags, indicats use which events
    public enum UIECUseEvents

    {
        Custom = 1 << 0,
        OnPointerEnter = 1 << 1,
        OnPointerExit = 1 << 2,
        OnPointerMove = 1 << 3,
        OnPointerUp = 1 << 4,
        OnPointerDown = 1 << 5,
        OnPointerClick = 1 << 6,
        OnInitializePotentialDrag = 1 << 7,
        OnBeginDrag = 1 << 8,
        OnDrag = 1 << 9,
        OnEndDrag = 1 << 10,
        OnDrop = 1 << 11,
        OnScroll = 1 << 12,
        OnUpdateSelected = 1 << 13,
        OnSelect = 1 << 14,
        OnDeselect = 1 << 15,
        OnMove = 1 << 16,
        OnSubmit = 1 << 17,
        OnCancel = 1 << 18,


        AllEvents = ~0

    }

    public enum EventType
    {
        EventBlock,
        EventPassThrough
    }
}