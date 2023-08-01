using System;
using Script.RewardScript;
using Script.RobbyScript.StoreMenuGroup;
using Script.RobbyScript.TopMenuGroup;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script.AdsScript
{
    public class RewardManager : MonoBehaviour
    {
        [SerializeField] private Text rewardText;
        [SerializeField] private GameObject rewardBtn;
        public static RewardManager Instance { get; private set; }
        private StoreMenu.BoxGrade _boxGrade;
        private Action _currentRewardAction;
        private bool _isRetry;

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
            var rewardClick = rewardBtn.GetComponent<Button>().onClick;

            if (_currentRewardAction != null)
            {
                rewardClick.RemoveListener(_currentRewardAction.Invoke);
                _currentRewardAction = null;
            }
            switch (buttonType)
            {
                case "Coin":
                    rewardText.text = " Coin 5 Get";
                    rewardClick.AddListener(GiveCoinReward);
                    break;
                case "Gem":
                    rewardText.text = " Gem 5 Get";
                    rewardClick.AddListener(GiveGemReward);
                    break;
                case "Stamina":
                    rewardText.text = " Stamina 5 Get";
                    rewardClick.AddListener(GiveStaminaReward);
                    break;
                case "Retry":
                    rewardText.text = " Retry ";
                    _isRetry = true;
                    rewardClick.AddListener(Retry);
                    break;
                case "Green":
                    rewardText.text = " GreenBox ";
                    _boxGrade = StoreMenu.BoxGrade.Green;
                    _currentRewardAction = () => StoreMenu.Instance.Reward(_boxGrade);
                    rewardClick.AddListener(_currentRewardAction.Invoke);
                    break;
                case "Blue":
                    rewardText.text = " BlueBox ";
                    _boxGrade = StoreMenu.BoxGrade.Blue;
                    _currentRewardAction = () => StoreMenu.Instance.Reward(_boxGrade);
                    rewardClick.AddListener(_currentRewardAction.Invoke);
                    break;
                case "Purple":
                    rewardText.text = " PurpleBox ";
                    _boxGrade = StoreMenu.BoxGrade.Purple;
                    _currentRewardAction = () => StoreMenu.Instance.Reward(_boxGrade);
                    rewardClick.AddListener(_currentRewardAction.Invoke);
                    break;
                case "CommonShuffle":
                    rewardText.text = " CommonShuffle ";
                    rewardClick.AddListener(CommonShuffle);
                    break;
                case "ExpShuffle":
                    rewardText.text = " ExpShuffle ";
                    rewardClick.AddListener(ExpShuffle);
                    break;
            }
        }

        private void GiveCoinReward()
        {
            Debug.Log("코인 보상을 제공합니다.");
            if (CoinsScript.Instance != null)
            {
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
            if (!_isRetry) return;
            SceneManager.LoadScene("StageScene");
            _isRetry = false;
            rewardBtn.GetComponent<Button>().onClick.RemoveListener(Retry);
        }

        private void CommonShuffle()
        {
            CommonRewardManager.Instance.ReEnqueueTreasure();
            rewardBtn.GetComponent<Button>().onClick.RemoveListener(CommonShuffle);
        }

        private void ExpShuffle()
        {
            Debug.Log("레벨업 동작");
            LevelUpRewardManager.Instance.ReLevelUpReward();
            rewardBtn.GetComponent<Button>().onClick.RemoveListener(ExpShuffle);
        }
    }
}

