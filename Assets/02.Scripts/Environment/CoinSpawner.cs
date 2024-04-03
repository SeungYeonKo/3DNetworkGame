using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    // 시간
    private float _currentTime;
    private float _createTime;
    public float MinCreateTime = 10f;
    public float MaxCreateTime = 40f;
    // 램덤 개수
    private int _createCount;
    public int MinCreateCount = 10;
    public int MaxCreateCount = 30;

    // 생성한 아이템
    private List<ItemObject> _items = new List<ItemObject>();

    private void Start()
    {
        _createTime = 1f;
    }

    private void Update()
    {
        // 방장 외에 생성하면 안되기 때문에 
        // 방장이 아니라면 return하겠다
            _items.RemoveAll(i => i == null || !i.isActiveAndEnabled); // 아이템이 null이면 삭제하겠다
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            _currentTime += Time.deltaTime;
            if (_currentTime >= _createTime)
            {
                if (_items.Count >= MaxCreateCount)
                {
                    return;
                }
                _createCount = UnityEngine.Random.Range(MinCreateCount, MaxCreateCount);

                for (int i = 0; i < _createCount; i++)
                {
                    Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-10, 10), 1, UnityEngine.Random.Range(-10, 10)) + transform.position;

                    // 코인 타입 랜덤 선택
                    ItemType randomCoinType = GetRandomCoinType();

                    ItemObject itemObject = ItemObjectFactory.Instance.MasterCreate(randomCoinType, randomPosition);
                    _items.Add(itemObject);
                }
                // 시간 초기화
                _currentTime = 0f;
                // 생성할 시간을 다시 랜덤
                _createTime = UnityEngine.Random.Range(MinCreateTime, MaxCreateTime);
            }
        }
    
        // 코인 타입을 랜덤으로 반환하는 메소드 추가
        private ItemType GetRandomCoinType()
        {
            ItemType[] coinTypes = { ItemType.Coin, ItemType.Coin2, ItemType.Coin3 };
            int randomIndex = UnityEngine.Random.Range(0, coinTypes.Length);
            return coinTypes[randomIndex];
        }
    }