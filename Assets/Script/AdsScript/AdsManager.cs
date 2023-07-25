using UnityEngine;
using UnityEngine.UI;

namespace Script.AdsScript
{
    public class AdsManager : MonoBehaviour
    {
        [SerializeField] private GameObject coinBtn;
        [SerializeField] private GameObject staminaBtn;
        [SerializeField] private GameObject gemBtn;

        private void Awake()
        {
            coinBtn.GetComponent<Button>().onClick.AddListener(Coin);
            staminaBtn.GetComponent<Button>().onClick.AddListener(Stamina);
            gemBtn.GetComponent<Button>().onClick.AddListener(Gem);
        }

        public void Coin()
        {
            AppLovinScript.ShowRewardedAd();
            RewardManager.Instance.RewardButtonClicked("Coin");
        }

        public void Stamina()
        {
            AppLovinScript.ShowRewardedAd();
            RewardManager.Instance.RewardButtonClicked("Stamina");
        }

        public void Gem()
        {
            AppLovinScript.ShowRewardedAd();
            RewardManager.Instance.RewardButtonClicked("Gem");
        }
    }
}