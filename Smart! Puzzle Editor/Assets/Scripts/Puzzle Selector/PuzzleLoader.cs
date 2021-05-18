using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleLoader : MonoBehaviour
{
    [HideInInspector]
    public string sceneToLoad = "";
    [HideInInspector]
    public Level levelToLoad;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(sceneToLoad);
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.instance && LevelManager.instance.isReady)
        {
            LevelManager.instance.LoadThisLevel(levelToLoad);
            LevelManager.instance.ChangeMode(LevelManager.LevelMode.Play);
            Destroy(gameObject);
        }
    }
}
