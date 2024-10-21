using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class TutorialHandTouchableArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("------------------ Flick -----------------")]
    private Vector3 _flickStartPos;
    private Vector3 _flickEndPos;
    private float _flickMinDistance = 10f; // 최소한의 드래그 거리
    private float _flickMinSpeed = 1000f; // 최소한의 속도

    public UnityAction clickCompleteAction { get; set; } = null;
    public UnityAction holdStartAction { get; set; } = null;
    public UnityAction holdFailAction { get; set; } = null;
    public UnityAction flickCompleteAction = null;
    //hold 는 따로 처리

    private void OnDisable()
    {
        holdStartAction = null;
        clickCompleteAction = null;
        flickCompleteAction = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            _flickStartPos = Input.GetTouch(0).position;
        }
        else
        {
            _flickStartPos = Input.mousePosition;
        }

        clickCompleteAction?.Invoke();
        holdStartAction?.Invoke();
        clickCompleteAction = null;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            _flickEndPos = Input.GetTouch(0).position;
        }
        else
        {
            _flickEndPos = Input.mousePosition;
        }

        holdFailAction?.Invoke();
        CheckFlicking(ref flickCompleteAction);
    }

    private void CheckFlicking(ref UnityAction completeAction)
    {
        float flickDistance = Vector3.Distance(_flickStartPos, _flickEndPos);
        float flickSpeed = flickDistance / Time.deltaTime;

        if (flickDistance >= _flickMinDistance && flickSpeed >= _flickMinSpeed)
        {
            // Flick 감지됨
            //DebugX.Log("Flick Detected!");
            Vector3 flickDirection = (_flickEndPos - _flickStartPos).normalized;

            completeAction?.Invoke();
            completeAction = null;
        }
    }
}
