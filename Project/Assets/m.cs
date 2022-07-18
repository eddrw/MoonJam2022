using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class m : MonoBehaviour
{
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _animator.SetBool("Idle", true);
        _animator.SetBool("Cooking", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
