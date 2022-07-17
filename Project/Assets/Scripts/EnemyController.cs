using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private Transform _player;

    private float _stunTimer = 0.0f;
    private readonly static float STUN_DURATION = 5.0f;
    private float _checkTimer;

    void Start()
    {
        _rb = this.GetComponent<Rigidbody>();
        _agent = this.GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        _checkTimer += Time.deltaTime;
        if (_checkTimer > 0.5f)
        {
            _checkTimer = 0.0f;
            if (_agent.enabled && IsAgentOnNavMesh(_agent.transform))
            {
                _agent.SetDestination(_player.position);
            }
        }
        else {
            _stunTimer += Time.deltaTime;
            if (_stunTimer > STUN_DURATION)
            {
                _stunTimer = 0.0f;
                this.transform.position += Vector3.up * 5;
                if (Physics.Raycast(transform.position - Vector3.up * 3, -Vector3.up, out RaycastHit hit, Mathf.Infinity))
                {
                    this.transform.position = hit.point + Vector3.up * 1;
                    this.transform.localRotation = Quaternion.identity;
                    _agent.enabled = true;
                }
            }
        }
    }

    public bool IsAgentOnNavMesh(Transform agentObject)
    {
        float onMeshThreshold = 1.0f;
        Vector3 agentPosition = agentObject.position;
        NavMeshHit hit;

        // Check for nearest point on navmesh to agent, within onMeshThreshold
        if (NavMesh.SamplePosition(agentPosition, out hit, onMeshThreshold, NavMesh.AllAreas))
        {
            // Check if the positions are vertically aligned
            if (Mathf.Approximately(agentPosition.x, hit.position.x)
                && Mathf.Approximately(agentPosition.z, hit.position.z))
            {
                // Lastly, check if object is below navmesh
                return agentPosition.y >= hit.position.y;
            }
        }

        return false;
    }

    public void OnAttacked(Vector3 dir)
    {
        if (_agent.enabled)
        {
            _agent.isStopped = true;
        }
        //_agent.enabled = false;
        _rb.isKinematic = false;
        _rb.AddForce(dir * 20f + Vector3.up * 5, ForceMode.VelocityChange);
    }
}
