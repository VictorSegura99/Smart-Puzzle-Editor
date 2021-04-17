using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapPathfinding : MonoBehaviour
{
    static public List<Vector3Int> FindPathInTilemap(Tilemap tilemap, Vector3Int startCoordinate, Vector3Int endCoordinate)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(startCoordinate);
        Dictionary<Vector3Int, Vector3Int> came_from = new Dictionary<Vector3Int, Vector3Int>();
        came_from.Add(startCoordinate, new Vector3Int(-999, -999, -999));

        while (queue.Count > 0)
        {
            Vector3Int t = queue.Dequeue();

            for (int i = 0; i < 4; ++i)
            {
                Vector3Int offset = new Vector3Int(-999, -999, -999);

                switch (i)
                {
                    case 0:
                        offset = new Vector3Int(0, 1, 0);
                        break;
                    case 1:
                        offset = new Vector3Int(1, 0, 0);
                        break;
                    case 2:
                        offset = new Vector3Int(0, -1, 0);
                        break;
                    case 3:
                        offset = new Vector3Int(-1, 0, 0);
                        break;
                }

                Vector3Int tile = t + offset;
                if (tilemap.GetTile(tile) && !came_from.ContainsKey(tile))
                {
                    queue.Enqueue(tile);
                    came_from.Add(tile, t);

                    if (tile == endCoordinate)
                    {
                        break;
                    }
                }
            }
        }

        Vector3Int current_tile = endCoordinate;
        List<Vector3Int> path = new List<Vector3Int>();

        while (current_tile != startCoordinate)
        {
            path.Add(current_tile);
            current_tile = came_from[current_tile];
        }

        path.Reverse();

        return path;
    }

    static public List<Vector3Int> FindPathCoordinates(Vector3Int startCoordinate, Vector3Int endCoordinate)
    {
        if (startCoordinate == endCoordinate)
        {
            return null;
        }

        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(startCoordinate);
        Dictionary<Vector3Int, Vector3Int> came_from = new Dictionary<Vector3Int, Vector3Int>();
        came_from.Add(startCoordinate, new Vector3Int(-999, -999, -999));

        while (queue.Count > 0)
        {
            Vector3Int t = queue.Dequeue();

            for (int i = 0; i < 4; ++i)
            {
                Vector3Int offset = new Vector3Int(-999, -999, -999);

                switch (i)
                {
                    case 0:
                        offset = new Vector3Int(0, 1, 0);
                        break;
                    case 1:
                        offset = new Vector3Int(1, 0, 0);
                        break;
                    case 2:
                        offset = new Vector3Int(0, -1, 0);
                        break;
                    case 3:
                        offset = new Vector3Int(-1, 0, 0);
                        break;
                }

                Vector3Int tile = t + offset;
                if (!came_from.ContainsKey(tile))
                {
                    queue.Enqueue(tile);
                    came_from.Add(tile, t);

                    if (tile == endCoordinate)
                    {
                        queue.Clear();
                        break;
                    }
                }
            }
        }

        Vector3Int current_tile = endCoordinate;
        List<Vector3Int> path = new List<Vector3Int>();

        while (current_tile != startCoordinate)
        {
            path.Add(current_tile);
            current_tile = came_from[current_tile];
        }

        path.Add(startCoordinate);

        path.Reverse();

        return path;
    }
}
