using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace Script.RobbyScript.MainMenuGroup
{
    public class DailyQuestManager : MonoBehaviour
    {
        [SerializeField] private GameObject questPanel; // 패널
        //[SerializeField] private GameObject receiveBtn; // 보상 받기 버튼
        [SerializeField] private GameObject questContents; // 프리펩 
        [SerializeField] private GameObject closeBtn; // 패널 닫기 버튼  
        [SerializeField] private Transform contentLayout; // Content Layout 오브젝트에 대한 참조

        private void Start()
        {
            const int numberOfQuestPanels = 5;
            List<string> questDescriptions = GetRandomQuestDescriptions(numberOfQuestPanels - 2);
            questDescriptions.Insert(0, "골드 소모");
            questDescriptions.Insert(1, "골드 획득");

            for (int i = 0; i < numberOfQuestPanels; i++)
            {
                GameObject instantiatedQuestPanel = Instantiate(questContents, contentLayout);

                // 프리펩 내부의 모든 TextMeshProUGUI 컴포넌트를 찾음
                TextMeshProUGUI[] Assemble = instantiatedQuestPanel.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var questDesc in Assemble)
                {
                    // 컴포넌트의 이름이 "QuestDesc"인 경우에만 텍스트를 변경
                    if (questDesc.gameObject.name == "QuestDesc")
                    {
                        // 게임 요구 사항에 따라 텍스트 내용을 수정
                        questDesc.SetText(questDescriptions[i]);
                        break; // 원하는 컴포넌트를 찾았으므로 루프 종료
                    }
                }
                foreach (var progressText in Assemble)
                {
                    // 컴포넌트의 이름이 "QuestDesc"인 경우에만 텍스트를 변경
                    if (progressText.gameObject.name == "ProgressText")
                    {
                        // 게임 요구 사항에 따라 텍스트 내용을 수정
                        Debug.Log("progressText : "+progressText);
                        break; // 원하는 컴포넌트를 찾았으므로 루프 종료
                    }
                }
            }
        }

        // 랜덤한 요소를 반환하는 함수
        private List<string> GetRandomQuestDescriptions(int count)
        {
            List<string> questDescriptions = new List<string>();
            List<string> availableQuests = new List<string>
            {
                "보물상자(상점) 오픈",
                "카드 획득하기",
                "적을 처치하기",
                "보스 처치하기",
                "광고 시청하기",
                "스테이지에서 상자 합성하기",
                "전투에서 승리하기"
            };

            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random.Range(0, availableQuests.Count);
                questDescriptions.Add(availableQuests[randomIndex]);
                availableQuests.RemoveAt(randomIndex);
            }

            return questDescriptions;
        }
    }
}
