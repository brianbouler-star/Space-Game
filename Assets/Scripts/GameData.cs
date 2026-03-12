using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradeDefinition
{
    public string Id;
    public string DisplayName;
    public double BaseCost;
    public double Value;
    public bool IsOneTime;
}

[Serializable]
public class SaveData
{
    public double Ore;
    public double DarkMatter;
    public int Gems;
    public double TotalOreEarned;
    public int PrestigeCount;
    public bool RemoveAdsPurchased;
    public bool VipActive;
    public long LastLoginUnix;
    public long BoostEndUnix;
    public Dictionary<string, int> OwnedUpgrades = new Dictionary<string, int>();
    public Dictionary<string, int> FleetCounts = new Dictionary<string, int>();
    public Dictionary<string, int> PlanetUnlocks = new Dictionary<string, int>();
}

public static class ProductIds
{
    public const string RemoveAds = "remove_ads";
    public const string VipSubscription = "vip_pass_monthly";
    public const string Gems100 = "gems_100";
    public const string Gems550 = "gems_550";
    public const string Gems1200 = "gems_1200";
    public const string Gems2500 = "gems_2500";
    public const string Gems6500 = "gems_6500";
}

public static class AdsPlacementIds
{
    public const string RewardedBoost = "Rewarded_Boost";
    public const string RewardedGems = "Rewarded_Gems";
    public const string BannerBottom = "Banner_Bottom";
    public const string InterstitialPrestige = "Interstitial_Prestige";
}
