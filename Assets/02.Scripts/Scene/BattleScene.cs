using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class BattleScene : MonoBehaviourPunCallbacks
{
   public static BattleScene Instance { get; private set; }

    public List<Transform> RandomSpawnPoint;

    private void Awake()
    {
        Instance = this;
    }

    // 플레이어 랜덤 포인트 생성
    public Vector3 GetRandomSpawnPoint()
    {
        int randomIndex = UnityEngine.Random.Range(0, RandomSpawnPoint.Count);
        return RandomSpawnPoint[randomIndex].position;
    }
}
