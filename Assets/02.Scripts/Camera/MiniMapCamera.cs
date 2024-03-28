using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    public Transform Target;   // 타겟이 플레이어
    public float YDistance = 50f;  // 오프셋 적용 .
    private Vector3 _initialEulerAngles;  // 각도 회전할때 이용, 플레이어 위치에 맞게 회전

    private void Start()
    {
        _initialEulerAngles = transform.eulerAngles;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = Target.position;
        targetPosition.y += YDistance;

        transform.position = targetPosition;

        Vector3 targetEulerAngles = Target.eulerAngles;
        targetEulerAngles.x = _initialEulerAngles.x;
        targetEulerAngles.z = _initialEulerAngles.z;
        transform.eulerAngles = targetEulerAngles;
    }
}
