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

    LevelInfo level = new LevelInfo();

    public void SummaryClicked()
    {
        PuzzleSelectorManager.instance.ApplyLevelInfo(level, this);
    }

    public void ApplyInfo(LevelInfo info)
    {
        level = info;

        levelName.text = level.levelname;

        string likesS = level.likesNumber < 10 ? "0" + level.likesNumber.ToString() : level.likesNumber.ToString();
        likes.text = likesS;
    }
}
