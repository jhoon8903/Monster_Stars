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
                case PowerTypeManager.Types.GroupDamage1:
                    EnforceManager.Instance.IncreaseGroupDamage(selectedReward,selectedReward.Property[0]);
                    break;
                case PowerTypeManager.Types.GroupDamage2:
                    EnforceManager.Instance.IncreaseGroupDamage(selectedReward,selectedReward.Property[0]);
                    break;
                case PowerTypeManager.Types.GroupDamage3:
                    EnforceManager.Instance.IncreaseGroupDamage(selectedReward,selectedReward.Property[0]);
                    break;
                case PowerTypeManager.Types.GroupAtkSpeed1:
                    EnforceManager.Instance.IncreaseGroupRate(selectedReward,selectedReward.Property[0]);
                    break;
                case PowerTypeManager.Types.GroupAtkSpeed2:
                    EnforceManager.Instance.IncreaseGroupRate(selectedReward,selectedReward.Property[0]);
                    break;
                case PowerTypeManager.Types.GroupAtkSpeed3:
                    EnforceManager.Instance.IncreaseGroupRate(selectedReward,selectedReward.Property[0]);
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
                    EnforceManager.Instance.SlowCount(selectedReward);
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
                case PowerTypeManager.Types.StepLimit:
                    EnforceManager.Instance.PermanentIncreaseMoveCount(selectedReward, 1);
                    break;
                // A
                case PowerTypeManager.Types.OctopusThirdAttackBoost:
                    EnforceManager.Instance.octopusThirdAttackBoost = true;
                    break;
                case PowerTypeManager.Types.OctopusPoisonAttack:
                    EnforceManager.Instance.octopusPoisonAttack = true;
                    break;
                case PowerTypeManager.Types.OctopusBleedDamageBoost:
                    EnforceManager.Instance.octopusBleedDamageBoost = true;
                    break;
                case PowerTypeManager.Types.OctopusPoisonDurationBoost:
                    EnforceManager.Instance.octopusPoisonDurationBoost = true;
                    break;
                case PowerTypeManager.Types.OctopusPoisonDamageBoost:
                    EnforceManager.Instance.octopusPoisonDamageBoost = true;
                    break;
                case PowerTypeManager.Types.OctopusDamageBoost:
                    EnforceManager.Instance.octopusDamageBoost = true;
                    break;
                 case PowerTypeManager.Types.OctopusRateBoost: 
                     EnforceManager.Instance.OctopusRateBoost();
                     break;
                // Ogre
                case PowerTypeManager.Types.OgreThirdAttackDamageBoost:
                    EnforceManager.Instance.ogreThirdAttackDamageBoost = true;
                    break;
                case PowerTypeManager.Types.OgreStatusAilmentSlowEffect:
                    EnforceManager.Instance.ogreStatusAilmentSlowEffect = true;
                    break;
                case PowerTypeManager.Types.OgreRangeIncrease:
                    EnforceManager.Instance.ogreRangeIncrease = true;
                    break;
                case PowerTypeManager.Types.OgreAttackPowerBoost:
                    EnforceManager.Instance.OgreAttackDamageBoost();
                    break;
                case PowerTypeManager.Types.OgreStatusAilmentDamageBoost:
                    EnforceManager.Instance.ogreStatusAilmentDamageBoost = true;
                    break;
                case PowerTypeManager.Types.OgreAttackSpeedBoost:
                    EnforceManager.Instance.OgreAttackSpeedBoost();
                    break;
                case PowerTypeManager.Types.OgreKnockBackChance:
                    EnforceManager.Instance.ogreKnockBackChance = true;
                    break;
                // DeathChiller
                case PowerTypeManager.Types.DeathChillerFreeze:
                    EnforceManager.Instance.deathChillerFreeze = true;
                    break;
                case PowerTypeManager.Types.DeathChillerFreezeChance:
                    EnforceManager.Instance.deathChillerFreezeChance = true;
                    break;
                case PowerTypeManager.Types.DeathChillerSlowDurationBoost:
                    EnforceManager.Instance.DeathChillerSlowDurationBoost();
                    break;
                case PowerTypeManager.Types.DeathChillerFreezeDamageBoost:
                    EnforceManager.Instance.deathChillerFreezeDamageBoost = true;
                    break;
                case PowerTypeManager.Types.DeathChillerSlowCPowerBoost:
                    EnforceManager.Instance.deathChillerSlowCPowerBoost = true;
                    break;
                case PowerTypeManager.Types.DeathChillerAttackRateBoost:
                    EnforceManager.Instance.DeathChillerAttackRateBoost();
                    break;
                case PowerTypeManager.Types.DeathChillerBackAttackBoost:
                    EnforceManager.Instance.deathChillerBackAttackBoost = true;
                    break;
                // Orc
                case PowerTypeManager.Types.OrcSwordScaleIncrease:
                    EnforceManager.Instance.orcSwordScaleIncrease = true;
                    break;
                case PowerTypeManager.Types.OrcSwordAddition:
                    EnforceManager.Instance.orcSwordAddition = true;
                    break;
                case PowerTypeManager.Types.OrcAttackSpeedBoost:
                    EnforceManager.Instance.OrcAttackSpeedIncrease();
                    break;
                case PowerTypeManager.Types.OrcRatePerAttack:
                    EnforceManager.Instance.orcRatePerAttack = true;
                    break;
                case PowerTypeManager.Types.OrcBindBleed:
                    EnforceManager.Instance.orcBindBleed = true;
                    break;
                case PowerTypeManager.Types.OrcDamageBoost:
                    EnforceManager.Instance.OrcDamageBoost();
                    break;
                case PowerTypeManager.Types.OrcBleedDuration:
                    EnforceManager.Instance.orcBleedDuration = true;
                    break;
                // Fishman
                case PowerTypeManager.Types.FishmanFreeze:
                    EnforceManager.Instance.fishmanFreeze = true;
                    break;
                case PowerTypeManager.Types.FishmanSlowPowerBoost:
                    EnforceManager.Instance.fishmanSlowPowerBoost = true;
                    break;
                case PowerTypeManager.Types.FishmanFreezeTimeBoost:
                    EnforceManager.Instance.fishmanFreezeTimeBoost = true;
                    break;
                case PowerTypeManager.Types.FishmanDamageBoost:
                    EnforceManager.Instance.FishmanDamageBoost();
                    break;
                case PowerTypeManager.Types.FishmanFreezeChanceBoost:
                    EnforceManager.Instance.fishmanFreezeChanceBoost = true;
                    break;
                case PowerTypeManager.Types.FishmanFreezeDamageBoost:
                    EnforceManager.Instance.fishmanFreezeDamageBoost = true;
                    break;
                case PowerTypeManager.Types.FishmanSlowTimeBoost:
                    EnforceManager.Instance.FishmanSlowTimeBoost();
                    break;
                // Skeleton
                case PowerTypeManager.Types.SkeletonPerHitEffect:
                    EnforceManager.Instance.skeletonPerHitEffect = true;
                    break;
                case PowerTypeManager.Types.SkeletonBleedingEnemyDamageBoost:
                    EnforceManager.Instance.skeletonBleedingEnemyDamageBoost = true;
                    break;
                case PowerTypeManager.Types.SkeletonDamagePerBoost:
                    EnforceManager.Instance.skeletonDamagePerBoost = true;
                    break;
                case PowerTypeManager.Types.SkeletonDamageBoost:
                    EnforceManager.Instance.skeletonDamageBoost = true;
                    break;
                case PowerTypeManager.Types.SkeletonDotDamageBoost:
                    EnforceManager.Instance.skeletonDotDamageBoost = true;
                    break;
                case PowerTypeManager.Types.SkeletonAttackSpeedIncrease:
                    EnforceManager.Instance.SkeletonAttackSpeedIncrease();
                    break;
                case PowerTypeManager.Types.SkeletonDurationBoost:
                    EnforceManager.Instance.skeletonDurationBoost = true;
                    break;
                // Phoenix
                case PowerTypeManager.Types.PhoenixFreezeDamageBoost:
                    EnforceManager.Instance.phoenixFreezeDamageBoost = true;
                    break;
                case PowerTypeManager.Types.PhoenixBurnDurationBoost:
                    EnforceManager.Instance.phoenixBurnDurationBoost = true;
                    break;
                case PowerTypeManager.Types.PhoenixChangeProperty:
                    EnforceManager.Instance.phoenixChangeProperty = true;
                    break;
                case PowerTypeManager.Types.PhoenixDamageBoost:
                    EnforceManager.Instance.PhoenixDamageBoost();
                    break;
                case PowerTypeManager.Types.PhoenixRangeBoost:
                    EnforceManager.Instance.phoenixRangeBoost = true;
                    break;
                case PowerTypeManager.Types.PhoenixRateBoost:
                    EnforceManager.Instance.phoenixRateBoost = true;
                    break;
                case PowerTypeManager.Types.PhoenixBossDamageBoost:
                    EnforceManager.Instance.phoenixBossDamageBoost = true;
                    break;
                // Beholder
                case PowerTypeManager.Types.BeholderBurnPerAttackEffect:
                    EnforceManager.Instance.beholderBurnPerAttackEffect = true;
                    break;
                case PowerTypeManager.Types.BeholderStackOverlap:
                    EnforceManager.Instance.beholderStackOverlap = true;
                    break;
                case PowerTypeManager.Types.BeholderProjectileBounceIncrease:
                    EnforceManager.Instance.beholderProjectileBounceIncrease = true;
                    break;
                case PowerTypeManager.Types.BeholderDamageBoost:
                    EnforceManager.Instance.BeholderAttackDamageBoost();
                    break;
                case PowerTypeManager.Types.BeholderAttackSpeedBoost:
                    EnforceManager.Instance.BeholderAttackSpeedBoost();
                    break;
                case PowerTypeManager.Types.BeholderProjectileSpeedIncrease:
                    EnforceManager.Instance.beholderProjectileSpeedIncrease = true;
                    break;
                case PowerTypeManager.Types.BeholderProjectileBounceDamage:
                    EnforceManager.Instance.beholderProjectileBounceDamage = true;
                    break;
                // Cobra
                case PowerTypeManager.Types.CobraStunToChance:
                    EnforceManager.Instance.cobra2StunToChance = true;
                    break;
                case PowerTypeManager.Types.CobraRangeBoost:
                    EnforceManager.Instance.cobraRangeBoost = true;
                    break;
                case PowerTypeManager.Types.CobraDotDamageBoost:
                    EnforceManager.Instance.cobraDotDamageBoost = true;
                    break;
                case PowerTypeManager.Types.CobraStunTimeBoost:
                    EnforceManager.Instance.CobraStunTimeBoost();
                    break;
                case PowerTypeManager.Types.CobraDamageBoost:
                    EnforceManager.Instance.CobraDamageBoost();
                    break;
                case PowerTypeManager.Types.CobraRateBoost:
                    EnforceManager.Instance.cobraRateBoost = true;
                    break;
                case PowerTypeManager.Types.CobraStunChanceBoost:
                    EnforceManager.Instance.cobraStunChanceBoost = true;
                    break;
                // J
                case PowerTypeManager.Types.BerserkerCastleCrushStatBoost:
                    EnforceManager.Instance.berserkerCastleCrushStatBoost = true;
                    break;
                case PowerTypeManager.Types.BerserkerThirdBoost:
                    EnforceManager.Instance.berserkerThirdBoost = true;
                    break;
                case PowerTypeManager.Types.BerserkerBleedTimeBoost:
                    EnforceManager.Instance.berserkerBleedTimeBoost = true;
                    break;
                case PowerTypeManager.Types.BerserkerPoisonDamageBoost:
                    EnforceManager.Instance.berserkerPoisonDamageBoost = true;
                    break;
                case PowerTypeManager.Types.BerserkerRangeBoost:
                    EnforceManager.Instance.berserkerRangeBoost = true;
                    break;
                case PowerTypeManager.Types.BerserkerRateBoost:
                    EnforceManager.Instance.BerserkerRateBoost();
                    break;
                case PowerTypeManager.Types.BerserkerBossBoost:
                    EnforceManager.Instance.berserkerBossBoost = true;
                    break;
                // DarkElf
                case PowerTypeManager.Types.DarkElfBackBoost:
                    EnforceManager.Instance.darkElfBackBoost = true;
                    break;
                case PowerTypeManager.Types.DarkElfDualAttack:
                    EnforceManager.Instance.darkElfDualAttack = true;
                    break;
                case PowerTypeManager.Types.DarkElfStatusDamageBoost:
                    EnforceManager.Instance.darkElfStatusDamageBoost = true;
                    break;
                case PowerTypeManager.Types.DarkElfExplosionBoost:
                    EnforceManager.Instance.darkElfExplosionBoost = true;
                    break;
                case PowerTypeManager.Types.DarkElfDoubleAttack:
                    EnforceManager.Instance.darkElfDoubleAttack = true;
                    break;
                case PowerTypeManager.Types.DarkElfStatusPoison:
                    EnforceManager.Instance.darkElfStatusPoison = true;
                    break;
                case PowerTypeManager.Types.DarkElfSameEnemyBoost:
                    EnforceManager.Instance.darkElfSameEnemyBoost = true;
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
            if (StageManager.Instance.currentWave == StageManager.Instance.maxWaveCount || levelUpRewardPanel.activeInHierarchy) yield break;
            Time.timeScale = 0; // 게임 일시 정지
            levelUpRewardPanel.SetActive(true); // 보물 패널 활성화
            expShuffle.gameObject.SetActive(true);
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
                unitLevel = unit.unitPieceLevel;
            }
            return unitLevel;
        }
        private bool IsValidOption(Data powerUp, ICollection<int> selectedCodes)
        {
            if (selectedCodes.Contains(powerUp.Code)) return false;
            switch (powerUp.Type)
            {
                case PowerTypeManager.Types.AddRow:
                case PowerTypeManager.Types.Step1:
                case PowerTypeManager.Types.Step2:
                case PowerTypeManager.Types.Step3:
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
                            if (StageManager.Instance != null && !StageManager.Instance.isBossClear) return false;
                            if (StageManager.Instance != null && StageManager.Instance.currentWave % 10 != 0) return false;
                            break;
                        case PowerTypeManager.Types.Match5Upgrade:
                            if (EnforceManager.Instance.match5Upgrade) return false;
                            break;
                        case PowerTypeManager.Types.StepLimit:
                            if (EnforceManager.Instance.permanentIncreaseMovementCount >= 1) return false;
                            break;
                        // Unit A
                        case PowerTypeManager.Types.OctopusThirdAttackBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Octopus)) return false;
                            if (EnforceManager.Instance.octopusThirdAttackBoost) return false;
                            break;
                        case PowerTypeManager.Types.OctopusPoisonAttack:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Octopus)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Octopus) < 3) return false;
                            if (EnforceManager.Instance.octopusPoisonAttack) return false;
                            break;
                        case PowerTypeManager.Types.OctopusBleedDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Octopus)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Octopus) < 5) return false;
                            if (EnforceManager.Instance.octopusBleedDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.OctopusPoisonDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Octopus)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Octopus) < 7) return false;
                            if (!EnforceManager.Instance.octopusPoisonAttack) return false;
                            if (EnforceManager.Instance.octopusPoisonDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.OctopusPoisonDurationBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Octopus)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Octopus) < 9) return false;
                            if (EnforceManager.Instance.octopusPoisonDurationBoost) return false;
                            break;
                        case PowerTypeManager.Types.OctopusDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Octopus)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Octopus) < 11) return false;
                            if (EnforceManager.Instance.octopusDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.OctopusRateBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Octopus)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Octopus) < 13) return false;
                            if (EnforceManager.Instance.dark3RateBoost >= 0.36f) return false;
                            break;
                        // Unit Ogre
                        case PowerTypeManager.Types.OgreThirdAttackDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Ogre)) return false;
                            if (EnforceManager.Instance.ogreThirdAttackDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.OgreStatusAilmentSlowEffect:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Ogre)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Ogre) < 3) return false;
                            if (EnforceManager.Instance.ogreStatusAilmentSlowEffect) return false;
                            break;
                        case PowerTypeManager.Types.OgreRangeIncrease:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Ogre)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Ogre) < 5) return false;
                            if (EnforceManager.Instance.ogreRangeIncrease) return false;
                            break;
                        case PowerTypeManager.Types.OgreAttackPowerBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Ogre)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Ogre) < 7) return false;
                            if (EnforceManager.Instance.darkAttackPowerBoost >= 0.24f) return false;
                            break;
                        case PowerTypeManager.Types.OgreStatusAilmentDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Ogre)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Ogre) < 9) return false;
                            if (EnforceManager.Instance.ogreStatusAilmentDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.OgreAttackSpeedBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Ogre)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Ogre) < 11) return false;
                            if (EnforceManager.Instance.darkAttackSpeedBoost >= 0.36f) return false;
                            break;
                        case PowerTypeManager.Types.OgreKnockBackChance:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Ogre)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Ogre) < 13) return false;
                            if (EnforceManager.Instance.ogreKnockBackChance) return false;
                            break;
                        // Unit DeathChiller
                        case PowerTypeManager.Types.DeathChillerFreeze:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.DeathChiller)) return false;
                            if (EnforceManager.Instance.deathChillerFreeze) return false;
                            break;
                        case PowerTypeManager.Types.DeathChillerFreezeChance:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.DeathChiller)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.DeathChiller) < 3) return false;
                            if (!EnforceManager.Instance.deathChillerFreeze) return false;
                            break;
                        case PowerTypeManager.Types.DeathChillerSlowDurationBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.DeathChiller)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.DeathChiller) < 5) return false;
                            if (EnforceManager.Instance.waterSlowDurationBoost >= 1f) return false;
                            break;
                        case PowerTypeManager.Types.DeathChillerFreezeDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.DeathChiller)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.DeathChiller) < 7) return false;
                            if (!EnforceManager.Instance.deathChillerFreeze) return false;
                            if (EnforceManager.Instance.deathChillerFreezeDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.DeathChillerSlowCPowerBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.DeathChiller)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.DeathChiller) < 9) return false;
                            if (EnforceManager.Instance.deathChillerSlowCPowerBoost) return false;
                            break;
                        case PowerTypeManager.Types.DeathChillerAttackRateBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.DeathChiller)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.DeathChiller) < 11) return false;
                            if (EnforceManager.Instance.waterAttackRateBoost >= 0.24f) return false;
                            break;
                        case PowerTypeManager.Types.DeathChillerBackAttackBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.DeathChiller)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.DeathChiller) < 13) return false;
                            if (EnforceManager.Instance.deathChillerBackAttackBoost) return false;
                            break;
                        // Unit Orc
                        case PowerTypeManager.Types.OrcSwordScaleIncrease:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Orc)) return false;
                            if (EnforceManager.Instance.orcSwordScaleIncrease) return false;
                            break;
                        case PowerTypeManager.Types.OrcSwordAddition:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Orc)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Orc) < 3) return false;
                            if (EnforceManager.Instance.orcSwordAddition) return false;
                            break;
                        case PowerTypeManager.Types.OrcAttackSpeedBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Orc)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Orc) < 5) return false;
                            if (EnforceManager.Instance.physicalAttackSpeedBoost >= 0.36f) return false;
                            break;
                        case PowerTypeManager.Types.OrcRatePerAttack:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Orc)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Orc) < 7) return false;
                            if (EnforceManager.Instance.orcRatePerAttack) return false;
                            break;
                        case PowerTypeManager.Types.OrcBindBleed:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Orc)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Orc) < 9) return false;
                            if (EnforceManager.Instance.orcBindBleed) return false;
                            break;
                        case PowerTypeManager.Types.OrcDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Orc)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Orc) < 11) return false;
                            if (EnforceManager.Instance.physicalDamageBoost >= 0.18f) return false;
                            break;
                        case PowerTypeManager.Types.OrcBleedDuration:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Orc)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Orc) < 13) return false;
                            if (!EnforceManager.Instance.orcBindBleed) return false;
                            if (EnforceManager.Instance.orcBleedDuration) return false;
                            break;
                        // Unit Fishman
                        case PowerTypeManager.Types.FishmanFreeze:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Fishman)) return false;
                            if (EnforceManager.Instance.fishmanFreeze) return false;
                            break;
                        case PowerTypeManager.Types.FishmanSlowPowerBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Fishman)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Fishman) < 3) return false;
                            if (EnforceManager.Instance.fishmanSlowPowerBoost) return false;
                            break;
                        case PowerTypeManager.Types.FishmanFreezeTimeBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Fishman)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Fishman) < 5) return false;
                            if (!EnforceManager.Instance.fishmanFreeze) return false;
                            if (EnforceManager.Instance.fishmanFreezeTimeBoost) return false;
                            break;
                        case PowerTypeManager.Types.FishmanDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Fishman)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Fishman) < 7) return false;
                            if (EnforceManager.Instance.water2DamageBoost >= 0.24f) return false;
                            break;
                        case PowerTypeManager.Types.FishmanFreezeChanceBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Fishman)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Fishman) < 9) return false;
                            if (!EnforceManager.Instance.fishmanFreeze) return false;
                            if (EnforceManager.Instance.fishmanFreezeChanceBoost) return false;
                            break;
                        case PowerTypeManager.Types.FishmanFreezeDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Fishman)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Fishman) < 11) return false;
                            if (!EnforceManager.Instance.fishmanFreeze) return false;
                            if (EnforceManager.Instance.fishmanFreezeDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.FishmanSlowTimeBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Fishman)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Fishman) < 13) return false;
                            if (EnforceManager.Instance.water2SlowTimeBoost >= 0.5f) return false;
                            break;
                        //Unit Skeleton
                        case PowerTypeManager.Types.SkeletonPerHitEffect:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Skeleton)) return false;
                            if (EnforceManager.Instance.skeletonPerHitEffect) return false;
                            break;
                        case PowerTypeManager.Types.SkeletonBleedingEnemyDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Skeleton)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Skeleton) < 3) return false;
                            if (EnforceManager.Instance.skeletonBleedingEnemyDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.SkeletonDamagePerBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Skeleton)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Skeleton) < 5) return false;
                            if (EnforceManager.Instance.skeletonDamagePerBoost) return false;
                            break;
                        case PowerTypeManager.Types.SkeletonDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Skeleton)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Skeleton) < 7) return false;
                            if (EnforceManager.Instance.skeletonDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.SkeletonDotDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Skeleton)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Skeleton) < 9) return false;
                            if (!EnforceManager.Instance.skeletonPerHitEffect) return false;
                            if (EnforceManager.Instance.skeletonDotDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.SkeletonAttackSpeedIncrease:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Skeleton)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Skeleton) < 11) return false;
                            if (EnforceManager.Instance.poisonAttackSpeedIncrease >= 0.24f) return false;
                            break;
                        case PowerTypeManager.Types.SkeletonDurationBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Skeleton)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Skeleton) < 13) return false;
                            if (!EnforceManager.Instance.skeletonPerHitEffect) return false;
                            if (EnforceManager.Instance.skeletonDurationBoost) return false;
                            break;
                        //Unit Phoenix
                        case PowerTypeManager.Types.PhoenixFreezeDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Phoenix)) return false;
                            if (EnforceManager.Instance.phoenixFreezeDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.PhoenixBurnDurationBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Phoenix)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Phoenix) < 3) return false;
                            if (EnforceManager.Instance.phoenixBurnDurationBoost) return false;
                            break;
                        case PowerTypeManager.Types.PhoenixChangeProperty:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Phoenix)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Phoenix) < 5) return false;
                            if (EnforceManager.Instance.phoenixChangeProperty) return false;
                            break;
                        case PowerTypeManager.Types.PhoenixDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Phoenix)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Phoenix) < 7) return false;
                            if (EnforceManager.Instance.fire2DamageBoost >= 0.24f) return false;
                            break;
                        case PowerTypeManager.Types.PhoenixRangeBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Phoenix)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Phoenix) < 9) return false;
                            if (EnforceManager.Instance.phoenixRangeBoost) return false;
                            break;
                        case PowerTypeManager.Types.PhoenixRateBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Phoenix)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Phoenix) < 11) return false;
                            if (EnforceManager.Instance.phoenixRateBoost) return false;
                            break;
                        case PowerTypeManager.Types.PhoenixBossDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Phoenix)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Phoenix) < 13) return false;
                            if (EnforceManager.Instance.phoenixBossDamageBoost) return false;
                            break;
                        //Unit Beholder
                        case PowerTypeManager.Types.BeholderBurnPerAttackEffect:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Beholder)) return false;
                            if (EnforceManager.Instance.beholderBurnPerAttackEffect) return false;
                            break;
                        case PowerTypeManager.Types.BeholderStackOverlap:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Beholder)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Beholder) < 3) return false;
                            if (!EnforceManager.Instance.beholderBurnPerAttackEffect) return false;
                            if (EnforceManager.Instance.beholderStackOverlap) return false;
                            break;
                        case PowerTypeManager.Types.BeholderProjectileBounceDamage:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Beholder)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Beholder) < 5) return false;
                            if (EnforceManager.Instance.beholderProjectileBounceDamage) return false;
                            break;
                        case PowerTypeManager.Types.BeholderDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Beholder)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Beholder) < 7) return false;
                            if (EnforceManager.Instance.beholderAttackDamageBoost >= 0.24f) return false;
                            break;
                        case PowerTypeManager.Types.BeholderAttackSpeedBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Beholder)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Beholder) < 9) return false;
                            if (EnforceManager.Instance.fireAttackSpeedBoost >= 0.24f) return false;
                            break;
                        case PowerTypeManager.Types.BeholderProjectileSpeedIncrease:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Beholder)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Beholder) < 11) return false;
                            if (EnforceManager.Instance.beholderProjectileSpeedIncrease) return false;
                            break;
                        case PowerTypeManager.Types.BeholderProjectileBounceIncrease:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Beholder)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Beholder) < 13) return false;
                            if (!EnforceManager.Instance.beholderProjectileBounceDamage) return false;
                            if (EnforceManager.Instance.beholderProjectileBounceIncrease) return false;
                            break;
                        // Unit Cobra
                        case PowerTypeManager.Types.CobraStunToChance:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Cobra)) return false;
                            if (EnforceManager.Instance.cobra2StunToChance) return false;
                            break;
                        case PowerTypeManager.Types.CobraRangeBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Cobra)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Cobra) < 3) return false;
                            if (EnforceManager.Instance.cobraRangeBoost) return false;
                            break;
                        case PowerTypeManager.Types.CobraDotDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Cobra)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Cobra) < 5) return false;
                            if (EnforceManager.Instance.cobraDotDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.CobraStunTimeBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Cobra)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Cobra) < 7) return false;
                            if (!EnforceManager.Instance.cobra2StunToChance) return false;
                            if (EnforceManager.Instance.poison2StunTimeBoost >= 0.5f) return false;
                            break;
                        case PowerTypeManager.Types.CobraDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Cobra)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Cobra) < 9) return false;
                            if (EnforceManager.Instance.cobraDamageBoost >= 0.36f) return false;
                            break;
                        case PowerTypeManager.Types.CobraRateBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Cobra)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Cobra) < 11) return false;
                            if (EnforceManager.Instance.cobraRateBoost) return false;
                            break;
                        case PowerTypeManager.Types.CobraStunChanceBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Cobra)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Cobra) < 13) return false;
                            if (EnforceManager.Instance.cobraStunChanceBoost) return false;
                            break;
                        // Unit J
                        case PowerTypeManager.Types.BerserkerCastleCrushStatBoost:                         
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Berserker)) return false;
                            if (EnforceManager.Instance.berserkerCastleCrushStatBoost) return false;
                            break;
                        case PowerTypeManager.Types.BerserkerThirdBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Berserker)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Berserker) < 3) return false;
                            if (EnforceManager.Instance.berserkerThirdBoost) return false;
                            break;
                        case PowerTypeManager.Types.BerserkerBleedTimeBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Berserker)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Berserker) < 5) return false;
                            if (EnforceManager.Instance.berserkerBleedTimeBoost) return false;
                            break;
                        case PowerTypeManager.Types.BerserkerPoisonDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Berserker)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Berserker) < 7) return false;
                            if (EnforceManager.Instance.berserkerPoisonDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.BerserkerRangeBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Berserker)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Berserker) < 9) return false;
                            if (EnforceManager.Instance.berserkerRangeBoost) return false;
                            break;
                        case PowerTypeManager.Types.BerserkerRateBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Berserker)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Berserker) < 11) return false;
                            if (EnforceManager.Instance.physical2RateBoost >= 0.36f) return false;
                            break;
                        case PowerTypeManager.Types.BerserkerBossBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.Berserker)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.Berserker) < 13) return false;
                            if (EnforceManager.Instance.berserkerBossBoost) return false;
                            break;
                        // Unit DarkElf
                        case PowerTypeManager.Types.DarkElfBackBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.DarkElf)) return false;
                            if (EnforceManager.Instance.darkElfBackBoost) return false;
                            break;
                        case PowerTypeManager.Types.DarkElfDualAttack:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.DarkElf)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.DarkElf) < 3) return false;
                            if (EnforceManager.Instance.darkElfDualAttack) return false;
                            break;
                        case PowerTypeManager.Types.DarkElfStatusDamageBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.DarkElf)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.DarkElf) < 5) return false;
                            if (EnforceManager.Instance.darkElfStatusDamageBoost) return false;
                            break;
                        case PowerTypeManager.Types.DarkElfExplosionBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.DarkElf)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.DarkElf) < 7) return false;
                            if (EnforceManager.Instance.darkElfExplosionBoost) return false;
                            break;
                        case PowerTypeManager.Types.DarkElfDoubleAttack:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.DarkElf)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.DarkElf) < 9) return false;
                            if (EnforceManager.Instance.darkElfDoubleAttack) return false;
                            break;
                        case PowerTypeManager.Types.DarkElfStatusPoison:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.DarkElf)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.DarkElf) < 11) return false;
                            if (EnforceManager.Instance.darkElfStatusPoison) return false;
                            break;
                        case PowerTypeManager.Types.DarkElfSameEnemyBoost:
                            if (!HasUnitInGroup(CharacterBase.UnitGroups.DarkElf)) return false;
                            if (UnitPieceLevel(CharacterBase.UnitGroups.DarkElf) < 13) return false;
                            if (EnforceManager.Instance.darkElfSameEnemyBoost) return false;
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
            if (powerUps[2] == null)
            {
                Debug.Log("???");
            }
            
            LevelUpDisplayText(exp1Button, exp1Text, icon1, exp1BtnBadge, powerUps[0]);
            LevelUpDisplayText(exp2Button, exp2Text, icon2, exp2BtnBadge, powerUps[1]);
            LevelUpDisplayText(exp3Button, exp3Text, icon3, exp3BtnBadge, powerUps[2]);
        }
        private void LevelUpDisplayText(Button expButton, TMP_Text powerText, Image icon, Image btnBadge, Data powerUp)
        {
            var finalDesc = powerUp.Desc;
            var placeholderValues = new Dictionary<string, Func<double>> {{ "{p}", () => powerUp.Property[0]}};
            finalDesc = placeholderValues.Aggregate(finalDesc, (current, placeholder) => current.Replace(placeholder.Key, placeholder.Value().ToString(CultureInfo.CurrentCulture)));
            var finalTranslation = finalDesc.Replace("||", "\n");
            icon.sprite = powerUp.Icon;
            btnBadge.sprite = powerUp.BtnColor;
            expButton.GetComponent<Image>().sprite = powerUp.BackGroundColor;
            powerText.text = powerUp.Type switch
            {
                PowerTypeManager.Types.GroupDamage1 => finalTranslation,
                PowerTypeManager.Types.GroupDamage2 => finalTranslation,
                PowerTypeManager.Types.GroupDamage3 => finalTranslation,
                PowerTypeManager.Types.GroupAtkSpeed1 => finalTranslation,
                PowerTypeManager.Types.GroupAtkSpeed2 => finalTranslation,
                PowerTypeManager.Types.GroupAtkSpeed3 => finalTranslation,
                PowerTypeManager.Types.StepDirection => finalTranslation,
                PowerTypeManager.Types.Exp => finalTranslation,
                PowerTypeManager.Types.CastleRecovery => finalTranslation,
                PowerTypeManager.Types.CastleMaxHp => finalTranslation,
                PowerTypeManager.Types.Slow => finalTranslation,
                PowerTypeManager.Types.NextStage => finalTranslation,
                PowerTypeManager.Types.Gold => finalTranslation,
                PowerTypeManager.Types.Match5Upgrade => finalTranslation,
                PowerTypeManager.Types.StepLimit => finalTranslation,
                PowerTypeManager.Types.OctopusThirdAttackBoost => finalTranslation,
                PowerTypeManager.Types.OctopusPoisonAttack => finalTranslation,
                PowerTypeManager.Types.OctopusBleedDamageBoost => finalTranslation,
                PowerTypeManager.Types.OctopusPoisonDurationBoost => finalTranslation,
                PowerTypeManager.Types.OctopusPoisonDamageBoost => finalTranslation,
                PowerTypeManager.Types.OctopusDamageBoost => finalTranslation,
                PowerTypeManager.Types.OctopusRateBoost => finalTranslation,
                PowerTypeManager.Types.OgreThirdAttackDamageBoost => finalTranslation,
                PowerTypeManager.Types.OgreAttackSpeedBoost => finalTranslation,
                PowerTypeManager.Types.OgreAttackPowerBoost => finalTranslation,
                PowerTypeManager.Types.OgreKnockBackChance => finalTranslation,
                PowerTypeManager.Types.OgreStatusAilmentDamageBoost => finalTranslation,
                PowerTypeManager.Types.OgreRangeIncrease => finalTranslation,
                PowerTypeManager.Types.OgreStatusAilmentSlowEffect => finalTranslation,
                PowerTypeManager.Types.DeathChillerFreeze => finalTranslation,
                PowerTypeManager.Types.DeathChillerFreezeChance => finalTranslation,
                PowerTypeManager.Types.DeathChillerSlowDurationBoost => finalTranslation,
                PowerTypeManager.Types.DeathChillerFreezeDamageBoost => finalTranslation,
                PowerTypeManager.Types.DeathChillerSlowCPowerBoost => finalTranslation,
                PowerTypeManager.Types.DeathChillerAttackRateBoost => finalTranslation,
                PowerTypeManager.Types.DeathChillerBackAttackBoost => finalTranslation,
                PowerTypeManager.Types.OrcSwordScaleIncrease => finalTranslation,
                PowerTypeManager.Types.OrcSwordAddition => finalTranslation,
                PowerTypeManager.Types.OrcAttackSpeedBoost => finalTranslation,
                PowerTypeManager.Types.OrcRatePerAttack => finalTranslation,
                PowerTypeManager.Types.OrcDamageBoost => finalTranslation,
                PowerTypeManager.Types.OrcBindBleed => finalTranslation,
                PowerTypeManager.Types.OrcBleedDuration => finalTranslation,
                PowerTypeManager.Types.FishmanFreeze => finalTranslation,
                PowerTypeManager.Types.FishmanSlowPowerBoost => finalTranslation,
                PowerTypeManager.Types.FishmanFreezeTimeBoost => finalTranslation,
                PowerTypeManager.Types.FishmanDamageBoost => finalTranslation,
                PowerTypeManager.Types.FishmanFreezeChanceBoost => finalTranslation,
                PowerTypeManager.Types.FishmanFreezeDamageBoost => finalTranslation,
                PowerTypeManager.Types.FishmanSlowTimeBoost => finalTranslation,
                PowerTypeManager.Types.SkeletonPerHitEffect => finalTranslation,
                PowerTypeManager.Types.SkeletonBleedingEnemyDamageBoost => finalTranslation,
                PowerTypeManager.Types.SkeletonDamagePerBoost => finalTranslation,
                PowerTypeManager.Types.SkeletonDamageBoost => finalTranslation,
                PowerTypeManager.Types.SkeletonDotDamageBoost => finalTranslation,
                PowerTypeManager.Types.SkeletonAttackSpeedIncrease => finalTranslation,
                PowerTypeManager.Types.SkeletonDurationBoost => finalTranslation,
                PowerTypeManager.Types.PhoenixFreezeDamageBoost => finalTranslation,
                PowerTypeManager.Types.PhoenixBurnDurationBoost => finalTranslation,
                PowerTypeManager.Types.PhoenixChangeProperty => finalTranslation,
                PowerTypeManager.Types.PhoenixDamageBoost => finalTranslation,
                PowerTypeManager.Types.PhoenixRangeBoost => finalTranslation,
                PowerTypeManager.Types.PhoenixRateBoost => finalTranslation,
                PowerTypeManager.Types.PhoenixBossDamageBoost => finalTranslation,
                PowerTypeManager.Types.BeholderBurnPerAttackEffect => finalTranslation,
                PowerTypeManager.Types.BeholderStackOverlap => finalTranslation,
                PowerTypeManager.Types.BeholderProjectileBounceDamage => finalTranslation,
                PowerTypeManager.Types.BeholderDamageBoost => finalTranslation,
                PowerTypeManager.Types.BeholderAttackSpeedBoost => finalTranslation,
                PowerTypeManager.Types.BeholderProjectileSpeedIncrease => finalTranslation,
                PowerTypeManager.Types.BeholderProjectileBounceIncrease => finalTranslation,
                PowerTypeManager.Types.CobraStunToChance => finalTranslation,
                PowerTypeManager.Types.CobraRangeBoost => finalTranslation,
                PowerTypeManager.Types.CobraDotDamageBoost => finalTranslation,
                PowerTypeManager.Types.CobraStunTimeBoost => finalTranslation,
                PowerTypeManager.Types.CobraDamageBoost => finalTranslation,
                PowerTypeManager.Types.CobraRateBoost => finalTranslation,
                PowerTypeManager.Types.CobraStunChanceBoost => finalTranslation,
                PowerTypeManager.Types.BerserkerCastleCrushStatBoost => finalTranslation,
                PowerTypeManager.Types.BerserkerThirdBoost => finalTranslation,
                PowerTypeManager.Types.BerserkerBleedTimeBoost => finalTranslation,
                PowerTypeManager.Types.BerserkerPoisonDamageBoost => finalTranslation,
                PowerTypeManager.Types.BerserkerRangeBoost => finalTranslation,
                PowerTypeManager.Types.BerserkerRateBoost => finalTranslation,
                PowerTypeManager.Types.BerserkerBossBoost => finalTranslation,
                PowerTypeManager.Types.DarkElfBackBoost => finalTranslation,
                PowerTypeManager.Types.DarkElfDualAttack => finalTranslation,
                PowerTypeManager.Types.DarkElfStatusDamageBoost => finalTranslation,
                PowerTypeManager.Types.DarkElfExplosionBoost => finalTranslation,
                PowerTypeManager.Types.DarkElfDoubleAttack => finalTranslation,
                PowerTypeManager.Types.DarkElfStatusPoison => finalTranslation,
                PowerTypeManager.Types.DarkElfSameEnemyBoost => finalTranslation,
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
