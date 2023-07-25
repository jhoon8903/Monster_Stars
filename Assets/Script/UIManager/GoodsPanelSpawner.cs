using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Script.AdsScript;

public class GoodsPanelSpawner : MonoBehaviour
{
    public GameObject goodsPanelPrefab;
    public int numberOfPanels = 3;
    public float initialPanelPosY = 205.0f; // 처음 생성되는 패널의 Y 위치
    public float panelSpacing = 400.0f; // 패널 사이의 간격
    public AdsManager adsManager;

    public string[] goodsNames = { "COIN", "GEM", "STAMINA" }; // 각 굿즈 패널에 할당할 이름 배열


    private void Start()
    {
        Transform homeworkPanel = transform; // 홈워크 패널의 Transform 가져오기

        // 반복해서 굿즈 패널을 생성
        for (int i = 0; i < numberOfPanels; i++)
        {
            float spawnPosY = initialPanelPosY - i * panelSpacing;
            Vector3 spawnPosition = new Vector3(0, spawnPosY, 0); // 원하는 위치 계산
            GameObject goodsPanel = Instantiate(goodsPanelPrefab, spawnPosition, Quaternion.identity);

            // 굿즈 패널을 홈워크 패널의 자식으로 설정
            goodsPanel.transform.SetParent(homeworkPanel, false);

            // 굿즈 패널의 자식 오브젝트들 중에서 TextMeshProUGUI 컴포넌트를 찾아서 텍스트를 변경
            TextMeshProUGUI goodsText = goodsPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (goodsText != null && i < goodsNames.Length)
            {
                string newName = goodsNames[i]; // 이름 배열에서 가져옴
                goodsText.text = newName;

                // 굿즈 패널에 해당하는 버튼 이벤트를 설정
                Button goodsButton = goodsPanel.GetComponent<Button>();
                if (goodsButton != null)
                {
                    goodsButton.onClick.AddListener(() => OnGoodsButtonClicked(newName));
                }
            }
        }
    }

    private void OnGoodsButtonClicked(string goodsName)
    {
        // 광고 이벤트 처리 또는 원하는 작업을 수행
        switch (goodsName)
        {
            case "COIN":
                adsManager.Coin();
                break;
            case "GEM":
                adsManager.Gem();
                break;
            case "STAMINA":
                adsManager.Stamina();
                break;
            default:
                // 기본적으로 처리할 내용
                break;
        }
    }
}