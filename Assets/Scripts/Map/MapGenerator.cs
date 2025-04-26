using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : Singleton<MapGenerator>
{
    [SerializeField] GameObject grid;

    [Header("Map Settings")]
    [SerializeField] int mapWidth = 30;
    [SerializeField] int mapHeight = 20;
    [SerializeField] float tileSize = 1f;
    [SerializeField] float scale = 8f;
    [SerializeField] int seed;

    [Header("Tiles")]
    [SerializeField] GameObject mountainTilePrefab;
    [SerializeField] GameObject forestTilePrefab;
    [SerializeField] GameObject waterTilePrefab;
    [SerializeField] GameObject plainTilePrefab;

    [Header("Thresholds")]
    [SerializeField] float waterThreshold = 0.4f;
    [SerializeField] float forestThreshold = 0.7f;
    [SerializeField] float mountainThreshold = 0.85f;

    public int GetSeed() { return seed; }

    void Start()
    {
        seed = Random.Range(0, 100000);
        GenerateMap();
    }

    public void RegenerateMap()
    {
        InteractionManager.Instance.ClearAllTiles();
        seed = Random.Range(0, 100000);
        GenerateMap();
    }

    void GenerateMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector2 tilePosition = HexPos(x, y);
                GameObject tilePrefab = SelectTilePrefab(x, y);
                CreateTile(tilePrefab, tilePosition, new Vector2Int(x, y));
            }
        }
        // After map generation, initializes the TileSelector
        InteractionManager.Instance.Initialize();
        AnalyticsManager.Instance.AnalyticsMapGeneration();
    }

    private Vector2 HexPos(int x, int y) // https://youtu.be/ArarXhubJ1Y?feature=shared
    {
        float xPos = x * tileSize * Mathf.Cos(Mathf.Deg2Rad * 30);
        float yPos = y * tileSize + ((x % 2 == 1) ? tileSize * 0.5f : 0);

        return new Vector2(xPos, yPos);
    }

    GameObject SelectTilePrefab(int x, int y)
    {
        float noiseValue = Mathf.PerlinNoise((x + seed) / scale, (y + seed) / scale);

        if (noiseValue > mountainThreshold)
            return mountainTilePrefab; // Mountain prefab
        else if (noiseValue > forestThreshold)
            return forestTilePrefab; // Forest prefab
        else if (noiseValue < waterThreshold)
            return waterTilePrefab; // Water prefab
        else
            return plainTilePrefab; // Default plain tile prefab
    }
    private void CreateTile(GameObject tilePrefab, Vector3 position, Vector2Int offset)
    {
        GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity, grid.transform);
        CustomTile customTile = tileObject.GetComponent<CustomTile>();
        customTile.coordinate = offset;

        int q = offset.x;
        int r = offset.y - (offset.x - (offset.x % 2)) / 2;
        int s = -q - r;

        customTile.cubeCoordinate = new Vector3Int(q, r, s);
        InteractionManager.Instance.AddTile(customTile);
    }
    public void LoadMapFromSeed(int loadedSeed, List<PlayerData> loadedPlayers)
    {
        seed = loadedSeed;

        // Clear old map
        InteractionManager.Instance.ClearAllTiles();

        // Generate new map
        GenerateMap();

        foreach (PlayerData player in loadedPlayers)
        {
            List<CustomTile> fixedTiles = new List<CustomTile>();

            foreach (CustomTileData savedTile in player.tilesData)
            {
                CustomTile realTile = InteractionManager.Instance.GetTileAt(savedTile.coordinate);
                if (realTile != null)
                {
                    realTile.SetOwner(player);
                    fixedTiles.Add(realTile);
                }
            }

            player.tiles = fixedTiles;
        }
    }
}