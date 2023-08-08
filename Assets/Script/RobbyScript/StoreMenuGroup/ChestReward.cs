using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.StoreMenuGroup
{
    public class ChestReward : MonoBehaviour
    {
        [SerializeField] private Transform chestCheckContents;
        [SerializeField] private Sprite coinsprite;
        [SerializeField] private Sprite greensprite;
        [SerializeField] private Sprite bluesprite;
        [SerializeField] private Sprite purplesprite;
        private readonly Dictionary<StoreMenu.ButtonType, Dictionary<Sprite, int>> _chestReward = new Dictionary<StoreMenu.ButtonType, Dictionary<Sprite, int>>();
        private List<ChestItem> createdChests = new List<ChestItem>(); // Keep track of created chests
        [SerializeField] private ChestItem chestPrefabs;
        public ChestItem newChestRewardList;
        public static ChestReward Instance { get; private set; }

        public int sliverAdsOpen;
        public int goldAdsOpen;
        
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
        }

        public void SetStart(StoreMenu.ButtonType chestType)
        {
            switch (chestType)
            {
                case StoreMenu.ButtonType.BronzeGem:
                    Dictionary<Sprite, int> bronzeGemSprites = new Dictionary<Sprite, int>
                    {
                        { coinsprite, 500 },
                        { greensprite, 24 },
                        { bluesprite, 0 },
                        { purplesprite, 0 },
                    };
                    _chestReward.Add(StoreMenu.ButtonType.BronzeGem, bronzeGemSprites);
                    break;
                case StoreMenu.ButtonType.BronzeAds:
                    Dictionary<Sprite, int> bronzeAdsSprites = new Dictionary<Sprite, int>
                    {
                        { coinsprite, 500 },
                        { greensprite, 24 },
                        { bluesprite, 0 },
                        { purplesprite, 0 },
                    };
                    _chestReward.Add(StoreMenu.ButtonType.BronzeAds, bronzeAdsSprites);
                    break;
                case StoreMenu.ButtonType.SilverGem:
                    Dictionary<Sprite, int> silverGemSprites = new Dictionary<Sprite, int>
                    {
                        { coinsprite, 25000 },
                        { greensprite, 180 },
                        { bluesprite, 114 },
                        { purplesprite, 5 },
                    };
                    _chestReward.Add(StoreMenu.ButtonType.SilverGem, silverGemSprites);
                    break;
                case StoreMenu.ButtonType.SilverAds:
                    Dictionary<Sprite, int> silverAdsSprites = new System.Collections.Generic.Dictionary<Sprite, int>
                    {   
                        { coinsprite, CalculateSilverAdsCoinReward(StoreMenu.Instance.SilverAdsOpen) },
                        { greensprite, 180 },
                        { bluesprite, 24 + 15 * (StoreMenu.Instance.SilverAdsOpen - 1) },
                        { purplesprite, StoreMenu.Instance.SilverAdsOpen > 2 ? StoreMenu.Instance.SilverAdsOpen - 2 : 0 },
                    };
                    _chestReward.Add(StoreMenu.ButtonType.SilverAds, silverAdsSprites);
                    break;
                case StoreMenu.ButtonType.GoldGem:
                    Dictionary<Sprite, int> goldGemSprites = new Dictionary<Sprite, int>
                    {
                        { coinsprite, 60000 },
                        { greensprite, 480 },
                        { bluesprite, 285 },
                        { purplesprite, 14 },
                    };
                    _chestReward.Add(StoreMenu.ButtonType.GoldGem, goldGemSprites);
                    break;
                case StoreMenu.ButtonType.GoldAds:
                    Dictionary<Sprite, int> goldAdsSprites = new Dictionary<Sprite, int>
                    {
                        { coinsprite, CalculateGoldAdsCoinReward(StoreMenu.Instance.GoldAdsOpen) },
                        { greensprite, 480 },
                        { bluesprite, 165 + 40 * (StoreMenu.Instance.GoldAdsOpen - 1) },
                        { purplesprite, StoreMenu.Instance.GoldAdsOpen > 2 ? 6 + (StoreMenu.Instance.GoldAdsOpen - 1) * 2 : 6 },
                    };
                    _chestReward.Add(StoreMenu.ButtonType.GoldAds, goldAdsSprites);
                    break;
            }
            SetPrefabs(chestType);
        }
        
        private int CalculateSilverAdsCoinReward(int openCount)
        {
            int[] coinRewards = { 2000, 3000, 5000, 7000, 10000, 15000, 25000 };

            if (openCount >= 0 && openCount < coinRewards.Length)
            {
                return coinRewards[openCount];
            }
            else
            {
                return 0; // 혹시나 범위 밖의 값이 들어온 경우 기본값
            }
        }
        
        private int CalculateGoldAdsCoinReward(int openCount)
        {
            int[] coinRewards = { 5000, 15000, 30000, 45000, 60000 };

            if (openCount >= 0 && openCount < coinRewards.Length)
            {
                return coinRewards[openCount];
            }
            else
            {
                return 0; // 혹시나 범위 밖의 값이 들어온 경우 기본값
            }
        }

        public void ClearChests()
        {
            // Destroy all created chests
            foreach (var chest in createdChests)
            {
                Destroy(chest.gameObject);
            }
            createdChests.Clear();

            // Clear the _chestReward dictionary
            _chestReward.Clear();
        }
        
        private void SetPrefabs(StoreMenu.ButtonType chestType)
        {
            if (_chestReward.TryGetValue(chestType, out var spriteDict))
            {
                var sprites = spriteDict.Keys.ToList();

                var numOfPrefabs = sprites.Count; // Create one prefab for each sprite
                for (var i = 0; i < numOfPrefabs; i++)
                {
                    ChestItem newChestRewardList = Instantiate(chestPrefabs, chestCheckContents.position, Quaternion.identity);
                    Transform transform1;
                    (transform1 = newChestRewardList.transform).SetParent(chestCheckContents);
                    transform1.localScale = new Vector3(1, 1, 1);
                    createdChests.Add(newChestRewardList); // Add created chest to the list

                    // ChestGrade with specific sprite index
                    ChestGrade(newChestRewardList, chestType, i);
                }
            }
            else
            {
                Debug.Log("Unknown box grade!");
            }
        }


        public void ChestGrade(ChestItem chestRewardList, StoreMenu.ButtonType chestType, int spriteIndex)
        {
            Dictionary<Sprite, int> spriteDict;

            if (_chestReward.TryGetValue(chestType, out spriteDict))
            {
                var sprites = spriteDict.Keys.ToList();

                if (spriteIndex < sprites.Count)
                {
                    Sprite sprite = sprites[spriteIndex];
                    int quantity = spriteDict[sprite];

                    chestRewardList.chestRewardItem.GetComponent<Image>().sprite = sprite;

                    if (quantity > 0)
                    {
                        chestRewardList.chestRewardText.text = quantity.ToString();
                        chestRewardList.chestRewardText.gameObject.SetActive(true);
                    }
                    else
                    {
                        chestRewardList.gameObject.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("No more sprites for this grade!");
                }
            }
            else
            {
                Debug.Log("Unknown box grade!");
            }
        }

    }
}


