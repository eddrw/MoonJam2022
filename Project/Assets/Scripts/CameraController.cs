using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private GameObject _character;
    [SerializeField] private Vector3 characterOffset;
    [SerializeField] private bool inDialog;

    private Vector3 _offsetDirection;
    [SerializeField] private float _offsetDistance;
    //private Vector3 _targetOffset;
    private float _smoothTime = 0.3f;
    private Vector3 _velocity = Vector3.zero;


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
        _offsetDistance = offset.magnitude;
        this.transform.LookAt(_target.position);
    }


    void LateUpdate()
    {
        // this.transform.position = _target.position + _targetOffset;
        var targetPosition = _target.position + _offsetDirection * _offsetDistance;
        transform.position = Vector3.SmoothDamp(this.transform.position, targetPosition, ref _velocity, _smoothTime);
        this._character.transform.position = this.transform.position + characterOffset;
    }
}
