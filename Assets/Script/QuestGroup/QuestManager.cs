using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Script.QuestGroup
{ 
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private GameObject questPanel;
        [SerializeField] private Button questOpenBtn;
        [SerializeField] private Button questCloseBtn;

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

        // Quest Prefabs
        [SerializeField] private QuestObject questPrefab;
        [SerializeField] private GameObject questTransform;

        private readonly List<QuestObject> _fixQuestList = new List<QuestObject>();
        private readonly List<QuestObject> _rotationQuestList = new List<QuestObject>();
        public enum QuestTypes { AllClear, UseCoin, GetCoin, OpenBox, GetPiece, KillEnemy, KillBoss, ViewAds, MatchCoin, Victory }
        public enum QuestCondition { Fix, Rotation }
        private const int MaxRotationQuests = 3; // 최대 무작위 회전 퀘스트 개수
        private const string QuestLoadKey = "SelectedRotationQuests";

        private void Awake()
        {
            questOpenBtn.onClick.AddListener(OpenQuest);
            questCloseBtn.onClick.AddListener(CloseQuest);
            LoadQuests(QuestCondition.Fix);
            if (PlayerPrefs.HasKey(QuestLoadKey))
            {
                LoadSelectedQuestList();
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
            var csvFile = Resources.Load<TextAsset>("QuestData");
            var csvText = csvFile.text;
            var csvData = csvText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var rotationQuestData = targetCondition == QuestCondition.Rotation ? new List<string[]>() : null;

            for (var i = 1; i < csvData.Length; i++)
            {
                var line = csvData[i];
                if (string.IsNullOrEmpty(line)) continue;
                var data = line.Split(',');
                var questType = (QuestTypes)Enum.Parse(typeof(QuestTypes), data[0]);
                var condition = (QuestCondition)Enum.Parse(typeof(QuestCondition), data[1]);
                var desc = data[2];
                var questKey = data[3];
                var goal = int.Parse(data[4]);
                var item1RewardValue = int.Parse(data[5]);
                var item2GreenPiece = int.Parse(data[6]);
                var item2BluePiece = int.Parse(data[7]);
                var item2PurplePiece = int.Parse(data[8]);
                if (condition != targetCondition) continue;
                
                if (targetCondition == QuestCondition.Rotation)
                {
                    rotationQuestData?.Add(data);
                    continue;
                }

                if (questType is QuestTypes.ViewAds or QuestTypes.AllClear)
                {
                    switch (questType)
                    {
                        case QuestTypes.ViewAds:
                            adsDesc.text = desc;
                            adsValue = PlayerPrefs.GetInt(questKey, 0);
                            adsProgress.value = adsValue;
                            adsGoal = goal;
                            adsProgress.maxValue = adsGoal;
                            adsProgressText.text = $"{adsValue} / {adsGoal}";
                            break;
                        case QuestTypes.AllClear:
                            allClearDesc.text = desc;
                            allClearValue = PlayerPrefs.GetInt(questKey, 0);
                            allClearProgress.value = allClearValue;
                            allClearGoal = goal;
                            allClearProgress.maxValue = allClearGoal;
                            allClearProgressText.text = $"{allClearValue} / {allClearGoal}";
                            break;
                    }
                }
                else
                {
                    var fixQuestObject = Instantiate(questPrefab, questTransform.transform);
                    fixQuestObject.questType = questType;
                    fixQuestObject.questCondition = condition;
                    fixQuestObject.questDesc.text = desc;
                    fixQuestObject.questValue = PlayerPrefs.GetInt(questKey, 0);
                    fixQuestObject.questProgress.value = fixQuestObject.questValue;
                    fixQuestObject.questGoal = goal;
                    fixQuestObject.questProgress.maxValue = fixQuestObject.questGoal;
                    fixQuestObject.questProgressText.text = $"{fixQuestObject.questValue} / {fixQuestObject.questGoal}";
                    fixQuestObject.item1Value.text = item1RewardValue.ToString();
                    fixQuestObject.item2Value.text = (item2GreenPiece + item2BluePiece + item2PurplePiece).ToString();
                    _fixQuestList.Add(fixQuestObject);
                }
            }

            if (targetCondition != QuestCondition.Rotation) return;
            {
                if (rotationQuestData != null)
                {
                    var selectedQuestData = rotationQuestData.OrderBy(q => UnityEngine.Random.value).Take(MaxRotationQuests).ToList();

                    foreach (var data in selectedQuestData)
                    {
                        var questType = (QuestTypes)Enum.Parse(typeof(QuestTypes), data[0]);
                        var condition = (QuestCondition)Enum.Parse(typeof(QuestCondition), data[1]);
                        var desc = data[2];
                        var questKey = data[3];
                        var goal = int.Parse(data[4]);
                        var item1RewardValue = int.Parse(data[5]);
                        var item2GreenPiece = int.Parse(data[6]);
                        var item2BluePiece = int.Parse(data[7]);
                        var item2PurplePiece = int.Parse(data[8]);
                        var rotationQuestObject = Instantiate(questPrefab, questTransform.transform);
                        rotationQuestObject.questType = questType;
                        rotationQuestObject.questCondition = condition;
                        rotationQuestObject.questDesc.text = desc;
                        rotationQuestObject.questValue = PlayerPrefs.GetInt(questKey, 0);
                        rotationQuestObject.questProgress.value = rotationQuestObject.questValue;
                        rotationQuestObject.questGoal = goal;
                        rotationQuestObject.questProgress.maxValue = rotationQuestObject.questGoal;
                        rotationQuestObject.questProgressText.text = $"{rotationQuestObject.questValue} / {rotationQuestObject.questGoal}";
                        rotationQuestObject.item1Value.text = item1RewardValue.ToString();
                        rotationQuestObject.item2Value.text = (item2GreenPiece + item2BluePiece + item2PurplePiece).ToString();
                        _rotationQuestList.Add(rotationQuestObject);
                    }
                }
                SaveSelectedQuestList(_rotationQuestList);
            }
        }
        private static void SaveSelectedQuestList(List<QuestObject> selectedQuests)
        {
            var selectedQuestKeys = string.Join(",", selectedQuests.Select(q => q.questType.ToString()));
            PlayerPrefs.SetString(QuestLoadKey, selectedQuestKeys);
            foreach (var quest in selectedQuests)
            {
                PlayerPrefs.SetString(quest.questType + "_desc", quest.questDesc.text);
                PlayerPrefs.SetInt(quest.questType + "_value", quest.questValue);
                PlayerPrefs.SetInt(quest.questType + "_goal", quest.questGoal); 
                PlayerPrefs.SetString(quest.item1Value + "_item1Value", quest.item1Value.text);
                PlayerPrefs.SetString(quest.item2Value + "_item2Value", quest.item2Value.text);
            }
            PlayerPrefs.Save();
        }
        private void LoadSelectedQuestList()
        {
            if (!PlayerPrefs.HasKey(QuestLoadKey)) return;
            var selectedQuestKeys = PlayerPrefs.GetString(QuestLoadKey);
            var questTypes = selectedQuestKeys.Split(',').Select(q => (QuestTypes)Enum.Parse(typeof(QuestTypes), q));
            foreach (var questType in questTypes)
            {
                var questObject = Instantiate(questPrefab, questTransform.transform);
                questObject.questType = questType;
                questObject.questDesc.text = PlayerPrefs.GetString(questType + "_desc");
                questObject.questValue = PlayerPrefs.GetInt(questType + "_value");
                questObject.questGoal = PlayerPrefs.GetInt(questType + "_goal");
                questObject.questProgress.value = questObject.questValue;
                questObject.questProgress.maxValue = questObject.questGoal;
                questObject.questProgressText.text = $"{questObject.questValue} / {questObject.questGoal}";
                questObject.item1Value.text = PlayerPrefs.GetString(questType + "_item1Value");
                questObject.item2Value.text = PlayerPrefs.GetString(questType + "_item2Value"); 
                _rotationQuestList.Add(questObject);
            }
        }
    }
}
