using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Net.Http.Headers;

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
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private BearState _currentState = BearState.Idle;

    // 체력
    public int Health;
    public int MaxHealth = 100;
    //public Slider HealthSliderUI;

    // 이동
    public float MoveSpeed = 15f;
    public Vector3 StartPosition;

    // 공격
    public int Damage = 15;
    public const float AttackDelay = 1f;
    private float _attackTimer = 0f;

    // AI
    private Transform _target;
    public float FindDistance = 20f;
    public float AttackDistance = 7f;
    public float MoveDistance = 40f;
    public const float TOLERANCE = 0.1f;
    private const float IDLE_DURATION = 3f;
    private float _idleTimer;

    private bool _isTargetSet = false;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        StartPosition = transform.position;
        Init();
    }

    public void Init()
    {
        _idleTimer = 0f;
        Health = MaxHealth;
    }

    private void Update()
    {
        //HealthSliderUI.value = (float)Health / (float)MaxHealth;
        if (!_isTargetSet)
        {
            TrySetTarget();
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
            _target = player.transform;
            _isTargetSet = true;
            Init(); // 타겟이 설정된 후 초기화 작업을 수행
        }
    }

    private void Idle()
    {
        _idleTimer += Time.deltaTime;
        if(_idleTimer > IDLE_DURATION)
        {
            _idleTimer = 0f;
            _animator.SetTrigger("IdleToPatrol");
            _currentState = BearState.Patrol;
        }
    }

    private void Patrol()
    {
        // 플레이어가 감지 범위 내에 있으면 상태를 Trace로 변경하여 플레이어를 추적
        if (Vector3.Distance(_target.position, transform.position) <= FindDistance)
        {
            _animator.SetTrigger("PatrolToTrace");
            _currentState = BearState.Trace;
        }

        // 추가: Patrol 상태에서 일정 시간 대기 후 Comeback으로 전환
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= TOLERANCE)
        {
            StartCoroutine(WaitAndComeback());
        }
    }

    private IEnumerator WaitAndComeback()
    {
        yield return new WaitForSeconds(2f);  // 대기 시간 조절 필요
        _animator.SetTrigger("PatrolToComeback");
        _currentState = BearState.Comeback;
    }

    private void Trace()
    {
        Vector3 dir = _target.transform.position - this.transform.position;
        dir.y = 0;
        dir.Normalize();

        _navMeshAgent.stoppingDistance = AttackDistance;
        _navMeshAgent.destination = _target.position;

        // 플레이어와의 거리가 공격 범위 내에 있는지 확인
        if (Vector3.Distance(_target.position, transform.position) <= AttackDistance)
        {
            // 공격 범위 내에 있으면 Attack 상태로 전환
            if (_currentState != BearState.Attack)   // 현재 상태가 Attack이 아닐 때만 전환
            {
                _animator.SetTrigger("TraceToAttack");
                _currentState = BearState.Attack;
            }
        }
        else if (Vector3.Distance(_target.position, transform.position) >= FindDistance)
        {
            // 플레이어와의 거리가 찾기 범위를 벗어나면 Comeback 상태로 전환
            _animator.SetTrigger("TraceToComeback");
            _currentState = BearState.Comeback;
        }
    }

    private void Attack()
    {
        float distanceToTarget = Vector3.Distance(_target.position, transform.position);
        _attackTimer += Time.deltaTime;
        if(_attackTimer >= AttackDelay) 
        {
            if (distanceToTarget <= AttackDistance)
            {
                _animator.SetTrigger("Attack");
            }
            _attackTimer = 0;
        }
        if (distanceToTarget > AttackDistance || distanceToTarget > FindDistance)
        {
            _attackTimer = 0f;
            _animator.SetTrigger("AttackToTrace");
            _currentState = BearState.Trace;
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
            _animator.SetTrigger("ComebackToIdle");

            _currentState = BearState.Idle;
        }
    }

    private void Damaged()
    {
 
    }

    private Coroutine _dieCoroutine;
    private void Die()
    {
        _animator.SetTrigger("Die");

        if (_dieCoroutine == null)
        {
            _dieCoroutine = StartCoroutine(Die_Coroutine());
        }

    }
    private IEnumerator Die_Coroutine()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();

        //HealthSliderUI.gameObject.SetActive(false);
        Destroy(gameObject);
        yield return new WaitForSeconds(3f);
    }

}
