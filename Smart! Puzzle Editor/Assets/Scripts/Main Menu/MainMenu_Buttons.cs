using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void ContactMe()
    {
        Application.OpenURL("www.victorsegurablanco.com");
    }
}
