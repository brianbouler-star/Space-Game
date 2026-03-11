using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public double Ore { get; private set; }
    public double DarkMatter { get; private set; }
    public int Gems { get; private set; }
    public double TotalOreEarned { get; private set; }

    public event Action OnCurrencyChanged;

    public void Initialize(double ore, double darkMatter, int gems, double totalOre)
    {
        Ore = ore;
        DarkMatter = darkMatter;
        Gems = gems;
        TotalOreEarned = totalOre;
        OnCurrencyChanged?.Invoke();
    }

    public void AddOre(double amount)
    {
        if (amount <= 0) return;
        Ore += amount;
        TotalOreEarned += amount;
        OnCurrencyChanged?.Invoke();
    }

    public bool SpendOre(double amount)
    {
        if (Ore < amount) return false;
        Ore -= amount;
        OnCurrencyChanged?.Invoke();
        return true;
    }

    public void AddDarkMatter(double amount)
    {
        DarkMatter += Math.Max(0, amount);
        OnCurrencyChanged?.Invoke();
    }

    public bool SpendGems(int amount)
    {
        if (Gems < amount) return false;
        Gems -= amount;
        OnCurrencyChanged?.Invoke();
        return true;
    }

    public void AddGems(int amount)
    {
        Gems += Math.Max(0, amount);
        OnCurrencyChanged?.Invoke();
    }

    public static string FormatNumber(double value)
    {
        string[] suffixes = { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "De" };
        if (value < 1000d) return Math.Floor(value).ToString("0");

        int idx = 0;
        while (value >= 1000d && idx < suffixes.Length - 1)
        {
            value /= 1000d;
            idx++;
        }

        if (idx >= suffixes.Length - 1 && value >= 1000d)
        {
            return value.ToString("0.###E+0");
        }

        return value.ToString(value >= 100 ? "0" : "0.0") + suffixes[idx];
    }
}
