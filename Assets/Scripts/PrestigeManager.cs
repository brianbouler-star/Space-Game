using UnityEngine;

public class PrestigeManager : MonoBehaviour
{
    [SerializeField] private double prestigeUnlockOre = 1000000000d;
    [SerializeField] private double darkMatterDivisor = 100000000d;

    private CurrencyManager _currency;

    public int PrestigeCount { get; private set; }

    public void Initialize(CurrencyManager currency, int prestigeCount)
    {
        _currency = currency;
        PrestigeCount = prestigeCount;
    }

    public bool CanPrestige()
    {
        return _currency.TotalOreEarned >= prestigeUnlockOre;
    }

    public int PreviewDarkMatterEarned()
    {
        return Mathf.Max(1, Mathf.FloorToInt((float)(_currency.TotalOreEarned / darkMatterDivisor)));
    }

    public double GetPermanentMultiplier()
    {
        return 1d + (_currency.DarkMatter * 0.1d);
    }

    public int PerformPrestige()
    {
        if (!CanPrestige()) return 0;

        int gained = PreviewDarkMatterEarned();
        _currency.AddDarkMatter(gained);
        PrestigeCount++;
        return gained;
    }
}
