using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private bool _hideWalls = false;

    private Vector3 _offsetDirection;
    [SerializeField] private float _offsetDistance;
    //private Vector3 _targetOffset;
    private float _smoothTime = 0.3f;
    private Vector3 _velocity = Vector3.zero;
    private float _wallCheckFrequency = 0.3f;

    private float _wallCheckTimer;
    private Dictionary<int, MeshRenderer> _wallRenderers;


    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_target == null)
        {
            Debug.LogError("[CameraController] Missing reference to player game object.");
        }

        var offset = this.transform.position - _target.position;
        _offsetDirection = offset.normalized;
        _offsetDistance = _offsetDistance <= 0.001f ? offset.magnitude : _offsetDistance;
        this.transform.LookAt(_target.position);

        _wallRenderers = new Dictionary<int, MeshRenderer>();

    }

    private void Update()
    {
        if (_hideWalls)
        {
            _wallCheckTimer += Time.deltaTime;
            if (_wallCheckTimer > _wallCheckFrequency)
            {
                _wallCheckTimer = 0.0f;
                HideBlockingWalls();
            }
        }
    }

    private void HideBlockingWalls()
    {
        var mr = GetWallRenderer();

        if (mr == null)
        {
            foreach (var renderer in _wallRenderers.Values)
            {
                renderer.enabled = true;
            }

            _wallRenderers.Clear();
        }
        else
        {
            var id = mr.gameObject.GetInstanceID();
            if (!_wallRenderers.ContainsKey(id))
            {
                _wallRenderers.Add(id, mr);
                mr.enabled = false;
            }
        }
    }

    MeshRenderer GetWallRenderer()
    {
        Vector3 dir = (_target.position + _target.up * 1.0f) - this.transform.position;

        Ray ray = new Ray(this.transform.position, dir);

        if (Physics.Raycast(ray, out RaycastHit hit, dir.magnitude, LayerMask.GetMask("Wall")))
        {
            var mr = hit.transform.gameObject.GetComponent<MeshRenderer>();
            if (mr.enabled)
            {
                return mr;
            }
        }
        return null;
    }



    void LateUpdate()
    {
        var targetPosition = _target.position + _offsetDirection * _offsetDistance;
        transform.position = Vector3.SmoothDamp(this.transform.position, targetPosition, ref _velocity, _smoothTime);
    }
}
