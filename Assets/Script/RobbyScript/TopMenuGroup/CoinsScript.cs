using Script.RewardScript;
using TMPro;
using UnityEngine;

namespace Script.RobbyScript.TopMenuGroup
{
    public class CoinsScript : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;
        public static CoinsScript Instance;
        private const string CoinKey = "Coins";

        public int Coin
        {
            get => PlayerPrefs.GetInt(CoinKey, 0);
            set
            {
                PlayerPrefs.SetInt(CoinKey, value);
                UpdateCoin();
            }
        }

        public void Awake()
        {
            if (Instance == null)
            { 
                Instance = this;
            }
            else 
            { 
                Destroy(gameObject);
            }
            UpdateCoin();
        }

        public void UpdateCoin()
        {
            var coinValue = Coin switch
            {
                >= 1000000 => Coin / 1000000 + "M",
                >= 1000 => Coin / 1000 + "k",
                _ => Coin.ToString()
            };
            coinText.text = $"{coinValue}";
        }
    }
}
