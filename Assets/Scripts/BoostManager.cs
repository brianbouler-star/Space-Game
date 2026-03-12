using System;
using UnityEngine;

public class BoostManager : MonoBehaviour
{
    [SerializeField] private float adBoostDurationSeconds = 1800f;
    [SerializeField] private float gemBoostDurationSeconds = 7200f;
    [SerializeField] private int gemBoostCost = 10;

    public long BoostEndUnix { get; private set; }
    public bool IsBoostActive => DateTimeOffset.UtcNow.ToUnixTimeSeconds() < BoostEndUnix;

    public event Action OnBoostChanged;

    public void Initialize(long boostEndUnix)
    {
        BoostEndUnix = boostEndUnix;
        OnBoostChanged?.Invoke();
    }

    public void ActivateAdBoost()
    {
        ExtendBoost((long)adBoostDurationSeconds);
    }

    public bool ActivateGemBoost(CurrencyManager currencyManager)
    {
        if (!currencyManager.SpendGems(gemBoostCost)) return false;
        ExtendBoost((long)gemBoostDurationSeconds);
        return true;
    }

    public float CurrentMultiplier()
    {
        return IsBoostActive ? 2f : 1f;
    }

    public TimeSpan RemainingTime()
    {
        long remaining = Math.Max(0, BoostEndUnix - DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        return TimeSpan.FromSeconds(remaining);
    }

    private void ExtendBoost(long seconds)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (BoostEndUnix < now) BoostEndUnix = now;
        BoostEndUnix += seconds;
        OnBoostChanged?.Invoke();
    }
}
