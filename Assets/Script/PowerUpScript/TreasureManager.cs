using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.CharacterManagerScript;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Script.PowerUpScript
{
    public class TreasureManager : MonoBehaviour
    {
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
        [SerializeField] internal Sprite greenBtn; // 녹색 버튼 스프라이트
        [SerializeField] internal Sprite blueBtn; // 파란색 버튼 스프라이트
        [SerializeField] internal Sprite purpleBtn; // 보라색 버튼 스프라이트
        [SerializeField] private Common common; // 공통 데이터
        [SerializeField] private Exp exp; // 경험치
        [SerializeField] private SpawnManager spawnManager;
        // [SerializeField] private StagePowerUp stagePowerUp;
        public readonly Queue<GameObject> PendingTreasure = new Queue<GameObject>(); // 보류 중인 보물 큐
        private GameObject _currentTreasure = null; // 현재 보물


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
            var treasureChestName = treasure.GetComponent<CharacterBase>()._characterName; // 보물 상자 이름

            switch (treasureChestName)
            {
                case "Unit_Treasure00":
                    break;
                case "Unit_Treasure01":
                    yield return StartCoroutine(Treasure1Reward()); // 보물1 보상 처리
                    break;
                case "Unit_Treasure02":
                    yield return StartCoroutine(Treasure2Reward()); // 보물2 보상 처리
                    break;
                case "Unit_Treasure03":
                    yield return StartCoroutine(Treasure3Reward()); // 보물3 보상 처리
                    break;
            }
            yield return new WaitUntil(() => treasurePanel.activeSelf == false); // 보물 패널이 비활성화될 때까지 대기
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

        private IEnumerator Treasure1Reward()
        {
            var powerUps = CommonSelectPower(70, 25, 5); // 공통 보상1 선택
            CommonDisplayPowerUps(powerUps); // 보상 표시
            yield return null;
        }
        private IEnumerator Treasure2Reward()
        {
            var powerUps = CommonSelectPower(30, 55, 15); // 공통 보상2 선택
            CommonDisplayPowerUps(powerUps); // 보상 표시
            yield return null;
        }
        private IEnumerator Treasure3Reward()
        {
            var powerUps = CommonSelectPower(0, 50, 50); // 공통 보상3 선택
            CommonDisplayPowerUps(powerUps); // 보상 표시
            yield return null;
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

            power1Text.text = $"{powerUps[0].Type} {powerUps[0].Property[0]}% PowerUp_Property";
            powerOption1Button.sprite = powerUps[0].Image;
            power1Code.text = $"{powerUps[0].Code}";

            power2Text.text = $"{powerUps[1].Type} {powerUps[1].Property[0]}% PowerUp_Property";
            powerOption2Button.sprite = powerUps[1].Image;
            power2Code.text = $"{powerUps[1].Code}";

            power3Text.text = $"{powerUps[2].Type} {powerUps[2].Property[0]}% PowerUp_Property";
            powerOption3Button.sprite = powerUps[2].Image;
            power3Code.text = $"{powerUps[2].Code}";
        }
    }
}
