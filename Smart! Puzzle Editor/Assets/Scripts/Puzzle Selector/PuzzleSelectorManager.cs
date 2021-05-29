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
    [SerializeField]
    GameObject likeGO;
    [SerializeField]
    GameObject commentPrefab;
    [SerializeField]
    CanvasGroup selectorGroup;
    [SerializeField]
    CanvasGroup commentButtons;
    [SerializeField]
    GameObject commentMenu;
    [SerializeField]
    InputField commentField;
    [SerializeField]
    VerticalLayoutGroup communityContent;
    [SerializeField]
    Image scrollbarCommunity;
    [SerializeField]
    Image scrollbarLocal;
    [SerializeField]
    VerticalLayoutGroup communityLocal;

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
    [SerializeField]
    RectTransform commentsContent;
    [SerializeField]
    GameObject commentsGO;
    [SerializeField]
    Image scrollbar;
    [SerializeField]
    GameObject commentGOButton;

    [Header("Delete Confirmation Menu")]
    [SerializeField]
    GameObject deleteConfirmationMenu;

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

        likeGO.SetActive(level.type == LevelInfo.LevelType.Online);
        commentsGO.SetActive(level.type == LevelInfo.LevelType.Online);
        commentGOButton.SetActive(level.type == LevelInfo.LevelType.Online);

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

            for (int i = 0; i < commentsContent.childCount; ++i)
            {
                Destroy(commentsContent.GetChild(i).gameObject);
            }

            for (int i = 0; i < level.comments.Count; ++i)
            {
                CommentStructure cs = Instantiate(commentPrefab, commentsContent).GetComponent<CommentStructure>();
                cs.SetCommentInfo(level.comments[i].Key, level.comments[i].Value);
            }

            scrollbar.enabled = level.comments.Count > 2;
            commentsContent.GetComponent<VerticalLayoutGroup>().padding.right = level.comments.Count > 2 ? 50 : 20;

            Invoke(nameof(SetCommentsSize), 0.1f);
        }

        editButton.SetActive(level.type == LevelInfo.LevelType.Local);
        deleteButton.SetActive(level.username == currentUsername || level.type == LevelInfo.LevelType.Local);
        lastLevelShown = level;
    }

    public void SetCommentsSize()
    {
        Vector2 size = commentsContent.sizeDelta;
        size.y = commentsContent.GetComponent<VerticalLayoutGroup>().preferredHeight;
        commentsContent.sizeDelta = size;
    }

    public void SetCommunityLevelsSize()
    {
        RectTransform cRT = communityContent.GetComponent<RectTransform>();
        Vector2 size = cRT.sizeDelta;
        size.y = communityContent.preferredHeight;
        cRT.sizeDelta = size;
    }

    public void SetLocalLevelsSize()
    {
        RectTransform cRT = communityLocal.GetComponent<RectTransform>();
        Vector2 size = cRT.sizeDelta;
        size.y = communityLocal.preferredHeight;
        cRT.sizeDelta = size;

        ChangePuzzleSelectedShowing((int)PuzzlesSelected.Community);
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
                pl.levelToLoad = BinarySaveSystem.LoadFile<Level>(Path.Combine(Application.persistentDataPath, "Levels", lastLevelShown.id));
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

                while (levelsData[lastChar] != levelSeparator)
                {
                    data += levelsData[lastChar];
                    ++lastChar;
                }

                levelInfo.usersLiked = data;
                data = "";
                ++lastChar;

                levelInfo.comments = new List<KeyValuePair<string, string>>();
                string comment = "";

                while (levelsData[lastChar] != '/')
                {
                    ++lastChar;

                    while (levelsData[lastChar] != 'ª')
                    {
                        comment += levelsData[lastChar];
                        ++lastChar;
                    }

                    ++lastChar;

                    while (levelsData[lastChar] != '|' && levelsData[lastChar] != '/')
                    {
                        data += levelsData[lastChar];
                        ++lastChar;
                    }

                    levelInfo.comments.Add(new KeyValuePair<string, string>(data, comment));
                    data = "";
                    comment = "";
                }
            }

            ++lastChar;
            LevelSummary ls = Instantiate(levelSummary, communityContent.transform).GetComponent<LevelSummary>();
            levelInfo.levelSummary = ls;
            ls.ApplyInfo(levelInfo);
            levelInfo = new LevelInfo(LevelInfo.LevelType.Online);
        }


        communityContent.padding.right = levels > 12 ? 60 : 15;
        scrollbarCommunity.enabled = levels > 12;
        Invoke(nameof(SetCommunityLevelsSize), 0.1f);
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
        ShowDeleteConfirmationMenu();

        if (lastLevelShown.type == LevelInfo.LevelType.Online)
        {
            DataTransferer.instance.DeleteLevel(lastLevelShown.id);
        }
        else if (lastLevelShown.type == LevelInfo.LevelType.Local)
        {
            if (File.Exists(Path.Combine(Application.persistentDataPath, "Levels", lastLevelShown.id)))
            {
                File.Delete(Path.Combine(Application.persistentDataPath, "Levels", lastLevelShown.id));
            }
        }

        Destroy(lastLevelShown.levelSummary.gameObject);
        ApplyLevelInfo(null, null, true);
    }

    public void LoadSavedLevels()
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels");
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
            LevelSummary ls = Instantiate(levelSummary, communityLocal.transform).GetComponent<LevelSummary>();
            levelInfo.levelSummary = ls;

            Level level = BinarySaveSystem.LoadFile<Level>(levels[i]);

            levelInfo.id = Path.GetFileName(levels[i]);
            levelInfo.levelname = level.name;
            levelInfo.size = level.size;
            levelInfo.username = level.creatorName;
            levelInfo.description = level.description;

            ls.ApplyInfo(levelInfo);
        }

        communityLocal.padding.right = levels.Length > 12 ? 60 : 15;
        scrollbarLocal.enabled = levels.Length > 12;
        Invoke(nameof(SetLocalLevelsSize), 0.1f);
    }

    public void EditLevel()
    {
        PuzzleLoader pl = Instantiate(puzzleLoader).GetComponent<PuzzleLoader>();
        pl.loadMode = LevelManager.LevelMode.Editor;
        pl.levelToLoad = BinarySaveSystem.LoadFile<Level>(Path.Combine(Application.persistentDataPath, "Levels", lastLevelShown.id));
    }

    public void ShowCommentMenu()
    {
        commentMenu.SetActive(!commentMenu.activeSelf);
        commentButtons.interactable = commentMenu.activeSelf;
        selectorGroup.interactable = !commentMenu.activeSelf;
    }

    public void PublishComment()
    {
        DataTransferer.instance.PublishComment(lastLevelShown.id, commentField.text, currentUsername);
    }

    public void CreateComment(bool abort = false)
    {
        ShowCommentMenu();

        if (abort)
        {
            commentField.text = "";
            return;
        }

        CommentStructure cs = Instantiate(commentPrefab, commentsContent).GetComponent<CommentStructure>();
        cs.SetCommentInfo(currentUsername, commentField.text);

        lastLevelShown.comments.Add(new KeyValuePair<string, string>(currentUsername, commentField.text));

        scrollbar.enabled = lastLevelShown.comments.Count > 2;
        commentsContent.GetComponent<VerticalLayoutGroup>().padding.right = lastLevelShown.comments.Count > 2 ? 50 : 20;

        Invoke(nameof(SetCommentsSize), 0.1f);

        commentField.text = "";
    }

    public void ShowDeleteConfirmationMenu()
    {
        deleteConfirmationMenu.SetActive(!deleteConfirmationMenu.activeSelf);
        selectorGroup.interactable = !deleteConfirmationMenu.activeSelf;
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
    public List<KeyValuePair<string, string>> comments;
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
