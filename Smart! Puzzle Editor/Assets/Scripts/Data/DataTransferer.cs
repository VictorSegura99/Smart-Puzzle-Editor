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
            puzzleLoader.loadMode = LevelManager.LevelMode.Play;
            puzzleLoader.levelToLoad = (Level)BinarySaveSystem.ByteArrayToObject(w.downloadHandler.data);
        }
        w.Dispose();
    }

    public void UploadLevel(Level level)
    {
        int id = Random.Range(0, 1000000);

        StartCoroutine(CheckLevelID(level, id));
    }

    IEnumerator StartUploadingLevel(Level level, int id)
    {
        WWWForm w = new WWWForm();
        w.AddField("id", id);
        w.AddField("levelName", level.name);
        w.AddField("levelDescription", level.description);
        w.AddField("likes", 0);
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
        form.AddBinaryData("myfile", BinarySaveSystem.ObjectToByteArray(level), id.ToString(), "text/plain");

        UnityWebRequest wL = UnityWebRequest.Post(serverURL + "index.php", form);
        yield return wL.SendWebRequest();

        LevelManager.instance.ShowPublishLevelMenu(false);

        if (wL.error != null)
        {
            LevelManager.instance.ShowSuccessMenu(wL.error);
        }
        else
        {
            LevelManager.instance.ShowSuccessMenu();
        }

        wL.Dispose();
    }

    IEnumerator CheckLevelID(Level level, int id)
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
                StartCoroutine(StartUploadingLevel(level, id));
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
                if (int.Parse(numbers[i]) == id)
                {
                    id = Random.Range(0, 1000000);
                    StartCoroutine(CheckLevelID(level, id));
                    yield break;
                }
            }

            StartCoroutine(StartUploadingLevel(level, id));
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
                level.usersLiked = newUsersList;
                level.likesNumber = int.Parse(vv.downloadHandler.text);
                PuzzleSelectorManager.instance.UpdateLikeCount(level);
            }
        }
    }

    public void DeleteLevel(string id)
    {
        StartCoroutine(DeleteLevelProcess(id));
    }

    IEnumerator DeleteLevelProcess(string id)
    {
        string fileURL = serverURL + "DeleteLevelFile.php";
        string dataURL = serverURL + "DeleteLevelData.php";

        WWWForm w = new WWWForm();
        w.AddField("id", id);

        using (UnityWebRequest www = UnityWebRequest.Post(dataURL, w))
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
        }

        UnityWebRequest wL = UnityWebRequest.Post(fileURL, w);
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

    public void PublishComment(string id, string comment, string username)
    {
        StartCoroutine(StartPublishingComment(id, comment, username));
    }

    IEnumerator StartPublishingComment(string id, string comment, string username)
    {
        string url = serverURL + "PublishComment.php";

        WWWForm w = new WWWForm();
        w.AddField("id", id);
        w.AddField("username", username);
        w.AddField("comment", comment);

        using (UnityWebRequest www = UnityWebRequest.Post(url, w))
        {
            yield return www.SendWebRequest();

            if (www.error != null || www.downloadHandler.text.Contains("Error"))
            {
                PuzzleSelectorManager.instance.CreateComment(true);
            }
            else
            {
                PuzzleSelectorManager.instance.CreateComment();
            }
        }
    }
}
