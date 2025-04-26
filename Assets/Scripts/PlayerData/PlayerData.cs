using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string nationName;
    public Color nationColor;
    public List<CustomTile> tiles = new List<CustomTile>(60);// for saving
    public List<CustomTileData> tilesData = new();// for loading
    public NationButton nationButton;

    public bool isUnlocked;
}