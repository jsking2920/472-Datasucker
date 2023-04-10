using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    SceneChange sceneChange;

    private void Start()
    {
        sceneChange = GetComponent<SceneChange>();
    }

    public void PlayGame()
    {
        sceneChange.ChangeScene();
    }
}
