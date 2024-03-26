using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class CharacterMoveAbility : CharacterAbility
{
    // 목표: [W],[A],[S],[D] 및 방향키를 누르면 캐릭터를 그 뱡향으로 이동시키고 싶다.
    private CharacterController _characterController;
    private Animator _animator;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        Owner.Stat.Health = Owner.Stat.MaxHealth;
        Owner.Stat.Stamina = Owner.Stat.MaxStamina;
    }

    private void Update()
    {
        // 순서
        // 1. 사용자의 키보드 입력을 받는다.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. '캐릭터가 바라보는 방향'을 기준으로 방향을 설정한다.
        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();
        dir = Camera.main.transform.TransformDirection(dir);

        _animator.SetFloat("Move", dir.magnitude);

        // 3. 중력 적용하세요.
        dir.y = -1f;

        float moveSpeed = Owner.Stat.MoveSpeed;
       

        if (Input.GetKey(KeyCode.LeftShift) && Owner.Stat.Stamina > 0 )
        {
            moveSpeed = Owner.Stat.RunSpeed;
            Owner.Stat.Stamina -= Time.deltaTime * Owner.Stat.RunConsumeStamina;
        }
        else
        {
            Owner.Stat.Stamina += Time.deltaTime * Owner.Stat.RecoveryStamina;
        }

        // 4. 이동속도에 따라 그 방향으로 이동한다.
        _characterController.Move(dir * (moveSpeed * Time.deltaTime));
    }
}