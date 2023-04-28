using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager S;

    public string levelName;

    public string nextSceneName;

    // Start is called before the first frame update
    void Start()
    {
        S = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void btn_StartTheGame()
    {
        Debug.Log("StartPushed");
        SceneManager.LoadScene("Main");
    }

    public void btn_BackToMain()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void btn_Tutorial()
    {
        SceneManager.LoadScene("NewTutorial");
    }

    public void btn_Credits()
    {
        SceneManager.LoadScene("Credits");
    }

}
