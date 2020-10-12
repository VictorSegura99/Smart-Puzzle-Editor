using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Menu_Manager : MonoBehaviour
{
    // Components

    // Inspector Variables
    //    - Menus
    public GameObject main_menu;
    public GameObject tutorial_menu;

    // Internal Variables
    enum Menu_States
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
        
    }

    void ChangeMenu(Menu_States state_to_change)
    {
        if (current_state != state_to_change)
        {
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
}
