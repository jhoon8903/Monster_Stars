using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.PuzzleManagerGroup;
using Script.UIManager;
using Script.WeaponScriptGroup;
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
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private SwipeManager swipeManager;
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private EnemyPatternManager enemyPatternManager;
        [SerializeField] private WeaponBase weaponBase;
        [SerializeField] private CharacterBase characterBase;
        [SerializeField] private WeaponsPool weaponsPool;
        [SerializeField] private AtkManager atkManager;
        [SerializeField] private EnforceManager enforceManager;
        private void ProcessExpReward(ExpData selectedReward)
        {
            switch (selectedReward.Type)
            {
                case ExpData.Types.GroupDamage: 
                    enforceManager.IncreaseGroupDamage(selectedReward.Property[0]);
                    break;
                case ExpData.Types.GroupAtkSpeed:
                    enforceManager.IncreaseGroupRate(selectedReward.Property[0]);
                    break;
                case ExpData.Types.StepLimit:
                    enforceManager.PermanentIncreaseMoveCount(selectedReward.Property[0]);
                    break;
                case ExpData.Types.StepDirection:
                    swipeManager.EnableDiagonalMovement(); 
                    characterManager.diagonalMovement = true;
                    break;
                case ExpData.Types.Exp:
                    enforceManager.IncreaseExpBuff(selectedReward.Property[0]);
                    break;
                case ExpData.Types.CastleRecovery:
                    enforceManager.recoveryCastle = true;
                    break;
                case ExpData.Types.CastleMaxHp:
                    enforceManager.IncreaseCastleMaxHp();
                    break;
                case ExpData.Types.Slow:
                    characterManager.slowCount += 1;
                    break;
                case ExpData.Types.NextStage:
                    enforceManager.NextCharacterUpgrade(selectedReward.Property[0]);
                    break;
                case ExpData.Types.Gold:
                    characterManager.goldGetMore = true; 
                    Debug.LogWarning($"Unhandled reward type: {selectedReward.Type}");
                    break;
                case ExpData.Types.DivineRestraint:
                    enemyPatternManager.IncreaseRestraintTime = true;
                    break;
                case ExpData.Types.DivinePenetrate:
                    weaponBase.DivinePenetrate = true;
                    break;
                case ExpData.Types.DivineAtkRange:
                    characterBase.DivineAtkRange = true;
                    break;
                case ExpData.Types.DivinePoisonAdditionalDamage:
                    weaponBase.DivinePoisonAdditionalDamage = true;
                    break;
                // 칼 추가 확인 완료
                case ExpData.Types.PhysicAdditionalWeapon:
                    weaponsPool.PhysicAdditionalWeapon = true;
                    break;
                case ExpData.Types.PhysicIncreaseWeaponScale:
                    characterBase.PhysicIncreaseWeaponScale = true;
                    weaponBase.PhysicIncreaseWeaponScale = true;
                    break;
                // 둔화 적 추가 데미지 적용
                case ExpData.Types.PhysicSlowAdditionalDamage:
                    weaponBase.PhysicSlowAdditionalDamage = true;
                    break;
                case ExpData.Types.PhysicAtkSpeed:
                    characterBase.PhysicAtkSpeed = true;
                    break;
                case ExpData.Types.PhysicIncreaseDamage:
                    characterBase.PhysicIncreaseDamage = true;
                    break;
                case ExpData.Types.PoisonDoubleAtk:
                    atkManager.PoisonDoubleAtk = true;
                    break;
                case ExpData.Types.PoisonRestraintAdditionalDamage:
                    weaponBase.PoisonRestraintAdditionalDamage = true;
                    break;
                case ExpData.Types.PoisonIncreaseTime:
                    weaponBase.PoisonIncreaseTime = true;
                    break;
                case ExpData.Types.PoisonInstantKill:
                    weaponBase.PoisonInstantKill = true;
                    break;
                case ExpData.Types.PoisonIncreaseAtkRange:
                    characterBase.PoisonIncreaseAtkRange = true;
                    break;
                case ExpData.Types.WaterStun:
                    enemyPatternManager.WaterStun = true;
                    break;
                case ExpData.Types.WaterIncreaseSlowTime:
                    enemyPatternManager.WaterIncreaseSlowTime = true;
                    break;
                case ExpData.Types.WaterIncreaseSlowPower:
                    enemyPatternManager.WaterIncreaseSlowPower = true;
                    break;
                case ExpData.Types.WaterRestraintKnockBack:
                    weaponBase.WaterRestraintKnockBack = true;
                    break;
                case ExpData.Types.WaterIncreaseDamage:
                    characterBase.WaterIncreaseDamage = true;
                    break;
                default: 
                    Debug.LogWarning($"Unhandled reward type: {selectedReward.Type}"); 
                    break;
            }
            selectedReward.ChosenProperty = null;
        }
        private void Selected(ExpData selectedReward)
        {
            levelUpRewardPanel.SetActive(false);
            gameManager.GameSpeed();
            // if (!spawnManager.isWave10Spawning)
            // {
            //     StartCoroutine(spawnManager.PositionUpCharacterObject());
            // }
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
        private ExpData LevelUpUnique(IEnumerable<ExpData> powerUpsData, ICollection<int> selectedCodes)
        {
            var validOptions = powerUpsData.Where(p => IsValidOption(p, selectedCodes));
            return SelectRandom(validOptions);
        }
        private bool IsValidOption (ExpData powerUp, ICollection<int> selectedCodes)
        {
            if (selectedCodes.Contains(powerUp.Code)) return false;
            switch (powerUp.Type)
            {
                case ExpData.Types.Slow:
                    if(characterManager.slowCount > 3) return false;
                    break;
                case ExpData.Types.Exp:
                    if (enforceManager.expPercentage > 30) return false;
                    break;
                case ExpData.Types.CastleRecovery:
                    if (enforceManager.recoveryCastle) return false;
                    break;
                case ExpData.Types.NextStage:
                    if (enforceManager.SelectedCount > 3) return false;
                    break;
                case ExpData.Types.StepDirection:
                    if (characterManager.diagonalMovement || gameManager.wave !=11) return false;
                    break;
                case ExpData.Types.StepLimit:
                    if (enforceManager.permanentIncreaseMovementCount > 3) return false;
                    break;
                case ExpData.Types.CastleMaxHp:
                    if (enforceManager.castleMaxHp >= 1000) return false;
                    break;
                case ExpData.Types.DivineRestraint:
                    if (enemyPatternManager.IncreaseRestraintTime) return false;
                    break;
                case ExpData.Types.DivinePenetrate:
                    if (weaponBase.DivinePenetrate) return false;
                    break;
                case ExpData.Types.DivineAtkRange:
                    if(characterBase.DivineAtkRange) return false;
                    break;
                case ExpData.Types.DivinePoisonAdditionalDamage:
                    if(weaponBase.DivinePoisonAdditionalDamage) return false;
                    break;
                case ExpData.Types.PhysicAdditionalWeapon:
                    if(weaponsPool.PhysicAdditionalWeapon) return false;
                    break;
                case ExpData.Types.PhysicIncreaseWeaponScale:
                    if(characterBase.PhysicIncreaseWeaponScale) return false;
                    if(weaponBase.PhysicIncreaseWeaponScale) return false;
                    break;
                case ExpData.Types.PhysicSlowAdditionalDamage:
                    if(weaponBase.PhysicSlowAdditionalDamage) return false;
                    break;
                case ExpData.Types.PhysicAtkSpeed:
                    if(characterBase.PhysicAtkSpeed) return false;
                    break;
                case ExpData.Types.PhysicIncreaseDamage:
                    if(characterBase.PhysicIncreaseDamage) return false;
                    break;
                case ExpData.Types.PoisonDoubleAtk:
                    if(atkManager.PoisonDoubleAtk) return false;
                    break;
                case ExpData.Types.PoisonRestraintAdditionalDamage:
                    if(weaponBase.PoisonRestraintAdditionalDamage) return false;
                    break;                                                                    
                case ExpData.Types.PoisonIncreaseTime:
                    if(weaponBase.PoisonIncreaseTime) return false;
                    break;
                case ExpData.Types.PoisonInstantKill:
                    if(weaponBase.PoisonInstantKill) return false;
                    break;
                case ExpData.Types.PoisonIncreaseAtkRange:
                    if(characterBase.PoisonIncreaseAtkRange) return false;
                    break;
                case ExpData.Types.WaterStun:
                    if(enemyPatternManager.WaterStun) return false;
                    break;
                case ExpData.Types.WaterIncreaseSlowTime:
                    if(enemyPatternManager.WaterIncreaseSlowTime) return false;
                    break;
                case ExpData.Types.WaterIncreaseSlowPower:
                    if(enemyPatternManager.WaterIncreaseSlowPower) return false;
                    break;
                case ExpData.Types.WaterRestraintKnockBack:
                    if(weaponBase.WaterRestraintKnockBack) return false;
                    break;
                case ExpData.Types.WaterIncreaseDamage:
                    if(characterBase.WaterIncreaseDamage) return false;
                    break;
                default:
                    return true;
            }
            return true;
        }
        private static ExpData SelectRandom(IEnumerable<ExpData> validOptions)
        {
            var commonDataList = validOptions.ToList();
            var count = commonDataList.Count();
            if (count == 0) return null;
            var randomIndex = Random.Range(0, count);
            return commonDataList.ElementAt(randomIndex);
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
                    powerText.text = $"적 처치시 경험치 획득량 {p}% 증가\n(현재 {enforceManager.expPercentage}% 최대 30%)";
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
                    powerText.text = $"보스 스테이지 이후 {p} 개의\n케릭터 추가 이동 (현재 {enforceManager.highLevelCharacterCount})";
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
                    break;
            }
            powerCode.text = $"{powerUp.Code}";
            btnBadge.sprite = powerUp.BtnColor;
            expButton.image = expButton.image;
            expButton.onClick.RemoveAllListeners();
            expButton.onClick.AddListener(() => Selected(powerUp));
        }
    }
}
