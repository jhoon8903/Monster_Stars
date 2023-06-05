using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
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
        [SerializeField] private Image common1BtnBadge;
        [SerializeField] private Image common2BtnBadge;
        [SerializeField] private Image common3BtnBadge;
        [SerializeField] private TextMeshProUGUI common1Code; // 파워1 코드 텍스트
        [SerializeField] private TextMeshProUGUI common2Code; // 파워2 코드 텍스트
        [SerializeField] private TextMeshProUGUI common3Code; // 파워3 코드 텍스트
        [SerializeField] internal Sprite greenSprite; // 녹색 버튼 스프라이트
        [SerializeField] internal Sprite blueSprite; // 파란색 버튼 스프라이트
        [SerializeField] internal Sprite purpleSprite; // 보라색 버튼 스프라이트
        [SerializeField] private Common common; // 공통 데이터
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private CountManager countManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private SwipeManager swipeManager;
        [SerializeField] private ExpManager expManager;
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private EnemyManager enemyManager;
        public readonly Queue<GameObject> PendingTreasure = new Queue<GameObject>(); // 보류 중인 보물 큐
        private GameObject _currentTreasure = null; // 현재 보물
        public bool openBoxing = false;
        private bool _waveRewards = false; 
        private CommonData _selectedCommonReward;

        private void ProcessCommonReward(CommonData selectedReward)
        {
            var characterManager = FindObjectOfType<CharacterManager>();
            
            switch (selectedReward.Type)
            {
                // Row 추가 강화 효과
                case CommonData.Types.AddRow:
                    gridManager.AddRow();
                    break;
                // 전체 데미지 증가 효과
                case CommonData.Types.GroupDamage:
                    characterManager.IncreaseGroupDamage(selectedReward.Property[0]);
                        break;
                // 전체 공격 속도 증가 효과
                case CommonData.Types.GroupAtkSpeed:
                    characterManager.IncreaseGroupAtkRate(selectedReward.Property[0]);
                    break;
                // 카운트 증가
                case CommonData.Types.Step:
                    countManager.IncreaseRewardMoveCount(selectedReward.Property[0]);
                    break;
                // 영구적 카운트 증가
                case CommonData.Types.StepLimit:
                     countManager.PermanentIncreaseMoveCount(selectedReward.Property[0]);
                     break;
                // 대각선 이동
                case CommonData.Types.StepDirection:
                     swipeManager.EnableDiagonalMovement();
                     break;
                // 랜덤 케릭터 레벨업
                case CommonData.Types.RandomLevelUp:
                    characterManager.RandomCharacterLevelUp(selectedReward.Property[0]);
                    break;
                // 케릭터 그룹 레벨업
                case CommonData.Types.LevelUp:
                    characterManager.CharacterGroupLevelUp(selectedReward.Property[0]);
                    break;
                // 기본 2레벨 케릭터 생성
                case CommonData.Types.LevelUpPattern:
                    characterManager.PermanentIncreaseCharacter(selectedReward.Property[0]);
                    break;
                // 경험치 5% 증가
                case CommonData.Types.Exp:
                     expManager.IncreaseExpBuff(selectedReward.Property[0]);
                     break;
                // 성 체력 회복
                case CommonData.Types.CastleRecovery:
                    gameManager.RecoveryCastle = true;
                    break;
                // 성 최대 체력 증가 & 회복
                case CommonData.Types.CastleMaxHp:
                    castleManager.IncreaseMaxHp(selectedReward.Property[0]);
                    break;
                //  5 Match Pattern Upgrade
                case CommonData.Types.Match5Upgrade:
                    matchManager.match5Upgrade = true;
                    break;
                // 적 이동속도 감소 
                case CommonData.Types.Slow:
                    enemyManager.DecreaseMoveSpeed(selectedReward.Property[0]);
                    break;
                // 보드 초기화 시 케릭터 상속되는 케릭터 Count 증가
                case CommonData.Types.NextStage: 
                    spawnManager.nextCharacterUpgrade(selectedReward.Property[0]);
                    break;
                case CommonData.Types.Gold:
                    Debug.LogWarning($"Unhandled reward type: {selectedReward.Type}");
                    break;
                default:
                    Debug.LogWarning($"Unhandled reward type: {selectedReward.Type}");
                    break;
            }
            _selectedCommonReward = null;
        }
        private void Selected()
        {
            commonRewardPanel.SetActive(false);
            if (countManager.baseMoveCount == 0)
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
            if (PendingTreasure.Count > 0)
            {
                _currentTreasure = PendingTreasure.Dequeue();
                EnqueueTreasure(_currentTreasure);
            }
            else
            {
                _currentTreasure = null; // 현재 보물 없음
            }
            ProcessCommonReward(_selectedCommonReward);
        }
        public static int RandomChanceMethod(IReadOnlyList<int> rangeValues)
        {
            var random = new System.Random();
            var randomIndex = random.Next(rangeValues.Count);
            return rangeValues[randomIndex];
        }
        public void EnqueueTreasure(GameObject treasure)
        {
            openBoxing = true;
            var shake = treasure.transform.DOShakeScale(1.0f, 0.5f, 8); // 흔들리는 애니메이션 재생
            shake.OnComplete(() =>
                {
                    _currentTreasure = treasure;
                    StartCoroutine(OpenBox(_currentTreasure));
                });
            openBoxing = false;
        }
        private IEnumerator OpenBox(GameObject treasure)
        {
            _waveRewards = false;
            Time.timeScale = 0; // 게임 일시 정지
            commonRewardPanel.SetActive(true); // 보물 패널 활성화
            var treasureChestName = treasure.GetComponent<CharacterBase>().CharacterName; // 보물 상자 이름

            switch (treasureChestName)
            {
                case "Unit_Treasure00":
                    break;
                case "Unit_Treasure01":
                    yield return StartCoroutine(CommonChance(70, 25, 5));
                    break;
                case "Unit_Treasure02":
                    yield return StartCoroutine(CommonChance(30, 55, 15));
                    break;
                case "Unit_Treasure03":
                    yield return StartCoroutine(CommonChance(0, 50, 50));
                    break;
            }
            yield return new WaitUntil(() => commonRewardPanel.activeSelf == false); // 보물 패널이 비활성화될 때까지 대기
        }
        private IEnumerator CommonChance(int greenChance, int blueChance, int purpleChance)
        {
            var powerUps = CommonPowerList(greenChance, blueChance, purpleChance);
            CommonDisplay(powerUps);
            yield return null;
        }
        private List<CommonData> CommonPowerList(int greenChance, int blueChance, int purpleChance)
        {
            var commonPowerUps = new List<CommonData>();
            var selectedCodes = new HashSet<int>();

            for (var i = 0; i < 3; i++)
            {
                if (gameManager.wave == 11 && i == 0 && _waveRewards)
                {
                    var desiredPowerUp = new CommonPurpleData(purpleSprite, 13, CommonData.CommonRepeatTypes.NoneRepeat, 1, CommonData.Types.AddRow, new[]{1});
                    commonPowerUps.Add(desiredPowerUp);
                    selectedCodes.Add(desiredPowerUp.Code);
                }
                else
                {
                    var total = greenChance + blueChance + purpleChance;
                    var randomValue = Random.Range(0, total);
                    CommonData selectedPowerUp = null;
                    if (randomValue < greenChance)
                    {
                        selectedPowerUp = CommonUnique(common.CommonGreenList, selectedCodes);
                    }
                    else if (randomValue < greenChance + blueChance)
                    {
                        selectedPowerUp = CommonUnique(common.CommonBlueList, selectedCodes);
                    }
                    else
                    {
                        selectedPowerUp = CommonUnique(common.CommonPurpleList, selectedCodes);
                    }
                    if (selectedPowerUp == null) continue;
                    commonPowerUps.Add(selectedPowerUp);
                    selectedCodes.Add(selectedPowerUp.Code);
                }
            }
            return commonPowerUps;
        }
        private static CommonData CommonUnique(IEnumerable<CommonData> powerUps, ICollection<int> selectedCodes)
        {
            var validOptions = powerUps.Where(p => !selectedCodes.Contains(p.Code)).ToList();
            if (validOptions.Count == 0)
            {
                return null;
            }
            var randomIndex = Random.Range(0, validOptions.Count);
            return validOptions[randomIndex];
        }
        private void CommonDisplay(IReadOnlyList<CommonData> powerUps)
        {
            CommonDisplayText(common1Button,common1Text, common1Code, common1BtnBadge,powerUps[0]);
            CommonDisplayText(common2Button,common2Text, common2Code, common2BtnBadge,powerUps[1]);
            CommonDisplayText(common3Button,common3Text, common3Code, common3BtnBadge,powerUps[2]);
        }
        private void CommonDisplayText(Button commonButton, TMP_Text powerText, TMP_Text powerCode, Image btnBadge ,CommonData powerUp)
        {
            powerText.text = $"{powerUp.Type} {powerUp.Property[0]}% PowerUp_Property";
            powerCode.text = $"{powerUp.Code}";
            btnBadge.sprite = powerUp.BtnColor;
            commonButton.image = commonButton.image;
            commonButton.onClick.RemoveAllListeners();
            commonButton.onClick.AddListener(() => SelectCommonReward(powerUp));
        }
        private void SelectCommonReward(CommonData selectedReward)
        {
            _selectedCommonReward = selectedReward;
            Selected();
        }
        public IEnumerator WaveReward()
        {
            Time.timeScale = 0;
            commonRewardPanel.SetActive(true);
            _waveRewards = true;
            yield return StartCoroutine(CommonChance(30, 55, 15));
        }
    }
}
