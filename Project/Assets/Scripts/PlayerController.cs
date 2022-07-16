using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform _model;
    [SerializeField] private float _speed = 8.0f;
    [SerializeField] private float _jump = 10.0f;
    [SerializeField] private float _gravity = -20f;
    private Transform _camera;
    private Rigidbody _rb;


    void Start()
    {
        _camera = Camera.main.transform;
        _rb = this.GetComponent<Rigidbody>();
        _rb.useGravity = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Attack();
        }
    }

    void FixedUpdate()
    {
        HandleMovement();

        _rb.AddForce(Vector3.up * _gravity);
    }

    void HandleMovement()
    {
        var forward = _camera.forward;
        var right = _camera.right;

        var direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction += forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction -= forward;
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction += right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction -= right;
        }

        direction.y = 0.0f;
        direction.Normalize();

        // this.transform.position += direction * Time.deltaTime * _speed;
        Move(direction);
    }


    private void Move(Vector3 dir)
    {
        var v = _rb.velocity;
        // Maintain vertical velocity to preserve gravity.
        v.x = dir.x * _speed;
        v.z = dir.z * _speed;
        _rb.velocity = v;

        _model.LookAt(this.transform.position + dir);
    }

    private void Jump()
    {
        _rb.AddForce(Vector3.up * _jump, ForceMode.Impulse);
    }

    private Vector3 _center;
    private float _radius;

    private void Attack()
    {
        _center = this.transform.position + _model.forward * 2f;
        _radius = 2.0f;
        var colliders = Physics.OverlapSphere(_center, _radius, LayerMask.GetMask("Enemy"));
        foreach (var collider in colliders)
        {
            var enemy = collider.gameObject.GetComponentInParent<EnemyController>();
            enemy.OnAttacked(enemy.transform.position - this.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_center, _radius);
    }
}
