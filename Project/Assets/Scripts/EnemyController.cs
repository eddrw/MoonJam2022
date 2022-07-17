using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    [SerializeField] private int _maxHealth;
    [SerializeField] private float _repathFrequency = 0.5f;
    [SerializeField] private float _playerDetectionDistance = 20.0f;

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
    private Transform _player;


    void Awake()
    {
        _rb = this.GetComponent<Rigidbody>();
        _agent = this.GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player").transform;
        _levelManager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
        _moveTarget = this.transform.position;
        _repathTimer = Random.Range(0f, _repathFrequency);
        
        _health = _maxHealth;

        SwitchState(_initState);
    }

    void Update()
    {
        ExecuteState();

        UpdateMovePath();

        //_checkTimer += Time.deltaTime;
        //if (_checkTimer > 0.5f)
        //{
        //    _checkTimer = 0.0f;
        //    if (_agent.enabled && IsAgentOnNavMesh(_agent.transform))
        //    {
        //        _agent.SetDestination(_player.position);
        //    }
        //}
        //else {
        //    _stunTimer += Time.deltaTime;
        //    if (_stunTimer > STUN_DURATION)
        //    {
        //        _stunTimer = 0.0f;
        //        this.transform.position += Vector3.up * 5;
        //        if (Physics.Raycast(transform.position - Vector3.up * 3, -Vector3.up, out RaycastHit hit, Mathf.Infinity))
        //        {
        //            this.transform.position = hit.point + Vector3.up * 1;
        //            this.transform.localRotation = Quaternion.identity;
        //            _agent.enabled = true;
        //        }
        //    }
        //}
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

        SetMoveTarget(_player.position);
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
        var toPlayer = _player.position - this.transform.position;
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

    public void OnAttacked(Vector3 dir)
    {
        //if (_agent.enabled)
        //{
        //    _agent.isStopped = true;
        //}
        _agent.enabled = false;
        _rb.isKinematic = false;
        _rb.AddForce(dir * 20f + Vector3.up * 5, ForceMode.VelocityChange);
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
