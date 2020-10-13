using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class UI_Manager : MonoBehaviour
{
    // Components
    Animator youwin_anim;
    Player_Controller player;

    // Inspector Variables
    public GameObject YouWin_Menu;

    // Internal Variables

    // Enums
    enum Active_Menu
    {
        NONE,
        PAUSE,
        YOU_WIN
    }
    Active_Menu current_menu = Active_Menu.NONE;

    // Start is called before the first frame update
    void Start()
    {
        youwin_anim = YouWin_Menu.GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<Player_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        // Input Management
        switch (current_menu)
        {
            case Active_Menu.NONE:
                {
                    if (Input.GetKeyDown(KeyCode.Escape) || GamePad.GetState(0).Buttons.Start == ButtonState.Pressed)
                    {
                        PauseGame();
                    }
                    break;
                }
            case Active_Menu.PAUSE:
                {
                    if (Input.GetKeyDown(KeyCode.Escape) || GamePad.GetState(0).Buttons.Start == ButtonState.Pressed || GamePad.GetState(0).Buttons.B == ButtonState.Pressed)
                    {
                        ResumeGame();
                    }
                    break;
                }
            case Active_Menu.YOU_WIN:
                {
                    if (Input.GetKeyDown(KeyCode.Escape) || GamePad.GetState(0).Buttons.Start == ButtonState.Pressed || GamePad.GetState(0).Buttons.B == ButtonState.Pressed)
                    {
                        HideYouWinMenu();
                    }
                    break;
                }
        }
    }

    public void ShowYouWinMenu()
    {
        current_menu = Active_Menu.YOU_WIN;
        youwin_anim.SetTrigger("Enter");
    }

    public void HideYouWinMenu()
    {
        ChangeMenuState(Active_Menu.NONE);
        youwin_anim.SetTrigger("Exit");
    }

    void ChangeMenuState(Active_Menu new_menu)
    {
        if (current_menu != new_menu)
        {
            current_menu = new_menu;
            //if (new_menu != Active_Menu.NONE)
            //{
            //    if (!player.IsInputBlocked())
            //    {
            //        player.BlockInput(true);
            //    }
            //}
            //else
            //{
            //    if (player.IsInputBlocked())
            //    {
            //        player.BlockInput(false);
            //    }
            //}
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        ChangeMenuState(Active_Menu.YOU_WIN);
        // Show Pause Menu
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        ChangeMenuState(Active_Menu.NONE);
    }
}
