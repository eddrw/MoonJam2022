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

        float intensity = Mathf.Sin(2*Time.realtimeSinceStartup);
        _blinkText.color = new Color(1,1,1,intensity);
    }
}
