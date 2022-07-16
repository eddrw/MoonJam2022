using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScriptBullet : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private TMP_Text _text;

    private float _timer;
    private static readonly float MAX_LIFE_TIME = 10.0f;

    private Transform _camera;

    public void Initialize(string text, Transform camera)
    {
        _text.text = text;
        _camera = camera;

        var cameraToText = _camera.position - this.transform.position;
        if (Vector3.Dot(cameraToText, this.transform.right) < 0)
        {
            _text.rectTransform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= MAX_LIFE_TIME)
        {
            Explode();
        }


        // Move forward
        this.transform.position += _speed * this.transform.forward * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            var enemy = other.GetComponentInParent<EnemyController>();
            enemy.OnAttacked(enemy.transform.position - this.transform.position);
        }

        if (other.tag != "Player" && other.tag != "ScriptBullet")
        {
            Explode();
            //Debug.Log("Destroyed! " + other.tag + " | " + other.name);
        }
    }

    private void Explode()
    {
        GameObject.Destroy(this.gameObject);
    }
}
