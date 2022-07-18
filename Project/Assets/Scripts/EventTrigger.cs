using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent _event;

    private bool _hasInvoked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!_hasInvoked)
            {
                _hasInvoked = true;
                _event.Invoke();
            }
            
        }

    }
}
