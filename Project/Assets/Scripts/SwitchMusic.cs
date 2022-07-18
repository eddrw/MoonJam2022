using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMusic : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    [SerializeField] private AudioClip outro;
    [SerializeField] private GameObject finalDialogTrigger;
    
    // Start is called before the first frame update
    void Start()
    {
        finalDialogTrigger.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchToEnd()
    {
        cam.GetComponent<AudioSource>().clip = outro;
        cam.GetComponent<AudioSource>().Play();
        finalDialogTrigger.SetActive(true);
    }
}
