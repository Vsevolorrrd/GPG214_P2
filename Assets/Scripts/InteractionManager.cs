using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : Singleton<InteractionManager>
{
    [SerializeField] List<CustomTile> allTiles = new List<CustomTile>();
    // Dictionary to store tiles using their cube coordinates
    [SerializeField] Dictionary<Vector3Int, CustomTile> tiles = new Dictionary<Vector3Int, CustomTile>();

    public void AddTile(CustomTile customTile)
    {
        allTiles.Add(customTile);
    }
    public void ClearAllTiles()
    {
        for (int i = allTiles.Count - 1; i >= 0; i--)
        {
            Destroy(allTiles[i].gameObject);
        }
        allTiles.Clear();
        tiles.Clear();
    }
    public CustomTile GetTileAt(Vector2Int coord)
    {
        foreach (var tile in allTiles)
        {
            if (tile.coordinate == coord)
                return tile;
        }
        return null;
    }
    public void Initialize()
    {
        // Register all the tiles
        foreach (CustomTile tile in allTiles)
        {
            tiles.Add(tile.cubeCoordinate, tile);
        }
    }
    public CustomTile GetRandomTile()
    {
        // Select a random tile
        int R = Random.Range(0, allTiles.Count);
        return allTiles[R];
    }
}