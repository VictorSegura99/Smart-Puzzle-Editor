using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Buttons : MonoBehaviour
{
    // Components

    // Inspector Variables

    // Internal Variables

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenTutorialMenu()
    {
        Main_Menu_Manager.instance.ChangeMenu(Main_Menu_Manager.Menu_States.TUTORIAL);
    }
    
    public void OpenPuzzleSelector()
    {
        Main_Menu_Manager.instance.ChangeMenu(Main_Menu_Manager.Menu_States.Selector);
    }

    public void OpenPuzzleEditor()
    {
        SceneManager.LoadScene("PuzzleEditor");
    }

    public void ReturnMainMenu()
    {
        Main_Menu_Manager.instance.ChangeMenu(Main_Menu_Manager.Menu_States.MAIN);
    }

    public void ContactMe()
    {
        Application.OpenURL("https://victorsegura99.github.io/Personal_Website/");
    }
}
