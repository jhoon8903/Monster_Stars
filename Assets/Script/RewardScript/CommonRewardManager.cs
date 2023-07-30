using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.PuzzleManagerGroup;
using Script.UIManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Script.RewardScript
{
    public class CommonRewardManager : MonoBehaviour
    {
        [SerializeField] public GameObject commonRewardPanel; // 보물 패널
        [SerializeField] private TextMeshProUGUI common1Text; // 파워1 텍스트
        [SerializeField] private TextMeshProUGUI common2Text; // 파워2 텍스트
        [SerializeField] private TextMeshProUGUI common3Text; // 파워3 텍스트
        [SerializeField] private Button common1Button; 
        [SerializeField] private Button common2Button; 
        [SerializeField] private Button common3Button; 
        [SerializeField] private Image common1BtnBadge;
        [SerializeField] private Image common2BtnBadge;
        [SerializeField] private Image common3BtnBadge;
        [SerializeField] private TextMeshProUGUI common1Code; // 파워1 코드 텍스트
        [SerializeField] private TextMeshProUGUI common2Code; // 파워2 코드 텍스트
        [SerializeField] private TextMeshProUGUI common3Code; // 파워3 코드 텍스트
        [SerializeField] internal Sprite greenSprite; // 녹색 버튼 스프라이트
        [SerializeField] internal Sprite blueSprite; // 파란색 버튼 스프라이트
        [SerializeField] internal Sprite purpleSprite; // 보라색 버튼 스프라이트
        [SerializeField] private Common common; // 공통 데이터
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private CountManager countManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private Language language; // 텍스트 

        public readonly Queue<GameObject> PendingTreasure = new Queue<GameObject>(); // 보류 중인 보물 큐
        private GameObject _currentTreasure; // 현재 보물
        public bool openBoxing = true;
        private List<CommonData> _powerUps;
        private string _groupName;
        private int _bossRewardSelected;
        public bool isOpenBox;
        
        // 1. 상자가 매치 되면 상자를 큐에 추가
        public void EnqueueTreasure()
        {
            if (isOpenBox) return;

            isOpenBox = true;
            var treasure = PendingTreasure.Dequeue();
            openBoxing = true;
            var shake = treasure.transform.DOShakeScale(1.0f, 0.5f, 8); // 흔들리는 애니메이션 재생
            shake.OnComplete(() =>
            {
                _currentTreasure = treasure;
                StartCoroutine(OpenBox(_currentTreasure));
            });
        }

        // 2. 상자마다의 확률 분배
        private IEnumerator CommonChance(int greenChance, int blueChance, int purpleChance, string forcedColor)
        {
            if (StageManager.Instance.currentWave >= 11 && !StageManager.Instance.isBossClear)
            {
                greenChance = 60;
                blueChance = 35;
                purpleChance = 5;
                _powerUps = CommonPowerList(greenChance, blueChance, purpleChance, "blue");
                CommonDisplay(_powerUps);
                yield return null;
            }
            else
            {
                _powerUps = CommonPowerList(greenChance, blueChance, purpleChance, forcedColor);
                CommonDisplay(_powerUps);
                yield return null;
            }
        }

        // 3. 확률별로 선택 옵션 계산
        private List<CommonData> CommonPowerList(int greenChance, int blueChance, int purpleChance, string forcedColor)
        {
            var commonPowerUps = new List<CommonData>();
            var selectedCodes = new HashSet<int>();
            if (StageManager.Instance.isBossClear && !EnforceManager.Instance.addRow)
            {
                var firstDesiredPowerUp = new CommonPurpleData(purpleSprite, 16, CommonData.Types.AddRow, new[] { 1 });
                commonPowerUps.Add(firstDesiredPowerUp);
                selectedCodes.Add(firstDesiredPowerUp.Code);

                for (var i = 0; i < 2; i++)
                {
                    CommonData selectedPowerUp;
                    switch (forcedColor)
                    {
                        case "blue" when i == 0: selectedPowerUp = CommonUnique(common.CommonBlueList, selectedCodes); break;
                        case "purple" when i == 0: selectedPowerUp = CommonUnique(common.CommonPurpleList, selectedCodes); break;
                        default:
                        {
                            var total = greenChance + blueChance + purpleChance;
                            var randomValue = Random.Range(0, total);
                            if (randomValue < greenChance) { selectedPowerUp = CommonUnique(common.CommonGreenList, selectedCodes); }
                            else if (randomValue < greenChance + blueChance) { selectedPowerUp = CommonUnique(common.CommonBlueList, selectedCodes); }
                            else { selectedPowerUp = CommonUnique(common.CommonPurpleList, selectedCodes); }
                            break;
                        }
                    }
                    if (selectedPowerUp == null) continue;
                    commonPowerUps.Add(selectedPowerUp);
                    selectedCodes.Add(selectedPowerUp.Code);
                }
            }
            else
            {
                // 랜덤 선택지 추가
                for (var i = 0; i < 3; i++)
                {
                    CommonData selectedPowerUp;
                    switch (forcedColor)
                    {
                        case "blue" when i == 0: selectedPowerUp = CommonUnique(common.CommonBlueList, selectedCodes); break;
                        case "purple" when i == 0: selectedPowerUp = CommonUnique(common.CommonPurpleList, selectedCodes); break;
                        default:
                            {
                                var total = greenChance + blueChance + purpleChance;
                                var randomValue = Random.Range(0, total);
                                if (randomValue < greenChance) { selectedPowerUp = CommonUnique(common.CommonGreenList, selectedCodes); }
                                else if (randomValue < greenChance + blueChance) { selectedPowerUp = CommonUnique(common.CommonBlueList, selectedCodes); }
                                else { selectedPowerUp = CommonUnique(common.CommonPurpleList, selectedCodes); }
                                break;
                            }
                    }
                    if (selectedPowerUp == null) continue;
                    commonPowerUps.Add(selectedPowerUp);
                    selectedCodes.Add(selectedPowerUp.Code);
                }
            }
            return commonPowerUps;
        }

        // 4. 코드 중복검사
        private static CommonData CommonUnique(IEnumerable<CommonData> powerUpsData, ICollection<int> selectedCodes)
        {
            var validOptions = powerUpsData.Where(p => IsValidOption(p, selectedCodes));
            return SelectRandom(validOptions);
        }

        // 5. case별 예외처리
       private static bool IsValidOption(CommonData powerUp, ICollection<int> selectedCodes)
            {
                if (selectedCodes.Contains(powerUp.Code)) return false; // Do not select already selected reward codes again
                switch (powerUp.Type)
                {
                    case CommonData.Types.Gold:
                        if (EnforceManager.Instance.addGold) return false;
                        break;
                    case CommonData.Types.CastleMaxHp:
                        if (EnforceManager.Instance.castleMaxHp >= 1000) return false; // Make sure the max HP of the castle does not exceed 2000
                        break;
                    case CommonData.Types.Exp:
                        if (EnforceManager.Instance.expPercentage >= 30) return false; // Make sure the EXP increment does not exceed 30%
                        break;
                    case CommonData.Types.AddRow:
                        if (EnforceManager.Instance.addRow) return false;
                        if (!StageManager.Instance.isBossClear) return false;
                        if (StageManager.Instance.currentWave % 10 != 0 ) return false;// Show row extra reward only after boss stage, up to 2 times
                        break;
                    case CommonData.Types.Slow:
                        if (EnforceManager.Instance.slowCount >= 4) return false; // Displays the enemy movement speed reduction effect up to 3 times
                        break;
                    case CommonData.Types.NextStage:
                        if (EnforceManager.Instance.selectedCount > 3) return false; // Only use up to 3 next stage character upgrades
                        break;
                    case CommonData.Types.StepDirection:
                        if (EnforceManager.Instance.diagonalMovement)return false;
                        if (!StageManager.Instance.isBossClear) return false;
                        if (StageManager.Instance.currentWave % 10 != 0 ) return false;
                        break;
                    case CommonData.Types.Match5Upgrade:
                        if (EnforceManager.Instance.match5Upgrade) return false; // Don't show this option if 5 matching upgrade option is enabled
                        break;
                    case CommonData.Types.StepLimit:
                        if (EnforceManager.Instance.permanentIncreaseMovementCount > 1) return false; // don't show this option if permanent move count increment is enabled
                        break;
                    case CommonData.Types.CastleRecovery:
                        if (EnforceManager.Instance.recoveryCastle) return false; // Castle recovery can only be used once
                        break;
                    case CommonData.Types.GroupLevelUp:
                        if (EnforceManager.Instance.permanentGroupIndex.Contains(powerUp.Property[0])) return false; // Do not display GroupLevelUp options for groups where LevelUpPattern is executed
                        break;
                }
                return true;
            }

       // 6. 예외처리되고 처리 된 옵션값 리턴
       private static CommonData SelectRandom(IEnumerable<CommonData> validOptions)
       {
           var commonDataList = validOptions.ToList();
           var count = commonDataList.Count();
           if (count == 0) return null;
           var randomIndex = Random.Range(0, count);
           return commonDataList.ElementAt(randomIndex);
       }

       // 7. 옵션값 출력
       private void CommonDisplay(IReadOnlyList<CommonData> powerUpsDisplayData)
        {
            CommonDisplayText(common1Button,common1Text, common1Code, common1BtnBadge,powerUpsDisplayData[0], language);
            CommonDisplayText(common2Button,common2Text, common2Code, common2BtnBadge,powerUpsDisplayData[1], language);
            CommonDisplayText(common3Button,common3Text, common3Code, common3BtnBadge,powerUpsDisplayData[2], language);
        }

       // 8. 옵션 텍스트
       private void CommonDisplayText(Button commonButton, TMP_Text powerText, TMP_Text powerCode, Image btnBadge, CommonData powerUp, Language languages)
       {
           var translationKey = powerUp.Type.ToString();
           var powerTextTranslation = languages.GetTranslation(translationKey);
           var p = powerUp.Property[0].ToString();
           var finalPowerText = powerTextTranslation.Replace("{p}", p);

           var placeholderValues = new Dictionary<string, Func<double>> {
               { "{15*EnforceManager.Instance.slowCount}", () => 15*EnforceManager.Instance.slowCount },
               { "{EnforceManager.Instance.expPercentage}", () => EnforceManager.Instance.expPercentage },
               { "{EnforceManager.Instance.highLevelCharacterCount}", () => EnforceManager.Instance.highLevelCharacterCount},

           };
           finalPowerText = placeholderValues.Aggregate(finalPowerText, (current, placeholder) => current.Replace(placeholder.Key, placeholder.Value().ToString(CultureInfo.CurrentCulture)));

           var finalTranslation = finalPowerText.Replace("||", "\n");

            switch (powerUp.Type)
            {
                case CommonData.Types.Exp:
                    powerText.text = finalTranslation;
                    break;
                case CommonData.Types.Slow: 
                    powerText.text = finalTranslation;
                    break;
                case CommonData.Types.GroupDamage: 
                    powerText.text = finalTranslation; 
                    break;
                case CommonData.Types.GroupAtkSpeed: 
                    powerText.text = finalTranslation; 
                    break;
                case CommonData.Types.Step: 
                    powerText.text = finalTranslation; 
                    break;
                case CommonData.Types.StepLimit: 
                    powerText.text =finalTranslation; 
                    break;
                case CommonData.Types.StepDirection: 
                    powerText.text = finalTranslation; 
                    break;
                case CommonData.Types.RandomLevelUp: 
                    powerText.text = finalTranslation; 
                    break;
                case CommonData.Types.GroupLevelUp:
                    _groupName = characterManager.characterList[powerUp.Property[0]].name;
                    powerText.text = finalTranslation.Replace("{_groupName}", _groupName);
                    break;
                case CommonData.Types.LevelUpPattern:
                    _groupName = characterManager.characterList[powerUp.Property[0]].name;
                    powerText.text = finalTranslation.Replace("{_groupName}", _groupName);
                    break;
                case CommonData.Types.CastleRecovery: 
                    powerText.text = finalTranslation; 
                    break;
                case CommonData.Types.CastleMaxHp: 
                    powerText.text = finalTranslation; 
                    break;
                case CommonData.Types.Match5Upgrade: 
                    powerText.text = finalTranslation; 
                    break;
                case CommonData.Types.NextStage: 
                    powerText.text = finalTranslation;
                    break;
                case CommonData.Types.Gold: 
                    powerText.text = finalTranslation; 
                    break;
                case CommonData.Types.AddRow: 
                    powerText.text = finalTranslation; 
                    break;
            }
            powerCode.text = $"{powerUp.Code}";
            btnBadge.sprite = powerUp.BtnColor;
            _groupName = null;
            commonButton.image = commonButton.image;
            commonButton.onClick.RemoveAllListeners();
            commonButton.onClick.AddListener(() => SelectCommonReward(powerUp));
        }

        // 9. 상자 오픈
        private IEnumerator OpenBox(GameObject treasure)
        {
            Time.timeScale = 0; // 게임 일시 정지
            commonRewardPanel.SetActive(true); // 보물 패널 활성화
            var treasureChestLevel = treasure.GetComponent<CharacterBase>().unitPuzzleLevel; // 보물 상자 이름

            switch(treasureChestLevel)
            {
                case 1:
                    break;
                case 2:
                    yield return StartCoroutine(CommonChance(80, 17, 3, null));
                    break;
                case 3:
                    yield return StartCoroutine(CommonChance(60, 35, 5, "blue"));
                    break;
                case 4:
                    yield return StartCoroutine(CommonChance(0, 80, 20, "purple"));
                    break;
            }

            yield return new WaitUntil(() => commonRewardPanel.activeSelf == false); // 보물 패널이 비활성화될 때까지 대기
            openBoxing = false;

            if (PendingTreasure.Count > 0)
            {
                EnqueueTreasure();
            }
            isOpenBox = false;
        }

        // 10. 상자 선택
        private void SelectCommonReward(CommonData selectedReward)
        {
            Selected(selectedReward);
        }
       
        // 11. 선택 처리
        private void Selected(CommonData selectedReward)
        {
            commonRewardPanel.SetActive(false);
            if (countManager.TotalMoveCount == 0)
            {
                gameManager.GameSpeed();
            }
            else
            {
                Time.timeScale = 1; // 게임 재개
            }
            CharacterPool.ReturnToPool(_currentTreasure); // 보물을 풀에 반환
            if (!spawnManager.isWave10Spawning)
            {
                StartCoroutine(spawnManager.PositionUpCharacterObject());
            }
            else
            {
                _currentTreasure = null; // 현재 보물 없음
            }
            ProcessCommonReward(selectedReward);
        }

        // 12. 선택된 버프 적용 
        private void ProcessCommonReward(CommonData selectedCommonReward)
        {
            switch (selectedCommonReward.Type)
            {
                case CommonData.Types.AddRow:
                    EnforceManager.Instance.AddRow();
                    break; // Row 추가 강화 효과
                case CommonData.Types.GroupDamage: 
                    EnforceManager.Instance.IncreaseGroupDamage(selectedCommonReward.Property[0]); 
                    break;     // 전체 데미지 증가 효과
                case CommonData.Types.GroupAtkSpeed: 
                    EnforceManager.Instance.IncreaseGroupRate(selectedCommonReward.Property[0]); 
                    break;  // 전체 공격 속도 증가 효과
                case CommonData.Types.Step:
                    countManager.IncreaseMoveCount(selectedCommonReward.Property[0]);
                    break; // 카운트 증가
                case CommonData.Types.StepLimit: 
                    EnforceManager.Instance.PermanentIncreaseMoveCount(selectedCommonReward.Property[0]);
                    break; // 영구적 카운트 증가
                case CommonData.Types.StepDirection: 
                    EnforceManager.Instance.diagonalMovement = true; 
                    break;    // 대각선 이동
                case CommonData.Types.RandomLevelUp: 
                    EnforceManager.Instance.RandomCharacterLevelUp(selectedCommonReward.Property[0]); 
                    break;// 랜덤 케릭터 레벨업
                case CommonData.Types.GroupLevelUp: 
                    EnforceManager.Instance.CharacterGroupLevelUp(selectedCommonReward.Property[0]); 
                    break;  // 케릭터 그룹 레벨업
                case CommonData.Types.LevelUpPattern: 
                    EnforceManager.Instance.PermanentIncreaseCharacter(selectedCommonReward.Property[0]);
                    break; // 기본 2레벨 케릭터 생성
                case CommonData.Types.Exp: 
                    EnforceManager.Instance.IncreaseExpBuff(selectedCommonReward.Property[0]);
                    break;  // 경험치 5% 증가
                case CommonData.Types.CastleRecovery:
                    EnforceManager.Instance.recoveryCastle = true;
                    break;         // 성 체력 회복
                case CommonData.Types.CastleMaxHp: 
                    EnforceManager.Instance.IncreaseCastleMaxHp();
                    break;  
                case CommonData.Types.Match5Upgrade: 
                    EnforceManager.Instance.match5Upgrade = true;
                    break;     // 5매치 패턴 업그레이드
                case CommonData.Types.Slow:
                    EnforceManager.Instance.SlowCount();
                    break; // 적 이동속도 감소 
                case CommonData.Types.NextStage: 
                    EnforceManager.Instance.NextCharacterUpgrade(selectedCommonReward.Property[0]);
                    break; // 보드 초기화 시 케릭터 상속되는 케릭터 Count 증가
                case CommonData.Types.Gold: 
                    EnforceManager.Instance.addGold = true;
                    break;
                default: Debug.LogWarning($"Unhandled reward type: {selectedCommonReward.Type}"); 
                    break;
            }
            selectedCommonReward.ChosenProperty = null;
        }
        // # 보스 웨이브 클리어 별도 보상
        public IEnumerator WaveRewardChance()
        {
            if (StageManager.Instance.latestStage == 1) yield break;
            Time.timeScale = 0;
            commonRewardPanel.SetActive(true);
            yield return StartCoroutine(CommonChance(30, 55, 15, null));
        }
    }
}
