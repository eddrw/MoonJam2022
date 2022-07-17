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
    [SerializeField] private Transform _bashCollider;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Camera _camera;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private ScriptGun _scriptGun;

    [Header("Prefabs")]
    [SerializeField] private GameObject _dashEffectPrefab;
    [SerializeField] private GameObject _bashEffectPrefab;
    [SerializeField] private GameObject _hitPointIndicator;

    [Header("Movement Settings")]
    [SerializeField] private float _speed = 8.0f;
    [SerializeField] private float _dashSpeed = 10f; // Units per second
    [SerializeField] private float _jump = 10.0f;
    [SerializeField] private float _gravity = -20f;
    private bool _gravityEnabled = true;

    [Header("Attack Settings")]
    [SerializeField] private int _bashDamage = 2;
    [SerializeField] private int _scriptDamage = 1;

    [SerializeField] private float _dashCooldownDuration = 1.0f;
    [SerializeField] private float _bashCooldownDuration = 1.0f;
    [SerializeField] private float _jumpCooldownDuration = 0.2f;
    [SerializeField] private float _scriptCooldownDuration = 1.0f;

    private float _jumpCooldownTimer;
    private float _scriptCooldownTimer;

    [SerializeField] private AnimationCurve _bashSpin;
    [SerializeField] private float _bashDelay;
    [SerializeField] private float _bashDuration;
    [SerializeField] private float _bashRotationCount;
    [SerializeField] private Vector2 _bashHeightRange = new Vector2(0.5f, 1.5f);

    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 5;


    private Rigidbody _rb;
    private Collider _collider;

    // Phantom Dash Data
    private bool _isDashing;
    private float _dashCooldownTimer;
    private float _dashTimer;
    private Vector3 _dashStart;
    private Vector3 _dashEnd;
    private ParticleSystem _dashEffect;
    private float _dashDuration;

    // Briefcase Bash Data
    private bool _isBashing;
    private float _bashCooldownTimer;
    private float _bashTimer;
    private Quaternion _bashInitRot;
    private TrailRenderer _bashEffect;

    // Health
    private int _health;

    private void Awake()
    {
        _collider = this.GetComponent<CapsuleCollider>();
        _rb = this.GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _health = _maxHealth;
        _uiManager.SetHealthPercentage(_health / _maxHealth);
        _scriptGun.Initialize(_scriptDamage);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnHeal(1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnTakeDamage(1);
        }

        UpdatedCursorPosition();

        if (Input.GetKeyDown(KeyCode.Space) && _jumpCooldownTimer >= _jumpCooldownDuration)
        {
            Jump();
        }
        else
        {
            _jumpCooldownTimer += Time.deltaTime;
            var cooldownPercentage = 1f - Mathf.Clamp01(_jumpCooldownTimer / _jumpCooldownDuration);
            _uiManager.SetAbilityPercentage("Jump", cooldownPercentage);
        }

        if (Input.GetMouseButtonDown(1) && !_isDashing)
        {
            if (_dashCooldownTimer >= _dashCooldownDuration)
            {
                OnPhantomDashBegin();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !_isBashing)
        {
            if (_bashCooldownTimer >= _bashCooldownDuration)
            {
                OnBriefcaseBashBegin();
            }
        }

        UpdatePhantomDash();
        UpdateBriefcaseBash();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();

        ApplyGravity();
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
        if (!_isBashing)
        {
            var lookAt = _cursor.position;
            lookAt.y = _base.position.y;
            _base.LookAt(lookAt);
        }
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
        if (IsGrounded())
        {
            _rb.AddForce(Vector3.up * _jump, ForceMode.Impulse);
            _jumpCooldownTimer = 0.0f;
            _uiManager.SetAbilityPercentage("Jump", 1.0f);
        }
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

    private bool IsGrounded()
    {
        var colliders = Physics.OverlapSphere(_groundCheck.position, 0.1f);
        foreach (var collider in colliders)
        {
            if (collider.tag != "Player")
            {
                return true;
            }
        }
        return false;
    }


    /*-------------------------  Phantom Dash  -------------------------*/

    private void UpdatePhantomDash()
    {
        if (_isDashing)
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
        else
        {
            _dashCooldownTimer += Time.deltaTime;
            var cooldownPercentage = 1f - Mathf.Clamp01(_dashCooldownTimer / _dashCooldownDuration);
            _uiManager.SetAbilityPercentage("Dash", cooldownPercentage);
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

        _uiManager.SetAbilityPercentage("Dash", 1.0f);
    }

    private void OnPhantomDashEnd()
    {
        SetPlayerCollisions(true);
        SetPlayerGravity(true);
        _isDashing = false;
        _dashCooldownTimer = 0.0f;
        _dashEffect.transform.parent = null;

        _model.gameObject.SetActive(true);
    }

    /*-------------------------  Briefcase Bash  -------------------------*/

    private void UpdateBriefcaseBash()
    {
        if (_isBashing)
        {
            _bashTimer += Time.deltaTime;

            float t = Mathf.Min(_bashTimer / _bashDuration, 1.0f);

            t = _bashSpin.Evaluate(t);

            float height = Mathf.Lerp(_bashHeightRange.x, _bashHeightRange.y, t);
            _bashEffect.transform.localPosition = new Vector3(_bashEffect.transform.localPosition.x, height, _bashEffect.transform.localPosition.z);

            float angle = (360 * _bashRotationCount) * t;

            Quaternion rot = Quaternion.AngleAxis(angle, _base.up);

            _base.localRotation = rot * _bashInitRot;

            if (_bashTimer >= _bashDelay)
            {
                if (!_bashEffect.gameObject.activeSelf)
                {
                    _bashEffect.gameObject.SetActive(true);
                }

                var colliders = Physics.OverlapBox(_bashCollider.position, _bashCollider.localScale / 2, Quaternion.identity, LayerMask.GetMask("Attackable"));
                foreach (var collider in colliders)
                {
                    var attackTarget = collider.gameObject.GetComponentInParent<IAttackable>();
                    attackTarget.OnAttacked(this.transform.position, _bashDamage);
                    //var enemy = collider.gameObject.GetComponentInParent<EnemyController>();
                    //enemy.OnAttacked(enemy.transform.position - this.transform.position);
                }
            }

            if (t >= 1.0f)
            {
                OnBriefcaseBashEnd();
            }
        }
        else
        {
            _bashCooldownTimer += Time.deltaTime;
            var cooldownPercentage = 1f - Mathf.Clamp01(_bashCooldownTimer / _bashCooldownDuration);
            _uiManager.SetAbilityPercentage("Bash", cooldownPercentage);
        }
        

    }

    private void OnBriefcaseBashBegin()
    {
        _isBashing = true;
        _bashTimer = 0.0f;
        _bashInitRot = _base.localRotation;

        GameObject go = Instantiate<GameObject>(_bashEffectPrefab, _base);
        go.SetActive(false);
        go.transform.localPosition = new Vector3(0, _bashHeightRange.x, 2);
        go.transform.localRotation = Quaternion.Euler(90, 0, 90);
        _bashEffect = go.GetComponent<TrailRenderer>();

        _uiManager.SetAbilityPercentage("Bash", 1.0f);
    }

    private void OnBriefcaseBashEnd()
    {
        _isBashing = false;
        _bashCooldownTimer = 0.0f;
        _bashEffect.emitting = false;
    }

    /*-------------------------  Health  -------------------------*/

    public void OnHeal(int heal)
    {
        OnChangeHealth(heal);
    }

    public void OnTakeDamage(int damage)
    {
        OnChangeHealth(-damage);
    }

    private void OnChangeHealth(int change)
    {
        _health = Mathf.Max(Mathf.Min(_maxHealth, _health + change), 0);
        SpawnHPIndicator(change);
        _uiManager.SetHealthPercentage(_health / (float)_maxHealth);

        if (_health == 0)
        {
            OnDeath();
        }
    }

    private void SpawnHPIndicator(int hp)
    {
        var go = Instantiate<GameObject>(_hitPointIndicator, null);
        go.transform.position = this.transform.position + this.transform.up * 2.0f;
        var indicator = go.GetComponent<HitPointIndicator>();
        indicator.Initialize(hp);
    }

    private void OnDeath()
    {
        Debug.Log("You Died :(");
    }

}
