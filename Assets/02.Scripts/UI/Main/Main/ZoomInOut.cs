using DG.Tweening;
using UnityEngine;

public class ZoomInOut : MonoBehaviour
{
    [SerializeField] 
    private RectTransform _zoomTargetRt;

    private readonly float _ZOOM_DELAY = 0.2f;
    private readonly float _ZOOM_IN_MAX = 16f;
    private readonly float _ZOOM_OUT_MAX = 1f;
    private readonly float _ZOOM_SPEED = 1.5f;

    private bool _isZooming = false;
    private float _delayCount = 0f;

    private void Update()
    {
        if (Input.touchCount == 2)
        {
            ZoomAndPan();
        }
        else
        {
            _isZooming = false;
            _delayCount = 0f;
            _zoomTargetRt.DOScale(1f, 0.2f);
            _zoomTargetRt.DOAnchorPos(Vector2.zero, 0.2f);
        }
    }

    private void ZoomAndPan()
    {
        if (!_isZooming)
        {
            _isZooming = true;
        }

        if (_delayCount < _ZOOM_DELAY)
        {
            _delayCount += Time.deltaTime;
            return;
        }

        /* get zoomAmount */
        var prevTouchAPos = Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition;
        var prevTouchBPos = Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition;
        var curTouchAPos = Input.GetTouch(0).position;
        var curTouchBPos = Input.GetTouch(1).position;
        var deltaDistance =
            Vector2.Distance(Normalize(curTouchAPos), Normalize(curTouchBPos))
            - Vector2.Distance(Normalize(prevTouchAPos), Normalize(prevTouchBPos));
        var currentScale = _zoomTargetRt.localScale.x;
        var zoomAmount = deltaDistance * currentScale * _ZOOM_SPEED; // zoomAmount == deltaScale

        /* clamp & zoom */
        var zoomedScale = currentScale + zoomAmount;
        if (zoomedScale < _ZOOM_OUT_MAX)
        {
            zoomedScale = _ZOOM_OUT_MAX;
            zoomAmount = 0f;
        }
        if (_ZOOM_IN_MAX < zoomedScale)
        {
            zoomedScale = _ZOOM_IN_MAX;
            zoomAmount = 0f;
        }
        _zoomTargetRt.localScale = zoomedScale * Vector3.one;

        /* apply offset */
        // offset is a value against movement caused by scale up & down
        var pivotPos = _zoomTargetRt.anchoredPosition;
        var fromCenterToInputPos = new Vector2(
                Input.mousePosition.x - Screen.width * 0.5f,
                Input.mousePosition.y - Screen.height * 0.5f);
        var fromPivotToInputPos = fromCenterToInputPos - pivotPos;
        var offsetX = (fromPivotToInputPos.x / zoomedScale) * zoomAmount;
        var offsetY = (fromPivotToInputPos.y / zoomedScale) * zoomAmount;
        _zoomTargetRt.anchoredPosition -= new Vector2(offsetX, offsetY);

        /* get moveAmount */
        var deltaPosTouchA = Input.GetTouch(0).deltaPosition;
        var deltaPosTouchB = Input.GetTouch(1).deltaPosition;
        var deltaPosTotal = (deltaPosTouchA + deltaPosTouchB) * 0.5f;
        var moveAmount = new Vector2(deltaPosTotal.x, deltaPosTotal.y);

        /* clamp & pan */
        var clampX = (Screen.width * zoomedScale - Screen.width) * 0.5f;
        var clampY = (Screen.height * zoomedScale - Screen.height) * 0.5f;
        var clampedPosX = Mathf.Clamp(_zoomTargetRt.localPosition.x + moveAmount.x, -clampX, clampX);
        var clampedPosY = Mathf.Clamp(_zoomTargetRt.localPosition.y + moveAmount.y, -clampY, clampY);
        _zoomTargetRt.anchoredPosition = new Vector3(clampedPosX, clampedPosY);
    }

    private Vector2 Normalize(Vector2 position)
    {
        var normlizedPos = new Vector2(
            (position.x - Screen.width * 0.5f) / (Screen.width * 0.5f),
            (position.y - Screen.height * 0.5f) / (Screen.height * 0.5f));
        return normlizedPos;
    }
}
