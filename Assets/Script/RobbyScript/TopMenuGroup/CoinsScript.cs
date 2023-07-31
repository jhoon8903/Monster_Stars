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
                var c and >= 1000000 => $"{c / 1000000f:F1}M",
                var c and >= 1000 => $"{c / 1000f:F1}K",
                _ => Coin.ToString()
            };
            coinText.text = coinValue;
        }

    }
}
