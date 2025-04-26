using System.Collections.Generic;
using System.Text;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

public class CloudSystem : MonoBehaviour
{
    private const string CLOUD_SAVE_KEY = "save_file";
    private const string CURRENT_VERSION = "1.0.0";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SaveToCloud(MapGenerator.Instance.GetSeed(), PlayerManager.Instance.nations);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            LoadFromCloud();
        }
    }
    private async void Awake()
    {
        try
        {
            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                await UnityServices.InitializeAsync();
                Debug.Log("Unity Services Initialized.");
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Signed in anonymously to Unity Services.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Unity Services initialization/sign-in failed: {ex.Message}");
        }
    }

    public async void SaveToCloud(int seed, List<PlayerData> players)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"Version:{CURRENT_VERSION}");
        builder.AppendLine($"Seed:{seed}");

        foreach (var player in players)
        {
            builder.AppendLine($"Player:{player.nationName},{ColorUtility.ToHtmlStringRGBA(player.nationColor)},{player.isUnlocked}");
            foreach (var tile in player.tiles)
            {
                builder.AppendLine($"Tile:{player.nationName},{tile.coordinate.x},{tile.coordinate.y}");
            }
        }

        var saveData = new Dictionary<string, object>
        {
            { CLOUD_SAVE_KEY, builder.ToString() }
        };

        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(saveData);
            Debug.Log("Game saved to cloud.");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Failed to save to cloud: {ex.Message}");
        }
    }

    public async void LoadFromCloud()
    {
        try
        {
            var savedData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { CLOUD_SAVE_KEY });

            if (savedData.TryGetValue(CLOUD_SAVE_KEY, out var data))
            {
                string fileContent = data.Value.GetAsString();

                // Parse the loaded cloud data using the LoadGame logic from SaveSystem
                int loadedSeed;
                List<PlayerData> loadedPlayers;
                ParseSaveData(fileContent, out loadedSeed, out loadedPlayers);

                if (loadedSeed != 0)
                {
                    PlayerManager.Instance.LoadPlayers(loadedPlayers);
                    MapGenerator.Instance.LoadMapFromSeed(loadedSeed, loadedPlayers);
                    Debug.Log("Game loaded from cloud.");
                }
            }
            else
            {
                Debug.LogWarning("No cloud save found.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Failed to load from cloud: {ex.Message}");
        }
    }

    private void ParseSaveData(string fileContent, out int seed, out List<PlayerData> loadedPlayers)
    {
        seed = 0;
        loadedPlayers = new List<PlayerData>();
        Dictionary<string, PlayerData> playerLookup = new();
        string loadedVersion = "0.0.0";

        var lines = fileContent.Split('\n');

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (line.StartsWith("Version:"))
            {
                loadedVersion = line.Split(':')[1].Trim(); // Trim because of how windows handles line endings
                if (loadedVersion != CURRENT_VERSION)      // It took so much time to fix this issue and find out
                Debug.LogWarning($"File: {loadedVersion}, Expected: {CURRENT_VERSION}"); // what was causing it ):
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
                owner.tilesData.Add(new CustomTileData(new Vector2Int(x, y)));
            }
        }
    }
}