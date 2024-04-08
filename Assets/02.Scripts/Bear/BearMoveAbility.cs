using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Net.Http.Headers;
using Photon.Pun;

public enum BearState
{
    Idle,
    Patrol,
    Trace,
    Attack,
    Comeback,
    Damaged,
    Die
}


public class BearMoveAbility : MonoBehaviour
{ 
    private BearState _currentState = BearState.Idle;


    private NavMeshAgent _navMeshAgent;
    public Animator BearAnimator;
    public Transform PatrolTarget;

    private List<Character> _characterList = new List<Character>();
    public SphereCollider CharacterDetectCollider;
    private Character _targetCharacter;
    

    public Vector3 StartPosition;
    public Slider HealthSliderUI;
    public Stat Stat;

    private bool _isTargetSet = false;
    // AI
    // [IDLE]  
    private float _idleTime;
    private const float IDLE_MAX_TIME = 5f;

    public float MoveDistance = 40f;
    public float FindDistance = 20f;

    // [Attack]
    private float _attackTimer = 0f;
    private float AttackDelay = 0.5f;
    public float AttackDistance = 3f;
    
    public const float TOLERANCE = 0.1f;
    public float ComebackFindPlayerDistance = 10f;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        CharacterDetectCollider.radius = FindDistance;
        StartPosition = transform.position;
        _navMeshAgent.speed = Stat.MoveSpeed;
        Init();
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            Character character = col.GetComponent<Character>();
            if (!_characterList.Contains(character))
            {
                Debug.Log("새로운 인간을 찾았다 !");
                _characterList.Add(character);
            }
        }
    }

    public void Init()
    {
        _idleTime = 0f;
        Stat.Health = Stat.MaxHealth;
    }

    private void Update()
    {
        if (!_isTargetSet)
        {
            TrySetTarget();
            return;
        }
        // _targetCharacter이 null이 아닌지 확인
        if (_targetCharacter != null)
        {
            float distanceToPlayer = Vector3.Distance(_targetCharacter.transform.position, transform.position);
            HealthSliderUI.gameObject.SetActive(distanceToPlayer <= FindDistance);
            HealthSliderUI.value = (float)Stat.Health / (float)Stat.MaxHealth;
        }
        else
        {
            // _targetCharacter가 null이면, HealthSliderUI를 비활성화
            HealthSliderUI.gameObject.SetActive(false);
        }

        if (_currentState == BearState.Trace || _currentState == BearState.Attack)
        {
            if (_targetCharacter != null) // 여기도 검사 추가
            {
                LookAtPlayerSmoothly();
            }
        }
        // 조기 반환문
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        switch (_currentState)
        {
            case BearState.Idle: Idle(); break;
            case BearState.Patrol: Patrol(); break;
            case BearState.Trace: Trace(); break;
            case BearState.Attack: Attack(); break;
            case BearState.Comeback: Comeback(); break;
            case BearState.Damaged: Damaged(); break;
            case BearState.Die: Die(); break;
        }
    }

    private void TrySetTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _targetCharacter = player.GetComponent<Character>();
            if (_targetCharacter != null)
            {
                _isTargetSet = true;
                Init(); // 타겟이 설정된 후 초기화 작업을 수행
            }
        }
    }

    private void Idle()
    {
        _idleTime += Time.deltaTime;
        if(_idleTime >= IDLE_MAX_TIME)
        {
            _idleTime = 0f;
            Debug.Log("Idle->Patrol");
            BearAnimator.SetTrigger("IdleToPatrol");
            _currentState = BearState.Patrol;
        }
        if (_targetCharacter != null && Vector3.Distance(_targetCharacter.transform.position, transform.position) <= FindDistance)
        {
            Debug.Log("Idle -> Trace");
            BearAnimator.SetTrigger("IdleToTrace");
            _currentState = BearState.Trace;
        }
    }

    private void Patrol()
    {
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= TOLERANCE)
        {
            // 새로운 랜덤 위치 설정
            Vector3 newPatrolPoint = RandomNavSphere(StartPosition, MoveDistance, -1);
            _navMeshAgent.SetDestination(newPatrolPoint);
        }

        if (Vector3.Distance(_targetCharacter.transform.position, transform.position) <= FindDistance)
        {
            Debug.Log("Patrol -> Trace");
            BearAnimator.SetTrigger("PatrolToTrace");
            _currentState = BearState.Trace;
        }
    }

    // NavMesh 위에 지정된 반경 내에서 랜덤한 위치를 반환
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    private void Trace()
    {
        if(_targetCharacter == null)
        {
            _currentState = BearState.Comeback;
            return;
        }

        Vector3 dir = _targetCharacter.transform.position - this.transform.position;
        dir.y = 0;
        dir.Normalize();

        _navMeshAgent.stoppingDistance = AttackDistance;
        _navMeshAgent.destination = _targetCharacter.transform.position;

        // 플레이어와의 거리가 공격 범위 내에 있는지 확인
        if (Vector3.Distance(_targetCharacter.transform.position, transform.position) <= AttackDistance)
        {
            Stat.MoveSpeed = Stat.RunSpeed;
            if (_currentState != BearState.Attack)   // 현재 상태가 Attack이 아닐 때만 전환
            {
                BearAnimator.SetTrigger("TraceToAttack");
                _currentState = BearState.Attack;
            }
        }
        else if (Vector3.Distance(_targetCharacter.transform.position, transform.position) >= MoveDistance)
        {
            Debug.Log("Trace->Comeback");
            BearAnimator.SetTrigger("TraceToComeback");
            _currentState = BearState.Comeback;
        }
    }

    private void Attack()
    {
        // 타겟이 게임에서 나가면 복귀
        if (_targetCharacter == null)
        {
            Debug.Log("Attack -> Comeback");
            _navMeshAgent.isStopped = false;
            StartPosition = transform.position;
            _currentState = BearState.Comeback;
            return;
        }

        // 타겟이 죽거나 공격 범위에서 벗어나면 복귀
        _navMeshAgent.destination = _targetCharacter.transform.position;
        if (_targetCharacter.State == State.Death || GetDistance(_targetCharacter.transform) > AttackDistance)
        {
            Debug.Log("Trace -> Comeback");
            _navMeshAgent.isStopped = false;
            StartPosition = transform.position;
            _currentState = BearState.Comeback;
            return;
        }


    }
        private void Comeback()
        {
            Vector3 dir = StartPosition - this.transform.position;
            dir.y = 0;
            dir.Normalize();

            _navMeshAgent.stoppingDistance = TOLERANCE;
            _navMeshAgent.destination = StartPosition;

            if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= TOLERANCE)
            {
                Debug.Log("Comeback -> idle");
                BearAnimator.SetTrigger("ComebackToIdle");
                _currentState = BearState.Idle;
            }
            if (Vector3.Distance(_targetCharacter.transform.position, transform.position) <= ComebackFindPlayerDistance)
            {
                Debug.Log("Comeback -> Trace");
                BearAnimator.SetTrigger("ComebackToTrace");

                _currentState = BearState.Trace;
            }
        }
    
    [PunRPC]
    public void Damaged()
    {
        Debug.Log("Damaged -> Trace");
        BearAnimator.SetTrigger("DamagedToTrace");
        _currentState = BearState.Trace;   
    }
  
    private Coroutine _dieCoroutine;
    private void Die()
    {
        BearAnimator.SetTrigger("Die");

        if (_dieCoroutine == null)
        {
            _dieCoroutine = StartCoroutine(Die_Coroutine());
        }

    }
    private IEnumerator Die_Coroutine()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();

        HealthSliderUI.gameObject.SetActive(false);
        Destroy(gameObject);
        yield return new WaitForSeconds(3f);
    }

    void LookAtPlayerSmoothly()
    {
        Vector3 directionToTarget = _targetCharacter.transform.position - transform.position;
        directionToTarget.y = 0; //수평 회전만
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5); // 회전 속도 조절
    }

    // 나와의 거리가 distance보다 짧은 플레이어를 반환
    private Character FindTarget(float distance)
    {
        _characterList.RemoveAll(c => c == null);

        Vector3 myPosition = transform.position;
        foreach (Character character in _characterList)
        {
            if (character.State == State.Death)
            {
                continue;
            }

            if (Vector3.Distance(character.transform.position, myPosition) <= distance)
            {
                return character;
            }
        }

        return null;
    }
    private List<Character> FindTargets(float distance)
    {
        _characterList.RemoveAll(c => c == null);

        List<Character> characters = new List<Character>();

        Vector3 myPosition = transform.position;
        foreach (Character character in _characterList)
        {
            if (character.State == State.Death)
            {
                continue;
            }

            if (Vector3.Distance(character.transform.position, myPosition) <= distance)
            {
                characters.Add(character);
            }
        }

        return characters;
    }


    private float GetDistance(Transform otherTransform)
    {
        return Vector3.Distance(transform.position, otherTransform.position);
    }


    public void AttackAction()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        Debug.Log("AttackAction!");
        // 일정 범위 안에 있는 모든 플레이어에게 데미지를 주고 싶다.
        List<Character> targets = FindTargets(AttackDistance + 0.1f);
        foreach (Character target in targets)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            int viewAngle = 160 / 2;
            float angle = Vector3.Angle(transform.forward, dir);
            Debug.Log(angle);
            if (Vector3.Angle(transform.forward, dir) < viewAngle)
            {
                target.PhotonView.RPC("Damaged", RpcTarget.All, Stat.Damage, -1);
            }
        }
    }

    private void RequestPlayAnimation(string animationName)
    {
        GetComponent<PhotonView>().RPC(nameof(PlayAnimation), RpcTarget.All, animationName);
    }

    [PunRPC]
    private void PlayAnimation(string animationName)
    {
        BearAnimator.SetTrigger(animationName);
    }
}