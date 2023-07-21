using System;
using System.Collections.Generic;
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
    public class EnforceData
    {
        //Divine Unit A
        public bool divinePoisonDamageBoost;
        public float divineBindDurationBoost;
        public bool divineShackledExplosion;
        public bool divineFifthAttackBoost;
        public int divineAttackBoost;
        public bool divineBindChanceBoost;
        public bool divineDualAttack;
        public bool divineProjectilePierce;

        //Darkness Unit B
        public bool darkTenthAttackDamageBoost;
        public int darkAttackSpeedBoost;
        public int darkAttackPowerBoost;
        public bool darkStatusAilmentDamageChance;
        public bool darkKnockBackChance;
        public bool darkStatusAilmentDamageBoost;
        public bool darkRangeIncrease;
        public bool darkStatusAilmentSlowEffect;

        //Water1 Unit C
        public int waterAttackSpeedBoost;
        public bool waterAllyDamageBoost;
        public bool waterProjectileIncrease;
        public int waterAttackBoost;
        public bool waterSlowEnemyDamageBoost;
        public bool waterGlobalSlowEffect;
        public bool waterSlowEnemyStunChance;
        public bool waterDamageIncreaseDebuff;

        //Physical Unit D
        public int physicalAttackSpeedBoost;
        public bool physicalDamage35Boost;
        public int physicalDamage6Boost;
        public bool physicalBleedingChance;
        public bool physicalSwordAddition;
        public bool physicalSlowEnemyDamageBoost;
        public bool physicalSwordScaleIncrease;
        public bool physicalDamage18Boost;

        //Water2 Unit E
        public int water2DebuffDurationIncrease;
        public int water2AttackSpeedIncrease;
        public bool water2StunChanceAgainstBleeding;
        public bool water2IceSpikeProjectile;
        public int water2AttackPowerIncrease;
        public bool water2ProjectileSpeedIncrease;
        public bool water2DebuffStrengthIncrease;
        public bool water2AttackSpeedBuffToAdjacentAllies;

        //Poison Unit F
        public int poisonAttackSpeedIncrease;
        public int poisonMaxStackIncrease;
        public int poisonDamageAttackPowerIncrease;
        public bool poisonProjectileIncrease;
        public bool poisonRangeIncrease;
        public bool poisonBleedingEnemyDamageBoost;
        public bool poisonEnemyInstantKill;
        public bool poisonPerHitEffect;

        //Fire2 Unit G
        public bool fire2PoisonDamageIncrease;
        public int fire2AttackSpeedIncrease;
        public bool fire2BurnStackIncrease;
        public int fire2AttackPowerIncrease;
        public bool fire2StunChance;
        public bool fire2SwordSizeIncrease;
        public int fire2BurningDamageBoost;
        public bool fire2NoBurnDamageIncrease;
        
        //Fire1 Unit H
        public int fireImageOverlapIncrease;
        public int fireAttackSpeedBoost;
        public bool fireSlowEnemyDamageBoost;
        public bool fireProjectileSpeedIncrease;
        public bool fireBurnedEnemyExplosion;
        public bool fireProjectileBounceDamage;
        public bool fireBurnPerAttackEffect;
        public bool fireProjectileBounceIncrease;
      
        //common
        public int addRowCount;
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
        public bool addGold;
        public int addGoldCount;
        public List<int> permanentGroupIndex;

    }
    public class EnforceManager : MonoBehaviour
    {
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private CharacterPool characterPool;
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
        }
        private void Start()
        {
            var selectUnitList = SelectedUnitHolder.Instance.selectedUnit;
            foreach (var unit in selectUnitList)
            {
                characterList.Add(unit);
            }
        }

        [Header("\n\nA 신성: Blue\n")]
        // 완료
        [Header("Green / Default: 중독된 적 추가데미지 25%")] 
        public bool divinePoisonDamageBoost;
        // 완료
        [Header("Green / 7Lv: 속박지속시간 0.1초씩 증가 (최대 0.5초 / 5회)")] 
        public float divineBindDurationBoost; 
        protected internal void DivineBindDurationIncrease()
        {
            if (divineBindDurationBoost >= 0.5f) return;
            divineBindDurationBoost += 0.1f;
        }
        // 완료
        [Header("Green / 3Lv: 적 제거시 주변 1칸 범위의 100% 폭발데미지 추가")] 
        public bool divineShackledExplosion;
        // 완료
        [Header("Blue / Default: 5회 공격마다 200% 추가데미지 (투사체 컬러 변경)")] 
        public bool divineFifthAttackBoost;
        // 완료
        [Header("Blue / 13Lv: 공격력 12% 증가 (최대 4회)")] 
        public int divineAttackBoost; 
        protected internal void DivineAttackDamageIncrease()
        {   
            if (divineAttackBoost >= 4) return;
            divineAttackBoost++;
        }
        // 완료
        [Header("Purple / Lv5: 속박확률 20% 증가 (20% > 40%)")] 
        public bool divineBindChanceBoost;
        // 완료
        [Header("Purple / 11Lv: 백어텍 가능")]
        public bool divineDualAttack;
        // 완료
        [Header("Purple / 9Lv: 관통 가능 (1회)")] 
        public bool divineProjectilePierce;
        

        [Header("\n\nB 어둠: Green\n")]
        // 완료 
        [Header("Green / 5Lv: 10회 공격마다 300% 추가 데미지 (투사체 컬러 변경)")] 
        public bool darkTenthAttackDamageBoost;
        // 완료
        [Header("Green / Default: 공격속도 6% 증가 (최대 4회)")] 
        public int darkAttackSpeedBoost;
        protected internal void DarkAttackSpeedIncrease()
        {
            if(darkAttackSpeedBoost >= 4) return;
            darkAttackSpeedBoost++;
        }
        // 완료
        [Header("Green / 9Lv: 공격력 6% 증가 (최대 4회)")] 
        public int darkAttackPowerBoost; 
        protected internal void DarkAttackDamageIncrease()
        {
            if (darkAttackPowerBoost >= 4) return;
            darkAttackPowerBoost++;
        }
        // 완료
        [Header("Blue / 11Lv: 상태이상 적 공격시 10% 확률로 500% 추가데미지")] 
        public bool darkStatusAilmentDamageChance;
        // 완료
        [Header("Blue / 3Lv: 10% 확률로 적 밀침 (1칸)")] 
        public bool darkKnockBackChance;
        // 완료
        [Header("Blue / Default: 상태이상 적 공격시 15% 추가데미지")] 
        public bool darkStatusAilmentDamageBoost;
        // 완료
        [Header("Purple / 7Lv: 사거리 1 증가")] 
        public bool darkRangeIncrease;
        // 완료 
        [Header("Purple / 13Lv: 상태이상 적 공격시 1초 이동속도 20% 감소")] 
        public bool darkStatusAilmentSlowEffect;


        [Header("\n\nC 물: Purple\n")]
        // 완료
        [Header("Green / 5Lv: 공격속도 6% 증가 (최대 4회)")] 
        public int waterAttackSpeedBoost; 
        protected internal void WaterAttackSpeedIncrease()
        {
            if (waterAttackSpeedBoost >= 4) return;
            waterAttackSpeedBoost++;
        }
        // 완료 
        [Header("Green / Default: 같은 속성(물) 유닛 존재시 둔화 비활성화 200% 추가데미지")] 
        public bool waterAllyDamageBoost;
        // 완료
        [Header("Blue / 11Lv: 발사체의 갯수가 2개로 변화합니다.")] 
        public bool waterProjectileIncrease;
        // 완료
        [Header("Blue / 13Lv: 공격력 12% 증가 (최대 4회)")] 
        public int waterAttackBoost; 
        protected internal void WaterAttackDamageIncrease()
        {
            if (waterAttackBoost >= 4) return;
            waterAttackBoost++;
        }
        // 완료
        [Header("Blue / 7Lv: 둔화 적 공격 시 20% 추가데미지")] 
        public bool waterSlowEnemyDamageBoost;
        // 완료
        [Header("Purple / Default: 100회 공격시 모든적 20% 둔화 1초 (웨이브마다 초기화)")] 
        public bool waterGlobalSlowEffect;
        // 완료
        [Header("Purple / 3Lv: 둔화상태의 적 공격시 40% 확률로 0.1초간 기절")] 
        public bool waterSlowEnemyStunChance;
        // 완료
        [Header("Purple / 9Lv: 피격당한 적은 5초간 받는데미지 15% 증가")] 
        public bool waterDamageIncreaseDebuff;


        [Header("\n\nD 물리: Green\n")]
        // 완료 
        [Header("Green / 9Lv: 공격속도 7% 증가 (최대 4회)")] 
        public int physicalAttackSpeedBoost; 
        protected internal void PhysicalAttackSpeedIncrease()
        {
            if (physicalAttackSpeedBoost >= 4) return;
            physicalAttackSpeedBoost++;
        }
        // 완료
        [Header("Green / Default: 다른 물리 속성 유닛이 존재시 데미지 35% 증가")] 
        public bool physicalDamage35Boost;
        // 완료
        [Header("Green / 5Lv: 공격력 6% 증가 (최대 4회)")] 
        public int physicalDamage6Boost; 
        protected internal void PhysicalAttackDamageIncrease()
        {
            if (physicalDamage6Boost >= 4) return;
            physicalDamage6Boost++;
        }
        // 완료 
        [Header("Blue / 11Lv: 공격 시 10% 확률로 10% 데미지를 주는 2초간 출혈")] 
        public bool physicalBleedingChance;
        // 완료
        [Header("Blue / Default: 공격 시 검 한자루 추가")] 
        public bool physicalSwordAddition;
        // 완료
        [Header("BLue / 7Lv: 둔화된 적을 공격할 시 데미지가 15% 증가")] 
        public bool physicalSlowEnemyDamageBoost;
        // 완료
        [Header("Purple / 3Lv: 검의 크기가 100% 증가")] 
        public bool physicalSwordScaleIncrease;
        // 완료
        [Header("Purple / 13Lv: 공격력 18% 증가")] 
        public bool physicalDamage18Boost;


        [Header("\n\nE 물: Green\n")]
        // 완료 
        [Header("Green / Default: 둔화지속시간 0.1초씩 증가 (최대 0.5초 / 5회)")] 
        public int water2DebuffDurationIncrease; 
        protected internal void Water2DebuffDurationIncrease()
        {
            if (water2DebuffDurationIncrease >= 5) return;
            water2DebuffDurationIncrease++;
        }
        // 완료
        [Header("Green / 7Lv: 공격속도 6% 증가 (최대 4회)")] 
        public int water2AttackSpeedIncrease; 
        protected internal void Water2AttackSpeedIncrease()
        {
            if (water2AttackSpeedIncrease >= 4) return;
            water2AttackSpeedIncrease++;
        }
        // 완료
        [Header("Green / 11Lv: 출혈 중인 적을 공격 시 10% 확률로 0.5초 기절")] 
        public bool water2StunChanceAgainstBleeding;
        // 완료
        [Header("Blue / 9Lv: 적을 죽이면 그 위치에서 좌우로 고드름 발사")] 
        public bool water2IceSpikeProjectile;
        // 완료
        [Header("Blue / 5Lv: 공격력 12% 증가 (최대 4회)")] 
        public int water2AttackPowerIncrease; 
        protected internal void Water2AttackPowerIncrease()
        {
            if (water2AttackPowerIncrease >= 4) return;
            water2AttackPowerIncrease++;
        }
        // 완료
        [Header("BLue / 3Lv: 투사체 속도 50% 증가")] 
        public bool water2ProjectileSpeedIncrease;
        // 완료
        [Header("Purple / Default: 둔화 강도 10% 증가 (30% => 40%)")] 
        public bool water2DebuffStrengthIncrease;
        // 완료 
        [Header("Purple / 13Lv: 주위의 아군 공격속도 10% 증가")] 
        public bool water2AttackSpeedBuffToAdjacentAllies;


        [Header("\n\nF 독: Green\n")]
        // 완료
        [Header("Green / Default: 공격속도 6% 증가 (최대4회)")] 
        public int poisonAttackSpeedIncrease; 
        protected internal void PoisonAttackSpeedIncrease()
        {
            if(poisonAttackSpeedIncrease >= 4) return;
            poisonAttackSpeedIncrease++;
        }
        // 완료
        [Header("Green / 3Lv: 중독최대중첩 수 1회 증가 (최대 5)")] 
        public int poisonMaxStackIncrease; 
        protected internal void PoisonMaxStackIncrease()
        {
            if (poisonMaxStackIncrease >= 5) return;
            poisonMaxStackIncrease++;
        }
        // 완료
        [Header("Green / 11Lv: 중독 데미지 10% 증가 (최대 40% / 최대 3회)")] 
        public int poisonDamageAttackPowerIncrease; 
        protected internal void PoisonDamageAttackPowerIncrease()
        {
            if (poisonDamageAttackPowerIncrease >= 3) return;
            poisonDamageAttackPowerIncrease++;
        }
        // 완료
        [Header("Blue / 5Lv: 투사체 1개 증가")] 
        public bool poisonProjectileIncrease;
        // 완료
        [Header("Blue / 9Lv: 사거리 1칸 증가")] 
        public bool poisonRangeIncrease;
        // 완료
        [Header("Blue / 13Lv: 출혈중인 적 공격시 50% 데미지 증가")] 
        public bool poisonBleedingEnemyDamageBoost;
        // 완료
        [Header("Purple / Default: 중독 된 적의 체력이 7% 미만이면 즉사")] 
        public bool poisonEnemyInstantKill;
        // 완료
        [Header("Purple / 7Lv: 초당 10% 데미지를 가하는 중족을 4초간 발생")] 
        public bool poisonPerHitEffect;


        [Header("\n\nG 불: Blue\n")]
        // 완료 
        [Header("Green/ 3Lv: 독에 걸린 적 공격시 15% 데미지 추가")] 
        public bool fire2PoisonDamageIncrease;
        // 완료 
        [Header("Green/ 9Lv: 공격속도 6% 증가 (최대 4회)")] 
        public int fire2AttackSpeedIncrease; 
        protected internal void Fire2AttackSpeedIncrease()
        {
            if (fire2AttackSpeedIncrease >= 4) return;
            fire2AttackSpeedIncrease++;
        }
        // 완료
        [Header("Green/ 11Lv: 화상 최대 중첩 3회 증가")] 
        public bool fire2BurnStackIncrease;
        // 완료
        [Header("Blue/ Default: 공격력 12% 증가 (최대 4회)")] 
        public int fire2AttackPowerIncrease; 
        protected internal void Fire2AttackPowerIncrease()
        {
            if (fire2AttackPowerIncrease >= 4) return;
            fire2AttackPowerIncrease++;
        }
        // 완료
        [Header("Blue/ 13Lv: 독에 걸린 적 공격시 20% 확률로 0.5초 기절")] 
        public bool fire2StunChance;
        // 완료
        [Header("Blue/ 5Lv: 화상 데미지 10% 증가 (최대 40% / 최대 3회)")]
        public int fire2BurningDamageBoost;
        protected internal void Fire2BurningDamageBoost()
        {
            if(fire2BurningDamageBoost >= 3) return;
            fire2BurningDamageBoost++;
        }
        // 완료
        [Header("Purple/ Default: 검의 크기가 100% 증가")] 
        public bool fire2SwordSizeIncrease;
        // 완료
        [Header("Purple/ 7Lv: 더이상 화상을 발생시키지 않는 대신 150% 데미지 추가")] 
        public bool fire2NoBurnDamageIncrease;


        [Header("\n\nH 불: Blue\n")]
        // 완료
        [Header("Green/ Default: 화상 중첩수 증가 (최대 5회)")] 
        public int fireImageOverlapIncrease = 1 ; 
        protected internal void FireOverLapIncrease()
        {
            if (fireImageOverlapIncrease >= 5) return;
            fireImageOverlapIncrease++;
        }
        // 완료
        [Header("Green/ 9Lv: 공격속도 6% 증가 (최대 4회)")] 
        public int fireAttackSpeedBoost; 
        protected internal void FireAttackSpeedBoost()
        {
            if (fireAttackSpeedBoost >= 4) return;
            fireAttackSpeedBoost++;
        }
        // 완료
        [Header("Green/ 11Lv: 둔화중인 적을 공격시 10% 데미지 추가")]
        public bool fireSlowEnemyDamageBoost;
        // 완료
        [Header("Blue/ 3Lv: 투사체 속도가 100% 증가, 반드시 명중")] 
        public bool fireProjectileSpeedIncrease;
        // 완료
        [Header("Blue/ 7Lv: 화상에 걸린 적 제거시 주변 1칸 범위의 200% 폭발데미지 추가")] 
        public bool fireBurnedEnemyExplosion;
        // 완료
        [Header("Purple/ 5Lv: 적 적중시 가장 가까운적에게 투사체 튕김")] 
        public bool fireProjectileBounceDamage;
        // 완료 
        [Header("Purple/ Default: 적을 공격하면 5초간 화상 초당 10% 데미지")] 
        public bool fireBurnPerAttackEffect;
        // 완료
        [Header("Purple/ 13Lv: 투사체가 튕기는 횟수 증가")] 
        public bool fireProjectileBounceIncrease;
       
        
        [Header("\n\n공통강화\n")] 
        [Header("가로줄 추가")] 
        public int addRowCount; 
        public delegate void AddRowDelegate(); 
        public event AddRowDelegate OnAddRow; 
        protected internal void AddRow()
        {
            addRowCount += 1;
            gridManager.AddRow();
            OnAddRow?.Invoke();
        }
        [Header("적 이동속도 감소 15%증가 (최대 45%)")] 
        public int slowCount; 
        protected internal void SlowCount()
        {
            slowCount++;
            if (slowCount >= 4)
            {
                slowCount = 4;
            }
        }
        [Header("대각선 이동")] 
        public bool diagonalMovement;
        [Header("Castle 체력회복 200")] 
        public bool recoveryCastle;
        [Header("Castle 최대체력 증가 (최대 2000)")] 
        public float castleMaxHp; 
        protected internal void IncreaseCastleMaxHp()
        {
            castleMaxHp += 200f;
            castleManager.IncreaseMaxHp();
        }
        [Header("보드 초기화 케릭터")] 
        public int highLevelCharacterCount = 6; 
        public int selectedCount; 
        protected internal void NextCharacterUpgrade(int moveCharacterCount)
        {
            highLevelCharacterCount += moveCharacterCount;
            selectedCount += 1;
        }
        [Header("경험치 증가량")] 
        public int expPercentage; 
        protected internal void IncreaseExpBuff(int increaseAmount)
        {
            expPercentage += increaseAmount;
        }
        [Header("최대 이동횟수 증가")] 
        public int permanentIncreaseMovementCount; 
        protected internal void PermanentIncreaseMoveCount(int increaseStepAmount)
        {
            permanentIncreaseMovementCount += increaseStepAmount;
        }
        [Header("5매치 가운데 유닛 추가 레벨증가")] 
        public bool match5Upgrade;
        [Header("전체 공격력 증가 (%)")] 
        public float increaseAtkDamage = 1f; 
        protected internal void IncreaseGroupDamage(int increaseAmount)
        {
            increaseAtkDamage += increaseAmount;
        }
        [Header("전체 공격속도 증가 (%)")] 
        public float increaseAtkRate = 1f; 
        protected internal void IncreaseGroupRate(float increaseRateAmount)
        {
            increaseAtkRate += increaseRateAmount;
        }
        [Header("이동횟수 추가")] 
        public int rewardMoveCount; 
        protected internal void RewardMoveCount(int moveCount)
        {
            rewardMoveCount += moveCount;
        }
        [Header("추가코인")] 
        public bool addGold; 
        public int addGoldCount; 
        protected internal void AddGold()
        {
            addGoldCount++;
        }
        // RandomUnitLevelUp
        public void RandomCharacterLevelUp(int characterCount)
        {
            var activeCharacters = characterPool.UsePoolCharacterList();
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
            var activeCharacterGroup = characterPool.UsePoolCharacterList();

            foreach (var character in  activeCharacterGroup)
            {
                var characterObj = character.GetComponent<CharacterBase>();
                if (group == characterObj.unitGroup && characterObj.unitPuzzleLevel == 1)
                {
                    characterObj.LevelUpScale(character);
                }
            }
        }
        // Unit Group PermanentLevelUp
        public List<int> permanentGroupIndex = new List<int>(); public void PermanentIncreaseCharacter(int characterListIndex)
        {
            permanentGroupIndex.Add(characterListIndex);
        }
        public void SaveEnforceData()
        {
            var data = new EnforceData
            {
                //Divine
                divinePoisonDamageBoost = divinePoisonDamageBoost,
                divineBindDurationBoost = divineBindDurationBoost,
                divineShackledExplosion = divineShackledExplosion,
                divineFifthAttackBoost = divineFifthAttackBoost,
                divineAttackBoost = divineAttackBoost,
                divineBindChanceBoost = divineBindChanceBoost,
                divineDualAttack = divineDualAttack,
                divineProjectilePierce = divineProjectilePierce,
                //Darkness Unit B
                darkTenthAttackDamageBoost = darkTenthAttackDamageBoost,
                darkAttackSpeedBoost = darkAttackSpeedBoost,                        
                darkAttackPowerBoost = darkAttackPowerBoost,
                darkStatusAilmentDamageChance = darkStatusAilmentDamageChance,
                darkKnockBackChance = darkKnockBackChance,
                darkStatusAilmentDamageBoost = darkStatusAilmentDamageBoost,
                darkRangeIncrease = darkRangeIncrease,
                darkStatusAilmentSlowEffect = darkStatusAilmentSlowEffect,
                //Water1 Unit C
                waterAttackSpeedBoost = waterAttackSpeedBoost, 
                waterAllyDamageBoost = waterAllyDamageBoost, 
                waterProjectileIncrease = waterProjectileIncrease, 
                waterAttackBoost = waterAttackBoost, 
                waterSlowEnemyDamageBoost = waterSlowEnemyDamageBoost, 
                waterGlobalSlowEffect = waterGlobalSlowEffect, 
                waterSlowEnemyStunChance = waterSlowEnemyStunChance, 
                waterDamageIncreaseDebuff = waterDamageIncreaseDebuff,
                //Physical Unit D
                physicalAttackSpeedBoost = physicalAttackSpeedBoost,
                physicalDamage35Boost = physicalDamage35Boost,
                physicalDamage6Boost = physicalDamage6Boost,
                physicalBleedingChance = physicalBleedingChance,
                physicalSwordAddition = physicalSwordAddition,
                physicalSlowEnemyDamageBoost = physicalSlowEnemyDamageBoost,
                physicalSwordScaleIncrease = physicalSwordScaleIncrease,
                physicalDamage18Boost = physicalDamage18Boost,
                //Water2 Unit E
                water2DebuffDurationIncrease = water2DebuffDurationIncrease,
                water2AttackSpeedIncrease = water2AttackSpeedIncrease,
                water2StunChanceAgainstBleeding = water2StunChanceAgainstBleeding,
                water2IceSpikeProjectile = water2IceSpikeProjectile,
                water2AttackPowerIncrease = water2AttackPowerIncrease,
                water2ProjectileSpeedIncrease = water2ProjectileSpeedIncrease,
                water2DebuffStrengthIncrease = water2DebuffStrengthIncrease,
                water2AttackSpeedBuffToAdjacentAllies = water2AttackSpeedBuffToAdjacentAllies,
                //Poison Unit F
                poisonAttackSpeedIncrease = poisonAttackSpeedIncrease,
                poisonMaxStackIncrease = poisonMaxStackIncrease,
                poisonDamageAttackPowerIncrease = poisonDamageAttackPowerIncrease,
                poisonProjectileIncrease = poisonProjectileIncrease,
                poisonRangeIncrease = poisonRangeIncrease,
                poisonBleedingEnemyDamageBoost = poisonBleedingEnemyDamageBoost,
                poisonEnemyInstantKill = poisonEnemyInstantKill,
                poisonPerHitEffect = poisonPerHitEffect,
                //Fire2 Unit G
                fire2PoisonDamageIncrease = fire2PoisonDamageIncrease,
                fire2AttackSpeedIncrease = fire2AttackSpeedIncrease,
                fire2BurnStackIncrease = fire2BurnStackIncrease,
                fire2AttackPowerIncrease = fire2AttackPowerIncrease,
                fire2StunChance = fire2StunChance,
                fire2SwordSizeIncrease = fire2SwordSizeIncrease,
                fire2BurningDamageBoost = fire2BurningDamageBoost,
                fire2NoBurnDamageIncrease = fire2NoBurnDamageIncrease,
                //Fire1 Unit H
                fireImageOverlapIncrease = fireImageOverlapIncrease,
                fireAttackSpeedBoost = fireAttackSpeedBoost,
                fireSlowEnemyDamageBoost = fireSlowEnemyDamageBoost,
                fireProjectileSpeedIncrease = fireProjectileSpeedIncrease,
                fireBurnedEnemyExplosion = fireBurnedEnemyExplosion,
                fireProjectileBounceDamage = fireProjectileBounceDamage,
                fireBurnPerAttackEffect = fireBurnPerAttackEffect,
                fireProjectileBounceIncrease = fireProjectileBounceIncrease,

                addRowCount = addRowCount,
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
                addGold = addGold, 
                addGoldCount = addGoldCount,
                permanentGroupIndex = permanentGroupIndex
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
            divinePoisonDamageBoost = data.divinePoisonDamageBoost;
            divineBindDurationBoost = data.divineBindDurationBoost;
            divineShackledExplosion = data.divineShackledExplosion;
            divineFifthAttackBoost = data.divineFifthAttackBoost;
            divineAttackBoost = data.divineAttackBoost;
            divineBindChanceBoost = data.divineBindChanceBoost;                                                        
            divineDualAttack = data.divineDualAttack;
            divineProjectilePierce = data.divineProjectilePierce;
            //Darkness Unit B
            darkTenthAttackDamageBoost = data.darkTenthAttackDamageBoost;
            darkAttackSpeedBoost = data.darkAttackSpeedBoost;
            darkAttackPowerBoost = data.darkAttackPowerBoost;
            darkStatusAilmentDamageChance = data.darkStatusAilmentDamageChance;
            darkKnockBackChance = data.darkKnockBackChance;
            darkStatusAilmentDamageBoost = data.darkStatusAilmentDamageBoost;
            darkRangeIncrease = data.darkRangeIncrease;
            darkStatusAilmentSlowEffect = data.darkStatusAilmentSlowEffect;
            //Water1 Unit C
            waterAttackSpeedBoost = data.waterAttackSpeedBoost; 
            waterAllyDamageBoost = data.waterAllyDamageBoost; 
            waterProjectileIncrease = data.waterProjectileIncrease; 
            waterAttackBoost = data.waterAttackBoost; 
            waterSlowEnemyDamageBoost = data.waterSlowEnemyDamageBoost; 
            waterGlobalSlowEffect = data.waterGlobalSlowEffect; 
            waterSlowEnemyStunChance = data.waterSlowEnemyStunChance; 
            waterDamageIncreaseDebuff = data.waterDamageIncreaseDebuff;
            //Physical Unit D
            physicalAttackSpeedBoost = data.physicalAttackSpeedBoost;
            physicalDamage35Boost = data.physicalDamage35Boost;
            physicalDamage6Boost = data.physicalDamage6Boost;
            physicalBleedingChance = data.physicalBleedingChance;
            physicalSwordAddition = data.physicalSwordAddition;
            physicalSlowEnemyDamageBoost = data.physicalSlowEnemyDamageBoost;
            physicalSwordScaleIncrease = data.physicalSwordScaleIncrease;
            physicalDamage18Boost = data.physicalDamage18Boost;
            //Water2 Unit E
            water2DebuffDurationIncrease = data.water2DebuffDurationIncrease;
            water2AttackSpeedIncrease = data.water2AttackSpeedIncrease;
            water2StunChanceAgainstBleeding = data.water2StunChanceAgainstBleeding;
            water2IceSpikeProjectile = data.water2IceSpikeProjectile;
            water2AttackPowerIncrease = data.water2AttackPowerIncrease;
            water2ProjectileSpeedIncrease = data.water2ProjectileSpeedIncrease;
            water2DebuffStrengthIncrease = data.water2DebuffStrengthIncrease;
            water2AttackSpeedBuffToAdjacentAllies = data.water2AttackSpeedBuffToAdjacentAllies;
            //Poison Unit F
            poisonAttackSpeedIncrease = data.poisonAttackSpeedIncrease;
            poisonMaxStackIncrease = data.poisonMaxStackIncrease;
            poisonDamageAttackPowerIncrease = data.poisonDamageAttackPowerIncrease;
            poisonProjectileIncrease = data.poisonProjectileIncrease;
            poisonRangeIncrease = data.poisonRangeIncrease;
            poisonBleedingEnemyDamageBoost = data.poisonBleedingEnemyDamageBoost;
            poisonEnemyInstantKill = data.poisonEnemyInstantKill;
            poisonPerHitEffect = data.poisonPerHitEffect;
            //Fire2 Unit G
            fire2PoisonDamageIncrease = data.fire2PoisonDamageIncrease;
            fire2AttackSpeedIncrease = data.fire2AttackSpeedIncrease;
            fire2BurnStackIncrease = data.fire2BurnStackIncrease;
            fire2AttackPowerIncrease = data.fire2AttackPowerIncrease;
            fire2StunChance = data.fire2StunChance;
            fire2SwordSizeIncrease = data.fire2SwordSizeIncrease;
            fire2BurningDamageBoost = data.fire2BurningDamageBoost;
            fire2NoBurnDamageIncrease = data.fire2NoBurnDamageIncrease;
            //Fire1 Unit H
            fireImageOverlapIncrease = data.fireImageOverlapIncrease;
            fireAttackSpeedBoost = data.fireAttackSpeedBoost;
            fireSlowEnemyDamageBoost = data.fireSlowEnemyDamageBoost;
            fireProjectileSpeedIncrease = data.fireProjectileSpeedIncrease;
            fireBurnedEnemyExplosion = data.fireBurnedEnemyExplosion;
            fireProjectileBounceDamage = data.fireProjectileBounceDamage;
            fireBurnPerAttackEffect = data.fireBurnPerAttackEffect;
            fireProjectileBounceIncrease = data.fireProjectileBounceIncrease;

            addRowCount = data.addRowCount;
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
            addGold = data.addGold;
            addGoldCount = data.addGoldCount;
            permanentGroupIndex = new List<int>(data.permanentGroupIndex);
        }                                  
    }
}

