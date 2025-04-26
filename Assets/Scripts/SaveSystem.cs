using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : Singleton<SaveSystem>
{
    private string saveFilePath;
    private const string CURRENT_VERSION = "1.0.0";

    private void Start()
    {
        string folder = Path.Combine(Application.persistentDataPath, "Saves");
        if (!Directory.Exists(folder))
        Directory.CreateDirectory(folder);

        saveFilePath = Path.Combine(folder, "save.txt");

        Debug.Log($"Save File is stored at {saveFilePath}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame(MapGenerator.Instance.GetSeed(), PlayerManager.Instance.nations);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            int loadedSeed;
            List<PlayerData> loadedPlayers;

            LoadGame(out loadedSeed, out loadedPlayers);

            if (loadedSeed != 0)
            {
                PlayerManager.Instance.LoadPlayers(loadedPlayers);
                MapGenerator.Instance.LoadMapFromSeed(loadedSeed, loadedPlayers);
            }
        }
    }

    public void SaveGame(int seed, List<PlayerData> players)
    {
        List<string> lines = new List<string>
        {
            $"Version:{CURRENT_VERSION}",
            $"Seed:{seed}"
        };

        foreach (var player in players)
        {
            lines.Add($"Player:{player.nationName},{ColorUtility.ToHtmlStringRGBA(player.nationColor)},{player.isUnlocked}");
            foreach (var tile in player.tiles)
            {
                lines.Add($"Tile:{player.nationName},{tile.coordinate.x},{tile.coordinate.y}");
            }
        }

        File.WriteAllLines(saveFilePath, lines);
        Debug.Log("Game saved");
    }

    public void LoadGame(out int seed, out List<PlayerData> loadedPlayers)
    {
        seed = 0;
        loadedPlayers = new List<PlayerData>();

        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("No save file found");
            return;
        }

        Dictionary<string, PlayerData> playerLookup = new();
        string loadedVersion = "0.0.0"; // in case no save file is found

        foreach (var line in File.ReadAllLines(saveFilePath))
        {
            if (line.StartsWith("Version:"))
            {
                loadedVersion = line.Split(':')[1];
                if (loadedVersion != CURRENT_VERSION)
                {
                    Debug.LogWarning($"File: {loadedVersion}, Expected: {CURRENT_VERSION}");
                }
            }
            else if (line.StartsWith("Seed:"))
            {
                seed = int.Parse(line.Split(':')[1]);
            }
            else if (line.StartsWith("Player:"))
            {
                string[] parts = line.Split(':')[1].Split(',');
                string name = parts[0];
                Color color;
                ColorUtility.TryParseHtmlString("#" + parts[1], out color);
                bool unlocked = bool.Parse(parts[2]);

                var player = new PlayerData { nationName = name, nationColor = color, isUnlocked = unlocked };
                loadedPlayers.Add(player);
                playerLookup[name] = player;
            }
            else if (line.StartsWith("Tile:"))
            {
                string[] parts = line.Split(':')[1].Split(',');
                string playerName = parts[0];
                int x = int.Parse(parts[1]);
                int y = int.Parse(parts[2]);
                if (playerLookup.TryGetValue(playerName, out PlayerData owner))
                {
                    owner.tilesData.Add(new CustomTileData(new Vector2Int(x, y)));
                }
            }
        }

        Debug.Log("Game loaded.");
    }

}