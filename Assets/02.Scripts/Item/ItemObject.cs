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

    [Header("아이템 이펙트")]
    public GameObject HealthItemEffect;
    public GameObject StaminaItemEffect;
    public GameObject CoinItemEffect;


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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Character character = other.GetComponent<Character>();
            if(character.State == State.Death)
            {
                return;
            }
            GameObject effectToSpawn = null; // 생성할 이펙트를 저장할 변수

            switch (ItemType)
            {
                case ItemType.HealthPotion:
                {
                    character.Stat.Health += (int)Value;
                    if (character.Stat.Health >= character.Stat.MaxHealth)
                    {
                        character.Stat.Health = character.Stat.MaxHealth;
                    }
                    effectToSpawn = HealthItemEffect; 
                    break;
                }
                case ItemType.StaminaPotion:
                {
                    character.Stat.Stamina += Value;
                    if (character.Stat.Stamina > character.Stat.MaxStamina)
                    {
                        character.Stat.Stamina = character.Stat.MaxStamina;
                    }
                    effectToSpawn = StaminaItemEffect; 
                    break;
                }
                case ItemType.Coin:
                {
                    character.Score += 1;
                    effectToSpawn = CoinItemEffect;
                    break;
                }
            }
            // 이펙트 생성
            if (effectToSpawn != null)
            {
                Vector3 effectPosition = other.transform.position + new Vector3(0, other.bounds.extents.y, 0);
                Instantiate(effectToSpawn, effectPosition, Quaternion.identity);
            }
            // 삭제하기 전에 꺼야한다
            gameObject.SetActive(false);
            ItemObjectFactory.Instance.RequestDelete(photonView.ViewID);
        }
    }
}
