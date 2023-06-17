using Script.CharacterManagerScript;
using Script.PuzzleManagerGroup;
using Script.UIManager;
using UnityEngine;

namespace Script.RewardScript
{
    public class EnforceManager : MonoBehaviour
    {
        // Script Group
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private CountManager countManager;
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private GridManager gridManager;

        [Header("\n\n신성 속성\n")] 

        [Header("속박속성 부여")] public bool activeRestraint;
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
        

        [Header("\n\n물리 속성\n")]

        [Header("Sword 추가")] public bool physicAdditionalWeapon; 
        
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


        [Header("\n\n독 속성\n")] 
        
        
        [Header("더블어택")] public bool poisonDoubleAtk;
        
        [Header("속박된 적 추가데미지")] public bool poisonRestraintAdditionalDamage;

        [Header("15% 확률 즉사")] public bool poisonInstantKill;
        
        [Header("공격사거리 1 증가")] public bool poisonIncreaseAtkRange;

        [Header("중독활성화")] public bool activatePoison;

        [Header("중족최대 중첩수 증가")] public int poisonOverlapping = 1;
        protected internal void AddPoisonOverlapping()
        {
            poisonOverlapping += 1;
        }



        [Header("\n\n물 속성\n")] 
        
        [Header("15% 적 기절")] public bool waterStun;
        
        [Header("둔화시간 증가")] public float waterIncreaseSlowTime = 1f;
        protected internal void IncreaseSlowTime()
        {
            waterIncreaseSlowTime += 0.2f;
        }
        
        [Header("둔화강도 증가")] public bool waterIncreaseSlowPower;
        
        [Header("넉백")] public bool waterRestraintKnockBack;

        [Header("공격력증가")] protected internal float IncreaseWaterDamage = 1;
        protected internal void WaterIncreaseDamage()
        {
            IncreaseWaterDamage *= 1.2f;
        }





        [Header("\n\n공통강화\n")] [Header("가로줄 추가")]
        public int addRowCount;
        protected internal void AddRow()
        {
            addRowCount += 1;
            gridManager.AddRow();
        }
        [Header("적 이동속도 감소 15%증가 (최대 45%)")] public int slowCount;
        [Header("대각선 이동")] public bool diagonalMovement;
        [Header("Castle 체력회복 200")] public bool recoveryCastle;
        [Header("Castle 최대체력 증가 (최대 2000)")] public int castleMaxHp;
        protected internal void IncreaseCastleMaxHp()
        {
            castleMaxHp += 200;
            castleManager.IncreaseMaxHp(castleMaxHp);
        }

        [Header("보드 초기화 케릭터")] public int highLevelCharacterCount = 6;
        protected internal int SelectedCount;
        protected internal void NextCharacterUpgrade(int moveCharacterCount)
        {
            highLevelCharacterCount += moveCharacterCount;
            SelectedCount += 1;
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
            countManager.RewardMoveCount += permanentIncreaseMovementCount;
            countManager.TotalMoveCount = countManager.BaseMoveCount + countManager.RewardMoveCount;
            countManager.UpdateMoveCountText();
        }
       
        [Header("5매치 가운데 유닛 추가 레벨증가")] public bool match5Upgrade ;
        [Header("전체 공격력 증가 (%)")] public int increaseAtkDamage;
        protected internal void IncreaseGroupDamage(int increaseAmount)
        {
            increaseAtkDamage += increaseAmount;
            foreach (var character in characterManager.characterList)
            {
                character.IncreaseDamage();
            }
        }

        [Header("전체 공격속도 증가 (%)")] public int increaseAtkRate;
        protected internal void IncreaseGroupRate(int increaseRateAmount)
        {
            increaseAtkRate += increaseRateAmount;
            foreach (var character  in characterManager.characterList)
            {
                   character.IncreaseAtkRate();
            }
        }
    }                                                                   
}
