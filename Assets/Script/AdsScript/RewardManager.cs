using UnityEngine;
using UnityEngine.UI;

namespace Script.AdsScript
{
    public class RewardManager : MonoBehaviour
    {
        public static RewardManager Instance { get; private set; }

        [SerializeField] private Text rewardText;
        [SerializeField] private GameObject rewardBtn;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void RewardButtonClicked(string buttonType)
        {
            switch (buttonType)
            {
                case "Coin":
                    rewardBtn.GetComponent<Button>().onClick.AddListener(GiveCoinReward);
                    break;
                case "Gem":
                    rewardBtn.GetComponent<Button>().onClick.AddListener(GiveGemReward);
                    break;
                case "Stamina":
                    rewardBtn.GetComponent<Button>().onClick.AddListener(GiveStaminaReward);
                    break;
                default:
                    Debug.LogWarning("Unknown button type: " + buttonType);
                    break;
            }
        }

        private void GiveCoinReward()
        {
            // 코인 보상 제공 로직 추가
            Debug.Log("코인 보상을 제공합니다.");
        }

        private void GiveGemReward()
        {
            // 재화 보상 제공 로직 추가
            Debug.Log("재화 보상을 제공합니다.");
        }

        private void GiveStaminaReward()
        {
            // 스테미너 보상 제공 로직 추가
            Debug.Log("스테미너 보상을 제공합니다.");
        }
    }
}

