using System;
using System.Collections;
using System.Collections.Generic;
using Script.AdsScript;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.RobbyScript.TopMenuGroup;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Script.RobbyScript.StoreMenuGroup
{
    public class TreasureChest : MonoBehaviour
    {
        public GameObject bronzeGemBtn;
        public GameObject silverGemBtn;
        public GameObject goldGemBtn;
        public GameObject bronzeAdsBtn;
        public GameObject silverAdsBtn;
        public GameObject goldAdsBtn;
        public Sprite bronzeSprite;
        public Sprite silverSprite;
        public Sprite goldSprite;
        public Sprite adsBtn;
        public Sprite gemBtn;
        public TextMeshProUGUI bronzeGemBtnText;
        public TextMeshProUGUI silverGemBtnText;
        public TextMeshProUGUI goldGemBtnText;

        private const int BronzeGemText = 150;
        private const int SilverGemText = 450;
        private const int GoldGemText = 900;

        public int BronzeOpenCount { get; set; }
        public const string BronzeOpenCountKey = "BronzeOpenCount";
        public const int BronzeOpenMaxCount = 20;
        public int SilverOpenCount { get; set; }
        public const string SilverOpenCountKey = "SilverOpenCount";
        public DateTime SilverOpenTime;
        public const string SilverOpenTimeKey = "SilverOpenTime";
        private TimeSpan SilverPassed { get; set; }
        public const int SilverOpenMaxCount = 7;
        private const int SilverRewardCoolTime = 5;

        public int GoldOpenCount { get; set; }
        public const string GoldOpenCountKey = "GoldOpenCount";
        public DateTime GoldOpenTime;
        public const string GoldOpenTimeKey = "GoldOpenTimeKey";
        private TimeSpan GoldPassed { get; set; }
        public const int GoldOpenMaxCount = 5;
        private const int GoldRewardCoolTime = 30;

        public TreasureChest TreasureInstance { get; private set; }

        public void InstanceTreasureChest()
        {
            // Green Count Check
            if (PlayerPrefs.HasKey(BronzeOpenCountKey))
            {
                BronzeOpenCount = PlayerPrefs.GetInt(BronzeOpenCountKey);
            }
            else
            {
                BronzeOpenCount = 0;
                PlayerPrefs.SetInt(BronzeOpenCountKey, BronzeOpenCount);
            }
            // Blue Count Check
            if (PlayerPrefs.HasKey(SilverOpenCountKey))
            {
                SilverOpenCount = PlayerPrefs.GetInt(SilverOpenCountKey);
            }
            else
            {
                SilverOpenCount = 0;
                PlayerPrefs.SetInt(SilverOpenCountKey, SilverOpenCount);
            }
            // Blue last Open Time Check
            if (PlayerPrefs.HasKey(SilverOpenTimeKey))
            {
                SilverOpenTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(SilverOpenTimeKey)));
            }
            else
            {
                SilverOpenTime = DateTime.Now.AddMinutes(-SilverRewardCoolTime);
                PlayerPrefs.SetString(SilverOpenTimeKey, SilverOpenTime.ToBinary().ToString());
                PlayerPrefs.Save();
            }
            // Purple Count Check
            if (PlayerPrefs.HasKey(GoldOpenCountKey))
            {
                GoldOpenCount = PlayerPrefs.GetInt(GoldOpenCountKey);
            }
            else
            {
                GoldOpenCount = 0;
                PlayerPrefs.SetInt(GoldOpenCountKey, GoldOpenCount);
            }
            // Purple Last Open Check
            if (PlayerPrefs.HasKey(GoldOpenTimeKey))
            {
                GoldOpenTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(GoldOpenTimeKey)));
            }
            else
            {
                GoldOpenTime = DateTime.Now.AddMinutes(-GoldRewardCoolTime);
                PlayerPrefs.SetString(GoldOpenTimeKey, GoldOpenTime.ToBinary().ToString());
                PlayerPrefs.Save();
            }
            TreasureInstance = Instantiate(this, StoreMenu.Instance.treasureLayer);
            TreasureInstance.ChestBtnSet();
        }
        public void ResetTreasureChest()
        {
            BronzeOpenCount = 0;
            PlayerPrefs.SetInt(BronzeOpenCountKey, BronzeOpenCount);
            SilverOpenCount = 0;
            PlayerPrefs.SetInt(SilverOpenCountKey, SilverOpenCount);
            SilverOpenTime = DateTime.Now.AddMinutes(-SilverRewardCoolTime);
            PlayerPrefs.SetString(SilverOpenTimeKey, SilverOpenTime.ToBinary().ToString());
            GoldOpenCount = 0;
            PlayerPrefs.SetInt(GoldOpenCountKey, GoldOpenCount);
            GoldOpenTime = DateTime.Now.AddMinutes(-GoldRewardCoolTime);
            PlayerPrefs.SetString(GoldOpenTimeKey, GoldOpenTime.ToBinary().ToString());
            PlayerPrefs.Save();
        }
        public void UpdateButtonState()
        {
            UpdateBronzeButton();
            UpdateSilverButton();
            UpdateGoldButton();
            UpdateBtnColor();
        }
        public void ChestBtnSet()
        {
            bronzeAdsBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheck.Instance.ChestCheckClick(StoreMenu.ButtonType.BronzeAds));
            silverAdsBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheck.Instance.ChestCheckClick(StoreMenu.ButtonType.SilverAds));
            goldAdsBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheck.Instance.ChestCheckClick(StoreMenu.ButtonType.GoldAds));
            bronzeGemBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheck.Instance.ChestCheckClick(StoreMenu.ButtonType.BronzeGem));
            silverGemBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheck.Instance.ChestCheckClick(StoreMenu.ButtonType.SilverGem));
            goldGemBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheck.Instance.ChestCheckClick(StoreMenu.ButtonType.GoldGem));
        }
        private void UpdateBtnColor()
        {
            var currentGemCount = GemScript.Instance.Gem;
            bronzeGemBtnText.color = BronzeGemText > currentGemCount ? Color.red : Color.white;
            silverGemBtnText.color = SilverGemText > currentGemCount ? Color.red : Color.white;
            goldGemBtnText.color = GoldGemText > currentGemCount ? Color.red : Color.white;
        }
        public void ChestBtnRemove()
        {
            bronzeAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            bronzeGemBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            silverAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            goldAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            goldGemBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            silverGemBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        private void UpdateBronzeButton()
        {
            BronzeOpenCount = PlayerPrefs.GetInt(BronzeOpenCountKey);
            if (StoreMenu.Instance.isReset || BronzeOpenCount < BronzeOpenMaxCount)
            {
                bronzeAdsBtn.GetComponent<Button>().interactable = true;
                bronzeAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{BronzeOpenCount} / {BronzeOpenMaxCount}";
            }
            else
            {
                var resetTime = StoreMenu.Instance.LastDayCheck.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
                Debug.Log(StoreMenu.Instance.LastDayCheck);
                bronzeAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{resetTime}";
            }
            bronzeGemBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = "150";
        }
        private void UpdateSilverButton()
        {
            SilverOpenTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(SilverOpenTimeKey)));
            SilverPassed = DateTime.Now - SilverOpenTime;
            SilverOpenCount = PlayerPrefs.GetInt(SilverOpenCountKey);
            if (StoreMenu.Instance.isReset || (SilverOpenCount < SilverOpenMaxCount && SilverPassed.TotalMinutes >= SilverRewardCoolTime))
            {
                silverAdsBtn.GetComponent<Button>().interactable = true;
                silverAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{SilverOpenCount} / {SilverOpenMaxCount}";
            }
            else
            {
                silverAdsBtn.GetComponent<Button>().interactable = false;
                var remainingTime = TimeSpan.FromMinutes(SilverRewardCoolTime) - SilverPassed;
                if (SilverOpenCount == SilverOpenMaxCount)
                {
                    var resetTime = StoreMenu.Instance.LastDayCheck.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
                    silverAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{resetTime}";
                }
                else if (remainingTime > TimeSpan.Zero)
                {
                    var remainingTimeText = remainingTime.ToString(@"mm\:ss");
                    silverAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = remainingTimeText;
                }
            }
            silverGemBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = "450";
        }
        private void UpdateGoldButton()
        {
            GoldOpenTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(GoldOpenTimeKey)));
            GoldPassed = DateTime.Now - GoldOpenTime;
            GoldOpenCount = PlayerPrefs.GetInt(GoldOpenCountKey);
            if (StoreMenu.Instance.isReset || (GoldOpenCount < GoldOpenMaxCount && GoldPassed.TotalMinutes >= GoldRewardCoolTime))
            {
                goldAdsBtn.GetComponent<Button>().interactable = true;
                goldAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{GoldOpenCount} / {GoldOpenMaxCount}";
            }
            else
            {
                goldAdsBtn.GetComponent<Button>().interactable = false;
                var remainingTime = TimeSpan.FromMinutes(GoldRewardCoolTime) - GoldPassed;
                if (GoldOpenCount == GoldOpenMaxCount)
                {
                    var resetTime = StoreMenu.Instance.LastDayCheck.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
                    goldAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{resetTime}";
                }
                else if (remainingTime > TimeSpan.Zero)
                {
                    var remainingTimeText = remainingTime.ToString(@"mm\:ss");
                    goldAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = remainingTimeText;
                }
            }
            goldGemBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = "900";
        }

        public static void BronzeAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_greenbox");
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.BronzeAds;
        }

        public static void SilverAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_bluebox");
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.SilverAds;
        }

        public static void GoldAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_goldbox");
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.GoldAds;
        }
    }
}

