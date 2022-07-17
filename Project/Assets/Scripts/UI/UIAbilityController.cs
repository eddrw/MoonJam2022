using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAbilityController : MonoBehaviour
{
    [SerializeField] private Image _progressImage;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _iconImage;

    [SerializeField] private float _percentage;
    private float _height;

    private void Awake()
    {
        _height = GetHeight();
    }

    public float Percentage
    {
        get { return _percentage; }
        set { SetPercentage(value); }
    }

    private float GetHeight()
    {
        var rt = (RectTransform)this.transform;
        return rt.rect.height;
    }

    public void SetPercentage(float percentage)
    {
        _percentage = percentage;
        _percentage = Mathf.Clamp01(_percentage);
        var top = _height * _percentage;
        var rt = _progressImage.transform as RectTransform;
        rt.localScale = new Vector3(rt.localScale.x, percentage, rt.localScale.z);
    }

    private void Update()
    {
        //this.Percentage += Time.deltaTime;
        //if (this.Percentage >= 0.99f)
        //{
        //    this.Percentage = 0.0f;
        //}
    }
}
