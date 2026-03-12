using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Top Bar")]
    [SerializeField] private TMP_Text oreText;
    [SerializeField] private TMP_Text orePerSecondText;
    [SerializeField] private TMP_Text gemsText;
    [SerializeField] private GameObject vipBadge;

    [Header("Boost")]
    [SerializeField] private TMP_Text boostText;
    [SerializeField] private Button boostAdButton;
    [SerializeField] private Button boostGemButton;

    [Header("Popups")]
    [SerializeField] private TMP_Text popupText;
    [SerializeField] private GameObject popupPanel;

    private CurrencyManager _currency;
    private FleetManager _fleet;
    private PrestigeManager _prestige;
    private BoostManager _boost;

    private float _refreshTimer;

    public void Initialize(GameManager gameManager, CurrencyManager currency, UpgradeManager upgrades, FleetManager fleet, PrestigeManager prestige, BoostManager boost)
    {
        _currency = currency;
        _fleet = fleet;
        _prestige = prestige;
        _boost = boost;

        _currency.OnCurrencyChanged += Refresh;
        _boost.OnBoostChanged += Refresh;

        if (vipBadge != null) vipBadge.SetActive(gameManager.VipActive);

        if (boostAdButton != null)
            boostAdButton.onClick.AddListener(() => FindObjectOfType<AdsManager>()?.ShowRewardedBoostAd());

        if (boostGemButton != null)
            boostGemButton.onClick.AddListener(() =>
            {
                if (_boost.ActivateGemBoost(_currency)) Refresh();
            });

        Refresh();
    }

    private void Update()
    {
        _refreshTimer += Time.deltaTime;
        if (_refreshTimer >= 0.1f)
        {
            _refreshTimer = 0f;
            RefreshPassiveRate();
            RefreshBoostTimer();
        }
    }

    private void Refresh()
    {
        if (oreText != null) oreText.text = $"ORE: {CurrencyManager.FormatNumber(_currency.Ore)}";
        if (gemsText != null) gemsText.text = $"Gems: {_currency.Gems}";
        RefreshPassiveRate();
        RefreshBoostTimer();
    }

    private void RefreshPassiveRate()
    {
        if (orePerSecondText != null)
            orePerSecondText.text = $"⚡ {CurrencyManager.FormatNumber(_fleet.GetIncomePerSecond())}/sec";
    }

    private void RefreshBoostTimer()
    {
        if (boostText == null) return;
        if (!_boost.IsBoostActive)
        {
            boostText.text = "2x BOOST Ready";
            return;
        }

        var remaining = _boost.RemainingTime();
        boostText.text = $"2x BOOST {remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
    }

    public void ShowOfflineEarnings(double amount)
    {
        ShowPopup($"Welcome back! You earned {CurrencyManager.FormatNumber(amount)} ore while away.");
    }

    public void ShowPrestigeResult(int darkMatter)
    {
        ShowPopup($"Galaxy Reset complete! +{darkMatter} Dark Matter earned.");
    }

    private void ShowPopup(string message)
    {
        if (popupPanel != null) popupPanel.SetActive(true);
        if (popupText != null) popupText.text = message;
    }

    public void HidePopup()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
    }

    public void ShowPrestigePreview()
    {
        if (_prestige == null) return;
        ShowPopup($"Prestige now for {_prestige.PreviewDarkMatterEarned()} Dark Matter?");
    }
}
