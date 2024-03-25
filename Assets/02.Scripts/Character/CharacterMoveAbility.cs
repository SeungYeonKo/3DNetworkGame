using UnityEngine;

[RequireComponent (typeof(CharacterController))]

public class CharacterMoveAbility : MonoBehaviour
{
    // 목표 : [W], [A], [S], [D] 및 방향키를 누르면 캐릭터를 그 방향으로 이동시키고 싶다
    private CharacterController _characterController;
    public float MoveSpeed = 5f;
    private float _gravity = -20;
    public float _yVelocity = 0f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // 순서
        // 1. 사용자의 키보드 입력을 받는다
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v);
        Vector3 unNormalizedDir = dir;
        dir.Normalize();
       
        // 2. '캐릭터가 바라보는 방향'을 기준으로 방향을 설정한다

        // 3. 이동 속도에 따라 그 방향으로 이동한다
        
    }
}
