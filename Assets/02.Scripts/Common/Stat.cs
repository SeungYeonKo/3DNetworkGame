using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]  //직렬화
public class Stat
{
    public int Damage;

    public int    Health;
    public int    MaxHealth;

    public float Stamina;
    public float MaxStamina;
    public float RunConsumeStamina;
    public float RecoveryStamina;

    public float MoveSpeed;
    public float RunSpeed;

    public float RotationSpeed;
    public float AttackCoolTime;
    public float AttackConsumeStamina;

    public void Init()
    {
        Health = MaxHealth;
        Stamina = MaxStamina;
    }
}
