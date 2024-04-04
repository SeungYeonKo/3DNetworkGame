using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CharacterAttackAbility : CharacterAbility
{
    // SOLID 법칙: 객체지향 5가지 법칙
    // 1. 단일 책임 원칙 (가장 단순하지만 꼭 지켜야 하는 원칙)
    // - 클래스는 단 한개의 책임을 가져야 한다.
    // - 클래스를 변경하는 이유는 단 하나여야 한다.
    // - 이를 지키지 않으면 한 책임 변경에 의해 다른 책임과 관련된 코드도 영향을 미칠 수 있어서
    //    -> 유지보수가 매우 어렵다.
    // 준수 전략
    // - 기존의 클래스로 해결할 수 없다면 새로운 클래스를 구현
    // 때린 애들을 기억해 놓는 리스트
    private List<IDamaged> _damagedList = new List<IDamaged>();

    private Animator _animator;
    private float _attackTimer = 0;

    public Collider WeaponCollider;
    public GameObject WeaponObject;

    // 1000점마다 무기 크기 증가
    [PunRPC]
    public void RefreshWeaponScale()
    {
        int score = _owner.GetPropertyIntValue("Score");
        float scale = 1f;
        scale += (score / 1000) * 0.1f;

        WeaponObject.transform.localScale = new Vector3(scale, scale, scale);
    }


    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_owner.State == State.Death || !_owner.PhotonView.IsMine)
        {
            return;
        }

        _attackTimer += Time.deltaTime;

        bool haveStamina = _owner.Stat.Stamina >= _owner.Stat.AttackConsumeStamina;
        if (Input.GetMouseButtonDown(0) && _attackTimer >= _owner.Stat.AttackCoolTime && haveStamina)
        {
            _owner.Stat.Stamina -= _owner.Stat.AttackConsumeStamina;

            _attackTimer = 0f;

            if (GetComponent<CharacterMoveAbility>().IsJumping)
            {
                _owner.PhotonView.RPC(nameof(PlayAttackAnimation), RpcTarget.All, 4);
            }
            else
            {
                _owner.PhotonView.RPC(nameof(PlayAttackAnimation), RpcTarget.All, Random.Range(1, 4));
            }
            // RpcTarget.All     : 모두에게
            // RpcTarget.Others  : 나 자신을 제외하고 모두에게
            // RPcTarget.Master  : 방장에게만
        }
    }

    [PunRPC]
    public void PlayAttackAnimation(int index)
    {
        _animator.SetTrigger($"Attack{index}");
    }

    public void OnTriggerEnter(Collider other)
    {
        if (_owner.PhotonView.IsMine == false || other.transform == transform)
        {
            return;
        }

        // O: 개방 폐쇄 원칙 + 인터페이스 
        // 수정에는 닫혀있고, 확장에는 열려있다.
        IDamaged damagedAbleObject = other.GetComponent<IDamaged>();
        if (damagedAbleObject != null)
        {
            // 내가 이미 때렸던 애라면 안때리겠다...
            if (_damagedList.Contains(damagedAbleObject))
            {
                return;
            }
            // 안 맞은 애면 때린 리스트에 추가
            _damagedList.Add(damagedAbleObject);

            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null)
            {
                // 피격 이펙트 생성
                Vector3 hitPosition = (transform.position + other.transform.position) / 2f + new Vector3(0f, 1f);
                PhotonNetwork.Instantiate("HitEffect", hitPosition, Quaternion.identity);
                photonView.RPC("Damaged", RpcTarget.All, _owner.Stat.Damage, _owner.PhotonView.OwnerActorNr);
            }
            //damagedAbleObject.Damaged(_owner.Stat.Damage);
        }
    }

    public void ActiveCollider()
    {
        WeaponCollider.enabled = true;
    }
    public void InactiveCollider()
    {
        WeaponCollider.enabled = false;
        _damagedList.Clear();
    }
}