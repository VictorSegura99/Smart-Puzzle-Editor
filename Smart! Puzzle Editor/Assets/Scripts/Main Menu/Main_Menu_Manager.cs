using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class Main_Menu_Manager : MonoBehaviour
{
    public static Main_Menu_Manager instance;
    static public string accountDataPath = "";
    static bool firstTime = true;

    // Inspector Variables
    //    - Menus
    public GameObject main_menu;
    public GameObject tutorial_menu;
    [SerializeField]
    GameObject puzzleSelector;

    [Header("LogIn Menu")]
    [SerializeField]
    GameObject loginMenu;
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
    GameObject registerMenu;
    [SerializeField]
    InputField nicknameField_register;
    [SerializeField]
    InputField passwordField_register;
    [SerializeField]
    InputField repeatedPasswordField_register;
    [SerializeField]
    Button RegisterButton;
    [SerializeField]
    Text errorMessages_register;

    [Header("Account Detected Menu")]
    [SerializeField]
    GameObject accountDetectedMenu;
    [SerializeField]
    Text accountText;
    [SerializeField]
    Button acceptButton;
    [SerializeField]
    Button logOutButton;

    // Internal Variables
    AccountFile accountFile = null;

    public enum Menu_States
    {
        MAIN,
        TUTORIAL,
        REGISTER,
        LOGIN,
        ACCOUNTDETECTED,
        Selector
    }

    Menu_States current_state = Menu_States.LOGIN;

    private void Awake()
    {
        instance = this;

        accountDataPath = Path.Combine(Application.persistentDataPath, "Data", "playerAccount.data");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (File.Exists(accountDataPath))
        {
            accountFile = BinarySaveSystem.LoadFile<AccountFile>(accountDataPath);
            
            if (!firstTime)
            {
                ChangeMenu(Menu_States.Selector);
                return;
            }

            ChangeMenu(Menu_States.ACCOUNTDETECTED);

            return;
        }

        ChangeMenu(current_state);
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
        if (firstTime)
            firstTime = false;

        current_state = state_to_change;

        switch (state_to_change)
        {
            case Menu_States.MAIN:
                main_menu.SetActive(true);
                tutorial_menu.SetActive(false);
                loginMenu.SetActive(false);
                accountDetectedMenu.SetActive(false);
                puzzleSelector.SetActive(false);
                break;
            case Menu_States.TUTORIAL:
                main_menu.SetActive(false);
                tutorial_menu.SetActive(true);
                break;
            case Menu_States.LOGIN:
                LogInButton.interactable = true;
                main_menu.SetActive(false);
                registerMenu.SetActive(false);
                loginMenu.SetActive(true);
                accountDetectedMenu.SetActive(false);
                break;
            case Menu_States.REGISTER:
                registerMenu.SetActive(true);
                loginMenu.SetActive(false);
                break;
            case Menu_States.ACCOUNTDETECTED:
                accountText.text = "Account Detected: " + accountFile.Username;
                loginMenu.SetActive(false);
                accountDetectedMenu.SetActive(true);
                acceptButton.interactable = true;
                logOutButton.interactable = true;
                break;
            case Menu_States.Selector:
                main_menu.SetActive(false);
                puzzleSelector.SetActive(true);
                break;
        }
    }


    // Buttons
    public void LogIn()
    {
        if (nicknameField.text != "" && passwordField.text != "")
        {
            LogInButton.interactable = false;
            StartCoroutine(BeginLogin(nicknameField.text, passwordField.text));
        }
    }
    
    public void LogOut()
    {
        if (File.Exists(accountDataPath))
        {
            File.Delete(accountDataPath);
        }

        ChangeMenu(Menu_States.LOGIN);
    }

    IEnumerator BeginLogin(string username, string password)
    {
        string url = DataTransferer.serverURL + "Login.php";

        WWWForm w = new WWWForm();
        w.AddField("username", username);
        w.AddField("Password", password);

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
                        ChangeMenu(Menu_States.MAIN);
                        if (!File.Exists(accountDataPath))
                            SaveAccountFile(username, password);
                    }
                }
            }
        }
    }

    public void Register()
    {
        if (nicknameField_register.text == "" || passwordField_register.text == "")
        {
            return;
        }

        if (repeatedPasswordField_register.text != passwordField_register.text)
        {
            errorMessages_register.text = "Passwords don't match";
            errorMessages_register.color = Color.red;
            errorMessages_register.gameObject.SetActive(true);
            return;
        }

        StartCoroutine(BeginRegister(nicknameField_register.text, passwordField_register.text));
    }

    IEnumerator BeginRegister(string username, string password)
    {
        RegisterButton.interactable = false;

        string url = DataTransferer.serverURL + "Register.php";

        WWWForm w = new WWWForm();
        w.AddField("username", username);
        w.AddField("Password", password);

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
                        Invoke("GoToLogIn", 0.25f);
                    }
                }
            }
        }

        RegisterButton.interactable = true;
    }

    void SaveAccountFile(string username, string password)
    {
        BinarySaveSystem.SaveFile(accountDataPath, new AccountFile(username, password));
    }

    public void EnterAccount()
    {
        acceptButton.interactable = false;
        logOutButton.interactable = false;
        StartCoroutine(BeginLogin(accountFile.Username, accountFile.Password));
    }

    public void GoToRegister()
    {
        nicknameField.text = "";
        passwordField.text = "";
        ChangeMenu(Menu_States.REGISTER);
    }

    void GoToLogIn()
    {
        nicknameField_register.text = "";
        passwordField_register.text = "";
        repeatedPasswordField_register.text = "";
        ChangeMenu(Menu_States.LOGIN);
    }

    public void ChangePasswordVisibility(InputField inputField)
    {
        inputField.contentType = inputField.contentType == InputField.ContentType.Password ? InputField.ContentType.Standard : InputField.ContentType.Password;
        inputField.ForceLabelUpdate();
    }

    public void BlockStrangeChars(InputField inputField)
    {
        if (inputField.text.Length < 1)
        {
            return;
        }

        if (inputField.text[inputField.text.Length - 1] == '/' || inputField.text[inputField.text.Length - 1] == '|' || inputField.text[inputField.text.Length - 1] == 'ª')
        {
            inputField.text = inputField.text.Remove(inputField.text.Length - 1);
        }
    }
}

[System.Serializable]
public class AccountFile
{
    public string Username;
    public string Password;

    public AccountFile(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
