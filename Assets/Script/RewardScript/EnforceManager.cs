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
        public bool fireBurnedEnemyExplosion;
        public float fireAttackSpeedBoost;
        public bool fireProjectileSpeedIncrease;
        public bool fireProjectileBounceIncrease;

        //Poison2 Unit Cobra
        public bool poison2StunToChance;
        public bool poison2RangeBoost;
        public bool poison2DotDamageBoost;
        public float poison2StunTimeBoost;
        public bool poison2SpawnPoisonArea;
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
        [SerializeField] private Language language;
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private CountManager countManager;
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

        [Header("\n\nA 어둠: Blue\n")]
        // 완료
        [Header("Blue / 1Lv: 5회 공격마다 100% 추가데미지 (투사체 컬러 변경)")] 
        public bool dark3FifthAttackBoost;
        // 완료
        [Header("Purple / 3Lv: 적을 공격하면, 초당 공격력의 20% 데미지를 주는 출혈을 3초간 발생시킵니다.")]
        public bool dark3BleedAttack; 
        // 완료
        [Header("Green / 5Lv: 중독된 적 추가데미지 50%")] 
        public bool dark3PoisonDamageBoost;
        // 완료
        [Header("Blue / 7Lv: 적 제거시 주변 1칸 범위의 100% 폭발데미지 추가")] 
        public bool dark3ShackledExplosion;
        // 완료
        [Header("Green / 9Lv: 출혈지속시간 2초 증가")] 
        public bool dark3BleedDurationBoost;
        // 완료
        [Header("Purple / 11Lv: 공력력 19% 증가")] 
        public bool dark3DamageBoost;
        // 완료
        [Header("Blue / 13Lv: 공격속도 9% 증가 (최대 4회)")]
        public float dark3RateBoost; 
        protected internal void Dark3RateBoost()
        {   
            if (dark3RateBoost >= 0.36f) return;
            dark3RateBoost+= 0.09f;
        }


        [Header("\n\nB 어둠: Green\n")]
        // 완료
        [Header("Green / 1Lv: 5회 공격마다 100% 추가 데미지")] 
        public bool darkFifthAttackDamageBoost;
        // 완료
        [Header("Blue / 3Lv: 상태이상 적 공격시 1초 이동속도 20% 감소")] 
        public bool darkStatusAilmentSlowEffect;
        // 완료
        [Header("Purple / 5Lv: 사거리 1 증가")] 
        public bool darkRangeIncrease;
        // 완료
        [Header("Green / 7Lv: 공격력 4% 증가 (최대 6회)")]
        public float darkAttackPowerBoost; 
        protected internal void DarkAttackDamageBoost()
        {
            if (darkAttackPowerBoost >= 0.24f) return;
            darkAttackPowerBoost += 0.04f;
        }
        // 완료
        [Header("Purple / 9Lv: 상태이상 적 공격시 50% 확률로 50% 추가데미지")] 
        public bool darkStatusAilmentDamageBoost;
        // 완료
        [Header("Blue / 11: 공격속도 9% 증가 (최대 4회)")]
        public float darkAttackSpeedBoost;
        protected internal void DarkAttackSpeedBoost()
        {
            if(darkAttackSpeedBoost >= 0.36f) return;
            darkAttackSpeedBoost += 0.09f;
        }
        // 완료
        [Header("Blue / 13Lv: 10% 확률로 적 밀침 (0.5칸)")] 
        public bool darkKnockBackChance;
        

        [Header("\n\nC 물: Purple\n")]
        // 완료
        [Header("Blue / 1Lv: 15% 확률로 적을 1초간 빙결(이동불가)시킵니다.")]
        public bool waterFreeze;
        // 완료
        [Header("Blue / 3Lv: 빙결 확률이 10% 증가합니다.")]
        public bool waterFreezeChance;
        // 완료
        [Header("Green / 5Lv: 둔화 지속시간 0.2초 증가 (최대 5회 / 1초)")]
        public float waterSlowDurationBoost;
        protected internal void WaterSlowDurationBoost()
        {
            if (waterSlowDurationBoost >= 1f) return;
            waterSlowDurationBoost += 0.2f;
        }
        // 완료
        [Header("Purple / 7Lv: DeathChiller 유닛에게 빙결된 적은 받는 피해 15% 증가")]
        public bool waterFreezeDamageBoost;
        // 완료
        [Header("Blue / 9Lv: 둔화강도 15% 증가")] 
        public bool waterSlowCPowerBoost;
        // 완료
        [Header("Green / 11Lv: 공격속도 4% 증가 (최대 6회)")]
        public float waterAttackRateBoost;
        protected internal void WaterAttackRateBoost()
        {
            if ( waterAttackRateBoost >= 0.24f) return;
            waterAttackRateBoost += 0.06f;
        }
        // 완료
        [Header("Purple / 13Lv: 퍼즐위 모든 DeathChiller 유닛의 공격 횟수의 합이 100이면 될 때마다 눈보라를 일으켜 보스를 제외한 모든 적을 빙결")]
        public bool waterGlobalFreeze;



        [Header("\n\nD 물리: Green\n")]
        // 완료
        [Header("Purple / 1Lv: 검의 크기가 100% 증가")] 
        public bool physicalSwordScaleIncrease;
        // 완료
        [Header("Purple / 3Lv: 공격 시 검 한자루 추가")] 
        public bool physicalSwordAddition;
        // 완료
        [Header("Blue / 5Lv: 공격속도 9% 증가 (최대 4회)")]
        public float physicalAttackSpeedBoost; 
        protected internal void PhysicalAttackSpeedIncrease()
        {
            if (physicalAttackSpeedBoost >= 0.36f) return;
            physicalAttackSpeedBoost += 0.09f;
        }
        // 완료
        [Header("Blue / 7Lv: 해당 웨이브에서 유닛 3회 공격당 공속 1% 증가 (최대 60%)")]
        public bool physicalRatePerAttack;
        // 완료
        [Header("Blue / 9Lv: 속박에 걸린적을 공격하면 초당 20% 데미지의 3초간 출혈 발생")]
        public bool physicalBindBleed;
        // 완료
        [Header("Green / 11Lv: 공격력 3% 증가 (최대 6회)")]
        public float physicalDamageBoost;
        protected internal void PhysicalDamageBoost()
        {
            if (physicalDamageBoost >= 0.18f) return;
            physicalDamageBoost += 0.06f;
        }
        // 완료
        [Header("Green / 13Lv: 출혈 지속시간 2초 증가")]
        public bool physicalBleedDuration;


        [Header("\n\nE 물: Green\n")]
        // 완료
        [Header("Blue / 1Lv: 타격시 10% 확률로 적을 1초간 빙결 (이동불가)")]
        public bool water2Freeze;
        // 완료
        [Header("Purple / 3Lv: 둔화강도가 10% 증가합니다.")]
        public bool water2SlowPowerBoost;
        // 완료
        [Header("Blue / 5Lv: 빙결지속시간 0.5초 증가")] 
        public bool water2FreezeTimeBoost;
        // 완료
        [Header("Green / 7Lv: 공격력 4% 증가 (최대 6회)")]
        public float water2DamageBoost;
        protected internal void Water2DamageBoost()
        {
            if (water2DamageBoost >= 0.24f) return;
            water2DamageBoost += 0.04f;
        }
        // 완료
        [Header("Purple / 9Lv: 빙결확률 10% 증가")] 
        public bool water2FreezeChanceBoost;
        // 완료 
        [Header("Blue / 11Lv: 빙결당한적은 받는 피해가 15% 증가")]
        public bool water2FreezeDamageBoost;
        // 완료
        [Header("Green / 13Lv: 둔화지속시간 0.1초 증가 (최대 0.5초)")]
        public float water2SlowTimeBoost;
        protected internal void Water2SlowTimeBoost()
        {
            if (water2SlowTimeBoost >= 0.5f) return;
            water2SlowTimeBoost += 0.1f;
        }

        [Header("\n\nF 독: Green\n")]
        // 완료
        [Header("Purple / 1Lv: 초당 20% 데미지를 가하는 중독을 3초간 발생")] 
        public bool poisonPerHitEffect;
        // 완료
        [Header("Green / 3Lv: 출혈중인 적 공격시 80% 데미지 증가")] 
        public bool poisonBleedingEnemyDamageBoost;
        // 완료
        [Header("Blue / 5Lv: 유닛 C가 5회 공격마다 공격력 1% 증가, 웨이브마다 초기화 (최대 60%)")]
        public bool poisonDamagePerBoost;
        // 완료
        [Header("Purple / 7Lv: 공격력 16% 증가")] 
        public bool poisonDamageBoost;
        // 완료
        [Header("Blue / 9Lv: 중독 피해 10% 증가")] 
        public bool poisonDotDamageBoost;
        // 완료
        [Header("Green / 11Lv: 공격속도 4% 증가 (최대 6회)")]
        public float poisonAttackSpeedIncrease; 
        protected internal void PoisonAttackSpeedIncrease()
        {
            if(poisonAttackSpeedIncrease >= 0.24f) return;
            poisonAttackSpeedIncrease += 0.04f;
        }
        // 완료
        [Header("Blue / 13Lv: 중족 지속시간 2초 증가")] 
        public bool poisonDurationBoost;



        [Header("\n\nG 불: Blue\n")] 
        // 완료
        [Header("Purple / 1Lv: 빙결된 적에게 가하는 추가데미지 200%")]
        public bool fire2FreezeDamageBoost;
        // 완료
        [Header("Green / 3Lv: 화상지속시간 2초 증가")] 
        public bool fire2BurnDurationBoost;
        // 완료
        [Header("Blue / 5Lv: 화상 비활성화 데미지 150% 증가")]
        public bool fire2ChangeProperty;
        // 완료
        [Header("Green / 7Lv: 공격력 4% 증가 (최대 6회)")]
        public float fire2DamageBoost;
        protected internal void Fire2DamageBoost()
        {
            if (fire2DamageBoost >= 0.24f) return;
            fire2DamageBoost += 0.04f;
        }
        // 완료
        [Header("Blue / 7Lv: 사거리가 1 증가합니다. ")] 
        public bool fire2RangeBoost;
        // 완료
        [Header("Purple / 11Lv: 공격속도 15% 증가")] 
        public bool fire2RateBoost;
        // 완료
        [Header("Blue / 13Lv: 보스 데미지 30% 증가")] 
        public bool fire2BossDamageBoost;


        [Header("\n\nH 불: Blue\n")]
        // 완료
        [Header("Purple/ 1Lv: 적을 공격하면 5초간 화상 초당 10% 데미지")] 
        public bool fireBurnPerAttackEffect;
        // 완료
        [Header("Blue/ 3Lv: 화상 중첩수 3회 증기")] 
        public bool fireStackOverlap;
        // 완료
        [Header("Purple/ 5Lv: 적 적중시 가장 가까운적에게 투사체 튕김")] 
        public bool fireProjectileBounceDamage;
        // 완료
        [Header("Blue/ 7Lv: 화상에 걸린 적 제거시 주변 1칸 범위의 200% 폭발데미지 추가")] 
        public bool fireBurnedEnemyExplosion;
        // 완료
        [Header("Green/ 9Lv: 공격속도 4% 증가 (최대 6회)")]
        public float fireAttackSpeedBoost; 
        protected internal void FireAttackSpeedBoost()
        {
            if (fireAttackSpeedBoost >= 0.24f) return;
            fireAttackSpeedBoost += 0.06f;
        }
        // 완료
        [Header("Green / 11Lv: 투사체 속도가 100% 증가, 반드시 명중")] 
        public bool fireProjectileSpeedIncrease;
        // 완료
        [Header("Purple/ 13Lv: 투사체가 튕기는 횟수 증가")] 
        public bool fireProjectileBounceIncrease;



        [Header("\n\nI 독: Blue\n")] 
        // 완료
        [Header("Purple / 1Lv: 타격시 50% 확률로 적을 0.4초간 기절 시킵니다.")]
        public bool poison2StunToChance;
        // 완료
        [Header("Purple / 3Lv: 사거리 1칸 증가")] 
        public bool poison2RangeBoost;
        // 완료
        [Header("Blue / 5Lv: 중독 데미지가 10% 증가")]
        public bool poison2DotDamageBoost;
        // 완료
        [Header("Green / 7Lv: 기절 시간 0.1초 증가 (최대 5회 / 0.5초)")]
        public float poison2StunTimeBoost;
        protected internal void Poison2StunTimeBoost()
        {
            poison2StunTimeBoost += 0.1f;
        }
        // 완료
        [Header("Blue / 9Lv: 중독 된 적이 죽으면, 그 자리에 2초간 독 웅덩이가 초당 200% 데미지를 입힘")]
        public bool poison2SpawnPoisonArea;
        // 완료
        [Header("Purple / 11Lv: 공격속도 20% 증가")] 
        public bool poison2RateBoost;
        // 완료
        [Header("Blue / 13Lv: 독 웅덩이 지속시간 1초 증가")]
        public bool poison2PoolTimeBoost;


        [Header("\n\nJ 물리: Blue\n")] 
        // 완료
        [Header("Purple / 1Lv: 성이 데미지를 받을 경우 해당 웨이브동안 사거리가 1 증가, 데미지 30% 증가")]
        public bool physical2CastleCrushStatBoost;
        // 완료 
        [Header("Purple / 3Lv: 5회 공격마다 공격력의 200% 데미지 투사체 전방 발사")]
        public bool physical2FifthBoost;
        // 완료
        [Header("Green / 5Lv: 출혈 지속시간 2초 증가")] 
        public bool physical2BleedTimeBoost;
        // 완료
        [Header("Blue / 7Lv: 중독상태의 적 공격시 데미지 60% 증가")]
        public bool physical2PoisonDamageBoost;
        // 완료
        [Header("Purple / 9Lv: 사거리가 1 증가합니다.")]
        public bool physical2RangeBoost;
        // 완료
        [Header("Blue / 11Lv: 공격속도 9% 증가 (최대 4회)")]
        public float physical2RateBoost;
        protected internal void Physical2RateBoost()
        {
            physical2RateBoost += 0.09f;
        }
        // 완료
        [Header("Blue / 13Lv: 보스 데미지 30% 증가")] 
        public bool physical2BossBoost;



        [Header("\n\nK 어둠: Purple\n")] 
        // 완료
        [Header("Purple / 1Lv: 적의 뒤를 공격하면, 데미지 30% 증가")]
        public bool dark2BackBoost;
        // 완료
        [Header("Purple / 3Lv: 앞 뒤 동시공격")]
        public bool dark2DualAttack;
        // 완료
        [Header("Blue / 5Lv: 적에게 걸린 상태이상 갯수 마다 15% 데미지 증가 (최대 5개)")]
        public bool dark2StatusDamageBoost;
        // 완료
        [Header("Blue / 7Lv: 상태이상적을 처치하면 주변 2칸을 100% 추가데미지")]
        public bool dark2ExplosionBoost;
        // 완료
        [Header("Purple / 9Lv: 발사하는 투사체 갯수 1개 증가")]
        public bool dark2DoubleAttack;
        // 완료
        [Header("Green / 11Lv: 적이 상태이상에 걸리지 않았을때, 초당 공격력의 20%의 중독을 3초간 발생시킵니다.")]
        public bool dark2StatusPoison;
        // 완료
        [Header("Blue / 13Lv: 동일한 적을 타격할때마다 데미지가 5% 증가 (최대 10회)")]
        public bool dark2SameEnemyBoost;


        public Dictionary<int, bool> GetActivatedSkills(CharacterBase.UnitGroups unitGroup)
        {
            var activatedSkills = new Dictionary<int, bool>();
        
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.Octopus:
                    activatedSkills[1] = dark3FifthAttackBoost;
                    activatedSkills[3] = dark3BleedAttack;
                    activatedSkills[5] = dark3PoisonDamageBoost;
                    activatedSkills[7] = dark3ShackledExplosion;
                    activatedSkills[9] = dark3BleedDurationBoost;
                    activatedSkills[11] = dark3DamageBoost;
                    activatedSkills[13] = dark3RateBoost > 0;
                    break;
                case CharacterBase.UnitGroups.Ogre:
                    activatedSkills[1] = darkFifthAttackDamageBoost;
                    activatedSkills[3] = darkStatusAilmentSlowEffect;
                    activatedSkills[5] = darkRangeIncrease;
                    activatedSkills[7] = darkAttackPowerBoost > 0;
                    activatedSkills[9] = darkStatusAilmentDamageBoost;
                    activatedSkills[11] = darkAttackSpeedBoost > 0; 
                    activatedSkills[13] = darkKnockBackChance;
                    break;
                case CharacterBase.UnitGroups.DeathChiller:
                    activatedSkills[1] = waterFreeze;
                    activatedSkills[3] = waterFreezeChance;
                    activatedSkills[5] = waterSlowDurationBoost > 0;
                    activatedSkills[7] = waterFreezeDamageBoost;
                    activatedSkills[9] = waterSlowCPowerBoost;
                    activatedSkills[11] = waterAttackRateBoost > 0; 
                    activatedSkills[13] = waterGlobalFreeze;
                    break;
                case CharacterBase.UnitGroups.Orc:
                    activatedSkills[1] = physicalSwordScaleIncrease;
                    activatedSkills[3] = physicalSwordAddition;
                    activatedSkills[5] = physicalAttackSpeedBoost > 0;
                    activatedSkills[7] = physicalRatePerAttack;
                    activatedSkills[9] = physicalBindBleed;
                    activatedSkills[11] = physicalDamageBoost > 0; 
                    activatedSkills[13] = physicalBleedDuration;
                    break;
                case CharacterBase.UnitGroups.Fishman:
                    activatedSkills[1] = water2Freeze;
                    activatedSkills[3] = water2SlowPowerBoost;
                    activatedSkills[5] = water2FreezeTimeBoost;
                    activatedSkills[7] = water2DamageBoost > 0;
                    activatedSkills[9] = water2FreezeChanceBoost;
                    activatedSkills[11] =  water2FreezeDamageBoost;
                    activatedSkills[13] = water2SlowTimeBoost > 0;
                    break;
                case CharacterBase.UnitGroups.Skeleton:
                    activatedSkills[1] = poisonPerHitEffect;
                    activatedSkills[3] = poisonBleedingEnemyDamageBoost;
                    activatedSkills[5] = poisonDamagePerBoost;
                    activatedSkills[7] = poisonDamageBoost;
                    activatedSkills[9] = poisonDotDamageBoost;
                    activatedSkills[11] = poisonAttackSpeedIncrease > 0; 
                    activatedSkills[13] = poisonDurationBoost;
                    break;
                case CharacterBase.UnitGroups.Phoenix:
                    activatedSkills[1] = fire2FreezeDamageBoost;
                    activatedSkills[3] = fire2BurnDurationBoost;
                    activatedSkills[5] = fire2ChangeProperty;
                    activatedSkills[7] = fire2DamageBoost > 0;
                    activatedSkills[9] = fire2RangeBoost;
                    activatedSkills[11] = fire2RateBoost;
                    activatedSkills[13] = fire2BossDamageBoost;
                    break;
                case CharacterBase.UnitGroups.Beholder:
                    activatedSkills[1] = fireBurnPerAttackEffect;
                    activatedSkills[3] = fireStackOverlap;
                    activatedSkills[5] = fireProjectileBounceDamage;
                    activatedSkills[7] = fireBurnedEnemyExplosion;
                    activatedSkills[9] = fireAttackSpeedBoost > 0;
                    activatedSkills[11] = fireProjectileSpeedIncrease;
                    activatedSkills[13] = fireProjectileBounceIncrease;
                    break;
                case CharacterBase.UnitGroups.Cobra:
                    activatedSkills[1] = poison2StunToChance;
                    activatedSkills[3] = poison2RangeBoost;
                    activatedSkills[5] = poison2DotDamageBoost;
                    activatedSkills[7] = poison2StunTimeBoost > 0;
                    activatedSkills[9] = poison2SpawnPoisonArea;
                    activatedSkills[11] = poison2RateBoost;
                    activatedSkills[13] = poison2PoolTimeBoost;
                    break;
                case CharacterBase.UnitGroups.Berserker:
                    activatedSkills[1] = physical2CastleCrushStatBoost;
                    activatedSkills[3] = physical2FifthBoost;
                    activatedSkills[5] = physical2BleedTimeBoost;
                    activatedSkills[7] = physical2PoisonDamageBoost;
                    activatedSkills[9] = physical2RangeBoost;
                    activatedSkills[11] = physical2RateBoost > 0;
                    activatedSkills[13] = physical2BossBoost;
                    break;
                case CharacterBase.UnitGroups.DarkElf:
                    activatedSkills[1] = dark2BackBoost;
                    activatedSkills[3] = dark2DualAttack;
                    activatedSkills[5] = dark2StatusDamageBoost;
                    activatedSkills[7] = dark2ExplosionBoost;
                    activatedSkills[9] = dark2DoubleAttack;
                    activatedSkills[11] = dark2StatusPoison;
                    activatedSkills[13] = dark2SameEnemyBoost;
                    break;
            }
            return activatedSkills;
        }

        private float _property;
        private readonly Dictionary<(PowerTypeManager.Types Type, int Value), PauseSkillObjectScript> _instantiatedSkills = 
            new Dictionary<(PowerTypeManager.Types Type, int Value), PauseSkillObjectScript>();
        private PauseSkillObjectScript _skill;
       
        private string GetGroupNameFromValue(int value)
        {
            var unitName = value switch
            {
                0 => characterList[0].name,
                1 => characterList[1].name,
                2 => characterList[2].name,
                3 => characterList[3].name,
            };
            return unitName;
        }
        
        private void SkillInstance(Data data, float? value = null)
        {
            _property = value.GetValueOrDefault(); 
            var translationKey = data.Type.ToString();
            var powerTextTranslation = language.GetTranslation(translationKey);
            var finalPowerText = powerTextTranslation;
            var placeholderValues = new Dictionary<PowerTypeManager.Types, Dictionary<string, float>>
            {
                { PowerTypeManager.Types.Slow , new Dictionary<string, float>{{"{15*EnforceManager.Instance.slowCount}", _property}}},
                { PowerTypeManager.Types.StepLimit , new Dictionary<string, float>{{"powerUp.Property[0]", 1}}},
                { PowerTypeManager.Types.GroupDamage , new Dictionary<string, float>{{"{p}", _property}}},
                { PowerTypeManager.Types.GroupAtkSpeed , new Dictionary<string, float>{{"{p}", _property}}},
                { PowerTypeManager.Types.Exp , new Dictionary<string, float>{{"{EnforceManager.Instance.expPercentage}", _property}}},
                { PowerTypeManager.Types.CastleMaxHp, new Dictionary<string, float>{{"{p}", _property}}},
            };

            if (placeholderValues.TryGetValue(data.Type, out var placeholderValue))
            {
                finalPowerText = placeholderValue.Aggregate(finalPowerText, (current, placeholder) => current.Replace(placeholder.Key, placeholder.Value.ToString(CultureInfo.CurrentCulture)));
            }

            if (data.Type == PowerTypeManager.Types.NextStage)
            {
                finalPowerText = finalPowerText.Replace("{p}", _property.ToString(CultureInfo.CurrentCulture));
                finalPowerText.Replace("{EnforceManager.Instance.highLevelCharacterCount}", highLevelCharacterCount.ToString(CultureInfo.CurrentCulture));
            }
            var finalTranslation = finalPowerText.Replace("||", "\n");
         
            
            if (data.Type == PowerTypeManager.Types.LevelUpPattern)
            {
                var groupName = GetGroupNameFromValue((int)_property);
                finalTranslation = finalPowerText.Replace("{_groupName}", groupName);
                _skill.skillIcon.sprite = characterList[(int)_property].GetSpriteForLevel(characterList[(int)_property].unitPeaceLevel);
                _skill.skillBackGround.sprite = characterList[(int)_property].UnitGrade switch
                {
                    CharacterBase.UnitGrades.Green => PowerTypeManager.Instance.green,
                    CharacterBase.UnitGrades.Blue => PowerTypeManager.Instance.blue,
                    CharacterBase.UnitGrades.Purple => PowerTypeManager.Instance.purple,
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
        public static void RandomCharacterLevelUp(int characterCount)
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
                dark3FifthAttackBoost = dark3FifthAttackBoost,
                dark3BleedAttack = dark3BleedAttack,
                dark3PoisonDamageBoost = dark3PoisonDamageBoost,
                dark3ShackledExplosion = dark3ShackledExplosion,
                dark3BleedDurationBoost = dark3BleedDurationBoost,
                dark3DamageBoost = dark3DamageBoost,
                dark3RateBoost = dark3RateBoost,
                //Darkness Unit Ogre
                darkFifthAttackDamageBoost = darkFifthAttackDamageBoost,
                darkAttackSpeedBoost = darkAttackSpeedBoost,
                darkAttackPowerBoost = darkAttackPowerBoost,
                darkKnockBackChance = darkKnockBackChance,
                darkStatusAilmentDamageBoost = darkStatusAilmentDamageBoost,
                darkRangeIncrease = darkRangeIncrease,
                darkStatusAilmentSlowEffect = darkStatusAilmentSlowEffect,
                //Water1 Unit DeathChiller
                waterFreeze = waterFreeze,
                waterFreezeChance = waterFreezeChance,
                waterSlowDurationBoost = waterSlowDurationBoost,
                waterFreezeDamageBoost = waterFreezeDamageBoost,
                waterSlowCPowerBoost =  waterSlowCPowerBoost,
                waterAttackRateBoost = waterAttackRateBoost,
                waterGlobalFreeze = waterGlobalFreeze, 
                //Physical Unit Orc
                physicalAttackSpeedBoost = physicalAttackSpeedBoost,
                physicalSwordAddition = physicalSwordAddition,
                physicalSwordScaleIncrease = physicalSwordScaleIncrease,
                physicalRatePerAttack = physicalRatePerAttack, 
                physicalBindBleed = physicalBindBleed,
                physicalDamageBoost = physicalDamageBoost,
                physicalBleedDuration = physicalBleedDuration,
                //Water2 Unit Fishman
                water2Freeze = water2Freeze,
                water2SlowPowerBoost = water2SlowPowerBoost,
                water2FreezeTimeBoost = water2FreezeTimeBoost,
                water2DamageBoost = water2DamageBoost,
                water2FreezeChanceBoost = water2FreezeChanceBoost,
                water2FreezeDamageBoost = water2FreezeDamageBoost,
                water2SlowTimeBoost = water2SlowTimeBoost,
                //Poison Unit Skeleton
                poisonPerHitEffect = poisonPerHitEffect,
                poisonBleedingEnemyDamageBoost = poisonBleedingEnemyDamageBoost,
                poisonDamagePerBoost = poisonDamagePerBoost,
                poisonDamageBoost = poisonDamageBoost,
                poisonDotDamageBoost = poisonDotDamageBoost,
                poisonAttackSpeedIncrease = poisonAttackSpeedIncrease,
                poisonDurationBoost = poisonDurationBoost,
                //Fire2 Unit Phoenix
                fire2FreezeDamageBoost = fire2FreezeDamageBoost,
                fire2BurnDurationBoost = fire2BurnDurationBoost,
                fire2ChangeProperty = fire2ChangeProperty,
                fire2DamageBoost =  fire2DamageBoost,
                fire2RangeBoost = fire2RangeBoost,
                fire2RateBoost = fire2RateBoost, 
                fire2BossDamageBoost = fire2BossDamageBoost,  
        
                //Fire1 Unit Beholder
                fireBurnPerAttackEffect = fireBurnPerAttackEffect,
                fireStackOverlap = fireStackOverlap,
                fireProjectileBounceDamage = fireProjectileBounceDamage,
                fireBurnedEnemyExplosion = fireBurnedEnemyExplosion,
                fireAttackSpeedBoost = fireAttackSpeedBoost,
                fireProjectileSpeedIncrease = fireProjectileSpeedIncrease,
                fireProjectileBounceIncrease = fireProjectileBounceIncrease,
        
                //Poison2 Unit Cobra
                poison2StunToChance = poison2StunToChance,
                poison2RangeBoost = poison2RangeBoost,
                poison2DotDamageBoost = poison2DotDamageBoost,
                poison2StunTimeBoost = poison2StunTimeBoost,
                poison2SpawnPoisonArea = poison2SpawnPoisonArea,
                poison2RateBoost = poison2RateBoost,
                poison2PoolTimeBoost = poison2PoolTimeBoost,
        
                //Physical2 Unit J
                physical2CastleCrushStatBoost = physical2CastleCrushStatBoost,
                physical2FifthBoost = physical2FifthBoost,
                physical2BleedTimeBoost = physical2BleedTimeBoost,
                physical2PoisonDamageBoost = physical2PoisonDamageBoost,       
                physical2RangeBoost = physical2RangeBoost,
                physical2RateBoost = physical2RateBoost,
                physical2BossBoost = physical2BossBoost,
        
                //Darkness Unit DarkElf
                dark2BackBoost = dark2BackBoost,
                dark2DualAttack = dark2DualAttack,
                dark2StatusDamageBoost = dark2StatusDamageBoost,
                dark2ExplosionBoost = dark2ExplosionBoost,
                dark2DoubleAttack = dark2DoubleAttack,
                dark2StatusPoison = dark2StatusPoison,
                dark2SameEnemyBoost = dark2SameEnemyBoost,
        
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
            dark3FifthAttackBoost = data.dark3FifthAttackBoost;
            dark3BleedAttack = data.dark3BleedAttack;
            dark3PoisonDamageBoost = data.dark3PoisonDamageBoost;
            dark3ShackledExplosion = data.dark3ShackledExplosion;
            dark3BleedDurationBoost = data.dark3BleedDurationBoost;
            dark3DamageBoost = data.dark3DamageBoost;
            dark3RateBoost = data.dark3RateBoost;
            //Darkness Unit Ogre
            darkFifthAttackDamageBoost = data.darkFifthAttackDamageBoost; 
            darkAttackSpeedBoost = data.darkAttackSpeedBoost;
            darkAttackPowerBoost = data.darkAttackPowerBoost;
            darkKnockBackChance = data.darkKnockBackChance;
            darkStatusAilmentDamageBoost = data.darkStatusAilmentDamageBoost;
            darkRangeIncrease = data.darkRangeIncrease;
            darkStatusAilmentSlowEffect = data.darkStatusAilmentSlowEffect;
            //Water1 Unit DeathChiller
            waterFreeze = data.waterFreeze;
            waterFreezeChance = data.waterFreezeChance;
            waterSlowDurationBoost = data.waterSlowDurationBoost;
            waterFreezeDamageBoost = data.waterFreezeDamageBoost;
            waterSlowCPowerBoost =  data.waterSlowCPowerBoost;
            waterAttackRateBoost = data.waterAttackRateBoost;
            waterGlobalFreeze = data.waterGlobalFreeze;
            //Physical Unit Orc
            physicalAttackSpeedBoost = data.physicalAttackSpeedBoost;
            physicalSwordAddition = data.physicalSwordAddition;
            physicalSwordScaleIncrease = data.physicalSwordScaleIncrease;
            physicalRatePerAttack = data.physicalRatePerAttack; 
            physicalBindBleed = data.physicalBindBleed;
            physicalDamageBoost = data.physicalDamageBoost;
            physicalBleedDuration = data.physicalBleedDuration;
            //Water2 Unit Fishman
            water2Freeze = data.water2Freeze;
            water2SlowPowerBoost = data.water2SlowPowerBoost;
            water2FreezeTimeBoost = data.water2FreezeTimeBoost;
            water2DamageBoost = data.water2DamageBoost;
            water2FreezeChanceBoost = data.water2FreezeChanceBoost;
            water2FreezeDamageBoost = data.water2FreezeDamageBoost;
            water2SlowTimeBoost = data.water2SlowTimeBoost;
            //Poison Unit Skeleton
            poisonPerHitEffect = data.poisonPerHitEffect;
            poisonBleedingEnemyDamageBoost = data.poisonBleedingEnemyDamageBoost;
            poisonDamagePerBoost = data.poisonDamagePerBoost;
            poisonDamageBoost = data.poisonDamageBoost;
            poisonDotDamageBoost = data.poisonDotDamageBoost;
            poisonAttackSpeedIncrease = data.poisonAttackSpeedIncrease;
            poisonDurationBoost = data.poisonDurationBoost;
            //Fire2 Unit Phoenix
            fire2FreezeDamageBoost = data.fire2FreezeDamageBoost;
            fire2BurnDurationBoost = data.fire2BurnDurationBoost;
            fire2ChangeProperty = data.fire2ChangeProperty;
            fire2DamageBoost =  data.fire2DamageBoost;
            fire2RangeBoost = data.fire2RangeBoost;
            fire2RateBoost = data.fire2RateBoost; 
            fire2BossDamageBoost = data.fire2BossDamageBoost;  
            //Fire1 Unit Beholder
            fireBurnPerAttackEffect = data.fireBurnPerAttackEffect;
            fireStackOverlap = data.fireStackOverlap;
            fireProjectileBounceDamage = data.fireProjectileBounceDamage;
            fireBurnedEnemyExplosion = data.fireBurnedEnemyExplosion;
            fireAttackSpeedBoost = data.fireAttackSpeedBoost;
            fireProjectileSpeedIncrease = data.fireProjectileSpeedIncrease;                            
            fireProjectileBounceIncrease = data.fireProjectileBounceIncrease;
            //Poison2 Unit Cobra
            poison2StunToChance = data.poison2StunToChance;
            poison2RangeBoost = data.poison2RangeBoost;
            poison2DotDamageBoost = data.poison2DotDamageBoost;
            poison2StunTimeBoost = data.poison2StunTimeBoost;
            poison2SpawnPoisonArea = data.poison2SpawnPoisonArea;
            poison2RateBoost = data.poison2RateBoost;
            poison2PoolTimeBoost = data.poison2PoolTimeBoost;
            //Physical2 Unit J
            physical2CastleCrushStatBoost = data.physical2CastleCrushStatBoost;
            physical2FifthBoost = data.physical2FifthBoost;
            physical2BleedTimeBoost = data.physical2BleedTimeBoost;
            physical2PoisonDamageBoost = data.physical2PoisonDamageBoost;       
            physical2RangeBoost = data.physical2RangeBoost;
            physical2RateBoost = data.physical2RateBoost;
            physical2BossBoost = data.physical2BossBoost;
            //Darkness Unit DarkElf
            dark2BackBoost = data.dark2BackBoost;
            dark2DualAttack = data.dark2DualAttack;
            dark2StatusDamageBoost = data.dark2StatusDamageBoost;
            dark2ExplosionBoost = data.dark2ExplosionBoost;
            dark2DoubleAttack = data.dark2DoubleAttack;              
            dark2StatusPoison = data.dark2StatusPoison;
            dark2SameEnemyBoost = data.dark2SameEnemyBoost;
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

