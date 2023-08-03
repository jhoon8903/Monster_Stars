using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.QuestGroup
{ 
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private GameObject questPrefab;
        [SerializeField] private GameObject questContentsLayer;
        [SerializeField] private GameObject allClearQuestLayer;
        public List<QuestObject> questList = new List<QuestObject>();
        private void Awake()
        {
            LoadQuestData();
        }
        private void LoadQuestData()
        {
            var csvFile = Resources.Load<TextAsset>($"QuestData");
            var csvText = csvFile.text;
            var csvData = csvText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var fixQuests = new List<QuestObject>();
            var rotationQuests = new List<QuestObject>();
            for (var i = 1; i < csvData.Length; i++)
            { 
                var questData = csvData[i].Split(',');
                var questInstance = Instantiate(questPrefab);
                var quest = questInstance.GetComponent<QuestObject>();
                quest.questType = (QuestObject.QuestTypes)Enum.Parse(typeof(QuestObject.QuestTypes), questData[0].Trim());
                quest.questGoal = int.Parse(questData[1]);
                quest.rewardCoin = int.Parse(questData[2]);
                quest.rewardGreenPiece = int.Parse(questData[3]);
                quest.rewardBluePiece = int.Parse(questData[4]);
                quest.rewardPurplePiece = int.Parse(questData[5]);
                quest.questCondition = (QuestObject.QuestConditions)Enum.Parse(typeof(QuestObject.QuestConditions), questData[6].Trim());
                quest.questDescKor = questData[7];
                quest.questDescEng = questData[8];
                if (quest.questCondition == QuestObject.QuestConditions.Fix)
                {
                    fixQuests.Add(quest);
                }
                else
                {
                    rotationQuests.Add(quest);
                }
            }
            foreach (var quest in fixQuests)
                questList.Add(quest);

            for (var i = 0; i < Math.Min(3, rotationQuests.Count); i++)
            {
                var randomIndex = UnityEngine.Random.Range(0, rotationQuests.Count);
                questList.Add(rotationQuests[randomIndex]);
                rotationQuests.RemoveAt(randomIndex);
            }

            foreach (var quest in questList)
            {
                var parentTransform = quest.questType == QuestObject.QuestTypes.AllClear ? allClearQuestLayer.transform : questContentsLayer.transform;
                quest.transform.SetParent(parentTransform, false);
            }
        }
    }
}
