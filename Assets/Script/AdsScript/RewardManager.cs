using Script.RobbyScript.TopMenuGroup;
using Script.UIManager;
using UnityEngine;
using UnityEngine.UI;

namespace Script.AdsScript
{
    public class RewardManager : MonoBehaviour
    {

        [SerializeField] private Text rewardText;
        [SerializeField] private GameObject rewardBtn;
        public static RewardManager Instance { get; private set; }
        public bool isRetry;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public void RewardButtonClicked(string buttonType)
        {
            switch (buttonType)
            {
                case "Coin":
                    rewardText.text = " Coin 5 Get";
                    rewardBtn.GetComponent<Button>().onClick.AddListener(GiveCoinReward);
                    break;
                case "Gem":
                    rewardText.text = " Gem 5 Get";
                    rewardBtn.GetComponent<Button>().onClick.AddListener(GiveGemReward);
                    break;
                case "Stamina":
                    rewardText.text = " Stamina 5 Get";
                    rewardBtn.GetComponent<Button>().onClick.AddListener(GiveStaminaReward);
                    break;
                case "Retry":
                    rewardText.text = " Retry ";
                    MaxSdkCallbacks.Instance.isRetry = true;
                    rewardBtn.GetComponent<Button>().onClick.AddListener(Retry);
                    break;
            }
        }

        private void GiveCoinReward()
        {
            Debug.Log("코인 보상을 제공합니다.");
            if (CoinsScript.Instance != null)
            {
                // Access the Coin property and add the specified amount
                CoinsScript.Instance.Coin += 1000;
            }
            rewardBtn.GetComponent<Button>().onClick.RemoveListener(GiveCoinReward);
        }

        private void GiveGemReward()
        {
            Debug.Log("재화 보상을 제공합니다.");
            if (GemScript.Instance != null)
            {
                GemScript.Instance.Gem += 100;
            }
            rewardBtn.GetComponent<Button>().onClick.RemoveListener(GiveGemReward);

        }

        private void GiveStaminaReward()
        {
            Debug.Log("스테미너 보상을 제공합니다.");
            StaminaScript.Instance.CurrentStamina += 5;
            rewardBtn.GetComponent<Button>().onClick.RemoveListener(GiveStaminaReward);
        }

        private void Retry()
        {
            rewardBtn.GetComponent<Button>().onClick.RemoveListener(Retry);
        }
    }
}

