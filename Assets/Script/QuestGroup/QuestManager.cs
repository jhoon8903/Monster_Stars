using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.AdsScript;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.RobbyScript.CharacterSelectMenuGroup;
using Script.RobbyScript.StoreMenuGroup;
using Script.RobbyScript.TopMenuGroup;
using TMPro;
using Unity.VisualScripting;
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
        [SerializeField] private Button questRewardCloseBtn;

        public readonly List<QuestData> _rotationQuestCandidates = new List<QuestData>();
        public readonly List<QuestAssemble> FixQuestList = new List<QuestAssemble>();
        public readonly List<QuestAssemble> RotationQuestList = new List<QuestAssemble>();

        public enum QuestTypes
        {
            AllClear,
            UseCoin,
            GetCoin,
            OpenBox,
            GetPiece,
            KillEnemy,
            KillBoss,
            ViewAds,
            MergeBox,
            Victory
        }

        public enum QuestCondition
        {
            Fix,
            Rotation
        }

        private string[] _csvData;
        public static QuestManager Instance { get; private set; }
        private readonly Dictionary<CharacterBase, int> _unitPieceDict = new Dictionary<CharacterBase, int>();
        private int _unitPieceReward;
        private Goods _coinObject;
        private Goods _unitPieceObject;
        public List<QuestAssemble> questInstances = new List<QuestAssemble>();

        // Data Key
        protected internal const string CompleteKey = "_Complete";
        protected internal const string ShuffleKey = "_Shuffle";
        protected internal const string ReceiveKey = "_Receive";
        private const string QuestDataKey = "QuestData";
        protected internal const string ValueKey = "_value";
        protected internal const string GoalKey = "_goal";

        public static string SetKey(QuestAssemble quest, string key)
        {
            return $"{quest.questKey}{key}";
        }

        [Serializable]
        public class QuestData
        {
            public string questType;
            public string questCondition;
            public string questDesc;
            public string questKey;
            public int questGoal;
            public int item1CoinValue;
            public int item2GreenPieceValue;
            public int item3BluePieceValue;
            public int item4PurplePieceValue;
        }

        private void Awake()
        {
            Instance = this;
            questOpenBtn.onClick.AddListener(() =>
            {
                questPanel.SetActive(true);
                questPanel.transform.localScale = Vector3.zero; 
                questPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
            });
            questCloseBtn.onClick.AddListener(() =>
            {
                questPanel.transform.DOScale(0.1f, 0.3f).SetEase(Ease.InBack).OnComplete(() => questPanel.SetActive(false));
            });
            // questRewardCloseBtn.onClick.AddListener(ReceiveQuestReward);
            if (PlayerPrefs.HasKey(QuestDataKey))
            {
                SetUpRotationList();
                LoadQuests();
            }
            else
            {
                CreateNewQuests();
            }
            DontDestroyOnLoad(gameObject);
        }
        public static QuestTypes ParseQuestType(string type) => (QuestTypes)Enum.Parse(typeof(QuestTypes), type);
        private static QuestCondition ParseQuestCondition(string condition) => (QuestCondition)Enum.Parse(typeof(QuestCondition), condition);
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
            foreach (var data in from data in dataList
                     let condition = ParseQuestCondition(data.questCondition)
                     where condition == QuestCondition.Rotation
                     select data)
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
            foreach (var selectedQuest in from selectedQuest in selectedQuestData let newRotationQuest = questObject.RotationQuestCreate(selectedQuest) select selectedQuest)
            {
                _rotationQuestCandidates.Remove(selectedQuest);
            }
        }
        public static void SaveQuest(IEnumerable<QuestAssemble> quests)
        {
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
            }).ToList();
            var jsonDataList = questDataList.Select(JsonUtility.ToJson).ToList();
            PlayerPrefs.DeleteKey(QuestDataKey);
            PlayerPrefs.SetString(QuestDataKey, string.Join(";", jsonDataList));
            PlayerPrefs.Save();
        }
        private void LoadQuests()
        {
            var jsonData = PlayerPrefs.GetString(QuestDataKey);
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
                    questObject.FixQuestCreate(data);
                }
                else
                {
                    questObject.RotationQuestCreate(data);
                }
            }

            var dataList = ParseCsvData();
            foreach (var data in dataList)
            {
                var type = ParseQuestType(data.questType);
                if (type is QuestTypes.AllClear or QuestTypes.ViewAds)
                {
                    questObject.SpecialQuests(type, data);
                }
            }
        }
        private static QuestData ConvertQuestAssembleToQuestData(QuestAssemble questAssemble)
        {
            var questData = new QuestData
            {
                questType = questAssemble.QuestType.ToString(),
                questCondition = questAssemble.QuestCondition.ToString(),
                questDesc = questAssemble.questDesc.text,
                questKey = questAssemble.questKey,
                questGoal = questAssemble.questGoal,
                item1CoinValue = questAssemble.coinValue,
                item2GreenPieceValue = questAssemble.greenValue,
                item3BluePieceValue = questAssemble.blueValue,
                item4PurplePieceValue = questAssemble.purpleValue
            };
            return questData;
        }

        public IEnumerator ShuffleDestroy(QuestAssemble shuffleQuest)
        {
            Debug.Log(shuffleQuest.QuestType);
            InitQuest(shuffleQuest);
            var shuffleData = ConvertQuestAssembleToQuestData(shuffleQuest);
            if (questInstances.Contains(shuffleQuest))                            
            {
                questInstances.Remove(shuffleQuest);
            }
            if (RotationQuestList.Contains(shuffleQuest))
            {
                RotationQuestList.Remove(shuffleQuest);
            }
            Destroy(shuffleQuest.gameObject);
            AdsManager.Instance.shuffleQuest = null;
            yield return null;
            ShuffleQuest();
        }

        private void ShuffleQuest()
        {
            var existingQuestTypes = questInstances.Where(q => q.QuestCondition != QuestCondition.Fix).Select(q => q.QuestType).ToList();
            var availableQuestData = (from data in _rotationQuestCandidates from type in existingQuestTypes where data.questType == type.ToString() select data).ToList();
            var filteredQuestData = availableQuestData
                .Where(q => PlayerPrefs.GetString($"{q.questKey}{CompleteKey}", "false") == "false")
                .ToList();
            if (filteredQuestData.Any())
            {
                var newQuestData = filteredQuestData.OrderBy(_ => Random.value).First();
                _rotationQuestCandidates.Remove(newQuestData);
                filteredQuestData.Remove(newQuestData);
                var newQuest = questObject.RotationQuestCreate(newQuestData);
                newQuest.isShuffled = true;
                PlayerPrefs.SetString(SetKey(newQuest, ShuffleKey), "true");
                QuestObject.UpdateQuestStates(newQuest);
                PlayerPrefs.Save();
            }
            SaveQuest(FixQuestList.Concat(RotationQuestList));
        }
        private static void InitQuest(QuestAssemble quest)
        {
            PlayerPrefs.DeleteKey(SetKey(quest, ValueKey));
            PlayerPrefs.DeleteKey(SetKey(quest, CompleteKey));
            PlayerPrefs.DeleteKey(SetKey(quest, ReceiveKey));
            PlayerPrefs.DeleteKey(SetKey(quest, ShuffleKey));
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
            QuestObject.InitSpecialQuest(QuestTypes.AllClear);
            QuestObject.InitSpecialQuest(QuestTypes.ViewAds);
            PlayerPrefs.DeleteKey(QuestDataKey);
            PlayerPrefs.Save();
            CreateNewQuests();
        }
        private void CloseReward()
        {
            foreach (var unitReward in _unitPieceDict)
            {
                unitReward.Key.UnitPieceCount += unitReward.Value;
                HoldCharacterList.Instance.UpdateRewardPiece(unitReward.Key);
                Destroy(_unitPieceObject.gameObject);
                _unitPieceObject = null;
            }
            if (_coinObject != null)
            {
                Destroy(_coinObject.gameObject);
                _coinObject = null;
            }
            _unitPieceDict.Clear();
            Quest.Instance.AllClearQuest();
            questRewardPanel.SetActive(false);
            
        }
        public IEnumerator UpdateQuest(QuestAssemble quest, int value)
        {
            if (FixQuestList.Contains(quest) || RotationQuestList.Contains(quest))
            {
                quest.questValue = PlayerPrefs.GetInt(SetKey(quest, ValueKey), 0);
                quest.questValue += value;
                quest.questProgress.value = quest.questValue;
                quest.questProgressText.text = $"{quest.questValue} / {quest.questGoal}";
                PlayerPrefs.SetInt(SetKey(quest, ValueKey), quest.questValue);
                if (quest.questValue >= quest.questGoal)
                {
                    quest.questProgressText.text = $"{quest.questGoal} / {quest.questGoal}";
                    quest.isCompleted = true;
                    PlayerPrefs.GetString(SetKey(quest, CompleteKey), "true");
                }
                QuestObject.UpdateQuestStates(quest);
                PlayerPrefs.Save();
            }
            yield return null;
        }
        public IEnumerator SpecialQuestUpdate(QuestTypes questType, int value)
        {
            if (questObject == null) yield break;
            switch (questType)
            {
                case QuestTypes.ViewAds:
                {
                    questObject.adsGoal = PlayerPrefs.GetInt($"{questType}{GoalKey}", 0);
                    questObject.adsValue = PlayerPrefs.GetInt($"{questType}{ValueKey}", 0);
                    questObject.adsValue += value;
                    questObject.adsProgress.maxValue = questObject.adsGoal;
                    questObject.adsProgress.value = questObject.adsValue;
                    questObject.adsProgressText.text = $"{questObject.adsValue} / {questObject.adsGoal}";
                    if (questObject.adsValue >= questObject.adsGoal)
                    {
                        questObject.adsProgressText.text = $"{questObject.adsGoal} / {questObject.adsGoal}";
                        questObject.adsRewardBtn.interactable = true;
                        questObject.adsRewardBtn.onClick.AddListener(() => SpecialQuestReward(questType));
                        PlayerPrefs.SetString($"{questType}{CompleteKey}", "true");
                    }
                    PlayerPrefs.SetInt($"{questType}{ValueKey}", questObject.adsValue);
                    PlayerPrefs.Save();
                    yield return null;
                    break;
                }
                case QuestTypes.AllClear:
                {
                    questObject.allClearGoal = PlayerPrefs.GetInt($"{questType}{GoalKey}", 0);
                    questObject.allClearValue = PlayerPrefs.GetInt($"{questType}{ValueKey}", 0);
                    questObject.allClearValue += value;
                    questObject.allClearProgress.maxValue = questObject.allClearGoal;
                    questObject.allClearProgress.value = questObject.allClearValue;
                    questObject.allClearProgressText.text = $"{questObject.allClearValue} / {questObject.allClearGoal}";
                    if (questObject.allClearValue >= questObject.allClearGoal)
                    {
                        questObject.allClearProgressText.text = $"{questObject.allClearGoal} / {questObject.allClearGoal}";
                        questObject.allClearRewardBtn.interactable = true;
                        questObject.allClearRewardBtn.onClick.AddListener(() => SpecialQuestReward(questType));
                        PlayerPrefs.SetString($"{questType}{CompleteKey}", "true");
                    }
                    PlayerPrefs.SetInt($"{questType}{ValueKey}", questObject.allClearValue);
                    PlayerPrefs.Save();
                    yield return null;
                    break;
                }
            }

        }
        public void SpecialQuestReward(QuestTypes quest)
        {
            questObject.adsRewardBtn.onClick.RemoveListener(() => SpecialQuestReward(quest));
            questObject.allClearRewardBtn.onClick.RemoveListener(() => SpecialQuestReward(quest));
            questRewardPanel.SetActive(true);
            SpecialQuestCoinCalculate(quest);
            SpecialQuestPieceCalculate(quest);
            switch (quest)
            {
                case QuestTypes.ViewAds:
                    questObject.adsRewardBtn.interactable = false;
                    PlayerPrefs.SetString($"{quest}{ReceiveKey}", "true");
                    break;
                case QuestTypes.AllClear:
                    questObject.allClearRewardBtn.interactable = false;
                    PlayerPrefs.SetString($"{quest}{ReceiveKey}", "true");
                    break;
            }
            questRewardCloseBtn.onClick.AddListener(CloseReward);
        }
        private void SpecialQuestPieceCalculate(QuestTypes quest)
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
                { CharacterBase.UnitGrades.G, SpecialRewardPieceValue(CharacterBase.UnitGrades.G, quest) },
                { CharacterBase.UnitGrades.B, SpecialRewardPieceValue(CharacterBase.UnitGrades.B, quest) },
                { CharacterBase.UnitGrades.P, SpecialRewardPieceValue(CharacterBase.UnitGrades.P, quest) }
            };

            foreach (var grade in totalPiecesPerGrade.Keys)
            {
                var unitsOfThisGrade = selectedUnitIndices.Where(index =>
                    TimeRewardManager.Instance.unitList[index].UnitGrade == grade &&
                    TimeRewardManager.Instance.unitList[index].unitPieceLevel < 14).ToList();
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
                if (unit.unitPieceLevel >= 14) continue;
                unit.Initialize();
                _unitPieceReward = pieceCountPerUnit[index];
                if (_unitPieceReward == 0) continue;
                _unitPieceObject = Instantiate(rewardItem, questRewardContents.transform);
                _unitPieceObject.goodsBack.GetComponent<Image>().color = Color.white;
                _unitPieceObject.goodsBack.GetComponent<Image>().sprite = unit.UnitGrade switch
                {
                    CharacterBase.UnitGrades.G => StoreMenu.Instance.gGradeSprite,
                    CharacterBase.UnitGrades.B => StoreMenu.Instance.bGradeSprite,
                    CharacterBase.UnitGrades.P => StoreMenu.Instance.pGradeSprite,
                };
                _unitPieceObject.goodsSprite.GetComponent<Image>().sprite = unit.GetSpriteForLevel(unit.unitPieceLevel);
                _unitPieceObject.goodsValue.text = $"{_unitPieceReward}";
                _unitPieceDict[unit] = _unitPieceReward;
            }

            foreach (var unitPiece in _unitPieceDict)
            {
                unitPiece.Key.UnitPieceCount += unitPiece.Value;
                HoldCharacterList.Instance.UpdateRewardPiece(unitPiece.Key);
            }

            _unitPieceDict.Clear();
        }
        private int SpecialRewardPieceValue(CharacterBase.UnitGrades unitGrades, QuestTypes quest)
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
                if (questType != quest) continue;
                greenPiece = int.Parse(data[6]); 
                bluePiece = int.Parse(data[7]);
                purplePiece = int.Parse(data[8]);
            }
            return unitGrades switch
            {
                CharacterBase.UnitGrades.G => greenPiece,
                CharacterBase.UnitGrades.B => bluePiece,
                CharacterBase.UnitGrades.P => purplePiece
            };
        }
        private void SpecialQuestCoinCalculate(QuestTypes quest)
        {
            if (_coinObject != null)
            {
                Destroy(_coinObject.gameObject);
            }
            _coinObject = Instantiate(rewardItem, questRewardContents.transform);
            _coinObject.goodsBack.GetComponent<Image>().sprite = _coinObject.coinSprite;
            _coinObject.goodsSprite.GetComponent<Image>().sprite = StoreMenu.Instance.specialOffer.coinSprite;
            var value = 0;
            foreach (var data in ParseCsvData().Where(data => data.questType == quest.ToString()))
            {
                value = data.item1CoinValue;
            }
            _coinObject.goodsValue.text = value.ToString();
            CoinsScript.Instance.Coin += value;
        }
        public void ReceiveQuestReward(QuestAssemble quest)
        {
            quest.isReceived = true;
            PlayerPrefs.SetString(SetKey(quest,ReceiveKey), "true");
            questRewardPanel.SetActive(true);
            CalculateCoinReward(quest);
            CalculateUnitPieceReward(quest);
            QuestObject.UpdateQuestStates(quest);
            PlayerPrefs.Save();
            questRewardCloseBtn.onClick.AddListener(CloseReward);
        }
        private void CalculateCoinReward(QuestAssemble quest)
        {
            if (_coinObject != null)
            {
                Destroy(_coinObject.gameObject);
            }
            if (quest.coinValue == 0) return;
            _coinObject = Instantiate(rewardItem, questRewardContents.transform);
            _coinObject.goodsBack.GetComponent<Image>().sprite = _coinObject.coinSprite;
            _coinObject.goodsSprite.GetComponent<Image>().sprite = StoreMenu.Instance.specialOffer.coinSprite;
            _coinObject.goodsValue.text = quest.coinValue.ToString();
            CoinsScript.Instance.Coin += quest.coinValue;
        }
        private static int UnitPieceReceiveValue(CharacterBase.UnitGrades unitGrade, QuestAssemble quest)
        {
            return unitGrade switch
            {
                CharacterBase.UnitGrades.G => quest.greenValue,
                CharacterBase.UnitGrades.B => quest.blueValue,
                CharacterBase.UnitGrades.P => quest.purpleValue
            };
        }
        private void CalculateUnitPieceReward(QuestAssemble quest)
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
                { CharacterBase.UnitGrades.P, UnitPieceReceiveValue(CharacterBase.UnitGrades.P, quest) }
            };
        
            foreach (var grade in totalPiecesPerGrade.Keys)
            {
                var unitsOfThisGrade = selectedUnitIndices.Where(index =>
                    TimeRewardManager.Instance.unitList[index].UnitGrade == grade && TimeRewardManager.Instance.unitList[index].unitPieceLevel < 14).ToList();
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
                if (unit.unitPieceLevel >= 14) continue;
                unit.Initialize();
                _unitPieceReward = pieceCountPerUnit[index];
                if (_unitPieceReward == 0) continue;
                if (_unitPieceObject != null)
                {
                    Destroy(_unitPieceObject.gameObject);
                }
                _unitPieceObject = Instantiate(rewardItem, questRewardContents.transform);
                _unitPieceObject.goodsBack.GetComponent<Image>().color = Color.white;
                _unitPieceObject.goodsBack.GetComponent<Image>().sprite = unit.UnitGrade switch
                {
                    CharacterBase.UnitGrades.G => StoreMenu.Instance.gGradeSprite,
                    CharacterBase.UnitGrades.B => StoreMenu.Instance.bGradeSprite,
                    CharacterBase.UnitGrades.P => StoreMenu.Instance.pGradeSprite,
                };
                _unitPieceObject.goodsSprite.GetComponent<Image>().sprite = unit.GetSpriteForLevel(unit.unitPieceLevel);
                _unitPieceObject.goodsValue.text = $"{_unitPieceReward}";
                _unitPieceDict[unit] = _unitPieceReward;
            }
            foreach (var unitPiece in _unitPieceDict)
            {
                unitPiece.Key.UnitPieceCount += unitPiece.Value;
                PlayerPrefs.SetInt($"{unitPiece.Key.unitGroup}{CharacterBase.PieceKey}",unitPiece.Key.UnitPieceCount);
                HoldCharacterList.Instance.UpdateRewardPiece(unitPiece.Key);
            }
            _unitPieceDict.Clear();
        }
    }   
}
