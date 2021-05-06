using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    static public LevelManager instance;

    public enum LevelMode
    {
        SelectingSize,
        Editor,
        Play
    }

    public LevelMode mode = LevelMode.SelectingSize;

    [SerializeField]
    PixelPerfectCamera cam;
    [HideInInspector]
    public UnityEvent replacingCallbacks;
    [HideInInspector]
    public UnityEvent reActivatePE;
    public Transform mainElementsEditor;
    public Transform mainElementsPlay;
    [SerializeField]
    Text buttonText;
    public InputField levelNameField;

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
    GameObject VLDoorsPH;
    [SerializeField]
    GameObject VRDoorsPH;
    [SerializeField]
    GameObject movingBoxPH;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeMode(mode);
    }

    public void ChangeMode(LevelMode newMode)
    {
        switch (newMode)
        {
            case LevelMode.SelectingSize:
                break;
            case LevelMode.Editor:
                buttonText.text = "PLAY MODE";
                if (!buttonText.transform.parent.parent.gameObject.activeSelf)
                    buttonText.transform.parent.parent.gameObject.SetActive(true);

                reActivatePE.Invoke();
                for (int i = 0; i < mainElementsPlay.childCount; ++i)
                {
                    Destroy(mainElementsPlay.GetChild(i).gameObject);
                }
                break;
            case LevelMode.Play:
                replacingCallbacks.Invoke();
                PuzzleEditorController.instance.ClearCurrentPath();
                PuzzleEditorController.instance.HideAllInspectors();
                buttonText.text = "STOP PLAY";
                break;
        }

        UIEditorManager.instance.selectingSizeMenu.SetActive(newMode == LevelMode.SelectingSize);
        UIEditorManager.instance.mainPanel.gameObject.SetActive(newMode == LevelMode.Editor);
        UIEditorManager.instance.toolsPanel.gameObject.SetActive(newMode == LevelMode.Editor);
        UIEditorManager.instance.saveLoadMenu.SetActive(newMode == LevelMode.Editor);
        UIEditorManager.instance.resetLevelMenu.SetActive(newMode == LevelMode.Editor);
        UIManager.instance.gameObject.SetActive(newMode == LevelMode.Play);
        PuzzleEditorController.instance.sizeLimit.gameObject.SetActive(newMode == LevelMode.Editor);

        mode = newMode;
    }

    public void StartPlayMode()
    {
        if (mode == LevelMode.Editor)
        {
            ChangeMode(LevelMode.Play);
            return;
        }

        ChangeMode(LevelMode.Editor);
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

            LevelBuilder.SaveLevel(levelNameField.text, PuzzleEditorController.instance.levelSize, gameElements, PuzzleEditorController.instance.baseTM, PuzzleEditorController.instance.collidable);
        }
    }

    public void LoadLevel()
    {
        if (levelNameField.text != "")
        {
            Level level = LevelBuilder.LoadLevel(levelNameField.text);

            if (level == null)
            {
                return;
            }

            // Clearing Elements -----------------------------------------
            for (int i = 0; i < mainElementsEditor.childCount; ++i)
            {
                Destroy(mainElementsEditor.GetChild(i).gameObject);
            }

            PuzzleEditorController.instance.linkingObjects.Clear();
            PuzzleEditorController.instance.baseTM.ClearAllTiles();
            PuzzleEditorController.instance.collidable.ClearAllTiles();
            PuzzleEditorController.instance.pathLinks.ClearAllTiles();
            PuzzleEditorController.instance.sizeLimit.ClearAllTiles();
            // -----------------------------------------------------------

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
                    case (int)PuzzleElementPlaceHolder.PuzzleElementType.VerticalLeftDoors:
                        go = Instantiate(VLDoorsPH, pos, VLDoorsPH.transform.rotation, mainElementsEditor);
                        break;
                    case (int)PuzzleElementPlaceHolder.PuzzleElementType.VerticalRightDoors:
                        go = Instantiate(VRDoorsPH, pos, VRDoorsPH.transform.rotation, mainElementsEditor);
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

            PuzzleEditorController.instance.SetSize(level.size);
            SetCameraSize(level.size);

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

    public void SetMapSize(int size)
    {
        SetCameraSize(size);
        PuzzleEditorController.instance.SetSize(size);
        ChangeMode(LevelMode.Editor);
    }

    public void SetCameraSize(int tilesSize)
    {
        switch (tilesSize)
        {
            case 8:
                cam.refResolutionX = 320;
                cam.refResolutionY = 180;
                break;
            case 16:
                cam.refResolutionX = (int)(320 * 1.75f);
                cam.refResolutionY = (int)(180 * 1.75f);
                break;
            case 24:
                cam.refResolutionX = 320 * 3;
                cam.refResolutionY = 180 * 3;
                break;
        }
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
