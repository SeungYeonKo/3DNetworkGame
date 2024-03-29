using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class CharacterMoveAbility : CharacterAbility
{
    // 목표: [W],[A],[S],[D] 및 방향키를 누르면 캐릭터를 그 뱡향으로 이동시키고 싶다.
    private CharacterController _characterController;
    private Animator _animator;

    protected  override void Awake()    //characterAbility에서 이미 사용중이어서 override를 써줘서 재정의
    {
        base.Awake();   // characterAbility에 있는 awake에 awake를 추가

        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }


    private void Update()
    {
        if (_owner.State == State.Death || !_owner.PhotonView.IsMine)
        {
            return;
        }

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

        float moveSpeed = _owner.Stat.MoveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && _owner.Stat.Stamina > 0)
        {
            moveSpeed = _owner.Stat.RunSpeed;
            _owner.Stat.Stamina -= Time.deltaTime * _owner.Stat.RunConsumeStamina;
        }
        else
        {
            _owner.Stat.Stamina += Time.deltaTime * _owner.Stat.RecoveryStamina;
            if (_owner.Stat.Stamina >= _owner.Stat.MaxStamina)
            {
                _owner.Stat.Stamina = _owner.Stat.MaxStamina;
            }
        }

        // 4. 이동속도에 따라 그 방향으로 이동한다.
        _characterController.Move(dir * (moveSpeed * Time.deltaTime));
    }

    public void Teleport(Vector3 position)
    {
        _characterController.enabled = false; //

        transform.position = position;

        _characterController.enabled = true;
    }
}