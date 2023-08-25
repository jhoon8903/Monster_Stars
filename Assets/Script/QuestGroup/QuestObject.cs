using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.QuestGroup
{
    public class QuestObject : MonoBehaviour
    {
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
        
        [SerializeField] internal QuestAssemble assemble;
        internal QuestAssemble QuestInstance;

        public void FixQuestCreate(QuestManager.QuestData data)
        {
            var questType = QuestManager.ParseQuestType(data.questType);
            if (questType is QuestManager.QuestTypes.ViewAds or QuestManager.QuestTypes.AllClear)
            {
                SpecialQuests(questType, data);
            }
            else
            {
                CoinQuestsCreate(data);
            }
        }
        private void SpecialQuests(QuestManager.QuestTypes questType, QuestManager.QuestData data)
        {
            var desc = data.questDesc;
            var questGoal = data.questGoal;
            var questKey = data.questKey;
            var questValue = PlayerPrefs.GetInt( questKey + "_value", 0);
            switch (questType)
            {
                case QuestManager.QuestTypes.ViewAds:
                    SetSpecialQuest(adsDesc, adsProgress, adsProgressText, desc, adsGoal, adsValue);
                    adsValue = questValue;
                    adsGoal = questGoal;
                    break;
                case QuestManager.QuestTypes.AllClear:
                    allClearValue = questValue;
                    allClearGoal = questGoal;
                    SetSpecialQuest(allClearDesc, allClearProgress, allClearProgressText, desc, allClearGoal, allClearValue);
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
        public void CoinQuestsCreate(QuestManager.QuestData data)
        {
            var questInstance = CreateQuestFromData(data);
            QuestManager.Instance.FixQuestList.Add(questInstance);
            QuestManager.Instance.questInstances.Add(questInstance);
            QuestManager.Instance.SaveQuest(QuestManager.Instance.FixQuestList);
        }
        public QuestAssemble RotationQuestCreate(QuestManager.QuestData data)
        {
            var questInstance = CreateQuestFromData(data);
            QuestManager.Instance.RotationQuestList.Add(questInstance);
            QuestManager.Instance.questInstances.Add(questInstance);
            QuestManager.Instance.SaveQuest(QuestManager.Instance.RotationQuestList);
            return questInstance;
        }
        public QuestAssemble CreateQuestFromData(QuestManager.QuestData data)
        {
            QuestInstance = Instantiate(assemble, QuestManager.Instance.questTransform);
            QuestAssemble.InitializeFromData(QuestInstance, data);
            SetActiveOrToggleParent(QuestInstance.coinValueText.transform.parent.gameObject, data.item1CoinValue, QuestInstance.coinValueText);
            SetActiveOrToggleParent(QuestInstance.greenPieceValueText.transform.parent.gameObject, data.item2GreenPieceValue, QuestInstance.greenPieceValueText);
            SetActiveOrToggleParent(QuestInstance.bluePieceValueText.transform.parent.gameObject, data.item3BluePieceValue, QuestInstance.bluePieceValueText);
            SetActiveOrToggleParent(QuestInstance.purplePieceValueText.transform.parent.gameObject, data.item4PurplePieceValue, QuestInstance.purplePieceValueText);
            UpdateQuestStates(QuestInstance);
            return QuestInstance;
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
        public void UpdateQuestStates(QuestAssemble instanceObject)
        {
            Debug.Log("QO "+ instanceObject.QuestType);
            instanceObject.shuffleBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            var receive = bool.Parse(PlayerPrefs.GetString(  instanceObject.questKey + "_isReceived", "false"));
            var complete = bool.Parse(PlayerPrefs.GetString(  instanceObject.questKey + "_isCompleted", "false"));
            var shuffle = bool.Parse(PlayerPrefs.GetString( instanceObject.questKey + "_isShuffled", "false"));
    
            switch (receive)
            {
                case true:
                    instanceObject.shuffleBtn.SetActive(false);
                    instanceObject.receiveBtn.SetActive(true);
                    instanceObject.receiveBtn.GetComponent<Button>().interactable = false;
                    instanceObject.receiveBtnText.text = "Completed";
                    break;
                case false :
                    switch (complete)
                    {
                        case true :
                            instanceObject.shuffleBtn.SetActive(false);
                            instanceObject.receiveBtn.SetActive(true);
                            instanceObject.receiveBtn.GetComponent<Button>().interactable = true;
                            instanceObject.receiveBtnText.text = "Receive";
                            // QuestInstance.receiveBtn.GetComponent<Button>().onClick.AddListener(() => QuestManager.Instance.ReceiveQuestReward(questObject));
                            break;
                        case false :
                            if ( instanceObject.QuestCondition == QuestManager.QuestCondition.Fix || shuffle)
                            {
                                instanceObject.shuffleBtn.SetActive(false);
                                instanceObject.receiveBtn.SetActive(true);
                                instanceObject.receiveBtn.GetComponent<Button>().interactable = false;
                                instanceObject.receiveBtnText.text = "Proceeding";
                            }
                            else
                            {
                                instanceObject.receiveBtn.SetActive(false);
                                instanceObject.shuffleBtn.SetActive(true);
                                instanceObject.shuffleBtn.GetComponent<Button>().interactable = true;
                                instanceObject.shuffleBtn.GetComponent<Button>().onClick.AddListener(() => QuestManager.Instance.CallShuffleAds(instanceObject));
                             
                            }
                            break;
                    }
                    break;
            }
        }
    }                           
}
