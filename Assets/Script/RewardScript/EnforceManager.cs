using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Script.CharacterManagerScript;
using Script.PuzzleManagerGroup;
using Script.RobbyScript.CharacterSelectMenuGroup;
using Script.UIManager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.RewardScript
{
    [Serializable]
    public class SkillInstanceData
    {
        public PowerTypeManager.Types type;
        public float? Value;
    }

    [Serializable]
    public class EnforceData
    {
        //Darkness3 Unit A
        public bool dark3FifthAttackBoost;
        public bool dark3BleedAttack;
        public bool dark3PoisonDamageBoost;
        public bool dark3ShackledExplosion;
        public bool dark3BleedDurationBoost;
        public bool dark3DamageBoost;
        public float dark3RateBoost;

        //Darkness Unit Ogre
        public bool darkFifthAttackDamageBoost;
        public float darkAttackSpeedBoost;
        public float darkAttackPowerBoost;
        public bool darkKnockBackChance;
        public bool darkStatusAilmentDamageBoost;
        public bool darkRangeIncrease;
        public bool darkStatusAilmentSlowEffect;

        //Water1 Unit DeathChiller
        public bool waterFreeze;
        public bool waterFreezeChance;
        public float waterSlowDurationBoost;
        public bool waterFreezeDamageBoost;
        public bool waterSlowCPowerBoost;
        public float waterAttackRateBoost;
        public bool waterGlobalFreeze;

        //Physical Unit Orc
        public bool physicalSwordScaleIncrease;
        public bool physicalSwordAddition;
        public float physicalAttackSpeedBoost;
        public bool physicalRatePerAttack;
        public bool physicalBindBleed;
        public float physicalDamageBoost;
        public bool physicalBleedDuration;

        //Water2 Unit Fishman
        public bool water2Freeze;
        public bool water2SlowPowerBoost;
        public bool water2FreezeTimeBoost;
        public float water2DamageBoost;
        public bool water2FreezeChanceBoost;
        public bool water2FreezeDamageBoost;
        public float water2SlowTimeBoost;

        //Poison Unit Skeleton
        public bool poisonPerHitEffect;
        public bool poisonBleedingEnemyDamageBoost;
        public bool poisonDamagePerBoost;
        public bool poisonDamageBoost;
        public bool poisonDotDamageBoost;
        public float poisonAttackSpeedIncrease;
        public bool poisonDurationBoost;

        //Fire2 Unit Phoenix
        public bool fire2FreezeDamageBoost;
        public bool fire2BurnDurationBoost;
        public bool fire2ChangeProperty;
        public float fire2DamageBoost;
        public bool fire2RangeBoost;
        public bool fire2RateBoost;
        public bool fire2BossDamageBoost;


        //Fire1 Unit Beholder
        public bool fireBurnPerAttackEffect;
        public bool fireStackOverlap;
        public bool fireProjectileBounceDamage;
        public float fireBurnedEnemyExplosion;
        public float fireAttackSpeedBoost;
        public bool fireProjectileSpeedIncrease;
        public bool fireProjectileBounceIncrease;

        //Poison2 Unit Cobra
        public bool poison2StunToChance;
        public bool poison2RangeBoost;
        public bool poison2DotDamageBoost;
        public float poison2StunTimeBoost;
        public float poison2SpawnPoisonArea;
        public bool poison2RateBoost;
        public bool poison2PoolTimeBoost;
      
        //Physical2 Unit J
        public bool physical2CastleCrushStatBoost;
        public bool physical2FifthBoost;
        public bool physical2BleedTimeBoost;
        public bool physical2PoisonDamageBoost;
        public bool physical2RangeBoost;
        public float physical2RateBoost;
        public bool physical2BossBoost;

        //Dark2 Unit DarkElf
        public bool dark2BackBoost;
        public bool dark2DualAttack;
        public bool dark2StatusDamageBoost;
        public bool dark2ExplosionBoost;
        public bool dark2DoubleAttack;
        public bool dark2StatusPoison;
        public bool dark2SameEnemyBoost;

        //common
        public bool addRow;
        public int slowCount;
        public bool diagonalMovement;
        public bool recoveryCastle;
        public float castleMaxHp;
        public int highLevelCharacterCount;
        public int selectedCount;
        public int expPercentage;
        public int permanentIncreaseMovementCount;
        public bool match5Upgrade;
        public float increaseAtkDamage;
        public float increaseAtkRate;
        public int rewardMoveCount;
        public int addGoldCount;
        public bool addGold;
        public List<int> permanentGroupIndex;

        public List<SkillInstanceData> skillInstances;
    }

    public class EnforceManager : MonoBehaviour
    {
        // Skill Grid
        [SerializeField] private GameObject skillGrid;
        [SerializeField] private PauseSkillObjectScript skillPrefabs;
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private CountManager countManager;
        [SerializeField] private SpawnManager spawnManager;
        [RuntimeInitializeOnLoadMethod] 
        private static void InitializeOnLoad()
        {
            var existingInstance = FindObjectOfType<EnforceManager>();
            if (existingInstance == null)
            {
                new GameObject("EnforceManager").AddComponent<EnforceManager>();
            }
        }
        public static EnforceManager Instance { get; private set; }
        public List<CharacterBase> characterList = new List<CharacterBase>();
       
        private void Awake()
        {
            Instance = this;
            var selectUnitList = SelectedUnitHolder.Instance.selectedUnit;
            foreach (var unit in selectUnitList)
            {
                characterList.Add(unit);
            }
        }

        [Header("\n\nA 어둠: B\n")]
        // 완료
        [Header("B / 1Lv: 3회 공격마다 100% 추가데미지 (투사체 컬러 변경)")] 
        public bool octopusThirdAttackBoost;
        // 완료
        [Header("P / 3Lv: 적을 공격하면, 초당 공격력의 20% 데미지를 주는 중독을 3초간 발생시킵니다.")]
        public bool octopusPoisonAttack; 
        // 완료
        [Header("G / 5Lv: 출혈중인 적 추가데미지 150%")] 
        public bool octopusBleedDamageBoost;
        // 완료
        [Header("B / 7Lv: 중독 피해가 초당 30% 증가")] 
        public bool octopusPoisonDamageBoost;
        // 완료
        [Header("G / 9Lv: 중독지속시간 2초 증가")] 
        public bool octopusPoisonDurationBoost;
        // 완료
        [Header("P / 11Lv: 공력력 19% 증가")] 
        public bool octopusDamageBoost;
        // 완료
        [Header("B / 13Lv: 공격속도 9% 증가 (최대 4회)")]
        public float dark3RateBoost; 
        protected internal void OctopusRateBoost()
        {   
            if (dark3RateBoost >= 0.36f) return;
            dark3RateBoost+= 0.09f;
        }


        [Header("\n\nB 어둠: G\n")]
        // 완료
        [Header("G / 1Lv: 3회 공격마다 100% 추가 데미지")] 
        public bool ogreThirdAttackDamageBoost;
        // 완료
       [Header("B / 3Lv: 상태이상 적 공격시 1초 이동속도 20% 감소")] 
        public bool ogreStatusAilmentSlowEffect;
        // 완료
        [Header("P / 5Lv: 사거리 1 증가")] 
        public bool ogreRangeIncrease;
        // 완료
        [Header("G / 7Lv: 공격력 4% 증가 (최대 6회)")]
        public float darkAttackPowerBoost; 
        protected internal void OgreAttackDamageBoost()
        {
            if (darkAttackPowerBoost >= 0.24f) return;
            darkAttackPowerBoost += 0.04f;
        }
        // 완료
      [Header("P / 9Lv: 상태이상 적 공격시 50% 확률로 50% 추가데미지")] 
        public bool ogreStatusAilmentDamageBoost;
        // 완료
        [Header("B / 11: 공격속도 9% 증가 (최대 4회)")]
        public float darkAttackSpeedBoost;
        protected internal void OgreAttackSpeedBoost()
        {
            if(darkAttackSpeedBoost >= 0.36f) return;
            darkAttackSpeedBoost += 0.09f;
        }
        // 완료
        [Header("B / 13Lv: 10% 확률로 적 밀침 (0.5칸)")] 
        public bool ogreKnockBackChance;
        


        [Header("\n\nC 물: P\n")]
        // 완료
        [Header("B / 1Lv: 15% 확률로 적을 1초간 빙결(이동불가)시킵니다.")]
        public bool deathChillerFreeze;
        // 완료
        [Header("B / 3Lv: 빙결 확률이 10% 증가합니다.")]
        public bool deathChillerFreezeChance;
        // 완료
        [Header("G / 5Lv: 둔화 지속시간 0.2초 증가 (최대 5회 / 1초)")]
        public float waterSlowDurationBoost;
        protected internal void DeathChillerSlowDurationBoost()
        {
            if (waterSlowDurationBoost >= 1f) return;
            waterSlowDurationBoost += 0.2f;
        }
        // 완료
[Header("P / 7Lv: DeathChiller 유닛에게 빙결된 적은 받는 피해 15% 증가")]
        public bool deathChillerFreezeDamageBoost;
        // 완료
[Header("B / 9Lv: 둔화강도 15% 증가")] 
        public bool deathChillerSlowCPowerBoost;
        // 완료
        [Header("G / 11Lv: 공격속도 4% 증가 (최대 6회)")]
        public float waterAttackRateBoost;
        protected internal void DeathChillerAttackRateBoost()
        {
            if ( waterAttackRateBoost >= 0.24f) return;
            waterAttackRateBoost += 0.06f;
        }
        // 완료
        [Header("P / 13Lv: 적의 뒤를 공격하면 데미지 20% 증가")]
        public bool deathChillerBackAttackBoost;



        [Header("\n\nD 물리: G\n")]
        // 완료
        [Header("P / 1Lv: 검의 크기가 100% 증가")] 
        public bool orcSwordScaleIncrease;
        // 완료
        [Header("P / 3Lv: 공격 시 검 한자루 추가")] 
        public bool orcSwordAddition;
        // 완료
        [Header("B / 5Lv: 공격속도 9% 증가 (최대 4회)")]
        public float physicalAttackSpeedBoost; 
        protected internal void OrcAttackSpeedIncrease()
        {
            if (physicalAttackSpeedBoost >= 0.36f) return;
            physicalAttackSpeedBoost += 0.09f;
        }
        // 완료
[Header("B / 7Lv: 해당 웨이브에서 유닛 3회 공격당 공속 1% 증가 (최대 60%)")]
        public bool orcRatePerAttack;
        // 완료
[Header("B / 9Lv: 속박에 걸린적을 공격하면 초당 20% 데미지의 3초간 출혈 발생")]
        public bool orcBindBleed;
        // 완료
        [Header("G / 11Lv: 공격력 3% 증가 (최대 6회)")]
        public float physicalDamageBoost;
        protected internal void OrcDamageBoost()
        {
            if (physicalDamageBoost >= 0.18f) return;
            physicalDamageBoost += 0.06f;
        }
        // 완료
[Header("G / 13Lv: 출혈 지속시간 2초 증가")]
        public bool orcBleedDuration;



        [Header("\n\nE 물: G\n")]
        // 완료
        [Header("B / 1Lv: 타격시 10% 확률로 적을 1초간 빙결 (이동불가)")]
        public bool fishmanFreeze;
        // 완료
[Header("P / 3Lv: 둔화강도가 10% 증가합니다.")]
        public bool fishmanSlowPowerBoost;
        // 완료
[Header("B / 5Lv: 빙결지속시간 0.5초 증가")] 
        public bool fishmanFreezeTimeBoost;
        // 완료
        [Header("G / 7Lv: 공격력 4% 증가 (최대 6회)")]
        public float water2DamageBoost;
        protected internal void FishmanDamageBoost()
        {
            if (water2DamageBoost >= 0.24f) return;
            water2DamageBoost += 0.04f;
        }
        // 완료
        [Header("P / 9Lv: 빙결확률 10% 증가")] 
        public bool fishmanFreezeChanceBoost;
        // 완료 
        [Header("B / 11Lv: 빙결당한적은 받는 피해가 15% 증가")]
        public bool fishmanFreezeDamageBoost;
        // 완료
        [Header("G / 13Lv: 둔화지속시간 0.1초 증가 (최대 0.5초)")]
        public float water2SlowTimeBoost;
        protected internal void FishmanSlowTimeBoost()
        {
            if (water2SlowTimeBoost >= 0.5f) return;
            water2SlowTimeBoost += 0.1f;
        }

        [Header("\n\nF 독: G\n")]
        // 완료
        [Header("P / 1Lv: 초당 20% 데미지를 가하는 중독을 3초간 발생")] 
        public bool skeletonPerHitEffect;
        // 완료
        [Header("G / 3Lv: 출혈중인 적 공격시 80% 데미지 증가")] 
        public bool skeletonBleedingEnemyDamageBoost;
        // 완료
       [Header("B / 5Lv: 유닛 C가 5회 공격마다 공격력 1% 증가, 웨이브마다 초기화 (최대 60%)")]
        public bool skeletonDamagePerBoost;
        // 완료
      [Header("P / 7Lv: 공격력 16% 증가")] 
        public bool skeletonDamageBoost;
        // 완료
        [Header("B / 9Lv: 중독 피해 10% 증가")] 
        public bool skeletonDotDamageBoost;
        // 완료
        [Header("G / 11Lv: 공격속도 4% 증가 (최대 6회)")]
        public float poisonAttackSpeedIncrease; 
        protected internal void SkeletonAttackSpeedIncrease()
        {
            if(poisonAttackSpeedIncrease >= 0.24f) return;
            poisonAttackSpeedIncrease += 0.04f;
        }
        // 완료
       [Header("B / 13Lv: 중족 지속시간 2초 증가")] 
        public bool skeletonDurationBoost;


        [Header("\n\nG 불: B\n")] 
        // 완료
        [Header("P / 1Lv: 빙결된 적에게 가하는 추가데미지 200%")]
        public bool phoenixFreezeDamageBoost;
        // 완료
        [Header("G / 3Lv: 화상지속시간 2초 증가")] 
        public bool phoenixBurnDurationBoost;
        // 완료
       [Header("B / 5Lv: 화상 비활성화 데미지 150% 증가")]
        public bool phoenixChangeProperty;
        // 완료
        [Header("G / 7Lv: 공격력 4% 증가 (최대 6회)")]
        public float fire2DamageBoost;
        protected internal void PhoenixDamageBoost()
        {
            if (fire2DamageBoost >= 0.24f) return;
            fire2DamageBoost += 0.04f;
        }
        // 완료
       [Header("B / 7Lv: 사거리가 1 증가합니다. ")] 
        public bool phoenixRangeBoost;
        // 완료
       [Header("P / 11Lv: 공격속도 15% 증가")] 
        public bool phoenixRateBoost;
        // 완료
        [Header("B / 13Lv: 보스 데미지 30% 증가")] 
        public bool phoenixBossDamageBoost;



        [Header("\n\nH 불: B\n")]
        // 완료
        [Header("P/ 1Lv: 적을 공격하면 5초간 화상 초당 10% 데미지")] 
        public bool beholderBurnPerAttackEffect;
        // 완료
       [Header("B/ 3Lv: 화상 중첩수 3회 증기")] 
        public bool beholderStackOverlap;
        // 완료
       [Header("P/ 5Lv: 적 적중시 가장 가까운적에게 투사체 튕김")] 
        public bool beholderProjectileBounceDamage;
        // 완료
      [Header("B/ 7Lv: 공격력 +4% 증가 (최대 6회)")] 
        public float beholderAttackDamageBoost;
        protected internal void BeholderAttackDamageBoost()
        {
            if (beholderAttackDamageBoost >= 0.24f) return;
            beholderAttackDamageBoost += 0.04f;
        }
        // 완료
        [Header("G/ 9Lv: 공격속도 4% 증가 (최대 6회)")]
        public float fireAttackSpeedBoost; 
        protected internal void BeholderAttackSpeedBoost()
        {
            if (fireAttackSpeedBoost >= 0.24f) return;
            fireAttackSpeedBoost += 0.06f;
        }
        // 완료
      [Header("G / 11Lv: 투사체 속도가 100% 증가, 반드시 명중")] 
        public bool beholderProjectileSpeedIncrease;
        // 완료
       [Header("P/ 13Lv: 투사체가 튕기는 횟수 증가")] 
        public bool beholderProjectileBounceIncrease;



        [Header("\n\nI 독: B\n")] 
        // 완료
        [Header("P / 1Lv: 타격시 50% 확률로 적을 0.4초간 기절 시킵니다.")]
        public bool cobra2StunToChance;
        // 완료
        [Header("P / 3Lv: 사거리 1칸 증가")] 
        public bool cobraRangeBoost;
        // 완료
        [Header("B / 5Lv: 중독 데미지가 10% 증가")]
        public bool cobraDotDamageBoost;
        // 완료
        [Header("G / 7Lv: 기절 시간 0.1초 증가 (최대 5회 / 0.5초)")]
        public float poison2StunTimeBoost;
        protected internal void CobraStunTimeBoost()
        {
            poison2StunTimeBoost += 0.1f;
        }
        // 완료
       [Header("B / 9Lv: 공격력 9% 증가 (최대 4회)")]
        public float cobraDamageBoost;
        protected internal void CobraDamageBoost()
        {
            if (cobraDamageBoost > 0.36f) return;
            cobraDamageBoost += 0.09f;
        }
        // 완료
       [Header("P / 11Lv: 공격속도 20% 증가")] 
        public bool cobraRateBoost;
        // 완료
        [Header("B / 13Lv: 스턴 확률이 100%로 증가합니다.")]
        public bool cobraStunChanceBoost;


        [Header("\n\nJ 물리: B\n")] 
        // 완료
        [Header("P / 1Lv: 성이 데미지를 받을 경우 해당 웨이브동안 사거리가 1 증가, 데미지 30% 증가")]
        public bool berserkerCastleCrushStatBoost;
        // 완료 
        [Header("P / 3Lv: 3회 공격마다 공격력의 200% 데미지 투사체 전방 발사")]
        public bool berserkerThirdBoost;
        // 완료
        [Header("G / 5Lv: 출혈 지속시간 2초 증가")] 
        public bool berserkerBleedTimeBoost;
        // 완료
        [Header("B / 7Lv: 중독상태의 적 공격시 데미지 60% 증가")]
        public bool berserkerPoisonDamageBoost;
        // 완료
        [Header("P / 9Lv: 사거리가 1 증가합니다.")]
        public bool berserkerRangeBoost;
        // 완료
        [Header("B / 11Lv: 공격속도 9% 증가 (최대 4회)")]
        public float physical2RateBoost;
        protected internal void BerserkerRateBoost()
        {
            physical2RateBoost += 0.09f;
        }
        // 완료
        [Header("B / 13Lv: 보스 데미지 30% 증가")] 
        public bool berserkerBossBoost;


        [Header("\n\nK 어둠: P\n")] 
        // 완료
        [Header("P / 1Lv: 적의 뒤를 공격하면, 데미지 30% 증가")]
        public bool darkElfBackBoost;
        // 완료
        [Header("P / 3Lv: 앞 뒤 동시공격")]
        public bool darkElfDualAttack;
        // 완료
        [Header("B / 5Lv: 적에게 걸린 상태이상 갯수 마다 15% 데미지 증가 (최대 5개)")]
        public bool darkElfStatusDamageBoost;
        // 완료
        [Header("B / 7Lv: 상태이상적을 처치하면 주변 2칸을 100% 추가데미지")]
        public bool darkElfExplosionBoost;
        // 완료
       [Header("P / 9Lv: 발사하는 투사체 갯수 1개 증가")]
        public bool darkElfDoubleAttack;
        // 완료
        [Header("G / 11Lv: 적이 상태이상에 걸리지 않았을때, 초당 공격력의 20%의 중독을 3초간 발생시킵니다.")]
        public bool darkElfStatusPoison;
        // 완료
        [Header("B / 13Lv: 동일한 적을 타격할때마다 데미지가 5% 증가 (최대 10회)")]
        public bool darkElfSameEnemyBoost;


        public Dictionary<int, bool> GetActivatedSkills(CharacterBase.UnitGroups unitGroup)
        {
            var activatedSkills = new Dictionary<int, bool>();
        
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.Octopus:
                    activatedSkills[1] = octopusThirdAttackBoost;
                    activatedSkills[3] = octopusPoisonAttack;
                    activatedSkills[5] = octopusBleedDamageBoost;
                    activatedSkills[7] = octopusPoisonDamageBoost;
                    activatedSkills[9] = octopusPoisonDurationBoost;
                    activatedSkills[11] = octopusDamageBoost;
                    activatedSkills[13] = dark3RateBoost > 0;
                    break;
                case CharacterBase.UnitGroups.Ogre:
                    activatedSkills[1] = ogreThirdAttackDamageBoost;
                    activatedSkills[3] = ogreStatusAilmentSlowEffect;
                    activatedSkills[5] = ogreRangeIncrease;
                    activatedSkills[7] = darkAttackPowerBoost > 0;
                    activatedSkills[9] = ogreStatusAilmentDamageBoost;
                    activatedSkills[11] = darkAttackSpeedBoost > 0; 
                    activatedSkills[13] = ogreKnockBackChance;
                    break;
                case CharacterBase.UnitGroups.DeathChiller:
                    activatedSkills[1] = deathChillerFreeze;
                    activatedSkills[3] = deathChillerFreezeChance;
                    activatedSkills[5] = waterSlowDurationBoost > 0;
                    activatedSkills[7] = deathChillerFreezeDamageBoost;
                    activatedSkills[9] = deathChillerSlowCPowerBoost;
                    activatedSkills[11] = waterAttackRateBoost > 0; 
                    activatedSkills[13] = deathChillerBackAttackBoost;
                    break;
                case CharacterBase.UnitGroups.Orc:
                    activatedSkills[1] = orcSwordScaleIncrease;
                    activatedSkills[3] = orcSwordAddition;
                    activatedSkills[5] = physicalAttackSpeedBoost > 0;
                    activatedSkills[7] = orcRatePerAttack;
                    activatedSkills[9] = orcBindBleed;
                    activatedSkills[11] = physicalDamageBoost > 0; 
                    activatedSkills[13] = orcBleedDuration;
                    break;
                case CharacterBase.UnitGroups.Fishman:
                    activatedSkills[1] = fishmanFreeze;
                    activatedSkills[3] = fishmanSlowPowerBoost;
                    activatedSkills[5] = fishmanFreezeTimeBoost;
                    activatedSkills[7] = water2DamageBoost > 0;
                    activatedSkills[9] = fishmanFreezeChanceBoost;
                    activatedSkills[11] =  fishmanFreezeDamageBoost;
                    activatedSkills[13] = water2SlowTimeBoost > 0;
                    break;
                case CharacterBase.UnitGroups.Skeleton:
                    activatedSkills[1] = skeletonPerHitEffect;
                    activatedSkills[3] = skeletonBleedingEnemyDamageBoost;
                    activatedSkills[5] = skeletonDamagePerBoost;
                    activatedSkills[7] = skeletonDamageBoost;
                    activatedSkills[9] = skeletonDotDamageBoost;
                    activatedSkills[11] = poisonAttackSpeedIncrease > 0; 
                    activatedSkills[13] = skeletonDurationBoost;
                    break;
                case CharacterBase.UnitGroups.Phoenix:
                    activatedSkills[1] = phoenixFreezeDamageBoost;
                    activatedSkills[3] = phoenixBurnDurationBoost;
                    activatedSkills[5] = phoenixChangeProperty;
                    activatedSkills[7] = fire2DamageBoost > 0;
                    activatedSkills[9] = phoenixRangeBoost;
                    activatedSkills[11] = phoenixRateBoost;
                    activatedSkills[13] = phoenixBossDamageBoost;
                    break;
                case CharacterBase.UnitGroups.Beholder:
                    activatedSkills[1] = beholderBurnPerAttackEffect;
                    activatedSkills[3] = beholderStackOverlap;
                    activatedSkills[5] = beholderProjectileBounceDamage;
                    activatedSkills[7] = beholderAttackDamageBoost > 0;
                    activatedSkills[9] = fireAttackSpeedBoost > 0;
                    activatedSkills[11] = beholderProjectileSpeedIncrease;
                    activatedSkills[13] = beholderProjectileBounceIncrease;
                    break;
                case CharacterBase.UnitGroups.Cobra:
                    activatedSkills[1] = cobra2StunToChance;
                    activatedSkills[3] = cobraRangeBoost;
                    activatedSkills[5] = cobraDotDamageBoost;
                    activatedSkills[7] = poison2StunTimeBoost > 0;
                    activatedSkills[9] = cobraDamageBoost>0;
                    activatedSkills[11] = cobraRateBoost;
                    activatedSkills[13] = cobraStunChanceBoost;
                    break;
                case CharacterBase.UnitGroups.Berserker:
                    activatedSkills[1] = berserkerCastleCrushStatBoost;
                    activatedSkills[3] = berserkerThirdBoost;
                    activatedSkills[5] = berserkerBleedTimeBoost;
                    activatedSkills[7] = berserkerPoisonDamageBoost;
                    activatedSkills[9] = berserkerRangeBoost;
                    activatedSkills[11] = physical2RateBoost > 0;
                    activatedSkills[13] = berserkerBossBoost;
                    break;
                case CharacterBase.UnitGroups.DarkElf:
                    activatedSkills[1] = darkElfBackBoost;
                    activatedSkills[3] = darkElfDualAttack;
                    activatedSkills[5] = darkElfStatusDamageBoost;
                    activatedSkills[7] = darkElfExplosionBoost;
                    activatedSkills[9] = darkElfDoubleAttack;
                    activatedSkills[11] = darkElfStatusPoison;
                    activatedSkills[13] = darkElfSameEnemyBoost;
                    break;
            }
            return activatedSkills;
        }

        private float _property;
        private readonly Dictionary<(PowerTypeManager.Types Type, int Value), PauseSkillObjectScript> _instantiatedSkills = 
            new Dictionary<(PowerTypeManager.Types Type, int Value), PauseSkillObjectScript>();
        private PauseSkillObjectScript _skill;
       
        // private string GetGroupNameFromValue(int value)
        // {
        //     var unitName = value switch
        //     {
        //         0 => characterList[0].name,
        //         1 => characterList[1].name,
        //         2 => characterList[2].name,
        //         3 => characterList[3].name,
        //     };
        //     return unitName;
        // }
        
        private void SkillInstance(Data data, float? value = null)
        {
            _property = value.GetValueOrDefault();
            var finalDesc = data.Desc;
            var placeholderValues = new Dictionary<PowerTypeManager.Types, Dictionary<string, float>>
            {
                { PowerTypeManager.Types.StepLimit , new Dictionary<string, float>{{"{p}", 1}}},
                { PowerTypeManager.Types.GroupDamage1 , new Dictionary<string, float>{{"{p}", _property}}},
                { PowerTypeManager.Types.GroupDamage2 , new Dictionary<string, float>{{"{p}", _property}}},
                { PowerTypeManager.Types.GroupDamage3 , new Dictionary<string, float>{{"{p}", _property}}},
                { PowerTypeManager.Types.GroupAtkSpeed1 , new Dictionary<string, float>{{"{p}", _property}}},
                { PowerTypeManager.Types.GroupAtkSpeed2 , new Dictionary<string, float>{{"{p}", _property}}},
                { PowerTypeManager.Types.GroupAtkSpeed3 , new Dictionary<string, float>{{"{p}", _property}}},
                { PowerTypeManager.Types.Step1 , new Dictionary<string, float>{{"{p}", _property}}},
                { PowerTypeManager.Types.Step2 , new Dictionary<string, float>{{"{p}", _property}}},
                { PowerTypeManager.Types.Step3 , new Dictionary<string, float>{{"{p}", _property}}},
                { PowerTypeManager.Types.Exp , new Dictionary<string, float>{{"{p}", _property}}},
                { PowerTypeManager.Types.CastleMaxHp, new Dictionary<string, float>{{"{p}", _property}}},
            };

            if (placeholderValues.TryGetValue(data.Type, out var placeholderValue))
            {
                finalDesc = placeholderValue.Aggregate(finalDesc, (current, placeholder) => current.Replace(placeholder.Key, placeholder.Value.ToString(CultureInfo.CurrentCulture)));
            }

            if (data.Type == PowerTypeManager.Types.NextStage)
            {
                finalDesc = finalDesc.Replace("{p}", _property.ToString(CultureInfo.CurrentCulture));
                finalDesc.Replace("{nextStage}", highLevelCharacterCount.ToString(CultureInfo.CurrentCulture));
            }
            var finalTranslation = finalDesc.Replace("||", "\n");
         
            
            if (data.Type == PowerTypeManager.Types.LevelUpPattern)
            {
                if (CommonRewardManager.Instance.LevelUpDict.TryGetValue(characterList[(int)_property].unitGroup, out var levelUpInfo))
                {
                    finalTranslation = levelUpInfo.Aggregate(finalTranslation, (current, item) => current.Replace("{0level_unit_N}", item.Value)
                        .Replace("{unit_N}", item.Key));
                }

                _skill.skillIcon.sprite = data.Icon;
                _skill.skillBackGround.sprite = characterList[(int)_property].UnitGrade switch
                {
                    CharacterBase.UnitGrades.G => PowerTypeManager.Instance.green,
                    CharacterBase.UnitGrades.B => PowerTypeManager.Instance.blue,
                    CharacterBase.UnitGrades.P => PowerTypeManager.Instance.purple,
                };

                if (_instantiatedSkills.TryGetValue((PowerTypeManager.Types.LevelUpPattern, (int)_property), out var instantiatedSkill))
                {
                    _skill = instantiatedSkill;
                }
                else
                {
                    _skill = Instantiate(skillPrefabs, skillGrid.transform);
                    _instantiatedSkills[(PowerTypeManager.Types.LevelUpPattern, (int)_property)] = _skill;
                    _skill.desc.text = finalTranslation;
                }
            }
            else
            {
                if (_instantiatedSkills.TryGetValue((data.Type,0), out var instantiatedSkill))
                {
                    _skill = instantiatedSkill;
                }
                else
                {
                    _skill = Instantiate(skillPrefabs, skillGrid.transform);
                    _instantiatedSkills[(data.Type,0)] = _skill;
                    _skill.skillBackGround.sprite = PowerTypeManager.Instance.purple;
                    _skill.skillIcon.sprite = data.Icon;
                    _skill.desc.text = finalTranslation;
                }
                if (value.HasValue)
                {
                    _skill.value.text = _property.ToString(CultureInfo.CurrentCulture);
                }
            }
        }

        [Header("\n\n공통강화\n")] 

        [Header("가로줄 추가")] 
        public bool addRow;
        protected internal void AddRow(Data data)
        {
            addRow = true;
            gridManager.AddRow();
            SkillInstance(data, 1);
        }

        [Header("전체 공격력 증가 (%)")]
        public float increaseAtkDamage; 
        protected internal void IncreaseGroupDamage(Data data, int increaseAmount)
        {
            increaseAtkDamage += increaseAmount;
            SkillInstance(data, increaseAtkDamage);
        }

        [Header("전체 공격속도 증가 (%)")]
        public float increaseAtkRate; 
        protected internal void IncreaseGroupRate(Data data, float increaseRateAmount)
        {
            increaseAtkRate += increaseRateAmount;
            SkillInstance(data, increaseAtkRate);
        }

        [Header("이동횟수 추가")]
        public int rewardMoveCount; 
        protected internal void RewardMoveCount(int moveCount)
        {
            rewardMoveCount += moveCount;
            countManager.IncreaseMoveCount(rewardMoveCount);
        }

        [Header("최대 이동횟수 증가")]
        public int permanentIncreaseMovementCount; 
        protected internal void PermanentIncreaseMoveCount(Data data, int increaseStepAmount)
        {
            permanentIncreaseMovementCount += increaseStepAmount;
            SkillInstance(data, permanentIncreaseMovementCount );
        }

        [Header("대각선 이동")] 
        public bool diagonalMovement;
        protected internal void DiagonalMovement(Data data)
        {
            diagonalMovement = true;
            SkillInstance(data);
        }

        // RandomUnitLevelUp
        public void RandomCharacterLevelUp(int characterCount)
        {
            var activeCharacters = CharacterPool.Instance.UsePoolCharacterList();
            if (activeCharacters.Count == 0) return;
            var rnd = new System.Random();
            var levelUpCount = 0;
            var eligibleCharacters = activeCharacters.Where(character => 
                character.GetComponent<CharacterBase>()?.unitPuzzleLevel < 5 && character.GetComponent<CharacterBase>().Type != CharacterBase.Types.Treasure).ToList();
            if (eligibleCharacters.Count == 0) return;
    
            while (levelUpCount < characterCount && eligibleCharacters.Count > 0)
            {
                var randomIndex = rnd.Next(eligibleCharacters.Count);
                var characterBase = eligibleCharacters[randomIndex].GetComponent<CharacterBase>();
                characterBase.LevelUpScale(eligibleCharacters[randomIndex]);
                levelUpCount++;
        
                eligibleCharacters = eligibleCharacters.Where(character => 
                    character.GetComponent<CharacterBase>()?.unitPuzzleLevel < 5).ToList();
            }
            spawnManager.AddToQueue(spawnManager.PositionUpCharacterObject());
        }

        // Unit Group LevelUp
        public void CharacterGroupLevelUp(int characterListIndex)
        {
            var group = characterList[characterListIndex].unitGroup;
            var activeCharacterGroup = CharacterPool.Instance.UsePoolCharacterList();

            foreach (var character in  activeCharacterGroup)
            {
                var characterObj = character.GetComponent<CharacterBase>();
                if (group == characterObj.unitGroup && characterObj.unitPuzzleLevel == 1)
                {
                    characterObj.LevelUpScale(character);
                }
            }
            spawnManager.AddToQueue(spawnManager.PositionUpCharacterObject());
        }

        [Header("그룹 영구 레벨업")]
        public List<int> index = new List<int>();
        public void PermanentIncreaseCharacter(Data data, int characterListIndex)
        {
            if (index.Contains(characterListIndex)) return;
            index.Add(characterListIndex);
            SkillInstance(data, characterListIndex);
        }

        [Header("경험치 증가량")]
        public int expPercentage; 
        protected internal void IncreaseExpBuff(Data data, int increaseAmount)
        {
            expPercentage += increaseAmount; 
            SkillInstance(data, expPercentage);
        }

        [Header("Castle 체력회복 200")] 
        public bool recoveryCastle;

        protected internal void RecoveryCastle(Data data)
        {
            recoveryCastle = true;
            SkillInstance(data);
        }

        [Header("Castle 최대체력 증가 (최대 2000)")]
        public float castleMaxHp; 
        protected internal void IncreaseCastleMaxHp(Data data)
        {
            if (castleMaxHp >=2000) return;
            castleMaxHp += 200f;
            castleManager.IncreaseMaxHp();
            SkillInstance(data, castleMaxHp);
        }

        [Header("5매치 가운데 유닛 추가 레벨증가")] 
        public bool match5Upgrade;

        protected internal void Match5Upgrade(Data data)
        {
            match5Upgrade = true;
            SkillInstance(data);
        }

        [Header("적 이동속도 감소 15%증가 (최대 45%)")]
        public int slowCount; 
        protected internal void SlowCount(Data data)
        {
            slowCount++;
            if (slowCount >= 4)
            {
                slowCount = 4;
            }

            var value = slowCount * 15f;
            SkillInstance(data, value);
        }

        [Header("보드 초기화 케릭터")]
        public int highLevelCharacterCount = 6; 
        public int selectedCount; 
        protected internal void NextCharacterUpgrade(Data data, int moveCharacterCount)
        {
            highLevelCharacterCount += moveCharacterCount;
            selectedCount += 1;
            SkillInstance(data, highLevelCharacterCount);
        }

        [Header("추가코인")] 
        public bool addGold;
        public int addGoldCount; 
        protected internal void AddGold(Data data)
        {
            addGold = true;
            SkillInstance(data, addGoldCount);
        }
        
        public void SaveEnforceData()
        {
            var data = new EnforceData
            {
                //Darkness3 Unit A
                dark3FifthAttackBoost = octopusThirdAttackBoost,
                dark3BleedAttack = octopusPoisonAttack,
                dark3PoisonDamageBoost = octopusBleedDamageBoost,
                dark3ShackledExplosion = octopusPoisonDamageBoost,
                dark3BleedDurationBoost = octopusPoisonDurationBoost,
                dark3DamageBoost = octopusDamageBoost,
                dark3RateBoost = dark3RateBoost,
                //Darkness Unit Ogre
                darkFifthAttackDamageBoost = ogreThirdAttackDamageBoost,
                darkAttackSpeedBoost = darkAttackSpeedBoost,
                darkAttackPowerBoost = darkAttackPowerBoost,
                darkKnockBackChance = ogreKnockBackChance,
                darkStatusAilmentDamageBoost = ogreStatusAilmentDamageBoost,
                darkRangeIncrease = ogreRangeIncrease,
                darkStatusAilmentSlowEffect = ogreStatusAilmentSlowEffect,
                //Water1 Unit DeathChiller
                waterFreeze = deathChillerFreeze,
                waterFreezeChance = deathChillerFreezeChance,
                waterSlowDurationBoost = waterSlowDurationBoost,
                waterFreezeDamageBoost = deathChillerFreezeDamageBoost,
                waterSlowCPowerBoost =  deathChillerSlowCPowerBoost,
                waterAttackRateBoost = waterAttackRateBoost,
                waterGlobalFreeze = deathChillerBackAttackBoost, 
                //Physical Unit Orc
                physicalAttackSpeedBoost = physicalAttackSpeedBoost,
                physicalSwordAddition = orcSwordAddition,
                physicalSwordScaleIncrease = orcSwordScaleIncrease,
                physicalRatePerAttack = orcRatePerAttack, 
                physicalBindBleed = orcBindBleed,
                physicalDamageBoost = physicalDamageBoost,
                physicalBleedDuration = orcBleedDuration,
                //Water2 Unit Fishman
                water2Freeze = fishmanFreeze,
                water2SlowPowerBoost = fishmanSlowPowerBoost,
                water2FreezeTimeBoost = fishmanFreezeTimeBoost,
                water2DamageBoost = water2DamageBoost,
                water2FreezeChanceBoost = fishmanFreezeChanceBoost,
                water2FreezeDamageBoost = fishmanFreezeDamageBoost,
                water2SlowTimeBoost = water2SlowTimeBoost,
                //Poison Unit Skeleton
                poisonPerHitEffect = skeletonPerHitEffect,
                poisonBleedingEnemyDamageBoost = skeletonBleedingEnemyDamageBoost,
                poisonDamagePerBoost = skeletonDamagePerBoost,
                poisonDamageBoost = skeletonDamageBoost,
                poisonDotDamageBoost = skeletonDotDamageBoost,
                poisonAttackSpeedIncrease = poisonAttackSpeedIncrease,
                poisonDurationBoost = skeletonDurationBoost,
                //Fire2 Unit Phoenix
                fire2FreezeDamageBoost = phoenixFreezeDamageBoost,
                fire2BurnDurationBoost = phoenixBurnDurationBoost,
                fire2ChangeProperty = phoenixChangeProperty,
                fire2DamageBoost =  fire2DamageBoost,
                fire2RangeBoost = phoenixRangeBoost,
                fire2RateBoost = phoenixRateBoost, 
                fire2BossDamageBoost = phoenixBossDamageBoost,  
        
                //Fire1 Unit Beholder
                fireBurnPerAttackEffect = beholderBurnPerAttackEffect,
                fireStackOverlap = beholderStackOverlap,
                fireProjectileBounceDamage = beholderProjectileBounceDamage,
                fireBurnedEnemyExplosion = beholderAttackDamageBoost,
                fireAttackSpeedBoost = fireAttackSpeedBoost,
                fireProjectileSpeedIncrease = beholderProjectileSpeedIncrease,
                fireProjectileBounceIncrease = beholderProjectileBounceIncrease,
        
                //Poison2 Unit Cobra
                poison2StunToChance = cobra2StunToChance,
                poison2RangeBoost = cobraRangeBoost,
                poison2DotDamageBoost = cobraDotDamageBoost,
                poison2StunTimeBoost = poison2StunTimeBoost,
                poison2SpawnPoisonArea = cobraDamageBoost,
                poison2RateBoost = cobraRateBoost,
                poison2PoolTimeBoost = cobraStunChanceBoost,
        
                //Physical2 Unit J
                physical2CastleCrushStatBoost = berserkerCastleCrushStatBoost,
                physical2FifthBoost = berserkerThirdBoost,
                physical2BleedTimeBoost = berserkerBleedTimeBoost,
                physical2PoisonDamageBoost = berserkerPoisonDamageBoost,       
                physical2RangeBoost = berserkerRangeBoost,
                physical2RateBoost = physical2RateBoost,
                physical2BossBoost = berserkerBossBoost,
        
                //Darkness Unit DarkElf
                dark2BackBoost = darkElfBackBoost,
                dark2DualAttack = darkElfDualAttack,
                dark2StatusDamageBoost = darkElfStatusDamageBoost,
                dark2ExplosionBoost = darkElfExplosionBoost,
                dark2DoubleAttack = darkElfDoubleAttack,
                dark2StatusPoison = darkElfStatusPoison,
                dark2SameEnemyBoost = darkElfSameEnemyBoost,
        
                // Common
                addRow = addRow,
                slowCount = slowCount,
                diagonalMovement = diagonalMovement,
                recoveryCastle = recoveryCastle,
                castleMaxHp = castleMaxHp,
                highLevelCharacterCount = highLevelCharacterCount,
                selectedCount = selectedCount,
                expPercentage = expPercentage,
                permanentIncreaseMovementCount = permanentIncreaseMovementCount,
                match5Upgrade = match5Upgrade,
                increaseAtkDamage = increaseAtkDamage,
                increaseAtkRate = increaseAtkRate,
                rewardMoveCount = rewardMoveCount,
                addGoldCount = addGoldCount,
                addGold = addGold,
                permanentGroupIndex = index, 

                skillInstances = _instantiatedSkills.Keys
                    .Select(key => new SkillInstanceData { type = key.Item1, Value = key.Item2 })
                    .ToList()
            };
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString("EnforceData", json);
        }

        public void LoadEnforceData()
        {
            if (!PlayerPrefs.HasKey("EnforceData")) return;
            var json = PlayerPrefs.GetString("EnforceData");
            var data = JsonUtility.FromJson<EnforceData>(json);

            // Divine Unit A
            octopusThirdAttackBoost = data.dark3FifthAttackBoost;
            octopusPoisonAttack = data.dark3BleedAttack;
            octopusBleedDamageBoost = data.dark3PoisonDamageBoost;
            octopusPoisonDamageBoost = data.dark3ShackledExplosion;
            octopusPoisonDurationBoost = data.dark3BleedDurationBoost;
            octopusDamageBoost = data.dark3DamageBoost;
            dark3RateBoost = data.dark3RateBoost;
            //Darkness Unit Ogre
            ogreThirdAttackDamageBoost = data.darkFifthAttackDamageBoost; 
            darkAttackSpeedBoost = data.darkAttackSpeedBoost;
            darkAttackPowerBoost = data.darkAttackPowerBoost;
            ogreKnockBackChance = data.darkKnockBackChance;
            ogreStatusAilmentDamageBoost = data.darkStatusAilmentDamageBoost;
            ogreRangeIncrease = data.darkRangeIncrease;
            ogreStatusAilmentSlowEffect = data.darkStatusAilmentSlowEffect;
            //Water1 Unit DeathChiller
            deathChillerFreeze = data.waterFreeze;
            deathChillerFreezeChance = data.waterFreezeChance;
            waterSlowDurationBoost = data.waterSlowDurationBoost;
            deathChillerFreezeDamageBoost = data.waterFreezeDamageBoost;
            deathChillerSlowCPowerBoost =  data.waterSlowCPowerBoost;
            waterAttackRateBoost = data.waterAttackRateBoost;
            deathChillerBackAttackBoost = data.waterGlobalFreeze;
            //Physical Unit Orc
            physicalAttackSpeedBoost = data.physicalAttackSpeedBoost;
            orcSwordAddition = data.physicalSwordAddition;
            orcSwordScaleIncrease = data.physicalSwordScaleIncrease;
            orcRatePerAttack = data.physicalRatePerAttack; 
            orcBindBleed = data.physicalBindBleed;
            physicalDamageBoost = data.physicalDamageBoost;
            orcBleedDuration = data.physicalBleedDuration;
            //Water2 Unit Fishman
            fishmanFreeze = data.water2Freeze;
            fishmanSlowPowerBoost = data.water2SlowPowerBoost;
            fishmanFreezeTimeBoost = data.water2FreezeTimeBoost;
            water2DamageBoost = data.water2DamageBoost;
            fishmanFreezeChanceBoost = data.water2FreezeChanceBoost;
            fishmanFreezeDamageBoost = data.water2FreezeDamageBoost;
            water2SlowTimeBoost = data.water2SlowTimeBoost;
            //Poison Unit Skeleton
            skeletonPerHitEffect = data.poisonPerHitEffect;
            skeletonBleedingEnemyDamageBoost = data.poisonBleedingEnemyDamageBoost;
            skeletonDamagePerBoost = data.poisonDamagePerBoost;
            skeletonDamageBoost = data.poisonDamageBoost;
            skeletonDotDamageBoost = data.poisonDotDamageBoost;
            poisonAttackSpeedIncrease = data.poisonAttackSpeedIncrease;
            skeletonDurationBoost = data.poisonDurationBoost;
            //Fire2 Unit Phoenix
            phoenixFreezeDamageBoost = data.fire2FreezeDamageBoost;
            phoenixBurnDurationBoost = data.fire2BurnDurationBoost;
            phoenixChangeProperty = data.fire2ChangeProperty;
            fire2DamageBoost =  data.fire2DamageBoost;
            phoenixRangeBoost = data.fire2RangeBoost;
            phoenixRateBoost = data.fire2RateBoost; 
            phoenixBossDamageBoost = data.fire2BossDamageBoost;  
            //Fire1 Unit Beholder
            beholderBurnPerAttackEffect = data.fireBurnPerAttackEffect;
            beholderStackOverlap = data.fireStackOverlap;
            beholderProjectileBounceDamage = data.fireProjectileBounceDamage;
            beholderAttackDamageBoost = data.fireBurnedEnemyExplosion;
            fireAttackSpeedBoost = data.fireAttackSpeedBoost;
            beholderProjectileSpeedIncrease = data.fireProjectileSpeedIncrease;                            
            beholderProjectileBounceIncrease = data.fireProjectileBounceIncrease;
            //Poison2 Unit Cobra
            cobra2StunToChance = data.poison2StunToChance;
            cobraRangeBoost = data.poison2RangeBoost;
            cobraDotDamageBoost = data.poison2DotDamageBoost;
            poison2StunTimeBoost = data.poison2StunTimeBoost;
            cobraDamageBoost = data.poison2SpawnPoisonArea;
            cobraRateBoost = data.poison2RateBoost;
            cobraStunChanceBoost = data.poison2PoolTimeBoost;
            //Physical2 Unit J
            berserkerCastleCrushStatBoost = data.physical2CastleCrushStatBoost;
            berserkerThirdBoost = data.physical2FifthBoost;
            berserkerBleedTimeBoost = data.physical2BleedTimeBoost;
            berserkerPoisonDamageBoost = data.physical2PoisonDamageBoost;       
            berserkerRangeBoost = data.physical2RangeBoost;
            physical2RateBoost = data.physical2RateBoost;
            berserkerBossBoost = data.physical2BossBoost;
            //Darkness Unit DarkElf
            darkElfBackBoost = data.dark2BackBoost;
            darkElfDualAttack = data.dark2DualAttack;
            darkElfStatusDamageBoost = data.dark2StatusDamageBoost;
            darkElfExplosionBoost = data.dark2ExplosionBoost;
            darkElfDoubleAttack = data.dark2DoubleAttack;              
            darkElfStatusPoison = data.dark2StatusPoison;
            darkElfSameEnemyBoost = data.dark2SameEnemyBoost;
            // Common
            addRow = data.addRow;
            slowCount = data.slowCount;
            diagonalMovement = data.diagonalMovement;
            recoveryCastle = data.recoveryCastle;
            castleMaxHp = data.castleMaxHp;
            highLevelCharacterCount = data.highLevelCharacterCount;
            selectedCount = data.selectedCount;
            expPercentage = data.expPercentage;
            permanentIncreaseMovementCount = data.permanentIncreaseMovementCount;
            match5Upgrade = data.match5Upgrade;
            increaseAtkDamage = data.increaseAtkDamage;
            increaseAtkRate = data.increaseAtkRate;
            rewardMoveCount = data.rewardMoveCount;
            addGoldCount = data.addGoldCount;
            addGold = data.addGold;
            index = new List<int>(data.permanentGroupIndex);

            foreach (var skillInfo in data.skillInstances)
            {
                var skillData = FindSkillDataByType(skillInfo.type);
                if (skillData != null)
                {
                    SkillInstance(skillData, skillInfo.Value);
                }
            }
        }          
        private static Data FindSkillDataByType(PowerTypeManager.Types type)
        {
            return PowerTypeManager.Instance.GreenList.FirstOrDefault(data => data.Type == type) ??
                   PowerTypeManager.Instance.BlueList.FirstOrDefault(data => data.Type == type) ??
                   PowerTypeManager.Instance.PurpleList.FirstOrDefault(data => data.Type == type);
        }
    }
}

