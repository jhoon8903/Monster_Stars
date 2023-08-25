using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.QuestGroup
{
    public class QuestAssemble : MonoBehaviour
    {
        [SerializeField] internal TextMeshProUGUI questDesc;
        [SerializeField] internal Slider questProgress;
        [SerializeField] internal TextMeshProUGUI questProgressText; 
        [SerializeField] internal TextMeshProUGUI coinValueText;
        [SerializeField] internal TextMeshProUGUI greenPieceValueText;
        [SerializeField] internal TextMeshProUGUI bluePieceValueText;
        [SerializeField] internal TextMeshProUGUI purplePieceValueText;
        [SerializeField] internal GameObject receiveBtn;
        [SerializeField] internal TextMeshProUGUI receiveBtnText;
        [SerializeField] internal GameObject shuffleBtn;
        public QuestManager.QuestTypes QuestType { get; set; }
        public QuestManager.QuestCondition QuestCondition { get; set; }
        public int questValue;
        public int questGoal;
        public bool isShuffled;
        public bool isCompleted;
        public bool isReceived;
        public string questKey;

        public static void InitializeFromData(QuestAssemble questAssemble, QuestManager.QuestData data)
        {
            var completeKey = QuestManager.Instance.completeKey;
            var receiveKey = QuestManager.Instance.receiveKey;
            var shuffleKey = QuestManager.Instance.shuffleKey;
            var valueKey = QuestManager.Instance.valueKey;
            questAssemble.QuestType = (QuestManager.QuestTypes)Enum.Parse(typeof(QuestManager.QuestTypes), data.questType);
            questAssemble.QuestCondition = (QuestManager.QuestCondition)Enum.Parse(typeof(QuestManager.QuestCondition), data.questCondition);
            questAssemble.questDesc.text = data.questDesc;
            questAssemble.questKey = data.questKey;
            questAssemble.questGoal = data.questGoal;
            questAssemble.questProgress.maxValue = questAssemble.questGoal;
            questAssemble.questValue = data.questValue;
            questAssemble.questProgress.value = PlayerPrefs.GetInt(QuestManager.SetKey(questAssemble,valueKey), 0);
            questAssemble.questProgressText.text = $"{questAssemble.questValue} / {questAssemble.questGoal}";
            questAssemble.coinValueText.text = data.item1CoinValue.ToString();
            questAssemble.greenPieceValueText.text = data.item2GreenPieceValue.ToString();
            questAssemble.bluePieceValueText.text = data.item3BluePieceValue.ToString();
            questAssemble.purplePieceValueText.text = data.item4PurplePieceValue.ToString();
            questAssemble.isReceived = bool.Parse(PlayerPrefs.GetString(QuestManager.SetKey(questAssemble,receiveKey),"false"));
            questAssemble.isCompleted = bool.Parse(PlayerPrefs.GetString(QuestManager.SetKey(questAssemble,completeKey),"false"));
            questAssemble.isShuffled = bool.Parse(PlayerPrefs.GetString(QuestManager.SetKey(questAssemble,shuffleKey),"false"));
        }
    }
}
