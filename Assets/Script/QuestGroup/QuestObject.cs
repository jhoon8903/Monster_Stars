using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.QuestGroup
{
    public class QuestObject : MonoBehaviour
    {
        [SerializeField] private QuestManager questManager;
        [SerializeField] internal TextMeshProUGUI questDesc;
        [SerializeField] internal Slider questProgress;
        [SerializeField] internal TextMeshProUGUI questProgressText;
        [SerializeField] internal Image item1Sprite;
        [SerializeField] internal TextMeshProUGUI item1Value;
        [SerializeField] internal Image item2Sprite;
        [SerializeField] internal TextMeshProUGUI item2Value;
        [SerializeField] internal Image item3Sprite;
        [SerializeField] internal TextMeshProUGUI item3Value;
        [SerializeField] internal Image item4Sprite;
        [SerializeField] internal TextMeshProUGUI item4Value;
        [SerializeField] internal GameObject receiveBtn;
        [SerializeField] internal TextMeshProUGUI receiveBtnText;
        [SerializeField] internal GameObject shuffleBtn;
        public QuestManager.QuestTypes questType;
        public QuestManager.QuestCondition questCondition;
        public int questValue;
        public int questGoal;
        public bool isShuffled;
        public bool isCompleted;
        public bool isReceived;
        public string questKey;

        // Quest Prefabs
        [SerializeField] private QuestObject questPrefab;
        [SerializeField] private GameObject questTransform;

        public void LoadSelectedQuestList()
        {
            if (!PlayerPrefs.HasKey(QuestManager.QuestLoadKey)) return;
            var selectedQuestData = PlayerPrefs.GetString(QuestManager.QuestLoadKey);
            var quests = selectedQuestData.Split(',')
                .Select(q => q.Split(':'))
                .Select(parts => new { QuestType = parts[0], QuestKey = parts[1] });
            foreach (var questData in quests)
            {
                var type = (QuestManager.QuestTypes)Enum.Parse(typeof(QuestManager.QuestTypes), questData.QuestType);
                var key = questData.QuestKey;
                var questObject = Instantiate(questPrefab, questTransform.transform);
                var shuffledState = PlayerPrefs.GetInt(type + "_isShuffled", 0);
                var completedState = PlayerPrefs.GetInt(type + "_isCompleted", 0);
                var receivedState = PlayerPrefs.GetInt(type + "_isReceived", 0);
                questObject.isReceived = receivedState == 1;
                questObject.isShuffled = shuffledState == 1;
                questObject.isCompleted = completedState == 1;
                questObject.questType = type;
                questObject.questDesc.text = PlayerPrefs.GetString(type + "_desc");
                questObject.questKey = key;
                questObject.questGoal = PlayerPrefs.GetInt(key + "_goal");
                questObject.questValue = PlayerPrefs.GetInt(key + "_value");
                questObject.questProgress.maxValue = questObject.questGoal;
                questObject.questProgress.value = questObject.questValue;
                questObject.questProgressText.text = $"{questObject.questValue} / {questObject.questGoal}";
                questObject.item1Value.text = PlayerPrefs.GetString(type + "_item1Value");
                questObject.item2Value.text = PlayerPrefs.GetString(type + "_item2Value");
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
                    questObject.receiveBtn.GetComponent<Button>().onClick.AddListener(() => questManager.ReceiveQuestReward(questObject));
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
                questObject.shuffleBtn.GetComponent<Button>().onClick.AddListener(QuestManager.CallShuffleAds);
                questManager.RotationQuestList.Add(questObject);
            }
        }

        public QuestObject CreateQuestFromData(string[] data)
        {
            var questObject = Instantiate(questPrefab, questTransform.transform);
            questObject.questType = QuestManager.ParseQuestType(data[0]);
            questObject.questCondition = QuestManager.ParseQuestCondition(data[1]);
            questObject.questDesc.text = data[2];
            questObject.questKey = data[3];
            questObject.questGoal = PlayerPrefs.GetInt(questObject.questKey + "_goal", int.Parse(data[4]));
            questObject.questProgress.maxValue = questObject.questGoal;
            questObject.questValue = PlayerPrefs.GetInt(questObject.questKey + "_value", 0);
            questObject.questProgress.value = questObject.questValue;
            questObject.questProgressText.text = $"{questObject.questValue} / {questObject.questGoal}";
            var item1RewardValue = int.Parse(data[5]);
            var item2RewardValue = int.Parse(data[6]);
            var itemReward3Value = int.Parse(data[7]);
            var item4RewardValue = int.Parse(data[8]);
            SetActiveOrToggleParent(questObject.item1Value.transform.parent.gameObject, item1RewardValue, questObject.item1Value);
            SetActiveOrToggleParent(questObject.item2Value.transform.parent.gameObject, item2RewardValue, questObject.item2Value);
            SetActiveOrToggleParent(questObject.item3Value.transform.parent.gameObject, itemReward3Value, questObject.item3Value);
            SetActiveOrToggleParent(questObject.item4Value.transform.parent.gameObject, item4RewardValue, questObject.item4Value);

            return questObject;
        }

        public static void SetQuestButtonStates(QuestObject questObject)
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

        private static void SetActiveOrToggleParent(GameObject parentObject, int value, TextMeshProUGUI textUI)
        {
            if (value != 0)
            {
                textUI.text = value.ToString();
                parentObject.SetActive(true); // 부모를 활성화
            }
            else
            {
                parentObject.SetActive(false); // 부모를 비활성화
            }
        }
    }                           
}
