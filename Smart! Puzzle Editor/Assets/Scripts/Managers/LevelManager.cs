using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    static public LevelManager instance;

    public enum LevelMode
    {
        Editor,
        Play
    }

    public LevelMode mode = LevelMode.Editor;

    [HideInInspector]
    public UnityEvent replacingCallbacks;
    [HideInInspector]
    public UnityEvent reActivatePE;
    public Transform mainElementsEditor;
    public Transform mainElementsPlay;
    [SerializeField]
    Text buttonText;
    [SerializeField]
    InputField levelNameField;

    [Header("Placeholder Prefabs")]
    [SerializeField]
    GameObject puzzlePH;
    [SerializeField]
    GameObject playerPH;
    [SerializeField]
    GameObject platformBoxPH;
    [SerializeField]
    GameObject platformCirclePH;
    [SerializeField]
    GameObject doorsPH;
    [SerializeField]
    GameObject movingBoxPH;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UIEditorManager.instance.mainPanel.gameObject.SetActive(mode == LevelMode.Editor);
        UIEditorManager.instance.toolsPanel.gameObject.SetActive(mode == LevelMode.Editor);
        UIEditorManager.instance.saveLoadMenu.SetActive(mode == LevelMode.Editor);
        UIManager.instance.gameObject.SetActive(mode == LevelMode.Play);
        buttonText.transform.parent.parent.gameObject.SetActive(mode == LevelMode.Editor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPlayMode()
    {
        mode = mode == LevelMode.Editor ? LevelMode.Play : LevelMode.Editor;

        UIEditorManager.instance.mainPanel.gameObject.SetActive(mode == LevelMode.Editor);
        UIEditorManager.instance.toolsPanel.gameObject.SetActive(mode == LevelMode.Editor);
        UIEditorManager.instance.saveLoadMenu.SetActive(mode == LevelMode.Editor);
        UIManager.instance.gameObject.SetActive(mode == LevelMode.Play);

        if (mode == LevelMode.Play)
        {
            replacingCallbacks.Invoke();
            PuzzleEditorController.instance.ClearCurrentPath();
            PuzzleEditorController.instance.HideAllInspectors();
            buttonText.text = "STOP PLAY";
        }
        else
        {
            buttonText.text = "PLAY MODE";

            reActivatePE.Invoke();
            for (int i = 0; i < mainElementsPlay.childCount; ++i)
            {
                Destroy(mainElementsPlay.GetChild(i).gameObject);
            }
        }
    }

    public void SaveLevel()
    {
        if (levelNameField.text != "")
        {
            List<GameObject> gameElements = new List<GameObject>();

            for (int i = 0; i < mainElementsEditor.childCount; ++i)
            {
                gameElements.Add(mainElementsEditor.GetChild(i).gameObject);
            }

            LevelBuilder.SaveLevel(levelNameField.text, gameElements, PuzzleEditorController.instance.baseTM, PuzzleEditorController.instance.collidable);
        }
    }

    public void LoadLevel()
    {
        if (levelNameField.text != "")
        {
            // Clearing Elements -----------------------------------------
            for (int i = 0; i < mainElementsEditor.childCount; ++i)
            {
                Destroy(mainElementsEditor.GetChild(i).gameObject);
            }

            PuzzleEditorController.instance.linkingObjects.Clear();
            PuzzleEditorController.instance.baseTM.ClearAllTiles();
            PuzzleEditorController.instance.collidable.ClearAllTiles();
            PuzzleEditorController.instance.pathLinks.ClearAllTiles();
            // -----------------------------------------------------------

            Level level = LevelBuilder.LoadLevel(levelNameField.text);

            // Elements
            Dictionary<GameObject, int> elementsToLink = new Dictionary<GameObject, int>();
            for (int i = 0; i < level.levelElements.Count; ++i)
            {
                ElementData ED = level.levelElements[i];
                Vector3 pos = new Vector3(ED.posX, ED.posY, ED.posZ);
                GameObject go = null;

                switch (ED.type)
                {
                    case (int)PuzzleElementPlaceHolder.PuzzleElementType.Puzzle:
                        go = Instantiate(puzzlePH, pos, puzzlePH.transform.rotation, mainElementsEditor);
                        break;
                    case (int)PuzzleElementPlaceHolder.PuzzleElementType.Player:
                        go = Instantiate(playerPH, pos, playerPH.transform.rotation, mainElementsEditor);
                        break;
                    case (int)PuzzleElementPlaceHolder.PuzzleElementType.PlatformBox:
                        go = Instantiate(platformBoxPH, pos, platformBoxPH.transform.rotation, mainElementsEditor);
                        break;
                    case (int)PuzzleElementPlaceHolder.PuzzleElementType.PlatformCircle:
                        go = Instantiate(platformCirclePH, pos, platformCirclePH.transform.rotation, mainElementsEditor);
                        break;
                    case (int)PuzzleElementPlaceHolder.PuzzleElementType.Doors:
                        go = Instantiate(doorsPH, pos, doorsPH.transform.rotation, mainElementsEditor);
                        break;
                    case (int)PuzzleElementPlaceHolder.PuzzleElementType.MovingBox:
                        go = Instantiate(movingBoxPH, pos, movingBoxPH.transform.rotation, mainElementsEditor);
                        break;
                }

                if (go)
                    go.GetComponent<PuzzleElementPlaceHolder>().ChangeState(PuzzleElementPlaceHolder.States.InLevel);

                LinkElementPlaceholder LEP = go.GetComponent<LinkElementPlaceholder>();
                if (LEP)
                {
                    var linkingObjects = PuzzleEditorController.instance.linkingObjects;
                    if (!linkingObjects.ContainsKey(go))
                    {
                        linkingObjects.Add(go, LEP.type);
                    }

                    if (ED.elementLinkedPos != -1)
                    {
                        foreach (KeyValuePair<GameObject, int> previousElement in elementsToLink)
                        {
                            if (i == previousElement.Value)
                            {
                                previousElement.Key.GetComponent<LinkElementPlaceholder>().elementLinked = go;
                                LEP.elementLinked = previousElement.Key;

                                elementsToLink.Remove(previousElement.Key);
                                break;
                            }
                        }

                        if (!LEP.elementLinked)
                        {
                            elementsToLink.Add(go, ED.elementLinkedPos);
                        }
                    }
                }
            }

            // Tiles
            AllTiles allTiles = PuzzleEditorController.instance.allTiles;

            for (int i = 0; i < level.groundTiles.Count; ++i)
            {
                TileData TD = level.groundTiles[i];
                Vector3Int pos = new Vector3Int(TD.posX, TD.posY, TD.posZ);
                PuzzleEditorController.instance.baseTM.SetTile(pos, GetTileFromInt(TD.id, allTiles));
            }

            for (int i = 0; i < level.collidableTiles.Count; ++i)
            {
                TileData TD = level.collidableTiles[i];
                Vector3Int pos = new Vector3Int(TD.posX, TD.posY, TD.posZ);
                PuzzleEditorController.instance.collidable.SetTile(pos, GetTileFromInt(TD.id, allTiles));
            }
        }
    }

    TileBase GetTileFromInt(int id, AllTiles allTiles)
    {
        TileBase tile = null;

        switch ((TileButton.Tiles)id)
        {
            case TileButton.Tiles.Ground:
                tile = allTiles.groundTile;
                break;
            case TileButton.Tiles.DarkGround:
                tile = allTiles.darkGroundTile;
                break;
            case TileButton.Tiles.VerticalWall:
                tile = allTiles.verticalWallTile;
                break;
            case TileButton.Tiles.HorizontalWall:
                tile = allTiles.horizontalWallTile;
                break;
            case TileButton.Tiles.LUCorner:
                tile = allTiles.LUCornerTile;
                break;
            case TileButton.Tiles.LDCorner:
                tile = allTiles.LDCornerTile;
                break;
            case TileButton.Tiles.RUCorner:
                tile = allTiles.RUCornerTile;
                break;
            case TileButton.Tiles.RDCorner:
                tile = allTiles.RDCornerTile;
                break;
            case TileButton.Tiles.DeadEndUp:
                tile = allTiles.DeadEndUp;
                break;
            case TileButton.Tiles.DeadEndDown:
                tile = allTiles.DeadEndDown;
                break;
            case TileButton.Tiles.DeadEndRight:
                tile = allTiles.DeadEndRight;
                break;
            case TileButton.Tiles.DeadEndLeft:
                tile = allTiles.DeadEndLeft;
                break;
            case TileButton.Tiles.DeadContinuousLeft:
                tile = allTiles.DeadContinuousLeft;
                break;
            case TileButton.Tiles.DeadContinuousRight:
                tile = allTiles.DeadContinuousRight;
                break;
            case TileButton.Tiles.DeadContinuousUp:
                tile = allTiles.DeadContinuousUp;
                break;
            case TileButton.Tiles.DeadContinuousDown:
                tile = allTiles.DeadContinuousDown;
                break;
            case TileButton.Tiles.RotatingCylinder:
                tile = allTiles.RotatingCylinder;
                break;
            case TileButton.Tiles.lightsPanel:
                tile = allTiles.LightsPanel;
                break;
        }

        return tile;
    }
}
