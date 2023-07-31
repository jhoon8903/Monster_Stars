using TMPro;
using UnityEngine;

namespace Script.RobbyScript.TopMenuGroup
{
    public class GemScript : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI gemText;
        public static GemScript Instance;
        private const string GemKey = "Gems";

        public int Gem
        {
            get => PlayerPrefs.GetInt(GemKey, 0);
            set
            {
                PlayerPrefs.SetInt(GemKey, value);
                UpdateGem();
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
            UpdateGem();
        }

        private void UpdateGem()
        {
            var gemValue = Gem switch
            {
                var g and >= 1000000 => $"{g / 1000000f:F1}M",
                var g and >= 1000 => $"{g / 1000f:F1}K",
              _=> Gem.ToString()
            };
            gemText.text = $"{gemValue}";
        }
    }
}
