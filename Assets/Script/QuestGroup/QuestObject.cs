using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.QuestGroup
{
    public class QuestObject : MonoBehaviour
    {
        [SerializeField] internal TextMeshProUGUI questDesc;
        [SerializeField] internal Slider questProgress;
        [SerializeField] internal TextMeshProUGUI questProgressText; 
        [SerializeField] internal TextMeshProUGUI coinValue;
        [SerializeField] internal TextMeshProUGUI greenPieceValue;
        [SerializeField] internal TextMeshProUGUI bluePieceValue;
        [SerializeField] internal TextMeshProUGUI purplePieceValue;
        [SerializeField] internal GameObject receiveBtn;
        [SerializeField] internal TextMeshProUGUI receiveBtnText;
        [SerializeField] internal GameObject shuffleBtn;
        public QuestManager.QuestTypes QuestType { get; private set; }
        private QuestManager.QuestCondition QuestCondition { get; set; }
        public int questValue;
        public int questGoal;
        public bool isShuffled;
        public bool isCompleted;
        public bool isReceived;
        public string questKey;

        public void HandleQuest(QuestManager.QuestCondition targetCondition, string[] data, QuestManager.QuestTypes questType, ICollection<string[]> rotationQuestData)
        {
            if (targetCondition == QuestManager.QuestCondition.Rotation)
            {
                rotationQuestData?.Add(data);
            }
            else if (questType is QuestManager.QuestTypes.ViewAds or QuestManager.QuestTypes.AllClear)
            {
                HandleSpecialQuests(questType, data);
            }
            else
            {
                CreateAndAddQuest(data);
            }
        }
        private static void HandleSpecialQuests(QuestManager.QuestTypes questType, IReadOnlyList<string> data)
        {
            var desc = data[2];
            var goal = int.Parse(data[4]);
            var questKey = data[3];
            var value = PlayerPrefs.GetInt(questKey + "_value", 0);
            switch (questType)
            {
                case QuestManager.QuestTypes.ViewAds:
                    SetSpecialQuest(QuestManager.Instance.adsDesc, QuestManager.Instance.adsProgress, QuestManager.Instance.adsProgressText, desc, goal, value);
                    break;
                case QuestManager.QuestTypes.AllClear:
                    SetSpecialQuest(QuestManager.Instance.allClearDesc, QuestManager.Instance.allClearProgress, QuestManager.Instance.allClearProgressText, desc, goal, value);
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
        private void CreateAndAddQuest(string[] data)
        {
            var questObject = CreateQuestFromData(data);
            SetQuestButtonStates(questObject);
            QuestManager.Instance.FixQuestList.Add(questObject);
        }
        public QuestObject CreateQuestFromData(string[] data)
        {
            var questObject = Instantiate(this, QuestManager.Instance.questTransform);
            questObject.QuestType = QuestManager.ParseQuestType(data[0]);
            questObject.QuestCondition = QuestManager.ParseQuestCondition(data[1]);
            questObject.questDesc.text = data[2];
            questObject.questKey = data[3];
            questObject.questGoal = PlayerPrefs.GetInt(questObject.questKey + "_goal", int.Parse(data[4]));
            questObject.questProgress.maxValue = questObject.questGoal;
            questObject.questValue = PlayerPrefs.GetInt(questObject.questKey + "_value", 0);
            questObject.questProgress.value = questObject.questValue;
            questObject.questProgressText.text = $"{questObject.questValue} / {questObject.questGoal}";
            var item1CoinValue = int.Parse(data[5]);
            var item2GreenPieceValue = int.Parse(data[6]);
            var item3BluePieceValue = int.Parse(data[7]);
            var item4PurplePieceValue = int.Parse(data[8]);
            SetActiveOrToggleParent(questObject.coinValue.transform.parent.gameObject, item1CoinValue, questObject.coinValue);
            SetActiveOrToggleParent(questObject.greenPieceValue.transform.parent.gameObject, item2GreenPieceValue, questObject.greenPieceValue);
            SetActiveOrToggleParent(questObject.bluePieceValue.transform.parent.gameObject, item3BluePieceValue, questObject.bluePieceValue);
            SetActiveOrToggleParent(questObject.purplePieceValue.transform.parent.gameObject, item4PurplePieceValue, questObject.purplePieceValue);
            return questObject;
        }
        public static void SetQuestButtonStates(QuestObject questObject)
        { 
            switch (questObject.isReceived)
            {
                case true:
                    SetButtonState(questObject, false, "Completed", questObject.questValue);
                    break;
                case false:
                    switch (questObject.isCompleted)
                    {
                        case true:
                            SetButtonState(questObject, true, "Receive", questObject.questValue);
                            break;
                        case false:
                            switch (questObject.isShuffled)
                            {
                                case true:
                                    SetButtonState(questObject, false, "Receive", questObject.questValue);
                                    break;
                                case false when questObject.QuestCondition != QuestManager.QuestCondition.Fix:
                                    SetButtonState(questObject, true, "Shuffle", questObject.questValue);
                                    break;
                                case false when questObject.QuestCondition == QuestManager.QuestCondition.Fix:
                                    SetButtonState(questObject, false, "Receive", questObject.questValue);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
        private static void SetButtonState(QuestObject questObject, bool interactable, string text, int value)
        {
            questObject.questValue = value;
            questObject.questProgress.value = questObject.questValue;
            questObject.questProgressText.text = $"{questObject.questValue} / {questObject.questGoal}";
            if (text != "Shuffle")
            {
                questObject.receiveBtn.GetComponent<Button>().interactable = interactable;
                questObject.receiveBtnText.text = text;
                if (interactable)
                {
                    questObject.receiveBtn.GetComponent<Button>().onClick.AddListener(() => QuestManager.Instance.ReceiveQuestReward(questObject));
                }
            }
            else
            {
                if (questObject.QuestCondition == QuestManager.QuestCondition.Fix) return;
                questObject.shuffleBtn.SetActive(true);
                questObject.receiveBtn.SetActive(false);
                questObject.shuffleBtn.GetComponent<Button>().interactable = true;
                questObject.shuffleBtn.GetComponent<Button>().onClick.AddListener(QuestManager.CallShuffleAds);
            }
        }
        private static void SetActiveOrToggleParent(GameObject parentObject, int value, TMP_Text textUI)
        {
            if (value != 0)
            {
                textUI.text = value.ToString();
                parentObject.SetActive(true);
            }
            else
            {
                parentObject.SetActive(false);
            }
        }
    }                           
}
