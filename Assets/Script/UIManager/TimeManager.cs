using UnityEngine;
using System;
using Script.QuestGroup;
using Script.RobbyScript.StoreMenuGroup;

namespace Script.UIManager
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }
        private const string LastSavedDateKey = "LastSavedDate";
        public DateTime LastDate;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            var lastSavedDateStr = PlayerPrefs.GetString(LastSavedDateKey, "");

            if (string.IsNullOrEmpty(lastSavedDateStr))
            {
                SaveTodayAsLastSavedDate();
            }
            else
            {
                LastDate = DateTime.Parse(lastSavedDateStr);
                var today = DateTime.Today;
                if (LastDate >= today) return;
                Reset();
                SaveTodayAsLastSavedDate();
            }
        }

        private void Update()
        {
            LeftNextDay();
            var today = DateTime.Today;
            if (LastDate >= today) return;
            Reset();
            SaveTodayAsLastSavedDate();
        }
        private static void SaveTodayAsLastSavedDate()
        {
            PlayerPrefs.SetString(LastSavedDateKey, DateTime.Today.ToString("yyyy-MM-dd"));
            PlayerPrefs.Save();
        }

        private void Reset()
        {
            StoreMenu.Instance.ResetButtonCounts();
            QuestManager.Instance.ResetQuest();
            LastDate= DateTime.Today;
            PlayerPrefs.SetString(LastSavedDateKey, LastDate.ToBinary().ToString());
            PlayerPrefs.Save();
        }

        private void LeftNextDay()
        {
            QuestTimer();
        }

        private void QuestTimer()
        {
            var resetTime = LastDate.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
            QuestManager.Instance.timer.text = $"Ends in : {resetTime}";
        }
    }
}