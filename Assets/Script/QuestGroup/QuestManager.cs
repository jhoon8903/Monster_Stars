using System;
using System.Collections.Generic;
using System.Linq;
using Script.AdsScript;
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
        private const int MaxRotationQuests = 3;
        private const string QuestLoadKey = "SelectedRotationQuests";
        public static QuestManager Instance;
        private void Awake()
        {
            Instance = this;
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
                    fixQuestObject.receiveBtn.SetActive(true);
                    if (fixQuestObject.isCompleted)
                    {
                        fixQuestObject.receiveBtn.GetComponent<Button>().interactable = false;
                        fixQuestObject.receiveBtnText.text = "Completed";
                    }
                    else
                    {
                        fixQuestObject.receiveBtn.GetComponent<Button>().interactable = false;
                        fixQuestObject.receiveBtnText.text = "Proceeding";
                    }
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
                        rotationQuestObject.receiveBtn.SetActive(false);
                        rotationQuestObject.shuffleBtn.SetActive(true);
                        rotationQuestObject.shuffleBtn.GetComponent<Button>().onClick.AddListener(CallShuffleAds);
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
                PlayerPrefs.SetInt(quest.questType + "_isShuffled", quest.isShuffled ? 1 : 0);
                PlayerPrefs.SetInt(quest.questType + "_isCompleted", quest.isCompleted ? 1 : 0);
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
                var shuffledState = PlayerPrefs.GetInt(questType + "_isShuffled", 0);
                var completedState = PlayerPrefs.GetInt(questType + "_isCompleted", 0);
                questObject.isShuffled = (shuffledState == 1);
                questObject.isCompleted = (completedState == 1);
                questObject.questType = questType;
                questObject.questDesc.text = PlayerPrefs.GetString(questType + "_desc");
                questObject.questValue = PlayerPrefs.GetInt(questType + "_value");
                questObject.questGoal = PlayerPrefs.GetInt(questType + "_goal");
                questObject.questProgress.value = questObject.questValue;
                questObject.questProgress.maxValue = questObject.questGoal;
                questObject.questProgressText.text = $"{questObject.questValue} / {questObject.questGoal}";
                questObject.item1Value.text = PlayerPrefs.GetString(questType + "_item1Value");
                questObject.item2Value.text = PlayerPrefs.GetString(questType + "_item2Value");
                if (questObject.isCompleted)
                {
                    questObject.receiveBtn.GetComponent<Button>().interactable = false;
                    questObject.receiveBtnText.text = "Completed";
                    questObject.questValue = questObject.questGoal;
                    questObject.questProgress.value = questObject.questValue;
                    questObject.questProgressText.text = $"{questObject.questValue} / {questObject.questGoal}";
                }
                else if (questObject.isShuffled)
                {
                    questObject.shuffleBtn.SetActive(false);
                    questObject.receiveBtn.SetActive(true);
                    questObject.receiveBtn.GetComponent<Button>().interactable = false;
                    questObject.receiveBtnText.text = "Proceeding";
                }
                else
                {
                    questObject.shuffleBtn.SetActive(true);
                    questObject.receiveBtn.SetActive(false);
                }
                questObject.shuffleBtn.GetComponent<Button>().onClick.AddListener(CallShuffleAds);
                _rotationQuestList.Add(questObject);
            }
        }
        public void ShuffleQuest()
        {
            var activeRotationQuestTypes = _rotationQuestList.Select(q => q.questType).ToList();
            var csvFile = Resources.Load<TextAsset>("QuestData");
            var csvText = csvFile.text;
            var csvData = csvText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var inactiveRotationQuestData = new List<string[]>();

            for (var i = 1; i < csvData.Length; i++)
            {
                var line = csvData[i];
                if (string.IsNullOrEmpty(line)) continue;
                var data = line.Split(',');
                var questType = (QuestTypes)Enum.Parse(typeof(QuestTypes), data[0]);
                var condition = (QuestCondition)Enum.Parse(typeof(QuestCondition), data[1]);
                if (condition == QuestCondition.Rotation && !activeRotationQuestTypes.Contains(questType))
                {
                    if (PlayerPrefs.GetInt(questType + "_isCompleted", 0) == 0)
                    {
                        inactiveRotationQuestData.Add(data);
                    }
                }
            }

            var newQuestData = inactiveRotationQuestData.OrderBy(q => UnityEngine.Random.value).FirstOrDefault();

            if (newQuestData != null)
            {
                var newQuest = CreateQuestFromData(newQuestData);
                var questToRemove = _rotationQuestList.FirstOrDefault();
                if (questToRemove != null)
                {
                    _rotationQuestList.Remove(questToRemove);
                    Destroy(questToRemove.gameObject);
                }
                _rotationQuestList.Add(newQuest);
                SaveSelectedQuestList(_rotationQuestList); 
            }
        }
        private QuestObject CreateQuestFromData(IReadOnlyList<string> data)
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
            rotationQuestObject.isShuffled = true; 
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
            rotationQuestObject.receiveBtn.SetActive(true);
            rotationQuestObject.receiveBtn.GetComponent<Button>().interactable = false;
            rotationQuestObject.receiveBtnText.text = "Proceeding";
            return rotationQuestObject;
        }
        private static void CallShuffleAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.ShuffleQuest;
        }
        private void ReceiveQuestReward(QuestObject quest)
        {
            switch (quest.questType)
            {
                case QuestTypes.ViewAds:
                    break;
                case QuestTypes.AllClear:
                    break;
                case QuestTypes.UseCoin:
                    break;
                case QuestTypes.GetCoin:
                    break;
                case QuestTypes.OpenBox:
                    break;
                case QuestTypes.GetPiece:
                    break;
                case QuestTypes.KillEnemy:
                    break;
                case QuestTypes.KillBoss:
                    break;
                case QuestTypes.MatchCoin:
                    break;
                case QuestTypes.Victory:
                    break;
            }
           AllClearQuest();
           PlayerPrefs.SetInt(quest.questType + "_isCompleted", 1);
           quest.isCompleted = true;
           quest.receiveBtn.GetComponent<Button>().interactable = false;
           quest.receiveBtnText.text = "Completed";
           PlayerPrefs.Save();
        }
        private void UpdateQuest(QuestObject quest, int value)
        {
            if (quest != null)
            {
                quest.questValue += value;
                quest.questProgress.value = quest.questValue;
                quest.questProgressText.text = $"{quest.questValue} / {quest.questGoal}";

                if (quest.questValue >= quest.questGoal)
                {
                    quest.questValue = quest.questGoal;
                    quest.questProgressText.text = $"{quest.questValue} / {quest.questGoal}";
                    quest.shuffleBtn.SetActive(false);
                    quest.receiveBtn.SetActive(true);
                    quest.receiveBtnText.text = "Receive";
                    quest.receiveBtn.GetComponent<Button>().onClick.AddListener(()=>ReceiveQuestReward(quest));
                }
            }
        }

        // View Ads Quest (Fix)
        public void AdsViewQuest()
        {
            var adsViewQuest = _fixQuestList.FirstOrDefault(q => q.questType == QuestTypes.ViewAds);
            UpdateQuest(adsViewQuest, 1);
        }
        // All Clear Quest (Fix)
        private void AllClearQuest()
        {
            var allClearQuest = _fixQuestList.FirstOrDefault(q => q.questType == QuestTypes.AllClear);
            UpdateQuest(allClearQuest,1);
        }
        // Use Coin Quest (Fix)
        public void UseCoinQuest(int useCoin)
        {
            var fixQuest = _fixQuestList.FirstOrDefault(q => q.questType == QuestTypes.UseCoin);
            UpdateQuest(fixQuest, useCoin);
        }
        // Get Coin Quest (Fix)
        public void GetCoinQuest(int getCoin)
        {
            var getCoinQuest = _fixQuestList.FirstOrDefault(q => q.questType == QuestTypes.GetCoin);
            UpdateQuest(getCoinQuest, getCoin);
        }
        // Kill Enemy Quest (Rotation)
        public void KillEnemyQuest()
        {
            var killEnemyQuest = _rotationQuestList.FirstOrDefault(q => q.questType == QuestTypes.KillEnemy);
            UpdateQuest(killEnemyQuest, 1);
        }
        // Open Box (Store) Quest (Rotation)
        public void OpenBoxQuest()
        {
            var openBoxQuest = _rotationQuestList.FirstOrDefault(q => q.questType == QuestTypes.OpenBox);
            UpdateQuest(openBoxQuest, 1);
        }
        // Kill Boss Quest (Rotation)
        public void KillBossQuest()
        {
            var killBossQuest = _rotationQuestList.FirstOrDefault(q => q.questType == QuestTypes.KillBoss);
            UpdateQuest(killBossQuest, 1);
        }
        // Match Coin Quest (Rotation)
        public void MatchCoinQuest()
        {
            var matchCoinQuest = _rotationQuestList.FirstOrDefault(q => q.questType == QuestTypes.MatchCoin);
            UpdateQuest(matchCoinQuest, 1);
        }
        // Victory Quest (Rotation)
        public void VictoryQuest()
        {
            var victoryQuest = _rotationQuestList.FirstOrDefault(q => q.questType == QuestTypes.Victory);
            UpdateQuest(victoryQuest, 1);
        }
        // Get Piece Quest (Rotation)
        public void GetPieceQuest(int getPiece)
        {
            var getPieceQuest = _rotationQuestList.FirstOrDefault(q => q.questType == QuestTypes.GetPiece);
            UpdateQuest(getPieceQuest, getPiece);
        }
    }   
}
