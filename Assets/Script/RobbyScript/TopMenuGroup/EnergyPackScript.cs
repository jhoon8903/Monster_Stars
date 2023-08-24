using Script.AdsScript;
using Script.UIManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.TopMenuGroup
{
    public class EnergyPackScript : MonoBehaviour
    {
        [SerializeField] private GameObject packPanel;
        [SerializeField] private TextMeshProUGUI packTitle;
        [SerializeField] private GameObject stamina;
        [SerializeField] private GameObject gem;
        [SerializeField] private GameObject coin;
        [SerializeField] private GameObject getFree;
        [SerializeField] private GameObject buyToGem;
        [SerializeField] private GameObject item1;
        [SerializeField] private TextMeshProUGUI item1Value;
        [SerializeField] private GameObject item2;
        [SerializeField] private TextMeshProUGUI item2Value;
        [SerializeField] private TextMeshProUGUI item2Price;
        [SerializeField] private Sprite item2Purchase;
        [SerializeField] public Sprite staminaSprite;
        [SerializeField] public Sprite gemSprite;
        [SerializeField] public Sprite coinSprite;
        private TextMeshProUGUI _title;

        private enum RewardTypes { Stamina, Gem, Coin }
        private RewardTypes _rewardTypes;

        private void Awake()
        {
            stamina.GetComponent<Button>().onClick.AddListener(()=>
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.popupOpen);
                OpenEnergyPack();
            });
            gem.GetComponent<Button>().onClick.AddListener(()=>
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.popupOpen);
                OpenGemPack();
            });
            coin.GetComponent<Button>().onClick.AddListener(()=>
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.popupOpen);
                OpenCoinPack();
            });
            _title = packTitle.GetComponent<TextMeshProUGUI>();
            getFree.GetComponent<Button>().onClick.AddListener(CheckRewardType);
            buyToGem.GetComponent<Button>().onClick.AddListener(BuyToGem);
            packPanel.SetActive(false);
        }

        private void OpenEnergyPack()
        {
            packPanel.SetActive(true);
            _title.text = "Energy Pack";
            _rewardTypes = RewardTypes.Stamina;
            
            item1.GetComponent<Image>().sprite = staminaSprite;
            item1Value.text = "10";
            
            item2.GetComponent<Image>().sprite = staminaSprite;
            item2Value.text = "10";
            item2Price.text = "50";
        }

        private void OpenGemPack()
        {
            packPanel.SetActive(true);
            _title.text = "Gem Pack";
            _rewardTypes = RewardTypes.Gem;

            item1.GetComponent<Image>().sprite= gemSprite;
            item1Value.text = "200";
            
            item2.GetComponent<Image>().sprite = gemSprite;
            item2Value.text = "1000";
            item2Purchase = coinSprite;
            item2Price.text = "50";
        }

        private void OpenCoinPack()
        {
            packPanel.SetActive(true);
            _title.text = "Coin Pack";
            _rewardTypes = RewardTypes.Coin;

            item1.GetComponent<Image>().sprite = coinSprite;
            item1Value.text = "1000";
            
            item1.GetComponent<Image>().sprite = coinSprite;
            item2Value.text = "10";
            item2Price.text = "50";
        }

        private void CheckRewardType()
        {
            switch (_rewardTypes)
            {
                case RewardTypes.Stamina:
                    AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Stamina;
                    break;
                case RewardTypes.Gem:
                    AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Gem;
                    break;
                case RewardTypes.Coin:
                    AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Coin;
                    break;
            }
            AdsManager.Instance.ShowRewardedAd();
            packPanel.SetActive(false);
        }

        private void BuyToGem()
        {
            switch (_rewardTypes)
            {
                case RewardTypes.Stamina:
                    GemScript.Instance.Gem -= 50;
                    StaminaScript.Instance.CurrentStamina += 10;
                    break;
                case RewardTypes.Coin:
                    GemScript.Instance.Gem -= 50;
                    CoinsScript.Instance.Coin += 1000;
                    break;
            }
        }
    }
}
