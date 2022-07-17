using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;

    private Vector3 _targetOffset;
    private float _smoothTime = 0.3f;
    private Vector3 _velocity = Vector3.zero;


    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_player == null)
        {
            Debug.LogError("[CameraController] Missing reference to player game object.");
        }

        _targetOffset = this.transform.position - _player.position;
        this.transform.LookAt(_player.position);

    }


    void LateUpdate()
    {
        // this.transform.position = _player.position + _targetOffset;
        var targetPosition = _player.position + _targetOffset;
        transform.position = Vector3.SmoothDamp(this.transform.position, targetPosition, ref _velocity, _smoothTime);
    }
}
