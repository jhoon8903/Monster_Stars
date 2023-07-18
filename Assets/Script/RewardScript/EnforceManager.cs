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
        public bool activeRestraint;
        public float increaseRestraintTime;
        public bool divinePenetrate;
        public bool divineAtkRange;
        public bool divinePoisonAdditionalDamage;
        public int divinePoisonAdditionalDamageCount;
        public bool darkSlowAdditionalDamage;
        public bool darkBleedAdditionalDamage;
        public int darkIncreaseAtkSpeed;
        public bool darkProjectileBounce;
        public int darkProjectileBounceCount;
        public int water2IncreaseDamage;
        public bool water2BleedAdditionalRestraint;
        public int water2IncreaseSlowTime;
        public bool water2BackAttack;
        public bool water2AdditionalProjectile;
        public bool physicAdditionalWeapon;
        public bool physicIncreaseWeaponScale;
        public bool physicSlowAdditionalDamage;
        public float increasePhysicAtkSpeed;
        public bool physicIncreaseDamage;
        public float increasePhysicsDamage;
        public bool waterBurnAdditionalDamage;
        public bool waterRestraintIncreaseDamage;
        public bool waterIncreaseSlowPower;
        public float increaseWaterDamage;
        public bool waterSideAttack;
        public bool poisonDoubleAtk;
        public bool poisonRestraintAdditionalDamage;
        public bool poisonInstantKill;
        public bool poisonIncreaseAtkRange;
        public bool activatePoison;
        public int poisonOverlapping;
        public bool fireBleedingAdditionalDamage;
        public int fireIncreaseDamage;
        public bool firePoisonAdditionalStun;
        public bool fireIncreaseAtkRange;
        public bool fireDeleteBurnIncreaseDamage;
        public int physics2AdditionalBleedingLayer;
        public bool physics2ActivateBleed;
        public int physics2AdditionalAtkSpeed;
        public bool physics2AdditionalProjectile;
        public bool physics2ProjectilePenetration;
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

        [Header("\n\nUnit_A 신성 속성\n")] [Header("속박 활성화")]
        public bool activeRestraint;

        [Header("속박시간 증가")] public float increaseRestraintTime = 0.1f;

        protected internal float IncreaseRestraintTime()
        {
            var restraintTime = 1f;
            restraintTime += increaseRestraintTime;
            return restraintTime;
        }

        [Header("관통 효과")] public bool divinePenetrate;
        [Header("백 어택")] public bool divineAtkRange;
        [Header("중독 시 데미지증가")] public bool divinePoisonAdditionalDamage;
        public int divinePoisonAdditionalDamageCount = 1;

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

        [Header("백 어택")] public bool water2BackAttack;
        [Header("투사체 추가")] public bool water2AdditionalProjectile;

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

