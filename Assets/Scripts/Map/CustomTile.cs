using System.Collections.Generic;
using UnityEngine;

public class CustomTile : MonoBehaviour
{
    [SerializeField] GameObject borderOverlay;
    [SerializeField] GameObject tileOverlay;
    [SerializeField] PlayerData Owner;  // Reference to the owner
    [HideInInspector] public Vector2Int coordinate;
    [HideInInspector] public Vector3Int cubeCoordinate;

    private void Start()
    {
        tileOverlay.SetActive(false);
    }

    // Method to update the tile's owner
    public void SetOwner(PlayerData newOwner)
    {
        Owner = newOwner;
        borderOverlay.SetActive(true);
        borderOverlay.GetComponent<SpriteRenderer>().color = Owner.nationColor; // Change the color to represent the owner
    }
    private void OnMouseEnter()
    {
        tileOverlay.SetActive(true);
    }
    private void OnMouseExit()
    {
        tileOverlay.SetActive(false);
    }
    private void OnMouseDown()
    {
        SetOwner(PlayerManager.Instance.GetCurrentPlayer());
        PlayerManager.Instance.AddTileToPlayerList(this);
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