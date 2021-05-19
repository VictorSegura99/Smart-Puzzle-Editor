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

    public void DownloadLevel(string id)
    {
        StartCoroutine(StartDownloadLevel(id));
    }

    IEnumerator StartDownloadLevel(string id)
    {
        string url = serverURL + "levels/" + id;

        UnityWebRequest w = UnityWebRequest.Get(url);

        yield return w.SendWebRequest();

        if (w.error != null)
        {
            Debug.Log("Error: " + w.error);
        }
        else
        {
            PuzzleLoader puzzleLoader = Instantiate(PuzzleSelectorManager.instance.puzzleLoader).GetComponent<PuzzleLoader>();
            puzzleLoader.sceneToLoad = "PuzzleEditor";
            puzzleLoader.levelToLoad = (Level)BinarySaveSystem.ByteArrayToObject(w.downloadHandler.data);
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
        w.AddField("levelSize", level.size);

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

    public void GetLevels()
    {
        StartCoroutine(GetAllLevelsInfo());
    }

    IEnumerator GetAllLevelsInfo()
    {
        string url = serverURL + "GetAllLevelsInfo.php";
        UnityWebRequest w = UnityWebRequest.Get(url);

        yield return w.SendWebRequest();

        if (w.error != null)
        {
            Debug.Log("Error: " + w.error);
        }
        else
        {
            PuzzleSelectorManager.instance.ApplyAllLevelsData(w.downloadHandler.text);
        }
        w.Dispose();
    }

    public void LikeLevel(int id, string currentUsername, LevelInfo level)
    {
        StartCoroutine(LikeLevelProcess(id, currentUsername, level));
    }

    IEnumerator LikeLevelProcess(int id, string currentUsername, LevelInfo level)
    {
        string url = serverURL + "CheckUserLike.php";
        
        WWWForm w = new WWWForm();
        w.AddField("id", id);
        
        using (UnityWebRequest www = UnityWebRequest.Post(url, w))
        {
            yield return www.SendWebRequest();

            if (www.error != null)
            {
                Debug.Log("404 not found");
                yield break;
            }
            else if (www.downloadHandler.text.Contains("Error"))
            {
                Debug.Log(www.downloadHandler.text);
                yield break;
            }
            else
            {
                string user = "";
                string usersLiked = www.downloadHandler.text;

                for (int i = 0; i < usersLiked.Length; ++i)
                {
                    if (usersLiked[i] == ',')
                    {
                        if (user == currentUsername)
                        {
                            int l = user.Length + 1;
                            int j = i - user.Length;
                            List<char> users = new List<char>(usersLiked);

                            while (l > 0)
                            {
                                users.Remove(users[j]);
                                --l;
                            }

                            //Dislike
                            StartCoroutine(AddLike(id, -1, new string(users.ToArray()), level));
                            yield break;
                        }

                        user = "";
                        continue;
                    }

                    user += usersLiked[i];
                }

                usersLiked += currentUsername + ",";

                // Like
                StartCoroutine(AddLike(id, 1, usersLiked, level));
            }
        }
    }

    IEnumerator AddLike(int id, int likeChange, string newUsersList, LevelInfo level)
    {
        string url = serverURL + "AddLike.php";
        WWWForm w = new WWWForm();
        w.AddField("id", id);
        w.AddField("likeChange", likeChange);
        w.AddField("usersList", newUsersList);

        using (UnityWebRequest vv = UnityWebRequest.Post(url, w))
        {
            yield return vv.SendWebRequest();

            if (vv.error != null)
            {
                Debug.Log("404 not found");
                yield break;
            }
            else if (vv.downloadHandler.text.Contains("Error"))
            {
                Debug.Log(vv.downloadHandler.text);
                yield break;
            }
            else
            {
                PuzzleSelectorManager.instance.UpdateLikeCount(int.Parse(vv.downloadHandler.text), level);
            }
        }
    }
}
