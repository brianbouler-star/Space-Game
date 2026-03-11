using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public readonly List<UpgradeDefinition> MiningUpgrades = new List<UpgradeDefinition>
    {
        new UpgradeDefinition { Id = "laser_mk1", DisplayName = "Mining Laser Mk1", BaseCost = 50, Value = 2, IsOneTime = true },
        new UpgradeDefinition { Id = "laser_mk2", DisplayName = "Mining Laser Mk2", BaseCost = 250, Value = 2, IsOneTime = true },
        new UpgradeDefinition { Id = "laser_mk3", DisplayName = "Mining Laser Mk3", BaseCost = 1000, Value = 2, IsOneTime = true },
        new UpgradeDefinition { Id = "laser_mk4", DisplayName = "Mining Laser Mk4", BaseCost = 5000, Value = 2, IsOneTime = true },
        new UpgradeDefinition { Id = "laser_mk5", DisplayName = "Mining Laser Mk5", BaseCost = 25000, Value = 2, IsOneTime = true },
    };

    public readonly List<UpgradeDefinition> FleetUpgrades = new List<UpgradeDefinition>
    {
        new UpgradeDefinition { Id = "fleet_drone", DisplayName = "Mining Drone", BaseCost = 100, Value = 1, IsOneTime = false },
        new UpgradeDefinition { Id = "fleet_scout", DisplayName = "Scout Ship", BaseCost = 500, Value = 5, IsOneTime = false },
        new UpgradeDefinition { Id = "fleet_hauler", DisplayName = "Hauler Freighter", BaseCost = 2500, Value = 25, IsOneTime = false },
        new UpgradeDefinition { Id = "fleet_cruiser", DisplayName = "Battle Cruiser", BaseCost = 10000, Value = 100, IsOneTime = false },
        new UpgradeDefinition { Id = "fleet_mothership", DisplayName = "Mothership", BaseCost = 75000, Value = 500, IsOneTime = false },
    };

    public readonly List<UpgradeDefinition> PlanetUpgrades = new List<UpgradeDefinition>
    {
        new UpgradeDefinition { Id = "planet_moon", DisplayName = "Colonize Moon", BaseCost = 10000, Value = 2, IsOneTime = true },
        new UpgradeDefinition { Id = "planet_mars", DisplayName = "Colonize Mars", BaseCost = 100000, Value = 5, IsOneTime = true },
        new UpgradeDefinition { Id = "planet_jupiter", DisplayName = "Colonize Jupiter", BaseCost = 1000000, Value = 10, IsOneTime = true },
        new UpgradeDefinition { Id = "planet_saturn", DisplayName = "Colonize Saturn", BaseCost = 10000000, Value = 25, IsOneTime = true },
        new UpgradeDefinition { Id = "planet_neptune", DisplayName = "Colonize Neptune", BaseCost = 1000000000, Value = 100, IsOneTime = true },
    };

    public Dictionary<string, int> OwnedMining = new Dictionary<string, int>();
    public Dictionary<string, int> OwnedFleet = new Dictionary<string, int>();
    public Dictionary<string, int> OwnedPlanets = new Dictionary<string, int>();

    public void Initialize(SaveData saveData)
    {
        OwnedMining = saveData.OwnedUpgrades ?? new Dictionary<string, int>();
        OwnedFleet = saveData.FleetCounts ?? new Dictionary<string, int>();
        OwnedPlanets = saveData.PlanetUnlocks ?? new Dictionary<string, int>();
    }

    public bool PurchaseUpgrade(UpgradeDefinition def, CurrencyManager currency)
    {
        int owned = GetOwnedCount(def.Id);
        if (def.IsOneTime && owned > 0) return false;
        double cost = GetCurrentCost(def, owned);
        if (!currency.SpendOre(cost)) return false;

        SetOwnedCount(def.Id, owned + 1);
        return true;
    }

    public double GetCurrentCost(UpgradeDefinition def, int owned)
    {
        return def.IsOneTime ? def.BaseCost : def.BaseCost * Mathf.Pow(1.15f, owned);
    }

    public int GetOwnedCount(string id)
    {
        if (OwnedMining.TryGetValue(id, out int mining)) return mining;
        if (OwnedFleet.TryGetValue(id, out int fleet)) return fleet;
        if (OwnedPlanets.TryGetValue(id, out int planet)) return planet;
        return 0;
    }

    private void SetOwnedCount(string id, int count)
    {
        if (id.StartsWith("laser")) OwnedMining[id] = count;
        else if (id.StartsWith("fleet")) OwnedFleet[id] = count;
        else if (id.StartsWith("planet")) OwnedPlanets[id] = count;
    }

    public double GetTapMultiplier()
    {
        double tapMultiplier = 1d;
        foreach (UpgradeDefinition def in MiningUpgrades)
        {
            if (GetOwnedCount(def.Id) > 0)
                tapMultiplier *= def.Value;
        }
        return tapMultiplier;
    }

    public double GetPlanetMultiplier()
    {
        double multiplier = 1d;
        foreach (UpgradeDefinition def in PlanetUpgrades)
        {
            if (GetOwnedCount(def.Id) > 0)
                multiplier *= def.Value;
        }
        return multiplier;
    }

    public double GetFleetIncomePerSecond()
    {
        double income = 0d;
        foreach (UpgradeDefinition def in FleetUpgrades)
        {
            income += GetOwnedCount(def.Id) * def.Value;
        }
        return income;
    }
}
