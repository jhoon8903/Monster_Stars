using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
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
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private ExpManager expManager;
        [SerializeField] private CountManager countManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private SwipeManager swipeManager;
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private EnemyManager enemyManager;
        private ExpData _selectedExpRewardData;

        private void ProcessExpReward(ExpData selectedReward)
        {
            var findCharacterManager = FindObjectOfType<CharacterManager>();
            switch (selectedReward.Type)
            {
                case ExpData.Types.GroupDamage: 
                    findCharacterManager.IncreaseGroupDamage(selectedReward.Property[0]);
                    break;
                case ExpData.Types.GroupAtkSpeed:
                    findCharacterManager.IncreaseGroupAtkRate(selectedReward.Property[0]);
                    break;
                case ExpData.Types.StepLimit:
                    countManager.PermanentIncreaseMoveCount(selectedReward.Property[0]); 
                    characterManager.permanentIncreaseMovementCount = true;
                    break;
                case ExpData.Types.StepDirection:
                    swipeManager.EnableDiagonalMovement(); 
                    characterManager.diagonalMovement = true;
                    break;
                case ExpData.Types.Exp:
                    expManager.IncreaseExpBuff(selectedReward.Property[0]); 
                    characterManager.expPercentage += 5;
                    break;
                case ExpData.Types.CastleRecovery:
                    gameManager.RecoveryCastle = true; 
                    characterManager.recoveryCastle = true;
                    break;
                case ExpData.Types.CastleMaxHp:
                    castleManager.IncreaseMaxHp(selectedReward.Property[0]);
                    break;
                case ExpData.Types.Slow:
                    enemyManager.DecreaseMoveSpeed(selectedReward.Property[0]); 
                    characterManager.slowCount += 1;
                    break;
                case ExpData.Types.NextStage:
                    spawnManager.nextCharacterUpgrade(selectedReward.Property[0]); 
                    characterManager.nextStageMembersSelectCount += 1;
                    break;
                case ExpData.Types.Gold:
                    characterManager.goldGetMore = true; 
                    Debug.LogWarning($"Unhandled reward type: {selectedReward.Type}");
                    break;
                case ExpData.Types.DivineRestraint:
                    break;
                case ExpData.Types.DivinePenetrate:
                    break;
                case ExpData.Types.DivineRestraintDamage:
                    break;
                case ExpData.Types.DivineAtkRange:
                    break;
                case ExpData.Types.DivinePoisonAdditionalDamage:
                    break;
                case ExpData.Types.PhysicAdditionalWeapon:
                    break;
                case ExpData.Types.PhysicIncreaseWeaponScale:
                    break;
                case ExpData.Types.PhysicSlowAdditionalDamage:
                    break;
                case ExpData.Types.PhysicAtkSpeed:
                    break;
                case ExpData.Types.PhysicIncreaseDamage:
                    break;
                case ExpData.Types.PoisonDoubleAtk:
                    break;
                case ExpData.Types.PoisonRestraintAdditionalDamage:
                    break;
                case ExpData.Types.PoisonIncreaseTime:
                    break;
                case ExpData.Types.PoisonInstantKill:
                    break;
                case ExpData.Types.PoisonIncreaseAtkRange:
                    break;
                case ExpData.Types.WaterStun:
                    break;
                case ExpData.Types.WaterIncreaseSlowTime:
                    break;
                case ExpData.Types.WaterIncreaseSlowPower:
                    break;
                case ExpData.Types.WaterRestraintKnockBack:
                    break;
                case ExpData.Types.WaterIncreaseDamage:
                    break;
                default: 
                    Debug.LogWarning($"Unhandled reward type: {selectedReward.Type}"); 
                    break;
            }
            selectedReward.ChosenProperty = null;
            _selectedExpRewardData = null;
        }
        private void Selected()
        {
            levelUpRewardPanel.SetActive(false);
            if (countManager.baseMoveCount == 0)
            {
                gameManager.GameSpeed();
            }
            else
            {
                Time.timeScale = 1; // 게임 재개
            }
            if (!spawnManager.isWave10Spawning)
            {
                StartCoroutine(spawnManager.PositionUpCharacterObject());
            }
            ProcessExpReward(_selectedExpRewardData);
        }
        public IEnumerator LevelUpReward() // 레벨업 보상 처리
        {
            Time.timeScale = 0; // 게임 일시 정지
            levelUpRewardPanel.SetActive(true); // 보물 패널 활성화
            var playerLevel = expManager.level;
            if (playerLevel <= 9)
            {
                yield return StartCoroutine(LevelUpChance(60, 30,10));
            }
            else
            {
                yield return StartCoroutine(LevelUpChance(30, 50, 20));
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
                ExpData selectedPowerUp = null;
                if (randomValue < greenChance)
                {
                    selectedPowerUp = LevelUpUnique(exp.ExpGreenList, selectedCodes);
                }
                else if (randomValue < greenChance + blueChance)
                {
                    selectedPowerUp = LevelUpUnique(exp.ExpBlueList, selectedCodes);
                }
                else
                {
                    selectedPowerUp = LevelUpUnique(exp.ExpPurpleList, selectedCodes);
                }
                if (selectedPowerUp == null) continue;
                levelUpPowerUps.Add(selectedPowerUp);
                selectedCodes.Add(selectedPowerUp.Code);
            }
            return levelUpPowerUps;
        }
        private static ExpData LevelUpUnique(IEnumerable<ExpData> powerUps, ICollection<int> selectedCodes)
        {
            var validOptions = powerUps.Where(p => !selectedCodes.Contains(p.Code)).ToList();
            if (validOptions.Count == 0)
            {
                return null;
            }
            var randomIndex = Random.Range(0, validOptions.Count);
            return validOptions[randomIndex];
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
                    powerText.text = $"Increases the damage of the entire character group by {p}%.";
                    break;
                case ExpData.Types.GroupAtkSpeed:
                    powerText.text = $"Increases the attack speed of the entire character group by {p}%.";
                    break;
                case ExpData.Types.StepLimit:
                    powerText.text = $"{p} steps are added to the movement count for each wave.";
                    break;
                case ExpData.Types.StepDirection:
                    powerText.text = $"{p} steps are added to the movement count for each wave."; 
                    break;
                case ExpData.Types.Exp:
                    powerText.text = $"Acquire additional Exp {p}% when killing an enemy. (up to 30%)";
                    break;
                case ExpData.Types.CastleRecovery:
                    powerText.text = $"If you take no damage from the wave, the castle's health is restored by {p}.";
                    break;
                case ExpData.Types.CastleMaxHp:
                    powerText.text = $"Increases the max HP and HP of the castle by {p}."; 
                    break;
                case ExpData.Types.Slow:
                    powerText.text = $"Enemy movement speed is slowed by {p}%. (up to 60%)";
                    break;
                case ExpData.Types.NextStage:
                    powerText.text = $"You can take {p} additional characters when initializing characters.(up to 3 Characters)";
                    break;
                case ExpData.Types.Gold:
                    powerText.text = $"At 5 matches, additional gold is obtained as much as {p}.";
                    break;
                case ExpData.Types.DivineRestraint:
                    powerText.text = $"At 5 matches, additional gold is obtained as much as {p}.";
                    break;
                case ExpData.Types.DivinePenetrate:
                    break;
                case ExpData.Types.DivineRestraintDamage:
                    break;
                case ExpData.Types.DivineAtkRange:
                    break;
                case ExpData.Types.DivinePoisonAdditionalDamage:
                    break;
                case ExpData.Types.PhysicAdditionalWeapon:
                    break;
                case ExpData.Types.PhysicIncreaseWeaponScale:
                    break;
                case ExpData.Types.PhysicSlowAdditionalDamage:
                    break;
                case ExpData.Types.PhysicAtkSpeed:
                    break;
                case ExpData.Types.PhysicIncreaseDamage:
                    break;
                case ExpData.Types.PoisonDoubleAtk:
                    break;
                case ExpData.Types.PoisonRestraintAdditionalDamage:
                    break;
                case ExpData.Types.PoisonIncreaseTime:
                    break;
                case ExpData.Types.PoisonInstantKill:
                    break;
                case ExpData.Types.PoisonIncreaseAtkRange:
                    break;
                case ExpData.Types.WaterStun:
                    break;
                case ExpData.Types.WaterIncreaseSlowTime:
                    break;
                case ExpData.Types.WaterIncreaseSlowPower:
                    break;
                case ExpData.Types.WaterRestraintKnockBack:
                    break;
                case ExpData.Types.WaterIncreaseDamage:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            powerCode.text = $"{powerUp.Code}";
            btnBadge.sprite = powerUp.BtnColor;
            expButton.image = expButton.image;
            expButton.onClick.RemoveAllListeners();
            expButton.onClick.AddListener(() => SelectExpReward(powerUp));
        }
        private void SelectExpReward(ExpData selectedReward)
        {
            _selectedExpRewardData = selectedReward;
            Selected();
        }
    }
}
