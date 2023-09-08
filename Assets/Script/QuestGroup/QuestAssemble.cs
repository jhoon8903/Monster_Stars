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
        public QuestManager.QuestTypes QuestType { get; private set; }
        public QuestManager.QuestCondition QuestCondition { get; private set; }
        public int coinValue;
        public int greenValue;
        public int blueValue;
        public int purpleValue;
        public int questValue;
        public int questGoal;
        public bool isShuffled;
        public bool isCompleted;
        public bool isReceived;
        public string questKey;

        public static void InitializeFromData(QuestAssemble questAssemble, QuestManager.QuestData data)
        {
            const string completeKey = QuestManager.CompleteKey;
            const string receiveKey = QuestManager.ReceiveKey;
            const string shuffleKey = QuestManager.ShuffleKey;
            const string valueKey = QuestManager.ValueKey;
            questAssemble.QuestType = data.questType;
            questAssemble.QuestCondition = data.questCondition;
            questAssemble.questDesc.text = data.questDesc;
            questAssemble.questKey = data.questKey;
            questAssemble.questGoal = data.questGoal;
            questAssemble.questProgress.maxValue = questAssemble.questGoal;
            questAssemble.questValue = PlayerPrefs.GetInt(QuestManager.SetKey(questAssemble,valueKey), 0);
            questAssemble.questProgress.value = questAssemble.questValue;
            questAssemble.questProgressText.text = $"{questAssemble.questValue} / {questAssemble.questGoal}";
            questAssemble.coinValue = data.item1CoinValue;
            questAssemble.coinValueText.text = questAssemble.coinValue.ToString();
            questAssemble.greenValue = data.item2GreenPieceValue;
            questAssemble.greenPieceValueText.text = questAssemble.greenValue.ToString();
            questAssemble.blueValue = data.item3BluePieceValue;
            questAssemble.bluePieceValueText.text = questAssemble.blueValue.ToString();
            questAssemble.purpleValue = data.item4PurplePieceValue;
            questAssemble.purplePieceValueText.text = questAssemble.purpleValue.ToString();
            questAssemble.isReceived = bool.Parse(PlayerPrefs.GetString(QuestManager.SetKey(questAssemble,receiveKey),"false"));
            questAssemble.isCompleted = bool.Parse(PlayerPrefs.GetString(QuestManager.SetKey(questAssemble,completeKey),"false"));
            questAssemble.isShuffled = bool.Parse(PlayerPrefs.GetString(QuestManager.SetKey(questAssemble,shuffleKey),"false"));
            if (questAssemble.questValue < questAssemble.questGoal) return;
            questAssemble.isCompleted = true;
            PlayerPrefs.SetString(QuestManager.SetKey(questAssemble,completeKey), "true");
        }
    }
}
