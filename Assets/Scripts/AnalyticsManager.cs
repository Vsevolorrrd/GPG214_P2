using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR // To save the analytics in editor as well
using UnityEditor;
#endif

public class AnalyticsManager : Singleton<AnalyticsManager>
{
    private string analyticsFilePath;

    [SerializeField] TextMeshProUGUI timePlayedText;
    [SerializeField] TextMeshProUGUI MapsGeneratedText;
    [SerializeField] TextMeshProUGUI TilesClaimedText;

    private int mapsGenerated = 0;
    private int tilesClaimed = 0;
    private float totalPlaytime = 0f;

    private void Start()
    {
        string folder = Path.Combine(Application.persistentDataPath, "Saves");
        if (!Directory.Exists(folder))
        Directory.CreateDirectory(folder);

        analyticsFilePath = Path.Combine(folder, "analytics.txt");
        LoadAnalytics();
    }
    private void Update()
    {
        UpdateAnalytics();
    }

    private void SaveAnalytics()
    {
        List<string> lines = new() {
            $"MapsGenerated={mapsGenerated}",
            $"TilesClaimed={tilesClaimed}",
            $"TotalPlaytime={totalPlaytime}"};

        File.WriteAllLines(analyticsFilePath, lines);
    }
    private void LoadAnalytics()
    {
        if (!File.Exists(analyticsFilePath)) return;

        foreach (string line in File.ReadAllLines(analyticsFilePath))
        {
            string[] parts = line.Split('=');
            if (parts.Length != 2) continue;

            switch (parts[0])
            {
                case "MapsGenerated":
                    mapsGenerated = int.Parse(parts[1]);
                    break;
                case "TilesClaimed":
                    tilesClaimed = int.Parse(parts[1]);
                    break;
                case "TotalPlaytime":
                    totalPlaytime = float.Parse(parts[1]);
                    break;
            }
        }
    }
    public void ResetAnalytics()
    {
        mapsGenerated = 0;
        tilesClaimed = 0;
        totalPlaytime = 0f;

        SaveAnalytics();
        UnlockablesManager.Instance.LockPlayers();
    }
    private void OnApplicationQuit()
    {
        SaveAnalytics();
    }

    public void AnalyticsMapGeneration() { mapsGenerated++; }
    public void AnalyticsTilesClaimed() { tilesClaimed++; }
    private void UpdateAnalytics()
    {
        totalPlaytime += Time.deltaTime;
        timePlayedText.text = $"Time Played\n{Mathf.FloorToInt(totalPlaytime)}s";
        MapsGeneratedText.text = $"Maps Generated\n{mapsGenerated}";
        TilesClaimedText.text = $"Tiles Claimed\n{tilesClaimed}";

        UnlockablesManager.Instance.CheckUnlockConditions(totalPlaytime, mapsGenerated, tilesClaimed);
    }

    #region Editor

    #if UNITY_EDITOR
    // to Save analytics when exiting play mode in editor

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }
    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }
    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            SaveAnalytics();
            Debug.Log("Saved analytics when exiting play mode");
        }
    }

    #endif

    #endregion
}