using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PuzzleSelectorManager : MonoBehaviour
{
    static public PuzzleSelectorManager instance;

    enum PuzzlesSelected
    {
        Community,
        Saved,

        None = -1
    }

    [Header("Game Objects")]
    public GameObject puzzleLoader;
    [SerializeField]
    GameObject levelSummary;
    [SerializeField]
    Transform communityPanel;
    [SerializeField]
    Transform savedPanel;

    [Header("Buttons")]
    [SerializeField]
    Button communityButton;
    [SerializeField]
    Button savedButton;
    [SerializeField]
    Sprite selectedButtonSprite;
    [SerializeField]
    Sprite nonSelectedButtonSprite;
    PuzzlesSelected currentPuzzlesShowing = PuzzlesSelected.None;

    [Header("Puzzle Info")]
    [SerializeField]
    Text levelname;
    [SerializeField]
    Text size;
    [SerializeField]
    Text username;
    [SerializeField]
    Text description;
    [SerializeField]
    Text likes;

    LevelInfo lastLevelShown;
    string currentUsername = "";

    private void Awake()
    {
        instance = this;

        if (File.Exists(Path.Combine(Application.persistentDataPath, "Data", "playerAccount.data")))
        {
            currentUsername = BinarySaveSystem.LoadFile<AccountFile>(Path.Combine(Application.persistentDataPath, "Data", "playerAccount.data")).Username;
        }
    }

    private void Start()
    {
        ChangePuzzleSelectedShowing((int)PuzzlesSelected.Community);
        DataTransferer.instance.GetLevels();
    }

    public void ChangePuzzleSelectedShowing(int selectedType)
    {
        PuzzlesSelected newType = (PuzzlesSelected)selectedType;

        if (newType == currentPuzzlesShowing)
        {
            return;
        }

        switch (newType)
        {
            case PuzzlesSelected.Community:
                communityButton.image.sprite = selectedButtonSprite;
                savedButton.image.sprite = nonSelectedButtonSprite;
                break;
            case PuzzlesSelected.Saved:
                communityButton.image.sprite = nonSelectedButtonSprite;
                savedButton.image.sprite = selectedButtonSprite;
                break;
        }

        currentPuzzlesShowing = newType;
    }

    public void ApplyLevelInfo(LevelInfo level)
    {
        levelname.text = level.levelname;
        size.text = "Size: " + level.size + "x" + level.size;
        username.text = "MADE BY " + level.username;
        description.text = level.description;

        string likesS = level.likesNumber < 10 ? "0" + level.likesNumber.ToString() : level.likesNumber.ToString();
        likes.text = likesS;

        lastLevelShown = level;
    }

    public void PlayLevel()
    {
        DataTransferer.instance.DownloadLevel(lastLevelShown.id);
    }

    public void ApplyAllLevelsData(string levelsData)
    {
        int levels = int.Parse(levelsData[0].ToString());
        int lastChar = 2;
        LevelInfo levelInfo = new LevelInfo();

        char levelSeparator = '|';

        for (int i = 0; i < levels; ++i)
        {
            while (levelsData[lastChar] != '/')
            {
                string data = "";
                while (levelsData[lastChar] != levelSeparator)
                {
                    data += levelsData[lastChar];
                    ++lastChar;
                }

                levelInfo.id = data;
                data = "";
                ++lastChar;

                while (levelsData[lastChar] != levelSeparator)
                {
                    data += levelsData[lastChar];
                    ++lastChar;
                }

                levelInfo.levelname = data;
                data = "";
                ++lastChar;

                while (levelsData[lastChar] != levelSeparator)
                {
                    data += levelsData[lastChar];
                    ++lastChar;
                }

                levelInfo.description = data;
                data = "";
                ++lastChar;

                while (levelsData[lastChar] != levelSeparator)
                {
                    data += levelsData[lastChar];
                    ++lastChar;
                }

                levelInfo.likesNumber = int.Parse(data);
                data = "";
                ++lastChar;

                while (levelsData[lastChar] != levelSeparator)
                {
                    data += levelsData[lastChar];
                    ++lastChar;
                }

                levelInfo.username = data;
                data = "";
                ++lastChar;

                while (levelsData[lastChar] != '/')
                {
                    data += levelsData[lastChar];
                    ++lastChar;
                }

                levelInfo.size = int.Parse(data);
            }

            ++lastChar;
            LevelSummary ls = Instantiate(levelSummary, communityPanel).GetComponent<LevelSummary>();
            ls.ApplyInfo(levelInfo);
            levelInfo = new LevelInfo();
        }
    }

    public void LikeLevel()
    {
        if (lastLevelShown != null)
            DataTransferer.instance.LikeLevel(int.Parse(lastLevelShown.id), currentUsername, lastLevelShown);
    }

    public void UpdateLikeCount(int likeCount, LevelInfo level)
    {
        if (level != lastLevelShown)
        {
            return;
        }

        string likesS = likeCount < 10 ? "0" + likeCount.ToString() : likeCount.ToString();
        likes.text = likesS;
        lastLevelShown.likesNumber = likeCount;
    }
}

[System.Serializable]
public class LevelInfo
{
    public string id;
    public string levelname;
    public string username;
    public string description;
    public int likesNumber;
    public int size;

    public LevelInfo() { }

    public LevelInfo(string id, string LevelName, string Username, string Description, int LikesNumber, int size)
    {
        this.id = id;
        levelname = LevelName;
        username = Username;
        description = Description;
        likesNumber = LikesNumber;
        this.size = size;
    }
}
