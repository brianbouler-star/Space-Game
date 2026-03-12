using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class AdsManager : MonoBehaviour
#if UNITY_ADS
    , IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
#endif
{
    [SerializeField] private string androidGameId = "YOUR_UNITY_ADS_GAME_ID";
    [SerializeField] private bool testMode = true;

    private BoostManager _boost;
    private CurrencyManager _currency;

    private void Start()
    {
        _boost = FindObjectOfType<BoostManager>();
        _currency = FindObjectOfType<CurrencyManager>();

#if UNITY_ADS
        Advertisement.Initialize(androidGameId, testMode, this);
#endif
    }

    public void ShowRewardedBoostAd()
    {
#if UNITY_ADS
        Advertisement.Show(AdsPlacementIds.RewardedBoost, this);
#else
        _boost?.ActivateAdBoost();
#endif
    }

    public void ShowRewardedGemsAd(int baseGems = 5)
    {
#if UNITY_ADS
        Advertisement.Show(AdsPlacementIds.RewardedGems, this);
#else
        _currency?.AddGems(baseGems);
#endif
    }

    public void ShowInterstitialPrestige()
    {
        if (GameManager.Instance != null && GameManager.Instance.RemoveAdsPurchased) return;
#if UNITY_ADS
        Advertisement.Show(AdsPlacementIds.InterstitialPrestige, this);
#endif
    }

#if UNITY_ADS
    public void OnInitializationComplete()
    {
        Advertisement.Load(AdsPlacementIds.RewardedBoost, this);
        Advertisement.Load(AdsPlacementIds.RewardedGems, this);
        Advertisement.Load(AdsPlacementIds.InterstitialPrestige, this);
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogWarning($"Ads init failed: {error} - {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId) { }
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"Ad load failed: {placementId}, {error}, {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"Ad show failed: {placementId}, {error}, {message}");
    }

    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (showCompletionState != UnityAdsShowCompletionState.COMPLETED) return;

        if (placementId == AdsPlacementIds.RewardedBoost)
        {
            _boost?.ActivateAdBoost();
        }
        else if (placementId == AdsPlacementIds.RewardedGems)
        {
            int gemReward = GameManager.Instance != null && GameManager.Instance.VipActive ? 10 : 5;
            _currency?.AddGems(gemReward);
        }

        Advertisement.Load(placementId, this);
    }
#endif
}
