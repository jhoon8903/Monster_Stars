using UnityEngine;
using UnityEngine.UI;

namespace Script.AdsScript
{
    public class AdsManager : MonoBehaviour
    {
 
        [SerializeField] private GameObject CoinBtn;
        [SerializeField] private GameObject StaminaBtn;
        [SerializeField] private GameObject GemBtn;


        private void Awake()
        {
            CoinBtn.GetComponent<Button>().onClick.AddListener(Coin);

            StaminaBtn.GetComponent<Button>().onClick.AddListener(Stamina);

            GemBtn.GetComponent<Button>().onClick.AddListener(Gem);
        }

        private void Coin()
        {
            AppLovinScript.Instance.ShowRewardedAd();
            RewardManager.Instance.RewardButtonClicked("Coin");
        }

        private void Stamina()
        {
            AppLovinScript.Instance.ShowRewardedAd();
            RewardManager.Instance.RewardButtonClicked("Stamina");
        }

        private void Gem()
        {
            AppLovinScript.Instance.ShowRewardedAd();
            RewardManager.Instance.RewardButtonClicked("Gem");
        }
    }
}