using System;
using UnityEngine;

namespace Script.AdsScript
{
    public class AppLovinScript : MonoBehaviour
    {
        private static AppLovinScript _instance;
        private const string MaxSdkKey = "EG4dCO2mV2THPcolJ7UkHmIGIfTqtwfpRaimZ-lyk-OV5RSBpi4KMT6P3FnnemsgdzXD-3swcClOldu3";
        private const string BannerAdUnitId = "BannerAdUnitId";
        private const string InterstitialAdUnitId = "InterstitialAdUnitId";
        private const string RewardedAdUnitId = "d70473ac274d7b4d";
        private int _interstitialRetryAttempt;
        private int _rewardedRetryAttempt;

        public static AppLovinScript Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var singletonObject = new GameObject();
                _instance = singletonObject.AddComponent<AppLovinScript>();
                singletonObject.name = "AppLovinMax (Singleton)";
                DontDestroyOnLoad(singletonObject);
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void Start()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                InitializeInterstitialAds();
                InitializeRewardedAds();
                InitializeBannerAds();
            };
            MaxSdk.SetSdkKey(MaxSdkKey);
            MaxSdk.InitializeSdk();
        }

        #region Interstitial Ad Methods

        private void InitializeInterstitialAds()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;
            LoadInterstitial();
        }

        private void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(InterstitialAdUnitId);
        }
        public static void ShowInterstitial()
        {
            if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
            {
                MaxSdk.ShowInterstitial(InterstitialAdUnitId);
            }
            else
            {
                Debug.Log("Ad not ready");
            }
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _interstitialRetryAttempt = 0;
        }

        private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            _interstitialRetryAttempt++;
            var retryDelay = Math.Pow(2, Math.Min(6, _interstitialRetryAttempt));
            Debug.Log("Interstitial failed to load with error code: " + errorInfo.Code);
            Invoke(nameof(LoadInterstitial), (float)retryDelay);
        }

        private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Interstitial failed to display with error code: " + errorInfo.Code);
            LoadInterstitial();
        }

        private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Interstitial dismissed");
            LoadInterstitial();
        }

        private static void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Interstitial revenue paid");
            var revenue = adInfo.Revenue;
            var countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            var networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            var adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            var placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        }

        #endregion

        #region Rewarded Ad Methods

        private void InitializeRewardedAds()
        {
            // Attach callbacks
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;

            // Load the first RewardedAd
            LoadRewardedAd();
        }

        private void LoadRewardedAd()
        {
            // Debug.Log("Loading...");
            MaxSdk.LoadRewardedAd(RewardedAdUnitId);
        }

        public static void ShowRewardedAd()
        {
            if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
            {
                Debug.Log("Showing");
                MaxSdk.ShowRewardedAd(RewardedAdUnitId);
            }
            else
            {
                Debug.Log("Ad not ready");
            }
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
            Debug.Log("Rewarded ad loaded");

            // Reset retry attempt
            _rewardedRetryAttempt = 0;
        }

        private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            _rewardedRetryAttempt++;
            var retryDelay = Math.Pow(2, Math.Min(6, _rewardedRetryAttempt));

            Debug.Log("Rewarded ad failed to load with error code: " + errorInfo.Code);

            Invoke(nameof(LoadRewardedAd), (float)retryDelay);
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. We recommend loading the next ad
            Debug.Log("Rewarded ad failed to display with error code: " + errorInfo.Code);
            LoadRewardedAd();
        }

        private static void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad displayed");
        }

        private static void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad clicked");
        }

        // closeBtn event
        private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            Debug.Log("Rewarded ad dismissed");
            Debug.Log("닫음");
            LoadRewardedAd();
        }

        private static void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad was displayed and user should receive the reward
            Debug.Log("Rewarded ad received reward");
        }

        private static void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad revenue paid. Use this callback to track user revenue.
            Debug.Log("Rewarded ad revenue paid");

            // Ad revenue
            var revenue = adInfo.Revenue;

            // Miscellaneous data
            var countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            var networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            var adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            var placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        }

        #endregion

        #region Banner Ad Methods

        private void InitializeBannerAds()
        {
            // Attach Callbacks
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;

            // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
            // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
            MaxSdk.CreateBanner(BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

            // Set background or background color for banners to be fully functional.
            MaxSdk.SetBannerBackgroundColor(BannerAdUnitId, Color.white);
        }

        private static void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Banner ad is ready to be shown.
            // If you have already called MaxSdk.ShowBanner(BannerAdUnitId) it will automatically be shown on the next ad refresh.
            Debug.Log("Banner ad loaded");
        }

        private static void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Banner ad failed to load. MAX will automatically try loading a new ad internally.
            Debug.Log("Banner ad failed to load with error code: " + errorInfo.Code);
        }

        private static void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Banner ad clicked");
        }

        private static void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Banner ad revenue paid. Use this callback to track user revenue.
            Debug.Log("Banner ad revenue paid");

            // Ad revenue
            var revenue = adInfo.Revenue;

            // Miscellaneous data
            var countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            var networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            var adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            var placement = adInfo.Placement; // The placement this ad's postbacks are tied to
        }
        #endregion
    }
}