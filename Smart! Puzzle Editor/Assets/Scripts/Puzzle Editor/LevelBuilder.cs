using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public static class LevelBuilder
{
    static public void SaveLevel(string levelName, List<GameObject> gameObjects, Tilemap ground, Tilemap collidable)
    {
        Level levelTS = new Level();

        for (int i = 0; i < gameObjects.Count; ++i)
        {
            if (gameObjects[i].GetComponent<LinkElementPlaceholder>())
            {
                LinkElementPlaceholder LEP = gameObjects[i].GetComponent<LinkElementPlaceholder>();

                int refPos = -1;

                if (LEP.elementLinked)
                {
                    for (int j = 0; j < gameObjects.Count; ++j)
                    {
                        if (gameObjects[j] == LEP.elementLinked)
                        {
                            refPos = j;
                            break;
                        }
                    }
                }

                levelTS.levelElements.Add(new ElementData(gameObjects[i].transform.position, (int)LEP.PEType, refPos));
            }
            else
            {
                PuzzleElementPlaceHolder PEP = gameObjects[i].GetComponent<PuzzleElementPlaceHolder>();

                levelTS.levelElements.Add(new ElementData(gameObjects[i].transform.position, (int)PEP.PEType, -1));
            }
        }

        foreach (var pos in ground.cellBounds.allPositionsWithin)
        {
            Vector3Int iPos = new Vector3Int(pos.x, pos.y, pos.z);

            if (ground.HasTile(iPos))
            {
                levelTS.groundTiles.Add(new TileData(iPos, CheckTile(ground.GetTile(iPos))));
            }
        }

        foreach (var pos in collidable.cellBounds.allPositionsWithin)
        {
            Vector3Int iPos = new Vector3Int(pos.x, pos.y, pos.z);

            if (collidable.HasTile(iPos))
            {
                levelTS.collidableTiles.Add(new TileData(iPos, CheckTile(collidable.GetTile(iPos))));
            }
        }

        string levelJson = JsonUtility.ToJson(levelTS, true);

        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Data")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Data"));
        }

        File.WriteAllText(Path.Combine(Application.persistentDataPath, "Data", levelName + ".puzzle"), levelJson);
    }

    static public Level LoadLevel(string levelName)
    {
        string path = Path.Combine(Application.persistentDataPath, "Data", levelName + ".puzzle");

        if (File.Exists(path))
        {
            return JsonUtility.FromJson<Level>(File.ReadAllText(path));
        }

        return null;
    }

    public static int CheckTile(TileBase tile)
    {
        TileButton.Tiles i = TileButton.Tiles.None;
        AllTiles aT = PuzzleEditorController.instance.allTiles;

        if (tile == aT.groundTile)
        {
            i = TileButton.Tiles.Ground;
        }
        else if (tile == aT.darkGroundTile)
        {
            i = TileButton.Tiles.DarkGround;
        }
        else if (tile == aT.verticalWallTile)
        {
            i = TileButton.Tiles.VerticalWall;
        }
        else if (tile == aT.horizontalWallTile)
        {
            i = TileButton.Tiles.HorizontalWall;
        }
        else if (tile == aT.LUCornerTile)
        {
            i = TileButton.Tiles.LUCorner;
        }
        else if (tile == aT.LDCornerTile)
        {
            i = TileButton.Tiles.LDCorner;
        }
        else if (tile == aT.RUCornerTile)
        {
            i = TileButton.Tiles.RUCorner;
        }
        else if (tile == aT.RDCornerTile)
        {
            i = TileButton.Tiles.RDCorner;
        }
        else if (tile == aT.DeadEndUp)
        {
            i = TileButton.Tiles.DeadEndUp;
        }
        else if (tile == aT.DeadEndDown)
        {
            i = TileButton.Tiles.DeadEndDown;
        }
        else if (tile == aT.DeadEndLeft)
        {
            i = TileButton.Tiles.DeadEndLeft;
        }
        else if (tile == aT.DeadEndRight)
        {
            i = TileButton.Tiles.DeadEndRight;
        }
        else if (tile == aT.DeadContinuousLeft)
        {
            i = TileButton.Tiles.DeadContinuousLeft;
        }
        else if (tile == aT.DeadContinuousRight)
        {
            i = TileButton.Tiles.DeadContinuousRight;
        }
        else if (tile == aT.DeadContinuousUp)
        {
            i = TileButton.Tiles.DeadContinuousUp;
        }
        else if (tile == aT.DeadContinuousDown)
        {
            i = TileButton.Tiles.DeadContinuousDown;
        }
        else if (tile == aT.RotatingCylinder)
        {
            i = TileButton.Tiles.RotatingCylinder;
        }
        else if (tile == aT.LightsPanel)
        {
            i = TileButton.Tiles.lightsPanel;
        }

        return (int)i;
    }
}

[System.Serializable]
public class Level
{
    public int size;
    public List<ElementData> levelElements = new List<ElementData>();
    public List<TileData> groundTiles = new List<TileData>();
    public List<TileData> collidableTiles = new List<TileData>();
}

[System.Serializable]
public class TileData
{
    public Vector3Int position;
    public int id = -1;

    public TileData(Vector3Int pos, int id)
    {
        position = pos;
        this.id = id;
    }
}

[System.Serializable]
public class ElementData
{
    public Vector3 position;
    public int type;
    // Position on Serialized ElementData List where the linked gameobject is
    public int elementLinkedPos;

    public ElementData(Vector3 pos, int elementType, int elementLinkedPos)
    {
        position = pos;
        type = elementType;
        this.elementLinkedPos = elementLinkedPos;
    }
}
