using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Buttons : MonoBehaviour
{
    // Components

    // Inspector Variables

    // Internal Variables


 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenTutorialMenu()
    {
        GameObject.Find("Canvas").GetComponent<Main_Menu_Manager>().ChangeMenu(Main_Menu_Manager.Menu_States.TUTORIAL);
    }


    public void OpenPuzzleEditor()
    {
        SceneManager.LoadScene("PuzzleEditor");
    }

    public void ContactMe()
    {
        Application.OpenURL("https://victorsegura99.github.io/Personal_Website/");
    }
}
