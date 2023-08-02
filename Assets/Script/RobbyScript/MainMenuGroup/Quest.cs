using UnityEngine;
using System.Collections.Generic;

namespace Script.RobbyScript.MainMenuGroup
{
    public class Quest : MonoBehaviour
    {
        [System.Serializable]
        public struct QuestData
        {
            public string name;
            public int count;
            public int coin;
            public int green;
            public int blue;
            public int purple;
            public int total;
            public string condition;
            public int num;
        }

        public List<QuestData> quests = new List<QuestData>();

        private void Awake()
        {
            LoadQuestData();
        }

        private void Start()
        {
            DisplayQuests();
        }

        private void LoadQuestData()
        {
            var csvFile = Resources.Load<TextAsset>("questData");
            string csvText = csvFile.text;
            string[] lines = csvText.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] data = line.Split(',');
                if (data.Length != 9)
                {
                    Debug.LogWarning("Invalid CSV line: " + line);
                    continue;
                }

                QuestData quest = new QuestData
                {
                    name = data[0],
                    count = int.Parse(data[1]),
                    coin = int.Parse(data[2]),
                    green = int.Parse(data[3]),
                    blue = int.Parse(data[4]),
                    purple = int.Parse(data[5]),
                    total = int.Parse(data[6]),
                    condition = data[7],
                    num = int.Parse(data[8])
                };

                quests.Add(quest);
            }
        }

        private void DisplayQuests()
        {
            foreach (QuestData quest in quests)
            {
                //Debug.Log("Name: " + quest.name);
                //Debug.Log("Count: " + quest.count);
                //Debug.Log("Coin: " + quest.coin);
                //Debug.Log("Green: " + quest.green);
                //Debug.Log("Blue: " + quest.blue);
                //Debug.Log("Purple: " + quest.purple);
                //Debug.Log("Condition: " + quest.condition);
            }
        }
    }
}