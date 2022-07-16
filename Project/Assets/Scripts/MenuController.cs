using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private TMP_Text _blinkText;

    private void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene("GamePlay");
        }

        float intensity = (1 + Mathf.Sin(2*Time.realtimeSinceStartup)) / 2;
        _blinkText.color = new Color(1,1,1,intensity);
    }
}
