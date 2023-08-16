using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using Script.AdsScript;
using Script.CharacterManagerScript;
using Script.PuzzleManagerGroup;
using Script.QuestGroup;
using Script.UIManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script.RewardScript
{
    public class CommonRewardManager : MonoBehaviour
    {
        [SerializeField] public GameObject commonRewardPanel; // 보물 패널
        [SerializeField] private TextMeshProUGUI common1Text; // 파워1 텍스트
        [SerializeField] private TextMeshProUGUI common2Text; // 파워2 텍스트
        [SerializeField] private TextMeshProUGUI common3Text; // 파워3 텍스트
        [SerializeField] private Button common1Button; 
        [SerializeField] private Button common2Button; 
        [SerializeField] private Button common3Button;
        [SerializeField] private Button commonShuffle;
        [SerializeField] private Image icon1;
        [SerializeField] private Image icon2;
        [SerializeField] private Image icon3;
        [SerializeField] private Image common1BtnBadge;
        [SerializeField] private Image common2BtnBadge;
        [SerializeField] private Image common3BtnBadge; // 공통 데이터
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private CountManager countManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private Language language; // 텍스트
        public static CommonRewardManager Instance;
        public readonly Queue<GameObject> PendingTreasure = new Queue<GameObject>(); // 보류 중인 보물 큐
        private GameObject _currentTreasure; // 현재 보물
        public bool openBoxing = true;
        private List<Data> _powerUps;
        private string _groupName;
        private int _bossRewardSelected;
        public bool isOpenBox;
        public event Action OnRewardSelected;
        private void Awake()
        {
            Instance = this;
        }
        // 1. 상자가 매치 되면 상자를 큐에 추가
        public void EnqueueTreasure()
        {
            if (isOpenBox) return;
            commonShuffle.gameObject.SetActive(true);
            isOpenBox = true;
            var treasure = PendingTreasure.Dequeue();
            openBoxing = true;
            var shake = treasure.transform.DOShakeScale(1.0f, 0.5f, 8); // 흔들리는 애니메이션 재생
            shake.OnComplete(() =>
            {
                _currentTreasure = treasure;
                StartCoroutine(OpenBox(_currentTreasure));
            });
        }
        // 1-1. 다시 섞기
        public void ReEnqueueTreasure()
        {
            StartCoroutine(OpenBox(_currentTreasure));
        }
        // 2. 상자마다의 확률 분배
        private IEnumerator CommonChance(int greenChance, int blueChance, int purpleChance, string forcedColor)
        {
            if (StageManager.Instance.currentWave >= 11 && !StageManager.Instance.isBossClear)
            {
                greenChance = 60;
                blueChance = 35;
                purpleChance = 5;
                _powerUps = CommonPowerList(greenChance, blueChance, purpleChance, "blue");
                CommonDisplay(_powerUps);
                yield return null;
            }
            else
            {
                _powerUps = CommonPowerList(greenChance, blueChance, purpleChance, forcedColor);
                CommonDisplay(_powerUps);
                yield return null;
            }
        }
        // 3. 확률별로 선택 옵션 계산
        private static List<Data> CommonPowerList(int greenChance, int blueChance, int purpleChance, string forcedColor)
        {
            var commonPowerUps = new List<Data>();
            var selectedCodes = new HashSet<int>();
            if (StageManager.Instance.isBossClear && !EnforceManager.Instance.addRow)
            {
                var firstDesiredPowerUp = new PurpleData(PowerTypeManager.Instance.purple, PowerTypeManager.Instance.purpleBack, 16, PowerTypeManager.Types.AddRow, new[] { 1 });
                commonPowerUps.Add(firstDesiredPowerUp);
                selectedCodes.Add(firstDesiredPowerUp.Code);

                for (var i = 0; i < 2; i++)
                {
                    Data selectedPowerUp;
                    switch (forcedColor)
                    {
                        case "blue" when i == 0: selectedPowerUp = CommonUnique(PowerTypeManager.Instance.BlueList, selectedCodes); break;
                        case "purple" when i == 0: selectedPowerUp = CommonUnique(PowerTypeManager.Instance.PurpleList, selectedCodes); break;
                        default:
                        {
                            var total = greenChance + blueChance + purpleChance;
                            var randomValue = Random.Range(0, total);
                            if (randomValue < greenChance) { selectedPowerUp = CommonUnique(PowerTypeManager.Instance.GreenList, selectedCodes); }
                            else if (randomValue < greenChance + blueChance) { selectedPowerUp = CommonUnique(PowerTypeManager.Instance.BlueList, selectedCodes); }
                            else { selectedPowerUp = CommonUnique(PowerTypeManager.Instance.PurpleList, selectedCodes); }
                            break;
                        }
                    }
                    if (selectedPowerUp == null) continue;
                    commonPowerUps.Add(selectedPowerUp);
                    selectedCodes.Add(selectedPowerUp.Code);
                }
            }
            else
            {
                // 랜덤 선택지 추가
                for (var i = 0; i < 3; i++)
                {
                    Data selectedPowerUp;
                    switch (forcedColor)
                    {
                        case "blue" when i == 0: selectedPowerUp = CommonUnique(PowerTypeManager.Instance.BlueList, selectedCodes); break;
                        case "purple" when i == 0: selectedPowerUp = CommonUnique(PowerTypeManager.Instance.PurpleList, selectedCodes); break;
                        default:
                            {
                                var total = greenChance + blueChance + purpleChance;
                                var randomValue = Random.Range(0, total);
                                if (randomValue < greenChance) { selectedPowerUp = CommonUnique(PowerTypeManager.Instance.GreenList, selectedCodes); }
                                else if (randomValue < greenChance + blueChance) { selectedPowerUp = CommonUnique(PowerTypeManager.Instance.BlueList, selectedCodes); }
                                else { selectedPowerUp = CommonUnique(PowerTypeManager.Instance.PurpleList, selectedCodes); }
                                break;
                            }
                    }
                    if (selectedPowerUp == null) continue;
                    commonPowerUps.Add(selectedPowerUp);
                    selectedCodes.Add(selectedPowerUp.Code);
                }
            }
            return commonPowerUps;
        }
        // 4. 코드 중복검사
        private static Data CommonUnique(IEnumerable<Data> powerUpsData, ICollection<int> selectedCodes)
        {
            var validOptions = powerUpsData.Where(p => IsValidOption(p, selectedCodes));
            return SelectRandom(validOptions);
        }
        // 5. case별 예외처리
        private static bool IsValidOption(Data powerUp, ICollection<int> selectedCodes)
            {
                if (selectedCodes.Contains(powerUp.Code)) return false; // Do not select already selected reward codes again
                switch (powerUp.Type)
                {
                    case PowerTypeManager.Types.NextStep:
                    case PowerTypeManager.Types.Water2Freeze:
                    case PowerTypeManager.Types.WaterFreeze:
                    case PowerTypeManager.Types.Dark2BackBoost:
                    case PowerTypeManager.Types.Dark3FifthAttackBoost:
                    case PowerTypeManager.Types.Dark3BleedDurationBoost:
                    case PowerTypeManager.Types.Dark3ShackledExplosion:
                    case PowerTypeManager.Types.Dark3PoisonDamageBoost:
                    case PowerTypeManager.Types.Dark3RateBoost:
                    case PowerTypeManager.Types.Dark3DamageBoost:
                    case PowerTypeManager.Types.Dark3BleedAttack:
                    case PowerTypeManager.Types.DarkFifthAttackDamageBoost:
                    case PowerTypeManager.Types.DarkStatusAilmentSlowEffect:
                    case PowerTypeManager.Types.DarkRangeIncrease:
                    case PowerTypeManager.Types.DarkAttackPowerBoost:
                    case PowerTypeManager.Types.DarkStatusAilmentDamageBoost:
                    case PowerTypeManager.Types.DarkAttackSpeedBoost:
                    case PowerTypeManager.Types.DarkKnockBackChance:
                    case PowerTypeManager.Types.WaterFreezeChance:
                    case PowerTypeManager.Types.WaterSlowDurationBoost:
                    case PowerTypeManager.Types.WaterFreezeDamageBoost:
                    case PowerTypeManager.Types.WaterSlowCPowerBoost:
                    case PowerTypeManager.Types.WaterAttackRateBoost:
                    case PowerTypeManager.Types.WaterGlobalFreeze:
                    case PowerTypeManager.Types.PhysicalSwordScaleIncrease:
                    case PowerTypeManager.Types.PhysicalSwordAddition:
                    case PowerTypeManager.Types.PhysicalAttackSpeedBoost:
                    case PowerTypeManager.Types.PhysicalRatePerAttack:
                    case PowerTypeManager.Types.PhysicalBindBleed:
                    case PowerTypeManager.Types.PhysicalDamageBoost:
                    case PowerTypeManager.Types.PhysicalBleedDuration:
                    case PowerTypeManager.Types.Water2SlowPowerBoost:
                    case PowerTypeManager.Types.Water2FreezeTimeBoost:
                    case PowerTypeManager.Types.Water2DamageBoost:
                    case PowerTypeManager.Types.Water2FreezeChanceBoost:
                    case PowerTypeManager.Types.Water2FreezeDamageBoost:
                    case PowerTypeManager.Types.Water2SlowTimeBoost:
                    case PowerTypeManager.Types.PoisonPerHitEffect:
                    case PowerTypeManager.Types.PoisonBleedingEnemyDamageBoost:
                    case PowerTypeManager.Types.PoisonDamagePerBoost:
                    case PowerTypeManager.Types.PoisonDamageBoost:
                    case PowerTypeManager.Types.PoisonDotDamageBoost:
                    case PowerTypeManager.Types.PoisonAttackSpeedIncrease:
                    case PowerTypeManager.Types.PoisonDurationBoost:
                    case PowerTypeManager.Types.Fire2FreezeDamageBoost:
                    case PowerTypeManager.Types.Fire2BurnDurationBoost:
                    case PowerTypeManager.Types.Fire2ChangeProperty:
                    case PowerTypeManager.Types.Fire2DamageBoost:
                    case PowerTypeManager.Types.Fire2RangeBoost:
                    case PowerTypeManager.Types.Fire2RateBoost:
                    case PowerTypeManager.Types.Fire2BossDamageBoost:
                    case PowerTypeManager.Types.FireBurnPerAttackEffect:
                    case PowerTypeManager.Types.FireStackOverlap:
                    case PowerTypeManager.Types.FireProjectileBounceDamage:
                    case PowerTypeManager.Types.FireBurnedEnemyExplosion:
                    case PowerTypeManager.Types.FireAttackSpeedBoost:
                    case PowerTypeManager.Types.FireProjectileSpeedIncrease:
                    case PowerTypeManager.Types.FireProjectileBounceIncrease:
                    case PowerTypeManager.Types.Poison2StunToChance:
                    case PowerTypeManager.Types.Poison2RangeBoost:
                    case PowerTypeManager.Types.Poison2DotDamageBoost:
                    case PowerTypeManager.Types.Poison2StunTimeBoost:
                    case PowerTypeManager.Types.Poison2SpawnPoisonArea:
                    case PowerTypeManager.Types.Poison2RateBoost:
                    case PowerTypeManager.Types.Poison2PoolTimeBoost:
                    case PowerTypeManager.Types.Physical2CastleCrushStatBoost:
                    case PowerTypeManager.Types.Physical2FifthBoost:
                    case PowerTypeManager.Types.Physical2BleedTimeBoost:
                    case PowerTypeManager.Types.Physical2PoisonDamageBoost:
                    case PowerTypeManager.Types.Physical2RangeBoost:
                    case PowerTypeManager.Types.Physical2RateBoost:
                    case PowerTypeManager.Types.Physical2BossBoost:
                    case PowerTypeManager.Types.Dark2DualAttack:
                    case PowerTypeManager.Types.Dark2StatusDamageBoost:
                    case PowerTypeManager.Types.Dark2ExplosionBoost:
                    case PowerTypeManager.Types.Dark2DoubleAttack:
                    case PowerTypeManager.Types.Dark2StatusPoison:
                    case PowerTypeManager.Types.Dark2SameEnemyBoost:
                        return false;
                    default:
                        switch (powerUp.Type)
                        {
                            case PowerTypeManager.Types.Gold:
                                if (EnforceManager.Instance.addGold) return false;
                                break;
                            case PowerTypeManager.Types.CastleMaxHp:
                                if (EnforceManager.Instance.castleMaxHp >= 1000) return false; // Make sure the max HP of the castle does not exceed 2000
                                break;
                            case PowerTypeManager.Types.Exp:
                                if (EnforceManager.Instance.expPercentage >= 30) return false; // Make sure the EXP increment does not exceed 30%
                                break;
                            case PowerTypeManager.Types.AddRow:
                                if (EnforceManager.Instance.addRow) return false;
                                if (!StageManager.Instance.isBossClear) return false;
                                if (StageManager.Instance.currentWave % 10 != 0 ) return false;// Show row extra reward only after boss stage, up to 2 times
                                break;
                            case PowerTypeManager.Types.Slow:
                                if (EnforceManager.Instance.slowCount >= 4) return false; // Displays the enemy movement speed reduction effect up to 3 times
                                break;
                            case PowerTypeManager.Types.NextStage:
                                if (EnforceManager.Instance.selectedCount > 3) return false; // Only use up to 3 next stage character upgrades
                                break;
                            case PowerTypeManager.Types.StepDirection:
                                if (EnforceManager.Instance.diagonalMovement)return false;
                                if (!StageManager.Instance.isBossClear) return false;
                                if (StageManager.Instance.currentWave % 10 != 0 ) return false;
                                break;
                            case PowerTypeManager.Types.Match5Upgrade:
                                if (EnforceManager.Instance.match5Upgrade) return false; // Don't show this option if 5 matching upgrade option is enabled
                                break;
                            case PowerTypeManager.Types.StepLimit:
                                if (EnforceManager.Instance.permanentIncreaseMovementCount > 1) return false; // don't show this option if permanent move count increment is enabled
                                break;
                            case PowerTypeManager.Types.CastleRecovery:
                                if (EnforceManager.Instance.recoveryCastle) return false; // Castle recovery can only be used once
                                break;
                            case PowerTypeManager.Types.GroupLevelUp:
                                if (EnforceManager.Instance.index.Contains(powerUp.Property[0])) return false; // Do not display GroupLevelUp options for groups where LevelUpPattern is executed
                                break;
                            case PowerTypeManager.Types.Step:
                                if (PlayerPrefs.GetInt("TutorialKey") == 1) return false;
                                break;
                            case PowerTypeManager.Types.LevelUpPattern:if (PlayerPrefs.GetInt("TutorialKey") == 1) return false;
                                if (PlayerPrefs.GetInt("TutorialKey") == 1) return false;
                                break;
                        }
                        return true;
                }
            }
        // 6. 예외처리되고 처리 된 옵션값 리턴
        private static Data SelectRandom(IEnumerable<Data> validOptions)
        {
           var commonDataList = validOptions.ToList();
           var count = commonDataList.Count();
           if (count == 0) return null;
           var randomIndex = Random.Range(0, count);
           return commonDataList.ElementAt(randomIndex);
        }
        // 7. 옵션값 출력
        private void CommonDisplay(IReadOnlyList<Data> powerUpsDisplayData)
        {            
            if (spawnManager.isTutorial)
            {
                common2Button.interactable = false;
                common3Button.interactable = false;
            }
            else
            {
                common2Button.interactable = true;
                common3Button.interactable = true;
            }
            CommonDisplayText(common1Button, common1Text, icon1, common1BtnBadge ,powerUpsDisplayData[0], language);
            CommonDisplayText(common2Button, common2Text, icon2, common2BtnBadge, powerUpsDisplayData[1], language);
            CommonDisplayText(common3Button, common3Text, icon3, common3BtnBadge, powerUpsDisplayData[2], language);
        }
        // 8. 옵션 텍스트
        private void CommonDisplayText(Button commonButton, TMP_Text powerText, Image icon, Image btnBadge, Data powerUp, Language languages)
        {
           var translationKey = powerUp.Type.ToString();
           var powerTextTranslation = languages.GetTranslation(translationKey);
           var finalPowerText = powerTextTranslation;

           var placeholderValues = new Dictionary<string, Func<double>>
           {
               { "{p}", () => powerUp.Property[0]},
               { "{powerUp.Property[0]}", () => powerUp.Property[0]},
               { "{15*EnforceManager.Instance.slowCount}", () => 15 * EnforceManager.Instance.slowCount },
               { "{EnforceManager.Instance.expPercentage}", () => EnforceManager.Instance.expPercentage },
               { "{EnforceManager.Instance.highLevelCharacterCount}", () => EnforceManager.Instance.highLevelCharacterCount},
           };
           finalPowerText = placeholderValues.Aggregate(finalPowerText, (current, placeholder) => current.Replace(placeholder.Key, placeholder.Value().ToString(CultureInfo.CurrentCulture)));
           var finalTranslation = finalPowerText.Replace("||", "\n");

           if (powerUp.Type is PowerTypeManager.Types.GroupLevelUp or PowerTypeManager.Types.LevelUpPattern)
           {
               icon.sprite = EnforceManager.Instance.characterList[powerUp.Property[0]]
                   .GetSpriteForLevel(EnforceManager.Instance.characterList[powerUp.Property[0]].unitPeaceLevel);

               btnBadge.sprite = EnforceManager.Instance.characterList[powerUp.Property[0]].UnitGrade switch
               {
                   CharacterBase.UnitGrades.Green => PowerTypeManager.Instance.green,
                   CharacterBase.UnitGrades.Blue => PowerTypeManager.Instance.blue,
                   CharacterBase.UnitGrades.Purple => PowerTypeManager.Instance.purple,
               };
               commonButton.GetComponent<Image>().sprite = EnforceManager.Instance.characterList[powerUp.Property[0]].UnitGrade switch
               {
                   CharacterBase.UnitGrades.Green => PowerTypeManager.Instance.greenBack,
                   CharacterBase.UnitGrades.Blue => PowerTypeManager.Instance.blueBack,
                   CharacterBase.UnitGrades.Purple => PowerTypeManager.Instance.purpleBack,
               };
           }
           else
           {
               icon.sprite = powerUp.Icon;
               btnBadge.sprite = powerUp.BtnColor;
               commonButton.GetComponent<Image>().sprite = powerUp.BackGroundColor;
           }
           switch (powerUp.Type)
            {
                case PowerTypeManager.Types.Exp:
                    powerText.text = finalTranslation;
                    break;
                case PowerTypeManager.Types.Slow: 
                    powerText.text = finalTranslation;
                    break;
                case PowerTypeManager.Types.GroupDamage: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.GroupAtkSpeed: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.Step: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.StepLimit: 
                    powerText.text =finalTranslation; 
                    break;
                case PowerTypeManager.Types.StepDirection: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.RandomLevelUp: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.GroupLevelUp:
                    var groupUnit = characterManager.characterList[powerUp.Property[0]];
                    _groupName = groupUnit.name;
                    icon.sprite = groupUnit.GetSpriteForLevel(1);
                    powerText.text = finalTranslation.Replace("{_groupName}", _groupName);
                    break;
                case PowerTypeManager.Types.LevelUpPattern:
                    var unit = characterManager.characterList[powerUp.Property[0]];
                    _groupName = unit.name;
                    icon.sprite = unit.GetSpriteForLevel(1);
                    powerText.text = finalTranslation.Replace("{_groupName}", _groupName);
                    break;
                case PowerTypeManager.Types.CastleRecovery: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.CastleMaxHp: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.Match5Upgrade: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.NextStage: 
                    powerText.text = finalTranslation;
                    break;
                case PowerTypeManager.Types.Gold: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.AddRow: 
                    powerText.text = finalTranslation; 
                    break;
            }
           _groupName = null;
           commonButton.onClick.RemoveAllListeners();
           commonShuffle.onClick.RemoveAllListeners();
           commonButton.onClick.AddListener(() => SelectCommonReward(powerUp));
           commonShuffle.onClick.AddListener(ShuffleCommonReward);
        }
        // 9. 상자 오픈
        private IEnumerator OpenBox(GameObject treasure)
        {
        Time.timeScale = 0; // 게임 일시 정지
        if (PlayerPrefs.GetInt("TutorialKey") == 1)
        {
            Time.timeScale = 1;
        }
        commonRewardPanel.SetActive(true); // 보물 패널 활성화
        var treasureChestLevel = treasure.GetComponent<CharacterBase>().unitPuzzleLevel; // 보물 상자 이름
        switch(treasureChestLevel)
        {
            case 1:
                break;
            case 2:
                yield return StartCoroutine(CommonChance(80, 17, 3, null));
                break;
            case 3:
                yield return StartCoroutine(CommonChance(60, 35, 5, "blue"));
                break;
            case 4:
                yield return StartCoroutine(CommonChance(0, 80, 20, "purple"));
                break;
        }

        yield return new WaitUntil(() => commonRewardPanel.activeSelf == false); // 보물 패널이 비활성화될 때까지 대기
        openBoxing = false;
        if (PendingTreasure.Count > 0)
        {
            EnqueueTreasure();
        }
        isOpenBox = false;
        }
        // 10. 상자 선택
        private void SelectCommonReward(Data selectedReward)
        {
        Selected(selectedReward);
        QuestManager.Instance.MatchCoinQuest();
        OnRewardSelected?.Invoke();
        }
        // 11. 선택 처리
        private void Selected(Data selectedReward)
        {
        commonRewardPanel.SetActive(false);
        if (countManager.TotalMoveCount == 0)
        {
            gameManager.GameSpeed();
        }
        else
        {
            Time.timeScale = 1; // 게임 재개
        }
        CharacterPool.ReturnToPool(_currentTreasure); // 보물을 풀에 반환

        if (!spawnManager.isWave10Spawning)
        {
            StartCoroutine(spawnManager.PositionUpCharacterObject());
        }
        else
        {
            _currentTreasure = null; // 현재 보물 없음
        }

        ProcessCommonReward(selectedReward);
        }
        // 12. 선택된 버프 적용 
        private static void ProcessCommonReward(Data selectedReward)
        {
            switch (selectedReward.Type)
            {
                case PowerTypeManager.Types.AddRow:
                    EnforceManager.Instance.AddRow(selectedReward);
                    break; // Row 추가 강화 효과
                case PowerTypeManager.Types.GroupDamage:
                    EnforceManager.Instance.IncreaseGroupDamage(selectedReward,selectedReward.Property[0]); 
                    break;     // 전체 데미지 증가 효과
                case PowerTypeManager.Types.GroupAtkSpeed:
                    EnforceManager.Instance.IncreaseGroupRate(selectedReward,selectedReward.Property[0]); 
                    break;  // 전체 공격 속도 증가 효과
                case PowerTypeManager.Types.Step: 
                    EnforceManager.Instance.RewardMoveCount(selectedReward.Property[0]);
                    break; // 카운트 증가
                case PowerTypeManager.Types.StepLimit:
                    EnforceManager.Instance.PermanentIncreaseMoveCount(selectedReward,selectedReward.Property[0]);
                    break; // 영구적 카운트 증가
                case PowerTypeManager.Types.StepDirection:
                    EnforceManager.Instance.DiagonalMovement(selectedReward);
                    break;    // 대각선 이동
                case PowerTypeManager.Types.RandomLevelUp:
                    EnforceManager.RandomCharacterLevelUp(selectedReward.Property[0]); 
                    break;// 랜덤 케릭터 레벨업
                case PowerTypeManager.Types.GroupLevelUp: 
                    EnforceManager.Instance.CharacterGroupLevelUp(selectedReward.Property[0]); 
                    break;  // 케릭터 그룹 레벨업
                case PowerTypeManager.Types.LevelUpPattern:
                    EnforceManager.Instance.PermanentIncreaseCharacter(selectedReward, selectedReward.Property[0]);
                    break; // 기본 2레벨 케릭터 생성
                case PowerTypeManager.Types.Exp:
                    EnforceManager.Instance.IncreaseExpBuff(selectedReward, selectedReward.Property[0]);
                    break;  // 경험치 5% 증가
                case PowerTypeManager.Types.CastleRecovery:
                    EnforceManager.Instance.RecoveryCastle(selectedReward);
                    break;  // 성 체력 회복
                case PowerTypeManager.Types.CastleMaxHp:
                    EnforceManager.Instance.IncreaseCastleMaxHp(selectedReward);
                    break;  
                case PowerTypeManager.Types.Match5Upgrade: 
                    EnforceManager.Instance.Match5Upgrade(selectedReward);
                    break;     // 5매치 패턴 업그레이드
                case PowerTypeManager.Types.Slow:
                    EnforceManager.Instance.SlowCount(selectedReward);
                    break; // 적 이동속도 감소 
                case PowerTypeManager.Types.NextStage:
                    EnforceManager.Instance.NextCharacterUpgrade(selectedReward, selectedReward.Property[0]);
                    break; // 보드 초기화 시 케릭터 상속되는 케릭터 Count 증가
                case PowerTypeManager.Types.Gold:
                    EnforceManager.Instance.AddGold(selectedReward);
                    break;
                default: Debug.LogWarning($"Unhandled reward type: {selectedReward.Type}"); 
                    break;
            }
            selectedReward.ChosenProperty = null;
        }
        // # 보스 웨이브 클리어 별도 보상
        public IEnumerator WaveRewardChance()
        {
        if (StageManager.Instance.latestStage == 1) yield break;
        Time.timeScale = 0;
        commonRewardPanel.SetActive(true);
        yield return StartCoroutine(CommonChance(30, 55, 15, null));
        }
        private void ShuffleCommonReward()
        {
        if (spawnManager.isTutorial) return;
        AdsManager.Instance.ShowRewardedAd();
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_refresh");
        AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Common;
        commonShuffle.gameObject.SetActive(false);
        }
    }
}
