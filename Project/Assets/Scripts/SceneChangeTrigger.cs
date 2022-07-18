using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeTrigger : MonoBehaviour
{
    private MeshRenderer[] _renderers;

    private Material _materialInstance;
    private Color _baseColor;
    [SerializeField] private bool _hideOnStart = false;
    [SerializeField] private Color _otherColor;
    [SerializeField] private float _flashSpeed;

    [SerializeField] private SceneController _sceneController;

    private BoxCollider _collider;

    private void Awake()
    {
        _collider = this.GetComponent<BoxCollider>();
        _renderers = this.transform.GetComponentsInChildren<MeshRenderer>();

        _materialInstance = _renderers[0].material;
        foreach (var mr in _renderers)
        {
            mr.material = _materialInstance;
        }

        _baseColor = _materialInstance.color;

        _otherColor.a = _baseColor.a;

        if (_hideOnStart)
        {
            ShowHide(false);
        }
    }

    public void Show()
    {
        ShowHide(true);
    }

    private void ShowHide(bool show)
    {
        _collider.enabled = show;
        foreach (var mr in _renderers)
        {
            mr.enabled = show;
        }
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
