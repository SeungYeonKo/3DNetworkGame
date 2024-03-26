using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttackAbility : CharacterAbility
{
    // SOLID 법칙: 객체지향 5가지 법칙
    // 1. 단일 책임 원칙 ( 가장 단순하지만 꼭 지켜야 하는 원칙)
    // - 클래스는 단 한개의 책임을 가져야 한다.
    // - 클래스를 변경하는 이유는 단 하나여야 한다.
    // - 이를 지키지 않으면 한 책임 변경에 의해 다른 책임과 관련된 코드도 영향을 미칠 수 있어서
    // -> 유지보수가 매우 어렵다
    // 준수 전략
    // - 기존의 클래스로 해결할 수 없다면 새로운 클래스를 구현

    private Animator _animator;
    private float _attackTimer;
    public float StaminaConsumeSpeed = 20f;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        _attackTimer += Time.deltaTime;

        bool haveStamina = Owner.Stat.Stamina >= Owner.Stat.AttackConsumeStamina;
        if (Input.GetMouseButtonDown(0) && _attackTimer >= Owner.Stat.AttackCoolTime && haveStamina)
        {
            _attackTimer = 0;
            _animator.SetTrigger($"Attack{Random.Range(1, 4)}");
        }
    }
}
