# Space Miner Idle - Unity 6 Android Build Guide

## 1) Implemented scripts
Place scripts under `Assets/Scripts/`.

- `GameManager.cs` - bootstrap, offline award, prestige reset, lifecycle save hooks.
- `TapManager.cs` - tap income + instant feedback (particle, floating text, sound, haptic).
- `UpgradeManager.cs` - mining/fleet/planet definitions and purchase logic.
- `FleetManager.cs` - passive income tick loop.
- `PrestigeManager.cs` - unlock/preview/award dark matter.
- `CurrencyManager.cs` - ore/dark matter/gems bookkeeping + number formatting.
- `OfflineManager.cs` - closed-app earnings (8h cap, 50% efficiency).
- `AdsManager.cs` - rewarded/interstitial hooks with `UNITY_ADS` compile guards.
- `IAPManager.cs` - product catalog + fulfillment with `UNITY_PURCHASING` guards.
- `UIManager.cs` - HUD refresh, boost timer, popups.
- `SaveManager.cs` - PlayerPrefs persistence + auto-save every 30s.
- `BoostManager.cs` - ad boost + gem boost duration handling.
- `UpgradeRow.cs` - reusable upgrade-list row binder.
- `FloatingText.cs` - floating tap reward text.
- `LoadingSceneController.cs` - load `MainScene`.

---

## 2) Scene setup instructions

## Scene: `LoadingScene`
1. Create a new scene called `LoadingScene`.
2. Add an empty object `LoadingRoot` and attach `LoadingSceneController`.
3. (Optional) Add logo/splash UI.
4. Add `MainScene` to Build Settings and ensure `LoadingScene` is first.

## Scene: `MainScene`
1. Create an empty `GameSystems` object and attach:
   - `GameManager`
   - `CurrencyManager`
   - `UpgradeManager`
   - `FleetManager`
   - `PrestigeManager`
   - `OfflineManager`
   - `SaveManager`
   - `BoostManager`
   - `AdsManager`
   - `IAPManager`
   - `UIManager`
2. Wire all serialized references in `GameManager` and `TapManager`.
3. Create a `Canvas` (Screen Space - Overlay), portrait-safe anchors.
4. Build HUD:
   - Top row: Gems text, VIP badge object, Settings button.
   - Ore text (large), Ore/sec text.
   - Asteroid button image centered; attach `TapManager` to asteroid object.
   - Boost controls: `Watch Ad` and `Spend 10 Gems` buttons + boost timer text.
   - Tabs row: Upgrades / Fleet / Planets.
   - ScrollView list area for `UpgradeRow` instances.
   - Bottom panel reserved for banner ad.
5. Create popup panel with message text + close button and connect to `UIManager`.
6. Add `EventSystem` if missing.
7. Audio:
   - Add `AudioSource` on asteroid/tap object for tap SFX.
   - Add upgrade and prestige SFX audio sources and trigger from UI purchase handlers.

---

## 3) Unity IAP Product IDs

- Non-consumable:
  - `remove_ads` ($2.99)
- Subscription:
  - `vip_pass_monthly` ($4.99/month)
- Consumables:
  - `gems_100` ($0.99)
  - `gems_550` ($4.99)
  - `gems_1200` ($9.99)
  - `gems_2500` ($19.99)
  - `gems_6500` ($49.99)

> Keep IDs exactly identical in Unity Dashboard + Google Play Console.

---

## 4) Unity Ads placement IDs setup

Use these placement names in Unity LevelPlay/Ads Dashboard and match constants in `GameData.cs`:

- `Rewarded_Boost`
- `Rewarded_Gems`
- `Banner_Bottom`
- `Interstitial_Prestige`

In `AdsManager`:
1. Set `androidGameId` from Unity Ads dashboard.
2. Set `testMode=true` during QA, then false for release.
3. Hide banner/interstitial presentation when `remove_ads` is purchased.

---

## 5) PlayerPrefs key list

- `save_ore`
- `save_dark_matter`
- `save_gems`
- `save_total_ore`
- `save_prestige_count`
- `save_remove_ads`
- `save_vip_active`
- `save_last_login`
- `save_boost_end`
- `save_owned_mining`
- `save_owned_fleet`
- `save_owned_planets`

---

## 6) Build APK/AAB for Google Play

1. **Install modules**
   - Unity Hub → Unity 6 LTS → Android Build Support + SDK/NDK + OpenJDK.
2. **Project settings**
   - Player Settings:
     - Company/Product + package name: `com.yourname.spacemineridle`
     - Orientation: Portrait only
     - Minimum API level: 26
     - Scripting backend: IL2CPP
     - Target architectures: ARM64
3. **Services setup**
   - Enable Unity Ads and Unity IAP package/services.
   - Configure Google Play Billing in IAP Catalog.
4. **Keystore**
   - Create/upload-release keystore and keep credentials safe.
5. **Build Settings**
   - Add scenes in order: `LoadingScene`, `MainScene`.
   - Platform: Android.
   - Build App Bundle (AAB) for Play Store.
6. **Google Play Console**
   - Create app listing.
   - Add in-app products + subscription with matching IDs.
   - Upload AAB to internal test track.
   - Test purchase flows with license test accounts.
7. **Release checks**
   - Verify offline gains, ad disable purchase, VIP state restore, and no-internet behavior.

---

## 7) Recommended free Asset Store packages (visual polish)

- **TextMeshPro** (essential UI text quality).
- **DOTween (Free)** for punch/pulse animations.
- **Simple Space Skybox** / free nebula skybox packs.
- **Kenney UI Pack** for icon placeholders.
- **Free Casual Game SFX Pack** for taps/upgrades/prestige cues.
- **URP Post-processing samples** for glow/bloom accents.

---

## Notes for v1 scope
- Keep one asteroid art variant and one scene gameplay.
- Delay achievements, leaderboards, social, and cloud save to post-launch.
- Use internal test track telemetry to tune upgrade prices and prestige speed.
