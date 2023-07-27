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
                // A
                case ExpData.Types.DivineFifthAttackBoost:
                    EnforceManager.Instance.divineFifthAttackBoost = true;
                    break;
                case ExpData.Types.DivineDualAttack:
                    EnforceManager.Instance.divineDualAttack = true;
                    break;
                case ExpData.Types.DivineBindDurationBoost:
                    EnforceManager.Instance.divineBindDurationBoost = true;
                    break;
                case ExpData.Types.DivineShackledExplosion:
                    EnforceManager.Instance.divineShackledExplosion = true;
                    break;
               case ExpData.Types.DivinePoisonDamageBoost:
                   EnforceManager.Instance.divinePoisonDamageBoost = true;
                   break;
                case ExpData.Types.DivineBindChanceBoost:
                    EnforceManager.Instance.divineBindChanceBoost = true;
                    break;
                case ExpData.Types.DivineRateBoost:
                    EnforceManager.Instance.DivineRateBoost();
                    break;
                // B
                case ExpData.Types.DarkFifthAttackDamageBoost:
                    EnforceManager.Instance.darkFifthAttackDamageBoost = true;
                    break;
                case ExpData.Types.DarkStatusAilmentSlowEffect:
                    EnforceManager.Instance.darkStatusAilmentSlowEffect = true;
                    break;
                case ExpData.Types.DarkRangeIncrease:
                    EnforceManager.Instance.darkRangeIncrease = true;
                    break;
                case ExpData.Types.DarkAttackPowerBoost:
                    EnforceManager.Instance.DarkAttackDamageBoost();
                    break;
                case ExpData.Types.DarkStatusAilmentDamageBoost:
                    EnforceManager.Instance.darkStatusAilmentDamageBoost = true;
                    break;
                case ExpData.Types.DarkAttackSpeedBoost:
                    EnforceManager.Instance.DarkAttackSpeedBoost();
                    break;
                case ExpData.Types.DarkKnockBackChance:
                    EnforceManager.Instance.darkKnockBackChance = true;
                    break;
                // C
                case ExpData.Types.WaterFreeze:
                    EnforceManager.Instance.waterFreeze = true;
                    break;
                case ExpData.Types.WaterFreezeChance:
                    EnforceManager.Instance.waterFreezeChance = true;
                    break;
                case ExpData.Types.WaterSlowDurationBoost:
                    EnforceManager.Instance.WaterSlowDurationBoost();
                    break;
                case ExpData.Types.WaterFreezeDamageBoost:
                    EnforceManager.Instance.waterFreezeDamageBoost = true;
                    break;
                case ExpData.Types.WaterSlowCPowerBoost:
                    EnforceManager.Instance.waterSlowCPowerBoost = true;
                    break;
                case ExpData.Types.WaterAttackRateBoost:
                    EnforceManager.Instance.WaterAttackRateBoost();
                    break;
                case ExpData.Types.WaterGlobalFreeze:
                    EnforceManager.Instance.waterGlobalFreeze = true;
                    break;
                // D
                case ExpData.Types.PhysicalSwordScaleIncrease:
                    EnforceManager.Instance.physicalSwordScaleIncrease = true;
                    break;
                case ExpData.Types.PhysicalSwordAddition:
                    EnforceManager.Instance.physicalSwordAddition = true;
                    break;
                case ExpData.Types.PhysicalAttackSpeedBoost:
                    EnforceManager.Instance.PhysicalAttackSpeedIncrease();
                    break;
                case ExpData.Types.PhysicalRatePerAttack:
                    EnforceManager.Instance.physicalRatePerAttack = true;
                    break;
                case ExpData.Types.PhysicalBindBleed:
                    EnforceManager.Instance.physicalBindBleed = true;
                    break;
                case ExpData.Types.PhysicalDamageBoost:
                    EnforceManager.Instance.PhysicalDamageBoost();
                    break;
                case ExpData.Types.PhysicalBleedDuration:
                    EnforceManager.Instance.physicalBleedDuration = true;
                    break;
                // E
                case ExpData.Types.Water2Freeze:
                    EnforceManager.Instance.water2Freeze = true;
                    break;
                case ExpData.Types.Water2SlowPowerBoost:
                    EnforceManager.Instance.water2SlowPowerBoost = true;
                    break;
                case ExpData.Types.Water2FreezeTimeBoost:
                    EnforceManager.Instance.water2FreezeTimeBoost = true;
                    break;
                case ExpData.Types.Water2DamageBoost:
                    EnforceManager.Instance.Water2DamageBoost();
                    break;
                case ExpData.Types.Water2FreezeChanceBoost:
                    EnforceManager.Instance.water2FreezeChanceBoost = true;
                    break;
                case ExpData.Types.Water2FreezeDamageBoost:
                    EnforceManager.Instance.water2FreezeDamageBoost = true;
                    break;
                case ExpData.Types.Water2SlowTimeBoost:
                    EnforceManager.Instance.Water2SlowTimeBoost();
                    break;
                // F
                case ExpData.Types.PoisonPerHitEffect:
                    EnforceManager.Instance.poisonPerHitEffect = true;
                    break;
                case ExpData.Types.PoisonBleedingEnemyDamageBoost:
                    EnforceManager.Instance.poisonBleedingEnemyDamageBoost = true;
                    break;
                case ExpData.Types.PoisonDamagePerBoost:
                    EnforceManager.Instance.poisonDamagePerBoost = true;
                    break;
                case ExpData.Types.PoisonDamageBoost:
                    EnforceManager.Instance.poisonDamageBoost = true;
                    break;
                case ExpData.Types.PoisonDotDamageBoost:
                   EnforceManager.Instance.poisonDotDamageBoost = true;
                   break;
                case ExpData.Types.PoisonAttackSpeedIncrease:
                    EnforceManager.Instance.PoisonAttackSpeedIncrease();
                    break;
                case ExpData.Types.PoisonDurationBoost:
                    EnforceManager.Instance.poisonDurationBoost = true;
                    break;
                // G


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
                // Common
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
                case ExpData.Types.DivineFifthAttackBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                    if (EnforceManager.Instance.divineFifthAttackBoost) return false;
                    break;
                case ExpData.Types.DivineDualAttack:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 3) return false;
                    if (EnforceManager.Instance.divineDualAttack) return false;
                    break;
                case ExpData.Types.DivineBindDurationBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 5) return false;
                    if (EnforceManager.Instance.divineBindDurationBoost) return false;
                    break;
                case ExpData.Types.DivineShackledExplosion:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 7) return false;
                    if (EnforceManager.Instance.divineShackledExplosion) return false;
                    break;
                case ExpData.Types.DivinePoisonDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 9) return false;
                    if (EnforceManager.Instance.divinePoisonDamageBoost) return false;
                    break;
                case ExpData.Types.DivineBindChanceBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 11) return false;
                    if (EnforceManager.Instance.divineBindChanceBoost) return false;
                    break;
                case ExpData.Types.DivineRateBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 13) return false;
                    if (EnforceManager.Instance.divineRateBoost >= 0.36f) return false;
                    break;
                // Unit B
                case ExpData.Types.DarkFifthAttackDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                    if (EnforceManager.Instance.darkFifthAttackDamageBoost) return false;
                    break;
                case ExpData.Types.DarkStatusAilmentSlowEffect:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 3) return false;
                    if (EnforceManager.Instance.darkStatusAilmentSlowEffect) return false;
                    break;
                case ExpData.Types.DarkRangeIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 5) return false;
                    if (EnforceManager.Instance.darkRangeIncrease) return false;
                    break;
                case ExpData.Types.DarkAttackPowerBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 7) return false;
                    if (EnforceManager.Instance.darkAttackPowerBoost >= 0.24f) return false;
                    break;
                case ExpData.Types.DarkStatusAilmentDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 9) return false;
                    if (EnforceManager.Instance.darkStatusAilmentDamageBoost) return false;
                    break;
                case ExpData.Types.DarkAttackSpeedBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 11) return false;
                    if (EnforceManager.Instance.darkAttackSpeedBoost >= 0.36f) return false;
                    break;
                case ExpData.Types.DarkKnockBackChance:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 13) return false;
                    if (EnforceManager.Instance.darkKnockBackChance) return false;
                    break;
                // Unit C
                case ExpData.Types.WaterFreeze:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                    if (EnforceManager.Instance.waterFreeze) return false; 
                    break;
                case ExpData.Types.WaterFreezeChance:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 3) return false;
                    if (!EnforceManager.Instance.waterFreeze) return false;
                    break;
                case ExpData.Types.WaterSlowDurationBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 5) return false;
                    if (EnforceManager.Instance.waterSlowDurationBoost >= 1f) return false;
                    break;
                case ExpData.Types.WaterFreezeDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 7) return false;
                    if (!EnforceManager.Instance.waterFreeze) return false;
                    if (EnforceManager.Instance.waterFreezeDamageBoost) return false;
                    break;
                case ExpData.Types.WaterSlowCPowerBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 9) return false;
                    if (EnforceManager.Instance.waterSlowCPowerBoost) return false;
                    break;
                case ExpData.Types.WaterAttackRateBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 11) return false;
                    if (EnforceManager.Instance.waterAttackRateBoost >= 0.24f) return false;
                    break;
                case ExpData.Types.WaterGlobalFreeze:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 13) return false;
                    if (EnforceManager.Instance.waterGlobalFreeze) return false;
                    break;
                // Unit D
                case ExpData.Types.PhysicalSwordScaleIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                    if (EnforceManager.Instance.physicalSwordScaleIncrease) return false;
                    break;
                case ExpData.Types.PhysicalSwordAddition:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 3) return false;
                    if (EnforceManager.Instance.physicalSwordAddition) return false;
                    break;
                case ExpData.Types.PhysicalAttackSpeedBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 5) return false;
                    if (EnforceManager.Instance.physicalAttackSpeedBoost >= 0.36f) return false;
                    break;
                case ExpData.Types.PhysicalRatePerAttack:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 7) return false;
                    if (EnforceManager.Instance.physicalRatePerAttack) return false;
                    break;
                case ExpData.Types.PhysicalBindBleed:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 9) return false;
                    if (EnforceManager.Instance.physicalBindBleed) return false;
                    break;
                case ExpData.Types.PhysicalDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 11) return false;
                    if (EnforceManager.Instance.physicalDamageBoost >= 0.18f) return false;
                    break;
                case ExpData.Types.PhysicalBleedDuration:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 13) return false;
                    if (!EnforceManager.Instance.physicalBindBleed) return false;
                    if (EnforceManager.Instance.physicalBleedDuration) return false;
                    break;
                // Unit E
                case ExpData.Types.Water2Freeze:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                    if (EnforceManager.Instance.water2Freeze) return false;
                    break;
                case ExpData.Types.Water2SlowPowerBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 3) return false;
                    if (EnforceManager.Instance.water2SlowPowerBoost) return false;
                    break;
                case ExpData.Types.Water2FreezeTimeBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 5) return false;
                    if (!EnforceManager.Instance.water2Freeze) return false;
                    if (EnforceManager.Instance.water2FreezeTimeBoost) return false;
                    break;
                case ExpData.Types.Water2DamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 7) return false;
                    if (EnforceManager.Instance.water2DamageBoost >= 0.24f) return false;
                    break;
                case ExpData.Types.Water2FreezeChanceBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 9) return false;
                    if (!EnforceManager.Instance.water2Freeze) return false;
                    if (EnforceManager.Instance.water2FreezeChanceBoost) return false;
                    break;
                case ExpData.Types.Water2FreezeDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 11) return false;
                    if (!EnforceManager.Instance.water2Freeze) return false;
                    if (EnforceManager.Instance.water2FreezeDamageBoost) return false;
                    break;
                case ExpData.Types.Water2SlowTimeBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 13) return false;
                    if (EnforceManager.Instance.water2SlowTimeBoost >= 0.5f) return false;
                    break;
                //Unit F
                case ExpData.Types.PoisonPerHitEffect:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                    if (EnforceManager.Instance.poisonPerHitEffect) return false;
                    break;
                case ExpData.Types.PoisonBleedingEnemyDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 3) return false;
                    if (EnforceManager.Instance.poisonBleedingEnemyDamageBoost) return false;
                    break;
                case ExpData.Types.PoisonDamagePerBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 5) return false;
                    if (EnforceManager.Instance.poisonDamagePerBoost) return false;
                    break;
                case ExpData.Types.PoisonDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 7) return false;
                    if (EnforceManager.Instance.poisonDamageBoost) return false;
                    break;
                case ExpData.Types.PoisonDotDamageBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 9) return false;
                    if (!EnforceManager.Instance.poisonPerHitEffect) return false;
                    if (EnforceManager.Instance.poisonDotDamageBoost) return false;
                    break;
                case ExpData.Types.PoisonAttackSpeedIncrease:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 11) return false;
                    if (EnforceManager.Instance.poisonAttackSpeedIncrease >= 0.24f) return false;
                    break;
                case ExpData.Types.PoisonDurationBoost:
                    if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                    if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 13) return false;
                    if (!EnforceManager.Instance.poisonPerHitEffect) return false;
                    if (EnforceManager.Instance.poisonDurationBoost) return false;
                    break;
  
                
                //Unit G
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
                ExpData.Types.DivineRateBoost => finalTranslation,
                ExpData.Types.DivineBindChanceBoost => finalTranslation,
                ExpData.Types.DivineDualAttack => finalTranslation,
                ExpData.Types.DarkFifthAttackDamageBoost => finalTranslation,
                ExpData.Types.DarkAttackSpeedBoost => finalTranslation,
                ExpData.Types.DarkAttackPowerBoost => finalTranslation,
                ExpData.Types.DarkKnockBackChance => finalTranslation,
                ExpData.Types.DarkStatusAilmentDamageBoost => finalTranslation,
                ExpData.Types.DarkRangeIncrease => finalTranslation,
                ExpData.Types.DarkStatusAilmentSlowEffect => finalTranslation,
                ExpData.Types.WaterFreeze => finalTranslation,
                ExpData.Types.WaterFreezeChance => finalTranslation,
                ExpData.Types.PhysicalSwordScaleIncrease => finalTranslation,
                ExpData.Types.PhysicalSwordAddition => finalTranslation,
                ExpData.Types.PhysicalAttackSpeedBoost => finalTranslation,
                ExpData.Types.PhysicalRatePerAttack => finalTranslation,
                ExpData.Types.PhysicalBindBleed => finalTranslation,
                ExpData.Types.PhysicalDamageBoost => finalTranslation,
                ExpData.Types.PhysicalBleedDuration => finalTranslation,
                ExpData.Types.Water2Freeze => finalTranslation,
                ExpData.Types.Water2SlowPowerBoost => finalTranslation,
                ExpData.Types.Water2FreezeTimeBoost => finalTranslation,
                ExpData.Types.Water2DamageBoost => finalTranslation,
                ExpData.Types.Water2FreezeChanceBoost => finalTranslation,
                ExpData.Types.Water2FreezeDamageBoost => finalTranslation,
                ExpData.Types.Water2SlowTimeBoost => finalTranslation,
                ExpData.Types.PoisonPerHitEffect => finalTranslation,
                ExpData.Types.PoisonBleedingEnemyDamageBoost => finalTranslation,
                ExpData.Types.PoisonDamagePerBoost => finalTranslation,
                ExpData.Types.PoisonDamageBoost => finalTranslation,
                ExpData.Types.PoisonDotDamageBoost => finalTranslation,
                ExpData.Types.PoisonAttackSpeedIncrease => finalTranslation,
                ExpData.Types.PoisonDurationBoost => finalTranslation,
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
