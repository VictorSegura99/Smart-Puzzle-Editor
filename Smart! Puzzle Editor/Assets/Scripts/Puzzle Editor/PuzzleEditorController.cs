using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PuzzleEditorController : MonoBehaviour
{
    static public PuzzleEditorController instance;

    public enum ElementsSelected
    {
        Tile,
        Element,

        None = -1
    }

    [Header("External Elements")]
    public Camera cam;

    [Header("Tilemaps")]
    public Tilemap baseTM;
    [SerializeField]
    Tilemap collidable;
    [SerializeField]
    Tilemap HLTilemap;

    [Header("Tiles")]
    [SerializeField]
    Tile blankCursorTile;
    [SerializeField]
    Tile groundTile;
    [SerializeField]
    Tile darkGroundTile;
    [SerializeField]
    Tile verticalWallTile;
    [SerializeField]
    Tile horizontalWallTile;
    [SerializeField]
    Tile LUCornerTile;
    [SerializeField]
    Tile LDCornerTile;
    [SerializeField]
    Tile RUCornerTile;
    [SerializeField]
    Tile RDCornerTile;
    [SerializeField]
    Tile DeadEndUp;
    [SerializeField]
    Tile DeadEndDown;
    [SerializeField]
    Tile DeadEndRight;
    [SerializeField]
    Tile DeadEndLeft;
    [SerializeField]
    Tile DeadContinuousLeft;
    [SerializeField]
    Tile DeadContinuousRight;
    [SerializeField]
    Tile DeadContinuousUp;
    [SerializeField]
    Tile DeadContinuousDown;

    Tilemap desiredTM;
    Tile currentSelectedTile;
    Vector3Int previousCoordinate;
    bool mouseBlockedByUI = false;
    GameObject currentPuzzleElementSelected = null;
    
    [HideInInspector]
    public ElementsSelected elementSelected = ElementsSelected.None;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentSelectedTile = blankCursorTile;
    }

    // Update is called once per frame
    void Update()
    {
        if (!mouseBlockedByUI)
        {
            Vector3Int mousePos = HLTilemap.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));

            if (mousePos != previousCoordinate)
            {
                HLTilemap.SetTile(previousCoordinate, null);
                HLTilemap.SetTile(mousePos, currentSelectedTile);
                previousCoordinate = mousePos;
            }

            switch (elementSelected)
            {
                case ElementsSelected.Tile:

                    if (Input.GetMouseButton(0) && currentSelectedTile != blankCursorTile)
                    {
                        desiredTM.SetTile(mousePos, currentSelectedTile);

                        if (desiredTM != baseTM)
                        {
                            baseTM.SetTile(mousePos, null);
                        }

                        if (desiredTM != collidable)
                        {
                            collidable.SetTile(mousePos, null);
                        }
                    }
                    break;

                case ElementsSelected.Element:

                    if (Input.GetMouseButton(0) && currentPuzzleElementSelected)
                    {
                        currentPuzzleElementSelected.GetComponent<PuzzleElementPlaceHolder>().ChangeState(PuzzleElementPlaceHolder.States.InLevel);
                        currentPuzzleElementSelected = null;
                        elementSelected = ElementsSelected.None;
                    }
                    break;

                case ElementsSelected.None:
                    break;
            }

            if (Input.GetMouseButton(1))
            {
                if (collidable.GetTile(mousePos))
                {
                    collidable.SetTile(mousePos, null);
                }

                if (baseTM.GetTile(mousePos))
                {
                    baseTM.SetTile(mousePos, null);
                }
            }
        }
    }

    public void TileToPaintSelected(TileButton.Tiles tile)
    {
        switch (tile)
        {
            case TileButton.Tiles.Ground:
                currentSelectedTile = groundTile;
                desiredTM = baseTM;
                break;
            case TileButton.Tiles.DarkGround:
                currentSelectedTile = darkGroundTile;
                desiredTM = baseTM;
                break;
            case TileButton.Tiles.VerticalWall:
                currentSelectedTile = verticalWallTile;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.HorizontalWall:
                currentSelectedTile = horizontalWallTile;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.LUCorner:
                currentSelectedTile = LUCornerTile;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.LDCorner:
                currentSelectedTile = LDCornerTile;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.RUCorner:
                currentSelectedTile = RUCornerTile;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.RDCorner:
                currentSelectedTile = RDCornerTile;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadEndUp:
                currentSelectedTile = DeadEndUp;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadEndDown:
                currentSelectedTile = DeadEndDown;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadEndRight:
                currentSelectedTile = DeadEndRight;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadEndLeft:
                currentSelectedTile = DeadEndLeft;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadContinuousLeft:
                currentSelectedTile = DeadContinuousLeft;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadContinuousRight:
                currentSelectedTile = DeadContinuousRight;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadContinuousUp:
                currentSelectedTile = DeadContinuousUp;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadContinuousDown:
                currentSelectedTile = DeadContinuousDown;
                desiredTM = collidable;
                break;
        }

        elementSelected = ElementsSelected.Tile;
    }

    public void BlockMouse(bool block)
    {
        mouseBlockedByUI = block;

        if (block)
        {
            HLTilemap.SetTile(previousCoordinate, null);
            previousCoordinate = new Vector3Int(-999, -999, -999);
            if (currentPuzzleElementSelected != null)
            {
                currentPuzzleElementSelected.SetActive(false);
            }
        }
        else
        {
            if (currentPuzzleElementSelected != null)
            {
                currentPuzzleElementSelected.SetActive(true);
            }
        }
    }

    public void PuzzleElementSelected(GameObject GO)
    {
        if (currentPuzzleElementSelected != null)
        {
            Destroy(currentPuzzleElementSelected);
        }

        if (GO != null)
        {
            currentPuzzleElementSelected = Instantiate(GO);
            if (mouseBlockedByUI)
            {
                currentPuzzleElementSelected.SetActive(false);
            }
        }

        currentSelectedTile = blankCursorTile;
        elementSelected = ElementsSelected.Element;
    }
}
