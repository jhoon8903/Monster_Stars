using Script.AdsScript;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.UIManager
{
    public class GoodsPanelSpawner : MonoBehaviour
    {
        public GameObject goodsPanelPrefab;
        public int numberOfPanels = 3;
        public float initialPanelPosY = 205.0f; // 처음 생성되는 패널의 Y 위치
        public float panelSpacing = 400.0f; // 패널 사이의 간격
        public string[] goodsNames = { "COIN", "GEM", "STAMINA" }; // 각 굿즈 패널에 할당할 이름 배열


        private void Start()
        {
            var homeworkPanel = transform; // 홈워크 패널의 Transform 가져오기

            // 반복해서 굿즈 패널을 생성
            for (var i = 0; i < numberOfPanels; i++)
            {
                var spawnPosY = initialPanelPosY - i * panelSpacing;
                var spawnPosition = new Vector3(0, spawnPosY, 0); // 원하는 위치 계산
                var goodsPanel = Instantiate(goodsPanelPrefab, spawnPosition, Quaternion.identity);

                // 굿즈 패널을 홈워크 패널의 자식으로 설정
                goodsPanel.transform.SetParent(homeworkPanel, false);

                // 굿즈 패널의 자식 오브젝트들 중에서 TextMeshProUGUI 컴포넌트를 찾아서 텍스트를 변경
                var goodsText = goodsPanel.GetComponentInChildren<TextMeshProUGUI>();
                if (goodsText == null || i >= goodsNames.Length) continue;
                var newName = goodsNames[i]; // 이름 배열에서 가져옴
                goodsText.text = newName;

                // 굿즈 패널에 해당하는 버튼 이벤트를 설정
                var goodsButton = goodsPanel.GetComponent<Button>();
                if (goodsButton != null)
                {
                    // goodsButton.onClick.AddListener(() => OnGoodsButtonClicked(newName));
                }
            }
        }

        private void OnGoodsButtonClicked(string goodsName)
        {
            // 광고 이벤트 처리 또는 원하는 작업을 수행
            switch (goodsName)
            {
                case "COIN":
                    AdsManager.Instance.Coin();
                    break;
                case "GEM":
                    AdsManager.Instance.Gem();
                    break;
                case "STAMINA":
                    AdsManager.Instance.Stamina();
                    break;
            }
        }
    }
}