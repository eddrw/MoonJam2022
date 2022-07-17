using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UIAbilityController[] _abilityControllers;

    private Dictionary<string, UIAbilityController> _abilities;

    private void Awake()
    {
        _abilities = new Dictionary<string, UIAbilityController>();
        foreach (var controller in _abilityControllers)
        {
            _abilities.Add(controller.name, controller);
        }

        foreach (var name in _abilities.Keys)
        {
            SetAbilityPercentage(name, 0.0f);
        }
    }

    public void SetAbilityPercentage(string name, float percentage)
    {
        if (_abilities.ContainsKey(name) == false)
        {
            Debug.LogError("Unknown ability name " + name);
        }

        _abilities[name].SetPercentage(percentage);
    }

}
