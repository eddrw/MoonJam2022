using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Object _nextScene;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(_nextScene.name);
    }
}
