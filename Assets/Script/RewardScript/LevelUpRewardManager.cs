using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.PuzzleManagerGroup;
using Script.UIManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Script.RewardScript
{
    public class LevelUpRewardManager : MonoBehaviour
    {
        [SerializeField] public GameObject levelUpRewardPanel; // 보물 패널
        [SerializeField] private TextMeshProUGUI exp1Text; // 파워1 텍스트
        [SerializeField] private TextMeshProUGUI exp2Text; // 파워2 텍스트
        [SerializeField] private TextMeshProUGUI exp3Text; // 파워3 텍스트
        [SerializeField] private Button exp1Button; 
        [SerializeField] private Button exp2Button; 
        [SerializeField] private Button exp3Button;
        [SerializeField] private Image exp1BtnBadge;
        [SerializeField] private Image exp2BtnBadge;
        [SerializeField] private Image exp3BtnBadge;
        [SerializeField] private TextMeshProUGUI exp1Code; // 파워1 코드 텍스트
        [SerializeField] private TextMeshProUGUI exp2Code; // 파워2 코드 텍스트
        [SerializeField] private TextMeshProUGUI exp3Code; // 파워3 코드 텍스트
        [SerializeField] internal Sprite greenSprite; // 녹색 버튼 스프라이트
        [SerializeField] internal Sprite blueSprite; // 파란색 버튼 스프라이트
        [SerializeField] internal Sprite purpleSprite; // 보라색 버튼 스프라이트
        [SerializeField] private Exp exp; // 경험치
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private ExpManager expManager;
        [SerializeField] private CountManager countManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private SwipeManager swipeManager;
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private EnemyManager enemyManager;

        private void ProcessExpReward(ExpData selectedReward)
        {
            var findCharacterManager = FindObjectOfType<CharacterManager>();
            switch (selectedReward.Type)
            {
                case ExpData.Types.GroupDamage: 
                    findCharacterManager.IncreaseGroupDamage(selectedReward.Property[0]);
                    break;
                case ExpData.Types.GroupAtkSpeed:
                    findCharacterManager.IncreaseGroupAtkRate(selectedReward.Property[0]);
                    break;
                case ExpData.Types.StepLimit:
                    countManager.PermanentIncreaseMoveCount(selectedReward.Property[0]); 
                    characterManager.permanentIncreaseMovementCount = true;
                    break;
                case ExpData.Types.StepDirection:
                    swipeManager.EnableDiagonalMovement(); 
                    characterManager.diagonalMovement = true;
                    break;
                case ExpData.Types.Exp:
                    expManager.IncreaseExpBuff(selectedReward.Property[0]); 
                    characterManager.expPercentage += 5;
                    break;
                case ExpData.Types.CastleRecovery:
                    gameManager.RecoveryCastle = true; 
                    characterManager.recoveryCastle = true;
                    break;
                case ExpData.Types.CastleMaxHp:
                    castleManager.IncreaseMaxHp(selectedReward.Property[0]);
                    break;
                case ExpData.Types.Slow:
                    characterManager.slowCount += 1;
                    break;
                case ExpData.Types.NextStage:
                    spawnManager.NextCharacterUpgrade(selectedReward.Property[0]); 
                    characterManager.nextStageMembersSelectCount += 1;
                    break;
                case ExpData.Types.Gold:
                    characterManager.goldGetMore = true; 
                    Debug.LogWarning($"Unhandled reward type: {selectedReward.Type}");
                    break;
                // case ExpData.Types.DivineRestraint:
                //     break;
                // case ExpData.Types.DivinePenetrate:
                //     break;
                // case ExpData.Types.DivineRestraintDamage:
                //     break;
                // case ExpData.Types.DivineAtkRange:
                //     break;
                // case ExpData.Types.DivinePoisonAdditionalDamage:
                //     break;
                // case ExpData.Types.PhysicAdditionalWeapon:
                //     break;
                // case ExpData.Types.PhysicIncreaseWeaponScale:
                //     break;
                // case ExpData.Types.PhysicSlowAdditionalDamage:
                //     break;
                // case ExpData.Types.PhysicAtkSpeed:
                //     break;
                // case ExpData.Types.PhysicIncreaseDamage:
                //     break;
                // case ExpData.Types.PoisonDoubleAtk:
                //     break;
                // case ExpData.Types.PoisonRestraintAdditionalDamage:
                //     break;
                // case ExpData.Types.PoisonIncreaseTime:
                //     break;
                // case ExpData.Types.PoisonInstantKill:
                //     break;
                // case ExpData.Types.PoisonIncreaseAtkRange:
                //     break;
                // case ExpData.Types.WaterStun:
                //     break;
                // case ExpData.Types.WaterIncreaseSlowTime:
                //     break;
                // case ExpData.Types.WaterIncreaseSlowPower:
                //     break;
                // case ExpData.Types.WaterRestraintKnockBack:
                //     break;
                // case ExpData.Types.WaterIncreaseDamage:
                //     break;
                default: 
                    Debug.LogWarning($"Unhandled reward type: {selectedReward.Type}"); 
                    break;
            }
            selectedReward.ChosenProperty = null;
        }
        private void Selected(ExpData selectedReward)
        {
            levelUpRewardPanel.SetActive(false);
            if (countManager.totalMoveCount == 0)
            {
                gameManager.GameSpeed();
            }
            else
            {
                Time.timeScale = 1; // 게임 재개
            }
            if (!spawnManager.isWave10Spawning)
            {
                StartCoroutine(spawnManager.PositionUpCharacterObject());
            }
            ProcessExpReward(selectedReward);
        }
        public IEnumerator LevelUpReward() // 레벨업 보상 처리
        {
            Time.timeScale = 0; // 게임 일시 정지
            levelUpRewardPanel.SetActive(true); // 보물 패널 활성화
            var playerLevel = expManager.level;
            if (playerLevel <= 9)
            {
                yield return StartCoroutine(LevelUpChance(60, 30,10));
            }
            else
            {
                yield return StartCoroutine(LevelUpChance(30, 50, 20));
            }
            yield return new WaitUntil(() => levelUpRewardPanel.activeSelf == false); // 보물 패널이 비활성화될 때까지 대기
        }
        private IEnumerator LevelUpChance(int greenChance, int blueChance, int purpleChance)
        {
            var leverUpReward = LevelUpList(greenChance, blueChance, purpleChance);
            LevelUpDisplay(leverUpReward);
            yield return null;
        }
        private List<ExpData> LevelUpList(int greenChance, int blueChance, int purpleChance)
        {
            var levelUpPowerUps = new List<ExpData>();
            var selectedCodes = new HashSet<int>();
            for (var i = 0; i < 3; i++)
            {
                var total = greenChance + blueChance + purpleChance;
                var randomValue = Random.Range(0, total);
                ExpData selectedPowerUp = null;
                if (randomValue < greenChance)
                {
                    selectedPowerUp = LevelUpUnique(exp.ExpGreenList, selectedCodes);
                }
                else if (randomValue < greenChance + blueChance)
                {
                    selectedPowerUp = LevelUpUnique(exp.ExpBlueList, selectedCodes);
                }
                else
                {
                    selectedPowerUp = LevelUpUnique(exp.ExpPurpleList, selectedCodes);
                }
                if (selectedPowerUp == null) continue;
                levelUpPowerUps.Add(selectedPowerUp);
                selectedCodes.Add(selectedPowerUp.Code);
            }
            return levelUpPowerUps;
        }
        private static ExpData LevelUpUnique(IEnumerable<ExpData> powerUps, ICollection<int> selectedCodes)
        {
            var validOptions = powerUps.Where(p => !selectedCodes.Contains(p.Code)).ToList();
            if (validOptions.Count == 0)
            {
                return null;
            }
            var randomIndex = Random.Range(0, validOptions.Count);
            return validOptions[randomIndex];
        }
        private void LevelUpDisplay(IReadOnlyList<ExpData> powerUps)
        {
            LevelUpDisplayText(exp1Button, exp1Text, exp1Code, exp1BtnBadge, powerUps[0]);
            LevelUpDisplayText(exp2Button, exp2Text, exp2Code, exp2BtnBadge, powerUps[1]);
            LevelUpDisplayText(exp3Button, exp3Text, exp3Code, exp3BtnBadge, powerUps[2]);
        }
        private void LevelUpDisplayText(Button expButton, TMP_Text powerText,TMP_Text powerCode, Image btnBadge, ExpData powerUp)
        {
            var p = powerUp.Property[0];
            switch (powerUp.Type)
            {
                case ExpData.Types.GroupDamage:
                    powerText.text = $"전체 데미지 {p}% 증가";
                    break;
                case ExpData.Types.GroupAtkSpeed:
                    powerText.text = $"전체 공격속도 {p}% 증가";
                    break;
                case ExpData.Types.StepLimit:
                    powerText.text = $"최대 이동거리 {p} 증가";
                    break;
                case ExpData.Types.StepDirection:
                    powerText.text = $"대각선 이동 가능"; 
                    break;
                case ExpData.Types.Exp:
                    powerText.text = $"적 처치시 경험치 획득량 {p}% 증가\n(최대 30%)";
                    break;
                case ExpData.Types.CastleRecovery:
                    powerText.text = $"웨이브 종료까지 피해를 입지 않으면\n캐슬 체력 {p} 매 웨이브 마다 증가";
                    break;
                case ExpData.Types.CastleMaxHp:
                    powerText.text = $"캐슬 최대체력 {p} 증가"; 
                    break;
                case ExpData.Types.Slow:
                    powerText.text = $"다음 웨이브 부터는 적 이동속도 {p}% 감소\n( 현재 {15*characterManager.slowCount}% / 최대 60%)";
                    break;
                case ExpData.Types.NextStage:
                    powerText.text = $"보스 스테이지 이후 {p} 개의\n케릭터 추가 이동 (최대 3개)";
                    break;
                case ExpData.Types.Gold:
                    powerText.text = $"5 매치시 골드 {p} 추가 획득";
                    break;
                case ExpData.Types.DivineRestraint:
                    powerText.text = $"[A그룹 - 노랑] 속박 지속시간이 {p} 초 증가";
                    break;
                case ExpData.Types.DivinePenetrate:
                    powerText.text = $"[A그룹 - 노랑] 투사체가 적을 1회 관통합니다."; 
                    break;
                case ExpData.Types.DivineRestraintDamage:
                    powerText.text = $"[A그룹 - 노랑] 속박 데미지 100% 증가\n(틱 데미지 10% 증가)"; 
                    break;
                case ExpData.Types.DivineAtkRange:
                    powerText.text = $"[A그룹 - 노랑] 뒤쪽 방향 공격이 가능";
                    break;
                case ExpData.Types.DivinePoisonAdditionalDamage:
                    powerText.text = $"[A그룹 - 노랑] 중독상태의 적에게 50% 공격력이 증가";
                    break;
                case ExpData.Types.PhysicAdditionalWeapon:
                    powerText.text = $"[D그룹 - 보라] 검 1개 추가";
                    break;
                case ExpData.Types.PhysicIncreaseWeaponScale:
                    powerText.text = $"[D그룹 - 보라] 검의 크기가 100% 증가합니다.";
                    break;
                case ExpData.Types.PhysicSlowAdditionalDamage:
                    powerText.text = $"[D그룹 - 보라] 둔화된 적에게 데비지 100% 증가";
                    break;
                case ExpData.Types.PhysicAtkSpeed:
                    powerText.text = $"[D그룹 - 보라] 공격속도가 50% 증가";
                    break;
                case ExpData.Types.PhysicIncreaseDamage:
                    powerText.text = $"[D그룹 - 보라] 적을 죽이면 적 1기당\n모든 D그룹의 데미지가 5% 증가 (해당 웨이브만 적용)";
                    break;
                case ExpData.Types.PoisonDoubleAtk:
                    powerText.text = $"[F그룹 - 초록] 공격이 더블어택으로 변경";
                    break;
                case ExpData.Types.PoisonRestraintAdditionalDamage:
                    powerText.text = $"[F그룹 - 초록] 속박된 적에게 가하는 데미지 200% 증가";
                    break;
                case ExpData.Types.PoisonIncreaseTime:
                    powerText.text = $"[F그룹 - 초록] 중독시간이 2초 증가";
                    break;
                case ExpData.Types.PoisonInstantKill:
                    powerText.text = $"[F그룹 - 초록] 체력 15% 미만의 적은 15% 확률로 즉사";
                    break;
                case ExpData.Types.PoisonIncreaseAtkRange:
                    powerText.text = $"[F그룹 - 초록] 사거리 1칸 증가";
                    break;
                case ExpData.Types.WaterStun:
                    powerText.text = $"[E그룹 - 파랑] 명중시 15% 확률로 1초간 적 기절";
                    break;
                case ExpData.Types.WaterIncreaseSlowTime:
                    powerText.text = $"[E그룹 - 파랑] 둔화 지속시간 0.5초 증가";
                    break;
                case ExpData.Types.WaterIncreaseSlowPower:
                    powerText.text = $"[E그룹 - 파랑] 둔화강도 50% 증가";
                    break;
                case ExpData.Types.WaterRestraintKnockBack:
                    powerText.text = $"[E그룹 - 파랑] 속박된 적을 뒤로 1칸 밀쳐냄";
                    break;
                case ExpData.Types.WaterIncreaseDamage:
                    powerText.text = $"[E그룹 - 파랑] 공격력 50% 증가";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            powerCode.text = $"{powerUp.Code}";
            btnBadge.sprite = powerUp.BtnColor;
            expButton.image = expButton.image;
            expButton.onClick.RemoveAllListeners();
            expButton.onClick.AddListener(() => Selected(powerUp));
        }
    }
}
