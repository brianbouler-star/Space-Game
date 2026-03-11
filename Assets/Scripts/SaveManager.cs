using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string KeyOre = "save_ore";
    private const string KeyDarkMatter = "save_dark_matter";
    private const string KeyGems = "save_gems";
    private const string KeyTotalOre = "save_total_ore";
    private const string KeyPrestigeCount = "save_prestige_count";
    private const string KeyRemoveAds = "save_remove_ads";
    private const string KeyVipActive = "save_vip_active";
    private const string KeyLastLogin = "save_last_login";
    private const string KeyBoostEnd = "save_boost_end";
    private const string KeyOwnedMining = "save_owned_mining";
    private const string KeyOwnedFleet = "save_owned_fleet";
    private const string KeyOwnedPlanets = "save_owned_planets";

    [SerializeField] private float autoSaveInterval = 30f;
    private float _timer;

    [Serializable]
    private class Pair
    {
        public string Key;
        public int Value;
    }

    [Serializable]
    private class PairCollection
    {
        public List<Pair> Items = new List<Pair>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= autoSaveInterval)
        {
            _timer = 0f;
            if (GameManager.Instance != null) Save(GameManager.Instance.BuildSaveData());
        }
    }

    public void Save(SaveData data)
    {
        PlayerPrefs.SetString(KeyOre, data.Ore.ToString("R"));
        PlayerPrefs.SetString(KeyDarkMatter, data.DarkMatter.ToString("R"));
        PlayerPrefs.SetInt(KeyGems, data.Gems);
        PlayerPrefs.SetString(KeyTotalOre, data.TotalOreEarned.ToString("R"));
        PlayerPrefs.SetInt(KeyPrestigeCount, data.PrestigeCount);
        PlayerPrefs.SetInt(KeyRemoveAds, data.RemoveAdsPurchased ? 1 : 0);
        PlayerPrefs.SetInt(KeyVipActive, data.VipActive ? 1 : 0);
        PlayerPrefs.SetString(KeyLastLogin, data.LastLoginUnix.ToString());
        PlayerPrefs.SetString(KeyBoostEnd, data.BoostEndUnix.ToString());
        PlayerPrefs.SetString(KeyOwnedMining, EncodePairs(data.OwnedUpgrades));
        PlayerPrefs.SetString(KeyOwnedFleet, EncodePairs(data.FleetCounts));
        PlayerPrefs.SetString(KeyOwnedPlanets, EncodePairs(data.PlanetUnlocks));
        PlayerPrefs.Save();
    }

    public SaveData Load()
    {
        SaveData data = new SaveData
        {
            Ore = ParseDouble(PlayerPrefs.GetString(KeyOre, "0")),
            DarkMatter = ParseDouble(PlayerPrefs.GetString(KeyDarkMatter, "0")),
            Gems = PlayerPrefs.GetInt(KeyGems, 0),
            TotalOreEarned = ParseDouble(PlayerPrefs.GetString(KeyTotalOre, "0")),
            PrestigeCount = PlayerPrefs.GetInt(KeyPrestigeCount, 0),
            RemoveAdsPurchased = PlayerPrefs.GetInt(KeyRemoveAds, 0) == 1,
            VipActive = PlayerPrefs.GetInt(KeyVipActive, 0) == 1,
            LastLoginUnix = ParseLong(PlayerPrefs.GetString(KeyLastLogin, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())),
            BoostEndUnix = ParseLong(PlayerPrefs.GetString(KeyBoostEnd, "0")),
            OwnedUpgrades = DecodePairs(PlayerPrefs.GetString(KeyOwnedMining, string.Empty)),
            FleetCounts = DecodePairs(PlayerPrefs.GetString(KeyOwnedFleet, string.Empty)),
            PlanetUnlocks = DecodePairs(PlayerPrefs.GetString(KeyOwnedPlanets, string.Empty))
        };
        return data;
    }

    private static string EncodePairs(Dictionary<string, int> source)
    {
        PairCollection collection = new PairCollection();
        foreach (var entry in source)
        {
            collection.Items.Add(new Pair { Key = entry.Key, Value = entry.Value });
        }
        return JsonUtility.ToJson(collection);
    }

    private static Dictionary<string, int> DecodePairs(string json)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        if (string.IsNullOrEmpty(json)) return result;
        PairCollection collection = JsonUtility.FromJson<PairCollection>(json);
        if (collection == null || collection.Items == null) return result;
        foreach (Pair pair in collection.Items)
        {
            result[pair.Key] = pair.Value;
        }
        return result;
    }

    private static double ParseDouble(string value)
    {
        return double.TryParse(value, out double parsed) ? parsed : 0d;
    }

    private static long ParseLong(string value)
    {
        return long.TryParse(value, out long parsed) ? parsed : 0L;
    }
}
