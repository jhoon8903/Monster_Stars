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
        public int rewardedRetryAttempt;

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

            _instance = this;
        }

        public void Start()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += _ =>
                {
                    InitializeInterstitialAds();
                    InitializeRewardedAds();
                    InitializeBannerAds();
                };
                MaxSdk.SetSdkKey(MaxSdkKey);
                MaxSdk.InitializeSdk();
        }

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
            MaxSdkUnityEditor.LoadInterstitial(InterstitialAdUnitId);
        }
        public static void ShowInterstitial()
        {
            if (MaxSdkUnityEditor.IsInterstitialReady(InterstitialAdUnitId))
            {
                MaxSdkUnityEditor.ShowInterstitial(InterstitialAdUnitId);
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
        }

        private void InitializeRewardedAds()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            LoadRewardedAd();
        }

        private void LoadRewardedAd()
        {
            MaxSdkUnityEditor.LoadRewardedAd(RewardedAdUnitId);

        }

        public static void ShowRewardedAd()
        {
            if (MaxSdkUnityEditor.IsRewardedAdReady(RewardedAdUnitId))
            {
                Debug.Log("Showing");
                MaxSdkUnityEditor.ShowRewardedAd(RewardedAdUnitId);
         
            }
            else
            {
                Debug.Log("Ad not ready");
            }
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad loaded");
            rewardedRetryAttempt = 0;
        }

        private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            rewardedRetryAttempt++;
            var retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));
            Debug.Log("Rewarded ad failed to load with error code: " + errorInfo.Code);
            Invoke(nameof(LoadRewardedAd), (float)retryDelay);
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
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

        private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("닫음");
            LoadRewardedAd();

        }

        private static void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad received reward");
        }

        private static void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad revenue paid");
        }

        private static void InitializeBannerAds()
        {
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
            MaxSdkAndroid.CreateBanner(BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
            MaxSdkAndroid.SetBannerBackgroundColor(BannerAdUnitId, Color.white);
        }

        private static void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Banner ad loaded");
        }

        private static void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.Log("Banner ad failed to load with error code: " + errorInfo.Code);
        }

        private static void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Banner ad clicked");
        }

        private static void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Banner ad revenue paid");
        }
    }
}