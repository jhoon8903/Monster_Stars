using Script.PuzzleManagerGroup;
using Script.UIManager;
using UnityEngine;

namespace Script.RewardScript
{
    public class EnforceManager : MonoBehaviour
    {
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private GridManager gridManager;

        public static EnforceManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        [Header("\n\nUnit_A 신성 속성\n")]
        [Header("속박 활성화")] public bool activeRestraint;
        [Header("속박시간 증가")] public float increaseRestraintTime = 0.1f; protected internal float IncreaseRestraintTime()
        {
            var restraintTime = 1f;
            restraintTime += increaseRestraintTime;
            return restraintTime;
        }
        [Header("관통 효과")] public bool divinePenetrate;
        [Header("백 어택")] public bool divineAtkRange;
        [Header("중독 시 데미지증가")] public bool divinePoisonAdditionalDamage; public int divinePoisonAdditionalDamageCount = 1;


        [Header("\n\n Unit_B 어둠 속성\n\n")] 
        [Header("둔화상태 적 추가 데미지")]  public bool darkSlowAdditionalDamage;
        [Header("출혈상태 적 추가 데미지")] public bool darkBleedAdditionalDamage;
        [Header("공격속도 17% 상승 ")] public int darkIncreaseAtkSpeed; protected internal void DarkIncreaseAtkSpeed()
        {
            darkIncreaseAtkSpeed ++;
        } 
        [Header("관통 효과")] public bool darkProjectilePenetration ;
        // [Header("공격방향 추가")] public bool darkAdditionalFrontAttack;


        [Header("\n\nUnit_C 물 속성2\n\n")] 
        [Header("공격력 증가")] public int water2IncreaseDamage; protected internal void Water2IncreaseDamage()
        {
            water2IncreaseDamage++;
        }
        [Header("출혈상태 적 공격시 속박")] public bool water2BleedAdditionalRestraint;
        [Header("둔화지속시간 증가")] public int water2IncreaseSlowTime; protected internal void Water2IncreaseSlowTime()
        {
            water2IncreaseSlowTime++;
        }
        [Header("백 어택")] public bool water2BackAttack;
        [Header("투사체 추가")] public bool water2AdditionalProjectile;


        [Header("\n\nUnit_D 물리 속성\n")]
        [Header("Sword 추가")] public bool physicAdditionalWeapon;
        [Header("Sword 크기 2배")] public bool physicIncreaseWeaponScale;
        [Header("둔화 시 데미지추가")] public bool physicSlowAdditionalDamage;
        [Header("공격속도 증가")] public float increasePhysicAtkSpeed = 1f; protected internal void IncreasePhysicAtkSpeed()
        {
            increasePhysicAtkSpeed += 0.2f;
        }
        [Header("데미지 증가")] public bool physicIncreaseDamage; public float increasePhysicsDamage = 1f; protected internal void PhysicIncreaseDamage()
        {
            increasePhysicsDamage += 0.05f;
        }



        [Header("\n\nUnit_E 물 속성\n")] 
        [Header("화상상태 적 추가데미지")] public bool waterBurnAdditionalDamage;
        [Header("속박상태 적 추가데미지")] public bool waterRestraintIncreaseDamage;
        [Header("둔화강도 증가")] public bool waterIncreaseSlowPower;
        [Header("공격력증가")] protected internal float IncreaseWaterDamage = 1f; protected internal void WaterIncreaseDamage()
        {
            IncreaseWaterDamage *= 1.2f;
        }
        [Header("좌/우 동시 공격")] public bool waterSideAttack;


        [Header("\n\nUnit_F 독 속성\n")]
        [Header("더블어택")] public bool poisonDoubleAtk;
        [Header("속박된 적 추가데미지")] public bool poisonRestraintAdditionalDamage;
        [Header("15% 확률 즉사")] public bool poisonInstantKill;
        [Header("공격사거리 1 증가")] public bool poisonIncreaseAtkRange;
        [Header("중독활성화")] public bool activatePoison;
        [Header("중족최대 중첩수 증가")] public int poisonOverlapping = 1; protected internal void AddPoisonOverlapping()
        {
            poisonOverlapping += 1;
        }



        [Header("\n\nUnit_G 불 속성\n\n")] 
        [Header("출혈상태 적 추가 데미지")] public bool fireBleedingAdditionalDamage;
        [Header("공격력 증가")] public int fireIncreaseDamage; protected internal void FireIncreaseDamage()
        {
            fireIncreaseDamage++;
        }
        [Header("중독상태 적 기절")] public bool firePoisonAdditionalStun;
        [Header("공격사거리 1 증가")] public bool fireIncreaseAtkRange;
        [Header("화상효과 비활성화 => 추가 데미지")] public bool fireDeleteBurnIncreaseDamage;



        [Header("\n\nUnit_H 물리 속성\n\n")] 
        [Header("출혈중첩 증가")] public int physics2AdditionalBleedingLayer = 1; protected internal void Physics2AdditionalBleedingLayer()
        {
            physics2AdditionalBleedingLayer++;

        }
        [Header("출혈 활성화")] public bool physics2ActivateBleed;
        [Header("공격속도 증가")] public int physics2AdditionalAtkSpeed; protected internal void Physics2AdditionalAtkSpeed()
        {
            physics2AdditionalAtkSpeed++;
        }
        [Header("투사체 추가")] public bool physics2AdditionalProjectile;
        [Header("관통 효과")] public bool physics2ProjectilePenetration;                                                                                                 




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
        [Header("적 이동속도 감소 15%증가 (최대 45%)")] public int slowCount;
        protected internal int SlowCount()
        {
            if (slowCount >= 3)
            {
                return slowCount = 3;
            }
            return slowCount;
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
        }
       
        [Header("5매치 가운데 유닛 추가 레벨증가")] public bool match5Upgrade ;
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
    }                                                                   
}
