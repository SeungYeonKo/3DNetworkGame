using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
public class Character : MonoBehaviour, IPunObservable
{
    public PhotonView PhotonView { get; private set; }
    public Stat Stat;

    private void Awake()
    {
        Stat.Init();

        PhotonView = GetComponent<PhotonView>();

        if (PhotonView.IsMine)
        {
            UI_CharacterStat.Instance.MyCharacter = this;
        }
    }
    // 트랜스폼뷰를 지우고, 트랜스폼 뷰가 하는 역할을 직접 만듦
    // 데이터 동기화를 위해 데이터 전송 및 수신 기능
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // stream은 서버에서 주고받을 데이터가 담겨있는 변수
        if (stream.IsWriting)               // 데이터를 전송하는 상황
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if (stream.IsReading)      // 데이터를 수신하는 상황
        {
            // 데이터를 전송한 순서와 똑같이 받은 데이터를 캐스팅해야한다.
            Vector3 receivedPosition = (Vector3)stream.ReceiveNext();
            Quaternion receivedRotation = (Quaternion)stream.ReceiveNext();

            if (!PhotonView.IsMine)
            {
                transform.position = receivedPosition;
                transform.rotation = receivedRotation;
            }
        }
        // info는 데이터 송수신 성공/실패 여부에 대한 메시지가 담겨있다
    }
}

