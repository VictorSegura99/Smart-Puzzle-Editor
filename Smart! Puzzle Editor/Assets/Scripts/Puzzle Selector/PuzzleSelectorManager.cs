using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
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
    [SerializeField]
    GameObject puzzleInfo;
    public GameObject puzzleLoader;
    [SerializeField]
    GameObject levelSummary;
    [SerializeField]
    Transform communityPanel;
    [SerializeField]
    Transform savedPanel;
    [SerializeField]
    TextMeshProUGUI heartLike;
    [SerializeField]
    GameObject deleteButton;
    [SerializeField]
    GameObject editButton;

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
        LoadSavedLevels();
        ApplyLevelInfo(null, null, true);
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

        communityPanel.gameObject.SetActive(newType == PuzzlesSelected.Community);
        savedPanel.gameObject.SetActive(newType == PuzzlesSelected.Saved);

        currentPuzzlesShowing = newType;
    }

    public void ApplyLevelInfo(LevelInfo level, LevelSummary ls, bool setActiveFalse = false)
    {
        if ((lastLevelShown != null && level == null) || setActiveFalse)
        {
            lastLevelShown = null;

            puzzleInfo.SetActive(false);            
            return;
        }
        else if (lastLevelShown == null && level != null)
        {
            puzzleInfo.SetActive(true);
        }

        levelname.text = level.levelname;
        size.text = "Size: " + level.size + "x" + level.size;
        username.text = "MADE BY " + level.username;
        description.text = level.description;

        if (level.type == LevelInfo.LevelType.Online)
        {
            string likesS = level.likesNumber < 10 ? "0" + level.likesNumber.ToString() : level.likesNumber.ToString();
            likes.text = likesS;
            ls.ApplyInfo(level);

            string user = "";
            string usersLiked = level.usersLiked;
            heartLike.color = Color.white;

            for (int i = 0; i < usersLiked.Length; ++i)
            {
                if (usersLiked[i] == ',')
                {
                    if (user == currentUsername)
                    {
                        heartLike.color = Color.green;
                    }

                    user = "";
                    continue;
                }

                user += usersLiked[i];
            }
        }

        editButton.SetActive(level.type == LevelInfo.LevelType.Local);
        deleteButton.SetActive(level.username == currentUsername || level.type == LevelInfo.LevelType.Local);
        lastLevelShown = level;
    }

    public void PlayLevel()
    {
        switch (lastLevelShown.type)
        {
            case LevelInfo.LevelType.Online:
                DataTransferer.instance.DownloadLevel(lastLevelShown.id);
                break;
            case LevelInfo.LevelType.Local:
                PuzzleLoader pl = Instantiate(puzzleLoader).GetComponent<PuzzleLoader>();
                pl.loadMode = LevelManager.LevelMode.Play;
                pl.levelToLoad = BinarySaveSystem.LoadFile<Level>(Path.Combine(Application.persistentDataPath, "Data", lastLevelShown.levelname));
                break;
        }
    }

    public void ApplyAllLevelsData(string levelsData)
    {
        int levels = int.Parse(levelsData[0].ToString());
        int lastChar = 2;
        LevelInfo levelInfo = new LevelInfo(LevelInfo.LevelType.Online);

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

                while (levelsData[lastChar] != levelSeparator)
                {
                    data += levelsData[lastChar];
                    ++lastChar;
                }

                levelInfo.size = int.Parse(data);
                data = "";
                ++lastChar;

                while (levelsData[lastChar] != '/')
                {
                    data += levelsData[lastChar];
                    ++lastChar;
                }

                levelInfo.usersLiked = data;
            }

            ++lastChar;
            LevelSummary ls = Instantiate(levelSummary, communityPanel).GetComponent<LevelSummary>();
            levelInfo.levelSummary = ls;
            ls.ApplyInfo(levelInfo);
            levelInfo = new LevelInfo(LevelInfo.LevelType.Online);
        }
    }

    public void LikeLevel()
    {
        if (lastLevelShown != null)
            DataTransferer.instance.LikeLevel(int.Parse(lastLevelShown.id), currentUsername, lastLevelShown);
    }

    public void UpdateLikeCount(LevelInfo level)
    {
        if (level != lastLevelShown)
        {
            return;
        }

        ApplyLevelInfo(level, level.levelSummary);
    }

    public void DeleteLevel()
    {
        Destroy(lastLevelShown.levelSummary.gameObject);
        ApplyLevelInfo(null, null, true);
        DataTransferer.instance.DeleteLevel(lastLevelShown.id);
    }

    public void LoadSavedLevels()
    {
        string path = Path.Combine(Application.persistentDataPath, "Data");
        if (!Directory.Exists(path))
        {
            return;
        }

        var levels = Directory.GetFiles(path);

        for (int i = 0; i < levels.Length; ++i)
        {
            if (Path.GetExtension(levels[i]) == ".data")
            {
                continue;
            }

            LevelInfo levelInfo = new LevelInfo(LevelInfo.LevelType.Local);
            LevelSummary ls = Instantiate(levelSummary, savedPanel).GetComponent<LevelSummary>();
            levelInfo.levelSummary = ls;

            Level level = BinarySaveSystem.LoadFile<Level>(levels[i]);

            levelInfo.levelname = level.name;
            levelInfo.size = level.size;
            levelInfo.username = level.creatorName;
            levelInfo.description = level.description;

            ls.ApplyInfo(levelInfo);
        }
    }

    public void EditLevel()
    {
        PuzzleLoader pl = Instantiate(puzzleLoader).GetComponent<PuzzleLoader>();
        pl.loadMode = LevelManager.LevelMode.Editor;
        pl.levelToLoad = BinarySaveSystem.LoadFile<Level>(Path.Combine(Application.persistentDataPath, "Data", lastLevelShown.levelname));
    }
}

[System.Serializable]
public class LevelInfo
{
    public enum LevelType
    {
        Online,
        Local
    }

    public string id;
    public string levelname;
    public string username;
    public string description;
    public int likesNumber;
    public int size;
    public string usersLiked;
    public LevelSummary levelSummary;
    public LevelType type;

    public LevelInfo() { }

    public LevelInfo(LevelType type)
    {
        this.type = type;
    }

    public LevelInfo(LevelType type, string id, string LevelName, string Username, string Description, int LikesNumber, int size, string usersLiked)
    {
        this.id = id;
        levelname = LevelName;
        username = Username;
        description = Description;
        likesNumber = LikesNumber;
        this.size = size;
        this.usersLiked = usersLiked;
        this.type = type;
    }
}
