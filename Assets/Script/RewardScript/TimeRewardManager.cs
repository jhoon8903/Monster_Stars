using System;
using Script.RobbyScript.MainMenuGroup;
using Script.RobbyScript.TopMenuGroup;
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
        private const string LastOpenTimeKey = "LastOpenTime";
        private const string LatestClearedStageKey = "LatestClearedStage";
        private DateTime lastOpenTime;
        private int latestClearedStage;
        private void Awake()
        {
            Instance = this;
            _rewardLevelText = timeRewardLevel.GetComponentInChildren<TextMeshProUGUI>();
            _rewardTimeText = timeRewardTime.GetComponentInChildren<TextMeshProUGUI>();
            // LoadTimeReward();
        }

        public void OpenPanel()
        {
            timeRewardPanel.SetActive(true);
            ApplyTimeReward();
            SaveTimeReward();
            UpdateReward();
        }

        private void ApplyTimeReward()
        {
            var timePassed = DateTime.Now - lastOpenTime;
            var goldReward = CalculateGoldReward(timePassed, latestClearedStage);
            // int greenShardReward = CalculateGreenShardReward(timePassed, latestClearedStage);
            // var blueShardReward = latestClearedStage >= 10 ? CalculateBlueShardReward(timePassed, latestClearedStage) : 0;
            CoinsScript.Instance.Coin += goldReward;
            CoinsScript.Instance.UpdateCoin();
        }

        private int CalculateGoldReward(TimeSpan timePassed, int stage)
        {
            return (int)(stage * 10 * timePassed.TotalMinutes / 10);
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

        private static void SaveTimeReward()
        {
            PlayerPrefs.SetString(LastOpenTimeKey, DateTime.Now.ToBinary().ToString());
            PlayerPrefs.SetInt(LatestClearedStageKey, MainPanel.Instance.LatestStage);
            PlayerPrefs.Save();
        }

        private void LoadTimeReward()
        {
            var lastOpenTimeStr = PlayerPrefs.GetString(LastOpenTimeKey, null);
            lastOpenTime = lastOpenTimeStr != null ? DateTime.FromBinary(Convert.ToInt64(lastOpenTimeStr)) : DateTime.Now;
            latestClearedStage = PlayerPrefs.GetInt(LatestClearedStageKey, 0);
        }
    }   
}