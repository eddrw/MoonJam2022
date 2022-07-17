using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField] private Transform _bar;
    [SerializeField] private Image _profile;

    public void SetHealthPercentage(float percentage)
    {
        var scale = _bar.localScale;
        scale.x = percentage;
        _bar.localScale = scale;

        _profile.color = Color.Lerp(Color.red, Color.white, percentage);
    }

}
