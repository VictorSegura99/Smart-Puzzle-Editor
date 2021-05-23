using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButtons : MonoBehaviour
{
    // Internal Variables
    UIManager manager;

    private void Start()
    {
        if (GameObject.Find("Levels_UI") != null)
        {
            manager = GameObject.Find("Levels_UI").GetComponent<UIManager>();
        }
    }

    public void ResumeGame()
    {
        if (manager != null)
        {
            manager.ResumeGame();
        }
    }

    public void ResetPuzzle()
    {
        LevelManager.instance.RestartLevel();
    }

    public void ExitToMenu()
    {
        ChangeScene("MainMenu_PreAlpha");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void ChangeScene(string scene_name)
    {
        if (Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }

        SceneManager.LoadScene(scene_name);
    }
}
