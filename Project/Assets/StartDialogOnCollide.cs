using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogOnCollide : MonoBehaviour
{
    [SerializeField] private DialogObject dialogObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
        {
            player.Dialog.ShowDialog(dialogObject);
        }
    }
}
