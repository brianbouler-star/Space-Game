using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Managers")]
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private FleetManager fleetManager;
    [SerializeField] private PrestigeManager prestigeManager;
    [SerializeField] private OfflineManager offlineManager;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private BoostManager boostManager;
    [SerializeField] private UIManager uiManager;

    [Header("Monetization State")]
    [SerializeField] private bool removeAdsPurchased;
    [SerializeField] private bool vipActive;

    public bool RemoveAdsPurchased => removeAdsPurchased;
    public bool VipActive => vipActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SaveData data = saveManager.Load();
        removeAdsPurchased = data.RemoveAdsPurchased;
        vipActive = data.VipActive;

        currencyManager.Initialize(data.Ore, data.DarkMatter, data.Gems, data.TotalOreEarned);
        boostManager.Initialize(data.BoostEndUnix);
        upgradeManager.Initialize(data);
        prestigeManager.Initialize(currencyManager, data.PrestigeCount);
        fleetManager.Initialize(upgradeManager, currencyManager, boostManager, prestigeManager);

        double offlineIncome = offlineManager.CalculateOfflineEarnings(data.LastLoginUnix, fleetManager.GetIncomePerSecond());
        if (offlineIncome > 0)
        {
            currencyManager.AddOre(offlineIncome);
            uiManager.ShowOfflineEarnings(offlineIncome);
        }

        uiManager.Initialize(this, currencyManager, upgradeManager, fleetManager, prestigeManager, boostManager);
    }

    public SaveData BuildSaveData()
    {
        return new SaveData
        {
            Ore = currencyManager.Ore,
            DarkMatter = currencyManager.DarkMatter,
            Gems = currencyManager.Gems,
            TotalOreEarned = currencyManager.TotalOreEarned,
            PrestigeCount = prestigeManager.PrestigeCount,
            RemoveAdsPurchased = removeAdsPurchased,
            VipActive = vipActive,
            LastLoginUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            BoostEndUnix = boostManager.BoostEndUnix,
            OwnedUpgrades = upgradeManager.OwnedMining,
            FleetCounts = upgradeManager.OwnedFleet,
            PlanetUnlocks = upgradeManager.OwnedPlanets
        };
    }

    public void ApplyRemoveAdsPurchase()
    {
        removeAdsPurchased = true;
        saveManager.Save(BuildSaveData());
    }

    public void SetVipActive(bool active)
    {
        vipActive = active;
        saveManager.Save(BuildSaveData());
    }

    public void PerformPrestigeReset()
    {
        int gained = prestigeManager.PerformPrestige();
        if (gained <= 0) return;

        SaveData reset = new SaveData
        {
            Ore = 0,
            DarkMatter = currencyManager.DarkMatter,
            Gems = currencyManager.Gems,
            TotalOreEarned = 0,
            PrestigeCount = prestigeManager.PrestigeCount,
            RemoveAdsPurchased = removeAdsPurchased,
            VipActive = vipActive,
            LastLoginUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            BoostEndUnix = 0
        };

        saveManager.Save(reset);
        upgradeManager.Initialize(reset);
        currencyManager.Initialize(reset.Ore, reset.DarkMatter, reset.Gems, reset.TotalOreEarned);
        boostManager.Initialize(reset.BoostEndUnix);
        uiManager.ShowPrestigeResult(gained);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) saveManager.Save(BuildSaveData());
    }

    private void OnApplicationQuit()
    {
        saveManager.Save(BuildSaveData());
    }
}
