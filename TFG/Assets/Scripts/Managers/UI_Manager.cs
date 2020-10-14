using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

public class UI_Manager : MonoBehaviour
{
    // Components
    Animator youwin_anim;
    Player_Controller player;


    // Inspector Variables
    public GameObject YouWin_Menu;
    public GameObject Pause_Menu;


    // Internal Variables
    GamePadState controller_current_state;
    GamePadState controller_last_frame_state;

    // Timer for Hold R to Reset
    float time_start = 0.0f;
    float time_to_hold = 1.0f;
    bool R_hold = false;


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
        Pause_Menu.SetActive(false);
        controller_current_state = GamePad.GetState(0);
    }

    // Update is called once per frame
    void Update()
    {
        ManageMenuInput();
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

    public void PauseGame()
    {
        Time.timeScale = 0;
        ChangeMenuState(Active_Menu.PAUSE);
        Pause_Menu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        ChangeMenuState(Active_Menu.NONE);
        Pause_Menu.SetActive(false);
    }

    void ManageMenuInput()
    {
        // TODO: Create GameManager
        // {
        if (Input.GetKeyDown(KeyCode.R))
        {
            time_start = Time.realtimeSinceStartup;
            R_hold = true;
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            R_hold = false;
        }

        if (R_hold)
        {
            if (time_start + time_to_hold <= Time.realtimeSinceStartup)
            {
                time_start = Time.realtimeSinceStartup; if (Time.timeScale != 1)
                {
                    Time.timeScale = 1;
                }

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Application.Quit();
        //}

        // INPUT:
        controller_last_frame_state = controller_current_state;
        controller_current_state = GamePad.GetState(0);

        // Input Management
        switch (current_menu)
        {
            case Active_Menu.NONE:
                {
                    if (Input.GetKeyDown(KeyCode.Escape) || (controller_current_state.Buttons.Start == ButtonState.Pressed && controller_last_frame_state.Buttons.Start == ButtonState.Released))
                    {
                        PauseGame();
                    }
                    break;
                }
            case Active_Menu.PAUSE:
                {
                    if (Input.GetKeyDown(KeyCode.Escape) || (controller_current_state.Buttons.B == ButtonState.Pressed && controller_last_frame_state.Buttons.B == ButtonState.Released))
                    {
                        ResumeGame();
                    }
                    break;
                }
            case Active_Menu.YOU_WIN:
                {
                    //if (Input.GetKeyDown(KeyCode.Escape) || GamePad.GetState(0).Buttons.Start == ButtonState.Pressed || GamePad.GetState(0).Buttons.B == ButtonState.Pressed)
                    //{
                    //    HideYouWinMenu();
                    //}
                    break;
                }
        }
    }
}
