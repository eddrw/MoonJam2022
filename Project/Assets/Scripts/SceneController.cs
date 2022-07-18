using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private string _nextSceneName;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(_nextSceneName);
    }
}
