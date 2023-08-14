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
                            adsGoal = goal;
                            adsProgress.maxValue = adsGoal;
                            adsValue = PlayerPrefs.GetInt(questKey, 0);
                            adsProgress.value = adsValue;
                            adsProgressText.text = $"{adsValue} / {adsGoal}";
                            break;
                        case QuestTypes.AllClear:
                            allClearDesc.text = desc;
                            allClearGoal = goal;
                            allClearProgress.maxValue = allClearGoal;
                            allClearValue = PlayerPrefs.GetInt(questKey, 0);
                            allClearProgress.value = allClearValue;
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
                    fixQuestObject.questKey = questKey;
                    fixQuestObject.questGoal = PlayerPrefs.GetInt(fixQuestObject.questKey + "_goal", goal);
                    fixQuestObject.questProgress.maxValue = fixQuestObject.questGoal;
                    fixQuestObject.questValue = PlayerPrefs.GetInt(fixQuestObject.questKey + "_value", 0);
                    fixQuestObject.questProgress.value = fixQuestObject.questValue;
                    fixQuestObject.questProgressText.text = $"{fixQuestObject.questValue} / {fixQuestObject.questGoal}";
                    fixQuestObject.item1Value.text = item1RewardValue.ToString();
                    fixQuestObject.item2Value.text = (item2GreenPiece + item2BluePiece + item2PurplePiece).ToString();
                    fixQuestObject.receiveBtn.SetActive(true);
                    if (fixQuestObject.isCompleted && fixQuestObject.isReceived)
                    {
                        fixQuestObject.receiveBtn.GetComponent<Button>().interactable = false;
                        fixQuestObject.receiveBtnText.text = "Completed";
                        fixQuestObject.questValue = fixQuestObject.questGoal;
                        fixQuestObject.questProgress.value = fixQuestObject.questValue;
                        fixQuestObject.questProgressText.text = $"{fixQuestObject.questValue} / {fixQuestObject.questGoal}";

                    }
                    else if (fixQuestObject.isCompleted && !fixQuestObject.isReceived)
                    {
                        fixQuestObject.receiveBtn.GetComponent<Button>().interactable = true;
                        fixQuestObject.receiveBtnText.text = "Receive";
                        fixQuestObject.questValue = fixQuestObject.questGoal;
                        fixQuestObject.questProgress.value = fixQuestObject.questValue;
                        fixQuestObject.questProgressText.text = $"{fixQuestObject.questValue} / {fixQuestObject.questGoal}";
                        fixQuestObject.receiveBtn.GetComponent<Button>().onClick.AddListener(()=>ReceiveQuestReward(fixQuestObject));

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
                        rotationQuestObject.questKey = questKey; 
                        rotationQuestObject.questGoal = PlayerPrefs.GetInt( rotationQuestObject.questKey + "_goal", goal);
                        rotationQuestObject.questProgress.maxValue = rotationQuestObject.questGoal;
                        rotationQuestObject.questValue = PlayerPrefs.GetInt( rotationQuestObject.questKey + "_value", 0);
                        rotationQuestObject.questProgress.value = rotationQuestObject.questValue;
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
            rotationQuestObject.questGoal = PlayerPrefs.GetInt(questKey + "_goal", goal);
            rotationQuestObject.questProgress.maxValue = rotationQuestObject.questGoal;
            rotationQuestObject.questValue = PlayerPrefs.GetInt(questKey + "_value", 0);
            rotationQuestObject.questProgress.value = rotationQuestObject.questValue;
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
