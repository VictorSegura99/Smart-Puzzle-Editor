using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PuzzleEditorController : MonoBehaviour
{
    static public PuzzleEditorController instance;

    public enum Tools
    {
        Arrow = 0,
        Brush = 1,
        Eraser = 2,
        Dropper = 3
    }

    public enum Function
    {
        Painting,
        Linking,

        None = -1
    }

    [Header("External Elements")]
    public Camera cam;

    [Header("Tilemaps")]
    public Tilemap baseTM;
    public Tilemap collidable;
    [SerializeField]
    Tilemap HLTilemap;
    public Tilemap pathLinks;

    [Header("Cursors")]
    [SerializeField]
    Texture2D standardCursor;
    [SerializeField]
    Texture2D brushCursor;
    [SerializeField]
    Texture2D eraserCursor;
    [SerializeField]
    Texture2D dropperCursor;

    public AllTiles allTiles;

    [Header("Lines Tiles")]
    [SerializeField]
    Tile[] pathLine;
    [SerializeField]
    Tile[] pathCorner;

    Tilemap desiredTM;
    TileBase currentSelectedTile;
    TileButton.Tiles selectedTile;
    Vector3Int previousCoordinate;
    bool mouseBlockedByUI = false;
    GameObject currentPuzzleElementSelected = null;

    [HideInInspector]
    public Dictionary<GameObject, LinkElementPlaceholder.LinkElementType> linkingObjects = new Dictionary<GameObject, LinkElementPlaceholder.LinkElementType>();
    [HideInInspector]
    public GameObject currentActivatorShowed = null;
    LinkElementPlaceholder currentLinkingObject = null;

    Tools currentTool = Tools.Arrow;
    int previousTool = 0;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        selectedTile = TileButton.Tiles.None;
        currentSelectedTile = allTiles.blankCursorTile;
        Cursor.SetCursor(standardCursor, Vector2.zero, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseBlockedByUI || LevelManager.instance.mode != LevelManager.LevelMode.Editor)
        {
            return;
        }

        Vector3Int mousePos = HLTilemap.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));

        // Change Tool to EyeDropper
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            ChangeTool((int)Tools.Dropper);
        }

        switch (currentTool)
        {
            case Tools.Arrow:

                    if (mousePos != previousCoordinate)
                    {
                        HLTilemap.SetTile(previousCoordinate, null);
                        HLTilemap.SetTile(mousePos, allTiles.blankArrowToolTile);
                        previousCoordinate = mousePos;
                    }

                    if (Input.GetMouseButton(0) && currentPuzzleElementSelected)
                    {
                        currentPuzzleElementSelected.GetComponent<PuzzleElementPlaceHolder>().ChangeState(PuzzleElementPlaceHolder.States.InLevel);

                        LinkElementPlaceholder LEP = currentPuzzleElementSelected.GetComponent<LinkElementPlaceholder>();
                        if (LEP)
                        {
                            if (!linkingObjects.ContainsKey(currentPuzzleElementSelected))
                                linkingObjects.Add(currentPuzzleElementSelected, LEP.type);
                        }

                        currentPuzzleElementSelected = null;
                    }

                    if (Input.GetMouseButton(1) && currentPuzzleElementSelected)
                    {
                        Destroy(currentPuzzleElementSelected);
                        currentPuzzleElementSelected = null;
                    }

                break;
            case Tools.Brush:

                if (mousePos != previousCoordinate)
                {
                    HLTilemap.SetTile(previousCoordinate, null);
                    HLTilemap.SetTile(mousePos, currentSelectedTile);
                    previousCoordinate = mousePos;
                }

                // Paint Tiles
                if (Input.GetMouseButton(0) && currentSelectedTile != allTiles.blankCursorTile)
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

                // Erase Tiles
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

                // Reset Brush
                if (Input.GetMouseButtonDown(2))
                {
                    selectedTile = TileButton.Tiles.None;
                    currentSelectedTile = allTiles.blankCursorTile;
                    previousCoordinate = new Vector3Int(-999, -999, -999);
                }

                break;
            case Tools.Eraser:

                if (mousePos != previousCoordinate)
                {
                    HLTilemap.SetTile(previousCoordinate, null);
                    HLTilemap.SetTile(mousePos, allTiles.blankArrowToolTile);
                    previousCoordinate = mousePos;
                }

                if (Input.GetMouseButton(0))
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

                break;
            case Tools.Dropper:

                if (Input.GetKeyUp(KeyCode.LeftAlt) && previousTool != -1 && previousTool != (int)Tools.Dropper)
                {
                    ChangeTool(previousTool);
                }

                if (mousePos != previousCoordinate)
                {
                    HLTilemap.SetTile(previousCoordinate, null);
                    HLTilemap.SetTile(mousePos, allTiles.blankArrowToolTile);
                    previousCoordinate = mousePos;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                    if (hit.collider && hit.collider.CompareTag("Placeholder"))
                    {
                        currentPuzzleElementSelected = Instantiate(hit.collider.gameObject);
                        currentPuzzleElementSelected.GetComponent<PuzzleElementPlaceHolder>().ChangeState(PuzzleElementPlaceHolder.States.Selected);
                        ChangeTool((int)Tools.Arrow);

                        return;
                    }

                    TileBase tileSelected = null;

                    if (collidable.GetTile(mousePos))
                    {
                        tileSelected = collidable.GetTile(mousePos);
                        desiredTM = collidable;
                    }

                    if (baseTM.GetTile(mousePos))
                    {
                        tileSelected = baseTM.GetTile(mousePos);
                        desiredTM = baseTM;
                    }

                    if (tileSelected)
                    {
                        currentSelectedTile = tileSelected;
                        ChangeTool((int)Tools.Brush);
                    }
                }

                break;
        }
    }

    public void TileToPaintSelected(TileButton.Tiles tile)
    {
        if (tile == selectedTile)
        {
            currentSelectedTile = allTiles.blankCursorTile;
            selectedTile = TileButton.Tiles.None;

            return;
        }

        selectedTile = tile;

        switch (tile)
        {
            case TileButton.Tiles.Ground:
                currentSelectedTile = allTiles.groundTile;
                desiredTM = baseTM;
                break;
            case TileButton.Tiles.DarkGround:
                currentSelectedTile = allTiles.darkGroundTile;
                desiredTM = baseTM;
                break;
            case TileButton.Tiles.VerticalWall:
                currentSelectedTile = allTiles.verticalWallTile;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.HorizontalWall:
                currentSelectedTile = allTiles.horizontalWallTile;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.LUCorner:
                currentSelectedTile = allTiles.LUCornerTile;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.LDCorner:
                currentSelectedTile = allTiles.LDCornerTile;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.RUCorner:
                currentSelectedTile = allTiles.RUCornerTile;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.RDCorner:
                currentSelectedTile = allTiles.RDCornerTile;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadEndUp:
                currentSelectedTile = allTiles.DeadEndUp;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadEndDown:
                currentSelectedTile = allTiles.DeadEndDown;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadEndRight:
                currentSelectedTile = allTiles.DeadEndRight;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadEndLeft:
                currentSelectedTile = allTiles.DeadEndLeft;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadContinuousLeft:
                currentSelectedTile = allTiles.DeadContinuousLeft;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadContinuousRight:
                currentSelectedTile = allTiles.DeadContinuousRight;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadContinuousUp:
                currentSelectedTile = allTiles.DeadContinuousUp;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.DeadContinuousDown:
                currentSelectedTile = allTiles.DeadContinuousDown;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.RotatingCylinder:
                currentSelectedTile = allTiles.RotatingCylinder;
                desiredTM = collidable;
                break;
            case TileButton.Tiles.lightsPanel:
                currentSelectedTile = allTiles.LightsPanel;
                desiredTM = collidable;
                break;
        }

        if (currentTool != Tools.Brush)
            ChangeTool(1);
    }

    public void BlockMouse(bool block)
    {
        mouseBlockedByUI = block;

        if (block)
        {
            HLTilemap.SetTile(previousCoordinate, null);
            previousCoordinate = new Vector3Int(-999, -999, -999);
            if (currentPuzzleElementSelected)
            {
                currentPuzzleElementSelected.SetActive(false);
            }
        }
        else
        {
            if (currentPuzzleElementSelected && currentTool == Tools.Arrow)
            {
                currentPuzzleElementSelected.SetActive(true);
            }
        }
    }

    public void PuzzleElementSelected(GameObject GO, bool instantiate = true)
    {
        if (currentPuzzleElementSelected != null)
        {
            Destroy(currentPuzzleElementSelected);
        }

        if (GO != null && instantiate)
        {
            currentPuzzleElementSelected = Instantiate(GO, LevelManager.instance.mainElementsEditor);
            if (mouseBlockedByUI)
            {
                currentPuzzleElementSelected.SetActive(false);
            }
        }
        else if (!instantiate)
        {
            currentPuzzleElementSelected = GO;
        }

        if (currentTool != Tools.Arrow)
            ChangeTool(0);
    }

    public void ChangeTool(int newTool)
    {
        if (newTool != (int)currentTool)
        {
            previousTool = (int)currentTool;
            currentTool = (Tools)newTool;

            switch (currentTool)
            {
                case Tools.Arrow:
                    Cursor.SetCursor(standardCursor, Vector2.zero, CursorMode.Auto);
                    if (currentPuzzleElementSelected && !currentPuzzleElementSelected.activeSelf)
                        currentPuzzleElementSelected.SetActive(true);
                    break;
                case Tools.Brush:
                    Cursor.SetCursor(brushCursor, new Vector2(0, brushCursor.height - 60), CursorMode.Auto);
                    if (currentPuzzleElementSelected && currentPuzzleElementSelected.activeSelf)
                        currentPuzzleElementSelected.SetActive(false);
                    break;
                case Tools.Eraser:
                    Cursor.SetCursor(eraserCursor, new Vector2(0, eraserCursor.height - 126), CursorMode.Auto);
                    if (currentPuzzleElementSelected && currentPuzzleElementSelected.activeSelf)
                        currentPuzzleElementSelected.SetActive(false);
                    break;
                case Tools.Dropper:
                    Cursor.SetCursor(dropperCursor, new Vector2(-11, dropperCursor.height - 11), CursorMode.Auto);
                    if (currentPuzzleElementSelected && currentPuzzleElementSelected.activeSelf)
                        currentPuzzleElementSelected.SetActive(false);
                    break;
            }
        }
        else
        {
            if (currentTool == Tools.Brush)
            {
                selectedTile = TileButton.Tiles.None;
                currentSelectedTile = allTiles.blankCursorTile;
            }
        }
    }

    public Tools GetCurrentTool()
    {
        return currentTool;
    }

    public void LinkElement(LinkElementPlaceholder GOToLink)
    {
        if (currentLinkingObject)
        {
            // Different Link types, link
            if (currentLinkingObject.type != GOToLink.type)
            {
                if (!GOToLink.elementLinked)
                {
                    GOToLink.elementLinked = currentLinkingObject.gameObject;
                    currentLinkingObject.buttonText.text = "Unlink";
                    currentLinkingObject.elementLinked = GOToLink.gameObject;
                    GOToLink.buttonText.text = "Unlink";
                    currentLinkingObject.ShowCanvas(true);
                    currentLinkingObject = null;

                    foreach (KeyValuePair<GameObject, LinkElementPlaceholder.LinkElementType> linkingGO in linkingObjects)
                    {
                        LinkElementPlaceholder LEP = linkingGO.Key.GetComponent<LinkElementPlaceholder>();
                        if (LEP != GOToLink && LEP != GOToLink.elementLinked.GetComponent<LinkElementPlaceholder>())
                            LEP.ShowCanvas(false);
                    }

                    GameObject activator = GOToLink.type == LinkElementPlaceholder.LinkElementType.Activator ? GOToLink.gameObject : GOToLink.elementLinked.gameObject;
                    currentActivatorShowed = activator;
                    DrawLine(TileMapPathfinding.FindPathCoordinates(V3Helper.V3ToV3Int(activator.transform.position), V3Helper.V3ToV3Int(activator.GetComponent<LinkElementPlaceholder>().elementLinked.transform.position)));
                }
                else
                {
                    currentLinkingObject.ShowCanvas(false);
                    currentLinkingObject = null;

                    LinkElementPlaceholder anotherLep = GOToLink.elementLinked.GetComponent<LinkElementPlaceholder>();
                    anotherLep.ShowCanvas(true);
                    anotherLep.elementLinked = null;
                    anotherLep.buttonText.text = "Link";

                    GOToLink.elementLinked = null;
                    GOToLink.buttonText.text = "Link";
                }
            }
            else
            {
                // Same Link types different objects, substitute
                if (currentLinkingObject != GOToLink)
                {
                    currentLinkingObject.buttonText.text = "Link";
                    GOToLink.buttonText.text = "Cancel";
                    currentLinkingObject = GOToLink;
                }
                // Same object, cancel linking process
                else
                {
                    currentLinkingObject.buttonText.text = "Link";
                    currentLinkingObject = null;

                    HideAllInspectors();
                }
            }
        }
        else
        {
            // The element is linked, unlink
            if (GOToLink.elementLinked)
            {
                LinkElementPlaceholder anotherLep = GOToLink.elementLinked.GetComponent<LinkElementPlaceholder>();
                anotherLep.elementLinked = null;
                anotherLep.buttonText.text = "Link";

                GOToLink.elementLinked = null;
                GOToLink.buttonText.text = "Link";

                currentActivatorShowed = null;
                ClearPath(TileMapPathfinding.FindPathCoordinates(V3Helper.V3ToV3Int(GOToLink.transform.position), V3Helper.V3ToV3Int(anotherLep.transform.position)));
            }
            // No element Linked, start linking
            else
            {
                currentLinkingObject = GOToLink;
                currentLinkingObject.buttonText.text = "Cancel";

                foreach (KeyValuePair<GameObject, LinkElementPlaceholder.LinkElementType> linkingGO in linkingObjects)
                {
                    LinkElementPlaceholder LEP = linkingGO.Key.GetComponent<LinkElementPlaceholder>();
                    if (LEP != GOToLink && !LEP.elementLinked && LEP.type != GOToLink.type)
                        LEP.ShowCanvas(true);
                }
            }
        }
    }

    public void MovedLinkingObject(LinkElementPlaceholder LEP, Vector3 oldPos)
    {
        GameObject activator = LEP.type == LinkElementPlaceholder.LinkElementType.Activator ? LEP.gameObject : LEP.elementLinked.gameObject;

        if (currentActivatorShowed)
        {
            if (activator != LEP.gameObject)
            {
                ClearPath(TileMapPathfinding.FindPathCoordinates(V3Helper.V3ToV3Int(activator.transform.position), V3Helper.V3ToV3Int(oldPos)));
            }
            else
            {
                ClearPath(TileMapPathfinding.FindPathCoordinates(V3Helper.V3ToV3Int(oldPos), V3Helper.V3ToV3Int(LEP.elementLinked.transform.position)));
            }
        }

        DrawLine(TileMapPathfinding.FindPathCoordinates(V3Helper.V3ToV3Int(activator.transform.position), V3Helper.V3ToV3Int(activator.GetComponent<LinkElementPlaceholder>().elementLinked.transform.position)));
        currentActivatorShowed = activator;
        LEP.canvas.gameObject.SetActive(true);
        LEP.elementLinked.GetComponent<LinkElementPlaceholder>().canvas.gameObject.SetActive(true);
    }

    public void DeleteLinkingObject(LinkElementPlaceholder LGO)
    {
        if (LGO.elementLinked && currentActivatorShowed)
        {
            ClearPath(TileMapPathfinding.FindPathCoordinates(V3Helper.V3ToV3Int(LGO.transform.position), V3Helper.V3ToV3Int(LGO.elementLinked.transform.position)));
            currentActivatorShowed = null;
        }

        if (linkingObjects.ContainsKey(LGO.gameObject))
        {
            linkingObjects.Remove(LGO.gameObject);
        }
    }

    public void ShowPath(LinkElementPlaceholder elementClicked)
    {
        if (currentActivatorShowed)
        {
            ClearPath(TileMapPathfinding.FindPathCoordinates(V3Helper.V3ToV3Int(currentActivatorShowed.transform.position), V3Helper.V3ToV3Int(currentActivatorShowed.GetComponent<LinkElementPlaceholder>().elementLinked.transform.position)));
        }

        foreach (KeyValuePair<GameObject, LinkElementPlaceholder.LinkElementType> pair in linkingObjects)
        {
            if (pair.Key != elementClicked.gameObject && pair.Key != elementClicked.elementLinked.gameObject)
            {
                LinkElementPlaceholder LEP = pair.Key.GetComponent<LinkElementPlaceholder>();
                if (LEP.canvas.gameObject.activeSelf)
                    LEP.canvas.gameObject.SetActive(false);
            }
        }

        GameObject activator = elementClicked.type == LinkElementPlaceholder.LinkElementType.Activator ? elementClicked.gameObject : elementClicked.elementLinked.gameObject;

        currentActivatorShowed = activator;
        DrawLine(TileMapPathfinding.FindPathCoordinates(V3Helper.V3ToV3Int(activator.transform.position), V3Helper.V3ToV3Int(activator.GetComponent<LinkElementPlaceholder>().elementLinked.transform.position)));
    }

    public void HidePath(LinkElementPlaceholder elementClicked)
    {
        GameObject activator = elementClicked.type == LinkElementPlaceholder.LinkElementType.Activator ? elementClicked.gameObject : elementClicked.elementLinked.gameObject;

        ClearPath(TileMapPathfinding.FindPathCoordinates(V3Helper.V3ToV3Int(currentActivatorShowed.transform.position), V3Helper.V3ToV3Int(currentActivatorShowed.GetComponent<LinkElementPlaceholder>().elementLinked.transform.position)));
        currentActivatorShowed = null;
    }

    void DrawLine(List<Vector3Int> path)
    {
        Vector3Int came_from = path[0];
        Vector3Int going_to = path[1] - path[0];

        for (int i = 0; i < path.Count - 1; ++i)
        {
            if (i == 0 || i == path.Count - 1)
            {
                continue;
            }

            came_from = path[i - 1] - path[i];
            going_to = path[i + 1] - path[i];
            

            Vector3Int direction = came_from + going_to;
            // Straight Line
            if (direction == Vector3Int.zero)
            {
                if (came_from.x != 0)
                {
                    pathLinks.SetTile(path[i], pathLine[0]);
                }
                else
                {
                    pathLinks.SetTile(path[i], pathLine[1]);
                }
            }
            // Turn
            else
            {
                if (direction.x < 0)
                {
                    if (direction.y < 0)
                    {
                        pathLinks.SetTile(path[i], pathCorner[0]);
                    }
                    else
                    {
                        pathLinks.SetTile(path[i], pathCorner[1]);
                    }
                }
                else
                {
                    if (direction.y > 0)
                    {
                        pathLinks.SetTile(path[i], pathCorner[2]);
                    }
                    else
                    {
                        pathLinks.SetTile(path[i], pathCorner[3]);
                    }
                }
            }
        }
    }

    void ClearPath(List<Vector3Int> path)
    {
        if (path.Count > 0)
        {
            for (int i = 0; i < path.Count; ++i)
            {
                pathLinks.SetTile(path[i], null);
            }
        }
    }

    public void ClearCurrentPath()
    {
        if (currentActivatorShowed)
        {
            ClearPath(TileMapPathfinding.FindPathCoordinates(V3Helper.V3ToV3Int(currentActivatorShowed.transform.position), V3Helper.V3ToV3Int(currentActivatorShowed.GetComponent<LinkElementPlaceholder>().elementLinked.transform.position)));
            currentActivatorShowed = null;
        }
    }

    public void HideAllInspectors()
    {
        foreach (KeyValuePair<GameObject, LinkElementPlaceholder.LinkElementType> pair in linkingObjects)
        {
            LinkElementPlaceholder LEP = pair.Key.GetComponent<LinkElementPlaceholder>();
            if (LEP.canvas.gameObject.activeSelf)
                LEP.canvas.gameObject.SetActive(false);
        }

        ClearCurrentPath();
    }
}

[Serializable]
public class AllTiles
{
    public Tile blankArrowToolTile;
    public Tile blankCursorTile;
    public Tile groundTile;
    public Tile darkGroundTile;
    public Tile verticalWallTile;
    public Tile horizontalWallTile;
    public Tile LUCornerTile;
    public Tile LDCornerTile;
    public Tile RUCornerTile;
    public Tile RDCornerTile;
    public Tile DeadEndUp;
    public Tile DeadEndDown;
    public Tile DeadEndRight;
    public Tile DeadEndLeft;
    public Tile DeadContinuousLeft;
    public Tile DeadContinuousRight;
    public Tile DeadContinuousUp;
    public Tile DeadContinuousDown;
    public AnimatedTile RotatingCylinder;
    public Tile LightsPanel;
}
