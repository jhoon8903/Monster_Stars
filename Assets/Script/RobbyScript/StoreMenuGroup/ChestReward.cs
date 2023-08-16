using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.StoreMenuGroup
{
    public class ChestReward : MonoBehaviour
    {
        [SerializeField] private Transform chestCheckContents;
        [SerializeField] private Sprite coinSprite;
        [SerializeField] private Sprite greenSprite;
        [SerializeField] private Sprite blueSprite;
        [SerializeField] private Sprite purpleSprite;
        private readonly Dictionary<StoreMenu.ButtonType, Dictionary<Sprite, int>> _chestReward = new Dictionary<StoreMenu.ButtonType, Dictionary<Sprite, int>>();
        private readonly List<ChestItem> _createdChests = new List<ChestItem>(); // Keep track of created chests
        [SerializeField] private ChestItem chestPrefabs;
        public static ChestReward Instance { get; private set; }
        
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
                    var bronzeGemSprites = new Dictionary<Sprite, int>
                    {
                        { coinSprite, 500 },
                        { greenSprite, 24 },
                        { blueSprite, 0 },
                        { purpleSprite, 0 },
                    };
                    _chestReward.Add(StoreMenu.ButtonType.BronzeGem, bronzeGemSprites);
                    break;
                case StoreMenu.ButtonType.BronzeAds:
                    var bronzeAdsSprites = new Dictionary<Sprite, int>
                    {
                        { coinSprite, 500 },
                        { greenSprite, 24 },
                        { blueSprite, 0 },
                        { purpleSprite, 0 },
                    };
                    _chestReward.Add(StoreMenu.ButtonType.BronzeAds, bronzeAdsSprites);
                    break;
                case StoreMenu.ButtonType.SilverGem:
                    var silverGemSprites = new Dictionary<Sprite, int>
                    {
                        { coinSprite, 25000 },
                        { greenSprite, 180 },
                        { blueSprite, 114 },
                        { purpleSprite, 5 },
                    };
                    _chestReward.Add(StoreMenu.ButtonType.SilverGem, silverGemSprites);
                    break;
                case StoreMenu.ButtonType.SilverAds:
                    var silverAdsSprites = new Dictionary<Sprite, int>
                    {   
                        { coinSprite, CalculateSilverAdsCoinReward(StoreMenu.Instance.SilverAdsOpen) },
                        { greenSprite, 180 },
                        { blueSprite, 24 + 15 * (StoreMenu.Instance.SilverAdsOpen - 1) },
                        { purpleSprite, StoreMenu.Instance.SilverAdsOpen > 2 ? StoreMenu.Instance.SilverAdsOpen - 2 : 0 },
                    };
                    _chestReward.Add(StoreMenu.ButtonType.SilverAds, silverAdsSprites);
                    break;
                case StoreMenu.ButtonType.GoldGem:
                    var goldGemSprites = new Dictionary<Sprite, int>
                    {
                        { coinSprite, 60000 },
                        { greenSprite, 480 },
                        { blueSprite, 285 },
                        { purpleSprite, 14 },
                    };
                    _chestReward.Add(StoreMenu.ButtonType.GoldGem, goldGemSprites);
                    break;
                case StoreMenu.ButtonType.GoldAds:
                    var goldAdsSprites = new Dictionary<Sprite, int>
                    {
                        { coinSprite, CalculateGoldAdsCoinReward(StoreMenu.Instance.GoldAdsOpen) },
                        { greenSprite, 480 },
                        { blueSprite, 165 + 40 * (StoreMenu.Instance.GoldAdsOpen - 1) },
                        { purpleSprite, StoreMenu.Instance.GoldAdsOpen > 2 ? 6 + (StoreMenu.Instance.GoldAdsOpen - 1) * 2 : 6 },
                    };
                    _chestReward.Add(StoreMenu.ButtonType.GoldAds, goldAdsSprites);
                    break;
            }
            SetPrefabs(chestType);
        }
        
        private static int CalculateSilverAdsCoinReward(int openCount)
        {
            int[] coinRewards = { 2000, 3000, 5000, 7000, 10000, 15000, 25000 };

            if (openCount >= 0 && openCount < coinRewards.Length)
            {
                return coinRewards[openCount];
            }
            return 0; // 혹시나 범위 밖의 값이 들어온 경우 기본값
        }
        
        private static int CalculateGoldAdsCoinReward(int openCount)
        {
            int[] coinRewards = { 5000, 15000, 30000, 45000, 60000 };

            if (openCount >= 0 && openCount < coinRewards.Length)
            {
                return coinRewards[openCount];
            }
            return 0; // 혹시나 범위 밖의 값이 들어온 경우 기본값
        }

        public void ClearChests()
        {
            // Destroy all created chests
            foreach (var chest in _createdChests)
            {
                Destroy(chest.gameObject);
            }
            _createdChests.Clear();

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
                    var chestRewardList = Instantiate(chestPrefabs, chestCheckContents.position, Quaternion.identity);
                    Transform transform1;
                    (transform1 = chestRewardList.transform).SetParent(chestCheckContents);
                    transform1.localScale = new Vector3(1, 1, 1);
                    _createdChests.Add(chestRewardList); // Add created chest to the list

                    // ChestGrade with specific sprite index
                    ChestGrade(chestRewardList, chestType, i);
                }
            }
            else
            {
                Debug.Log("Unknown box grade!");
            }
        }


        private void ChestGrade(ChestItem chestRewardList, StoreMenu.ButtonType chestType, int spriteIndex)
        {
            if (_chestReward.TryGetValue(chestType, out var spriteDict))
            {
                var sprites = spriteDict.Keys.ToList();

                if (spriteIndex < sprites.Count)
                {
                    var sprite = sprites[spriteIndex];
                    var quantity = spriteDict[sprite];

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


