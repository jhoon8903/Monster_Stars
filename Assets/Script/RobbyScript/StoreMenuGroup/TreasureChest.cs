using System;
using System.Collections;
using System.Collections.Generic;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.RobbyScript.TopMenuGroup;
using TMPro;
using UnityEngine;
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
        public Sprite chestAdsBtnSprite;
        public Sprite chestGemBtnSprite;
        public TextMeshProUGUI bronzeGemBtnText;
        public TextMeshProUGUI silverGemBtnText;
        public TextMeshProUGUI goldGemBtnText;

        private const int BronzeGemText = 150;
        private const int SilverGemText = 450;
        private const int GoldGemText = 900;

        public void ChestBtnSet()
        {
            bronzeAdsBtn.GetComponent<Button>().onClick.AddListener(() => StoreMenu.Instance.ChestCheckClick(StoreMenu.ButtonType.BronzeAds));
            silverAdsBtn.GetComponent<Button>().onClick.AddListener(() => StoreMenu.Instance.ChestCheckClick(StoreMenu.ButtonType.SilverAds));
            goldAdsBtn.GetComponent<Button>().onClick.AddListener(() => StoreMenu.Instance.ChestCheckClick(StoreMenu.ButtonType.GoldAds));
            bronzeGemBtn.GetComponent<Button>().onClick.AddListener(() => StoreMenu.Instance.ChestCheckClick(StoreMenu.ButtonType.BronzeGem));
            silverGemBtn.GetComponent<Button>().onClick.AddListener(() => StoreMenu.Instance.ChestCheckClick(StoreMenu.ButtonType.SilverGem));
            goldGemBtn.GetComponent<Button>().onClick.AddListener(() => StoreMenu.Instance.ChestCheckClick(StoreMenu.ButtonType.GoldGem));
        }

        public void UpdateBtnColor()
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
    }
}

