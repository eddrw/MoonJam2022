using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeTrigger : MonoBehaviour
{
    private MeshRenderer[] _renderers;

    private Material _materialInstance;
    private Color _baseColor;
    [SerializeField] private Color _otherColor;
    [SerializeField] private float _flashSpeed;

    [SerializeField] private SceneController _sceneController;

    private void Awake()
    {
        _renderers = this.transform.GetComponentsInChildren<MeshRenderer>();

        _materialInstance = _renderers[0].material;
        foreach (var mr in _renderers)
        {
            mr.material = _materialInstance;
        }

        _baseColor = _materialInstance.color;

        _otherColor.a = _baseColor.a;
    }

    private void Update()
    {
        float t = (1 + Mathf.Sin(Time.realtimeSinceStartup * _flashSpeed)) / 2;
        _materialInstance.color = Color.Lerp(_baseColor, _otherColor, t);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _sceneController.LoadNextScene();
        }
    }
}
