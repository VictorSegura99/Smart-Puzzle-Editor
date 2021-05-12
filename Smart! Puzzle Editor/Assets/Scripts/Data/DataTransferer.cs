using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Networking;

public class DataTransferer : MonoBehaviour
{
    static public DataTransferer instance;

    static public string serverURL = "https://smartpuzzleshoster.000webhostapp.com/";

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UploadFile(string filename, byte[] content)
    {
        StartCoroutine(StartUploadingFile(filename, content));
    }

    IEnumerator StartUploadingFile(string name, byte[] content)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("myfile", content, name, "text/plain");

        UnityWebRequest w = UnityWebRequest.Post(serverURL + "index.php", form);
        yield return w.SendWebRequest();

        if (w.error != null)
        {
            Debug.Log("Error: " + w.error);
        }
        else
        {
            Debug.Log(w.result);
        }
        w.Dispose();
    }

    public void DownloadLevel(string filename)
    {
        StartCoroutine(StartDownloadLevel(filename));
    }

    IEnumerator StartDownloadLevel(string filename)
    {
        string url = serverURL + "levels/" + filename;

        UnityWebRequest w = UnityWebRequest.Get(url);

        yield return w.SendWebRequest();

        if (w.error != null)
        {
            Debug.Log("Error: " + w.error);
        }
        else
        {
            LevelManager.instance.LoadThisLevel((Level)BinarySaveSystem.ByteArrayToObject(w.downloadHandler.data));
        }
        w.Dispose();
    }
}
