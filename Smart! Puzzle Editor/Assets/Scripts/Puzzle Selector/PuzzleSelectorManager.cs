using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ChangePuzzleSelectedShowing((int)PuzzlesSelected.Community);
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
