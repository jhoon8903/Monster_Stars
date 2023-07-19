using UnityEngine;
using UnityEngine.SceneManagement;
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
                    rewardBtn.GetComponent<Button>().onClick.AddListener(Retry);
                    break;
                default:

                    break;
            }
        }

        private static void GiveCoinReward()
        {
            
            Debug.Log("코인 보상을 제공합니다.");
        }

        private static void GiveGemReward()
        {
           
            Debug.Log("재화 보상을 제공합니다.");
        }

        private static void GiveStaminaReward()
        {
            Debug.Log("스테미너 보상을 제공합니다.");
        }

        private static void Retry()
        {
            SceneManager.LoadScene("StageScene");
        }
    }
}

