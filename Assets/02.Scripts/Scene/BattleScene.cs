using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class BattleScene : MonoBehaviourPunCallbacks
{
    public static BattleScene Instance { get; private set; }


    public List<Transform> RandomSpawnPoint;

    private bool _init = false; //초기화를 하지 않았다

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!_init)
        {
            Init();
        }
    }

    public void Init()
    {
        _init = true;
        Debug.Log("게임 시작! '0'");
        PhotonNetwork.Instantiate(nameof(Character), Vector3.zero, Quaternion.identity);

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        GameObject[] postints = GameObject.FindGameObjectsWithTag("PatrolPoint");
        foreach (GameObject p in postints)
        {
            PhotonNetwork.InstantiateRoomObject("Bear", p.transform.position, Quaternion.identity);
        }
    }


    public Vector3 GetRandomSpawnPoint()
    {
        int randomIndex = UnityEngine.Random.Range(0, RandomSpawnPoint.Count);
        return RandomSpawnPoint[randomIndex].position;
    }
    public override void OnJoinedRoom()
    {
        if (!_init)
        {
            Init();
        }
    }
}
