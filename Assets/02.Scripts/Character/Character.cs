using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
[RequireComponent(typeof(CharacterShakeAbility))]
public class Character : MonoBehaviour, IPunObservable, IDamaged
{
    public PhotonView PhotonView { get; private set; }
    public Stat Stat;
    public State State { get; private set; } = State.Live;
    private Vector3 _receivedPosition;
    private Quaternion _receivedRotation;

    private void Awake()
    {
        Stat.Init();
        PhotonView = GetComponent<PhotonView>();

        if (PhotonView.IsMine)
        {
            UI_CharacterStat.Instance.MyCharacter = this;
        }
    }

    private void Start()
    {
        SetRandomPositionAndRotate(); //
    }

    private void Update()
    {
        if (!PhotonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, _receivedPosition, Time.deltaTime * 50f);
            transform.rotation = Quaternion.Slerp(transform.rotation, _receivedRotation, Time.deltaTime * 50f);

        }
    }

    // transform view를  지우고, transform view가 하는 역할을 직접 만듦
    // 데이터 동기화를 위해 데이터 전송 및 수신 기능
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // stream은 서버에서 주고받을 데이터가 담겨있는 변수
        if (stream.IsWriting)               // 데이터를 전송하는 상황
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(Stat.Health);
            stream.SendNext(Stat.Stamina);
        }
        else if (stream.IsReading)      // 데이터를 수신하는 상황
        {
            // 데이터를 전송한 순서와 똑같이 받은 데이터를 캐스팅해야한다.
            _receivedPosition = (Vector3)stream.ReceiveNext();
            _receivedRotation = (Quaternion)stream.ReceiveNext();
            Stat.Health = (int)stream.ReceiveNext();
            Stat.Stamina = (float)stream.ReceiveNext();
        }
        // info는 데이터 송수신 성공/실패 여부에 대한 메시지가 담겨있다
    }

    [PunRPC]
    public void Damaged(int damage)
    {
        if (State == State.Death)
        {
            return;
        }
        Stat.Health -= damage;
        if (Stat.Health <= 0)
        {
            State = State.Death;
            PhotonView.RPC(nameof(Death), RpcTarget.All);
        }

        GetComponent<CharacterShakeAbility>().Shake();
        if (PhotonView.IsMine)
        {
            OnDamagedMine();
        }

    }

    private void OnDamagedMine()
    {


        if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource cinemachineImpulseSource))
        {
            float strength = 0.4f;
            cinemachineImpulseSource.GenerateImpulseWithVelocity(UnityEngine.Random.insideUnitSphere.normalized * strength);
        }

        UI_DamagedEffect.Instance.Show(0.5f);
    }

    [PunRPC]
    public void Death()
    {
        GetComponent<Animator>().SetTrigger("Death");
        GetComponent<CharacterAttackAbility>().InactiveCollider();

        // 죽고나서 5초후 리스폰

        if (PhotonView.IsMine)
        {
            StartCoroutine(Death_Coroutine());
        }
    }

    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(5f);

        SetRandomPositionAndRotate();

        PhotonView.RPC(nameof(Live), RpcTarget.All);
    }

    [PunRPC]
    private void Live()
    {
        State = State.Live;
        Stat.Init();
        GetComponent<Animator>().SetTrigger("Live");
    }

    private void SetRandomPositionAndRotate()
    {
        Vector3 spawnPoint = BattleScene.Instance.GetRandomSpawnPoint();
        GetComponent<CharacterMoveAbility>().Teleport(spawnPoint); //
        GetComponent<CharacterRotateAbility>().SetRandomRotation();
    }

   /* public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DeathZone"))
        {
            if (PhotonView.IsMine)
            {
                // 태그가 "DeathZone"인 객체와 충돌 시 Health를 0으로 설정
                PhotonView.RPC(nameof(Damaged), RpcTarget.All, Stat.MaxHealth);
            }
        }
    }*/
}

