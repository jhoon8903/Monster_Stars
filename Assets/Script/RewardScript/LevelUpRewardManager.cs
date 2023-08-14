using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Script.AdsScript;
using Script.CharacterManagerScript;
using Script.UIManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
#pragma warning disable CS8524

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
        [SerializeField] private Button expShuffle;
        [SerializeField] private Image icon1;
        [SerializeField] private Image icon2;
        [SerializeField] private Image icon3;
        [SerializeField] private Image exp1BtnBadge;
        [SerializeField] private Image exp2BtnBadge;
        [SerializeField] private Image exp3BtnBadge;
        [SerializeField] private ExpManager expManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private Language language;
        private Data _selectedPowerUp;
        public static LevelUpRewardManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        private static void ProcessExpReward(Data selectedReward)
        {
            switch (selectedReward.Type)
            {
                case PowerTypeManager.Types.GroupDamage:
                    EnforceManager.Instance.IncreaseGroupDamage(selectedReward,selectedReward.Property[0]);
                    break;
                case PowerTypeManager.Types.GroupAtkSpeed:
                    EnforceManager.Instance.IncreaseGroupRate(selectedReward,selectedReward.Property[0]);
                    break;
                case PowerTypeManager.Types.NextStep:
                    EnforceManager.Instance.RewardMoveCount(selectedReward.Property[0]);
                    break;
                case PowerTypeManager.Types.StepDirection:
                    EnforceManager.Instance.DiagonalMovement(selectedReward);
                    break;
                case PowerTypeManager.Types.Exp:
                    EnforceManager.Instance.IncreaseExpBuff(selectedReward,selectedReward.Property[0]);
                    break;
                case PowerTypeManager.Types.CastleRecovery:
                    EnforceManager.Instance.RecoveryCastle(selectedReward);
                    break;
                case PowerTypeManager.Types.CastleMaxHp:
                    EnforceManager.Instance.IncreaseCastleMaxHp(selectedReward);
                    break;
                case PowerTypeManager.Types.Slow:
                    EnforceManager.Instance.NextCharacterUpgrade(selectedReward, selectedReward.Property[0]);
                    break;
                case PowerTypeManager.Types.NextStage:
                    EnforceManager.Instance.NextCharacterUpgrade(selectedReward,selectedReward.Property[0]);
                    break;
                case PowerTypeManager.Types.Gold:
                    EnforceManager.Instance.AddGold(selectedReward);
                    break;
                case PowerTypeManager.Types.Match5Upgrade:
                    EnforceManager.Instance.Match5Upgrade(selectedReward);
                    break;
                // A
                case PowerTypeManager.Types.DivineFifthAttackBoost:
                    EnforceManager.Instance.divineFifthAttackBoost = true;
                    break;
                case PowerTypeManager.Types.DivineDualAttack:
                    EnforceManager.Instance.divineDualAttack = true;
                    break;
                case PowerTypeManager.Types.DivineBindDurationBoost:
                    EnforceManager.Instance.divineBindDurationBoost = true;
                    break;
                case PowerTypeManager.Types.DivineShackledExplosion:
                    EnforceManager.Instance.divineShackledExplosion = true;
                    break;
               case PowerTypeManager.Types.DivinePoisonDamageBoost:
                   EnforceManager.Instance.divinePoisonDamageBoost = true;
                   break;
                case PowerTypeManager.Types.DivineBindChanceBoost:
                    EnforceManager.Instance.divineBindChanceBoost = true;
                    break;
                case PowerTypeManager.Types.DivineRateBoost:
                    EnforceManager.Instance.DivineRateBoost();
                    break;
                // B
                case PowerTypeManager.Types.DarkFifthAttackDamageBoost:
                    EnforceManager.Instance.darkFifthAttackDamageBoost = true;
                    break;
                case PowerTypeManager.Types.DarkStatusAilmentSlowEffect:
                    EnforceManager.Instance.darkStatusAilmentSlowEffect = true;
                    break;
                case PowerTypeManager.Types.DarkRangeIncrease:
                    EnforceManager.Instance.darkRangeIncrease = true;
                    break;
                case PowerTypeManager.Types.DarkAttackPowerBoost:
                    EnforceManager.Instance.DarkAttackDamageBoost();
                    break;
                case PowerTypeManager.Types.DarkStatusAilmentDamageBoost:
                    EnforceManager.Instance.darkStatusAilmentDamageBoost = true;
                    break;
                case PowerTypeManager.Types.DarkAttackSpeedBoost:
                    EnforceManager.Instance.DarkAttackSpeedBoost();
                    break;
                case PowerTypeManager.Types.DarkKnockBackChance:
                    EnforceManager.Instance.darkKnockBackChance = true;
                    break;
                // C
                case PowerTypeManager.Types.WaterFreeze:
                    EnforceManager.Instance.waterFreeze = true;
                    break;
                case PowerTypeManager.Types.WaterFreezeChance:
                    EnforceManager.Instance.waterFreezeChance = true;
                    break;
                case PowerTypeManager.Types.WaterSlowDurationBoost:
                    EnforceManager.Instance.WaterSlowDurationBoost();
                    break;
                case PowerTypeManager.Types.WaterFreezeDamageBoost:
                    EnforceManager.Instance.waterFreezeDamageBoost = true;
                    break;
                case PowerTypeManager.Types.WaterSlowCPowerBoost:
                    EnforceManager.Instance.waterSlowCPowerBoost = true;
                    break;
                case PowerTypeManager.Types.WaterAttackRateBoost:
                    EnforceManager.Instance.WaterAttackRateBoost();
                    break;
                case PowerTypeManager.Types.WaterGlobalFreeze:
                    EnforceManager.Instance.waterGlobalFreeze = true;
                    break;
                // D
                case PowerTypeManager.Types.PhysicalSwordScaleIncrease:
                    EnforceManager.Instance.physicalSwordScaleIncrease = true;
                    break;
                case PowerTypeManager.Types.PhysicalSwordAddition:
                    EnforceManager.Instance.physicalSwordAddition = true;
                    break;
                case PowerTypeManager.Types.PhysicalAttackSpeedBoost:
                    EnforceManager.Instance.PhysicalAttackSpeedIncrease();
                    break;
                case PowerTypeManager.Types.PhysicalRatePerAttack:
                    EnforceManager.Instance.physicalRatePerAttack = true;
                    break;
                case PowerTypeManager.Types.PhysicalBindBleed:
                    EnforceManager.Instance.physicalBindBleed = true;
                    break;
                case PowerTypeManager.Types.PhysicalDamageBoost:
                    EnforceManager.Instance.PhysicalDamageBoost();
                    break;
                case PowerTypeManager.Types.PhysicalBleedDuration:
                    EnforceManager.Instance.physicalBleedDuration = true;
                    break;
                // E
                case PowerTypeManager.Types.Water2Freeze:
                    EnforceManager.Instance.water2Freeze = true;
                    break;
                case PowerTypeManager.Types.Water2SlowPowerBoost:
                    EnforceManager.Instance.water2SlowPowerBoost = true;
                    break;
                case PowerTypeManager.Types.Water2FreezeTimeBoost:
                    EnforceManager.Instance.water2FreezeTimeBoost = true;
                    break;
                case PowerTypeManager.Types.Water2DamageBoost:
                    EnforceManager.Instance.Water2DamageBoost();
                    break;
                case PowerTypeManager.Types.Water2FreezeChanceBoost:
                    EnforceManager.Instance.water2FreezeChanceBoost = true;
                    break;
                case PowerTypeManager.Types.Water2FreezeDamageBoost:
                    EnforceManager.Instance.water2FreezeDamageBoost = true;
                    break;
                case PowerTypeManager.Types.Water2SlowTimeBoost:
                    EnforceManager.Instance.Water2SlowTimeBoost();
                    break;
                // F
                case PowerTypeManager.Types.PoisonPerHitEffect:
                    EnforceManager.Instance.poisonPerHitEffect = true;
                    break;
                case PowerTypeManager.Types.PoisonBleedingEnemyDamageBoost:
                    EnforceManager.Instance.poisonBleedingEnemyDamageBoost = true;
                    break;
                case PowerTypeManager.Types.PoisonDamagePerBoost:
                    EnforceManager.Instance.poisonDamagePerBoost = true;
                    break;
                case PowerTypeManager.Types.PoisonDamageBoost:
                    EnforceManager.Instance.poisonDamageBoost = true;
                    break;
                case PowerTypeManager.Types.PoisonDotDamageBoost:
                    EnforceManager.Instance.poisonDotDamageBoost = true;
                    break;
                case PowerTypeManager.Types.PoisonAttackSpeedIncrease:
                    EnforceManager.Instance.PoisonAttackSpeedIncrease();
                    break;
                case PowerTypeManager.Types.PoisonDurationBoost:
                    EnforceManager.Instance.poisonDurationBoost = true;
                    break;
                // G
                case PowerTypeManager.Types.Fire2FreezeDamageBoost:
                    EnforceManager.Instance.fire2FreezeDamageBoost = true;
                    break;
                case PowerTypeManager.Types.Fire2BurnDurationBoost:
                    EnforceManager.Instance.fire2BurnDurationBoost = true;
                    break;
                case PowerTypeManager.Types.Fire2ChangeProperty:
                    EnforceManager.Instance.fire2ChangeProperty = true;
                    break;
                case PowerTypeManager.Types.Fire2DamageBoost:
                    EnforceManager.Instance.Fire2DamageBoost();
                    break;
                case PowerTypeManager.Types.Fire2RangeBoost:
                    EnforceManager.Instance.fire2RangeBoost = true;
                    break;
                case PowerTypeManager.Types.Fire2RateBoost:
                    EnforceManager.Instance.fire2RateBoost = true;
                    break;
                case PowerTypeManager.Types.Fire2BossDamageBoost:
                    EnforceManager.Instance.fire2BossDamageBoost = true;
                    break;
                // H
                case PowerTypeManager.Types.FireBurnPerAttackEffect:
                    EnforceManager.Instance.fireBurnPerAttackEffect = true;
                    break;
                case PowerTypeManager.Types.FireStackOverlap:
                    EnforceManager.Instance.fireStackOverlap = true;
                    break;
                case PowerTypeManager.Types.FireProjectileBounceIncrease:
                    EnforceManager.Instance.fireProjectileBounceIncrease = true;
                    break;
                case PowerTypeManager.Types.FireBurnedEnemyExplosion:
                    EnforceManager.Instance.fireBurnedEnemyExplosion = true;
                    break;
                case PowerTypeManager.Types.FireAttackSpeedBoost:
                    EnforceManager.Instance.FireAttackSpeedBoost();
                    break;
                case PowerTypeManager.Types.FireProjectileSpeedIncrease:
                    EnforceManager.Instance.fireProjectileSpeedIncrease = true;
                    break;
                case PowerTypeManager.Types.FireProjectileBounceDamage:
                    EnforceManager.Instance.fireProjectileBounceDamage = true;
                    break;
                // I
                case PowerTypeManager.Types.Poison2StunToChance:
                    EnforceManager.Instance.poison2StunToChance = true;
                    break;
                case PowerTypeManager.Types.Poison2RangeBoost:
                    EnforceManager.Instance.poison2RangeBoost = true;
                    break;
                case PowerTypeManager.Types.Poison2DotDamageBoost:
                    EnforceManager.Instance.poison2DotDamageBoost = true;
                    break;
                case PowerTypeManager.Types.Poison2StunTimeBoost:
                    EnforceManager.Instance.Poison2StunTimeBoost();
                    break;
                case PowerTypeManager.Types.Poison2SpawnPoisonArea:
                    EnforceManager.Instance.poison2SpawnPoisonArea = true;
                    break;
                case PowerTypeManager.Types.Poison2RateBoost:
                    EnforceManager.Instance.poison2RateBoost = true;
                    break;
                case PowerTypeManager.Types.Poison2PoolTimeBoost:
                    EnforceManager.Instance.poison2PoolTimeBoost = true;
                    break;
                // J
                case PowerTypeManager.Types.Physical2CastleCrushStatBoost:
                    EnforceManager.Instance.physical2CastleCrushStatBoost = true;
                    break;
                case PowerTypeManager.Types.Physical2FifthBoost:
                    EnforceManager.Instance.physical2FifthBoost = true;
                    break;
                case PowerTypeManager.Types.Physical2BleedTimeBoost:
                    EnforceManager.Instance.physical2BleedTimeBoost = true;
                    break;
                case PowerTypeManager.Types.Physical2PoisonDamageBoost:
                    EnforceManager.Instance.physical2PoisonDamageBoost = true;
                    break;
                case PowerTypeManager.Types.Physical2RangeBoost:
                    EnforceManager.Instance.physical2RangeBoost = true;
                    break;
                case PowerTypeManager.Types.Physical2RateBoost:
                    EnforceManager.Instance.Physical2RateBoost();
                    break;
                case PowerTypeManager.Types.Physical2BossBoost:
                    EnforceManager.Instance.physical2BossBoost = true;
                    break;
                // K
                case PowerTypeManager.Types.Dark2BackBoost:
                    EnforceManager.Instance.dark2BackBoost = true;
                    break;
                case PowerTypeManager.Types.Dark2DualAttack:
                    EnforceManager.Instance.dark2DualAttack = true;
                    break;
                case PowerTypeManager.Types.Dark2StatusDamageBoost:
                    EnforceManager.Instance.dark2StatusDamageBoost = true;
                    break;
                case PowerTypeManager.Types.Dark2ExplosionBoost:
                    EnforceManager.Instance.dark2ExplosionBoost = true;
                    break;
                case PowerTypeManager.Types.Dark2DoubleAttack:
                    EnforceManager.Instance.dark2DoubleAttack = true;
                    break;
                case PowerTypeManager.Types.Dark2StatusPoison:
                    EnforceManager.Instance.dark2StatusPoison = true;
                    break;
                case PowerTypeManager.Types.Dark2SameEnemyBoost:
                    EnforceManager.Instance.dark2SameEnemyBoost = true;
                    break;
                default:
                    Debug.Log("Default Value" + selectedReward.Type);
                    break;
            }
            selectedReward.ChosenProperty = null;
        }
        private void Selected(Data selectedReward)
        {
            levelUpRewardPanel.SetActive(false);
            expShuffle.gameObject.SetActive(true);
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
        public void ReLevelUpReward() // 레벨업 보상 처리
        {
            StartCoroutine(LevelUpReward());
        }
        private IEnumerator LevelUpChance(int greenChance, int blueChance, int purpleChance)
        {
            var leverUpReward = LevelUpList(greenChance, blueChance, purpleChance);
            LevelUpDisplay(leverUpReward);
            yield return null;
        }
        private List<Data> LevelUpList(int greenChance, int blueChance, int purpleChance)
        {
            var levelUpPowerUps = new List<Data>();
            var selectedCodes = new HashSet<int>();
            for (var i = 0; i < 3; i++)
            {
                var total = greenChance + blueChance + purpleChance;
                var randomValue = Random.Range(0, total);

                if (randomValue < greenChance)
                {
                    _selectedPowerUp = LevelUpUnique(PowerTypeManager.Instance.GreenList, selectedCodes);
                }
                else if (randomValue < greenChance + blueChance)
                {
                    _selectedPowerUp = LevelUpUnique(PowerTypeManager.Instance.BlueList, selectedCodes);
                }
                else
                {
                    _selectedPowerUp = LevelUpUnique(PowerTypeManager.Instance.PurpleList, selectedCodes);
                }
                if (_selectedPowerUp == null) continue;
                levelUpPowerUps.Add(_selectedPowerUp);
                selectedCodes.Add(_selectedPowerUp.Code);
            }
            return levelUpPowerUps;
        }
        private Data LevelUpUnique(IEnumerable<Data> powerUpsData, ICollection<int> selectedCodes)
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
                unitLevel = unit.unitPeaceLevel;
            }
            return unitLevel;
        }

        private bool IsValidOption(Data powerUp, ICollection<int> selectedCodes)
        {
            if (selectedCodes.Contains(powerUp.Code)) return false;
            switch (powerUp.Type)
            {
                case PowerTypeManager.Types.AddRow:
                case PowerTypeManager.Types.Step:
                case PowerTypeManager.Types.RandomLevelUp:
                case PowerTypeManager.Types.GroupLevelUp:
                case PowerTypeManager.Types.LevelUpPattern:
                    return false;
                default:
                    switch (powerUp.Type)
                    {
                        // Common
                        case PowerTypeManager.Types.Gold:
                            if (EnforceManager.Instance.addGold) return false;
                            break;
                        case PowerTypeManager.Types.Slow:
                            if (EnforceManager.Instance.slowCount >= 4) return false;
                            break;
                        case PowerTypeManager.Types.Exp:
                            if (EnforceManager.Instance.expPercentage >= 30) return false;
                            break;
                        case PowerTypeManager.Types.CastleRecovery:
                            if (EnforceManager.Instance.recoveryCastle) return false;
                            break;
                        case PowerTypeManager.Types.NextStage:
                            if (EnforceManager.Instance.selectedCount > 3) return false;
                            break;
                        case PowerTypeManager.Types.StepDirection:
                            if (EnforceManager.Instance.diagonalMovement) return false;
                            if (!StageManager.Instance.isBossClear) return false;
                            if (StageManager.Instance.currentWave % 10 != 0) return false;
                            break;
                        case PowerTypeManager.Types.CastleMaxHp:
                            if (EnforceManager.Instance.castleMaxHp >= 1000) return false;
                            break;
                        // Unit A
                        case PowerTypeManager.Types.DivineFifthAttackBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                            if (EnforceManager.Instance.divineFifthAttackBoost) return false;
                            break;
                        case PowerTypeManager.Types.DivineDualAttack:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 3) return false;
                            if (EnforceManager.Instance.divineDualAttack) return false;
                            break;
                        case PowerTypeManager.Types.DivineBindDurationBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 5) return false;
                            if (EnforceManager.Instance.divineBindDurationBoost) return false;
                            break;
                        case PowerTypeManager.Types.DivineShackledExplosion:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 7) return false;
                            if (EnforceManager.Instance.divineShackledExplosion) return false;
                            break;
                        case PowerTypeManager.Types.DivinePoisonDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 9) return false;
                            if (EnforceManager.Instance.divinePoisonDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.DivineBindChanceBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 11) return false;
                            if (EnforceManager.Instance.divineBindChanceBoost) return false;
                            break;
                        case PowerTypeManager.Types.DivineRateBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.A)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.A) < 13) return false;
                            if (EnforceManager.Instance.divineRateBoost >= 0.36f) return false;
                            break;
                        // Unit B
                        case PowerTypeManager.Types.DarkFifthAttackDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                            if (EnforceManager.Instance.darkFifthAttackDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.DarkStatusAilmentSlowEffect:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 3) return false;
                            if (EnforceManager.Instance.darkStatusAilmentSlowEffect) return false;
                            break;
                        case PowerTypeManager.Types.DarkRangeIncrease:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 5) return false;
                            if (EnforceManager.Instance.darkRangeIncrease) return false;
                            break;
                        case PowerTypeManager.Types.DarkAttackPowerBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 7) return false;
                            if (EnforceManager.Instance.darkAttackPowerBoost >= 0.24f) return false;
                            break;
                        case PowerTypeManager.Types.DarkStatusAilmentDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 9) return false;
                            if (EnforceManager.Instance.darkStatusAilmentDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.DarkAttackSpeedBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 11) return false;
                            if (EnforceManager.Instance.darkAttackSpeedBoost >= 0.36f) return false;
                            break;
                        case PowerTypeManager.Types.DarkKnockBackChance:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.B)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.B) < 13) return false;
                            if (EnforceManager.Instance.darkKnockBackChance) return false;
                            break;
                        // Unit C
                        case PowerTypeManager.Types.WaterFreeze:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                            if (EnforceManager.Instance.waterFreeze) return false;
                            break;
                        case PowerTypeManager.Types.WaterFreezeChance:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 3) return false;
                            if (!EnforceManager.Instance.waterFreeze) return false;
                            break;
                        case PowerTypeManager.Types.WaterSlowDurationBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 5) return false;
                            if (EnforceManager.Instance.waterSlowDurationBoost >= 1f) return false;
                            break;
                        case PowerTypeManager.Types.WaterFreezeDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 7) return false;
                            if (!EnforceManager.Instance.waterFreeze) return false;
                            if (EnforceManager.Instance.waterFreezeDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.WaterSlowCPowerBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 9) return false;
                            if (EnforceManager.Instance.waterSlowCPowerBoost) return false;
                            break;
                        case PowerTypeManager.Types.WaterAttackRateBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 11) return false;
                            if (EnforceManager.Instance.waterAttackRateBoost >= 0.24f) return false;
                            break;
                        case PowerTypeManager.Types.WaterGlobalFreeze:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.C)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.C) < 13) return false;
                            if (EnforceManager.Instance.waterGlobalFreeze) return false;
                            break;
                        // Unit D
                        case PowerTypeManager.Types.PhysicalSwordScaleIncrease:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                            if (EnforceManager.Instance.physicalSwordScaleIncrease) return false;
                            break;
                        case PowerTypeManager.Types.PhysicalSwordAddition:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 3) return false;
                            if (EnforceManager.Instance.physicalSwordAddition) return false;
                            break;
                        case PowerTypeManager.Types.PhysicalAttackSpeedBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 5) return false;
                            if (EnforceManager.Instance.physicalAttackSpeedBoost >= 0.36f) return false;
                            break;
                        case PowerTypeManager.Types.PhysicalRatePerAttack:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 7) return false;
                            if (EnforceManager.Instance.physicalRatePerAttack) return false;
                            break;
                        case PowerTypeManager.Types.PhysicalBindBleed:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 9) return false;
                            if (EnforceManager.Instance.physicalBindBleed) return false;
                            break;
                        case PowerTypeManager.Types.PhysicalDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 11) return false;
                            if (EnforceManager.Instance.physicalDamageBoost >= 0.18f) return false;
                            break;
                        case PowerTypeManager.Types.PhysicalBleedDuration:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.D)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.D) < 13) return false;
                            if (!EnforceManager.Instance.physicalBindBleed) return false;
                            if (EnforceManager.Instance.physicalBleedDuration) return false;
                            break;
                        // Unit E
                        case PowerTypeManager.Types.Water2Freeze:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                            if (EnforceManager.Instance.water2Freeze) return false;
                            break;
                        case PowerTypeManager.Types.Water2SlowPowerBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 3) return false;
                            if (EnforceManager.Instance.water2SlowPowerBoost) return false;
                            break;
                        case PowerTypeManager.Types.Water2FreezeTimeBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 5) return false;
                            if (!EnforceManager.Instance.water2Freeze) return false;
                            if (EnforceManager.Instance.water2FreezeTimeBoost) return false;
                            break;
                        case PowerTypeManager.Types.Water2DamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 7) return false;
                            if (EnforceManager.Instance.water2DamageBoost >= 0.24f) return false;
                            break;
                        case PowerTypeManager.Types.Water2FreezeChanceBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 9) return false;
                            if (!EnforceManager.Instance.water2Freeze) return false;
                            if (EnforceManager.Instance.water2FreezeChanceBoost) return false;
                            break;
                        case PowerTypeManager.Types.Water2FreezeDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 11) return false;
                            if (!EnforceManager.Instance.water2Freeze) return false;
                            if (EnforceManager.Instance.water2FreezeDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.Water2SlowTimeBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.E)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.E) < 13) return false;
                            if (EnforceManager.Instance.water2SlowTimeBoost >= 0.5f) return false;
                            break;
                        //Unit F
                        case PowerTypeManager.Types.PoisonPerHitEffect:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                            if (EnforceManager.Instance.poisonPerHitEffect) return false;
                            break;
                        case PowerTypeManager.Types.PoisonBleedingEnemyDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 3) return false;
                            if (EnforceManager.Instance.poisonBleedingEnemyDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.PoisonDamagePerBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 5) return false;
                            if (EnforceManager.Instance.poisonDamagePerBoost) return false;
                            break;
                        case PowerTypeManager.Types.PoisonDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 7) return false;
                            if (EnforceManager.Instance.poisonDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.PoisonDotDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 9) return false;
                            if (!EnforceManager.Instance.poisonPerHitEffect) return false;
                            if (EnforceManager.Instance.poisonDotDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.PoisonAttackSpeedIncrease:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 11) return false;
                            if (EnforceManager.Instance.poisonAttackSpeedIncrease >= 0.24f) return false;
                            break;
                        case PowerTypeManager.Types.PoisonDurationBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.F)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.F) < 13) return false;
                            if (!EnforceManager.Instance.poisonPerHitEffect) return false;
                            if (EnforceManager.Instance.poisonDurationBoost) return false;
                            break;
                        //Unit G
                        case PowerTypeManager.Types.Fire2FreezeDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.G)) return false;
                            if (EnforceManager.Instance.fire2FreezeDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.Fire2BurnDurationBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.G)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.G) < 3) return false;
                            if (EnforceManager.Instance.fire2BurnDurationBoost) return false;
                            break;
                        case PowerTypeManager.Types.Fire2ChangeProperty:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.G)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.G) < 5) return false;
                            if (EnforceManager.Instance.fire2ChangeProperty) return false;
                            break;
                        case PowerTypeManager.Types.Fire2DamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.G)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.G) < 7) return false;
                            if (EnforceManager.Instance.fire2DamageBoost >= 0.24f) return false;
                            break;
                        case PowerTypeManager.Types.Fire2RangeBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.G)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.G) < 9) return false;
                            if (EnforceManager.Instance.fire2RangeBoost) return false;
                            break;
                        case PowerTypeManager.Types.Fire2RateBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.G)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.G) < 11) return false;
                            if (EnforceManager.Instance.fire2RateBoost) return false;
                            break;
                        case PowerTypeManager.Types.Fire2BossDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.G)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.G) < 13) return false;
                            if (EnforceManager.Instance.fire2BossDamageBoost) return false;
                            break;
                        //Unit H
                        case PowerTypeManager.Types.FireBurnPerAttackEffect:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.H)) return false;
                            if (EnforceManager.Instance.fireBurnPerAttackEffect) return false;
                            break;
                        case PowerTypeManager.Types.FireStackOverlap:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.H)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.H) < 3) return false;
                            if (!EnforceManager.Instance.fireBurnPerAttackEffect) return false;
                            if (EnforceManager.Instance.fireStackOverlap) return false;
                            break;
                        case PowerTypeManager.Types.FireProjectileBounceDamage:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.H)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.H) < 5) return false;
                            if (EnforceManager.Instance.fireProjectileBounceDamage) return false;
                            break;
                        case PowerTypeManager.Types.FireBurnedEnemyExplosion:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.H)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.H) < 7) return false;
                            if (EnforceManager.Instance.fireBurnedEnemyExplosion) return false;
                            break;
                        case PowerTypeManager.Types.FireAttackSpeedBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.H)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.H) < 9) return false;
                            if (EnforceManager.Instance.fireAttackSpeedBoost >= 0.24f) return false;
                            break;
                        case PowerTypeManager.Types.FireProjectileSpeedIncrease:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.H)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.H) < 11) return false;
                            if (EnforceManager.Instance.fireProjectileSpeedIncrease) return false;
                            break;
                        case PowerTypeManager.Types.FireProjectileBounceIncrease:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.H)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.H) < 13) return false;
                            if (!EnforceManager.Instance.fireProjectileBounceDamage) return false;
                            if (EnforceManager.Instance.fireProjectileBounceIncrease) return false;
                            break;
                        // Unit I
                        case PowerTypeManager.Types.Poison2StunToChance:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.I)) return false;
                            if (EnforceManager.Instance.poison2StunToChance) return false;
                            break;
                        case PowerTypeManager.Types.Poison2RangeBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.I)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.I) < 3) return false;
                            if (EnforceManager.Instance.poison2RangeBoost) return false;
                            break;
                        case PowerTypeManager.Types.Poison2DotDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.I)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.I) < 5) return false;
                            if (EnforceManager.Instance.poison2DotDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.Poison2StunTimeBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.I)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.I) < 7) return false;
                            if (!EnforceManager.Instance.poison2StunToChance) return false;
                            if (EnforceManager.Instance.poison2StunTimeBoost >= 0.5f) return false;
                            break;
                        case PowerTypeManager.Types.Poison2SpawnPoisonArea:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.I)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.I) < 9) return false;
                            if (EnforceManager.Instance.poison2SpawnPoisonArea) return false;
                            break;
                        case PowerTypeManager.Types.Poison2RateBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.I)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.I) < 11) return false;
                            if (EnforceManager.Instance.poison2RateBoost) return false;
                            break;
                        case PowerTypeManager.Types.Poison2PoolTimeBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.I)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.I) < 13) return false;
                            if (!EnforceManager.Instance.poison2SpawnPoisonArea) return false;
                            if (EnforceManager.Instance.poison2PoolTimeBoost) return false;
                            break;
                        // Unit J
                        case PowerTypeManager.Types.Physical2CastleCrushStatBoost:                         
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.J)) return false;
                            if (EnforceManager.Instance.physical2CastleCrushStatBoost) return false;
                            break;
                        case PowerTypeManager.Types.Physical2FifthBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.J)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.J) < 3) return false;
                            if (EnforceManager.Instance.physical2FifthBoost) return false;
                            break;
                        case PowerTypeManager.Types.Physical2BleedTimeBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.J)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.J) < 5) return false;
                            if (EnforceManager.Instance.physical2BleedTimeBoost) return false;
                            break;
                        case PowerTypeManager.Types.Physical2PoisonDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.J)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.J) < 7) return false;
                            if (EnforceManager.Instance.physical2PoisonDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.Physical2RangeBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.J)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.J) < 9) return false;
                            if (EnforceManager.Instance.physical2RangeBoost) return false;
                            break;
                        case PowerTypeManager.Types.Physical2RateBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.J)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.J) < 11) return false;
                            if (EnforceManager.Instance.physical2RateBoost >= 0.36f) return false;
                            break;
                        case PowerTypeManager.Types.Physical2BossBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.J)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.J) < 13) return false;
                            if (EnforceManager.Instance.physical2BossBoost) return false;
                            break;
                        // Unit K
                        case PowerTypeManager.Types.Dark2BackBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.K)) return false;
                            if (EnforceManager.Instance.dark2BackBoost) return false;
                            break;
                        case PowerTypeManager.Types.Dark2DualAttack:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.K)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.K) < 3) return false;
                            if (EnforceManager.Instance.dark2DualAttack) return false;
                            break;
                        case PowerTypeManager.Types.Dark2StatusDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.K)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.K) < 5) return false;
                            if (EnforceManager.Instance.dark2StatusDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.Dark2ExplosionBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.K)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.K) < 7) return false;
                            if (EnforceManager.Instance.dark2ExplosionBoost) return false;
                            break;
                        case PowerTypeManager.Types.Dark2DoubleAttack:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.K)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.K) < 9) return false;
                            if (EnforceManager.Instance.dark2DoubleAttack) return false;
                            break;
                        case PowerTypeManager.Types.Dark2StatusPoison:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.K)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.K) < 11) return false;
                            if (EnforceManager.Instance.dark2StatusPoison) return false;
                            break;
                        case PowerTypeManager.Types.Dark2SameEnemyBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.K)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.K) < 13) return false;
                            if (EnforceManager.Instance.dark2SameEnemyBoost) return false;
                            break;
                    }
                    return true;
            }
        }

        private static Data SelectRandom(IEnumerable<Data> validOptions)
        {
            var commonDataList = validOptions.ToList();
            var count = commonDataList.Count;
            if (count == 0) return null;
            var randomIndex = Random.Range(0, count);
            return commonDataList.ElementAt(randomIndex);
        }
        private void LevelUpDisplay(IReadOnlyList<Data> powerUps)
        {
            LevelUpDisplayText(exp1Button, exp1Text, icon1, exp1BtnBadge, powerUps[0], language);
            LevelUpDisplayText(exp2Button, exp2Text, icon2, exp2BtnBadge, powerUps[1], language);
            LevelUpDisplayText(exp3Button, exp3Text, icon3, exp3BtnBadge, powerUps[2], language);
        }

        private void LevelUpDisplayText(Button expButton, TMP_Text powerText, Image icon, Image btnBadge, Data powerUp, Language selectedLanguage)
        {
            var translationKey = powerUp.Type.ToString();
            var powerTextTranslation = selectedLanguage.GetTranslation(translationKey);
            var finalPowerText = powerTextTranslation;
            var placeholderValues = new Dictionary<string, Func<double>> {
                { "{p}", () => powerUp.Property[0]},
                { "{powerUp.Property[0]}",() => powerUp.Property[0]},
                { "{15*EnforceManager.Instance.slowCount}", () => 15 * EnforceManager.Instance.slowCount },
                { "{EnforceManager.Instance.expPercentage}", () => EnforceManager.Instance.expPercentage },
                { "{EnforceManager.Instance.highLevelCharacterCount}", () => EnforceManager.Instance.highLevelCharacterCount},

            };
            finalPowerText = placeholderValues.Aggregate(finalPowerText, (current, placeholder) 
                => current.Replace(placeholder.Key, placeholder.Value().ToString(CultureInfo.CurrentCulture)));
            var finalTranslation = finalPowerText.Replace("||", "\n");

            icon.sprite = powerUp.Icon;
            btnBadge.sprite = powerUp.BtnColor;
            expButton.GetComponent<Image>().sprite = powerUp.BackGroundColor;

            powerText.text = powerUp.Type switch
            {
                PowerTypeManager.Types.GroupDamage => finalTranslation,
                PowerTypeManager.Types.GroupAtkSpeed => finalTranslation,
                PowerTypeManager.Types.StepDirection => finalTranslation,
                PowerTypeManager.Types.NextStep => finalTranslation,
                PowerTypeManager.Types.Exp => finalTranslation,
                PowerTypeManager.Types.CastleRecovery => finalTranslation,
                PowerTypeManager.Types.CastleMaxHp => finalTranslation,
                PowerTypeManager.Types.Slow => finalTranslation,
                PowerTypeManager.Types.NextStage => finalTranslation,
                PowerTypeManager.Types.Gold => finalTranslation,
                PowerTypeManager.Types.Match5Upgrade => finalTranslation,
                PowerTypeManager.Types.DivinePoisonDamageBoost => finalTranslation,
                PowerTypeManager.Types.DivineBindDurationBoost => finalTranslation,
                PowerTypeManager.Types.DivineShackledExplosion => finalTranslation,
                PowerTypeManager.Types.DivineFifthAttackBoost => finalTranslation,
                PowerTypeManager.Types.DivineRateBoost => finalTranslation,
                PowerTypeManager.Types.DivineBindChanceBoost => finalTranslation,
                PowerTypeManager.Types.DivineDualAttack => finalTranslation,
                PowerTypeManager.Types.DarkFifthAttackDamageBoost => finalTranslation,
                PowerTypeManager.Types.DarkAttackSpeedBoost => finalTranslation,
                PowerTypeManager.Types.DarkAttackPowerBoost => finalTranslation,
                PowerTypeManager.Types.DarkKnockBackChance => finalTranslation,
                PowerTypeManager.Types.DarkStatusAilmentDamageBoost => finalTranslation,
                PowerTypeManager.Types.DarkRangeIncrease => finalTranslation,
                PowerTypeManager.Types.DarkStatusAilmentSlowEffect => finalTranslation,
                PowerTypeManager.Types.WaterFreeze => finalTranslation,
                PowerTypeManager.Types.WaterFreezeChance => finalTranslation,
                PowerTypeManager.Types.WaterSlowDurationBoost => finalTranslation,
                PowerTypeManager.Types.WaterFreezeDamageBoost => finalTranslation,
                PowerTypeManager.Types.WaterSlowCPowerBoost => finalTranslation,
                PowerTypeManager.Types.WaterAttackRateBoost => finalTranslation,
                PowerTypeManager.Types.WaterGlobalFreeze => finalTranslation,
                PowerTypeManager.Types.PhysicalSwordScaleIncrease => finalTranslation,
                PowerTypeManager.Types.PhysicalSwordAddition => finalTranslation,
                PowerTypeManager.Types.PhysicalAttackSpeedBoost => finalTranslation,
                PowerTypeManager.Types.PhysicalRatePerAttack => finalTranslation,
                PowerTypeManager.Types.PhysicalBindBleed => finalTranslation,
                PowerTypeManager.Types.PhysicalDamageBoost => finalTranslation,
                PowerTypeManager.Types.PhysicalBleedDuration => finalTranslation,
                PowerTypeManager.Types.Water2Freeze => finalTranslation,
                PowerTypeManager.Types.Water2SlowPowerBoost => finalTranslation,
                PowerTypeManager.Types.Water2FreezeTimeBoost => finalTranslation,
                PowerTypeManager.Types.Water2DamageBoost => finalTranslation,
                PowerTypeManager.Types.Water2FreezeChanceBoost => finalTranslation,
                PowerTypeManager.Types.Water2FreezeDamageBoost => finalTranslation,
                PowerTypeManager.Types.Water2SlowTimeBoost => finalTranslation,
                PowerTypeManager.Types.PoisonPerHitEffect => finalTranslation,
                PowerTypeManager.Types.PoisonBleedingEnemyDamageBoost => finalTranslation,
                PowerTypeManager.Types.PoisonDamagePerBoost => finalTranslation,
                PowerTypeManager.Types.PoisonDamageBoost => finalTranslation,
                PowerTypeManager.Types.PoisonDotDamageBoost => finalTranslation,
                PowerTypeManager.Types.PoisonAttackSpeedIncrease => finalTranslation,
                PowerTypeManager.Types.PoisonDurationBoost => finalTranslation,
                PowerTypeManager.Types.Fire2FreezeDamageBoost => finalTranslation,
                PowerTypeManager.Types.Fire2BurnDurationBoost => finalTranslation,
                PowerTypeManager.Types.Fire2ChangeProperty => finalTranslation,
                PowerTypeManager.Types.Fire2DamageBoost => finalTranslation,
                PowerTypeManager.Types.Fire2RangeBoost => finalTranslation,
                PowerTypeManager.Types.Fire2RateBoost => finalTranslation,
                PowerTypeManager.Types.Fire2BossDamageBoost => finalTranslation,
                PowerTypeManager.Types.FireBurnPerAttackEffect => finalTranslation,
                PowerTypeManager.Types.FireStackOverlap => finalTranslation,
                PowerTypeManager.Types.FireProjectileBounceDamage => finalTranslation,
                PowerTypeManager.Types.FireBurnedEnemyExplosion => finalTranslation,
                PowerTypeManager.Types.FireAttackSpeedBoost => finalTranslation,
                PowerTypeManager.Types.FireProjectileSpeedIncrease => finalTranslation,
                PowerTypeManager.Types.FireProjectileBounceIncrease => finalTranslation,
                PowerTypeManager.Types.Poison2StunToChance => finalTranslation,
                PowerTypeManager.Types.Poison2RangeBoost => finalTranslation,
                PowerTypeManager.Types.Poison2DotDamageBoost => finalTranslation,
                PowerTypeManager.Types.Poison2StunTimeBoost => finalTranslation,
                PowerTypeManager.Types.Poison2SpawnPoisonArea => finalTranslation,
                PowerTypeManager.Types.Poison2RateBoost => finalTranslation,
                PowerTypeManager.Types.Poison2PoolTimeBoost => finalTranslation,
                PowerTypeManager.Types.Physical2CastleCrushStatBoost => finalTranslation,
                PowerTypeManager.Types.Physical2FifthBoost => finalTranslation,
                PowerTypeManager.Types.Physical2BleedTimeBoost => finalTranslation,
                PowerTypeManager.Types.Physical2PoisonDamageBoost => finalTranslation,
                PowerTypeManager.Types.Physical2RangeBoost => finalTranslation,
                PowerTypeManager.Types.Physical2RateBoost => finalTranslation,
                PowerTypeManager.Types.Physical2BossBoost => finalTranslation,
                PowerTypeManager.Types.Dark2BackBoost => finalTranslation,
                PowerTypeManager.Types.Dark2DualAttack => finalTranslation,
                PowerTypeManager.Types.Dark2StatusDamageBoost => finalTranslation,
                PowerTypeManager.Types.Dark2ExplosionBoost => finalTranslation,
                PowerTypeManager.Types.Dark2DoubleAttack => finalTranslation,
                PowerTypeManager.Types.Dark2StatusPoison => finalTranslation,
                PowerTypeManager.Types.Dark2SameEnemyBoost => finalTranslation,
                _=> "Default Value" + powerUp.Type
            };
            expButton.onClick.RemoveAllListeners();
            expShuffle.onClick.RemoveAllListeners();
            expButton.onClick.AddListener(() => Selected(powerUp));
            expShuffle.onClick.AddListener(ShuffleExpReward);
        }
        private void ShuffleExpReward()
        {
            AdsManager.Instance.ShowRewardedAd();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_refresh");
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.LevelUp;
            expShuffle.gameObject.SetActive(false);
        }
    }
}
