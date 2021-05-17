using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleSelectorManager : MonoBehaviour
{
    enum PuzzlesSelected
    {
        Community,
        Saved,

        None = -1
    }

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
}
