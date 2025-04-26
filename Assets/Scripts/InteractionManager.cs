using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionManager : Singleton<InteractionManager>
{
    [SerializeField] List<CustomTile> allTiles = new List<CustomTile>();
    //  to store tiles using their coordinates
    [SerializeField] Dictionary<Vector3Int, CustomTile> tiles = new Dictionary<Vector3Int, CustomTile>();

    private CustomTile currentHoverTile = null;

    private void Update()
    {
        HandleTileInteraction();
    }
    private void HandleTileInteraction()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; // Prevent clicking through UI

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider)
        {
            CustomTile hitTile = hit.collider.GetComponent<CustomTile>();

            // hover
            if (hitTile != currentHoverTile)
            {
                if (currentHoverTile != null)
                currentHoverTile.ShowOverlay(false);

                currentHoverTile = hitTile;

                if (currentHoverTile != null)
                currentHoverTile.ShowOverlay(true);
            }
        }
        // click
        if (Input.GetMouseButton(0) && currentHoverTile != null)
        {
            if (currentHoverTile.SetOwner(PlayerManager.Instance.GetCurrentPlayer()))
            PlayerManager.Instance.AddTileToPlayerList(currentHoverTile);
        }
    }
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