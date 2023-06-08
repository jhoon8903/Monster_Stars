using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
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
        [SerializeField] private GridManager gridManager;
        [SerializeField] private SwipeManager swipeManager;
        [SerializeField] private ExpManager expManager;
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private CharacterManager characterManager;
        public readonly Queue<GameObject> PendingTreasure = new Queue<GameObject>(); // 보류 중인 보물 큐
        private GameObject _currentTreasure = null; // 현재 보물
        public bool openBoxing = false;
        private bool _waveRewards = false; 
        private CommonData _selectedCommonReward = null;
        private List<CommonData> _powerUps = null;

        // 1. 상자가 매치 되면 상자를 큐에 추가
        public void EnqueueTreasure(GameObject treasure)
        {
            openBoxing = true;
            var shake = treasure.transform.DOShakeScale(1.0f, 0.5f, 8); // 흔들리는 애니메이션 재생
            shake.OnComplete(() =>
            {
                _currentTreasure = treasure;
                StartCoroutine(OpenBox(_currentTreasure));
            });
            openBoxing = false;
        }

        // 2. 상자마다의 확률 분배
        private IEnumerator CommonChance(int greenChance, int blueChance, int purpleChance, string forcedColor)
        {
            if (gameManager.wave == 11)
            {
                greenChance = 30;
                blueChance = 55;
                purpleChance = 15;
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

            for (var i = 0; i < 3; i++)
            {
                if (gameManager.wave == 11 || gameManager.wave == 21 && i == 0 && _waveRewards)
                {                                             
                    var firstDesiredPowerUp = new CommonPurpleData(purpleSprite, 13, CommonData.Types.AddRow, new[] { 1 });
                    commonPowerUps.Add(firstDesiredPowerUp);
                    selectedCodes.Add(firstDesiredPowerUp.Code);
                    var secondDesiredPowerUp = new CommonBlueData(blueSprite, 11, CommonData.Types.Slow, new[] { 15 }); 
                    commonPowerUps.Add(secondDesiredPowerUp);
                    selectedCodes.Add(secondDesiredPowerUp.Code);
                }
                else
                {
                   
                    CommonData selectedPowerUp = null;
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
        private CommonData CommonUnique(IEnumerable<CommonData> powerUpsData, ICollection<int> selectedCodes)
        {
            var validOptions = powerUpsData.Where(p => IsValidOption(p, selectedCodes));
            return SelectRandom(validOptions);
        }

        // 5. case별 예외처리
       private bool IsValidOption(CommonData powerUp, ICollection<int> selectedCodes)
            {
                if (selectedCodes.Contains(powerUp.Code)) return false; // Do not select already selected reward codes again
                switch (powerUp.Type)
                {
                    case CommonData.Types.CastleMaxHp:
                        if (castleManager.maxHpPoint >= 2000) return false; // Make sure the max HP of the castle does not exceed 2000
                        break;
                    case CommonData.Types.Exp:
                        if (characterManager.expPercentage > 30) return false; // Make sure the EXP increment does not exceed 30%
                        break;
                    case CommonData.Types.AddRow:
                        if (gameManager.wave != 11 || gameManager.wave != 21 && characterManager.addRowCount >= 2) return false; // Show row extra reward only after boss stage, up to 2 times
                        break;
                    case CommonData.Types.Slow:
                        if (characterManager.slowCount <= 3) return false; // Displays the enemy movement speed reduction effect up to 3 times
                        break;
                    case CommonData.Types.NextStage:
                        if (characterManager.nextStageMembersSelectCount <= 3) return false; // Only use up to 3 next stage character upgrades
                        break;
                    case CommonData.Types.StepDirection:
                        if (characterManager.diagonalMovement) return false; // If diagonal movement is possible, don't show this option
                        break;
                    case CommonData.Types.Match5Upgrade:
                        if (characterManager._5MatchUpgradeOption) return false; // Don't show this option if 5 matching upgrade option is enabled
                        break;
                    case CommonData.Types.StepLimit:
                        if (characterManager.permanentIncreaseMovementCount) return false; // don't show this option if permanent move count increment is enabled
                        break;
                    case CommonData.Types.CastleRecovery:
                        if (characterManager.recoveryCastle) return false; // Castle recovery can only be used once
                        break;
                    case CommonData.Types.GroupLevelUp:
                        if (characterManager.CharacterGroupLevelUpIndexes.Contains(powerUp.Property[0])) return false; // Do not display GroupLevelUp options for groups where LevelUpPattern is executed
                        break;
                    case CommonData.Types.Gold:
                        if (characterManager.goldGetMore) return false;
                        break;
                }
                return true;
            }

       // 6. 예외처리되고 처리 된 옵션값 리턴
       private static CommonData SelectRandom(IEnumerable<CommonData> validOptions)
       {
           var count = validOptions.Count();
           if (count == 0) return null;
           var randomIndex = Random.Range(0, count);
           return validOptions.ElementAt(randomIndex);
       }

       // 7. 옵션값 출략
       private void CommonDisplay(IReadOnlyList<CommonData> powerUpsDisplayData)
        {
            CommonDisplayText(common1Button,common1Text, common1Code, common1BtnBadge,powerUpsDisplayData[0]);
            CommonDisplayText(common2Button,common2Text, common2Code, common2BtnBadge,powerUpsDisplayData[1]);
            CommonDisplayText(common3Button,common3Text, common3Code, common3BtnBadge,powerUpsDisplayData[2]);
        }

       // 8. 옵션 텍스트
       private void CommonDisplayText(Button commonButton, TMP_Text powerText, TMP_Text powerCode, Image btnBadge ,CommonData powerUp)
       {
           var p = powerUp.Property[0];
            switch (powerUp.Type)
            {
                case CommonData.Types.Exp: 
                    powerText.text = $"Acquire additional Exp {p}% when killing an enemy. (up to 30%)"; 
                    break;
                case CommonData.Types.Slow: 
                    powerText.text = $"Enemy movement speed is slowed by {p}%. (up to 45%)"; 
                    break;
                case CommonData.Types.GroupDamage: 
                    powerText.text = $"Increases the damage of the entire character group by {p}%."; 
                    break;
                case CommonData.Types.GroupAtkSpeed: 
                    powerText.text = $"Increases the attack speed of the entire character group by {p}%."; 
                    break;
                case CommonData.Types.Step: 
                    powerText.text = $"Move count increases by {p} steps."; 
                    break;
                case CommonData.Types.StepLimit: 
                    powerText.text = $"{p} steps are added to the movement count for each wave."; 
                    break;
                case CommonData.Types.StepDirection: 
                    powerText.text = $"{p} steps are added to the movement count for each wave."; 
                    break;
                case CommonData.Types.RandomLevelUp: 
                    powerText.text = $"Level up {p} characters on the puzzle by 1 level."; 
                    break;
                case CommonData.Types.GroupLevelUp: 
                    var groupLevelUpName = characterManager.UnitGroupsCheck(p); 
                    powerText.text = $"Level 0 characters in the {groupLevelUpName} group on the puzzle will level up by 1"; 
                    break;
                case CommonData.Types.LevelUpPattern: 
                    var groupName = characterManager.UnitGroupsCheck(p); 
                    powerText.text = $"Level 0 characters in Group_{groupName} appear as level 1."; 
                    break;
                case CommonData.Types.CastleRecovery: 
                    powerText.text = $"If you take no damage from the wave, the castle's health is restored by {p}."; 
                    break;
                case CommonData.Types.CastleMaxHp: 
                    powerText.text = $"Increases the max HP and HP of the castle by {p}."; 
                    break;
                case CommonData.Types.Match5Upgrade: 
                    powerText.text = "When 5 matches are made, the level of the middle character increases by 1 additional level."; 
                    break;
                case CommonData.Types.NextStage: 
                    powerText.text = $"You can take {p} additional characters when initializing characters. (up to 3 Characters)"; 
                    break;
                case CommonData.Types.Gold: 
                    powerText.text = $"At 5 matches, additional gold is obtained as much as {p}."; 
                    break;
                case CommonData.Types.AddRow: 
                    powerText.text = $"A horizontal {p} line is added."; 
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
            powerCode.text = $"{powerUp.Code}";
            btnBadge.sprite = powerUp.BtnColor;
            commonButton.image = commonButton.image;
            commonButton.onClick.RemoveAllListeners();
            commonButton.onClick.AddListener(() => SelectCommonReward(powerUp));
        }

        // 9. 상자 오픈
        private IEnumerator OpenBox(GameObject treasure)
        {
            _waveRewards = false;
            Time.timeScale = 0; // 게임 일시 정지
            commonRewardPanel.SetActive(true); // 보물 패널 활성화
            var treasureChestName = treasure.GetComponent<CharacterBase>().CharacterName; // 보물 상자 이름

            switch(treasureChestName)
            {
                case "Unit_Treasure00":
                    break;
                case "Unit_Treasure01":
                    yield return StartCoroutine(CommonChance(70, 25, 5, null));
                    break;
                case "Unit_Treasure02":
                    yield return StartCoroutine(CommonChance(30, 55, 15, "blue"));
                    break;
                case "Unit_Treasure03":
                    yield return StartCoroutine(CommonChance(0, 50, 50, "purple"));
                    break;
            }

            yield return new WaitUntil(() => commonRewardPanel.activeSelf == false); // 보물 패널이 비활성화될 때까지 대기
        }

        // 10. 상자 선택
        private void SelectCommonReward(CommonData selectedReward)
        {
            _selectedCommonReward = selectedReward;
            Selected();
        }
       
        // 11. 선택 처리
        private void Selected()
        {
            commonRewardPanel.SetActive(false);
            if (countManager.baseMoveCount == 0)
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
            if (PendingTreasure.Count > 0)
            {
                _currentTreasure = PendingTreasure.Dequeue();
                EnqueueTreasure(_currentTreasure);
            }
            else
            {
                _currentTreasure = null; // 현재 보물 없음
            }
            ProcessCommonReward(_selectedCommonReward);
        }

        // 12. 선택된 버프 적용 
        private void ProcessCommonReward(CommonData selectedReward)
        {
            var findCharacterManager = FindObjectOfType<CharacterManager>();
            switch (selectedReward.Type)
            {
                case CommonData.Types.AddRow: gridManager.AddRow(); characterManager.addRowCount += 1; break;                                   // Row 추가 강화 효과
                case CommonData.Types.GroupDamage: findCharacterManager.IncreaseGroupDamage(selectedReward.Property[0]); break;     // 전체 데미지 증가 효과
                case CommonData.Types.GroupAtkSpeed: findCharacterManager.IncreaseGroupAtkRate(selectedReward.Property[0]); break;  // 전체 공격 속도 증가 효과
                case CommonData.Types.Step: countManager.IncreaseRewardMoveCount(selectedReward.Property[0]); break;            // 카운트 증가
                case CommonData.Types.StepLimit: countManager.PermanentIncreaseMoveCount(selectedReward.Property[0]); characterManager.permanentIncreaseMovementCount = true; break; // 영구적 카운트 증가
                case CommonData.Types.StepDirection: swipeManager.EnableDiagonalMovement(); characterManager.diagonalMovement = true; break;    // 대각선 이동
                case CommonData.Types.RandomLevelUp: findCharacterManager.RandomCharacterLevelUp(selectedReward.Property[0]); break;// 랜덤 케릭터 레벨업
                case CommonData.Types.GroupLevelUp: findCharacterManager.CharacterGroupLevelUp(selectedReward.Property[0]); break;  // 케릭터 그룹 레벨업
                case CommonData.Types.LevelUpPattern: findCharacterManager.PermanentIncreaseCharacter(selectedReward.Property[0]); characterManager.CharacterGroupLevelUpIndexes.Add(selectedReward.Property[0]); break; // 기본 2레벨 케릭터 생성
                case CommonData.Types.Exp: expManager.IncreaseExpBuff(selectedReward.Property[0]); characterManager.expPercentage += 5; break;  // 경험치 5% 증가
                case CommonData.Types.CastleRecovery: gameManager.RecoveryCastle = true; characterManager.recoveryCastle = true; break;         // 성 체력 회복
                case CommonData.Types.Match5Upgrade: matchManager.match5Upgrade = true; characterManager._5MatchUpgradeOption = true; break;     // 5매치 패턴 업그레이드
                case CommonData.Types.Slow: enemyManager.DecreaseMoveSpeed(selectedReward.Property[0]); characterManager.slowCount += 1; break; // 적 이동속도 감소 
                case CommonData.Types.NextStage: spawnManager.nextCharacterUpgrade(selectedReward.Property[0]); characterManager.nextStageMembersSelectCount += 1; break; // 보드 초기화 시 케릭터 상속되는 케릭터 Count 증가
                case CommonData.Types.Gold: characterManager.goldGetMore = true; Debug.LogWarning($"Unhandled reward type: {selectedReward.Type}"); break;
                case CommonData.Types.CastleMaxHp: castleManager.IncreaseMaxHp(selectedReward.Property[0]); break;               // 성 최대 체력 증가
                default: Debug.LogWarning($"Unhandled reward type: {selectedReward.Type}"); break;
            }
            selectedReward.ChosenProperty = null;
            _selectedCommonReward = null;
        }
        
        // # 보스 웨이브 클리어 변도 보상
        public IEnumerator WaveReward()
        {
            Time.timeScale = 0;
            commonRewardPanel.SetActive(true);
            _waveRewards = true;
            yield return StartCoroutine(CommonChance(30, 55, 15, null));
        }
    }
}
