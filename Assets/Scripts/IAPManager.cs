using System.Collections.Generic;
using UnityEngine;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

public class IAPManager : MonoBehaviour
#if UNITY_PURCHASING
    , IDetailedStoreListener
#endif
{
#if UNITY_PURCHASING
    private IStoreController _controller;
#endif

    private readonly Dictionary<string, int> _gemRewards = new Dictionary<string, int>
    {
        { ProductIds.Gems100, 100 },
        { ProductIds.Gems550, 550 },
        { ProductIds.Gems1200, 1200 },
        { ProductIds.Gems2500, 2500 },
        { ProductIds.Gems6500, 6500 }
    };

    private CurrencyManager _currency;

    private void Start()
    {
        _currency = FindObjectOfType<CurrencyManager>();
#if UNITY_PURCHASING
        InitializePurchasing();
#endif
    }

#if UNITY_PURCHASING
    private void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(ProductIds.RemoveAds, ProductType.NonConsumable);
        builder.AddProduct(ProductIds.VipSubscription, ProductType.Subscription);
        builder.AddProduct(ProductIds.Gems100, ProductType.Consumable);
        builder.AddProduct(ProductIds.Gems550, ProductType.Consumable);
        builder.AddProduct(ProductIds.Gems1200, ProductType.Consumable);
        builder.AddProduct(ProductIds.Gems2500, ProductType.Consumable);
        builder.AddProduct(ProductIds.Gems6500, ProductType.Consumable);
        UnityPurchasing.Initialize(this, builder);
    }

    public void Buy(string productId)
    {
        _controller?.InitiatePurchase(productId);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _controller = controller;
        ValidateVipState();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogWarning($"IAP initialize failed: {error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogWarning($"IAP initialize failed: {error} {message}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        string id = purchaseEvent.purchasedProduct.definition.id;
        if (id == ProductIds.RemoveAds)
        {
            GameManager.Instance?.ApplyRemoveAdsPurchase();
        }
        else if (id == ProductIds.VipSubscription)
        {
            GameManager.Instance?.SetVipActive(true);
        }
        else if (_gemRewards.TryGetValue(id, out int gems))
        {
            _currency?.AddGems(gems);
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogWarning($"Purchase failed: {product.definition.id} {failureReason}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogWarning($"Purchase failed: {failureDescription.productId} {failureDescription.reason} {failureDescription.message}");
    }

    private void ValidateVipState()
    {
        Product vip = _controller.products.WithID(ProductIds.VipSubscription);
        bool active = vip != null && vip.hasReceipt;
        GameManager.Instance?.SetVipActive(active);
    }
#else
    public void Buy(string productId)
    {
        Debug.Log($"IAP disabled in editor/package missing. Attempted: {productId}");
    }
#endif
}
