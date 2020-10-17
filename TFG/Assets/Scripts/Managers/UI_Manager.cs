using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class UI_Manager : MonoBehaviour
{
    // Components
    Animator youwin_anim;
    Player_Controller player;

    // Elements to change Alpha in PuzzleContinue Fade
    Text[] text_to_fade;

    // Inspector Variables
    public GameObject YouWin_Menu;
    public GameObject Pause_Menu;
    public GameObject Start_Puzzle_Continue_Menu;
    public bool start_puzzlepiece_menu = false;

    // Internal Variables
    GamePadState controller_current_state;
    GamePadState controller_last_frame_state;
    bool can_puzzlecontinue_continue = false;

    // Timer for PuzzleMenu Continue
    [SerializeField]
    float seconds_wait_puzzlemenu_continue = 2.0f;
    [SerializeField]
    float seconds_fade_puzzlemenu_continue = 1.0f;
    float puzzlemenu_timer_start = 0.0f;

    // Timer for Hold R to Reset
    float time_start = 0.0f;
    float time_to_hold = 1.0f;
    bool R_hold = false;


    // Enums
    enum Active_Menu
    {
        NONE,
        PAUSE,
        YOU_WIN,
        PUZZLEPIECE
    }
    Active_Menu current_menu = Active_Menu.NONE;

    // Start is called before the first frame update
    void Start()
    {
        youwin_anim = YouWin_Menu.GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<Player_Controller>();
        Pause_Menu.SetActive(false);
        controller_current_state = GamePad.GetState(0);

        if (start_puzzlepiece_menu)
        {
            Start_Puzzle_Continue_Menu.SetActive(true);
            Time.timeScale = 0;
            current_menu = Active_Menu.PUZZLEPIECE;
            text_to_fade = Start_Puzzle_Continue_Menu.GetComponentsInChildren<Text>();
            puzzlemenu_timer_start = Time.realtimeSinceStartup;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ManageMenuInput();

        if (start_puzzlepiece_menu)
        {
            HandlePuzzleContinueMenu();
        }
    }

    public void ShowYouWinMenu()
    {
        current_menu = Active_Menu.YOU_WIN;
        youwin_anim.SetTrigger("Enter");
    }

    public void HideYouWinMenu()
    {
        current_menu = Active_Menu.NONE;
        youwin_anim.SetTrigger("Exit");
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        current_menu = Active_Menu.PAUSE;
        Pause_Menu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        current_menu = Active_Menu.NONE;
        Pause_Menu.SetActive(false);
    }

    void HandlePuzzleContinueMenu()
    {
        if (Time.realtimeSinceStartup > puzzlemenu_timer_start + seconds_wait_puzzlemenu_continue)
        {
            start_puzzlepiece_menu = false;
            StartCoroutine(FadePuzzleContinueSpace());
        }
    }

    IEnumerator FadePuzzleContinueSpace()
    {
        puzzlemenu_timer_start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < puzzlemenu_timer_start + seconds_fade_puzzlemenu_continue)
        {
            float t = (Time.realtimeSinceStartup - puzzlemenu_timer_start) / seconds_fade_puzzlemenu_continue;

            // Int i starts in 1 to skip the main text: Touch (PuzzlePiece) to win!
            for (int i = 1; i < text_to_fade.Length; ++i)
            {
                text_to_fade[i].color = new Color(text_to_fade[i].color.r, text_to_fade[i].color.g, text_to_fade[i].color.b, t);
            }

            yield return null;
        }

        can_puzzlecontinue_continue = true;
    }

    void QuitPuzzleContinueMenu()
    {
        Time.timeScale = 1;
        Start_Puzzle_Continue_Menu.SetActive(false);
        current_menu = Active_Menu.NONE;
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
            case Active_Menu.PUZZLEPIECE:
                {
                    if (Input.GetKeyDown(KeyCode.Space) && can_puzzlecontinue_continue)
                    {
                        QuitPuzzleContinueMenu();
                    }
                    break;
                }
        }
    }
}
