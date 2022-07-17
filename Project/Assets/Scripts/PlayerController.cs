using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Component References")]
    // Rotation Base – Anything that should rotate with the player
    // is a child of the base object.
    [SerializeField] private Transform _base;
    [SerializeField] private Transform _model;
    [SerializeField] private Transform _cursor;
    [SerializeField] private Transform _shortAttack;
    [SerializeField] private Camera _camera;
    [SerializeField] private Animator _animator;

    [Header("Prefabs")]
    [SerializeField] private GameObject _dashEffectPrefab;

    [Header("Movement Settings")]
    [SerializeField] private float _speed = 8000.0f;
    [SerializeField] private float _dashSpeed = 10f; // Units per second
    [SerializeField] private float _jump = 20.0f;
    [SerializeField] private float _gravity = -20f;
    private bool _gravityEnabled = true;

    [Header("Attack Settings")]
    
    private Rigidbody _rb;
    private Collider _collider;


    // Dash Data
    private bool _isDashing;
    private float _dashTimer;
    private Vector3 _dashStart;
    private Vector3 _dashEnd;
    private ParticleSystem _dashEffect;
    private float _dashDuration;


    private void Awake()
    {
        _collider = this.GetComponent<CapsuleCollider>();
        _rb = this.GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdatedCursorPosition();

        if (_isDashing)
        {
            UpdatePhantomDash();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ShortAttack();
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnPhantomDashBegin();
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();

        ApplyGravity();

        if (_rb.velocity.sqrMagnitude > 0.01f)
        {
            _animator.SetBool("Walking", true);
            _animator.SetBool("Idle", false);
        }
        else
        {
            _animator.SetBool("Walking", false);
            _animator.SetBool("Idle", true);
        }
    }

    /*-------------------------  Cursor  -------------------------*/

    private void UpdatedCursorPosition()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Cursor"))) {
            _cursor.position = hit.point;
        }
    }

    /*-------------------------  Movement  -------------------------*/
    private void ApplyGravity()
    {
        if (_gravityEnabled)
        {
            _rb.AddForce(Vector3.up * _gravity * 100 * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    private void HandleRotation()
    {
        var lookAt = _cursor.position;
        lookAt.y = _base.position.y;
        _base.LookAt(lookAt);
    }

    void HandleMovement()
    {
        var forward = _camera.transform.forward;
        var right = _camera.transform.right;

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

        Move(direction);
    }


    private void Move(Vector3 dir)
    {
        var v = _rb.velocity;
        // Maintain vertical velocity to preserve gravity.
        v.x = dir.x * _speed;
        v.z = dir.z * _speed;
        _rb.velocity = v;
    }

    private void Jump()
    {
        _rb.AddForce(Vector3.up * _jump, ForceMode.Impulse);
    }

    /*-------------------------  Utility  -------------------------*/

    private void SetPlayerCollisions(bool enabled)
    {
        _collider.enabled = enabled;
        _rb.isKinematic = !enabled;
    }

    private void SetPlayerGravity(bool enabled)
    {
        _gravityEnabled = enabled;
    }


    /*-------------------------  Attacks  -------------------------*/
    private void UpdatePhantomDash()
    {
        _dashTimer += Time.deltaTime;
        
        float t = Mathf.Min(_dashTimer / _dashDuration, 1.0f);

        var position = Vector3.Lerp(_dashStart, _dashEnd, t);
        this.transform.position = position;

        if (t >= 1.0f)
        {
            OnPhantomDashEnd();
        }
    }

    private void OnPhantomDashBegin()
    {
        SetPlayerCollisions(false);
        SetPlayerGravity(false);
        _isDashing = true;
        _dashStart = this.transform.position;
        _dashEnd = _cursor.transform.position;
        _dashDuration = (_dashEnd - _dashStart).magnitude / _dashSpeed;
        _dashTimer = 0.0f;

        var dashGameObject = Instantiate<GameObject>(_dashEffectPrefab, _base);
        _dashEffect = dashGameObject.GetComponent<ParticleSystem>();

        _model.gameObject.SetActive(false);
    }

    private void OnPhantomDashEnd()
    {
        SetPlayerCollisions(true);
        SetPlayerGravity(true);
        _isDashing = false;
        _dashEffect.transform.parent = null;

        _model.gameObject.SetActive(true);
    }

    private void ShortAttack()
    {
        var center = _shortAttack.position;
        var halfSize = _shortAttack.localScale * 0.5f;

        var colliders = Physics.OverlapBox(center, halfSize, Quaternion.identity, LayerMask.GetMask("Enemy"));

        foreach (var collider in colliders)
        {
            var enemy = collider.gameObject.GetComponentInParent<EnemyController>();
            enemy.OnAttacked(enemy.transform.position - this.transform.position);
        }
    }


    private Vector3 _center;
    private float _radius;

    private void Attack()
    {
        // _center = this.transform.position + _base.forward * 2f;
        _center = _cursor.position;
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

        // if (_cursor != null)
        // {
        //     Gizmos.DrawSphere(_cursor.position, 1);
        // }
    }
}
