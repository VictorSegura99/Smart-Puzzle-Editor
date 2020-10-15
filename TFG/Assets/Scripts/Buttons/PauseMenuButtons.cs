using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButtons : MonoBehaviour
{
    // Components

    // Inspector Variables
    public string next_scene;

    // Internal Variables
    UI_Manager manager;

    private void Start()
    {
        manager = GameObject.Find("Levels_UI").GetComponent<UI_Manager>();
    }

    public void ResumeGame()
    {
        manager.ResumeGame();
    }

    public void ResetPuzzle()
    {
        ChangeScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMenu()
    {
        ChangeScene("MainMenu_PreAlpha");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void NextScene()
    {
        ChangeScene(next_scene);
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
