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
        [SerializeField] private GameObject questRewardPanel;

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
        private const string FixQuestLoadKey = "FixQuestLoadKey";
        private const string RotationQuestLoadKey = "RotationQuestLoadKey";
        public static QuestManager Instance;
        private string[] _csvData;
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
            foreach (var quest in _fixQuestList)
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
            foreach (var quest in _rotationQuestList)
            {
                quest.questValue = 0;
                PlayerPrefs.SetInt(quest.questKey + "_value", 0);
                quest.questProgress.value = 0;
                quest.questProgressText.text = $"0 / {quest.questGoal}";
                quest.isCompleted = false;
                PlayerPrefs.SetInt(quest.questType + "_isCompleted", 0);
            }
            ShuffleQuest();
            SaveSelectedQuestList(_rotationQuestList);
            PlayerPrefs.Save();
        }
        private static QuestTypes ParseQuestType(string type) => (QuestTypes)Enum.Parse(typeof(QuestTypes), type);
        private static QuestCondition ParseQuestCondition(string condition) => (QuestCondition)Enum.Parse(typeof(QuestCondition), condition);
        private void HandleQuest(QuestCondition targetCondition, string[] data, QuestTypes questType, ICollection<string[]> rotationQuestData)
        {
            if (targetCondition == QuestCondition.Rotation)
                rotationQuestData?.Add(data);
            else if (questType is QuestTypes.ViewAds or QuestTypes.AllClear)
                HandleSpecialQuests(questType, data);
            else
                CreateAndAddQuest(data);
        }
        private void CreateAndAddQuest(IReadOnlyList<string> data)
        {
            var questObject = CreateQuestFromData(data);
            questObject.receiveBtn.SetActive(true);
            SetQuestButtonStates(questObject);
            _fixQuestList.Add(questObject);
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
        private static void SetQuestButtonStates(QuestObject questObject)
        {
            switch (questObject.isCompleted)
            {
                case true when questObject.isReceived:
                    SetButtonState(questObject, false, "Completed", questObject.questGoal);
                    break;
                case true when !questObject.isReceived:
                    SetButtonState(questObject, true, "Receive", questObject.questGoal);
                    break;
                default:
                    questObject.receiveBtn.GetComponent<Button>().interactable = false;
                    break;
            }
        }
        private static void SetButtonState(QuestObject questObject, bool interactable, string text, int value)
        {
            questObject.receiveBtn.GetComponent<Button>().interactable = interactable;
            questObject.receiveBtnText.text = text;
            questObject.questValue = value;
            questObject.questProgress.value = questObject.questValue;
            questObject.questProgressText.text = $"{questObject.questValue} / {questObject.questGoal}";
        }
        private void HandleRotationQuest(QuestCondition targetCondition, IReadOnlyCollection<string[]> rotationQuestData)
        {
            if (targetCondition != QuestCondition.Rotation) return;
            if (rotationQuestData == null) return;
            var selectedQuestData = rotationQuestData.OrderBy(_ => UnityEngine.Random.value).Take(MaxRotationQuests).ToList();
            foreach (var data in selectedQuestData) CreateAndAddRotationQuest(data);
            SaveSelectedQuestList(_rotationQuestList);
        }
        private void CreateAndAddRotationQuest(IReadOnlyList<string> data)
        {
            var rotationQuestObject = CreateQuestFromData(data);
            rotationQuestObject.receiveBtn.SetActive(false);
            rotationQuestObject.shuffleBtn.SetActive(true);
            rotationQuestObject.shuffleBtn.GetComponent<Button>().onClick.AddListener(CallShuffleAds);
            _rotationQuestList.Add(rotationQuestObject);
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
                PlayerPrefs.SetInt(quest.questType + "_isShuffled", quest.isShuffled ? 1 : 0);
                PlayerPrefs.SetInt(quest.questType + "_isCompleted", quest.isCompleted ? 1 : 0);
                PlayerPrefs.SetInt(quest.isReceived + "_isReceived", quest.isReceived ? 1 : 0);
            }
            PlayerPrefs.Save();
        }
        private void LoadSelectedQuestList()
        {
            if (!PlayerPrefs.HasKey(QuestLoadKey)) return;
            var selectedQuestData = PlayerPrefs.GetString(QuestLoadKey);
            var quests = selectedQuestData.Split(',')
                .Select(q => q.Split(':'))
                .Select(parts => new { QuestType = parts[0], QuestKey = parts[1] });
            foreach (var questData in quests)
            {
                var questType = (QuestTypes)Enum.Parse(typeof(QuestTypes), questData.QuestType);
                var questKey = questData.QuestKey;
                var questObject = Instantiate(questPrefab, questTransform.transform);
                var shuffledState = PlayerPrefs.GetInt(questType + "_isShuffled", 0);
                var completedState = PlayerPrefs.GetInt(questType + "_isCompleted", 0);
                var receivedState = PlayerPrefs.GetInt(questType + "_isReceived", 0);
                questObject.isReceived = receivedState == 1;
                questObject.isShuffled = shuffledState == 1;
                questObject.isCompleted = completedState == 1;
                questObject.questType = questType;
                questObject.questDesc.text = PlayerPrefs.GetString(questType + "_desc");
                questObject.questKey = questKey;
                questObject.questGoal = PlayerPrefs.GetInt(questKey + "_goal");
                questObject.questValue = PlayerPrefs.GetInt(questKey + "_value");
                questObject.questProgress.maxValue = questObject.questGoal;
                questObject.questProgress.value = questObject.questValue;
                questObject.questProgressText.text = $"{questObject.questValue} / {questObject.questGoal}";
                questObject.item1Value.text = PlayerPrefs.GetString(questType + "_item1Value");
                questObject.item2Value.text = PlayerPrefs.GetString(questType + "_item2Value");
                if (questObject.isCompleted && questObject.isReceived)
                {
                    questObject.shuffleBtn.SetActive(false);
                    questObject.receiveBtn.SetActive(true);
                    questObject.receiveBtn.GetComponent<Button>().interactable = false;
                    questObject.receiveBtnText.text = "Completed";
                    questObject.questValue = questObject.questGoal;
                    questObject.questProgress.value = questObject.questValue;
                    questObject.questProgressText.text = $"{questObject.questValue} / {questObject.questGoal}";
                }
                else if (questObject.isCompleted && !questObject.isReceived)
                {
                    questObject.shuffleBtn.SetActive(false);
                    questObject.receiveBtn.SetActive(true);
                    questObject.receiveBtn.GetComponent<Button>().interactable = true;
                    questObject.receiveBtnText.text = "Receive";
                    questObject.questValue = questObject.questGoal;
                    questObject.questProgress.value = questObject.questValue;
                    questObject.questProgressText.text = $"{questObject.questValue} / {questObject.questGoal}";
                    questObject.receiveBtn.GetComponent<Button>().onClick.AddListener(() => ReceiveQuestReward(questObject));
                }
                else if (questObject.isShuffled && !questObject.isCompleted && !questObject.isReceived)
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
        private void UpdateQuest(QuestObject quest, int value)
        {
            if (_fixQuestList.Contains(quest) || _rotationQuestList.Contains(quest))
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
                if (_fixQuestList.Contains(quest))
                {
                    SaveFixQuestList(_fixQuestList);
                }
                else if (_rotationQuestList.Contains(quest))
                {
                    SaveRotationQuestList(_rotationQuestList);
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
                PlayerPrefs.SetInt(quest.questType + "_isShuffled", quest.isShuffled ? 1 : 0);
                PlayerPrefs.SetInt(quest.questType + "_isCompleted", quest.isCompleted ? 1 : 0);
                PlayerPrefs.SetInt(quest.questType + "_isReceived", quest.isReceived ? 1 : 0);
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
        private QuestObject CreateQuestFromData(IReadOnlyList<string> data)
        {
            var questObject = Instantiate(questPrefab, questTransform.transform);
            questObject.questType = ParseQuestType(data[0]);
            questObject.questCondition = ParseQuestCondition(data[1]);
            questObject.questDesc.text = data[2];
            questObject.questKey = data[3];
            questObject.questGoal = PlayerPrefs.GetInt(questObject.questKey + "_goal", int.Parse(data[4]));
            questObject.questProgress.maxValue = questObject.questGoal;
            questObject.questValue = PlayerPrefs.GetInt(questObject.questKey + "_value", 0);
            questObject.questProgress.value = questObject.questValue;
            questObject.questProgressText.text = $"{questObject.questValue} / {questObject.questGoal}";
            questObject.item1Value.text = data[5];
            questObject.item2Value.text = (int.Parse(data[6]) + int.Parse(data[7]) + int.Parse(data[8])).ToString();

            return questObject;
        }
        private static void CallShuffleAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.ShuffleQuest;
        }
        private void ReceiveQuestReward(QuestObject quest)
        {
            questRewardPanel.SetActive(true);
            // switch (quest.questType)
            // {
            //     case QuestTypes.ViewAds:
            //         CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
            //
            //         break;
            //     case QuestTypes.AllClear:
            //         CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
            //         break;
            //     case QuestTypes.UseCoin:
            //         CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
            //         break;
            //     case QuestTypes.GetCoin:
            //         CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
            //         break;
            //     case QuestTypes.OpenBox:
            //         CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
            //         break;
            //     case QuestTypes.GetPiece:
            //         CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
            //         break;
            //     case QuestTypes.KillEnemy:
            //         CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
            //         break;
            //     case QuestTypes.KillBoss:
            //         CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
            //         break;
            //     case QuestTypes.MatchCoin:
            //         CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
            //         break;
            //     case QuestTypes.Victory:
            //         CoinsScript.Instance.Coin += int.Parse(quest.item1Value.text);
            //         break;
            // }
           AllClearQuest();
           quest.isReceived = true;
           PlayerPrefs.SetInt(quest.questType + "isReceived", 1);
           quest.receiveBtn.GetComponent<Button>().interactable = false;
           quest.receiveBtnText.text = "Completed";
           if (_fixQuestList.Contains(quest))
           {
               SaveFixQuestList(_fixQuestList);
           }
           else if (_rotationQuestList.Contains(quest))
           {
               SaveRotationQuestList(_rotationQuestList);
           }
           PlayerPrefs.Save();
        }
        // View Ads Quest (Fix)
        public void AdsViewQuest()
        {
            Debug.Log("광고봄");
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
            Debug.Log("상자열림");
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
            Debug.Log("getPiece :"+getPiece);
            var getPieceQuest = _rotationQuestList.FirstOrDefault(q => q.questType == QuestTypes.GetPiece);
            UpdateQuest(getPieceQuest, getPiece);
        }
    }   
}
