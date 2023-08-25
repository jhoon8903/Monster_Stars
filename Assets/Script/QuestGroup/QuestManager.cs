using System;
using System.Collections.Generic;
using System.Linq;
using Script.AdsScript;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.RobbyScript.CharacterSelectMenuGroup;
using Script.RobbyScript.StoreMenuGroup;
using Script.RobbyScript.TopMenuGroup;
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
        [SerializeField] private GameObject questRewardContents;
        [SerializeField] private QuestObject questObject;
        [SerializeField] public Transform questTransform;
        [SerializeField] public TextMeshProUGUI timer;
        [SerializeField] private Goods rewardItem;
        [SerializeField] private Button questRewardCloseBtn ;
        
        private readonly List<QuestData> _rotationQuestCandidates = new List<QuestData>();
        public readonly List<QuestAssemble> FixQuestList = new List<QuestAssemble>();
        public readonly List<QuestAssemble> RotationQuestList = new List<QuestAssemble>();
        public enum QuestTypes { AllClear, UseCoin, GetCoin, OpenBox, GetPiece, KillEnemy, KillBoss, ViewAds, MatchCoin, Victory }
        public enum QuestCondition { Fix, Rotation }
        private string[] _csvData;
        public static QuestManager Instance { get; private set; } 
        private readonly Dictionary<CharacterBase, int> _unitPieceDict = new Dictionary<CharacterBase, int>();
        private int _unitPieceReward;
        private Goods _coinObject;
        private Goods _unitPieceObject;
        public List<QuestAssemble> questInstances = new List<QuestAssemble>();

        // Data Key
        public string completeKey = "_Complete";
        public string shuffleKey = "_Shuffle";
        public string receiveKey = "_Receive";
        public string questDataKey = "QuestData";
        public string valueKey = "_value";

        //Shuffle
        public QuestAssemble shuffleQuest;
        public static string SetKey(QuestAssemble quest, string key)
        {
            return $"{quest.questKey}{key}";
        }

        [Serializable] public class QuestData
        { 
            public string questType;
            public string questCondition;
            public string questDesc;
            public string questKey;
            public int questGoal;
            public int questValue;
            public int item1CoinValue;
            public int item2GreenPieceValue;
            public int item3BluePieceValue;
            public int item4PurplePieceValue;
            public bool isReceived;
            public bool isCompleted;
            public bool isShuffled;
        }

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
            questRewardCloseBtn.onClick.AddListener(ReceiveQuestReward);
      
            if (PlayerPrefs.HasKey(questDataKey))
            {
                SetUpRotationList();
                LoadQuests();
            }
            else
            {
                CreateNewQuests();
            }
        }
        public static QuestTypes ParseQuestType(string type) => (QuestTypes)Enum.Parse(typeof(QuestTypes), type);
        public static QuestCondition ParseQuestCondition(string condition) => (QuestCondition)Enum.Parse(typeof(QuestCondition), condition);
        private void SetupCsv()
        {
            var csvFile = Resources.Load<TextAsset>("questData");
            var csvText = csvFile.text;
            _csvData = csvText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        }
        private List<QuestData> ParseCsvData()
        {
            SetupCsv();
            var parsedData = new List<QuestData>();
            for (var i = 1; i < _csvData.Length; i++)
            {
                var line = _csvData[i];
                if (string.IsNullOrEmpty(line)) continue;
                var data = ConvertToQuestData(line.Split(','));
                parsedData.Add(data);
            }
            return parsedData;
        }
        private void CreateNewQuests()
        {
            if (questInstances.Count > 0)
            {
                foreach (var quest in questInstances)
                {
                    Destroy(quest.gameObject); 
                }
            }
            var dataList = ParseCsvData();
            foreach (var data in dataList)
            {
                var condition = ParseQuestCondition(data.questCondition);
                if (condition == QuestCondition.Fix)
                {
                    questObject.FixQuestCreate(data);
                }
                else
                {
                    _rotationQuestCandidates.Add(data);
                }
            }
            RotationQuestSelection();
        }
        private void SetUpRotationList()
        {
            var dataList = ParseCsvData();
            _rotationQuestCandidates.Clear();
            foreach (var data in from data in dataList let condition = ParseQuestCondition(data.questCondition) where condition == QuestCondition.Rotation select data)
            {
                _rotationQuestCandidates.Add(data);
            }
        }
        private static QuestData ConvertToQuestData(IReadOnlyList<string> data)
        {
            return new QuestData
            {
                questType = data[0],
                questCondition = data[1],
                questDesc = data[2],
                questKey = data[3],
                questGoal = int.Parse(data[4]),
                item1CoinValue = int.Parse(data[5]),
                item2GreenPieceValue = int.Parse(data[6]),
                item3BluePieceValue = int.Parse(data[7]),
                item4PurplePieceValue = int.Parse(data[8]),
            };
        }
        private void RotationQuestSelection()
        {
            var selectedQuestData = _rotationQuestCandidates
                .Where(data => questInstances.All(q => q.QuestType.ToString() != data.questType))
                .OrderBy(_ => Random.value)
                .Take(3)
                .ToList();

            foreach (var selectedQuest in selectedQuestData)
            {
                questObject.RotationQuestCreate(selectedQuest);
                _rotationQuestCandidates.Remove(selectedQuest);
            }
        }
        public void SaveQuest(IEnumerable<QuestAssemble> quests)
        {
            var existingDataJson = PlayerPrefs.GetString(questDataKey);
            var existingQuestList = new List<QuestData>();

            if (!string.IsNullOrEmpty(existingDataJson))
            {
                existingQuestList = existingDataJson.Split(';').Select(JsonUtility.FromJson<QuestData>).ToList();
            }

            var questDataList = quests.Select(quest => new QuestData
            {
                questType = quest.QuestType.ToString(),
                questCondition = quest.QuestCondition.ToString(),
                questDesc = quest.questDesc.text,
                questKey = quest.questKey,
                questGoal = quest.questGoal,
                item1CoinValue = int.Parse(quest.coinValueText.text),
                item2GreenPieceValue = int.Parse(quest.greenPieceValueText.text),
                item3BluePieceValue = int.Parse(quest.bluePieceValueText.text),
                item4PurplePieceValue = int.Parse(quest.purplePieceValueText.text),
                isReceived = quest.isReceived,
                isCompleted = quest.isCompleted,
                isShuffled = quest.isShuffled
            }).ToList();

            var nonDuplicateQuests = questDataList.Where(newQuest => existingQuestList.All(existingQuest => existingQuest.questType != newQuest.questType)).ToList();
            var combinedQuests = existingQuestList.Concat(nonDuplicateQuests).ToList();
            var jsonDataList = combinedQuests.Select(JsonUtility.ToJson).ToList();

            PlayerPrefs.SetString(questDataKey, string.Join(";", jsonDataList)); 
            Debug.Log(PlayerPrefs.GetString(questDataKey, string.Join(";", jsonDataList)));
            PlayerPrefs.Save();
        }
        private void LoadQuests()
        {
            var jsonData = PlayerPrefs.GetString(questDataKey); 
            Debug.Log(jsonData);
            var questList = jsonData.Split(';').Select(JsonUtility.FromJson<QuestData>).ToList();
            foreach (var instance in questInstances)
            {
                Destroy(instance.gameObject);
            }
            questInstances.Clear();
            foreach (var data in questList)
            {
                var condition = ParseQuestCondition(data.questCondition);
                if (condition == QuestCondition.Fix)
                {
                    questObject.CoinQuestsCreate(data);
                }
                else
                {
                    questObject.RotationQuestCreate(data);
                }
            }
        }
        public void ShuffleQuest()
        {
            Debug.Log("QM " + shuffleQuest.QuestType);
            RotationQuestList.Remove(shuffleQuest);
            questInstances.Remove(shuffleQuest);
            Destroy(shuffleQuest.gameObject);

            var existingQuestTypes = questInstances.Select(q => q.QuestType.ToString()).ToList();

            var availableQuestData = _rotationQuestCandidates
                .Where(data => !existingQuestTypes.Contains(data.questType))
                .Where(data => !bool.Parse(PlayerPrefs.GetString($"{data.questKey}{completeKey}", "false")))
                .ToList();

            if (availableQuestData.Any()) 
            {
                var newQuestData = availableQuestData.OrderBy(_ => Random.value).First();
                _rotationQuestCandidates.Remove(newQuestData); // Remove the selected quest data from candidates
                var newQuestInstance = questObject.RotationQuestCreate(newQuestData);
                RotationQuestList.Add(newQuestInstance);
            }

            shuffleQuest = null; 
            PlayerPrefs.Save();
            SaveQuest(FixQuestList.Concat(RotationQuestList).ToList());
        }

        public void CallShuffleAds(QuestAssemble questAssemble)
        {
            shuffleQuest = questAssemble;
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.ShuffleQuest;
            AdsManager.Instance.ShowRewardedAd();
        }
        private void InitQuest(QuestAssemble quest)
        {
            PlayerPrefs.DeleteKey(SetKey(quest,valueKey));
            PlayerPrefs.DeleteKey(SetKey(quest,completeKey));
            PlayerPrefs.DeleteKey(SetKey(quest,receiveKey));
            PlayerPrefs.DeleteKey(SetKey(quest,shuffleKey));
        }
        public void ResetQuest()
        {
            foreach (var quest in FixQuestList)
            {
                InitQuest(quest);
            }
            foreach (var quest in RotationQuestList)
            { 
                InitQuest(quest);
            } 
            FixQuestList.Clear();
            RotationQuestList.Clear();
            _rotationQuestCandidates.Clear();
            PlayerPrefs.DeleteKey(questDataKey);
            PlayerPrefs.Save();
            CreateNewQuests();
        }

  
        // public void ReceiveQuestReward(QuestObject quest)
        // {
        //     questRewardPanel.SetActive(true);
        //     CoinsScript.Instance.Coin += int.Parse(quest.coinValue.text);
        //     CalculateUnitPieceReward(quest);
        //     Quest.Instance.AllClearQuest();
        //     quest.isReceived = true;
        //     QuestObject.UpdateQuestStates(quest);
        //     PlayerPrefs.SetInt(quest.QuestType + "_isReceived", 1);
        //     quest.receiveBtn.GetComponent<Button>().interactable = false;
        //     quest.receiveBtnText.text = "Completed";
        //     if (FixQuestList.Contains(quest))
        //     {
        //        SaveFixQuestList(FixQuestList);
        //     }
        //     else if (RotationQuestList.Contains(quest))
        //     {
        //        SaveRotationQuestList(RotationQuestList);
        //     }
        //     PlayerPrefs.Save();
        // }
        // public void UpdateQuest(QuestAssemble quest, int value)
        // {
        //     if (FixQuestList.Contains(quest) || RotationQuestList.Contains(quest))
        //     {
        //         if (quest.QuestCondition == QuestCondition.Fix)
        //         {
        //             var fixValue = 0;
        //             var fixGoal = 0;
        //
        //
        //             if (fixValue >= fixGoal)
        //             {
        //                 // QuestClear 
        //                 // 버튼 활성화 
        //             }
        //
        //         }
        //         quest.questValue += value;
        //         if (quest.questValue >= quest.questGoal)
        //         {
        //             quest.isCompleted = true;
        //             PlayerPrefs.SetInt(quest.questKey + "_isCompleted", 1);
        //             QuestObject.UpdateQuestStates(quest);
        //         }
        //
        //
        //         if (FixQuestList.Contains(quest))
        //         {
        //             SaveFixQuestList(FixQuestList);
        //         }
        //         else if (RotationQuestList.Contains(quest))
        //         {
        //             SaveRotationQuestList(RotationQuestList);
        //         }
        //     }
        //     PlayerPrefs.Save();
        // }

        public void ReceiveQuestReward()
        {
            ChestCheck.Instance.CloseChestCheck();
            questRewardPanel.SetActive(false);
            foreach (var unitReward in _unitPieceDict)
            {
                unitReward.Key.UnitPieceCount += unitReward.Value;
                HoldCharacterList.Instance.UpdateRewardPiece(unitReward.Key);
                Destroy(_unitPieceObject.gameObject);
            }
            if (_coinObject != null)
            {
                Destroy(_coinObject.gameObject);
            }
            _unitPieceDict.Clear();
        }
        // private int UnitPieceReceiveValue(CharacterBase.UnitGrades unitGrade, QuestObject quest)
        // {
        //     var greenPiece = 0;
        //     var bluePiece = 0;
        //     var purplePiece = 0;
        //     for (var i = 1; i < _csvData.Length; i++)
        //     {
        //         var line = _csvData[i];
        //         if (string.IsNullOrEmpty(line)) continue;
        //         var data = line.Split(',');
        //         var questType = ParseQuestType(data[0]);
        //         if (questType != quest.QuestType) continue;
        //         greenPiece = int.Parse(data[6]); 
        //         bluePiece = int.Parse(data[7]);
        //         purplePiece = int.Parse(data[8]);
        //     }
        //     return unitGrade switch
        //     {
        //         CharacterBase.UnitGrades.G => greenPiece,
        //         CharacterBase.UnitGrades.B => bluePiece,
        //         CharacterBase.UnitGrades.P => purplePiece
        //     };
        // }
        // private void CalculateUnitPieceReward(QuestObject quest)
        // {
        //     var possibleIndices = Enumerable.Range(0, TimeRewardManager.Instance.unitList.Count).ToList();
        //     var selectedUnitIndices = new List<int>();
        //     var pieceCountPerUnit = new Dictionary<int, int>();
        //     foreach (var index in possibleIndices)
        //     {
        //         pieceCountPerUnit.TryAdd(index, 0);
        //     }
        //
        //     while (possibleIndices.Count > 0)
        //     {
        //         var randomIndex = Random.Range(0, possibleIndices.Count);
        //         selectedUnitIndices.Add(possibleIndices[randomIndex]);
        //         possibleIndices.RemoveAt(randomIndex);
        //     }
        //
        //     var totalPiecesPerGrade = new Dictionary<CharacterBase.UnitGrades, int>
        //     {
        //         { CharacterBase.UnitGrades.G, UnitPieceReceiveValue(CharacterBase.UnitGrades.G, quest) },
        //         { CharacterBase.UnitGrades.B, UnitPieceReceiveValue(CharacterBase.UnitGrades.B, quest) },
        //         {CharacterBase.UnitGrades.P, UnitPieceReceiveValue(CharacterBase.UnitGrades.P, quest)}
        //     };
        //
        //     foreach (var grade in totalPiecesPerGrade.Keys)
        //     {
        //         var unitsOfThisGrade = selectedUnitIndices.Where(index =>
        //             TimeRewardManager.Instance.unitList[index].UnitGrade == grade && TimeRewardManager.Instance.unitList[index].unitPieceLevel < 14).ToList();
        //         var remainingPieces = totalPiecesPerGrade[grade];
        //         foreach (var index in unitsOfThisGrade)
        //         {
        //             pieceCountPerUnit.TryAdd(index, 0);
        //             if (remainingPieces > 1)
        //             {
        //                 var piecesForThisUnit = Random.Range(1, remainingPieces);
        //                 pieceCountPerUnit[index] = piecesForThisUnit;
        //                 remainingPieces -= piecesForThisUnit;
        //             }
        //             else
        //             {
        //                 pieceCountPerUnit[index] = remainingPieces;
        //                 remainingPieces = 0;
        //                 break;
        //             }
        //         }
        //         while (remainingPieces > 0 && unitsOfThisGrade.Count > 0)
        //         {
        //             for (var i = 0; i < unitsOfThisGrade.Count && remainingPieces > 0; i++)
        //             {
        //                 var index = unitsOfThisGrade[i];
        //                 pieceCountPerUnit[index]++;
        //                 remainingPieces--;
        //             }
        //         }
        //     }
        //     foreach (var index in selectedUnitIndices)
        //     {
        //         var unit = TimeRewardManager.Instance.unitList[index];
        //         if (unit.unitPieceLevel >= 14) continue;
        //         unit.Initialize();
        //         _unitPieceReward = pieceCountPerUnit[index];
        //         if (_unitPieceReward == 0) continue;
        //         _unitPieceObject = Instantiate(rewardItem, questRewardContents.transform);
        //         _unitPieceObject.goodsBack.GetComponent<Image>().color = Color.white;
        //         _unitPieceObject.goodsBack.GetComponent<Image>().sprite = unit.UnitGrade switch
        //         {
        //             CharacterBase.UnitGrades.G => StoreMenu.Instance.gGradeSprite,
        //             CharacterBase.UnitGrades.B => StoreMenu.Instance.bGradeSprite,
        //             CharacterBase.UnitGrades.P => StoreMenu.Instance.pGradeSprite,
        //         };
        //         _unitPieceObject.goodsSprite.GetComponent<Image>().sprite = unit.GetSpriteForLevel(unit.unitPieceLevel);
        //         _unitPieceObject.goodsSprite.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
        //         _unitPieceObject.goodsValue.text = $"{_unitPieceReward}";
        //         _unitPieceObject.goodsValue.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
        //         _unitPieceDict[unit] = _unitPieceReward;
        //     }
        //     foreach (var unitPiece in _unitPieceDict)
        //     {
        //         unitPiece.Key.UnitPieceCount += unitPiece.Value;
        //         HoldCharacterList.Instance.UpdateRewardPiece(unitPiece.Key);
        //     }
        //     _unitPieceDict.Clear();
        // }
    }   
}
