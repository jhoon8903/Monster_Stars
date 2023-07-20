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
        public float divineBindDurationBoost;
        public bool divineShackledExplosion;
        public bool divineFifthAttackBoost;
        public int divineAttackBoost;
        public bool divineBindChanceBoost;
        public bool divineDualAttack;
        public bool divineProjectilePierce;

        //Darkness Unit B
        public bool darkTenthAttackDoubleDamage;
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
        public bool physicalDamage100Boost;
        public float physicalDamage9Boost;
        public float physicalBleedingChance;
        public bool physicalSwordAddition;
        public bool physicalSlowEnemyDamageBoost;
        public bool physicalSwordScaleIncrease;
        public float physicalDamage24Boost;

        //Water2 Unit E
        public int water2DebuffDurationIncrease;
        public int water2AttackSpeedIncrease;
        public bool water2StunChanceAgainstBleeding;
        public bool water2IceSpikeProjectile;
        public int water2AttackPowerIncrease;
        public bool water2ProjectileSpeedIncrease;
        public float water2DebuffStrengthIncrease;
        public bool water2AttackSpeedBuffToAdjacentAllies;

        //Poison Unit F
        public bool poisonAilmentStun;
        public int poisonMaxStackIncrease;
        public int poisonDamageAttackPowerIncrease;
        public bool poisonProjectileIncrease;
        public bool poisonRangeIncrease;
        public bool poisonBleedingEnemyDamageBoost;
        public bool poisonBleedingEnemyInstantKill;
        public bool poisonPerHitEffect;

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
        public float fireBurnPerAttackEffect;
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

        [Header("\n\nA 신성: Blue\n")] 
        [Header("Green / Default: 중독된 적 추가데미지 50%")] public bool divinePoisonDamageBoost;
        [Header("Green / 7Lv: 속박지속시간 0.1초씩 증가 (최대 0.5초 / 5회)")] public float divineBindDurationBoost; protected internal void DivineBindDurationIncrease()
        {
            if (divineBindDurationBoost >= 0.5f) return;
            divineBindDurationBoost += 0.1f;
        }
        [Header("Green / 3Lv: 속박된 적 공격시 적 제거시 주변 1칸 범위의 100% 폭발데미지 추가")] public bool divineShackledExplosion;
        [Header("Blue / Default: 5회 공격마다 100% 추가데미지 (투사체 컬러 변경)")] public bool divineFifthAttackBoost;
        [Header("Blue / 13Lv: 공격력 16% 증가 (최대 6회)")] public int divineAttackBoost; protected internal void DivineAttackDamageIncrease()
        {   
            if (divineAttackBoost >=6) return;
            divineAttackBoost++;
        }
        [Header("Purple / Lv5: 속박확률 20% 증가 (20% > 40%)")] public bool divineBindChanceBoost;
        [Header("Purple / 11Lv: 백어텍 가능")] public bool divineDualAttack;
        [Header("Purple / 9Lv: 관통 가능 (1회)")] public bool divineProjectilePierce;
        

        [Header("\n\nB 어둠: Green\n")]
        [Header("Green / 5Lv: 10회 공격마다 100% 추가 데미지 (투사체 컬러 변경)")] public bool darkTenthAttackDoubleDamage;
        [Header("Green / Default: 공격속도 9% 증가 (최대 8회)")] public int darkAttackSpeedBoost; protected internal void DarkAttackSpeedIncrease()
        {
            if(darkAttackSpeedBoost >= 8) return;
            darkAttackSpeedBoost++;
        }
        [Header("Green / 9Lv: 공격력 9% 증가 (최대 6회)")] public int darkAttackPowerBoost; protected internal void DarkAttackDamageIncrease()
        {
            if (darkAttackPowerBoost >= 6) return;
            darkAttackPowerBoost++;
        }
        [Header("Blue / 11Lv: 상태이상 적 공격시 5% 확률로 1000% 추가데미지")] public bool darkStatusAilmentDamageChance;
        [Header("Blue / 3Lv: 10% 확률로 적 밀침 (1칸)")] public bool darkKnockBackChance;
        [Header("Blue / Default: 상태이상 적 공격시 20% 추가데미지")] public bool darkStatusAilmentDamageBoost;
        [Header("Purple / 7Lv: 사거리 1 증가")] public bool darkRangeIncrease;
        [Header("Purple / 13Lv: 상태이상 적 공격시 1초 이동속도 20% 감소")] public bool darkStatusAilmentSlowEffect;

        [Header("\n\nC 물: Purple\n")]
        [Header("Green / 5Lv: 공격속도 7% 증가 (최대 8회)")] public int waterAttackSpeedBoost; protected internal void WaterAttackSpeedIncrease()
        {
            if (waterAttackSpeedBoost >= 8) return;
            waterAttackSpeedBoost++;
        }
        [Header("Green / Default: 같은 속성(물) 유닛 존재시 둔화 비활성화 500% 추가데미지")] public bool waterAllyDamageBoost;
        [Header("Blue / 11Lv: 발사체의 갯수가 3개로 변화합니다.")] public bool waterProjectileIncrease;
        [Header("Blue / 13Lv: 공격력 13% 증가 (최대 6회)")] public int waterAttackBoost; protected internal void WaterAttackDamageIncrease()
        {
            if (waterAttackBoost >= 6) return;
            waterAttackBoost++;
        }
        [Header("Blue / 7Lv: 둔화 적 공격 시 20% 추가데미지")] public bool waterSlowEnemyDamageBoost;
        [Header("Purple / Default: 80회 공격시 모든적 20% 둔화 1초 (웨이브마다 초기화)")] public bool waterGlobalSlowEffect;
        [Header("Purple / 3Lv: 둔화상태의 적 공격시 20% 확률로 0.5초간 기절")] public bool waterSlowEnemyStunChance;
        [Header("Purple / 9Lv: 피격당한 적은 5초간 받는데미지 20% 증가")] public bool waterDamageIncreaseDebuff;




        [Header("\n\n공통강화\n")] 
        [Header("가로줄 추가")] public int addRowCount; public delegate void AddRowDelegate(); public event AddRowDelegate OnAddRow; protected internal void AddRow()
        {
            addRowCount += 1;
            gridManager.AddRow();
            OnAddRow?.Invoke();
        }
        [Header("적 이동속도 감소 15%증가 (최대 45%)")] public int slowCount; protected internal void SlowCount()
        {
            slowCount++;
            if (slowCount >= 4)
            {
                slowCount = 4;
            }
        }
        [Header("대각선 이동")] public bool diagonalMovement;
        [Header("Castle 체력회복 200")] public bool recoveryCastle;
        [Header("Castle 최대체력 증가 (최대 2000)")] public float castleMaxHp; protected internal void IncreaseCastleMaxHp()
        {
            castleMaxHp += 200f;
            castleManager.IncreaseMaxHp();
        }
        [Header("보드 초기화 케릭터")] public int highLevelCharacterCount = 6; public int selectedCount; protected internal void NextCharacterUpgrade(int moveCharacterCount)
        {
            highLevelCharacterCount += moveCharacterCount;
            selectedCount += 1;
        }
        [Header("경험치 증가량")] public int expPercentage; protected internal void IncreaseExpBuff(int increaseAmount)
        {
            expPercentage += increaseAmount;
        }
        [Header("최대 이동횟수 증가")] public int permanentIncreaseMovementCount; protected internal void PermanentIncreaseMoveCount(int increaseStepAmount)
        {
            permanentIncreaseMovementCount += increaseStepAmount;
        }
        [Header("5매치 가운데 유닛 추가 레벨증가")] public bool match5Upgrade;
        [Header("전체 공격력 증가 (%)")] public float increaseAtkDamage = 1f; protected internal void IncreaseGroupDamage(int increaseAmount)
        {
            increaseAtkDamage += increaseAmount;
        }
        [Header("전체 공격속도 증가 (%)")] public float increaseAtkRate = 1f; protected internal void IncreaseGroupRate(float increaseRateAmount)
        {
            increaseAtkRate += increaseRateAmount;
        }
        [Header("이동횟수 추가")] public int rewardMoveCount; protected internal void RewardMoveCount(int moveCount)
        {
            rewardMoveCount += moveCount;
        }
        [Header("추가코인")] public bool addGold; public int addGoldCount; protected internal void AddGold()
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

