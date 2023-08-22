using System;
using System.Collections.Generic;
using System.Linq;
using Script.AdsScript;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.RobbyScript.CharacterSelectMenuGroup;
using Script.RobbyScript.StoreMenuGroup;
using Script.RobbyScript.TopMenuGroup;
using Script.UIManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script.QuestGroup
{ 
    public class QuestManager : MonoBehaviour
    {

        [SerializeField] private GameObject questPanel;
        [SerializeField] private Button questOpenBtn;
        [SerializeField] private Button questCloseBtn;
        [SerializeField] private GameObject questRewardPanel;
        [SerializeField] private QuestObject questObject;
        [SerializeField] public Transform questTransform;
        [SerializeField] public TextMeshProUGUI timer;

        // Ads Quest
        [SerializeField] public TextMeshProUGUI adsDesc;
        [SerializeField] public Slider adsProgress;
        [SerializeField] public TextMeshProUGUI adsProgressText;
        [SerializeField] private Button adsRewardBtn;
        public int adsValue;
        public int adsGoal;
        
        // AllClear Quest
        [SerializeField] public TextMeshProUGUI allClearDesc;
        [SerializeField] public Slider allClearProgress;
        [SerializeField] public TextMeshProUGUI allClearProgressText;
        [SerializeField] private Button allClearRewardBtn;
        public int allClearValue;
        public int allClearGoal;

        public readonly List<QuestObject> FixQuestList = new List<QuestObject>();
        public readonly List<QuestObject> RotationQuestList = new List<QuestObject>();
        public enum QuestTypes { AllClear, UseCoin, GetCoin, OpenBox, GetPiece, KillEnemy, KillBoss, ViewAds, MatchCoin, Victory }
        public enum QuestCondition { Fix, Rotation }
        private const int MaxRotationQuests = 3;
        private const string QuestLoadKey = "SelectedRotationQuests";
        private const string FixQuestLoadKey = "FixQuestLoadKey";
        private const string RotationQuestLoadKey = "RotationQuestLoadKey";
        private string[] _csvData;
        public static QuestManager Instance { get; private set; } 
        private readonly Dictionary<CharacterBase, int> _unitPieceDict = new Dictionary<CharacterBase, int>();
        private int _unitPieceReward;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
          
            questOpenBtn.onClick.AddListener(() => questPanel.SetActive(true));
            questCloseBtn.onClick.AddListener(() => questPanel.SetActive(false));
            LoadQuests(QuestCondition.Fix);
            LoadQuests(QuestCondition.Rotation);
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
                questObject.HandleQuest(targetCondition, data, questType, rotationQuestData);
            }
            HandleRotationQuest(targetCondition, rotationQuestData);
        }
        public void ResetQuest()
        {
            foreach (var quest in FixQuestList)
            {
                switch (quest.QuestType)
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
                        PlayerPrefs.SetInt(quest.QuestType + "_isCompleted", 0);
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
                PlayerPrefs.SetInt(quest.QuestType + "_isCompleted", 0);
            }
            ShuffleQuest();
            PlayerPrefs.Save();
        }
        public static QuestTypes ParseQuestType(string type) => (QuestTypes)Enum.Parse(typeof(QuestTypes), type);
        public static QuestCondition ParseQuestCondition(string condition) => (QuestCondition)Enum.Parse(typeof(QuestCondition), condition);
        private void HandleRotationQuest(QuestCondition targetCondition, IReadOnlyCollection<string[]> rotationQuestData)
        {
            if (targetCondition != QuestCondition.Rotation) return;
            if (rotationQuestData == null) return;
            var selectedQuestData = rotationQuestData.OrderBy(_ => Random.value).Take(MaxRotationQuests).ToList();
            foreach (var data in selectedQuestData)
            {
                CreateAndAddRotationQuest(data);
            }
        }
        private void CreateAndAddRotationQuest(string[] data)
        {
            var rotationQuestObject = questObject.CreateQuestFromData(data);
            QuestObject.SetQuestButtonStates(rotationQuestObject);
            rotationQuestObject.shuffleBtn.GetComponent<Button>().onClick.AddListener(CallShuffleAds);
            RotationQuestList.Add(rotationQuestObject);
            SaveSelectedQuestList(RotationQuestList);
        }
        private static void SaveSelectedQuestList(List<QuestObject> selectedQuests)
        {
            var selectedQuestKeys = string.Join(",", selectedQuests.Select(q => q.QuestType + ":" + q.questKey));
            PlayerPrefs.SetString(QuestLoadKey, selectedQuestKeys);
            foreach (var quest in selectedQuests)
            {
                var key = quest.questKey;
                PlayerPrefs.SetString(quest.QuestType + "_desc", quest.questDesc.text);
                PlayerPrefs.SetInt(key + "_value", quest.questValue);
                PlayerPrefs.SetInt(key + "_goal", quest.questGoal); 
                PlayerPrefs.SetString(quest.coinValue + "_item1Value", quest.coinValue.text);
                PlayerPrefs.SetString(quest.greenPieceValue + "_item2Value", quest.greenPieceValue.text);
                PlayerPrefs.SetString(quest.bluePieceValue + "_item3Value", quest.bluePieceValue.text);
                PlayerPrefs.SetString(quest.purplePieceValue + "_item4Value", quest.purplePieceValue.text);
                PlayerPrefs.SetInt(quest.QuestType + "_isShuffled", quest.isShuffled ? 1 : 0);
                PlayerPrefs.SetInt(quest.QuestType + "_isCompleted", quest.isCompleted ? 1 : 0);
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
                    PlayerPrefs.SetInt(quest.QuestType + "_isCompleted", 1);
                }

                if (quest.QuestType == QuestTypes.AllClear)
                {
                    PlayerPrefs.SetInt(quest.questKey + "_value", quest.questValue);
                    allClearProgress.maxValue = allClearGoal;
                    allClearProgress.value = quest.questValue;
                    allClearProgressText.text = $"{quest.questValue} / {allClearGoal}";
                    PlayerPrefs.Save();
                }
                else if (quest.QuestType == QuestTypes.ViewAds)
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
            PlayerPrefs.SetString(RotationQuestLoadKey, string.Join(",", rotationQuests.Select(q => q.QuestType.ToString())));
            SaveQuests(rotationQuests);
            PlayerPrefs.Save();
        }
        private static void SaveFixQuestList(List<QuestObject> fixQuests)
        {
            PlayerPrefs.SetString(FixQuestLoadKey, string.Join(",", fixQuests.Select(q => q.QuestType.ToString())));
            SaveQuests(fixQuests);
            PlayerPrefs.Save();
        }
        private static void SaveQuests(List<QuestObject> quests)
        {
            foreach (var quest in quests)
            {
                var key = quest.questKey;
                PlayerPrefs.SetString(quest.QuestType + "_desc", quest.questDesc.text);
                PlayerPrefs.SetInt(key + "_value", quest.questValue);
                PlayerPrefs.SetInt(key + "_goal", quest.questGoal); 
                PlayerPrefs.SetString(quest.coinValue + "_item1Value", quest.coinValue.text);
                PlayerPrefs.SetString(quest.greenPieceValue + "_item2Value", quest.greenPieceValue.text);
                PlayerPrefs.SetString(quest.bluePieceValue + "_item3Value", quest.bluePieceValue.text);
                PlayerPrefs.SetString(quest.purplePieceValue + "_item4Value", quest.purplePieceValue.text);
                PlayerPrefs.SetInt(quest.QuestType + "_isShuffled", quest.isShuffled ? 1 : 0);
                PlayerPrefs.SetInt(quest.QuestType + "_isCompleted", quest.isCompleted ? 1 : 0);
                PlayerPrefs.SetInt(quest.QuestType + "_isReceived", quest.isReceived ? 1 : 0);
            }
        }
        public void ShuffleQuest()
        {
            var activeRotationQuestTypes = RotationQuestList.Select(q => q.QuestType).ToList();
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

            var newQuestData = inactiveRotationQuestData.OrderBy(_ => Random.value).FirstOrDefault();
            if (newQuestData == null) return;
            var newQuest = questObject.CreateQuestFromData(newQuestData);
            var questToRemove = RotationQuestList.FirstOrDefault();
            if (questToRemove != null)
            {
                RotationQuestList.Remove(questToRemove);
                Destroy(questToRemove.gameObject);
            }
            RotationQuestList.Add(newQuest);
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
            CoinsScript.Instance.Coin += int.Parse(quest.coinValue.text);
            CalculateUnitPieceReward(quest);
            Quest.Instance.AllClearQuest();
            quest.isReceived = true;
            PlayerPrefs.SetInt(quest.QuestType + "_isReceived", 1);
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
        private int UnitPieceReceiveValue(CharacterBase.UnitGrades unitGrade, QuestObject quest)
        {
            var greenPiece = 0;
            var bluePiece = 0;
            var purplePiece = 0;
            for (var i = 1; i < _csvData.Length; i++)
            {
                var line = _csvData[i];
                if (string.IsNullOrEmpty(line)) continue;
                var data = line.Split(',');
                var questType = ParseQuestType(data[0]);
                if (questType != quest.QuestType) continue;
                greenPiece = int.Parse(data[6]); 
                bluePiece = int.Parse(data[7]);
                purplePiece = int.Parse(data[8]);
            }
            return unitGrade switch
            {
                CharacterBase.UnitGrades.G => greenPiece,
                CharacterBase.UnitGrades.B => bluePiece,
                CharacterBase.UnitGrades.P => purplePiece
            };
        }
        private void CalculateUnitPieceReward(QuestObject quest)
        {
            var possibleIndices = Enumerable.Range(0, TimeRewardManager.Instance.unitList.Count).ToList();
            var selectedUnitIndices = new List<int>();
            var pieceCountPerUnit = new Dictionary<int, int>();
            foreach (var index in possibleIndices)
            {
                pieceCountPerUnit.TryAdd(index, 0);
            }

            while (possibleIndices.Count > 0)
            {
                var randomIndex = Random.Range(0, possibleIndices.Count);
                selectedUnitIndices.Add(possibleIndices[randomIndex]);
                possibleIndices.RemoveAt(randomIndex);
            }

            var totalPiecesPerGrade = new Dictionary<CharacterBase.UnitGrades, int>
            {
                { CharacterBase.UnitGrades.G, UnitPieceReceiveValue(CharacterBase.UnitGrades.G, quest) },
                { CharacterBase.UnitGrades.B, UnitPieceReceiveValue(CharacterBase.UnitGrades.B, quest) },
                {CharacterBase.UnitGrades.P, UnitPieceReceiveValue(CharacterBase.UnitGrades.P, quest)}
            };

            foreach (var grade in totalPiecesPerGrade.Keys)
            {
                var unitsOfThisGrade = selectedUnitIndices.Where(index =>
                    TimeRewardManager.Instance.unitList[index].UnitGrade == grade && TimeRewardManager.Instance.unitList[index].unitPeaceLevel < 14).ToList();
                var remainingPieces = totalPiecesPerGrade[grade];
                foreach (var index in unitsOfThisGrade)
                {
                    pieceCountPerUnit.TryAdd(index, 0);
                    if (remainingPieces > 1)
                    {
                        var piecesForThisUnit = Random.Range(1, remainingPieces);
                        pieceCountPerUnit[index] = piecesForThisUnit;
                        remainingPieces -= piecesForThisUnit;
                    }
                    else
                    {
                        pieceCountPerUnit[index] = remainingPieces;
                        remainingPieces = 0;
                        break;
                    }
                }
                while (remainingPieces > 0 && unitsOfThisGrade.Count > 0)
                {
                    for (var i = 0; i < unitsOfThisGrade.Count && remainingPieces > 0; i++)
                    {
                        var index = unitsOfThisGrade[i];
                        pieceCountPerUnit[index]++;
                        remainingPieces--;
                    }
                }
            }
            foreach (var index in selectedUnitIndices)
            {
                var unit = TimeRewardManager.Instance.unitList[index];
                if (unit.unitPeaceLevel >= 14) continue;
                unit.Initialize();
                _unitPieceReward = pieceCountPerUnit[index];
                if (_unitPieceReward == 0) continue;
                _unitPieceDict[unit] = _unitPieceReward;
            }
            foreach (var unitPiece in _unitPieceDict)
            {
                unitPiece.Key.CharacterPeaceCount += unitPiece.Value;
                HoldCharacterList.Instance.UpdateRewardPiece(unitPiece.Key);
            }
            _unitPieceDict.Clear();
        }
    }   
}
