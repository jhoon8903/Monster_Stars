using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.UIManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Script.RewardScript
{
    public class LevelUpRewardManager : MonoBehaviour
    {
        [SerializeField] public GameObject levelUpRewardPanel; // 보물 패널
        [SerializeField] private TextMeshProUGUI exp1Text; // 파워1 텍스트
        [SerializeField] private TextMeshProUGUI exp2Text; // 파워2 텍스트
        [SerializeField] private TextMeshProUGUI exp3Text; // 파워3 텍스트
        [SerializeField] private Button exp1Button; 
        [SerializeField] private Button exp2Button; 
        [SerializeField] private Button exp3Button;
        [SerializeField] private Image exp1BtnBadge;
        [SerializeField] private Image exp2BtnBadge;
        [SerializeField] private Image exp3BtnBadge;
        [SerializeField] private TextMeshProUGUI exp1Code; // 파워1 코드 텍스트
        [SerializeField] private TextMeshProUGUI exp2Code; // 파워2 코드 텍스트
        [SerializeField] private TextMeshProUGUI exp3Code; // 파워3 코드 텍스트
        [SerializeField] internal Sprite greenSprite; // 녹색 버튼 스프라이트
        [SerializeField] internal Sprite blueSprite; // 파란색 버튼 스프라이트
        [SerializeField] internal Sprite purpleSprite; // 보라색 버튼 스프라이트
        [SerializeField] private Exp exp; // 경험치
        [SerializeField] private ExpManager expManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CharacterManager characterManager;
        private ExpData _selectedPowerUp;
        private void ProcessExpReward(ExpData selectedReward)
        {
            switch (selectedReward.Type)
            {
                case ExpData.Types.GroupDamage: 
                    EnforceManager.Instance.IncreaseGroupDamage(selectedReward.Property[0]);
                    break;
                case ExpData.Types.GroupAtkSpeed:
                    EnforceManager.Instance.IncreaseGroupRate(selectedReward.Property[0]);
                    break;
                case ExpData.Types.StepLimit:
                    EnforceManager.Instance.PermanentIncreaseMoveCount(selectedReward.Property[0]);
                    break;
                case ExpData.Types.StepDirection:
                    EnforceManager.Instance.diagonalMovement = true; 
                    break;
                case ExpData.Types.Exp:
                    EnforceManager.Instance.IncreaseExpBuff(selectedReward.Property[0]);
                    break;
                case ExpData.Types.CastleRecovery:
                    EnforceManager.Instance.recoveryCastle = true;
                    break;
                case ExpData.Types.CastleMaxHp:
                    EnforceManager.Instance.IncreaseCastleMaxHp();
                    break;
                case ExpData.Types.Slow:
                    EnforceManager.Instance.slowCount += 1;
                    break;
                case ExpData.Types.NextStage:
                    EnforceManager.Instance.NextCharacterUpgrade(selectedReward.Property[0]);
                    break;
                case ExpData.Types.Gold:
                    characterManager.goldGetMore = true; 
                    Debug.LogWarning($"Unhandled reward type: {selectedReward.Type}");
                    break;
                case ExpData.Types.DivineActiveRestraint:
                    EnforceManager.Instance.activeRestraint = true;
                    break;
                case ExpData.Types.DivineRestraintTime:
                    EnforceManager.Instance.increaseRestraintTime += 0.1f;
                    break;
                case ExpData.Types.DivinePenetrate:
                    EnforceManager.Instance.divinePenetrate = true;
                    break;
                case ExpData.Types.DivineAtkRange:
                    EnforceManager.Instance.divineAtkRange = true;
                    break;
                case ExpData.Types.DivinePoisonAdditionalDamage:
                    EnforceManager.Instance.divinePoisonAdditionalDamage = true;
                    break;
                // 칼 추가 확인 완료
                case ExpData.Types.PhysicAdditionalWeapon:
                    EnforceManager.Instance.physicAdditionalWeapon = true;
                    break;
                case ExpData.Types.PhysicIncreaseWeaponScale:
                    EnforceManager.Instance.physicIncreaseWeaponScale = true;
                    break;
                // 둔화 적 추가 데미지 적용
                case ExpData.Types.PhysicSlowAdditionalDamage:
                    EnforceManager.Instance.physicSlowAdditionalDamage = true;
                    break;
                case ExpData.Types.PhysicAtkSpeed:
                    EnforceManager.Instance.IncreasePhysicAtkSpeed();
                    break;
                case ExpData.Types.PhysicIncreaseDamage:
                    EnforceManager.Instance.PhysicIncreaseDamage();
                    break;
                case ExpData.Types.PoisonDoubleAtk:
                    EnforceManager.Instance.poisonDoubleAtk = true;
                    break;
                case ExpData.Types.PoisonRestraintAdditionalDamage:
                    EnforceManager.Instance.poisonRestraintAdditionalDamage = true;
                    break;
                case ExpData.Types.PoisonActivate:
                    EnforceManager.Instance.activatePoison = true;
                    break;
                case ExpData.Types.PoisonInstantKill:
                    EnforceManager.Instance.poisonInstantKill = true;
                    break;
                case ExpData.Types.PoisonIncreaseAtkRange:
                    EnforceManager.Instance.poisonIncreaseAtkRange = true;
                    break;
                case ExpData.Types.PoisonOverlapping:
                    EnforceManager.Instance.AddPoisonOverlapping();
                    break;
                case ExpData.Types.WaterStun:
                    EnforceManager.Instance.waterStun = true;
                    break;
                case ExpData.Types.WaterIncreaseSlowTime:
                    EnforceManager.Instance.IncreaseSlowTime();
                    break;
                case ExpData.Types.WaterIncreaseSlowPower:
                    EnforceManager.Instance.waterIncreaseSlowPower = true;
                    break;
                case ExpData.Types.WaterRestraintKnockBack:
                    EnforceManager.Instance.waterRestraintKnockBack = true;
                    break;
                case ExpData.Types.WaterIncreaseDamage:
                    EnforceManager.Instance.WaterIncreaseDamage();
                    break;
                default: 
                    Debug.LogWarning($"Unhandled reward type: {selectedReward.Type}"); 
                    break;
            }
            selectedReward.ChosenProperty = null;
        }
        private void Selected(ExpData selectedReward)
        {
            levelUpRewardPanel.SetActive(false);
            gameManager.GameSpeed();
            ProcessExpReward(selectedReward);
        }
        public IEnumerator LevelUpReward() // 레벨업 보상 처리
        {
            Time.timeScale = 0; // 게임 일시 정지
            levelUpRewardPanel.SetActive(true); // 보물 패널 활성화
            var playerLevel = expManager.level;
            if (playerLevel <= 9)
            {
                yield return StartCoroutine(LevelUpChance(60, 35,5));
            }
            else
            {
                yield return StartCoroutine(LevelUpChance(50, 40, 10));
            }
            yield return new WaitUntil(() => levelUpRewardPanel.activeSelf == false); // 보물 패널이 비활성화될 때까지 대기
        }
        private IEnumerator LevelUpChance(int greenChance, int blueChance, int purpleChance)
        {
            var leverUpReward = LevelUpList(greenChance, blueChance, purpleChance);
            LevelUpDisplay(leverUpReward);
            yield return null;
        }
        private List<ExpData> LevelUpList(int greenChance, int blueChance, int purpleChance)
        {
            var levelUpPowerUps = new List<ExpData>();
            var selectedCodes = new HashSet<int>();
            for (var i = 0; i < 3; i++)
            {
                var total = greenChance + blueChance + purpleChance;
                var randomValue = Random.Range(0, total);

                if (randomValue < greenChance)
                {
                    _selectedPowerUp = LevelUpUnique(exp.ExpGreenList, selectedCodes);
                }
                else if (randomValue < greenChance + blueChance)
                {
                    _selectedPowerUp = LevelUpUnique(exp.ExpBlueList, selectedCodes);
                }
                else
                {
                    _selectedPowerUp = LevelUpUnique(exp.ExpPurpleList, selectedCodes);
                }
                if (_selectedPowerUp == null) continue;
                levelUpPowerUps.Add(_selectedPowerUp);
                selectedCodes.Add(_selectedPowerUp.Code);
            }
            return levelUpPowerUps;
        }
        private ExpData LevelUpUnique(IEnumerable<ExpData> powerUpsData, ICollection<int> selectedCodes)
        {
            var validOptions = powerUpsData.Where(p => IsValidOption(p, selectedCodes));
            return SelectRandom(validOptions);
        }
        private bool IsValidOption (ExpData powerUp, ICollection<int> selectedCodes)
        {
            if (selectedCodes.Contains(powerUp.Code)) return false;
            switch (powerUp.Type)
            {
                case ExpData.Types.Slow:
                    if(EnforceManager.Instance.slowCount > 3) return false;
                    break;
                case ExpData.Types.Exp:
                    if (EnforceManager.Instance.expPercentage > 30) return false;
                    break;
                case ExpData.Types.CastleRecovery:
                    if (EnforceManager.Instance.recoveryCastle) return false;
                    break;
                case ExpData.Types.NextStage:
                    if (EnforceManager.Instance.SelectedCount > 3) return false;
                    break;
                case ExpData.Types.StepDirection:
                    if (EnforceManager.Instance.diagonalMovement) return false;
                    if (gameManager.wave !=11) return false;
                    break;
                case ExpData.Types.StepLimit:
                    if (EnforceManager.Instance.permanentIncreaseMovementCount > 1) return false;
                    break;
                case ExpData.Types.CastleMaxHp:
                    if (EnforceManager.Instance.castleMaxHp >= 1000) return false;
                    break;
                case ExpData.Types.DivineRestraintTime:
                    if (!EnforceManager.Instance.activeRestraint) return false;
                    break;
                case ExpData.Types.DivinePenetrate:
                    if (EnforceManager.Instance.divinePenetrate) return false;
                    break;
                case ExpData.Types.DivineAtkRange:
                    if(EnforceManager.Instance.divineAtkRange) return false;
                    break;
                case ExpData.Types.DivinePoisonAdditionalDamage:
                    if(EnforceManager.Instance.divinePoisonAdditionalDamage) return false;
                    if (!EnforceManager.Instance.activatePoison) return false;
                    break;
                case ExpData.Types.PhysicAdditionalWeapon:
                    if(EnforceManager.Instance.physicAdditionalWeapon) return false;
                    break;
                case ExpData.Types.PhysicIncreaseWeaponScale:
                    if(EnforceManager.Instance.physicIncreaseWeaponScale) return false;
                    break;
                case ExpData.Types.PhysicSlowAdditionalDamage:
                    if(EnforceManager.Instance.physicSlowAdditionalDamage) return false;
                    break;
                case ExpData.Types.PoisonDoubleAtk:
                    if(EnforceManager.Instance.poisonDoubleAtk) return false;
                    break;
                case ExpData.Types.PoisonRestraintAdditionalDamage:
                    if(EnforceManager.Instance.poisonRestraintAdditionalDamage) return false;
                    if (!EnforceManager.Instance.activeRestraint) return false;
                    break;
                case ExpData.Types.PoisonActivate:
                    if (EnforceManager.Instance.activatePoison) return false;
                    break;
                case ExpData.Types.PoisonInstantKill:
                    if(EnforceManager.Instance.poisonInstantKill) return false;
                    if (!EnforceManager.Instance.activatePoison) return false;
                    break;
                case ExpData.Types.PoisonIncreaseAtkRange:
                    if(EnforceManager.Instance.poisonIncreaseAtkRange) return false;
                    break;
                case ExpData.Types.PoisonOverlapping:
                    if (!EnforceManager.Instance.activatePoison) return false;
                    break;
                case ExpData.Types.WaterStun:
                    if(EnforceManager.Instance.waterStun) return false;
                    break;
                case ExpData.Types.WaterIncreaseSlowTime:
                    if(EnforceManager.Instance.waterIncreaseSlowTime > 5f) return false;
                    break;
                case ExpData.Types.WaterIncreaseSlowPower:
                    if(EnforceManager.Instance.waterIncreaseSlowPower) return false;
                    break;
                case ExpData.Types.WaterRestraintKnockBack:
                    if(EnforceManager.Instance.waterRestraintKnockBack) return false;
                    if (!EnforceManager.Instance.activeRestraint) return false;
                    break;
                default:
                    return true;
            }
            return true;
        }
        private static ExpData SelectRandom(IEnumerable<ExpData> validOptions)
        {
            var commonDataList = validOptions.ToList();
            var count = commonDataList.Count();
            if (count == 0) return null;
            var randomIndex = Random.Range(0, count);
            return commonDataList.ElementAt(randomIndex);
        }
        private void LevelUpDisplay(IReadOnlyList<ExpData> powerUps)
        {
            LevelUpDisplayText(exp1Button, exp1Text, exp1Code, exp1BtnBadge, powerUps[0]);
            LevelUpDisplayText(exp2Button, exp2Text, exp2Code, exp2BtnBadge, powerUps[1]);
            LevelUpDisplayText(exp3Button, exp3Text, exp3Code, exp3BtnBadge, powerUps[2]);
        }
        private void LevelUpDisplayText(Button expButton, TMP_Text powerText,TMP_Text powerCode, Image btnBadge, ExpData powerUp)
        {
            var p = powerUp.Property[0];
            switch (powerUp.Type)
            {
                case ExpData.Types.GroupDamage:
                    powerText.text = $"전체 데미지 {p}% 증가";
                    break;
                case ExpData.Types.GroupAtkSpeed:
                    powerText.text = $"전체 공격속도 {p}% 증가";
                    break;
                case ExpData.Types.StepLimit:
                    powerText.text = $"최대 이동거리 {p} 증가";
                    break;
                case ExpData.Types.StepDirection:
                    powerText.text = $"대각선 이동 가능"; 
                    break;
                case ExpData.Types.Exp:
                    powerText.text = $"적 처치시 경험치 획득량 {p}% 증가\n(현재 {EnforceManager.Instance.expPercentage}% 최대 30%)";
                    break;
                case ExpData.Types.CastleRecovery:
                    powerText.text = $"웨이브 종료까지 피해를 입지 않으면\n캐슬 체력 {p} 매 웨이브 마다 증가";
                    break;
                case ExpData.Types.CastleMaxHp:
                    powerText.text = $"캐슬 최대체력 {p} 증가"; 
                    break;
                case ExpData.Types.Slow:
                    powerText.text = $"다음 웨이브 부터는 적 이동속도 {p}% 감소\n( 현재 {15*EnforceManager.Instance.slowCount}% / 최대 60%)";
                    break;
                case ExpData.Types.NextStage:
                    powerText.text = $"보스 스테이지 이후 {p} 개의\n케릭터 추가 이동 (현재 {EnforceManager.Instance.highLevelCharacterCount})";
                    break;
                case ExpData.Types.Gold:
                    powerText.text = $"5 매치시 골드 {p} 추가 획득";
                    break;
                case ExpData.Types.DivineActiveRestraint:
                    powerText.text = "[A그룹 - 노랑] 적중된 적은 20% 확률로 속박 1초";
                    break;
                case ExpData.Types.DivineRestraintTime:
                    powerText.text = "[A그룹 - 노랑] 속박 지속시간이 0.1 초 증가";
                    break;
                case ExpData.Types.DivinePenetrate:
                    powerText.text = "[A그룹 - 노랑] 투사체가 적을 1회 관통합니다."; 
                    break;
                case ExpData.Types.DivineAtkRange:
                    powerText.text = "[A그룹 - 노랑] 뒤쪽 방향 공격이 가능";
                    break;
                case ExpData.Types.DivinePoisonAdditionalDamage:
                    powerText.text = "[A그룹 - 노랑] 중독상태의 적에게 30% 공격력이 증가";
                    break;
                case ExpData.Types.PhysicAdditionalWeapon:
                    powerText.text = "[D그룹 - 보라] 검 1개 추가";
                    break;
                case ExpData.Types.PhysicIncreaseWeaponScale:
                    powerText.text = "[D그룹 - 보라] 검의 크기가 100% 증가합니다.";
                    break;
                case ExpData.Types.PhysicSlowAdditionalDamage:
                    powerText.text = "[D그룹 - 보라] 둔화된 적에게 데비지 100% 증가";
                    break;
                case ExpData.Types.PhysicAtkSpeed:
                    powerText.text = "[D그룹 - 보라] 공격속도가 20% 증가";
                    break;
                case ExpData.Types.PhysicIncreaseDamage:
                    powerText.text = "[D그룹 - 보라] 적을 죽이면 적 1기당\n모든 D그룹의 데미지가 5% 증가 (해당 웨이브만 적용)";
                    break;
                case ExpData.Types.PoisonActivate:
                    powerText.text = "[F그룹 - 초록] 중독 효과부여";
                    break;
                case ExpData.Types.PoisonOverlapping:
                    powerText.text = "[F그룹 - 초록] 중독 중첩 1 증가";
                    break;
                case ExpData.Types.PoisonDoubleAtk:
                    powerText.text = "[F그룹 - 초록] 공격이 더블어택으로 변경";
                    break;
                case ExpData.Types.PoisonRestraintAdditionalDamage:
                    powerText.text = "[F그룹 - 초록] 속박된 적에게 가하는 데미지 200% 증가";
                    break;
                case ExpData.Types.PoisonInstantKill:
                    powerText.text = "[F그룹 - 초록] 체력 15% 미만의 적은 15% 확률로 즉사";
                    break;
                case ExpData.Types.PoisonIncreaseAtkRange:
                    powerText.text = "[F그룹 - 초록] 사거리 1칸 증가";
                    break;
                case ExpData.Types.WaterStun:
                    powerText.text = "[E그룹 - 파랑] 명중시 15% 확률로 1초간 적 기절";
                    break;
                case ExpData.Types.WaterIncreaseSlowTime:
                    powerText.text = "[E그룹 - 파랑] 둔화 지속시간 0.2초 증가 최대 5초";
                    break;
                case ExpData.Types.WaterIncreaseSlowPower:
                    powerText.text = "[E그룹 - 파랑] 둔화강도 50% 증가";
                    break;
                case ExpData.Types.WaterRestraintKnockBack:
                    powerText.text = "[E그룹 - 파랑] 속박된 적을 뒤로 0.5칸 밀쳐냄";
                    break;
                case ExpData.Types.WaterIncreaseDamage:
                    powerText.text = "[E그룹 - 파랑] 공격력 20%씩 증가";
                    break;
            }
            powerCode.text = $"{powerUp.Code}";
            btnBadge.sprite = powerUp.BtnColor;
            expButton.image = expButton.image;
            expButton.onClick.RemoveAllListeners();
            expButton.onClick.AddListener(() => Selected(powerUp));
        }
    }
}
