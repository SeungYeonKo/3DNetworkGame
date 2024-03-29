using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
[RequireComponent(typeof(CharacterShakeAbility))]
[RequireComponent (typeof(Animator))]

public class Character : MonoBehaviour, IPunObservable, IDamaged
{
    public PhotonView PhotonView { get; private set; }
    public Stat Stat; 

    private Animator _animator; // Animator 컴포넌트를 참조하기 위한 변수
    private CharacterMoveAbility _moveAbility;
    private CharacterAttackAbility _attackAbility;
    private CharacterRotateAbility _rotateAbility;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        Stat.Init();
        PhotonView = GetComponent<PhotonView>();
        _moveAbility = GetComponent<CharacterMoveAbility>(); // 이동 능력 컴포넌트 참조
        _attackAbility = GetComponent<CharacterAttackAbility>(); // 공격 능력 컴포넌트 참조

        if (PhotonView.IsMine)
        {
            UI_CharacterStat.Instance.MyCharacter = this;
        }
    }

    private Vector3 _receivedPosition;
    private Quaternion _receivedRotation;
    private void Update()
    {
        if (!PhotonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, _receivedPosition, Time.deltaTime * 20f);
            transform.rotation = Quaternion.Slerp(transform.rotation, _receivedRotation, Time.deltaTime * 20f);
        }
    }

    // 데이터 동기화를 위해 데이터 전송 및 수신 기능을 가진 약속
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // stream(통로)은 서버에서 주고받을 데이터가 담겨있는 변수
        if (stream.IsWriting)     // 데이터를 전송하는 상황
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(Stat.Health);
            stream.SendNext(Stat.Stamina);
        }
        else if (stream.IsReading) // 데이터를 수신하는 상황
        {
            // 데이터를 전송한 순서와 똑같이 받은 데이터를 캐스팅해야된다.
            _receivedPosition = (Vector3)stream.ReceiveNext();
            _receivedRotation = (Quaternion)stream.ReceiveNext();
            Stat.Health = (int)stream.ReceiveNext();
            Stat.Stamina = (float)stream.ReceiveNext();
        }
        // info는 송수신 성공/실패 여부에 대한 메시지 담겨있다.
    }

    [PunRPC]
    public void Damaged(int damage)
    {
        Stat.Health -= damage;
        GetComponent<CharacterShakeAbility>().Shake();
        if (PhotonView.IsMine)
        {
            CinemachineImpulseSource impulseSource;
            if (TryGetComponent<CinemachineImpulseSource>(out impulseSource))
            {
                float strength = 0.2f;
                impulseSource.GenerateImpulseWithVelocity(UnityEngine.Random.insideUnitSphere.normalized * strength);
            }
            UI_DamagedEffect.Instance.Show(0.5f);
        } 
        if(Stat.Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
            // 죽는 애니메이션 재생
            _animator.SetTrigger("Die");

            // 이동 및 공격 기능 비활성화
            if (_moveAbility != null)
            {
                _moveAbility.enabled = false; // 이동 능력 비활성화
            }
            if (_attackAbility != null)
            {
                _attackAbility.enabled = false; // 공격 능력 비활성화
            }
            if(_rotateAbility != null)
            {
                _rotateAbility.enabled = false;
            }
        // 바닥에 떨어지거나 죽으면 5초 후 체력/스태미나 MAX와  함께 랜덤한 위치에 리스폰
        //StartCoroutine(ReSpawn_Coroutine());
    }

 /*   private IEnumerator ReSpawn_Coroutine()
    {
        yield return new WaitForSeconds(5f);

        Stat.Health = Stat.MaxHealth;
        Stat.Stamina = Stat.MaxStamina;

        // 랜덤 스폰 포인트 찾기
        var spawnPoints = PhotonManager.Instance.RandomSpawnPoints;
        var randomSpawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)].position;

        // 캐릭터를 랜덤 스폰 포인트로 이동
        transform.position = randomSpawnPoint;

        if (_moveAbility != null) _moveAbility.enabled = true;
        if (_attackAbility != null) _attackAbility.enabled = true;
        if (_rotateAbility != null) _rotateAbility.enabled = true;

        _animator.ResetTrigger("Die");
    }*/
}