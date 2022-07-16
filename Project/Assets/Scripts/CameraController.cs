using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;

    private Vector3 _targetOffset;


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


    void Update()
    {
        this.transform.position = _player.position + _targetOffset;
    }
}
