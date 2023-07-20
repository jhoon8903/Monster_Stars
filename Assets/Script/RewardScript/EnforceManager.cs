using System;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.PuzzleManagerGroup;
using Script.RobbyScript.CharacterSelectMenuGroup;
using Script.UIManager;
using UnityEngine;

namespace Script.RewardScript
{
    [Serializable]
    public class EnforceData
    {
        //Divine 유닛 1
        public bool divinePoisonDamageBoost;
        public float divineBindDurationBoost;
        public bool divineShackledExplosion;
        public int divineFifthAttackBoost;
        public int divineAttackBoost;
        public float divineBindChanceBoost;
        public bool divineDualAttack;
        public bool divineProjectilePierce;

        //Physical 유닛 2
        public int physicalAttackSpeedBoost;
        public bool physicalDamage100Boost;
        public int physicalDamage9Boost;
        public bool physicalBleedingChance;
        public bool physicalSwordAddition;
        public bool physicalSlowEnemyDamageBoost;
        public bool physicalSwordScaleIncrease;
        public bool physicalDamage24Boost;

<<<<<<< Updated upstream
        //Poison 유닛 3
        public bool PoisonAilmentStun;
        public int PoisonMaxStackIncrease;
        public int PoisonDamageAttackPowerIncrease;
        public bool PoisonProjectileIncrease;
        public bool PoisonRangeIncrease;
        public bool PoisonBleedingEnemyDamageBoost;
        public bool PoisonBleedingEnemyInstantKill;
        public bool PoisonPerHitEffect;
=======
        //Water2 Unit E
        public int water2DebuffDurationIncrease;
        public int water2AttackSpeedIncrease;
        public bool water2StunChanceAgainstBleeding;
        public bool water2IceSpikeProjectile;
        public int water2AttackPowerIncrease;
        public bool water2ProjectileSpeedIncrease;
        public bool water2DebuffStrengthIncrease;
        public bool water2AttackSpeedBuffToAdjacentAllies;
>>>>>>> Stashed changes

        //Fire1 유닛 6
        public int FireImageOverlapIncrease;
        public int FireAttackSpeedBoost;
        public bool FireSlowEnemyDamageBoost;
        public bool FireProjectileSpeedIncrease;
        public bool FireBurnedEnemyExplosion;
        public bool FireProjectileBounceDamage;
        public float FireBurnPerAttackEffect;
        public bool FireProjectileBounceIncrease;

<<<<<<< Updated upstream
        //Water1 유닛 8
        public int WaterAttackSpeedBoost;
        public bool WaterAllyDamageBoost;
        public bool WaterProjectileIncrease;
        public int WaterAttackBoost;
        public bool WaterSlowEnemyDamageBoost;
        public int WaterGlobalSlowEffect;
        public bool WaterSlowEnemyStunChance;
        public bool WaterDamageIncreaseDebuff;

        //Fire2 유닛 5
        public bool Fire2PoisonDamageIncrease;
        public int Fire2AttackSpeedIncrease;
        public bool Fire2BleedingDamageIncrease;
        public int Fire2AttackPowerIncrease;
        public bool Fire2StunChance;
        public bool Fire2SwordSizeIncrease;
        public bool Fire2RangeIncrease;
        public bool Fire2NoBurnDamageIncrease;

        //Water2 유닛 4
        public int Water2DebuffDurationIncrease;
        public int Water2AttackSpeedIncrease;
        public bool Water2StunChanceAgainstBleeding;
        public bool Water2IceSpikeProjectile;
        public int Water2AttackPowerIncrease;
        public bool Water2ProjectileSpeedIncrease;
        public float Water2DebuffStrengthIncrease;
        public bool Water2AttackSpeedBuffToAdjacentAllies;

        //Darkness 유닛  7
        public int DarkTenthAttackDoubleDamage;
        public int DarkAttackSpeedBoost;
        public int DarkAttackPowerBoost;
        public bool DarkStatusAilmentDamageChance;
        public float DarkKnockBackChance;
        public bool DarkStatusAilmentDamageBoost;
        public bool DarkRangeIncrease;
        public bool DarkStatusAilmentSlowEffect;
=======
        //Fire2 Unit G
        public bool fire2PoisonDamageIncrease;
        public int fire2AttackSpeedIncrease;
        public bool fire2BleedingDamageIncrease;
        public int fire2AttackPowerIncrease;
        public bool fire2StunChance;
        public bool fire2SwordSizeIncrease;
        public bool fire2RangeIncrease;
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
>>>>>>> Stashed changes

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
}

    public class EnforceManager : MonoBehaviour
    {
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private CharacterPool characterPool;
        [SerializeField] private MatchManager matchManager;
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

        [Header("\n\nUnit_A 신성 속성\n")] 
        [Header("속박 활성화")] public bool activeRestraint;
        [Header("속박시간 증가")] public float increaseRestraintTime = 0.1f;
        protected internal float IncreaseRestraintTime()
        {
            var restraintTime = 1f;
            restraintTime += increaseRestraintTime;
            return restraintTime;
        }
        [Header("관통 효과")] public bool divinePenetrate;
        [Header("백 어택")] public bool divineAtkRange;
        [Header("중독 시 데미지증가")] public bool divinePoisonAdditionalDamage; public int divinePoisonAdditionalDamageCount = 1;
        [Header("\n\n Unit_B 어둠 속성\n\n")] 
        [Header("둔화상태 적 추가 데미지")] public bool darkSlowAdditionalDamage;
        [Header("출혈상태 적 추가 데미지")] public bool darkBleedAdditionalDamage;
        [Header("공격속도 17% 상승 ")] public int darkIncreaseAtkSpeed;

        protected internal void DarkIncreaseAtkSpeed()
        {
            darkIncreaseAtkSpeed++;
        }

        [Header("쿠션 효과")] public bool darkProjectileBounce;

        [Header("쿠션 추가")] public int darkProjectileBounceCount = 1;

        protected internal void AddBounceCount()
        {
            darkProjectileBounceCount++;
        }

        [Header("\n\nUnit_C 물 속성2\n\n")] [Header("공격력 증가")]
        public int water2IncreaseDamage;

        protected internal void Water2IncreaseDamage()
        {
            water2IncreaseDamage++;
        }

        [Header("출혈상태 적 공격시 속박")] public bool water2BleedAdditionalRestraint;
        [Header("둔화지속시간 증가")] public int water2IncreaseSlowTime;

        protected internal void Water2IncreaseSlowTime()
        {
            water2IncreaseSlowTime++;
        }

<<<<<<< Updated upstream
        [Header("백 어택")] public bool water2BackAttack;
        [Header("투사체 추가")] public bool water2AdditionalProjectile;
=======
        [Header("\n\nD 물리: Green\n")]
        [Header("Green / 9Lv: 공격속도 9% 증가 (최대 8회)")] public int physicalAttackSpeedBoost; protected internal void PhysicalAttackSpeedIncrease()
        {
            if (physicalAttackSpeedBoost >= 8) return;
            physicalAttackSpeedBoost++;
        }
        [Header("Green / Default: 다른 물리 속성 유닛이 존재시 데미지 100% 증가")] public bool physicalDamage100Boost;
        [Header("Green / 5Lv: 공격력 9% 증가 (최대 8회)")] public int physicalDamage9Boost; protected internal void PhysicalAttackDamageIncrease()
        {
            if (physicalDamage9Boost >= 8) return;
            physicalDamage9Boost++;
        }
        [Header("Blue / 11Lv: 공격 시 10% 확률로 2초간 출혈")] public bool physicalBleedingChance;
        [Header("Blue / Default: 공격 시 검 한자루 추가")] public bool physicalSwordAddition;
        [Header("BLue / 7Lv: 둔화된 적을 공격할 시 데미지가 20% 증가")] public bool physicalSlowEnemyDamageBoost;
        [Header("Purple / 3Lv: 검의 크기가 100% 증가")] public bool physicalSwordScaleIncrease;
        [Header("Purple / 13Lv: 공격력 24% 증가")] public bool physicalDamage24Boost;

        [Header("\n\nE 물: Green\n")]
        [Header("Green / Default: 둔화지속시간 0.2초씩 증가 ( 최대 1초 / 최대 5회)")] public int water2DebuffDurationIncrease;
        [Header("Green / 7Lv: 공격속도 6% 증가 (최대 8)")] public int water2AttackSpeedIncrease; protected internal void Water2AttackSpeedIncrease()
        {
            if (water2AttackSpeedIncrease >= 8) return;
            water2AttackSpeedIncrease++;
        }
        [Header("Green / 9Lv: 출혈 중인 적을 공격 시 10% 확률로 적 기절")] public bool water2StunChanceAgainstBleeding;
        [Header("Blue / 3Lv: E 유닛이 적을 죽이면 그 위치에서 좌우로 고드름 발사")] public bool water2IceSpikeProjectile;
        [Header("Blue / 5Lv: 공격력 18% 증가 (최대 6회)")] public int water2AttackPowerIncrease; protected internal void Water2AttackPowerIncrease()
        {
            if (water2AttackPowerIncrease >= 6) return;
            water2AttackPowerIncrease++;
        }
        [Header("BLue / 11Lv: 투사체 속도 100% 증가")] public bool water2ProjectileSpeedIncrease;
        [Header("Purple / Default:둔화 강도 20% 증가 (30% > 50%)")] public bool water2DebuffStrengthIncrease;
        [Header("Purple / 13Lv: 주위의 아군 공격속도 15% 증가")] public bool water2AttackSpeedBuffToAdjacentAllies;

        [Header("\n\nF 독: Green\n")]
        [Header("Green / Default: 상태이상에 걸린 적 공격시 0.2초 기절")] public bool poisonAilmentStun;
        [Header("Green / 3Lv: 중독최대중첩 수 1회 증가 (최대 5)")] public int poisonMaxStackIncrease;
        [Header("Green / 11Lv: 중독 데미지 10% 증가 (최대 50% / 최대 5회)")] public int poisonDamageAttackPowerIncrease;
        [Header("Blue / 5Lv: 투사체 1개 증가")] public bool poisonProjectileIncrease;
        [Header("Blue / 9Lv: 사거리 1칸 증가")] public bool poisonRangeIncrease;
        [Header("Blue / 13Lv: 출혈중인 적 공격시 50% 데미지 증가")] public bool poisonBleedingEnemyDamageBoost;
        [Header("Purple / Default: 출혈중인 적의 체력이 10% 미만이면 즉사")] public bool poisonBleedingEnemyInstantKill;
        [Header("Purple / 7Lv: F유닛 적 타격6 2초 중독")] public bool poisonPerHitEffect;

        [Header("\n\nG 불: Blue\n")]
        [Header(" Green/ 3Lv: 독에 걸린 적 공격시 25% 데미지 추가")] public bool fire2PoisonDamageIncrease;
        [Header(" Green/ 9Lv: 공격속도 7% 증가 (최대 8회)")] public int fire2AttackSpeedIncrease; protected internal void Fire2AttackSpeedIncrease()
        {
            if (fire2AttackSpeedIncrease >= 8) return;
            fire2AttackSpeedIncrease++;
        }
        [Header(" Green/ 11Lv: 출혈중인 적 공격시 25% 데미지 추가")] public bool fire2BleedingDamageIncrease;
        [Header(" Blue/ Default: 공격력 12% 증가 (최대 6회)")] public int fire2AttackPowerIncrease; protected internal void Fire2AttackPowerIncrease()
        {
            if (fire2AttackPowerIncrease >= 6) return;
            fire2AttackPowerIncrease++;
        }
        [Header(" Blue/ 13Lv: 독에 걸린 적 공격시 20% 확률로 0.5초 기절")] public bool fire2StunChance;
        [Header(" Blue/ 5Lv: 검의 크기가 100% 증가")] public bool fire2SwordSizeIncrease;
        [Header(" Purple/ Default: 사거리가 1칸 증가")] public bool fire2RangeIncrease;
        [Header(" Purple/ 7Lv: 더이상 화상을 발새시키지 않는 대신 100% 데미지 추가")] public bool fire2NoBurnDamageIncrease;

        [Header("\n\nH 불: Blue\n")]
        [Header(" Green/ Default: 화상 중첩수 증가 (최대 3회)")] public int fireImageOverlapIncrease;
        [Header(" Green/ 9Lv: 공격속도 9% 증가 (최대 8회)")] public int fireAttackSpeedBoost; protected internal void FireAttackSpeedBoost()
        {
            if (fireAttackSpeedBoost >= 8) return;
            fireAttackSpeedBoost++;
        }
        [Header(" Green/ 11Lv: 둔화중인 적을 공격시 15% 데미지 추가")] public bool fireSlowEnemyDamageBoost;
        [Header(" Blue/ 3Lv: 투사체 속도가 100% 증가, 반드시 명중")] public bool fireProjectileSpeedIncrease;
        [Header(" Blue/ 7Lv: 화상에 걸린 적 제거시 주변 1칸 범위의 100% 폭발데미지 추가")] public bool fireBurnedEnemyExplosion;
        [Header(" Purple/ 5Lv: 적 적중시 가장 가까운적에게 투사체 튕김")] public bool fireProjectileBounceDamage;
        [Header(" Purple/ Default: 적을 공격하면 5초간 화상 초당 20% 데미지")] public bool fireBurnPerAttackEffect;
        [Header(" Purple/ 13Lv: 투사체가 튕기는 횟수 증가")] public bool fireProjectileBounceIncrease;

>>>>>>> Stashed changes

        [Header("\n\nUnit_D 물리 속성\n")] [Header("Sword 추가")]
        public bool physicAdditionalWeapon;

        [Header("Sword 크기 2배")] public bool physicIncreaseWeaponScale;
        [Header("둔화 시 데미지추가")] public bool physicSlowAdditionalDamage;
        [Header("공격속도 증가")] public float increasePhysicAtkSpeed = 1f;

        protected internal void IncreasePhysicAtkSpeed()
        {
            increasePhysicAtkSpeed += 0.2f;
        }

        [Header("데미지 증가")] public bool physicIncreaseDamage;
        public float increasePhysicsDamage = 1f;

        protected internal void PhysicIncreaseDamage()
        {
            increasePhysicsDamage += 0.05f;
        }

        [Header("\n\nUnit_E 물 속성\n")] [Header("화상상태 적 추가데미지")]
        public bool waterBurnAdditionalDamage;

        [Header("속박상태 적 추가데미지")] public bool waterRestraintIncreaseDamage;
        [Header("둔화강도 증가")] public bool waterIncreaseSlowPower;
        [Header("공격력증가")] public float increaseWaterDamage = 1f;

        protected internal void WaterIncreaseDamage()
        {
            increaseWaterDamage *= 1.2f;
        }

        [Header("좌/우 동시 공격")] public bool waterSideAttack;

        [Header("\n\nUnit_F 독 속성\n")] [Header("더블어택")]
        public bool poisonDoubleAtk;

        [Header("속박된 적 추가데미지")] public bool poisonRestraintAdditionalDamage;
        [Header("15% 확률 즉사")] public bool poisonInstantKill;
        [Header("공격사거리 1 증가")] public bool poisonIncreaseAtkRange;
        [Header("중독활성화")] public bool activatePoison;
        [Header("중족최대 중첩수 증가")] public int poisonOverlapping = 1;

        protected internal void AddPoisonOverlapping()
        {
            poisonOverlapping += 1;
        }

        [Header("\n\nUnit_G 불 속성\n\n")] [Header("출혈상태 적 추가 데미지")]
        public bool fireBleedingAdditionalDamage;

        [Header("공격력 증가")] public int fireIncreaseDamage;

        protected internal void FireIncreaseDamage()
        {
            fireIncreaseDamage++;
        }

        [Header("중독상태 적 기절")] public bool firePoisonAdditionalStun;
        [Header("공격사거리 1 증가")] public bool fireIncreaseAtkRange;
        [Header("화상효과 비활성화 => 추가 데미지")] public bool fireDeleteBurnIncreaseDamage;

        [Header("\n\nUnit_H 물리 속성\n\n")] [Header("출혈중첩 증가")]
        public int physics2AdditionalBleedingLayer = 1;

        protected internal void Physics2AdditionalBleedingLayer()
        {
            physics2AdditionalBleedingLayer++;

        }

        [Header("출혈 활성화")] public bool physics2ActivateBleed;
        [Header("공격속도 증가")] public int physics2AdditionalAtkSpeed;

        protected internal void Physics2AdditionalAtkSpeed()
        {
            physics2AdditionalAtkSpeed++;
        }

        [Header("투사체 추가")] public bool physics2AdditionalProjectile;
        [Header("관통 효과")] public bool physics2ProjectilePenetration;

        [Header("\n\n공통강화\n")] [Header("가로줄 추가")]
        public int addRowCount;

        public delegate void AddRowDelegate();

        public event AddRowDelegate OnAddRow;

        protected internal void AddRow()
        {
            addRowCount += 1;
            gridManager.AddRow();
            OnAddRow?.Invoke();
        }

        [Header("적 이동속도 감소 15%증가 (최대 45%)")] public int slowCount;

        protected internal void SlowCount()
        {
            slowCount++;
            if (slowCount >= 4)
            {
                slowCount = 4;
            }
        }

        [Header("대각선 이동")] public bool diagonalMovement;
        [Header("Castle 체력회복 200")] public bool recoveryCastle;
        [Header("Castle 최대체력 증가 (최대 2000)")] public float castleMaxHp;

        protected internal void IncreaseCastleMaxHp()
        {
            castleMaxHp += 200f;
            castleManager.IncreaseMaxHp();
        }

        [Header("보드 초기화 케릭터")] public int highLevelCharacterCount = 6;
        public int selectedCount;

        protected internal void NextCharacterUpgrade(int moveCharacterCount)
        {
            highLevelCharacterCount += moveCharacterCount;
            selectedCount += 1;
        }

        [Header("경험치 증가량")] public int expPercentage;

        protected internal void IncreaseExpBuff(int increaseAmount)
        {
            expPercentage += increaseAmount;
        }

        [Header("최대 이동횟수 증가")] public int permanentIncreaseMovementCount;

        protected internal void PermanentIncreaseMoveCount(int increaseStepAmount)
        {
            permanentIncreaseMovementCount += increaseStepAmount;
        }

        [Header("5매치 가운데 유닛 추가 레벨증가")] public bool match5Upgrade;
        [Header("전체 공격력 증가 (%)")] public float increaseAtkDamage = 1f;

        protected internal void IncreaseGroupDamage(int increaseAmount)
        {
            increaseAtkDamage += increaseAmount;
        }

        [Header("전체 공격속도 증가 (%)")] public float increaseAtkRate = 1f;

        protected internal void IncreaseGroupRate(float increaseRateAmount)
        {
            increaseAtkRate += increaseRateAmount;
        }

        [Header("이동횟수 추가")] public int rewardMoveCount;

        protected internal void RewardMoveCount(int moveCount)
        {
            rewardMoveCount += moveCount;
        }

        [Header("추가코인")] public bool addGold; public int addGoldCount;
        protected internal void AddGold()
        {
            addGoldCount++;
        }
        
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

        public List<int> permanentGroupIndex = new List<int>();
        public void PermanentIncreaseCharacter(int characterListIndex)
        {
            permanentGroupIndex.Add(characterListIndex);
        }

        public void SaveEnforceData()
        {
            var data = new EnforceData
            {
<<<<<<< Updated upstream
                activeRestraint = activeRestraint,
                increaseRestraintTime = increaseRestraintTime,
                divinePenetrate = divinePenetrate,
                divineAtkRange = divineAtkRange,
                divinePoisonAdditionalDamage = divinePoisonAdditionalDamage,
                divinePoisonAdditionalDamageCount = divinePoisonAdditionalDamageCount,
                darkSlowAdditionalDamage = darkSlowAdditionalDamage,
                darkBleedAdditionalDamage = darkBleedAdditionalDamage,
                darkIncreaseAtkSpeed = darkIncreaseAtkSpeed,
                darkProjectileBounce = darkProjectileBounce,
                darkProjectileBounceCount =  darkProjectileBounceCount,
                water2IncreaseDamage = water2IncreaseDamage,
                water2BleedAdditionalRestraint = water2BleedAdditionalRestraint,
                water2IncreaseSlowTime = water2IncreaseSlowTime,
                water2BackAttack = water2BackAttack,
                water2AdditionalProjectile = water2AdditionalProjectile,
                physicAdditionalWeapon = physicAdditionalWeapon,
                physicIncreaseWeaponScale = physicIncreaseWeaponScale,
                physicSlowAdditionalDamage = physicSlowAdditionalDamage,
                increasePhysicAtkSpeed = increasePhysicAtkSpeed,
                physicIncreaseDamage = physicIncreaseDamage,
                increasePhysicsDamage = increasePhysicsDamage,
                waterBurnAdditionalDamage = waterBurnAdditionalDamage,
                waterRestraintIncreaseDamage = waterRestraintIncreaseDamage,
                waterIncreaseSlowPower = waterIncreaseSlowPower,
                increaseWaterDamage = increaseWaterDamage,
                waterSideAttack = waterSideAttack,
                poisonDoubleAtk = poisonDoubleAtk,
                poisonRestraintAdditionalDamage = poisonRestraintAdditionalDamage,
                poisonInstantKill = poisonInstantKill,
                poisonIncreaseAtkRange = poisonIncreaseAtkRange,
                activatePoison = activatePoison,
                poisonOverlapping = poisonOverlapping,
                fireBleedingAdditionalDamage = fireBleedingAdditionalDamage,
                fireIncreaseDamage = fireIncreaseDamage,
                firePoisonAdditionalStun = firePoisonAdditionalStun,
                fireIncreaseAtkRange = fireIncreaseAtkRange,
                fireDeleteBurnIncreaseDamage = fireDeleteBurnIncreaseDamage,
                physics2AdditionalBleedingLayer = physics2AdditionalBleedingLayer,
                physics2ActivateBleed = physics2ActivateBleed,
                physics2AdditionalAtkSpeed = physics2AdditionalAtkSpeed,
                physics2AdditionalProjectile = physics2AdditionalProjectile,
                physics2ProjectilePenetration = physics2ProjectilePenetration,
=======
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
                darkTenthAttackDoubleDamage = darkTenthAttackDoubleDamage,
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
                physicalDamage100Boost = physicalDamage100Boost,
                physicalDamage9Boost = physicalDamage9Boost,
                physicalBleedingChance = physicalBleedingChance,
                physicalSwordAddition = physicalSwordAddition,
                physicalSlowEnemyDamageBoost = physicalSlowEnemyDamageBoost,
                physicalSwordScaleIncrease = physicalSwordScaleIncrease,
                physicalDamage24Boost = physicalDamage24Boost,
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
                poisonAilmentStun = poisonAilmentStun,
                poisonMaxStackIncrease = poisonMaxStackIncrease,
                poisonDamageAttackPowerIncrease = poisonDamageAttackPowerIncrease,
                poisonProjectileIncrease = poisonProjectileIncrease,
                poisonRangeIncrease = poisonRangeIncrease,
                poisonBleedingEnemyDamageBoost = poisonBleedingEnemyDamageBoost,
                poisonBleedingEnemyInstantKill = poisonBleedingEnemyInstantKill,
                poisonPerHitEffect = poisonPerHitEffect,
                //Fire2 Unit G
                fire2PoisonDamageIncrease = fire2PoisonDamageIncrease,
                fire2AttackSpeedIncrease = fire2AttackSpeedIncrease,
                fire2BleedingDamageIncrease = fire2BleedingDamageIncrease,
                fire2AttackPowerIncrease = fire2AttackPowerIncrease,
                fire2StunChance = fire2StunChance,
                fire2SwordSizeIncrease = fire2SwordSizeIncrease,
                fire2RangeIncrease = fire2RangeIncrease,
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

>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            activeRestraint = data.activeRestraint;
            increaseRestraintTime = data.increaseRestraintTime;
            divinePenetrate = data.divinePenetrate;
            divineAtkRange = data.divineAtkRange;
            divinePoisonAdditionalDamage = data.divinePoisonAdditionalDamage;
            divinePoisonAdditionalDamageCount = data.divinePoisonAdditionalDamageCount;
            darkSlowAdditionalDamage = data.darkSlowAdditionalDamage;
            darkBleedAdditionalDamage = data.darkBleedAdditionalDamage;
            darkIncreaseAtkSpeed = data.darkIncreaseAtkSpeed;
            darkProjectileBounce = data.darkProjectileBounce;
            darkProjectileBounceCount = data.darkProjectileBounceCount;
            water2IncreaseDamage = data.water2IncreaseDamage;
            water2BleedAdditionalRestraint = data.water2BleedAdditionalRestraint;
            water2IncreaseSlowTime = data.water2IncreaseSlowTime;
            water2BackAttack = data.water2BackAttack;
            water2AdditionalProjectile = data.water2AdditionalProjectile;
            physicAdditionalWeapon = data.physicAdditionalWeapon;
            physicIncreaseWeaponScale = data.physicIncreaseWeaponScale;
            physicSlowAdditionalDamage = data.physicSlowAdditionalDamage;
            increasePhysicAtkSpeed = data.increasePhysicAtkSpeed;
            physicIncreaseDamage = data.physicIncreaseDamage;
            increasePhysicsDamage = data.increasePhysicsDamage;
            waterBurnAdditionalDamage = data.waterBurnAdditionalDamage;
            waterRestraintIncreaseDamage = data.waterRestraintIncreaseDamage;
            waterIncreaseSlowPower = data.waterIncreaseSlowPower;
            increaseWaterDamage = data.increaseWaterDamage;
            waterSideAttack = data.waterSideAttack;
            poisonDoubleAtk = data.poisonDoubleAtk;
            poisonRestraintAdditionalDamage = data.poisonRestraintAdditionalDamage;
            poisonInstantKill = data.poisonInstantKill;
            poisonIncreaseAtkRange = data.poisonIncreaseAtkRange;
            activatePoison = data.activatePoison;
            poisonOverlapping = data.poisonOverlapping;
            fireBleedingAdditionalDamage = data.fireBleedingAdditionalDamage;
            fireIncreaseDamage = data.fireIncreaseDamage;
            firePoisonAdditionalStun = data.firePoisonAdditionalStun;
            fireIncreaseAtkRange = data.fireIncreaseAtkRange;
            fireDeleteBurnIncreaseDamage = data.fireDeleteBurnIncreaseDamage;
            physics2AdditionalBleedingLayer = data.physics2AdditionalBleedingLayer;
            physics2ActivateBleed = data.physics2ActivateBleed;
            physics2AdditionalAtkSpeed = data.physics2AdditionalAtkSpeed;
            physics2AdditionalProjectile = data.physics2AdditionalProjectile;
            physics2ProjectilePenetration = data.physics2ProjectilePenetration;
=======
            
            // Divine
            divinePoisonDamageBoost = data.divinePoisonDamageBoost;
            divineBindDurationBoost = data.divineBindDurationBoost;
            divineShackledExplosion = data.divineShackledExplosion;
            divineFifthAttackBoost = data.divineFifthAttackBoost;
            divineAttackBoost = data.divineAttackBoost;
            divineBindChanceBoost = data.divineBindChanceBoost;                                                        
            divineDualAttack = data.divineDualAttack;
            divineProjectilePierce = data.divineProjectilePierce;
            //Darkness Unit B
            darkTenthAttackDoubleDamage = data.darkTenthAttackDoubleDamage;
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
            physicalDamage100Boost = data.physicalDamage100Boost;
            physicalDamage9Boost = data.physicalDamage9Boost;
            physicalBleedingChance = data.physicalBleedingChance;
            physicalSwordAddition = data.physicalSwordAddition;
            physicalSlowEnemyDamageBoost = data.physicalSlowEnemyDamageBoost;
            physicalSwordScaleIncrease = data.physicalSwordScaleIncrease;
            physicalDamage24Boost = data.physicalDamage24Boost;
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
            poisonAilmentStun = data.poisonAilmentStun;
            poisonMaxStackIncrease = data.poisonMaxStackIncrease;
            poisonDamageAttackPowerIncrease = data.poisonDamageAttackPowerIncrease;
            poisonProjectileIncrease = data.poisonProjectileIncrease;
            poisonRangeIncrease = data.poisonRangeIncrease;
            poisonBleedingEnemyDamageBoost = data.poisonBleedingEnemyDamageBoost;
            poisonBleedingEnemyInstantKill = data.poisonBleedingEnemyInstantKill;
            poisonPerHitEffect = data.poisonPerHitEffect;
            //Fire2 Unit G
            fire2PoisonDamageIncrease = data.fire2PoisonDamageIncrease;
            fire2AttackSpeedIncrease = data.fire2AttackSpeedIncrease;
            fire2BleedingDamageIncrease = data.fire2BleedingDamageIncrease;
            fire2AttackPowerIncrease = data.fire2AttackPowerIncrease;
            fire2StunChance = data.fire2StunChance;
            fire2SwordSizeIncrease = data.fire2SwordSizeIncrease;
            fire2RangeIncrease = data.fire2RangeIncrease;
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

>>>>>>> Stashed changes
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

