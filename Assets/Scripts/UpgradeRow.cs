using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeRow : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text effectText;
    [SerializeField] private TMP_Text ownedText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Image rowHighlight;

    private UpgradeDefinition _definition;
    private UpgradeManager _upgradeManager;
    private CurrencyManager _currencyManager;

    public void Bind(UpgradeDefinition definition, UpgradeManager upgradeManager, CurrencyManager currencyManager)
    {
        _definition = definition;
        _upgradeManager = upgradeManager;
        _currencyManager = currencyManager;

        if (nameText != null) nameText.text = definition.DisplayName;
        if (effectText != null) effectText.text = $"x{definition.Value}";

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(Buy);
        }

        _currencyManager.OnCurrencyChanged += Refresh;
        Refresh();
    }

    private void Buy()
    {
        _upgradeManager.PurchaseUpgrade(_definition, _currencyManager);
        Refresh();
    }

    private void Refresh()
    {
        int owned = _upgradeManager.GetOwnedCount(_definition.Id);
        double cost = _upgradeManager.GetCurrentCost(_definition, owned);
        bool affordable = _currencyManager.Ore >= cost;

        if (ownedText != null) ownedText.text = $"Owned: {owned}";
        if (costText != null) costText.text = CurrencyManager.FormatNumber(cost);
        if (buyButton != null) buyButton.interactable = affordable;
        if (rowHighlight != null) rowHighlight.enabled = affordable;
    }
}
