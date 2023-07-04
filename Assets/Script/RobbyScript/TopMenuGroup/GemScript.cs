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

        public void UpdateGem()
        {
            var gemValue = Gem switch
            {
              >=100000 => Gem / 1000000 +"M",
              >=1000 => Gem / 1000 + "k",
              _=> Gem.ToString()
            };
            gemText.text = $"{gemValue}";
        }
    }
}
