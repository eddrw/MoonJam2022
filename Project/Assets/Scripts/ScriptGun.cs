using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

public class ScriptGun : MonoBehaviour
{
    public string text = "Testing one two three!!";
    [SerializeField] private float _coolDown = 0.3f;
    [SerializeField] private float _bulletSpeed = 10f;

    [SerializeField] private GameObject _bulletPrefab;

    private string[] _words;
    private int _wordIndex = 0;

    private float _timeSinceLastShot = 0.0f;

    private Vector3 _prevPosition;
    private Vector3 _currPosition;

    private int _damage;

    public void Initialize(int damage)
    {
        _damage = damage;
        _words = Regex.Replace(text, "[^a-zA-Z0-9 _]", string.Empty).Split(" ");
        Debug.Log("Words: " + string.Join(", ", _words));
    }

    //private void Awake()
    //{
    //    _words = Regex.Replace(text, "[^a-zA-Z0-9 _]", string.Empty).Split(" ");
    //    Debug.Log("Words: " + string.Join(", ", _words));
    //}

    private void Update()
    {
        _timeSinceLastShot += Time.deltaTime;

        if (Input.GetMouseButton(0) && _timeSinceLastShot > _coolDown)
        {
            _timeSinceLastShot = 0.0f;
            Shoot();
        }

        _prevPosition = _currPosition;
        _currPosition = this.transform.position;
    }

    public void Shoot()
    {
        Vector3 velocity = (_currPosition - _prevPosition) / Time.deltaTime;

        string word = _words[_wordIndex];
        
        _wordIndex++;
        if (_wordIndex >= _words.Length)
        {
            _wordIndex = 0;
        }
        GameObject go = Instantiate(_bulletPrefab, null);
        go.transform.position = this.transform.position;
        go.transform.localRotation = this.transform.rotation;
        ScriptBullet bullet = go.GetComponent<ScriptBullet>();

        bullet.Initialize(word, Camera.main.transform, _bulletSpeed, velocity, _damage);
    }
}
