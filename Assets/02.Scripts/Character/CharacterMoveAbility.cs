using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMoveAbility : MonoBehaviour
{
    // 목표: [W],[A],[S],[D] 및 방향키를 누르면 캐릭터를 그 방향으로 이동시키고 싶다.
    private CharacterController _characterController;
    private float _gravity = -20;
    private float _yVelocity = 0;
    private float moveSpeed = 5f;
   
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 순서
        // 1. 사용자의 키보드 입력을 받는다.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        // 2. '캐릭터가 바라보는 방향'을 기준으로 방향을 설정한다.
        Vector3 dir = new Vector3(x: h, y: 0, z: v); 
        Vector3 unNormalizedDir = dir;
        dir.Normalize();
        dir = Camera.main.transform.TransformDirection(dir);
        // 3. 이동속도에 따라 그방향으로 이동한다.
        _yVelocity += _gravity * Time.deltaTime;
        dir.y = -1;
        _characterController.Move(dir * moveSpeed * Time.deltaTime);
    }
}
