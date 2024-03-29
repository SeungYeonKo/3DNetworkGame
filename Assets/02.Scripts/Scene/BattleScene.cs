using System.Collections.Generic;
using UnityEngine;

public class BattleScene : MonoBehaviour
{
   public static BattleScene Instance { get; private set; }

    public List<Transform> RandomSpawnPoint;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetRandomSpawnPoint()
    {
        int randomIndex = UnityEngine.Random.Range(0, RandomSpawnPoint.Count);
        return RandomSpawnPoint[randomIndex].position;
    }
}
