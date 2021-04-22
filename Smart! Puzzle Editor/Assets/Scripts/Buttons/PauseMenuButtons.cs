﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButtons : MonoBehaviour
{
    // Components

    // Inspector Variables
    public string next_scene;

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


    }
}
