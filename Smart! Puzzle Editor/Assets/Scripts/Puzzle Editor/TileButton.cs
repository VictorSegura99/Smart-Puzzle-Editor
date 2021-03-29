using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileButton : MonoBehaviour
{
    public enum Tiles
    {
        Ground,
        DarkGround,
        VerticalWall,
        HorizontalWall,
        LUCorner,
        LDCorner,
        RUCorner,
        RDCorner,
        DeadEndRight,
        DeadEndLeft,
        DeadEndUp,
        DeadEndDown,
        DeadContinuousLeft,
        DeadContinuousRight,
        DeadContinuousUp,
        DeadContinuousDown,

        None = -1
    }

    [SerializeField]
    Tiles tileType = Tiles.None;

    public void TileSelected()
    {
        PuzzleEditorController.instance.TileToPaintSelected(tileType);
    }
}
