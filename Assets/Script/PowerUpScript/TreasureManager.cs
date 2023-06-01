using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.CharacterManagerScript;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Script.PowerUpScript
{
    public class TreasureManager : MonoBehaviour
    {
        // UI elements for treasure management
        [SerializeField] public GameObject treasurePanel; // 보물 패널
        [SerializeField] private TextMeshProUGUI power1Text; // 파워1 텍스트
        [SerializeField] private TextMeshProUGUI power2Text; // 파워2 텍스트
        [SerializeField] private TextMeshProUGUI power3Text; // 파워3 텍스트
        [SerializeField] private Image powerOption1Button; // 파워1 버튼 이미지
        [SerializeField] private Image powerOption2Button; // 파워2 버튼 이미지
        [SerializeField] private Image powerOption3Button; // 파워3 버튼 이미지
        [SerializeField] private TextMeshProUGUI power1Code; // 파워1 코드 텍스트
        [SerializeField] private TextMeshProUGUI power2Code; // 파워2 코드 텍스트
        [SerializeField] private TextMeshProUGUI power3Code; // 파워3 코드 텍스트
        
        // Sprites for power up buttons 
        [SerializeField] internal Sprite greenBtn; // 녹색 버튼 스프라이트
        [SerializeField] internal Sprite blueBtn; // 파란색 버튼 스프라이트
        [SerializeField] internal Sprite purpleBtn; // 보라색 버튼 스프라이트
        
        // Other utilities
        [SerializeField] private Common common; // 공통 데이터
        [SerializeField] private Exp exp; // 경험치
        [SerializeField] private SpawnManager spawnManager;
        
        // [SerializeField] private StagePowerUp stagePowerUp;
        public readonly Queue<GameObject> PendingTreasure = new Queue<GameObject>(); // 보류 중인 보물 큐
        
        private GameObject _currentTreasure = null; // 현재 보물


        // Adds a new treasure into the queue and starts handling it
        public void EnqueueAndCheckTreasure(GameObject treasure) // 보물을 큐에 추가하고 확인
        {
            var shake = treasure.transform.DOShakeScale(1.0f, 0.5f, 8); // 흔들리는 애니메이션 재생
            shake.OnComplete(() =>
                {
                    _currentTreasure = treasure;
                    StartCoroutine(HandleTreasure(_currentTreasure));
                });
        }

        private IEnumerator HandleTreasure(GameObject treasure) // 보물 처리
        {
            Time.timeScale = 0; // 게임 일시 정지
            treasurePanel.SetActive(true); // 보물 패널 활성화
            var treasureChestName = treasure.GetComponent<CharacterBase>().CharacterName; // 보물 상자 이름

            switch (treasureChestName)
            {
                case "Unit_Treasure00":
                    break;
                case "Unit_Treasure01":
                    yield return StartCoroutine(HandleTreasureReward(70, 25, 5));
                    break;
                case "Unit_Treasure02":
                    yield return StartCoroutine(HandleTreasureReward(30, 55, 15));
                    break;
                case "Unit_Treasure03":
                    yield return StartCoroutine(HandleTreasureReward(0, 50, 50));
                    break;
            }
            yield return new WaitUntil(() => treasurePanel.activeSelf == false); // 보물 패널이 비활성화될 때까지 대기
        }

        private IEnumerator HandleTreasureReward(int greenChance, int blueChance, int purpleChance)
        {
            var powerUps = CommonSelectPower(greenChance, blueChance, purpleChance);
            CommonDisplayPowerUps(powerUps);
            yield return null;
        }

        private static void UpdatePowerUpDisplay(TMP_Text powerText, Image powerButton, TMP_Text powerCode, CommonData powerUp)
        {
            powerText.text = $"{powerUp.Type} {powerUp.Property[0]}% PowerUp_Property";
            powerButton.sprite = powerUp.Image;
            powerCode.text = $"{powerUp.Code}";
        }

        public void PowerUpSelected()
        {
            treasurePanel.SetActive(false);
            Time.timeScale = 1; // 게임 재개
            CharacterPool.ReturnToPool(_currentTreasure); // 보물을 풀에 반환
            StartCoroutine(spawnManager.PositionUpCharacterObject());
            if (PendingTreasure.Count > 0)
            {
                _currentTreasure = PendingTreasure.Dequeue();
                EnqueueAndCheckTreasure(_currentTreasure);
            }
            else
            {
                _currentTreasure = null; // 현재 보물 없음
            }
        }
        public static int IncreaseRange(IReadOnlyList<int> rangeValues)
        {
            var random = new System.Random();
            var randomIndex = random.Next(rangeValues.Count);
            return rangeValues[randomIndex];
        }
        private List<CommonData> CommonSelectPower(int greenChance, int blueChance, int purpleChance)
        {
            var commonPowerUps = new List<CommonData>();
            var selectedCodes = new HashSet<int>();
            for (var i = 0; i < 3; i++)
            {
                var total = greenChance + blueChance + purpleChance;
                var randomValue = Random.Range(0, total);
                CommonData selectedPowerUp = null;
                if (randomValue < greenChance)
                {
                    selectedPowerUp = SelectUniquePowerUp(common.CommonGreenList, selectedCodes);
                }
                else if (randomValue < greenChance + blueChance)
                {
                    selectedPowerUp = SelectUniquePowerUp(common.CommonBlueList, selectedCodes);
                }
                else
                {
                    selectedPowerUp = SelectUniquePowerUp(common.CommonPurpleList, selectedCodes);
                }
                if (selectedPowerUp == null) continue;
                commonPowerUps.Add(selectedPowerUp);
                selectedCodes.Add(selectedPowerUp.Code);
            }
            return commonPowerUps;
        }
        private static CommonData SelectUniquePowerUp(IEnumerable<CommonData> powerUps, ICollection<int> selectedCodes)
        {
            var validOptions = powerUps.Where(p => !selectedCodes.Contains(p.Code)).ToList();
            if (validOptions.Count == 0)
            {
                return null;
            }
            var randomIndex = Random.Range(0, validOptions.Count);
            return validOptions[randomIndex];
        }

        private void CommonDisplayPowerUps(IReadOnlyList<CommonData> powerUps)
        {
            treasurePanel.SetActive(true);
            UpdatePowerUpDisplay(power1Text, powerOption1Button, power1Code, powerUps[0]);
            UpdatePowerUpDisplay(power2Text, powerOption2Button, power2Code, powerUps[1]);
            UpdatePowerUpDisplay(power3Text, powerOption3Button, power3Code, powerUps[2]);
        }
    }
}
