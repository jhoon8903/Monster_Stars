using Script.RobbyScript.StoreMenuGroup;
using Script.RobbyScript.TopMenuGroup;
using Script.WeaponScriptGroup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.MainMenuGroup
{
    public class Assemble : MonoBehaviour
    {
        // 프리펩 내부의 아이템과 텍스트 관련 public 변수
        public Transform contentsItem; // Content Layout 오브젝트에 대한 참조
        public GameObject itemPrefab; // 프리펩을 Inspector에서 할당해주세요.
        public Slider contentsProgress;
        public TextMeshProUGUI progressText;
        public TextMeshProUGUI questDesc;
        public Button receiveBtn;

        // 아이템 프리펩에 사용할 이미지들
        public Sprite coinImage;
        public Sprite cardImage;

        // 아이템 프리펩 클론을 저장하는 변수
        private GameObject[] itemPrefabs;

        // 마지막 코인의 값을 저장하는 변수
        private int lastCoin;

        //골드 소모
        private int coinSpent = 0;
        //골드 획득
        private int coinEarned = 0;
        //보물상자(상점) 오픈
        private int treasureBoxOpened = 0;
        //카드 획득하기
        private int cardsAcquired = 0;
        //적을 처치하기
        private int enemiesDefeated = 0;
        //보스 처치하기
        private int bossesDefeated = 0;
        //광고 시청하기
        private int adsWatched = 0;
        //스테이지에서 상자 합성하기
        private int boxesCombinedInStage = 0;
        //전투에서 승리하기
        private int battlesWon = 0;

        public void Awake()
        {
            ExpandPrefabs();
            //UseCoinQuest();
        }

      

        // 아이템 프리펩의 텍스트를 지정하는 메서드
        public void SetItemText(string itemText, int prefabIndex)
        {
            // 유효한 프리펩 클론인지 확인
            if (prefabIndex >= 0 && prefabIndex < itemPrefabs.Length)
            {
                // 아이템 프리펩 클론의 텍스트를 지정
                var itemPrefabClone = itemPrefabs[prefabIndex];
                TextMeshProUGUI itemTextComponent = itemPrefabClone.GetComponentInChildren<TextMeshProUGUI>();
                if (itemTextComponent != null)
                {
                    itemTextComponent.text = itemText;
                }
            }
        }

        // 아이템 프리펩의 이미지를 지정하는 메서드
        public void SetItemImage(Sprite itemImage, int prefabIndex)
        {
            // 유효한 프리펩 클론인지 확인
            if (prefabIndex >= 0 && prefabIndex < itemPrefabs.Length)
            {
                // 아이템 프리펩 클론의 이미지를 지정
                var itemPrefabClone = itemPrefabs[prefabIndex];
                Image itemImageComponent = itemPrefabClone.GetComponentInChildren<Image>();
                if (itemImageComponent != null)
                {
                    itemImageComponent.sprite = itemImage;
                }
            }
        }
        // 프리펩을 2개로 늘리는 메서드
        public void ExpandPrefabs()
        {
            itemPrefabs = new GameObject[2]; // 두 개의 프리펩 클론을 저장하기 위한 배열

            for (int i = 0; i < 2; i++)
            {
                GameObject instantiateItemPrefab = Instantiate(itemPrefab, contentsItem);
                itemPrefabs[i] = instantiateItemPrefab; // 생성된 프리펩 클론을 배열에 저장
            }

            // 각 프리펩에 이미지 적용
            Image coinImageComponent = itemPrefabs[0].GetComponentInChildren<Image>();
            if (coinImageComponent != null)
            {
                coinImageComponent.sprite = coinImage;
            }

            Image cardImageComponent = itemPrefabs[1].GetComponentInChildren<Image>();
            if (cardImageComponent != null)
            {
                cardImageComponent.sprite = cardImage;
            }
        }

        public void Update()
        {
    
        }

        public void SetProgressText(int count, int num)
        {
            switch (num)
            {
                case 1:
                    progressText.text = coinSpent.ToString() + " / " + count.ToString();
                    break;
                case 2:
                    progressText.text = coinEarned.ToString() + " / " + count.ToString();
                    break;
                case 3:
                    progressText.text = treasureBoxOpened.ToString() + " / " + count.ToString();
                    break;
                case 4:
                    progressText.text = cardsAcquired.ToString() + " / " + count.ToString();
                    break;
                case 5:
                    progressText.text = enemiesDefeated.ToString() + " / " + count.ToString();
                    break;
                case 6:
                    progressText.text = bossesDefeated.ToString() + " / " + count.ToString();
                    break;
                case 7:
                    progressText.text = adsWatched.ToString() + " / " + count.ToString();
                    break;
                case 8:
                    progressText.text = boxesCombinedInStage.ToString() + " / " + count.ToString();
                    break;
                case 9:
                    progressText.text = battlesWon.ToString() + " / " + count.ToString();
                    break;

            }
        }

        public void UseCoinQuest(int spentAmount)
        {
            coinSpent += spentAmount;
        }
    }
}