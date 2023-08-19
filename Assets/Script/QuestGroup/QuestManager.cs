using System;
using System.Collections.Generic;
using System.Linq;
using Script.AdsScript;
using Script.RobbyScript.TopMenuGroup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.QuestGroup
{ 
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private GameObject questPanel;
        [SerializeField] private Button questOpenBtn;
        [SerializeField] private Button questCloseBtn;
        [SerializeField] private GameObject questRewardPanel;
        [SerializeField] private QuestObject questObject;

        // Ads Quest
        [SerializeField] private TextMeshProUGUI adsDesc;
        [SerializeField] private Slider adsProgress;
        [SerializeField] private TextMeshProUGUI adsProgressText;
        [SerializeField] private Button adsRewardBtn;
        public int adsValue;
        public int adsGoal;
        
        // AllClear Quest
        [SerializeField] private TextMeshProUGUI allClearDesc;
        [SerializeField] private Slider allClearProgress;
        [SerializeField] private TextMeshProUGUI allClearProgressText;
        [SerializeField] private Button allClearRewardBtn;
        public int allClearValue;
        public int allClearGoal;

        public readonly List<QuestObject> FixQuestList = new List<QuestObject>();
        public readonly List<QuestObject> RotationQuestList = new List<QuestObject>();
        public enum QuestTypes { AllClear, UseCoin, GetCoin, OpenBox, GetPiece, KillEnemy, KillBoss, ViewAds, MatchCoin, Victory }
        public enum QuestCondition { Fix, Rotation }
        private const int MaxRotationQuests = 3;
        public const string QuestLoadKey = "SelectedRotationQuests";
        private const string FixQuestLoadKey = "FixQuestLoadKey";
        private const string RotationQuestLoadKey = "RotationQuestLoadKey";
        private string[] _csvData;
        private void Awake()
        {
            questOpenBtn.onClick.AddListener(OpenQuest);
            questCloseBtn.onClick.AddListener(CloseQuest);
            LoadQuests(QuestCondition.Fix);
            if (PlayerPrefs.HasKey(QuestLoadKey))
            {
                questObject.LoadSelectedQuestList();
            }
            else
            {
                LoadQuests(QuestCondition.Rotation);
            }
        }
        private void OpenQuest()
        {
            questPanel.SetActive(true);
        }
        private void CloseQuest()
        {
            questPanel.SetActive(false);
        }
        private void LoadQuests(QuestCondition targetCondition)
        {
            var csvFile = Resources.Load<TextAsset>("questData");
            var csvText = csvFile.text;
            _csvData = csvText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var rotationQuestData = targetCondition == QuestCondition.Rotation ? new List<string[]>() : null;
            for (var i = 1; i < _csvData.Length; i++)
            {
                var line = _csvData[i];
                if (string.IsNullOrEmpty(line)) continue;
                var data = line.Split(',');
                var questType = ParseQuestType(data[0]);
                var condition = ParseQuestCondition(data[1]);
                if (condition != targetCondition) continue;
                HandleQuest(targetCondition, data, questType, rotationQuestData);
            }
            HandleRotationQuest(targetCondition, rotationQuestData);
        }
        public void ResetQuest()
        {
            foreach (var quest in FixQuestList)
            {
                switch (quest.questType)
                {
                    case QuestTypes.AllClear:
                    case QuestTypes.ViewAds:
                    case QuestTypes.GetCoin:
                    case QuestTypes.UseCoin:
                        quest.questValue = 0;
                        PlayerPrefs.SetInt(quest.questKey + "_value", 0);
                        quest.questProgress.value = 0;
                        quest.questProgressText.text = $"0 / {quest.questGoal}";
                        quest.isCompleted = false;
                        PlayerPrefs.SetInt(quest.questType + "_isCompleted", 0);
                        break;
                }
            }
            foreach (var quest in RotationQuestList)
            {
                quest.questValue = 0;
                PlayerPrefs.SetInt(quest.questKey + "_value", 0);
                quest.questProgress.value = 0;
                quest.questProgressText.text = $"0 / {quest.questGoal}";
                quest.isCompleted = false;
                PlayerPrefs.SetInt(quest.questType + "_isCompleted", 0);
            }
            ShuffleQuest();
            PlayerPrefs.Save();
        }

        public static QuestTypes ParseQuestType(string type) => (QuestTypes)Enum.Parse(typeof(QuestTypes), type);
        public static QuestCondition ParseQuestCondition(string condition) => (QuestCondition)Enum.Parse(typeof(QuestCondition), condition);
        private void HandleQuest(QuestCondition targetCondition, string[] data, QuestTypes questType, ICollection<string[]> rotationQuestData)
        {
            if (targetCondition == QuestCondition.Rotation) rotationQuestData?.Add(data);
            else if (questType is QuestTypes.ViewAds or QuestTypes.AllClear) HandleSpecialQuests(questType, data);
            else CreateAndAddQuest(data);
        }
        private void CreateAndAddQuest(string[] data)
        {
            var questObject = this.questObject.CreateQuestFromData(data);
            questObject.receiveBtn.SetActive(true);
            QuestObject.SetQuestButtonStates(questObject);
            FixQuestList.Add(questObject);
        }
        private void HandleSpecialQuests(QuestTypes questType, IReadOnlyList<string> data)
        {
            var desc = data[2];
            var goal = int.Parse(data[4]);
            var questKey = data[3];
            var value = PlayerPrefs.GetInt(questKey + "_value", 0);
    
            switch (questType)
            {
                case QuestTypes.ViewAds:
                    SetSpecialQuest(adsDesc, adsProgress, adsProgressText, desc, goal, value);
                    break;
                case QuestTypes.AllClear:
                    SetSpecialQuest(allClearDesc, allClearProgress, allClearProgressText, desc, goal, value);
                    break;
            }
        }
        private static void SetSpecialQuest(TMP_Text descText, Slider progress, TMP_Text progressText, string desc, int goal, int value)
        {
            descText.text = desc;
            progress.maxValue = goal;
            progress.value = value;
            progressText.text = $"{value} / {goal}";
        }

 
        private void HandleRotationQuest(QuestCondition targetCondition, IReadOnlyCollection<string[]> rotationQuestData)
        {
            if (targetCondition != QuestCondition.Rotation) return;
            if (rotationQuestData == null) return;
            var selectedQuestData = rotationQuestData.OrderBy(_ => UnityEngine.Random.value).Take(MaxRotationQuests).ToList();
            foreach (var data in selectedQuestData)CreateAndAddRotationQuest(data);
        }
        private void CreateAndAddRotationQuest(string[] data)
        {
            var rotationQuestObject = questObject.CreateQuestFromData(data);
            rotationQuestObject.receiveBtn.SetActive(false);
            rotationQuestObject.shuffleBtn.SetActive(true);
            rotationQuestObject.shuffleBtn.GetComponent<Button>().onClick.AddListener(CallShuffleAds);
            RotationQuestList.Add(rotationQuestObject);
            SaveSelectedQuestList(RotationQuestList);
        }
        private static void SaveSelectedQuestList(List<QuestObject> selectedQuests)
        {
            var selectedQuestKeys = string.Join(",", selectedQuests.Select(q => q.questType + ":" + q.questKey));
            PlayerPrefs.SetString(QuestLoadKey, selectedQuestKeys);
            foreach (var quest in selectedQuests)
            {
                var key = quest.questKey;
                PlayerPrefs.SetString(quest.questType + "_desc", quest.questDesc.text);
                PlayerPrefs.SetInt(key + "_value", quest.questValue);
                PlayerPrefs.SetInt(key + "_goal", quest.questGoal); 
                PlayerPrefs.SetString(quest.item1Value + "_item1Value", quest.item1Value.text);
                PlayerPrefs.SetString(quest.item2Value + "_item2Value", quest.item2Value.text);
                PlayerPrefs.SetString(quest.item3Value + "_item3Value", quest.item3Value.text);
                PlayerPrefs.SetString(quest.item4Value + "_item4Value", quest.item4Value.text);
                PlayerPrefs.SetInt(quest.questType + "_isShuffled", quest.isShuffled ? 1 : 0);
                PlayerPrefs.SetInt(quest.questType + "_isCompleted", quest.isCompleted ? 1 : 0);
                PlayerPrefs.SetInt(quest.isReceived + "_isReceived", quest.isReceived ? 1 : 0);
            }
            PlayerPrefs.Save();
        }
        public void UpdateQuest(QuestObject quest, int value)
        {
            if (FixQuestList.Contains(quest) || RotationQuestList.Contains(quest))
            {
                quest.questValue += value;

                if (quest.questValue >= quest.questGoal)
                {
                    quest.isCompleted = true;
                    PlayerPrefs.SetInt(quest.questType + "_isCompleted", 1);
                }

                if (quest.questType == QuestTypes.AllClear)
                {
                    PlayerPrefs.SetInt(quest.questKey + "_value", quest.questValue);
                    allClearProgress.maxValue = allClearGoal;
                    allClearProgress.value = quest.questValue;
                    allClearProgressText.text = $"{quest.questValue} / {allClearGoal}";
                    PlayerPrefs.Save();
                }
                else if (quest.questType == QuestTypes.ViewAds)
                {
                    PlayerPrefs.SetInt(quest.questKey + "_value", quest.questValue);
                    adsProgress.maxValue = adsGoal;
                    adsProgress.value = quest.questValue;
                    adsProgressText.text = $"{quest.questValue} / {adsGoal}";
                    PlayerPrefs.Save();
                }
                if (FixQuestList.Contains(quest))
                {
                    SaveFixQuestList(FixQuestList);
                }
                else if (RotationQuestList.Contains(quest))
                {
                    SaveRotationQuestList(RotationQuestList);
                }
            }
        }
        private static void SaveRotationQuestList(List<QuestObject> rotationQuests)
        {
            PlayerPrefs.SetString(RotationQuestLoadKey, string.Join(",", rotationQuests.Select(q => q.questType.ToString())));
            SaveQuests(rotationQuests);
            PlayerPrefs.Save();
        }
        private static void SaveFixQuestList(List<QuestObject> fixQuests)
        {
            PlayerPrefs.SetString(FixQuestLoadKey, string.Join(",", fixQuests.Select(q => q.questType.ToString())));
            SaveQuests(fixQuests);
            PlayerPrefs.Save();
        }
        private static void SaveQuests(List<QuestObject> quests)
        {
            foreach (var quest in quests)
            {
                var key = quest.questKey;
                PlayerPrefs.SetString(quest.questType + "_desc", quest.questDesc.text);
                PlayerPrefs.SetInt(key + "_value", quest.questValue);
                PlayerPrefs.SetInt(key + "_goal", quest.questGoal); 
                PlayerPrefs.SetString(quest.item1Value + "_item1Value", quest.item1Value.text);
                PlayerPrefs.SetString(quest.item2Value + "_item2Value", quest.item2Value.text);
                PlayerPrefs.SetString(quest.item3Value + "_item3Value", quest.item3Value.text);
                PlayerPrefs.SetString(quest.item4Value + "_item4Value", quest.item4Value.text);
                PlayerPrefs.SetInt(quest.questType + "_isShuffled", quest.isShuffled ? 1 : 0);
                PlayerPrefs.SetInt(quest.questType + "_isCompleted", quest.isCompleted ? 1 : 0);
                PlayerPrefs.SetInt(quest.questType + "_isReceived", quest.isReceived ? 1 : 0);
            }
        }
        public void ShuffleQuest()
        {
            var activeRotationQuestTypes = RotationQuestList.Select(q => q.questType).ToList();
            var csvFile = Resources.Load<TextAsset>("questData");
            var csvText = csvFile.text;
            var csvData = csvText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var inactiveRotationQuestData = new List<string[]>();

            for (var i = 1; i < csvData.Length; i++)
            {
                var line = csvData[i];
                if (string.IsNullOrEmpty(line)) continue;
                var data = line.Split(',');
                var questType = ParseQuestType(data[0]);
                var condition = ParseQuestCondition(data[1]);
                if (condition != QuestCondition.Rotation || activeRotationQuestTypes.Contains(questType)) continue;
                if (PlayerPrefs.GetInt(questType + "_isCompleted", 0) == 0)
                {
                    inactiveRotationQuestData.Add(data);
                }
            }

            var newQuestData = inactiveRotationQuestData.OrderBy(_ => UnityEngine.Random.value).FirstOrDefault();

            if (newQuestData == null) return;
            var newQuest = questObject.CreateQuestFromData(newQuestData);
            var questToRemove = RotationQuestList.FirstOrDefault();
            if (questToRemove != null)
            {
                RotationQuestList.Remove(questToRemove);
                Destroy(questToRemove.gameObject);
            }
            RotationQuestList.Add(newQuest);
            QuestObject.SetQuestButtonStates(newQuest);
            SaveSelectedQuestList(RotationQuestList);
        }
        public static void CallShuffleAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.ShuffleQuest;
        }
        public void ReceiveQuestReward(QuestObject quest)
        {
            questRewardPanel.SetActive(true);
            switch (quest.questType)
            {
                case QuestTypes.ViewAds:
                    CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
                    break;
                case QuestTypes.AllClear:
                    CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
                    break;
                case QuestTypes.UseCoin:
                    CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
                    break;
                case QuestTypes.GetCoin:
                    CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
                    break;
                case QuestTypes.OpenBox:
                    CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
                    break;
                case QuestTypes.GetPiece:
                    CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
                    break;
                case QuestTypes.KillEnemy:
                    CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
                    break;
                case QuestTypes.KillBoss:
                    CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
                    break;
                case QuestTypes.MatchCoin:
                    CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
                    break;
                case QuestTypes.Victory:
                    CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
                    break;
            }
           Quest.Instance.AllClearQuest();
           quest.isReceived = true;
           PlayerPrefs.SetInt(quest.questType + "isReceived", 1);
           quest.receiveBtn.GetComponent<Button>().interactable = false;
           quest.receiveBtnText.text = "Completed";
           if (FixQuestList.Contains(quest))
           {
               SaveFixQuestList(FixQuestList);
           }
           else if (RotationQuestList.Contains(quest))
           {
               SaveRotationQuestList(RotationQuestList);
           }
           PlayerPrefs.Save();
        }
    }   
}
