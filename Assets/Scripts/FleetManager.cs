using System;
using UnityEngine;

public class FleetManager : MonoBehaviour
{
    [SerializeField] private float tickRate = 1f;

    private float _timer;
    private UpgradeManager _upgradeManager;
    private CurrencyManager _currencyManager;
    private BoostManager _boostManager;
    private PrestigeManager _prestigeManager;

    public event Action<double> OnPassiveIncomeTick;

    public void Initialize(UpgradeManager upgradeManager, CurrencyManager currencyManager, BoostManager boostManager, PrestigeManager prestigeManager)
    {
        _upgradeManager = upgradeManager;
        _currencyManager = currencyManager;
        _boostManager = boostManager;
        _prestigeManager = prestigeManager;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        while (_timer >= tickRate)
        {
            _timer -= tickRate;
            double perSec = GetIncomePerSecond();
            _currencyManager.AddOre(perSec * tickRate);
            OnPassiveIncomeTick?.Invoke(perSec * tickRate);
        }
    }

    public double GetIncomePerSecond()
    {
        double fleet = _upgradeManager.GetFleetIncomePerSecond();
        double planet = _upgradeManager.GetPlanetMultiplier();
        double darkMatter = _prestigeManager.GetPermanentMultiplier();
        return fleet * planet * darkMatter * _boostManager.CurrentMultiplier();
    }
}
