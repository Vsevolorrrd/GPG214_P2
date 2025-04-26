using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] RectTransform nationsButtonBox;
    [SerializeField] GameObject nationButton;
    public List<PlayerData> nations = new List<PlayerData>(5); // List of all players
    private PlayerData currentPlayer;

    void Start()
    {
        if (nations.Count == 0)
        InitializePlayers();

        currentPlayer = nations[0];
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryChangePlayer(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryChangePlayer(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TryChangePlayer(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TryChangePlayer(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) TryChangePlayer(4);
    }
    public void TryChangePlayer(int index)
    {
        if (index < nations.Count && nations[index].isUnlocked)
        {
            ChangePlayer(index);
        }
        else
        {
            Debug.Log($"Player {index + 1} is locked!");
        }
    }

    public PlayerData GetCurrentPlayer()
    {
        return currentPlayer;
    }

    void InitializePlayers()
    {
        nations.Add(new PlayerData { nationName = "Player 1", nationColor = Color.red, isUnlocked = false });
        nations.Add(new PlayerData { nationName = "Player 2", nationColor = Color.blue, isUnlocked = false });

        nations.Add(new PlayerData { nationName = "Player 3", nationColor = Color.green, isUnlocked = false });
        nations.Add(new PlayerData { nationName = "Player 4", nationColor = Color.yellow, isUnlocked = false });
        nations.Add(new PlayerData { nationName = "Player 5", nationColor = Color.white, isUnlocked = false });

        for (int i = 0; i < nations.Count; i++) // creating buttons for nations
        {
            var _nationButton = Instantiate(nationButton, nationsButtonBox);
            NationButton buttonScript = _nationButton.GetComponent<NationButton>();
            _nationButton.GetComponent<Button>().onClick.AddListener(() => buttonScript.SetNation());

            buttonScript.SetUpButton(nations[i].nationColor, i);
            nations[i].nationButton = buttonScript;
        }

        UnlockPlayer(nations[0]);
        UnlockPlayer(nations[1]);
    }
    public void ChangePlayer(int player)
    {
        currentPlayer = nations[player];
    }
    public void AddTileToPlayerList(CustomTile tile)
    {
        currentPlayer.tiles.Add(tile);
        AnalyticsManager.Instance.AnalyticsTilesClaimed();
    }
    public void LoadPlayers(List<PlayerData> loaded)
    {
        nations = loaded;
        currentPlayer = nations[0];
    }
    public void UnlockPlayer(PlayerData player)
    {
        if (!player.isUnlocked)
        {
            player.isUnlocked = true;
            player.nationButton.UnlockNation(true);
            Debug.Log($"{player.nationName} is unlocked!");
        }
    }
    public void LockPlayer(PlayerData player)
    {
        if (player.isUnlocked)
        {
            player.isUnlocked = false;
            player.nationButton.UnlockNation(false);
        }
    }
}