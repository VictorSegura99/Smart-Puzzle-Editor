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

    Tilemap desiredTM;
    Tilemap eraseTM;
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
        }

        if (Input.GetMouseButtonDown(1)) 
        {
            if (collidable.GetTile(mousePos))
            {
                eraseTM = collidable;
            }
            else if (baseTM.GetTile(mousePos))
            {
                eraseTM = baseTM;
            }
        }

        if (Input.GetMouseButton(1) && eraseTM)
        {
            if (eraseTM.GetTile(mousePos))
            {
                eraseTM.SetTile(mousePos, null);
            }
        }

        if(Input.GetMouseButtonUp(1))
        {
            eraseTM = null;
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
        }
    }
}
