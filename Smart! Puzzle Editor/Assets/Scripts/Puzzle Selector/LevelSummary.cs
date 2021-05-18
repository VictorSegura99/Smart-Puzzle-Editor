using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSummary : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    Text levelName;
    [SerializeField]
    Text likes;

    [SerializeField]
    LevelInfo level;

    // Start is called before the first frame update
    void Start()
    {
        levelName.text = level.levelname;

        string likesS = level.likesNumber < 10 ? "0" + level.likesNumber.ToString() : level.likesNumber.ToString();
        likes.text = likesS;
    }

    public void SummaryClicked()
    {
        PuzzleSelectorManager.instance.ApplyLevelInfo(level);
    }
}
