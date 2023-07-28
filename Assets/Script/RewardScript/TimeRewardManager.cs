using System;
using Script.RobbyScript.MainMenuGroup;
using TMPro;
using UnityEngine;

namespace Script.RewardScript
{
    public class TimeRewardManager : MonoBehaviour
    {
        [SerializeField] private GameObject timeRewardPanel;
        [SerializeField] private GameObject timeRewardTop;
        [SerializeField] private GameObject timeRewardLevel;
        [SerializeField] private GameObject timeRewardTime;
        [SerializeField] private GameObject timeRewardContents;
        [SerializeField] private GameObject timeRewardBtn;
        [SerializeField] private Goods rewardItem;
        public static TimeRewardManager Instance { get; private set; }
        private TextMeshProUGUI _rewardLevelText;
        private TextMeshProUGUI _rewardTimeText;
        private int _hour;
        private int _min;
        private int _sec;
        private void Awake()
        {
            Instance = this;
            _rewardLevelText = timeRewardLevel.GetComponentInChildren<TextMeshProUGUI>();
            _rewardTimeText = timeRewardTime.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void OpenPanel()
        {
            timeRewardPanel.SetActive(true);
            LoadTimeReward();
            UpdateReward();
            
        }

        private void OnApplicationQuit()
        {
            SaveTimeReward();
        }
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveTimeReward();
            }
            else
            {
                LoadTimeReward();
            }
        }

        private void UpdateReward()
        {
            _rewardLevelText.text = $"보상레벨 [{MainPanel.Instance.LatestStage} Lv]";
            _rewardTimeText.text = $"보상누적 [{_hour}:{_min}:{_sec}]";
        }

        private void SaveTimeReward()
        {

        }

        private void LoadTimeReward()
        {

        }
    }   
}