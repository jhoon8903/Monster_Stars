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
        //Divine Unit A
        public bool divinePoisonDamageBoost;
        public bool divineBindDurationBoost;
        public bool divineShackledExplosion;
        public bool divineFifthAttackBoost;
        public float divineRateBoost;
        public bool divineBindChanceBoost;
        public bool divineDualAttack;

        //Darkness Unit B
        public bool darkFifthAttackDamageBoost;
        public float darkAttackSpeedBoost;
        public float darkAttackPowerBoost;
        public bool darkKnockBackChance;
        public bool darkStatusAilmentDamageBoost;
        public bool darkRangeIncrease;
        public bool darkStatusAilmentSlowEffect;

        //Water1 Unit C
        public bool waterFreeze;
        public bool waterFreezeChance;
        public float waterSlowDurationBoost;
        public bool waterFreezeDamageBoost;
        public bool waterSlowCPowerBoost;
        public float waterAttackRateBoost;
        public bool waterGlobalFreeze;

        //Physical Unit D
        public bool physicalSwordScaleIncrease;
        public bool physicalSwordAddition;
        public float physicalAttackSpeedBoost;
        public bool physicalRatePerAttack;
        public bool physicalBindBleed;
        public float physicalDamageBoost;
        public bool physicalBleedDuration;

        //Water2 Unit E
        public bool water2Freeze;
        public bool water2SlowPowerBoost;
        public bool water2FreezeTimeBoost;
        public float water2DamageBoost;
        public bool water2FreezeChanceBoost;
        public bool water2FreezeDamageBoost;
        public float water2SlowTimeBoost;

        //Poison Unit F
        public bool poisonPerHitEffect;
        public bool poisonBleedingEnemyDamageBoost;
        public bool poisonDamagePerBoost;
        public bool poisonDamageBoost;
        public bool poisonDotDamageBoost;
        public float poisonAttackSpeedIncrease;
        public bool poisonDurationBoost;

        //Fire2 Unit G
        public bool fire2FreezeDamageBoost;
        public bool fire2BurnDurationBoost;
        public bool fire2ChangeProperty;
        public float fire2DamageBoost;
        public bool fire2RangeBoost;
        public bool fire2RateBoost;
        public bool fire2BossDamageBoost;


        //Fire1 Unit H
        public bool fireBurnPerAttackEffect;
        public bool fireStackOverlap;
        public bool fireProjectileBounceDamage;
        public bool fireBurnedEnemyExplosion;
        public float fireAttackSpeedBoost;
        public bool fireProjectileSpeedIncrease;
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
        [RuntimeInitializeOnLoadMethod] private static void InitializeOnLoad()
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
        [Header("Blue / 1Lv: 5회 공격마다 100% 추가데미지 (투사체 컬러 변경)")] 
        public bool divineFifthAttackBoost;
        // 완료
        [Header("Purple / 3Lv: 앞 뒤 동시공격")]
        public bool divineDualAttack; 
        // 완료
        [Header("Green / 5Lv: 속박지속시간 0.5초 증가")] 
        public bool divineBindDurationBoost;
        // 완료
        [Header("Blue / 7Lv: 적 제거시 주변 1칸 범위의 100% 폭발데미지 추가")] 
        public bool divineShackledExplosion;
        // 완료
        [Header("Green / 9Lv: 중독된 적 추가데미지 25%")] 
        public bool divinePoisonDamageBoost;
        // 완료 
        [Header("Purple / Lv11: 속박확률 20% 증가 (30% > 50%)")] 
        public bool divineBindChanceBoost;
        // 완료
        [Header("Blue / 13Lv: 공격속도 9% 증가 (최대 4회)")] 
        public float divineRateBoost; 
        protected internal void DivineRateBoost()
        {   
            if (divineRateBoost >= 0.36f) return;
            divineRateBoost+= 0.09f;
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
        [Header("Purple / 7Lv: C 유닛에게 빙결된 적은 받는 피해 15% 증가")]
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
        [Header("Purple / 13Lv: 퍼즐위 모든 C 유닛의 공격 횟수의 합이 100이면 될 때마다 눈보라를 일으켜 보스를 제외한 모든 적을 빙결")]
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
        // 
        [Header("Green / 11Lv: 투사체 속도가 100% 증가, 반드시 명중")] 
        public bool fireProjectileSpeedIncrease;


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
        public float increaseAtkRate; 
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
        public List<int> permanentGroupIndex = new List<int>();

        public void PermanentIncreaseCharacter(int characterListIndex)
        {
            // 이미 해당 characterListIndex가 리스트에 있는지 확인
            if (!permanentGroupIndex.Contains(characterListIndex))
            {
                // 아직 적용되지 않았으면 리스트에 추가
                permanentGroupIndex.Add(characterListIndex);
            }
            else
            {
                // 이미 적용되었으므로 아무것도 하지 않고 함수 종료
                return;
            }
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
                divineRateBoost = divineRateBoost,
                divineBindChanceBoost = divineBindChanceBoost,
                divineDualAttack = divineDualAttack,
                //Darkness Unit B
                darkFifthAttackDamageBoost = darkFifthAttackDamageBoost,
                darkAttackSpeedBoost = darkAttackSpeedBoost,                        
                darkAttackPowerBoost = darkAttackPowerBoost,
                darkKnockBackChance = darkKnockBackChance,
                darkStatusAilmentDamageBoost = darkStatusAilmentDamageBoost,
                darkRangeIncrease = darkRangeIncrease,
                darkStatusAilmentSlowEffect = darkStatusAilmentSlowEffect,
                //Water1 Unit C
                waterFreeze = waterFreeze,
                waterFreezeChance = waterFreezeChance,
                waterSlowDurationBoost = waterSlowDurationBoost,
                waterFreezeDamageBoost = waterFreezeDamageBoost,
                waterSlowCPowerBoost =  waterSlowCPowerBoost,
                waterAttackRateBoost = waterAttackRateBoost,
                waterGlobalFreeze = waterGlobalFreeze, 
                //Physical Unit D
                physicalAttackSpeedBoost = physicalAttackSpeedBoost,
                physicalSwordAddition = physicalSwordAddition,
                physicalSwordScaleIncrease = physicalSwordScaleIncrease,
                physicalRatePerAttack = physicalRatePerAttack, 
                physicalBindBleed = physicalBindBleed,
                physicalDamageBoost = physicalDamageBoost,
                physicalBleedDuration = physicalBleedDuration,
                //Water2 Unit E
                water2Freeze = water2Freeze,
                water2SlowPowerBoost = water2SlowPowerBoost,
                water2FreezeTimeBoost = water2FreezeTimeBoost,
                water2DamageBoost = water2DamageBoost,
                water2FreezeChanceBoost = water2FreezeChanceBoost,
                water2FreezeDamageBoost = water2FreezeDamageBoost,
                water2SlowTimeBoost = water2SlowTimeBoost,
                //Poison Unit F
                poisonPerHitEffect = poisonPerHitEffect,
                poisonBleedingEnemyDamageBoost = poisonBleedingEnemyDamageBoost,
                poisonDamagePerBoost = poisonDamagePerBoost,
                poisonDamageBoost = poisonDamageBoost,
                poisonDotDamageBoost = poisonDotDamageBoost,
                poisonAttackSpeedIncrease = poisonAttackSpeedIncrease,
                poisonDurationBoost = poisonDurationBoost,
                //Fire2 Unit G
                fire2FreezeDamageBoost = fire2FreezeDamageBoost,
                fire2BurnDurationBoost = fire2BurnDurationBoost,
                fire2ChangeProperty = fire2ChangeProperty,
                fire2DamageBoost =  fire2DamageBoost,
                fire2RangeBoost = fire2RangeBoost,
                fire2RateBoost = fire2RateBoost, 
                fire2BossDamageBoost = fire2BossDamageBoost,  

                //Fire1 Unit H
                fireBurnPerAttackEffect = fireBurnPerAttackEffect,
                fireStackOverlap = fireStackOverlap,
                fireProjectileBounceDamage = fireProjectileBounceDamage,
                fireBurnedEnemyExplosion = fireBurnedEnemyExplosion,
                fireAttackSpeedBoost = fireAttackSpeedBoost,
                fireProjectileSpeedIncrease = fireProjectileSpeedIncrease,
                fireProjectileBounceIncrease = fireProjectileBounceIncrease,

                // Common
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
            divineBindChanceBoost = data.divineBindChanceBoost;                                                        
            divineDualAttack = data.divineDualAttack;
            divineRateBoost = data.divineRateBoost;
            //Darkness Unit B
            darkFifthAttackDamageBoost = data.darkFifthAttackDamageBoost;
            darkAttackSpeedBoost = data.darkAttackSpeedBoost;
            darkAttackPowerBoost = data.darkAttackPowerBoost;
            darkKnockBackChance = data.darkKnockBackChance;
            darkStatusAilmentDamageBoost = data.darkStatusAilmentDamageBoost;
            darkRangeIncrease = data.darkRangeIncrease;
            darkStatusAilmentSlowEffect = data.darkStatusAilmentSlowEffect;
            //Water1 Unit C
            waterFreeze = data.waterFreeze;
            waterFreezeChance = data.waterFreezeChance;
            waterSlowDurationBoost = data.waterSlowDurationBoost;
            waterFreezeDamageBoost = data.waterFreezeDamageBoost;
            waterSlowCPowerBoost =  data.waterSlowCPowerBoost;
            waterAttackRateBoost = data.waterAttackRateBoost;
            waterGlobalFreeze = data.waterGlobalFreeze;
            //Physical Unit D
            physicalAttackSpeedBoost = data.physicalAttackSpeedBoost;
            physicalSwordAddition = data.physicalSwordAddition;
            physicalSwordScaleIncrease = data.physicalSwordScaleIncrease;
            physicalRatePerAttack = data.physicalRatePerAttack; 
            physicalBindBleed = data.physicalBindBleed;
            physicalDamageBoost = data.physicalDamageBoost;
            physicalBleedDuration = data.physicalBleedDuration;
            //Water2 Unit E
            water2Freeze = data.water2Freeze;
            water2SlowPowerBoost = data.water2SlowPowerBoost;
            water2FreezeTimeBoost = data.water2FreezeTimeBoost;
            water2DamageBoost = data.water2DamageBoost;
            water2FreezeChanceBoost = data.water2FreezeChanceBoost;
            water2FreezeDamageBoost = data.water2FreezeDamageBoost;
            water2SlowTimeBoost = data.water2SlowTimeBoost;
            //Poison Unit F
            poisonPerHitEffect = data.poisonPerHitEffect;
            poisonBleedingEnemyDamageBoost = data.poisonBleedingEnemyDamageBoost;
            poisonDamagePerBoost = data.poisonDamagePerBoost;
            poisonDamageBoost = data.poisonDamageBoost;
            poisonDotDamageBoost = data.poisonDotDamageBoost;
            poisonAttackSpeedIncrease = data.poisonAttackSpeedIncrease;
            poisonDurationBoost = data.poisonDurationBoost;
            //Fire2 Unit G
            fire2FreezeDamageBoost = data.fire2FreezeDamageBoost;
            fire2BurnDurationBoost = data.fire2BurnDurationBoost;
            fire2ChangeProperty = data.fire2ChangeProperty;
            fire2DamageBoost =  data.fire2DamageBoost;
            fire2RangeBoost = data.fire2RangeBoost;
            fire2RateBoost = data.fire2RateBoost; 
            fire2BossDamageBoost = data.fire2BossDamageBoost;  
            //Fire1 Unit H
            fireBurnPerAttackEffect = data.fireBurnPerAttackEffect;
            fireStackOverlap = data.fireStackOverlap;
            fireProjectileBounceDamage = data.fireProjectileBounceDamage;
            fireBurnedEnemyExplosion = data.fireBurnedEnemyExplosion;
            fireAttackSpeedBoost = data.fireAttackSpeedBoost;
            fireProjectileSpeedIncrease = data.fireProjectileSpeedIncrease;                            
            fireProjectileBounceIncrease = data.fireProjectileBounceIncrease;
            // Common
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

