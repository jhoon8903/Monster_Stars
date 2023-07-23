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
                    if (EnforceManager.Instance.expPercentage > 30) return false;
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
                case ExpData.Types.StepDirection:
                    powerText.text = "대각선 이동 가능"; 
                    break;
                case ExpData.Types.LevelUpStep:
                    powerText.text = $"다음 Wave 이동횟수 {p} 증가";
                    break;
                case ExpData.Types.Exp:
                    powerText.text = $"적 처치시 경험치 획득량 {p}% 증가\n(현재 {EnforceManager.Instance.expPercentage}% 최대 30%)";
                    break;
                case ExpData.Types.CastleRecovery:
                    powerText.text = $"웨이브 종료까지 피해를 입지 않으면\n캐슬 체력 {p} 매 웨이브 마다 증가";
                    break;
                case ExpData.Types.CastleMaxHp:
                    powerText.text = $"캐슬 최대체력 {p} 증가"; 
                    break;
                case ExpData.Types.Slow:
                    powerText.text = $"다음 웨이브 부터는 적 이동속도 {p}% 감소\n( 현재 {15*EnforceManager.Instance.slowCount}% / 최대 60%)";
                    break;
                case ExpData.Types.NextStage:
                    powerText.text = $"보스 스테이지 이후 {p} 개의\n케릭터 추가 이동 (현재 {EnforceManager.Instance.highLevelCharacterCount})";
                    break;
                case ExpData.Types.Gold:
                    powerText.text = "5 매치시 골드 1추가 획득";
                    break;
                case ExpData.Types.DivinePoisonDamageBoost:
                    powerText.text = "중독상태의 적을 공격 시 50% 추가데미지";
                    break;
                case ExpData.Types.DivineBindDurationBoost:
                    powerText.text = $"속박 지속시간 0.1초 증가 (현재: 0.{EnforceManager.Instance.divineBindDurationBoost}초 / 최대 0.5초)";
                    break;
                case ExpData.Types.DivineShackledExplosion:
                    powerText.text = "속박중인 적이 제거되면 1칸 범위의 100%의 폭발 데미지를 입힘";
                    break;
                case ExpData.Types.DivineFifthAttackBoost:
                    powerText.text = "공격 5회마다 추가 데미지 100% 발사체 등장";
                    break;
                case ExpData.Types.DivineAttackBoost:
                    powerText.text =
                        $"데미지 16% 증가 (현재: {16 * EnforceManager.Instance.divineAttackBoost}% 증가 {EnforceManager.Instance.divineAttackBoost}회 / 최대 96% (8회))";
                    break;
                case ExpData.Types.DivineBindChanceBoost:
                    break;
                case ExpData.Types.DivineDualAttack:
                    break;
                case ExpData.Types.DivineProjectilePierce:
                    break;
                case ExpData.Types.DarkTenthAttackDamageBoost:
                    break;
                case ExpData.Types.DarkAttackSpeedBoost:
                    break;
                case ExpData.Types.DarkAttackPowerBoost:
                    break;
                case ExpData.Types.DarkStatusAilmentDamageChance:
                    break;
                case ExpData.Types.DarkKnockBackChance:
                    break;
                case ExpData.Types.DarkStatusAilmentDamageBoost:
                    break;
                case ExpData.Types.DarkRangeIncrease:
                    break;
                case ExpData.Types.DarkStatusAilmentSlowEffect:
                    break;
                case ExpData.Types.WaterAttackSpeedBoost:
                    break;
                case ExpData.Types.WaterAllyDamageBoost:
                    break;
                case ExpData.Types.WaterProjectileIncrease:
                    break;
                case ExpData.Types.WaterAttackBoost:
                    break;
                case ExpData.Types.WaterSlowEnemyDamageBoost:
                    break;
                case ExpData.Types.WaterGlobalSlowEffect:
                    break;
                case ExpData.Types.WaterSlowEnemyStunChance:
                    break;
                case ExpData.Types.WaterDamageIncreaseDebuff:
                    break;
                case ExpData.Types.PhysicalAttackSpeedBoost:
                    break;
                case ExpData.Types.PhysicalDamage35Boost:
                    break;
                case ExpData.Types.PhysicalDamage6Boost:
                    break;
                case ExpData.Types.PhysicalBleedingChance:
                    break;
                case ExpData.Types.PhysicalSwordAddition:
                    break;
                case ExpData.Types.PhysicalSlowEnemyDamageBoost:
                    break;
                case ExpData.Types.PhysicalSwordScaleIncrease:
                    break;
                case ExpData.Types.PhysicalDamage18Boost:
                    break;
                case ExpData.Types.Water2DebuffDurationIncrease:
                    break;
                case ExpData.Types.Water2AttackSpeedIncrease:
                    break;
                case ExpData.Types.Water2StunChanceAgainstBleeding:
                    break;
                case ExpData.Types.Water2IceSpikeProjectile:
                    break;
                case ExpData.Types.Water2AttackPowerIncrease:
                    break;
                case ExpData.Types.Water2ProjectileSpeedIncrease:
                    break;
                case ExpData.Types.Water2DebuffStrengthIncrease:
                    break;
                case ExpData.Types.Water2AttackSpeedBuffToAdjacentAllies:
                    break;
                case ExpData.Types.PoisonAttackSpeedIncrease:
                    break;
                case ExpData.Types.PoisonMaxStackIncrease:
                    break;
                case ExpData.Types.PoisonDamageAttackPowerIncrease:
                    break;
                case ExpData.Types.PoisonProjectileIncrease:
                    break;
                case ExpData.Types.PoisonRangeIncrease:
                    break;
                case ExpData.Types.PoisonBleedingEnemyDamageBoost:
                    break;
                case ExpData.Types.PoisonEnemyInstantKill:
                    break;
                case ExpData.Types.PoisonPerHitEffect:
                    break;
                case ExpData.Types.Fire2PoisonDamageIncrease:
                    break;
                case ExpData.Types.Fire2AttackSpeedIncrease:
                    break;
                case ExpData.Types.Fire2BurnStackIncrease:
                    break;
                case ExpData.Types.Fire2AttackPowerIncrease:
                    break;
                case ExpData.Types.Fire2StunChance:
                    break;
                case ExpData.Types.Fire2SwordSizeIncrease:
                    break;
                case ExpData.Types.Fire2BurningDamageBoost:
                    break;
                case ExpData.Types.Fire2NoBurnDamageIncrease:
                    break;
                case ExpData.Types.FireImageOverlapIncrease:
                    break;
                case ExpData.Types.FireAttackSpeedBoost:
                    break;
                case ExpData.Types.FireSlowEnemyDamageBoost:
                    break;
                case ExpData.Types.FireProjectileSpeedIncrease:
                    break;
                case ExpData.Types.FireBurnedEnemyExplosion:
                    break;
                case ExpData.Types.FireProjectileBounceDamage:
                    break;
                case ExpData.Types.FireBurnPerAttackEffect:
                    break;
                case ExpData.Types.FireProjectileBounceIncrease:
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
