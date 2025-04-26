using UnityEngine;

public class CustomTile : MonoBehaviour
{
    [SerializeField] GameObject borderOverlay;
    [SerializeField] GameObject tileOverlay;
    [SerializeField] PlayerData owner;  // Reference to the owner
    [HideInInspector] public Vector2Int coordinate;
    [HideInInspector] public Vector3Int cubeCoordinate;

    private void Start()
    {
        tileOverlay.SetActive(false);
    }

    // Method to update the tile's owner
    public bool SetOwner(PlayerData newOwner)
    {
        if (newOwner == owner)
        return false; // Already owned by this player

        bool isFirstTimeClaimed = owner == null; // To awoid adding the same tile if it was claimed by diffrent player

        owner = newOwner;
        borderOverlay.SetActive(true);
        borderOverlay.GetComponent<SpriteRenderer>().color = owner.nationColor;

        return isFirstTimeClaimed;
    }
    public void ShowOverlay(bool show)
    {
        tileOverlay.SetActive(show);
    }
}

[System.Serializable]
public class CustomTileData
{
    public Vector2Int coordinate;
    public CustomTileData(Vector2Int coord)
    {
        coordinate = coord;
    }
}