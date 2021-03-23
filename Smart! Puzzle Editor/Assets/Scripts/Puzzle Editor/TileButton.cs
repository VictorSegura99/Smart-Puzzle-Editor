using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileButton : MonoBehaviour
{
    public enum Tiles
    {
        Ground,
        VerticalWall,
        HorizontalWall,
        LUCorner,
        LDCorner,
        RUCorner,
        RDCorner,

        None = -1
    }

    [SerializeField]
    Tiles tileType = Tiles.None;

    public void TileSelected()
    {
        PuzzleEditorController.instance.TileToPaintSelected(tileType);
    }
}
