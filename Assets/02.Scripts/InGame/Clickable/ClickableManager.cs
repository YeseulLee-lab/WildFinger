using UnityEngine;

public class ClickableManager : MonoBehaviour
{
    #region Unity Life Cycle
    private void Update()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        // 모바일에서 터치 감지
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                ProcessClickOrTouch(touch.position);
            }
        }
#else
        // PC에서 마우스 왼쪽 버튼 클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
            ProcessClickOrTouch(Input.mousePosition);
        }
#endif  
    }
    #endregion

    private void ProcessClickOrTouch(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        // 레이캐스트를 쏴서 충돌 감지
        if (Physics.Raycast(ray, out hit))
        {
            // 충돌된 오브젝트에서 ClickableObject 컴포넌트를 가져옴
            ClickableObject clickable = hit.transform.GetComponent<ClickableObject>();
            if (clickable != null)
            {
                // 클릭된 오브젝트의 OnObjectClicked 메서드 호출
                clickable.OnObjectClicked();
            }
        }
    }
}
