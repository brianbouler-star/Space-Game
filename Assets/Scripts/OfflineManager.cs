using System;
using UnityEngine;

public class OfflineManager : MonoBehaviour
{
    [SerializeField] private int offlineHoursCap = 8;
    [SerializeField] private float offlineEfficiency = 0.5f;

    public double CalculateOfflineEarnings(long lastLoginUnix, double orePerSecond)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long elapsed = Math.Max(0, now - lastLoginUnix);
        long cap = offlineHoursCap * 3600;
        long effectiveSeconds = Math.Min(elapsed, cap);
        return orePerSecond * effectiveSeconds * offlineEfficiency;
    }
}
