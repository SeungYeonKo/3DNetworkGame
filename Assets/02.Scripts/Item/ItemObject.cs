using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Collider))]
public class ItemObject : MonoBehaviourPun
{ 
    [Header("아이템 타입")]
    public ItemType ItemType;
    public float Value = 100;
    CoinSpawner coinSpawner;

    private void Start()
    {
        // 아이템 흩뿌리기
        if (photonView.IsMine)
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            Vector3 randomVector = UnityEngine.Random.insideUnitSphere;
            randomVector.y = 1f;
            randomVector.Normalize();
            randomVector *= UnityEngine.Random.Range(3, 7f);
            rigidbody.AddForce(randomVector, ForceMode.Impulse);
        }
        // CoinSpawner 인스턴스 찾기
        coinSpawner = FindObjectOfType<CoinSpawner>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Character character = other.GetComponent<Character>();
            if(!character.PhotonView.IsMine || character.State == State.Death)
            {
                return;
            }
             character.GetComponent<CharacterEffectAbility>().RequestPlay((int)ItemType);
            switch (ItemType)
            {
                case ItemType.HealthPotion:
                {
                    character.Stat.Health += (int)Value;
                    if (character.Stat.Health >= character.Stat.MaxHealth)
                    {
                        character.Stat.Health = character.Stat.MaxHealth;
                    }
                 
                    break;
                }
                case ItemType.StaminaPotion:
                {
                    character.Stat.Stamina += Value;
                    if (character.Stat.Stamina > character.Stat.MaxStamina)
                    {
                        character.Stat.Stamina = character.Stat.MaxStamina;
                    }
                    break;
                }
                case ItemType.Coin:
                case ItemType.Coin2:
                case ItemType.Coin3:
                {
                    //character.Score += (int)Value;
                    character.AddScore((int)Value);
                    break;
                }
            }
            // 삭제하기 전에 꺼야한다
            gameObject.SetActive(false);
            ItemObjectFactory.Instance.RequestDelete(photonView.ViewID);
        }
    }
}
