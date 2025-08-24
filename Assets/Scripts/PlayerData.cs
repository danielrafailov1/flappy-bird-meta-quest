using System;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int totalScore;
    public int totalCoins;
    public int totalCollisions;
    public float bestTime;
    public DateTime lastPlayed;

    // Current session data
    public int sessionScore;
    public int sessionCoins;
    public int sessionCollisions;
    public float sessionTime;

    public PlayerData()
    {
        totalScore = 0;
        totalCoins = 0;
        totalCollisions = 0;
        bestTime = 0f;
        lastPlayed = DateTime.Now;

        sessionScore = 0;
        sessionCoins = 0;
        sessionCollisions = 0;
        sessionTime = 0f;
    }
}

public static class DataManager
{
    private static string savePath => Application.persistentDataPath + "/playerdata.json";

    public static PlayerData LoadData()
    {
        try
        {
            if (System.IO.File.Exists(savePath))
            {
                string json = System.IO.File.ReadAllText(savePath);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log("Data loaded successfully from: " + savePath);
                return data;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load data: " + e.Message);
        }

        Debug.Log("Creating new player data");
        return new PlayerData();
    }

    public static void SaveData(PlayerData data)
    {
        try
        {
            data.lastPlayed = DateTime.Now;
            string json = JsonUtility.ToJson(data, true);
            System.IO.File.WriteAllText(savePath, json);
            Debug.Log("Data saved successfully to: " + savePath);
            Debug.Log("Saved data: Score=" + data.totalScore + ", Coins=" + data.totalCoins + ", Collisions=" + data.totalCollisions);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save data: " + e.Message);
        }
    }
}