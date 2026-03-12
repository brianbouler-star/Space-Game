using UnityEngine;
using UnityEngine.EventSystems;

public class TapManager : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private PrestigeManager prestigeManager;
    [SerializeField] private BoostManager boostManager;
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private ParticleSystem tapParticles;
    [SerializeField] private AudioSource tapAudio;

    public void OnPointerClick(PointerEventData eventData)
    {
        double tapValue = GetTapValue();
        currencyManager.AddOre(tapValue);

        if (tapParticles != null) tapParticles.Play();
        if (tapAudio != null) tapAudio.Play();
        if (floatingTextPrefab != null)
        {
            GameObject spawned = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform.parent);
            FloatingText text = spawned.GetComponent<FloatingText>();
            if (text != null) text.SetText("+" + CurrencyManager.FormatNumber(tapValue));
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }

    public double GetTapValue()
    {
        return 1d * upgradeManager.GetTapMultiplier() * upgradeManager.GetPlanetMultiplier() * prestigeManager.GetPermanentMultiplier() * boostManager.CurrentMultiplier();
    }
}
