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
        [SerializeField] private GameObject common1Button; 
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
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private Sprite bronzeOpen;
        [SerializeField] private Sprite silverOpen;
        [SerializeField] private Sprite goldOpen;
        public static CommonRewardManager Instance;
        public readonly Queue<GameObject> PendingTreasure = new Queue<GameObject>(); // 보류 중인 보물 큐
        private GameObject _currentTreasure; // 현재 보물
        private List<Data> _powerUps;
        private string _zeroLevelUnitName;
        private string _changeUnitName;
        private int _bossRewardSelected;
        public bool isOpenBox;
        public Dictionary<CharacterBase.UnitGroups, Dictionary<string, string>> LevelUpDict = new Dictionary<CharacterBase.UnitGroups, Dictionary<string, string>>();

        public event Action OnRewardSelected;
        private void Awake()
        {
            Instance = this;
            LevelUpDict = new Dictionary<CharacterBase.UnitGroups, Dictionary<string, string>>
            {
                {CharacterBase.UnitGroups.Octopus, new Dictionary<string, string> {{"Octopus", "Tentacle"}}},
                {CharacterBase.UnitGroups.Orc, new Dictionary<string, string> {{"Orc", "Sword"}}},
                {CharacterBase.UnitGroups.Skeleton, new Dictionary<string, string> {{"Skeleton", "Bone"}}},
                {CharacterBase.UnitGroups.Fishman, new Dictionary<string, string> {{"Fishman", "Net"}}},
                {CharacterBase.UnitGroups.Phoenix, new Dictionary<string, string> {{"Phoenix", "Egg"}}},
                {CharacterBase.UnitGroups.Beholder, new Dictionary<string, string> {{"Beholder", "Rune"}}},
                {CharacterBase.UnitGroups.Ogre, new Dictionary<string, string> {{"Ogre", "Rock"}}},
                {CharacterBase.UnitGroups.DeathChiller, new Dictionary<string, string> {{"DeathChiller", "Icicle"}}},
                {CharacterBase.UnitGroups.Cobra, new Dictionary<string, string> {{"Cobra", "Fang"}}},
                {CharacterBase.UnitGroups.Berserker, new Dictionary<string, string> {{"Berserker", "Axe"}}},
                {CharacterBase.UnitGroups.DarkElf, new Dictionary<string, string> {{"DarkElf", "Arrow"}}}
            };

        }
        // 1. 상자가 매치 되면 상자를 큐에 추가
        public IEnumerator EnqueueTreasure()
        {
            if (isOpenBox) yield break;
            isOpenBox = true;
            commonShuffle.gameObject.SetActive(true);
            var treasure = PendingTreasure.Dequeue();
            var sprite = treasure.GetComponent<CharacterBase>().unitPuzzleLevel switch
            {
                2 => bronzeOpen,
                3 => silverOpen,
                4 => goldOpen,
                _ => bronzeOpen
            };
            var shake = treasure.transform.DOShakeScale(0.7f, 0.5f, 8); // 흔들리는 애니메이션 재생
            shake.OnComplete(() =>
            {
                treasure.GetComponent<SpriteRenderer>().sprite = sprite;
                _currentTreasure = treasure;
                StartCoroutine(WaitAndOpenBox(_currentTreasure, 0.3f));
            });
            yield return null;
        }
        private IEnumerator WaitAndOpenBox(GameObject treasure, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            StartCoroutine(OpenBox(treasure));
        }
        // 1-1. 다시 섞기
        public void ReEnqueueTreasure()
        {
            StartCoroutine(OpenBox(_currentTreasure));
        }
        // 2. 상자마다의 확률 분배
        private IEnumerator CommonChance(int greenChance, int blueChance, int purpleChance, string forcedColor)
        {
            if (StageManager.Instance != null && StageManager.Instance.currentWave >= 11 && !StageManager.Instance.isBossClear)
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
        private static (string desc, string popupDesc) GetSkillDesc(PowerTypeManager.Types type)
        {
            foreach (var data in PowerTypeManager.Instance.PurpleList.Where(data => data.Type == type))
            {
                return (data.Desc, data.PopupDesc);
            }
            return (null, null);
        }
        // 3. 확률별로 선택 옵션 계산
        private List<Data> CommonPowerList(int greenChance, int blueChance, int purpleChance, string forcedColor)
        {
            var commonPowerUps = new List<Data>();
            var selectedCodes = new HashSet<int>();
            if (StageManager.Instance != null && StageManager.Instance.isBossClear && !EnforceManager.Instance.addRow)
            {
                const PowerTypeManager.Types type = PowerTypeManager.Types.AddRow;
                var (desc, popupDesc) = GetSkillDesc(type);
                var firstDesiredPowerUp = new PurpleData(CharacterBase.UnitGroups.None, 1, 16, PowerTypeManager.Types.AddRow,PowerTypeManager.Instance.purple, PowerTypeManager.Instance.purpleBack,   desc, popupDesc, new[] { 1 });
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
        private Data CommonUnique(IEnumerable<Data> powerUpsData, ICollection<int> selectedCodes)
        {
            var validOptions = powerUpsData.Where(p => IsValidOption(p, selectedCodes));
            return SelectRandom(validOptions);
        }
        // 5. case별 예외처리
        private bool IsValidOption(Data powerUp, ICollection<int> selectedCodes)
        {
            if (selectedCodes.Contains(powerUp.Code)) return false; // Do not select already selected reward codes again
            if (powerUp.SkillGroup != CharacterBase.UnitGroups.None) return false;
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
                        if (spawnManager.isTutorial) return false;
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
                        if (spawnManager.isTutorial) return false;
                        if (EnforceManager.Instance.index.Contains(powerUp.Property[0])) return false; // Do not display GroupLevelUp options for groups where LevelUpPattern is executed
                        break;
                    case PowerTypeManager.Types.Step1:
                        if (countManager.TotalMoveCount == 0) return false;
                        if (spawnManager.isTutorial) return false;
                        break;
                    case PowerTypeManager.Types.Step2:
                        if (countManager.TotalMoveCount == 0) return false;
                        if (spawnManager.isTutorial) return false;
                        break;
                    case PowerTypeManager.Types.Step3:
                        if (countManager.TotalMoveCount == 0) return false;
                        if (spawnManager.isTutorial) return false;
                        break;
                    case PowerTypeManager.Types.LevelUpPattern:
                        if (spawnManager.isTutorial) return false;
                        break;
                    case PowerTypeManager.Types.RandomLevelUp:
                        if (spawnManager.isTutorial) return false;
                        break;
                }
                return true;
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
            CommonDisplayText(common1Button.GetComponent<Button>(), common1Text, icon1, common1BtnBadge ,powerUpsDisplayData[0]);
            CommonDisplayText(common2Button, common2Text, icon2, common2BtnBadge, powerUpsDisplayData[1]);
            CommonDisplayText(common3Button, common3Text, icon3, common3BtnBadge, powerUpsDisplayData[2]);
        }
        // 8. 옵션 텍스트
        private void CommonDisplayText(Button commonButton, TMP_Text powerText, Image icon, Image btnBadge, Data powerUp)
        {
            var finalDesc = powerUp.Desc;
            var placeholderValues = new Dictionary<string, Func<double>> {{ "{p}", () => powerUp.Property[0]}};
            finalDesc = placeholderValues.Aggregate(finalDesc, (current, placeholder) => current.Replace(placeholder.Key, placeholder.Value().ToString(CultureInfo.CurrentCulture)));
            var finalTranslation = finalDesc.Replace("||", "\n");
            if (powerUp.Type is PowerTypeManager.Types.GroupLevelUp or PowerTypeManager.Types.LevelUpPattern)
            {
               icon.sprite = EnforceManager.Instance.characterList[powerUp.Property[0]]
                   .GetSpriteForLevel(EnforceManager.Instance.characterList[powerUp.Property[0]].unitPieceLevel);

               btnBadge.sprite = EnforceManager.Instance.characterList[powerUp.Property[0]].UnitGrade switch
               {
                   CharacterBase.UnitGrades.G => PowerTypeManager.Instance.green,
                   CharacterBase.UnitGrades.B => PowerTypeManager.Instance.blue,
                   CharacterBase.UnitGrades.P => PowerTypeManager.Instance.purple,
               };
               commonButton.GetComponent<Image>().sprite = EnforceManager.Instance.characterList[powerUp.Property[0]].UnitGrade switch
               {
                   CharacterBase.UnitGrades.G => PowerTypeManager.Instance.greenBack,
                   CharacterBase.UnitGrades.B => PowerTypeManager.Instance.blueBack,
                   CharacterBase.UnitGrades.P => PowerTypeManager.Instance.purpleBack,
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
                case PowerTypeManager.Types.GroupDamage1: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.GroupDamage2: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.GroupDamage3: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.GroupAtkSpeed1: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.GroupAtkSpeed2: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.GroupAtkSpeed3: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.Step1: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.Step2: 
                    powerText.text = finalTranslation; 
                    break;
                case PowerTypeManager.Types.Step3: 
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
                    var groupLevelUpKey = characterManager.characterList[powerUp.Property[0]];
                    foreach (var item in LevelUpDict[groupLevelUpKey.unitGroup])
                    {
                        _changeUnitName = "{unit_N}";
                        _zeroLevelUnitName = "{0level_unit_N}";
                        finalTranslation = finalTranslation.Replace(_changeUnitName, item.Key)
                            .Replace(_zeroLevelUnitName, item.Value);
                    }
                    icon.sprite = groupLevelUpKey.GetSpriteForLevel(1);
                    powerText.text = finalTranslation;
                    break;
                case PowerTypeManager.Types.LevelUpPattern:
                    var levelUpPatternKey = characterManager.characterList[powerUp.Property[0]];
                    foreach (var item in LevelUpDict[levelUpPatternKey.unitGroup])
                    {
                        _changeUnitName = "{unit_N}";
                        _zeroLevelUnitName = "{0level_unit_N}";
                        finalTranslation = finalTranslation.Replace(_changeUnitName, item.Key)
                            .Replace(_zeroLevelUnitName, item.Value);
                    }
                    icon.sprite = levelUpPatternKey.GetSpriteForLevel(1);
                    powerText.text = finalTranslation;
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
           commonButton.onClick.RemoveAllListeners();
           commonShuffle.onClick.RemoveAllListeners();
           commonButton.onClick.AddListener(() => SelectCommonReward(powerUp));
           commonShuffle.onClick.AddListener(ShuffleCommonReward);
        }
        // 9. 상자 오픈
        private IEnumerator OpenBox(GameObject treasure)
        {
            commonRewardPanel.SetActive(true); // 보물 패널 활성화
            Quest.Instance.MergeBoxQuest();
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
            if (PlayerPrefs.GetInt("TutorialKey") == 1)
            {
                common1Button.GetComponent<Canvas>().enabled = true;
                common1Button.GetComponent<Canvas>().overrideSorting = true;
                common1Button.GetComponent<Canvas>().sortingOrder = 1;
            }
            yield return new WaitUntil(() => commonRewardPanel.activeSelf == false); // 보물 패널이 비활성화될 때까지 대기
            if (PendingTreasure.Count > 0)
            {
               yield return StartCoroutine(EnqueueTreasure());
            }
          
        }
        // 10. 상자 선택
        private void SelectCommonReward(Data selectedReward)
        {
            StartCoroutine(Selected(selectedReward));
            OnRewardSelected?.Invoke();
        }
        // 11. 선택 처리
        private IEnumerator Selected(Data selectedReward)
        {
            if (common1Button.GetComponent<Canvas>() != null)
            {
                Destroy(common1Button.GetComponent<GraphicRaycaster>());
                Destroy(common1Button.GetComponent<Canvas>());
            }
            commonRewardPanel.SetActive(false);
            CharacterPool.ReturnToPool(_currentTreasure); // 보물을 풀에 반환
            yield return null;
            spawnManager.AddToQueue(spawnManager.PositionUpCharacterObject());
            ProcessCommonReward(selectedReward);
        }
        // 12. 선택된 버프 적용 
        private void ProcessCommonReward(Data selectedReward)
        {
            switch (selectedReward.Type)
            {
                case PowerTypeManager.Types.AddRow:
                    EnforceManager.Instance.AddRow(selectedReward);
                    break; // Row 추가 강화 효과
                case PowerTypeManager.Types.GroupDamage1:
                    EnforceManager.Instance.IncreaseGroupDamage(selectedReward,selectedReward.Property[0]); 
                    break;     // 전체 데미지 증가 효과
                case PowerTypeManager.Types.GroupDamage2:
                    EnforceManager.Instance.IncreaseGroupDamage(selectedReward,selectedReward.Property[0]); 
                    break;     // 전체 데미지 증가 효과
                case PowerTypeManager.Types.GroupDamage3:
                    EnforceManager.Instance.IncreaseGroupDamage(selectedReward,selectedReward.Property[0]); 
                    break;     // 전체 데미지 증가 효과
                case PowerTypeManager.Types.GroupAtkSpeed1:
                    EnforceManager.Instance.IncreaseGroupRate(selectedReward,selectedReward.Property[0]); 
                    break;  // 전체 공격 속도 증가 효과
                case PowerTypeManager.Types.GroupAtkSpeed2:
                    EnforceManager.Instance.IncreaseGroupRate(selectedReward,selectedReward.Property[0]); 
                    break;  // 전체 공격 속도 증가 효과
                case PowerTypeManager.Types.GroupAtkSpeed3:
                    EnforceManager.Instance.IncreaseGroupRate(selectedReward,selectedReward.Property[0]); 
                    break;  // 전체 공격 속도 증가 효과
                case PowerTypeManager.Types.Step1: 
                    EnforceManager.Instance.RewardMoveCount(selectedReward.Property[0]);
                    break; // 카운트 증가
                case PowerTypeManager.Types.Step2: 
                    EnforceManager.Instance.RewardMoveCount(selectedReward.Property[0]);
                    break; // 카운트 증가
                case PowerTypeManager.Types.Step3: 
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
            isOpenBox = false;
            if (PendingTreasure.Count == 0)
            {
                _currentTreasure = null;
            }
        }
        // # 보스 웨이브 클리어 별도 보상
        public IEnumerator WaveRewardChance()
        {
            if (StageManager.Instance != null && StageManager.Instance.latestStage == 1) yield break;
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
