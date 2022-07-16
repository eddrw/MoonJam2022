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

    void Start()
    {
        _rb = this.GetComponent<Rigidbody>();
        _agent = this.GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if (_agent.enabled)
        {
            _agent.SetDestination(_player.position);
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

    public void OnAttacked(Vector3 dir)
    {
        if (_agent.enabled)
        {
            _agent.Stop();
        }
        _agent.enabled = false;
        _rb.isKinematic = false;
        _rb.AddForce(dir * 20f, ForceMode.VelocityChange);
    }
}
