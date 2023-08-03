using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Script.RobbyScript.StoreMenuGroup;

namespace Script.RobbyScript.MainMenuGroup
{
    public class DailyQuestManager : MonoBehaviour
    {
        [SerializeField] private GameObject assemblePrefab; // Assemble 스크립트가 연결된 프리펩을 Inspector에서 할당해주세요.
        [SerializeField] private GameObject questPanel; // 패널
        //[SerializeField] private GameObject receiveBtn; // 보상 받기 버튼
        [SerializeField] private GameObject closeBtn; // 패널 닫기 버튼  
        [SerializeField] private Transform contentLayout; // Content Layout 오브젝트에 대한 참조
        [SerializeField] private int Questprefabs = 5;
        [SerializeField] private Quest quest;
        [SerializeField] private Assemble assembleScript;

        private void Start()
        {
            SetQuest();
            LoadQuestData();
        }

        private void SetQuest()
        {
            for (int i = 0; i < Questprefabs; i++)
            {
                GameObject instantiatedAssemblePrefab = Instantiate(assemblePrefab, contentLayout);

                if (quest != null && quest.quests.Count > i)
                {
                    Quest.QuestData questData = quest.quests[i];

                    // Assemble 프리펩의 내부 컴포넌트에 접근하여 퀘스트 정보 설정
                    Assemble assembleScript = instantiatedAssemblePrefab.GetComponent<Assemble>();
                    assembleScript.questDesc.text = questData.name;
                    assembleScript.progressText.text = questData.count.ToString(); 

                    // 이미지가 코인 이미지인 경우
                    if (assembleScript.cardImage)
                    {
                        // 가져온 값을 Assemble 스크립트의 텍스트에 적용
                        assembleScript.SetItemText(questData.coin.ToString(), 0);
                    }
                    // 이미지가 코인 이미지인 경우
                    if (assembleScript.cardImage)
                    {
                        // 가져온 값을 Assemble 스크립트의 텍스트에 적용
                        assembleScript.SetItemText(questData.total.ToString(), 1);
                    }
                }
            }
        }

        private void LoadQuestData()
        {
            if (quest != null)
            {
                List<Quest.QuestData> Quests = new List<Quest.QuestData>();
                foreach (var questData in quest.quests)
                {
                    Quests.Add(questData);
                }
            }
        }
    }
}
