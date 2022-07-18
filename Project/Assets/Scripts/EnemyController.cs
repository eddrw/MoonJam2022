using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IAttackable
{

    [SerializeField] private int _maxHealth;
    [SerializeField] private float _repathFrequency = 0.5f;
    [SerializeField] private float _playerDetectionDistance = 20.0f;
    [SerializeField] private GameObject _hitPointIndicator;
    [SerializeField] private Transform _attackBox;
    [SerializeField] private float _attackCooldown = 1.0f;
    [SerializeField] private int _attackDamage = 1;

    [SerializeField] private Animator _animator;

    [SerializeField] private float _attackDelay = 0.5f;

    private bool _isAttacking;
    private float _attackTimer;
    [SerializeField] private float _attackCooldownTimer;

    private LevelManager _levelManager;

    private int _health;

    public enum State
    {
        None,
        Idle,
        Patrol,
        Agro
    };

    [SerializeField] private State _initState = State.Patrol;
    [SerializeField] private State _state = State.None;

    private float _repathTimer;
    private Vector3 _moveTarget;

    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private Transform _playerTarget;
    private PlayerController _player;

    private Vector3 _prevPosition;
    private Vector3 _currPosition;


    void Awake()
    {
        _rb = this.GetComponent<Rigidbody>();
        _agent = this.GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _playerTarget = _player.transform;
        _levelManager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
        _moveTarget = this.transform.position;
        _repathTimer = Random.Range(0f, _repathFrequency);
        
        _health = _maxHealth;

        SwitchState(_initState);
    }

    void Update()
    {
        _attackCooldownTimer += Time.deltaTime;

        ExecuteState();

        UpdateMovePath();

        if (_isAttacking)
        {
            _attackCooldownTimer = 0.0f;
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= _attackDelay)
            {
                AttackPlayer();
            }
        } else
        {

            _currPosition = this.transform.position;
            bool isMoving = (_currPosition - _prevPosition).sqrMagnitude / Time.deltaTime > 0.1f;
            _prevPosition = _currPosition;

            if (isMoving)
            {
                SetAnimationState("Walk");
            }
            else
            {
                SetAnimationState("Idle");
            }
        }

    }

    private void ExecuteState()
    {
        switch (_state)
        {
            case State.Idle:
                ExecuteIdle();
                break;
            case State.Patrol:
                ExecutePatrol();
                break;
            case State.Agro:
                ExecuteAgro();
                break;
        }
    }

    private void ExecuteIdle()
    {
        // Wait and check for nearby player

        if (CanDetectPlayer())
        {
            SwitchState(State.Agro);
        }
    }

    private void ExecutePatrol()
    {
        // Move around the map randomly

        var distSqrd = SquaredDistanceToTarget();

        if (distSqrd <= 5.0f)
        {
            MoveToRandomPatrolPoint();
        }

        if (CanDetectPlayer())
        {
            SwitchState(State.Agro);
        }
    }

    private void ExecuteAgro()
    {
        // Move towards player and attack

        SetMoveTarget(_playerTarget.position);


        if (!_isAttacking && _attackCooldownTimer > _attackCooldown)
        {
            if (SquaredDistanceToTarget() <= 1.6f * 1.6f)
            {
                StartPlayerAttack();
            }
        }
    }

    private void StartPlayerAttack()
    {
        _isAttacking = true;
        _attackTimer = 0.0f;
        _agent.isStopped = true;
        SetAnimationState("Slash");
    }

    private void AttackPlayer()
    {
        if (Physics.CheckBox(_attackBox.position, _attackBox.localScale / 2, Quaternion.identity, LayerMask.GetMask("Player")))
        {
            _player.OnTakeDamage(_attackDamage);
        }
        _isAttacking = false;
        if (_agent.enabled)
        {
            _agent.isStopped = false;
        }
        _attackCooldownTimer = 0.0f;

        SetAnimationState("Idle");
    }

    private void SwitchState(State newState)
    {
        if (_state == newState)
        {
            return;
        }

        switch (newState)
        {
            case State.Patrol:
                SwitchToPatrol();
                break;
        }

        _state = newState;
    }

    private void SwitchToPatrol()
    {
        _state = State.Patrol;
        MoveToRandomPatrolPoint();
    }

    private void SetAnimationState(string state)
    {
        _animator.SetBool("Slash", state == "Slash");
        _animator.SetBool("Idle", state == "Idle");
        _animator.SetBool("Walk", state == "Walk");
    }


    private float SquaredDistanceToTarget()
    {
        return (this.transform.position - _moveTarget).sqrMagnitude;
    }

    private void MoveToRandomPatrolPoint()
    {
        var positions = _levelManager.GetPatrolPositions();
        if (positions.Length > 0)
        {
            var randomPosition = positions[Random.Range(0, positions.Length - 1)];
            SetMoveTarget(randomPosition);
        }
        else
        {
            SetMoveTarget(this.transform.position);
        }
    }

    private void SetMoveTarget(Vector3 target)
    {
        var diff = (target - _moveTarget).sqrMagnitude;

        _moveTarget = target;

        if (diff > 2.0f * 2.0f)
        {
            ForceUpdateMovePath();
        }
    }

    private void UpdateMovePath()
    {
        _repathTimer += Time.deltaTime;
        if (_repathTimer >= _repathFrequency)
        {
            ForceUpdateMovePath();
        }
    }

    private void ForceUpdateMovePath()
    {
        if (_agent.enabled)
        {
            _repathTimer = 0.0f;
            _agent.SetDestination(_moveTarget);
        }
    }

    private bool CanDetectPlayer()
    {
        var toPlayer = _playerTarget.position - this.transform.position;
        var distSqrd = toPlayer.sqrMagnitude;
        if (distSqrd <= _playerDetectionDistance * _playerDetectionDistance)
        {
            if (Physics.Raycast(this.transform.position, toPlayer, out RaycastHit hit, _playerDetectionDistance, LayerMask.GetMask("Player")))
            {
                return true;
            }
        }

        return false;
    }

    //public bool IsAgentOnNavMesh(Transform agentObject)
    //{
    //    float onMeshThreshold = 1.0f;
    //    Vector3 agentPosition = agentObject.position;
    //    NavMeshHit hit;

    //    // Check for nearest point on navmesh to agent, within onMeshThreshold
    //    if (NavMesh.SamplePosition(agentPosition, out hit, onMeshThreshold, NavMesh.AllAreas))
    //    {
    //        // Check if the positions are vertically aligned
    //        if (Mathf.Approximately(agentPosition.x, hit.position.x)
    //            && Mathf.Approximately(agentPosition.z, hit.position.z))
    //        {
    //            // Lastly, check if object is below navmesh
    //            return agentPosition.y >= hit.position.y;
    //        }
    //    }

    //    return false;
    //}

    public void OnAttacked(Vector3 source, int damage)
    {
        _health -= damage;
        _health = Mathf.Max(0, _health);

        SpawnHPIndicator(-damage);

        if (_health == 0)
        {
            _agent.enabled = false;
            _rb.isKinematic = false;
            var dir = this.transform.position - source;
            _rb.AddForce(dir * 10f + Vector3.up * 5, ForceMode.VelocityChange);
        }
    }

    private void SpawnHPIndicator(int hp)
    {
        var go = Instantiate<GameObject>(_hitPointIndicator, null);
        go.transform.position = this.transform.position + this.transform.up * 2.0f;
        var indicator = go.GetComponent<HitPointIndicator>();
        indicator.Initialize(hp);
    }


    private void OnDrawGizmosSelected()
    {
        if (_moveTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, _moveTarget);
        }

        if (_state != State.Agro)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position, _playerDetectionDistance);
        }
    }
}
