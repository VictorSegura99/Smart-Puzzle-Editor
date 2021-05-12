﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Main_Menu_Manager : MonoBehaviour
{
    // Components

    // Inspector Variables
    //    - Menus
    public GameObject main_menu;
    public GameObject tutorial_menu;

    [Header("LogIn Menu")]
    [SerializeField]
    InputField nicknameField;
    [SerializeField]
    InputField passwordField;
    [SerializeField]
    Button LogInButton;
    [SerializeField]
    Text errorMessages;

    [Header("Register Menu")]
    [SerializeField]
    InputField nicknameField_register;
    [SerializeField]
    InputField passwordField_register;
    [SerializeField]
    Button RegisterButton;
    [SerializeField]
    Text errorMessages_register;

    // Internal Variables
    public enum Menu_States
    {
        MAIN,
        TUTORIAL,
    }
    Menu_States current_state = Menu_States.MAIN;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (current_state)
        {
            case Menu_States.MAIN:
                {
                    break;
                }
            case Menu_States.TUTORIAL:
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        ChangeMenu(Menu_States.MAIN);
                    }
                    break;
                }
        }
    }

    public void ChangeMenu(Menu_States state_to_change)
    {
        if (current_state != state_to_change)
        {
            current_state = state_to_change;

            switch (state_to_change)
            {
                case Menu_States.MAIN:
                    main_menu.SetActive(true);
                    tutorial_menu.SetActive(false);
                    break;
                case Menu_States.TUTORIAL:
                    main_menu.SetActive(false);
                    tutorial_menu.SetActive(true);
                    break;
            }
        }
    }


    // Buttons
    public void LogIn()
    {
        StartCoroutine(BeginLogin());
    }

    IEnumerator BeginLogin()
    {
        LogInButton.interactable = false;

        string url = DataTransferer.serverURL + "Login.php";

        WWWForm w = new WWWForm();
        w.AddField("username", nicknameField.text);
        w.AddField("Password", passwordField.text);

        using (UnityWebRequest www = UnityWebRequest.Post(url, w))
        {
            yield return www.SendWebRequest();

            if (www.error != null)
            {
                errorMessages.text = "404 not found";
            }
            else
            {
                if (www.isDone)
                {
                    errorMessages.gameObject.SetActive(true);

                    if (www.downloadHandler.text.Contains("Error") && !www.downloadHandler.text.Contains("Success"))
                    {
                        errorMessages.text = www.downloadHandler.text;
                        errorMessages.color = Color.red;
                    }
                    else
                    {
                        errorMessages.text = "Welcome!";
                        errorMessages.color = Color.green;
                    }
                }
            }
        }

        LogInButton.interactable = true;
    }

    public void Register()
    {
        StartCoroutine(BeginRegister());
    }

    IEnumerator BeginRegister()
    {
        RegisterButton.interactable = false;

        string url = DataTransferer.serverURL + "Register.php";

        WWWForm w = new WWWForm();
        w.AddField("username", nicknameField_register.text);
        w.AddField("Password", passwordField_register.text);

        using (UnityWebRequest www = UnityWebRequest.Post(url, w))
        {
            yield return www.SendWebRequest();

            if (www.error != null)
            {
                errorMessages_register.text = "404 not found";
            }
            else
            {
                if (www.isDone)
                {
                    errorMessages_register.gameObject.SetActive(true);

                    if (www.downloadHandler.text.Contains("Error") && !www.downloadHandler.text.Contains("Success"))
                    {
                        errorMessages_register.text = www.downloadHandler.text;
                        errorMessages_register.color = Color.red;
                        Debug.Log(errorMessages_register.text);
                    }
                    else
                    {
                        errorMessages_register.text = "Welcome!";
                        errorMessages_register.color = Color.green;
                    }
                }
            }
        }

        RegisterButton.interactable = true;
    }
}
