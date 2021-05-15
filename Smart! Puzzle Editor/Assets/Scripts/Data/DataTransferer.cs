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

    public void UploadLevel(Level level)
    {
        level.id = Random.Range(0, 1000000);
        level.likes = 0;
        level.creatorName = LevelManager.username;

        StartCoroutine(CheckLevelID(level));
    }

    IEnumerator StartUploadingLevel(Level level)
    {
        WWWForm w = new WWWForm();
        w.AddField("id", level.id);
        w.AddField("levelName", level.name);
        w.AddField("levelDescription", level.description);
        w.AddField("likes", level.likes);
        w.AddField("creatorName", level.creatorName);

        using (UnityWebRequest www = UnityWebRequest.Post(serverURL + "AddPuzzle.php", w))
        {
            yield return www.SendWebRequest();

            if (www.error != null)
            {
                Debug.Log("404 not found");
                yield break;
            }
            else if(www.downloadHandler.text.Contains("Error"))
            {
                Debug.Log(www.downloadHandler.text);
                yield break;
            }
        }

        WWWForm form = new WWWForm();
        form.AddBinaryData("myfile", BinarySaveSystem.ObjectToByteArray(level), level.id.ToString(), "text/plain");

        UnityWebRequest wL = UnityWebRequest.Post(serverURL + "index.php", form);
        yield return wL.SendWebRequest();

        if (wL.error != null)
        {
            Debug.Log("Error: " + wL.error);
        }
        else
        {
            Debug.Log(wL.result);
        }
        wL.Dispose();
    }

    IEnumerator CheckLevelID(Level level)
    {
        UnityWebRequest w = UnityWebRequest.Get(serverURL + "GetLevelsID.php");
        yield return w.SendWebRequest();

        if (w.error != null)
        {
            Debug.Log("Error: " + w.error);
        }
        else
        {
            string ids = w.downloadHandler.text;

            if (ids == "0 results")
            {
                Debug.Log("0 results funciona");
                StartCoroutine(StartUploadingLevel(level));
                yield break;
            }

            List<string> numbers = new List<string>();
            string number = "";
            for (int i = 0; i < ids.Length; ++i)
            {
                if (ids[i] != ',')
                {
                    number += ids[i];
                }
                else
                {
                    numbers.Add(number);
                    number = "";
                }
            }

            for (int i = 0; i < numbers.Count; ++i)
            {
                if (int.Parse(numbers[i]) == level.id)
                {
                    level.id = Random.Range(0, 1000000);
                    StartCoroutine(CheckLevelID(level));
                    yield break;
                }
            }

            StartCoroutine(StartUploadingLevel(level));
        }
    }
}
