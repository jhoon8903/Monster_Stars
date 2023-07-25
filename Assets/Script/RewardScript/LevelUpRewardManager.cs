using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
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
        [SerializeField] private ExpManager expManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private Language language;
        private ExpData _selectedPowerUp;
        public static LevelUpRewardManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        private static void ProcessExpReward(ExpData selectedReward)
        {
            switch (selectedReward.Type)
            {
                case ExpData.Types.GroupDamage: 
                    EnforceManager.Instance.IncreaseGroupDamage(selectedReward.Property[0]);
                    break;
                case ExpData.Types.GroupAtkSpeed:
                    EnforceManager.Instance.IncreaseGroupRate(selectedReward.Property[0]);
                    break;
                case ExpData.Types.LevelUpStep:
                    EnforceManager.Instance.RewardMoveCount(selectedReward.Property[0]);
                    break;
                case ExpData.Types.StepDirection:
                    EnforceManager.Instance.diagonalMovement = true; 
                    break;
                case ExpData.Types.Exp:
                    EnforceManager.Instance.IncreaseExpBuff(selectedReward.Property[0]);
                    break;
                case ExpData.Types.CastleRecovery:
                    EnforceManager.Instance.recoveryCastle = true;
                    break;
                case ExpData.Types.CastleMaxHp:
                    EnforceManager.Instance.IncreaseCastleMaxHp();
                    break;
                case ExpData.Types.Slow:
                    EnforceManager.Instance.slowCount += 1;
                    break;
                case ExpData.Types.NextStage:
                    EnforceManager.Instance.NextCharacterUpgrade(selectedReward.Property[0]);
                    break;
                case ExpData.Types.Gold:
                    EnforceManager.Instance.addGold = true;
                    break;
               case ExpData.Types.DivinePoisonDamageBoost:
                   EnforceManager.Instance.divinePoisonDamageBoost = true;
                   break;
                case ExpData.Types.DivineBindDurationBoost:
                    EnforceManager.Instance.DivineBindDurationIncrease();
                    break;
                case ExpData.Types.DivineShackledExplosion:
                    EnforceManager.Instance.divineShackledExplosion = true;
                    break;
                case ExpData.Types.DivineFifthAttackBoost:
                    EnforceManager.Instance.divineFifthAttackBoost = true;
                    break;
                case ExpData.Types.DivineAttackBoost:
                    EnforceManager.Instance.DivineAttackDamageIncrease();
                    break;
                case ExpData.Types.DivineBindChanceBoost:
                    EnforceManager.Instance.divineBindChanceBoost = true;
                    break;
                case ExpData.Types.DivineDualAttack:
                    EnforceManager.Instance.divineDualAttack = true;
                    break;
                case ExpData.Types.DivineProjectilePierce:
                    EnforceManager.Instance.divineProjectilePierce = true;
                    break;
                case ExpData.Types.DarkTenthAttackDamageBoost:
                    EnforceManager.Instance.darkTenthAttackDamageBoost = true;
                    break;
                case ExpData.Types.DarkAttackSpeedBoost:
                    EnforceManager.Instance.DarkAttackSpeedIncrease();
                    break;
                case ExpData.Types.DarkAttackPowerBoost:
                    EnforceManager.Instance.DarkAttackDamageIncrease();
                    break;
                case ExpData.Types.DarkStatusAilmentDamageChance:
                    EnforceManager.Instance.darkStatusAilmentDamageChance = true;
                    break;
                case ExpData.Types.DarkKnockBackChance:
                    EnforceManager.Instance.darkKnockBackChance = true;
                    break;
                case ExpData.Types.DarkStatusAilmentDamageBoost:
                    EnforceManager.Instance.darkStatusAilmentDamageBoost = true;
                    break;
                case ExpData.Types.DarkRangeIncrease:
                    EnforceManager.Instance.darkRangeIncrease = true;
                    break;
                case ExpData.Types.DarkStatusAilmentSlowEffect:
                    EnforceManager.Instance.darkStatusAilmentSlowEffect = true;
                    break;
                case ExpData.Types.WaterAttackSpeedBoost:
                    EnforceManager.Instance.WaterAttackSpeedIncrease();
                    break;
                case ExpData.Types.WaterAllyDamageBoost:
                    EnforceManager.Instance.waterAllyDamageBoost = true;
                    break;
                case ExpData.Types.WaterProjectileIncrease:
                    EnforceManager.Instance.waterProjectileIncrease = true;
                    break;
                case ExpData.Types.WaterAttackBoost:
                    EnforceManager.Instance.WaterAttackDamageIncrease();
                    break;
                case ExpData.Types.WaterSlowEnemyDamageBoost:
                    EnforceManager.Instance.waterSlowEnemyDamageBoost = true;
                    break;
                case ExpData.Types.WaterGlobalSlowEffect:
                    EnforceManager.Instance.waterGlobalSlowEffect = true;
                    break;
                case ExpData.Types.WaterSlowEnemyStunChance:
                    EnforceManager.Instance.waterSlowEnemyStunChance = true;
                    break;
                case ExpData.Types.WaterDamageIncreaseDebuff:
                    EnforceManager.Instance.waterDamageIncreaseDebuff = true;
                    break;
                case ExpData.Types.PhysicalAttackSpeedBoost:
                    EnforceManager.Instance.PhysicalAttackSpeedIncrease();
                    break;
                case ExpData.Types.PhysicalDamage35Boost:
                    EnforceManager.Instance.physicalDamage35Boost = true;
                    break;
                case ExpData.Types.PhysicalDamage6Boost:
                    EnforceManager.Instance.PhysicalAttackDamageIncrease();
                    break;
                case ExpData.Types.PhysicalBleedingChance:
                    EnforceManager.Instance.physicalBleedingChance = true;
                    break;
                case ExpData.Types.PhysicalSwordAddition:
                    EnforceManager.Instance.physicalSwordAddition = true;
                    break;
                case ExpData.Types.PhysicalSlowEnemyDamageBoost:
                    EnforceManager.Instance.physicalSlowEnemyDamageBoost = true;
                    break;
                case ExpData.Types.PhysicalSwordScaleIncrease:
                    EnforceManager.Instance.physicalSwordScaleIncrease = true;
                    break;
                case ExpData.Types.PhysicalDamage18Boost:
                    EnforceManager.Instance.physicalDamage18Boost = true;
                    break;
                case ExpData.Types.Water2DebuffDurationIncrease:
                    EnforceManager.Instance.Water2DebuffDurationIncrease();
                    break;
                case ExpData.Types.Water2AttackSpeedIncrease:
                    EnforceManager.Instance.Water2AttackSpeedIncrease();
                    break;
                case ExpData.Types.Water2StunChanceAgainstBleeding:
                    EnforceManager.Instance.water2StunChanceAgainstBleeding = true;
                    break;
                case ExpData.Types.Water2IceSpikeProjectile:
                    EnforceManager.Instance.water2IceSpikeProjectile = true;
                    break;
                case ExpData.Types.Water2AttackPowerIncrease:
                    EnforceManager.Instance.Water2AttackPowerIncrease();
                    break;
                case ExpData.Types.Water2ProjectileSpeedIncrease:
                    EnforceManager.Instance.water2ProjectileSpeedIncrease = true;
                    break;
                case ExpData.Types.Water2DebuffStrengthIncrease:
                    EnforceManager.Instance.water2DebuffStrengthIncrease = true;
                    break;
                case ExpData.Types.Water2AttackSpeedBuffToAdjacentAllies:
                    EnforceManager.Instance.water2AttackSpeedBuffToAdjacentAllies = true;
                    break;
                case ExpData.Types.PoisonAttackSpeedIncrease:
                    EnforceManager.Instance.PoisonAttackSpeedIncrease();
                    break;
                case ExpData.Types.PoisonMaxStackIncrease:
                    EnforceManager.Instance.PoisonMaxStackIncrease();
                    break;
                case ExpData.Types.PoisonDamageAttackPowerIncrease:
                    EnforceManager.Instance.PoisonDamageAttackPowerIncrease();
                    break;
                case ExpData.Types.PoisonProjectileIncrease:
                    EnforceManager.Instance.poisonProjectileIncrease = true;
                    break;
                case ExpData.Types.PoisonRangeIncrease:
                    EnforceManager.Instance.poisonRangeIncrease = true;
                    break;
                case ExpData.Types.PoisonBleedingEnemyDamageBoost:
                    EnforceManager.Instance.poisonBleedingEnemyDamageBoost = true;
                    break;
                case ExpData.Types.PoisonEnemyInstantKill:
                    EnforceManager.Instance.poisonEnemyInstantKill = true;
                    break;
                case ExpData.Types.PoisonPerHitEffect:
                    EnforceManager.Instance.poisonPerHitEffect = true;
                    break;
                case ExpData.Types.Fire2PoisonDamageIncrease:
                    EnforceManager.Instance.fire2PoisonDamageIncrease = true;
                    break;
                case ExpData.Types.Fire2AttackSpeedIncrease:
                    EnforceManager.Instance.Fire2AttackSpeedIncrease();
                    break;
                case ExpData.Types.Fire2BurnStackIncrease:
                    EnforceManager.Instance.fire2BurnStackIncrease = true;
                    break;
                case ExpData.Types.Fire2AttackPowerIncrease:
                    EnforceManager.Instance.Fire2AttackPowerIncrease();
                    break;
                case ExpData.Types.Fire2StunChance:
                    EnforceManager.Instance.fire2StunChance = true;
                    break;
                case ExpData.Types.Fire2SwordSizeIncrease:
                    EnforceManager.Instance.fire2SwordSizeIncrease = true;
                    break;
                case ExpData.Types.Fire2BurningDamageBoost:
                    EnforceManager.Instance.Fire2BurningDamageBoost();
                    break;
                case ExpData.Types.Fire2NoBurnDamageIncrease:
                    EnforceManager.Instance.fire2NoBurnDamageIncrease = true;
                    break;
                case ExpData.Types.FireImageOverlapIncrease:
                    EnforceManager.Instance.FireOverLapIncrease();
                    break;
                case ExpData.Types.FireAttackSpeedBoost:
                    EnforceManager.Instance.FireAttackSpeedBoost();
                    break;
                case ExpData.Types.FireSlowEnemyDamageBoost:
                    EnforceManager.Instance.fireSlowEnemyDamageBoost = true;
                    break;
                case ExpData.Types.FireProjectileSpeedIncrease:
                    EnforceManager.Instance.fireProjectileSpeedIncrease = true;
                    break;
                case ExpData.Types.FireBurnedEnemyExplosion:
                    EnforceManager.Instance.fireBurnedEnemyExplosion = true;
                    break;
                case ExpData.Types.FireProjectileBounceDamage:
                    EnforceManager.Instance.fireProjectileBounceDamage = true;
                    break;
                case ExpData.Types.FireBurnPerAttackEffect:
                    EnforceManager.Instance.fireBurnPerAttackEffect = true;
                    break;
                case ExpData.Types.FireProjectileBounceIncrease:
                    EnforceManager.Instance.fireProjectileBounceIncrease = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            selectedReward.ChosenProperty = null;
        }
        private void Selected(ExpData selectedReward)
        {
            levelUpRewardPanel.SetActive(false);
            gameManager.GameSpeed();
            ProcessExpReward(selectedReward);
        }
        public IEnumerator LevelUpReward() // 레벨업 보상 처리
        {
            Time.timeScale = 0; // 게임 일시 정지
            levelUpRewardPanel.SetActive(true); // 보물 패널 활성화
            var playerLevel = expManager.level;
            if (playerLevel <= 9)
            {
                yield return StartCoroutine(LevelUpChance(60, 35,5));
            }
            else
            {
                yield return StartCoroutine(LevelUpChance(50, 40, 10));
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

                if (randomValue < greenChance)
                {
                    _selectedPowerUp = LevelUpUnique(exp.ExpGreenList, selectedCodes);
                }
                else if (randomValue < greenChance + blueChance)
                {
                    _selectedPowerUp = LevelUpUnique(exp.ExpBlueList, selectedCodes);
                }
                else
                {
                    _selectedPowerUp = LevelUpUnique(exp.ExpPurpleList, selectedCodes);
                }
                if (_selectedPowerUp == null) continue;
                levelUpPowerUps.Add(_selectedPowerUp);
                selectedCodes.Add(_selectedPowerUp.Code);
            }
            return levelUpPowerUps;
        }
        private ExpData LevelUpUnique(IEnumerable<ExpData> powerUpsData, ICollection<int> selectedCodes)
        {
            var validOptions = powerUpsData.Where(p => IsValidOption(p, selectedCodes));
            return SelectRandom(validOptions);
        }
        public bool HasUnitInGroup(CharacterBase.UnitGroups group)
        {
            return characterManager.characterList.Any(unit => unit.unitGroup == group);
        }
        private int UnitPieceLevel(CharacterBase.UnitGroups group)
        {
            var unitLevel = new int();
            foreach (var unit in characterManager.characterList.Where(unit => unit.unitGroup == group))
            {
                unitLevel = unit.unitPieceLevel;
            }
            return unitLevel;
        }
        private bool IsValidOption (ExpData powerUp, ICollection<int> selectedCodes)
        {
            if (selectedCodes.Contains(powerUp.Code)) return false;
            switch (powerUp.Type)
            {
                case ExpData.Types.Gold:
                    if (EnforceManager.Instance.addGold) return false;
                    break;
                case ExpData.Types.Slow:
                    if (EnforceManager.Instance.slowCount >= 4) return false;
                    break;
                case ExpData.Types.Exp:
                    if (EnforceManager.Instance.expPercentage >= 30) return false;
                    break;
                case ExpData.Types.CastleRecovery:
                    if (EnforceManager.Instance.recoveryCastle) return false;
                    break;
                case ExpData.Types.NextStage:
                    if (EnforceManager.Instance.selectedCount > 3) return false;
                    break;
                case ExpData.Types.StepDirection:
                    if (EnforceManager.Instance.diagonalMovement) return false;
                    if (!StageManager.Instance.isBossClear) return false;
                    if (StageManager.Instance.currentWave % 10 != 0 ) return false;
                    break;
                case ExpData.Types.CastleMaxHp:
                    if (EnforceManager.Instance.castleMaxHp >= 1000) return false;
                    break;
                // Unit A
                case ExpData.Types.DivinePoisonDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                    if (EnforceManager.Instance.divinePoisonDamageBoost) return false;
                    break;
                case ExpData.Types.DivineBindDurationBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 7) return false;
                    if (EnforceManager.Instance.divineBindDurationBoost >= 0.5) return false;
                    break;
                case ExpData.Types.DivineShackledExplosion:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 3) return false;
                    if (EnforceManager.Instance.divineShackledExplosion) return false;
                    break;
                case ExpData.Types.DivineFifthAttackBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                    if (EnforceManager.Instance.divineFifthAttackBoost) return false;
                    break;
                case ExpData.Types.DivineAttackBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 13) return false;
                    if (EnforceManager.Instance.divineAttackBoost >= 4) return false;
                    break;
                case ExpData.Types.DivineBindChanceBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 5) return false;
                    if (EnforceManager.Instance.divineBindChanceBoost) return false;
                    break;
                case ExpData.Types.DivineDualAttack:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 11) return false;
                    if (EnforceManager.Instance.divineDualAttack) return false;
                    break;
                case ExpData.Types.DivineProjectilePierce:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 9) return false;
                    if (EnforceManager.Instance.divineProjectilePierce) return false;
                    break;
                // Unit B
                case ExpData.Types.DarkTenthAttackDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 5) return false;
                    if (EnforceManager.Instance.darkTenthAttackDamageBoost) return false;
                    break;
                case ExpData.Types.DarkAttackSpeedBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                    if (EnforceManager.Instance.darkAttackSpeedBoost >= 4) return false;
                    break;
                case ExpData.Types.DarkAttackPowerBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 9) return false;
                    if (EnforceManager.Instance.darkAttackPowerBoost >= 4) return false;
                    break;
                case ExpData.Types.DarkStatusAilmentDamageChance:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 11) return false;
                    if (EnforceManager.Instance.darkStatusAilmentDamageChance) return false;
                    break;
                case ExpData.Types.DarkKnockBackChance:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 3) return false;
                    if (EnforceManager.Instance.darkKnockBackChance) return false;
                    break;
                case ExpData.Types.DarkStatusAilmentDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                    if (EnforceManager.Instance.darkStatusAilmentDamageBoost) return false;
                    break;
                case ExpData.Types.DarkRangeIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 7) return false;
                    if (EnforceManager.Instance.darkRangeIncrease) return false;
                    break;
                case ExpData.Types.DarkStatusAilmentSlowEffect:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 13) return false;
                    if (EnforceManager.Instance.darkStatusAilmentSlowEffect) return false;
                    break;
                // Unit C
                case ExpData.Types.WaterAttackSpeedBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 5) return false;
                    if (EnforceManager.Instance.waterAttackSpeedBoost >= 4) return false;
                    break;
                case ExpData.Types.WaterAllyDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                    if (EnforceManager.Instance.waterAllyDamageBoost) return false;
                    break;
                case ExpData.Types.WaterProjectileIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 11) return false;
                    if (EnforceManager.Instance.waterProjectileIncrease) return false;
                    break;
                case ExpData.Types.WaterAttackBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 13) return false;
                    if (EnforceManager.Instance.waterAttackBoost >= 4) return false;
                    break;
                case ExpData.Types.WaterSlowEnemyDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 7) return false;
                    if (EnforceManager.Instance.waterSlowEnemyDamageBoost) return false;
                    break;
                case ExpData.Types.WaterGlobalSlowEffect:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                    if (EnforceManager.Instance.waterGlobalSlowEffect) return false;
                    break;
                case ExpData.Types.WaterSlowEnemyStunChance:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 3) return false;
                    if (EnforceManager.Instance.waterSlowEnemyStunChance) return false;
                    break;
                case ExpData.Types.WaterDamageIncreaseDebuff:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 9) return false;
                    if (EnforceManager.Instance.waterDamageIncreaseDebuff) return false;
                    break;
                // //Unit D
                case ExpData.Types.PhysicalAttackSpeedBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 9) return false;
                    if (EnforceManager.Instance.physicalAttackSpeedBoost >= 4) return false;
                    break;
                case ExpData.Types.PhysicalDamage35Boost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                    if (EnforceManager.Instance.physicalDamage35Boost) return false;
                    break;
                case ExpData.Types.PhysicalDamage6Boost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 5) return false;
                    if (EnforceManager.Instance.physicalDamage6Boost >= 4) return false;
                    break;
                case ExpData.Types.PhysicalBleedingChance:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 11) return false;
                    if (EnforceManager.Instance.physicalBleedingChance) return false;
                    break;
                case ExpData.Types.PhysicalSwordAddition:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                    if (EnforceManager.Instance.physicalSwordAddition) return false;
                    break;
                case ExpData.Types.PhysicalSlowEnemyDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 7) return false;
                    break;
                case ExpData.Types.PhysicalSwordScaleIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 3) return false;
                    break;
                case ExpData.Types.PhysicalDamage18Boost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 13) return false;
                    break;
                // //Unit E
                case ExpData.Types.Water2DebuffDurationIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                    if (EnforceManager.Instance.water2DebuffDurationIncrease >= 5) return false;
                    break;
                case ExpData.Types.Water2AttackSpeedIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 7) return false;
                    if (EnforceManager.Instance.water2AttackSpeedIncrease >= 4) return false;
                    break;
                case ExpData.Types.Water2StunChanceAgainstBleeding:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 11) return false;
                    if (EnforceManager.Instance.water2StunChanceAgainstBleeding) return false;
                    break;
                case ExpData.Types.Water2IceSpikeProjectile:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 9) return false;
                    if (EnforceManager.Instance.water2IceSpikeProjectile) return false;
                    break;
                case ExpData.Types.Water2AttackPowerIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 5) return false;
                    if (EnforceManager.Instance.water2AttackPowerIncrease >= 4) return false;
                    break;
                case ExpData.Types.Water2ProjectileSpeedIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 3) return false;
                    if (EnforceManager.Instance.water2ProjectileSpeedIncrease) return false;
                    break;
                case ExpData.Types.Water2DebuffStrengthIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                    if (EnforceManager.Instance.water2DebuffStrengthIncrease) return false;
                    break;
                case ExpData.Types.Water2AttackSpeedBuffToAdjacentAllies:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 13) return false;
                    if (EnforceManager.Instance.water2AttackSpeedBuffToAdjacentAllies) return false;
                    break;
                // //Unit F
                case ExpData.Types.PoisonAttackSpeedIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                    if (EnforceManager.Instance.poisonAttackSpeedIncrease >= 4) return false;
                    break;
                case ExpData.Types.PoisonMaxStackIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 3) return false;
                    if (EnforceManager.Instance.poisonMaxStackIncrease >= 5) return false;
                    break;
                case ExpData.Types.PoisonDamageAttackPowerIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 11) return false;
                    if (EnforceManager.Instance.poisonDamageAttackPowerIncrease >= 3) return false;
                    break;
                case ExpData.Types.PoisonProjectileIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 5) return false;
                    if (EnforceManager.Instance.poisonProjectileIncrease) return false;
                    break;
                case ExpData.Types.PoisonRangeIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 9) return false;
                    if (EnforceManager.Instance.poisonRangeIncrease) return false;
                    break;
                case ExpData.Types.PoisonBleedingEnemyDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 13) return false;
                    if (EnforceManager.Instance.poisonBleedingEnemyDamageBoost) return false;
                    break;
                case ExpData.Types.PoisonEnemyInstantKill:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                    if (EnforceManager.Instance.poisonEnemyInstantKill) return false;
                    break;
                case ExpData.Types.PoisonPerHitEffect:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 7) return false;
                    if (EnforceManager.Instance.poisonPerHitEffect) return false;
                    break;
                // //Unit G
                case ExpData.Types.Fire2PoisonDamageIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.G)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.G) < 3) return false;
                    if (EnforceManager.Instance.fire2PoisonDamageIncrease) return false;
                    break;
                case ExpData.Types.Fire2AttackSpeedIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.G)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.G) < 9) return false;
                    if (EnforceManager.Instance.fire2AttackSpeedIncrease >= 4) return false;
                    break;
                case ExpData.Types.Fire2BurnStackIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.G)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.G) < 11) return false;
                    if (EnforceManager.Instance.fire2BurnStackIncrease) return false;
                    break;
                case ExpData.Types.Fire2AttackPowerIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.G)) return false;
                    if (EnforceManager.Instance.fire2AttackPowerIncrease >= 4) return false;
                    break;
                case ExpData.Types.Fire2StunChance:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.G)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.G) < 13) return false;
                    if (EnforceManager.Instance.fire2StunChance) return false;
                    break;
                case ExpData.Types.Fire2SwordSizeIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.G)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.G) < 5) return false;
                    if (EnforceManager.Instance.fire2SwordSizeIncrease) return false;
                    break;
                case ExpData.Types.Fire2BurningDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.G)) return false;
                    if (EnforceManager.Instance.fire2BurningDamageBoost >= 3) return false;
                    break;
                case ExpData.Types.Fire2NoBurnDamageIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.G)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.G) < 7) return false;
                    if (EnforceManager.Instance.fire2NoBurnDamageIncrease) return false;
                    break;
                // //Unit H
                case ExpData.Types.FireImageOverlapIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.H)) return false;
                    if (EnforceManager.Instance.fireImageOverlapIncrease >= 5) return false;
                    break;
                case ExpData.Types.FireAttackSpeedBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.H)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.H) < 9) return false;
                    if (EnforceManager.Instance.fireAttackSpeedBoost >= 4) return false;
                    break;
                case ExpData.Types.FireSlowEnemyDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.H)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.H) < 11) return false;
                    if (EnforceManager.Instance.fireSlowEnemyDamageBoost) return false;
                    break;
                case ExpData.Types.FireProjectileSpeedIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.H)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.H) < 3) return false;
                    if (EnforceManager.Instance.fireProjectileSpeedIncrease) return false;
                    break;
                case ExpData.Types.FireBurnedEnemyExplosion:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.H)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.H) < 7) return false;
                    if (EnforceManager.Instance.fireBurnedEnemyExplosion) return false;
                    break;
                case ExpData.Types.FireProjectileBounceDamage:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.H)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.H) < 5) return false;
                    if (EnforceManager.Instance.fireProjectileBounceDamage) return false;
                    break;
                case ExpData.Types.FireBurnPerAttackEffect:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.H)) return false;
                    if (EnforceManager.Instance.fireBurnPerAttackEffect) return false;
                    break;
                case ExpData.Types.FireProjectileBounceIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.H)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.H) < 13) return false;
                    if (EnforceManager.Instance.fireProjectileBounceIncrease) return false;
                    break;
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
            LevelUpDisplayText(exp1Button, exp1Text, exp1Code, exp1BtnBadge, powerUps[0], language);
            LevelUpDisplayText(exp2Button, exp2Text, exp2Code, exp2BtnBadge, powerUps[1], language);
            LevelUpDisplayText(exp3Button, exp3Text, exp3Code, exp3BtnBadge, powerUps[2], language);
        }
        private void LevelUpDisplayText(Button expButton, TMP_Text powerText, TMP_Text powerCode, Image btnBadge, ExpData powerUp, Language selectedLanguage)
        {
            var translationKey = powerUp.Type.ToString();
            var powerTextTranslation = selectedLanguage.GetTranslation(translationKey);
            var p = powerUp.Property[0].ToString();
            var finalPowerText = powerTextTranslation.Replace("{p}", p);
            var placeholderValues = new Dictionary<string, Func<double>> {
                { "{15*EnforceManager.Instance.slowCount}", () => 15 * EnforceManager.Instance.slowCount },
                { "{EnforceManager.Instance.expPercentage}", () => EnforceManager.Instance.expPercentage },
                { "{EnforceManager.Instance.highLevelCharacterCount}", () => EnforceManager.Instance.highLevelCharacterCount},

            };
            finalPowerText = placeholderValues.Aggregate(finalPowerText, (current, placeholder) 
                => current.Replace(placeholder.Key, placeholder.Value().ToString()));
            var finalTranslation = finalPowerText.Replace("||", "\n");

            powerText.text = powerUp.Type switch
            {
                ExpData.Types.GroupDamage => finalTranslation,
                ExpData.Types.GroupAtkSpeed => finalTranslation,
                ExpData.Types.StepDirection => finalTranslation,
                ExpData.Types.LevelUpStep => finalTranslation,
                ExpData.Types.Exp => finalTranslation,
                ExpData.Types.CastleRecovery => finalTranslation,
                ExpData.Types.CastleMaxHp => finalTranslation,
                ExpData.Types.Slow => finalTranslation,
                ExpData.Types.NextStage => finalTranslation,
                ExpData.Types.Gold => finalTranslation,
                ExpData.Types.DivinePoisonDamageBoost => finalTranslation,
                ExpData.Types.DivineBindDurationBoost => finalTranslation,
                ExpData.Types.DivineShackledExplosion => finalTranslation,
                ExpData.Types.DivineFifthAttackBoost => finalTranslation,
                ExpData.Types.DivineAttackBoost => finalTranslation,
                ExpData.Types.DivineBindChanceBoost => finalTranslation,
                ExpData.Types.DivineDualAttack => finalTranslation,
                ExpData.Types.DivineProjectilePierce => finalTranslation,
                ExpData.Types.DarkTenthAttackDamageBoost => finalTranslation,
                ExpData.Types.DarkAttackSpeedBoost => finalTranslation,
                ExpData.Types.DarkAttackPowerBoost => finalTranslation,
                ExpData.Types.DarkStatusAilmentDamageChance => finalTranslation,
                ExpData.Types.DarkKnockBackChance => finalTranslation,
                ExpData.Types.DarkStatusAilmentDamageBoost => finalTranslation,
                ExpData.Types.DarkRangeIncrease => finalTranslation,
                ExpData.Types.DarkStatusAilmentSlowEffect => finalTranslation,
                ExpData.Types.WaterAttackSpeedBoost => finalTranslation,
                ExpData.Types.WaterAllyDamageBoost => finalTranslation,
                ExpData.Types.WaterProjectileIncrease => finalTranslation,
                ExpData.Types.WaterAttackBoost => finalTranslation,
                ExpData.Types.WaterSlowEnemyDamageBoost => finalTranslation,
                ExpData.Types.WaterGlobalSlowEffect => finalTranslation,
                ExpData.Types.WaterSlowEnemyStunChance => finalTranslation,
                ExpData.Types.WaterDamageIncreaseDebuff => finalTranslation,
                ExpData.Types.PhysicalAttackSpeedBoost => finalTranslation,
                ExpData.Types.PhysicalDamage35Boost => finalTranslation,
                ExpData.Types.PhysicalDamage6Boost => finalTranslation,
                ExpData.Types.PhysicalBleedingChance => finalTranslation,
                ExpData.Types.PhysicalSwordAddition => finalTranslation,
                ExpData.Types.PhysicalSlowEnemyDamageBoost => finalTranslation,
                ExpData.Types.PhysicalSwordScaleIncrease => finalTranslation,
                ExpData.Types.PhysicalDamage18Boost => finalTranslation,
                ExpData.Types.Water2DebuffDurationIncrease => finalTranslation,
                ExpData.Types.Water2AttackSpeedIncrease => finalTranslation,
                ExpData.Types.Water2StunChanceAgainstBleeding => finalTranslation,
                ExpData.Types.Water2IceSpikeProjectile => finalTranslation,
                ExpData.Types.Water2AttackPowerIncrease => finalTranslation,
                ExpData.Types.Water2ProjectileSpeedIncrease => finalTranslation,
                ExpData.Types.Water2DebuffStrengthIncrease => finalTranslation,
                ExpData.Types.Water2AttackSpeedBuffToAdjacentAllies => finalTranslation,
                ExpData.Types.PoisonAttackSpeedIncrease => finalTranslation,
                ExpData.Types.PoisonMaxStackIncrease => finalTranslation,
                ExpData.Types.PoisonDamageAttackPowerIncrease => finalTranslation,
                ExpData.Types.PoisonProjectileIncrease => finalTranslation,
                ExpData.Types.PoisonRangeIncrease => finalTranslation,
                ExpData.Types.PoisonBleedingEnemyDamageBoost => finalTranslation,
                ExpData.Types.PoisonEnemyInstantKill => finalTranslation,
                ExpData.Types.PoisonPerHitEffect => finalTranslation,
                ExpData.Types.Fire2PoisonDamageIncrease => finalTranslation,
                ExpData.Types.Fire2AttackSpeedIncrease => finalTranslation,
                ExpData.Types.Fire2BurnStackIncrease => finalTranslation,
                ExpData.Types.Fire2AttackPowerIncrease => finalTranslation,
                ExpData.Types.Fire2StunChance => finalTranslation,
                ExpData.Types.Fire2SwordSizeIncrease => finalTranslation,
                ExpData.Types.Fire2BurningDamageBoost => finalTranslation,
                ExpData.Types.Fire2NoBurnDamageIncrease => finalTranslation,
                ExpData.Types.FireImageOverlapIncrease => finalTranslation,
                ExpData.Types.FireAttackSpeedBoost => finalTranslation,
                ExpData.Types.FireSlowEnemyDamageBoost => finalTranslation,
                ExpData.Types.FireProjectileSpeedIncrease => finalTranslation,
                ExpData.Types.FireBurnedEnemyExplosion => finalTranslation,
                ExpData.Types.FireProjectileBounceDamage => finalTranslation,
                ExpData.Types.FireBurnPerAttackEffect => finalTranslation,
                ExpData.Types.FireProjectileBounceIncrease => finalTranslation,
            };
            powerCode.text = $"{powerUp.Code}";
            btnBadge.sprite = powerUp.BtnColor;
            expButton.image = expButton.image;
            expButton.onClick.RemoveAllListeners();
            expButton.onClick.AddListener(() => Selected(powerUp));
        }
    }
}
