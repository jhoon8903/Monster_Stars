using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.UIManager;
using UnityEngine;

namespace Script.RewardScript
{
    public class EnforceManager : MonoBehaviour
    {
        // Script Group
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private CharacterPool characterPool;
        [SerializeField] private CountManager countManager;

        [Header("신성 속성")]

        [Header("속박시간 증가")] public bool increaseRestraintTime = false; 
        [Header("관통 효과")] public bool divinePenetrate = false; 
        [Header("백 어택")] public bool divineAtkRange = false; 
        [Header("중독 시 데미지증가")] public bool divinePoisonAdditionalDamage = false;
     
        [Header("\n\n물리 속성")]

        [Header("Sword 추가")] public bool physicAdditionalWeapon = false; 
        [Header("Sword 크기 2배")] public bool physicIncreaseWeaponScale = false;
        [Header("둔화 시 데미지추가")] public bool physicSlowAdditionalDamage = false;
        [Header("공격속도 증가")] public bool physicAtkSpeed = false;
        [Header("데미지 증가")] public bool physicIncreaseDamage = false;

        [Header("\n\n독 속성")] 
        
        [Header("더블어택")] public bool poisonDoubleAtk = false;
        [Header("속박된 적 추가데미지")] public bool poisonRestraintAdditionalDamage = false;
        [Header("중독시간 증가")] public bool poisonIncreaseTime = false;
        [Header("15% 확률 즉사")] public bool poisonInstantKill = false;
        [Header("공격사거리 1 증가")] public bool poisonIncreaseAtkRange = false;

        [Header("\n\n물 속성")] 
        
        [Header("15% 적 기절")] public bool waterStun = false;
        [Header("둔화시간 증가")] public bool waterIncreaseSlowTime = false;
        [Header("둔화강도 증가")] public bool waterIncreaseSlowPower = false;
        [Header("넉백")] public bool waterRestraintKnockBack = false;
        [Header("공격력증가")] public bool waterIncreaseDamage = false;

        [Header("\n\n공통강화")] 

        [Header("적 이동속도 감소 15%증가 (최대 45%)")] public int slowCount = 0;
        
        [Header("대각선 이동")] public bool diagonalMovement = false;
        
        [Header("Castle 체력회복 200")] public bool recoveryCastle = false;
        
        [Header("Castle 최대체력 증가")] public int castleMaxHp = 0;
        
        [Header("보드 초기화 케릭터")] public int nextStageMembersSelectCount = 0;
        
        [Header("경험치 증가량")] public int expPercentage = 0;
       
        [Header("최대 이동횟수 증가")] public bool permanentIncreaseMovementCount = false;
        protected internal void PermanentIncreaseMoveCount(int increaseStepAmount)
        {
            countManager.RewardMoveCount += increaseStepAmount;
            countManager.TotalMoveCount = countManager.BaseMoveCount + countManager.RewardMoveCount;
            countManager.UpdateMoveCountText();
        }
       
        [Header("5매치 가운데 유닛 추가 레벨증가")] public bool match5Upgrade = false;
       
        [Header("전체 공격력 증가 (%)")] public int increaseAtkDamage = 0;
        protected internal void IncreaseGroupDamage(int increaseAmount)
        {
            increaseAtkDamage += increaseAmount;
            foreach (var character in characterManager.characterList)
            {
                character.IncreaseDamage();
            }
        }

        [Header("전체 공격속도 증가 (%)")] public int increaseAtkRate = 0;
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
