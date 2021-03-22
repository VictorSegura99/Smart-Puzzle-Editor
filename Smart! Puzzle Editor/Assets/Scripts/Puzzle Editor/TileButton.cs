using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileButton : MonoBehaviour
{
    public enum Tiles
    {
        Ground,
        VerticalWall,
        HorizontalWall
    }

    public void TileSelected(int tileNumber)
    {
        PuzzleEditorController.instance.TileToPaintSelected((Tiles)tileNumber);
    }
}
