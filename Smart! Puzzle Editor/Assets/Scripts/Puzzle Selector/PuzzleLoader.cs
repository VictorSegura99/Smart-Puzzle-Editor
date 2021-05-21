using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleLoader : MonoBehaviour
{
    [HideInInspector]
    public Level levelToLoad;
    [HideInInspector]
    public LevelManager.LevelMode loadMode;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("PuzzleEditor");
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.instance && LevelManager.instance.isReady)
        {
            LevelManager.instance.LoadThisLevel(levelToLoad);
            LevelManager.instance.ChangeMode(loadMode);
            Destroy(gameObject);
        }
    }
}
