using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 8.0f;
    private Transform _camera;


    void Start()
    {
        _camera = Camera.main.transform;
    }

    void Update()
    {
        HandleMovement();
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

        this.transform.position += direction * Time.deltaTime * _speed;
    }
}
