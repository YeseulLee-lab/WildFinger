using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCameraMove : MonoBehaviour
{
    private float _time;

    void Update()
    {
        _time += Time.deltaTime;
        // 현재 카메라의 Y 값 회전을 얻어옵니다.

        this.transform.localRotation = Quaternion.Euler(0f, _time * 1.5f, 0f);
    }
}
