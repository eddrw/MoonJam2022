using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitPointIndicator : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Transform _canvas;
    [SerializeField] private Color _healColor = Color.green;
    [SerializeField] private Color _damageColor = Color.red;
    [SerializeField] private Color _neutralColor = Color.gray;

    [SerializeField] private float _gravityMagnitude = 2f;
    [SerializeField] private float _initSpeed = 2f;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private float _baseScale = 1f;
    [SerializeField] private float _hpScale = 0.5f;

    private Vector3 _velocity;
    private float _timer;
    private int _hp;

    public void Initialize(int hp)
    {
        _hp = hp;

        var str = HPSign(hp) + Mathf.Abs(hp).ToString();
        var color = HPColor(hp);

        _text.text = str;
        _text.color = color;


        _velocity = (Vector3.up + new Vector3(0.3f * Random.Range(-1f,1f), 0, 0.3f * Random.Range(-1f, 1f))).normalized * _initSpeed;

        _canvas.transform.LookAt(Camera.main.transform.position);
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _duration)
        {
            GameObject.Destroy(this.gameObject);
        }

        float t = _timer / _duration;
        t = 1 - t;
        float scale = _baseScale * t * t + Mathf.Abs(_hp) * _hpScale;
        this.transform.localScale = scale * Vector3.one;

        this.transform.position += _velocity * Time.deltaTime;
        _velocity += -this.transform.up * _gravityMagnitude * Time.deltaTime;
    }

    private string HPSign(int hp)
    {
        if (hp > 0)
        {
            return "+";
        }
        else if (hp < 0)
        {
            return "-";
        }
        else
        {
            return "";
        }
    }

    private Color HPColor(int hp)
    {
        if (hp > 0)
        {
            return _healColor;
        }
        else if (hp < 0)
        {
            return _damageColor;
        }
        else
        {
            return _neutralColor;
        }
    }

}
