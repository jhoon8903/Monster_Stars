using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Script.RobbyScript.StoreMenuGroup
{

    public class ChestCheck : MonoBehaviour
    {
        public struct ChestInfo
        {
            public readonly string Name;
            public readonly Sprite ChestSprite;
            public readonly Sprite ButtonSprite;
            public readonly string ButtonText;
            public readonly UnityAction Click;

            public ChestInfo(string name, Sprite chestSprite, Sprite buttonSprite, string buttonText, UnityAction onClick)
            {
                Name = name;
                ChestSprite = chestSprite;
                ButtonSprite = buttonSprite;
                ButtonText = buttonText;
                Click = onClick;
            }
        }

        [SerializeField] private GameObject chestCheckCloseBtn;
        [SerializeField] private TreasureChest treasureChest;
        public GameObject chestCheckPanel;
        public TextMeshProUGUI chestCheckTopText;
        public TextMeshProUGUI chestCheckBtnText;
        public GameObject chestImage;
        public GameObject chestCheckBtn;
        public static ChestCheck Instance { get; private set; }
        
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
            chestCheckCloseBtn.GetComponent<Button>().onClick.AddListener(CloseChestCheck);
        }

        private void OpenPanel()
        {
            chestCheckPanel.SetActive(true);
        }

        public void CloseChestCheck()
        {
            chestCheckPanel.SetActive(false);
            StoreMenu.Instance.DeleteEvent();
        }

        public void ChestCheckClick(StoreMenu.ButtonType buttonType)
        {
            OpenPanel();
            var chestInfo = new ChestInfo();

            switch (buttonType)
            {
                case StoreMenu.ButtonType.BronzeAds:
                    chestInfo = new ChestInfo("Bronze Chest", treasureChest.TreasureInstance.bronzeSprite, treasureChest.TreasureInstance.adsBtn, $"{treasureChest.BronzeOpenCount} / {TreasureChest.BronzeOpenMaxCount}", TreasureChest.BronzeAds);
                    break;
                case StoreMenu.ButtonType.SilverAds:
                    chestInfo = new ChestInfo("Silver Chest", treasureChest.TreasureInstance.silverSprite, treasureChest.TreasureInstance.adsBtn, $"{treasureChest.SilverOpenCount} / {TreasureChest.SilverOpenMaxCount}", TreasureChest.SilverAds);
                    break;
                case StoreMenu.ButtonType.GoldAds:
                    chestInfo = new ChestInfo("Gold Chest", treasureChest.TreasureInstance.goldSprite, treasureChest.TreasureInstance.adsBtn, $"{treasureChest.GoldOpenCount} / {TreasureChest.GoldOpenMaxCount}", TreasureChest.GoldAds);
                    break;
                case StoreMenu.ButtonType.BronzeGem:
                    chestInfo = new ChestInfo("Bronze Chest", treasureChest.TreasureInstance.bronzeSprite, treasureChest.TreasureInstance.gemBtn, "150", () => StoreMenu.Instance.CheckAndSummonChest(StoreMenu.ButtonType.BronzeGem));
                    break;
                case StoreMenu.ButtonType.SilverGem:
                    chestInfo = new ChestInfo("Silver Chest", treasureChest.TreasureInstance.silverSprite, treasureChest.TreasureInstance.gemBtn, "450", () => StoreMenu.Instance.CheckAndSummonChest(StoreMenu.ButtonType.SilverGem));
                    break;
                case StoreMenu.ButtonType.GoldGem:
                    chestInfo = new ChestInfo("Gold Chest", treasureChest.TreasureInstance.goldSprite, treasureChest.TreasureInstance.gemBtn, "900", () => StoreMenu.Instance.CheckAndSummonChest(StoreMenu.ButtonType.GoldGem));
                    break;
            }
            chestCheckTopText.text = chestInfo.Name;
            chestImage.GetComponent<Image>().sprite = chestInfo.ChestSprite;
            chestCheckBtn.GetComponent<Image>().sprite = chestInfo.ButtonSprite;
            chestCheckBtnText.text = chestInfo.ButtonText;
            ChestReward.Instance.SetStart(buttonType);
            chestCheckBtn.GetComponent<Button>().onClick.AddListener(chestInfo.Click);
        }
    }
}