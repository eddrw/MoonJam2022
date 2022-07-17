using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScriptBullet : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private GameObject _hitEffectPrefab;

    private float _speed;
    private float _timer;
    private static readonly float MAX_LIFE_TIME = 10.0f;

    private Transform _camera;

    private float _baseSpeed;
    private int _damage;

    public void Initialize(string text, Transform camera, float speed, Vector3 baseVelocity, int damage)
    {
        _text.text = text;
        _camera = camera;
        _speed = speed;
        _damage = damage;

        _baseSpeed = Mathf.Max( 0.0f, Vector3.Dot(this.transform.forward, baseVelocity) );


        //var cameraToText = _camera.position - this.transform.position;
        //if (Vector3.Dot(cameraToText, this.transform.right) < 0)
        //{
        //    _text.rectTransform.localScale = new Vector3(-1, 1, 1);
        //}
        //_text.rectTransform.localScale = new Vector3(-1, 1, 1);
        _text.transform.LookAt(_camera.position);
        _text.transform.Rotate(this.transform.up, 180.0f);
    }

    void Update()
    {
        

        _timer += Time.deltaTime;
        if (_timer >= MAX_LIFE_TIME)
        {
            Explode();
        }


        // Move forward
        this.transform.position += (_speed + _baseSpeed) * this.transform.forward * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        {
            var attackTarget = other.GetComponentInParent<IAttackable>();
            attackTarget.OnAttacked(this.transform.position, _damage);
        }

        if (other.tag != "Player" && other.tag != "ScriptBullet")
        {
            Explode();
            //Debug.Log(LayerMask.NameToLayer("Attackable"));
            //Debug.Log("Destroyed! " + other.tag + " | " + other.name + " | " + other.gameObject.layer);
        }
    }

    private void Explode()
    {
        GameObject go = Instantiate<GameObject>(_hitEffectPrefab, null);
        go.transform.position = this.transform.position;

        GameObject.Destroy(this.gameObject);
    }
}
