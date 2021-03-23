using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PuzzleEditorController : MonoBehaviour
{
    static public PuzzleEditorController instance;

    [Header("External Elements")]
    [SerializeField]
    Camera cam;

    [Header("Tilemaps")]
    [SerializeField]
    Tilemap baseTM;
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

    Tilemap desiredTM;
    Tile currentSelectedTile;
    Vector3Int previousCoordinate;

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
        Vector3Int mousePos = HLTilemap.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));

        if (mousePos != previousCoordinate)
        {
            HLTilemap.SetTile(previousCoordinate, null);
            HLTilemap.SetTile(mousePos, currentSelectedTile);
            previousCoordinate = mousePos;
        }

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

    public void TileToPaintSelected(TileButton.Tiles tile)
    {
        switch (tile)
        {
            case TileButton.Tiles.Ground:
                currentSelectedTile = groundTile;
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
        }
    }
}
