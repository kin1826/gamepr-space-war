using System;
using System.IO;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int level          = 1;
    public int currentXP      = 0;
    public int xpToNextLevel  = 100;
    public int gold            = 0;
    public int diamond         = 0;
}

public static class SaveManager
{
    private static readonly string _path = Path.Combine(Application.persistentDataPath, "playerdata.json");

    public static void Save(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(_path, json);
    }

    public static PlayerData Load()
    {
        if (!File.Exists(_path))
            return new PlayerData();

        string json = File.ReadAllText(_path);
        return JsonUtility.FromJson<PlayerData>(json);
    }

    public static void Delete()
    {
        if (File.Exists(_path))
            File.Delete(_path);
    }
}
