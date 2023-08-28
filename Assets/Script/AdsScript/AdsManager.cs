using System;
using Script.QuestGroup;
using Script.RewardScript;
using Script.RobbyScript.MainMenuGroup;
using Script.RobbyScript.StoreMenuGroup;
using Script.RobbyScript.TopMenuGroup;
using Script.UIManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// ReSharper disable All

namespace Script.AdsScript
{
    public class AdsManager : MonoBehaviour
    {
        [SerializeField] private GameObject coinBtn;
        [SerializeField] private GameObject staminaBtn;
        [SerializeField] private GameObject gemBtn;
        [SerializeField] private QuestManager questManager;
        public static AdsManager Instance { get; private set; }
        private const string MaxSdkKey = "EG4dCO2mV2THPcolJ7UkHmIGIfTqtwfpRaimZ-lyk-OV5RSBpi4KMT6P3FnnemsgdzXD-3swcClOldu3";
        private const string BannerAdUnitId = "BannerAdUnitId";
        private const string AdUnitId = "d70473ac274d7b4d";
        private const string InterstitialAdUnitId = "InterstitialAdUnitId";
        private int _interstitialRetryAttempt;
        public int rewardedRetryAttempt;
        public int adsMoveCount;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
            coinBtn.GetComponent<Button>().onClick.AddListener(Coin);
            staminaBtn.GetComponent<Button>().onClick.AddListener(Stamina);
            gemBtn.GetComponent<Button>().onClick.AddListener(Gem);
        }
        
        public void Start()
        {
            // AppLovinMax 초기화
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                // AppLovin SDK is initialized, configure and start loading ads.
                Debug.Log("MAX SDK Initialized");

                InitializeInterstitialAds();
                InitializeRewardedAds();
                InitializeBannerAds();

                // 배너 실행
                // MaxSdk.ShowBanner(BannerAdUnitId);
            };

            MaxSdk.SetSdkKey(MaxSdkKey);
            MaxSdk.SetTestDeviceAdvertisingIdentifiers(new string[]{"67ab33ef-93d8-4fe2-a3cc-44bd6b0e2a59"});
            MaxSdk.InitializeSdk();
        }


        #region Interstitial Ad Methods

        private void InitializeInterstitialAds()
        {
            // Attach callbacks
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;

            // Load the first interstitial
            LoadInterstitial();
        }

        void LoadInterstitial()
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
            // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
            Debug.Log("Interstitial loaded");

            // Reset retry attempt
            _interstitialRetryAttempt = 0;
        }

        private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            _interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _interstitialRetryAttempt));

            Debug.Log("Interstitial failed to load with error code: " + errorInfo.Code);

            Invoke("LoadInterstitial", (float)retryDelay);
        }

        private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. We recommend loading the next ad
            Debug.Log("Interstitial failed to display with error code: " + errorInfo.Code);
            LoadInterstitial();
        }

        private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad
            Debug.Log("Interstitial dismissed");
            LoadInterstitial();
        }

        private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad revenue paid. Use this callback to track user revenue.
            Debug.Log("Interstitial revenue paid");

            // Ad revenue
            double revenue = adInfo.Revenue;

            // Miscellaneous data
            string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

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
            Debug.Log("Loading...");
            MaxSdk.LoadRewardedAd(AdUnitId);
        }

        public void ShowRewardedAd()
        {
            if (MaxSdk.IsRewardedAdReady(AdUnitId))
            {
                Debug.Log("Showing");
                MaxSdk.ShowRewardedAd(AdUnitId);
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
            rewardedRetryAttempt = 0;
        }

        private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            rewardedRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));

            Debug.Log("Rewarded ad failed to load with error code: " + errorInfo.Code);

            Invoke("LoadRewardedAd", (float)retryDelay);
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. We recommend loading the next ad
            Debug.Log("Rewarded ad failed to display with error code: " + errorInfo.Code);
            LoadRewardedAd();
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad displayed");
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad clicked");
        }

        // closebtn event
        private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            RewardButtonClicked();
            Quest.Instance.AdsViewQuest();
            LoadRewardedAd();
        }

        private static void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
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

        private static void InitializeBannerAds()
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

        public enum ButtonType
        {
            Coin, 
            Stamina, 
            Gem, 
            BronzeAds, 
            SilverAds, 
            GoldAds, 
            Retry, 
            Common, 
            LevelUp, 
            EnergyPackFreeStamina, 
            GemPackFree,
            CoinPackFree, 
            ShuffleQuest, 
            AdsStart,
            None
        }

        public StoreMenu.BoxGrade boxGrade;
        public bool isRetry;
        public ButtonType ButtonTypes { get; set; }

        public void Coin()
        {
            ShowRewardedAd();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_gold");
            ButtonTypes = ButtonType.Coin;
        }
        public void Stamina()
        {
            ShowRewardedAd();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_stamina");
            ButtonTypes = ButtonType.Stamina;
        }
        public void Gem()
        {
            ShowRewardedAd();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_gem");
            ButtonTypes = ButtonType.Gem;
        }
        private void RewardButtonClicked()
        {

            StoreMenu.ButtonType chestType;
            switch (ButtonTypes)
            {
                case ButtonType.Coin:
                    boxGrade = StoreMenu.BoxGrade.Coin;
                    StoreMenu.Instance.OpenAds(boxGrade);
                    GiveReward(ButtonType.Coin);
                    break;
                case ButtonType.Gem:
                    boxGrade = StoreMenu.BoxGrade.Gem;
                    StoreMenu.Instance.OpenAds(boxGrade);
                    GiveReward(ButtonType.Gem);
                    break;
                case ButtonType.Stamina:
                    boxGrade = StoreMenu.BoxGrade.Stamina;
                    StoreMenu.Instance.OpenAds(boxGrade);
                    GiveReward(ButtonType.Stamina);
                    break;
                case ButtonType.Retry:
                    isRetry = true;
                    Retry();
                    break;
                case ButtonType.BronzeAds:
                    chestType = StoreMenu.ButtonType.BronzeAds;
                    StoreMenu.Instance.SummonChest(chestType);
                    break;
                case ButtonType.SilverAds:
                    chestType = StoreMenu.ButtonType.SilverAds;
                    StoreMenu.Instance.SummonChest(chestType);
                    break;
                case ButtonType.GoldAds:
                    chestType = StoreMenu.ButtonType.GoldAds;
                    StoreMenu.Instance.SummonChest(chestType);
                    break;
                case ButtonType.Common:
                    CommonShuffle();
                    break;
                case ButtonType.LevelUp:
                    ExpShuffle();
                    break;
                case ButtonType.EnergyPackFreeStamina:
                     EnergyPackFree();
                     break;
                case ButtonType.None:
                    break;
                case ButtonType.GemPackFree:
                    FreeGemPack();
                    break;
                case ButtonType.CoinPackFree:
                    FreeCoinPack();
                    break;
                case ButtonType.ShuffleQuest:
                    StartCoroutine(questManager.ShuffleQuest());
                    break;
                case ButtonType.AdsStart:
                    adsMoveCount += 6;
                    MainPanel.Instance.StartGame();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private static void GiveReward(ButtonType buttonType)
        {
            switch (buttonType)
            {
                case ButtonType.Coin:
                    // CoinsScript.Instance.Coin += 1000;
                    break;
                case ButtonType.Gem:
                    GemScript.Instance.Gem += 200;
                    break;
                case ButtonType.Stamina:
                {
                    StaminaScript.Instance.CurrentStamina += 10;
                }
                    break;
            }
        }

        public void RetryAds()
        {
            ShowRewardedAd();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_revive");
            ButtonTypes = ButtonType.Retry;
        }
        private void Retry()
        {
            if (!isRetry) return;
            SceneManager.LoadScene("StageScene");
            isRetry = false;
        }
        private static void CommonShuffle()
        {
            CommonRewardManager.Instance.ReEnqueueTreasure();
        }
        private static void ExpShuffle()
        {
            Debug.Log("레벨업 동작");
            LevelUpRewardManager.Instance.ReLevelUpReward();
        }
        private void EnergyPackFree()
        {
            Debug.Log("스테미나 10 지급");
            StaminaScript.Instance.CurrentStamina += 10;
        }

        private void FreeGemPack()
        {
            Debug.Log("젬 10 지급");
            GemScript.Instance.Gem += 10;
        }

        private void FreeCoinPack()
        {
            Debug.Log("코인 10 지급");
            CoinsScript.Instance.Coin += 10;
        }
    }
}