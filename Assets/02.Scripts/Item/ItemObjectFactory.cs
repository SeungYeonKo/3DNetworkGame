using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ItemObjectFactory : MonoBehaviourPun
{
   public static ItemObjectFactory Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

    }

    public ItemObject MasterCreate(ItemType type, Vector3 position)
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("마스터 클라이언트만 호출할 수 있습니다");
            return null;
        }
        return Create(type, position);
    }

    public void RequestCreate(ItemType type, Vector3 position)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            Create(type, position);
        }
        else
        {
            photonView.RPC(nameof(Create), RpcTarget.MasterClient, type, position);
        }
    }

    // 아이템 생성 및 삭제
    [PunRPC]
    private ItemObject  Create(ItemType type, Vector3 position)
    {
        Vector3 dropPos = position + new Vector3(0.5f, 0.5f, 0f) + UnityEngine.Random.insideUnitSphere;
        GameObject gameObject = PhotonNetwork.InstantiateRoomObject("Item/" + type.ToString(), dropPos, Quaternion.identity);    
        // 폴더 안에 있는 프리팹을 불러올땐 => "폴더이름/" + 

        return gameObject.GetComponent<ItemObject>();
    }

    public void RequestDelete(int viewID)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            Delete(viewID);
        }
        else
        {
            photonView.RPC(nameof(Delete), RpcTarget.MasterClient, viewID);
        }
    }


    [PunRPC]
    private void Delete(int viewID)
    {
        // photonview.find(viewID)로 게임 오브젝트를 검색해오는 기능
        GameObject objectToDelete = PhotonView.Find(viewID)?.gameObject; 
        if(objectToDelete != null)
        {
            PhotonNetwork.Destroy(objectToDelete);
        }
    }
}
