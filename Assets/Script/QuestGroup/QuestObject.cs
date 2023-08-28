using System.Linq;
using Script.AdsScript;
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
        [SerializeField] public Button adsRewardBtn;
        public int adsValue;
        public int adsGoal;
        
        // AllClear Quest
        [SerializeField] public TextMeshProUGUI allClearDesc;
        [SerializeField] public Slider allClearProgress;
        [SerializeField] public TextMeshProUGUI allClearProgressText;
        [SerializeField] public Button allClearRewardBtn;
        public int allClearValue;
        public int allClearGoal;
        
        [SerializeField] internal QuestAssemble assemble;
        private QuestAssemble _questInstance;

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
        public void SpecialQuests(QuestManager.QuestTypes questType, QuestManager.QuestData data)
        {
            var desc = data.questDesc;
            var questGoal = PlayerPrefs.GetInt($"{questType}{QuestManager.GoalKey}", data.questGoal);
            PlayerPrefs.SetInt($"{questType}{QuestManager.GoalKey}", questGoal);
            var questValue = PlayerPrefs.GetInt( $"{questType}{QuestManager.ValueKey}", 0);
            var complete = bool.Parse(PlayerPrefs.GetString($"{questType}{QuestManager.CompleteKey}", "false"));
            var receive = bool.Parse(PlayerPrefs.GetString($"{questType}{QuestManager.ReceiveKey}", "false"));
            switch (questType)
            {
                case QuestManager.QuestTypes.ViewAds:
                    adsValue = questValue;
                    adsGoal = questGoal;
                    adsRewardBtn.interactable = receive switch
                    {
                        true => false,
                        false => complete switch
                        {
                            true => true,
                            false => false
                        }
                    };
                    SetSpecialQuest(adsDesc, adsProgress, adsProgressText, desc, adsGoal, adsValue);
                    break;
                case QuestManager.QuestTypes.AllClear:
                    allClearValue = questValue;
                    allClearGoal = questGoal;
                    allClearRewardBtn.interactable = receive switch
                    {
                        true => false,
                        false => complete switch
                        {
                            true => true,
                            false => false
                        }
                    };
                    SetSpecialQuest(allClearDesc, allClearProgress, allClearProgressText, desc, allClearGoal, allClearValue);
                    break;
            }
            PlayerPrefs.Save();
            adsRewardBtn.onClick.AddListener(()=>QuestManager.Instance.SpecialQuestReward(questType));
        }
        public static void InitSpecialQuest(QuestManager.QuestTypes questType)
        {
            PlayerPrefs.DeleteKey($"{questType}{QuestManager.ValueKey}");
            PlayerPrefs.DeleteKey($"{questType}{QuestManager.CompleteKey}");
            PlayerPrefs.DeleteKey($"{questType}{QuestManager.ReceiveKey}");
        }
        private static void SetSpecialQuest(TMP_Text descText, Slider progress, TMP_Text progressText, string desc, int goal, int value)
        {
            descText.text = desc;
            progress.maxValue = goal;
            progress.value = value;
            progressText.text = $"{value} / {goal}";
        }
        private void CoinQuestsCreate(QuestManager.QuestData data)
        {
            var questInstance = CreateQuestFromData(data);
            QuestManager.Instance.FixQuestList.Add(questInstance);
            QuestManager.Instance.questInstances.Add(questInstance);
            QuestManager.SaveQuest(QuestManager.Instance.FixQuestList.Concat(QuestManager.Instance.RotationQuestList));
        }
        public QuestAssemble RotationQuestCreate(QuestManager.QuestData data)
        {
            var questInstance = CreateQuestFromData(data);
            QuestManager.Instance.RotationQuestList.Add(questInstance);
            QuestManager.Instance.questInstances.Add(questInstance);
            QuestManager.Instance._rotationQuestCandidates.Remove(data);
            QuestManager.SaveQuest(QuestManager.Instance.FixQuestList.Concat(QuestManager.Instance.RotationQuestList));
            return questInstance;
        }
        private QuestAssemble CreateQuestFromData(QuestManager.QuestData data)
        {
            _questInstance = Instantiate(assemble, QuestManager.Instance.questTransform);
            QuestAssemble.InitializeFromData(_questInstance, data);
            SetActiveOrToggleParent(_questInstance.coinValueText.transform.parent.gameObject, data.item1CoinValue, _questInstance.coinValueText);
            SetActiveOrToggleParent(_questInstance.greenPieceValueText.transform.parent.gameObject, data.item2GreenPieceValue, _questInstance.greenPieceValueText);
            SetActiveOrToggleParent(_questInstance.bluePieceValueText.transform.parent.gameObject, data.item3BluePieceValue, _questInstance.bluePieceValueText);
            SetActiveOrToggleParent(_questInstance.purplePieceValueText.transform.parent.gameObject, data.item4PurplePieceValue, _questInstance.purplePieceValueText);
            UpdateQuestStates(_questInstance);
            return _questInstance;
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
        public static void UpdateQuestStates(QuestAssemble instanceObject)
        {
            if (instanceObject == null) return;
            instanceObject.shuffleBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            var receive = instanceObject.isReceived;
            var complete = instanceObject.isCompleted;
            var shuffle = instanceObject.isShuffled;
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
                            instanceObject.receiveBtn.GetComponent<Button>().onClick.AddListener(() => QuestManager.Instance.ReceiveQuestReward(instanceObject));
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
                                instanceObject.shuffleBtn.GetComponent<Button>().onClick.AddListener(() => AdsManager.Instance.CallShuffleAds(instanceObject));
                             
                            }
                            break;
                    }
                    break;
            }
        }
    }                           
}
