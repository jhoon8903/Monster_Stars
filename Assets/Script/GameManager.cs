using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.AdsScript;
using Script.CharacterGroupScript;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.PuzzleManagerGroup;
using Script.QuestGroup;
using Script.RewardScript;
using Script.UIManager;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TutorialManager tutorialManager;
        [SerializeField] private CountManager countManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private int moveCount;
        [SerializeField] private SwipeManager swipeManager;
        [SerializeField] private CameraManager cameraManager;
        [SerializeField] private BackGroundManager backgroundManager;
        [SerializeField] private GameObject commonRewardPanel;
        [SerializeField] private GameObject expRewardPanel;
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private EnemyPool enemyPool;
        [SerializeField] private LevelUpRewardManager levelUpRewardManager;
        [SerializeField] private ExpManager expManager;
        [SerializeField] private CommonRewardManager commonRewardManager;
        [SerializeField] private Image speedUpImage;
        [SerializeField] private Sprite normalSpeedImage;
        [SerializeField] private Sprite doubleSpeedImage;
        public List<GameObject> characterList = new List<GameObject>(); 
        public static GameManager Instance { get; private set; }
        private readonly WaitForSecondsRealtime _waitTwoSecRealtime = new WaitForSecondsRealtime(2f);
        public bool speedUp;
        private Vector3Int _bossSpawnArea;
        public bool IsBattle { get; private set; }
       

        private void Awake()
        {
            Instance = this;
            Input.multiTouchEnabled = false;
            Application.targetFrameRate = 60;
        }
        private void Start()
        {
            StartCoroutine(LoadGame());
        }
        private IEnumerator LoadGame()
        {
            swipeManager.isBusy = true;
            BackGroundManager.Instance.ChangedBackGround();
            gridManager.GenerateInitialGrid(PlayerPrefs.GetInt("GridHeight", 6));
            if (LoadingManager.Instance.isFirstContact)
            {
                // 사용가능 무브 6회 [3match, 4match, 5match, nullSwap, PressDelete, commonReward match] 
                 countManager.Initialize(6);
                 // 튜토리얼용 매치 오브젝트 생성
                 StartCoroutine(tutorialManager.TutorialState());
                 swipeManager.isBusy = false;
            }
            else
            {
                if (PlayerPrefs.HasKey("unitState"))
                {
                    StartCoroutine(SpawnManager.LoadGameState());
                    EnforceManager.Instance.LoadEnforceData();
                    countManager.Initialize(PlayerPrefs.GetInt("moveCount"));
                    expManager.LoadExp();
                    castleManager.LoadCastleHp();
                    if (StageManager.Instance.currentWave % 10 == 0)
                    {
                        _bossSpawnArea = new Vector3Int(Random.Range(1, 5), 9, 0);
                        gridManager.ApplyBossSpawnColor(_bossSpawnArea);
                    }
                }
                else
                {
                    countManager.Initialize(moveCount + AdsManager.Instance.adsMoveCount);
                    castleManager.castleCrushBoost = false;
                    AdsManager.Instance.adsMoveCount = 0;
                }
                StartCoroutine(spawnManager.PositionUpCharacterObject());
            }
   
            castleManager.castleCrushBoost = false;
            StageManager.Instance.SelectedStages();
            StageManager.Instance.UpdateWaveText();
            speedUp = true;
            GameSpeedSelect();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("stage_play", "play", PlayerPrefs.GetInt("LatestStage", 1));
            yield return null;
        }
        public IEnumerator Count0Call()
        {
            IsBattle = true;
            if (PlayerPrefs.GetInt("TutorialKey") == 1)
            {
                tutorialManager.EndTutorial();
            }
            StartCoroutine(cameraManager.CameraBattleSizeChange());
            StartCoroutine(backgroundManager.ChangeBattleSize());
            StartCoroutine(CoverUnit(true));
            yield return _waitTwoSecRealtime;
            StartCoroutine(AtkManager.Instance.CheckForAttack());
            StartCoroutine(StageManager.Instance.WaveController());

            // var allUnits = FindObjectsOfType<CharacterBase>();
            // foreach (var unit in allUnits)
            // {
            //     if (unit is Fishman unitE)
            //     {
            //         unitE.ApplyAttackSpeedBuffToAllies();
            //     }
            // }
            GameSpeed();
        }
        private IEnumerator CoverUnit(bool value)
        {
            characterList = CharacterPool.Instance.UsePoolCharacterList();
            foreach (var unit in characterList)
            {
                unit.GetComponent<CharacterBase>().cover.SetActive(value);
            }
            yield return null;
        }
        public IEnumerator ContinueOrLose()
        {
            IsBattle = false;
            AtkManager.Instance.ClearWeapons();
            // var allUnits = FindObjectsOfType<CharacterBase>();
            // foreach (var unit in allUnits)
            // {
            //     if (unit is not Fishman { HasAttackSpeedBuff: true } || !EnforceManager.Instance.water2AttackSpeedBuffToAdjacentAllies) continue;
            //     unit.defaultAtkRate /= 0.9f;
            //     unit.HasAttackSpeedBuff = false;
            // }
            if (castleManager.HpPoint > 0)
            {
                StageManager.Instance.SaveClearWave();
                if (StageManager.Instance.isBossClear)
                {
                    moveCount = 15 + EnforceManager.Instance.rewardMoveCount;
                    PlayerPrefs.SetInt("moveCount",moveCount);
                    countManager.Initialize(PlayerPrefs.GetInt("moveCount"));
                    yield return StartCoroutine(commonRewardManager.WaveRewardChance());
                    yield return StartCoroutine(spawnManager.BossStageClearRule());
                    yield return StartCoroutine(gridManager.ResetBossSpawnColor());
                    yield return StartCoroutine(InitializeWave());
                }
                else
                {
                    moveCount = 7 + EnforceManager.Instance.rewardMoveCount;
                    PlayerPrefs.SetInt("moveCount",moveCount);
                    countManager.Initialize(PlayerPrefs.GetInt("moveCount"));
                    yield return StartCoroutine(InitializeWave());
                }
            }
            else
            {
                LoseGame();
            }
            AtkManager.Instance.weaponsList.Clear();
        }
        private IEnumerator InitializeWave()
        {
            yield return StartCoroutine(KillMotion());
            SpawnManager.SaveUnitState();
            expManager.SaveExp();
            castleManager.SaveCastleHp();
            EnforceManager.Instance.SaveEnforceData();
            Time.timeScale = 1;
            if (EnforceManager.Instance.recoveryCastle)
            {
                castleManager.RecoverCastleHp();
            }
            castleManager.TookDamageLastWave = false;
            if (StageManager.Instance.currentWave % 10 == 0)
            {
                _bossSpawnArea = new Vector3Int(Random.Range(1, 5), 9, 0);
                gridManager.ApplyBossSpawnColor(_bossSpawnArea);
            }
            
            yield return StartCoroutine(backgroundManager.ChangePuzzleSize());
            yield return StartCoroutine(cameraManager.CameraPuzzleSizeChange());
            
            enemyPool.ClearList();
            StageManager.Instance.isBossClear = false;
            castleManager.castleCrushBoost = false;
            if (levelUpRewardManager.HasUnitInGroup(CharacterBase.UnitGroups.Orc))
            {
                AtkManager.Instance.groupDAtkCount = 0;
                FindObjectOfType<Orc>().groupDAtkRate = 0f;
            } else if (levelUpRewardManager.HasUnitInGroup(CharacterBase.UnitGroups.Skeleton))
            {
                AtkManager.Instance.groupFCount = 0;
                FindObjectOfType<Skeleton>().groupFDamage = 0f;
            }
            foreach (var unit in CharacterPool.Instance.pooledCharacters)
            {
                unit.GetComponent<CharacterBase>().cover.SetActive(false);
            }
            Quest.Instance.VictoryQuest();
        }
        private void LoseGame()
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("stage_fail","fail", StageManager.Instance.latestStage );
            Firebase.Analytics.FirebaseAnalytics.LogEvent("stage_fail","wave", StageManager.Instance.currentWave );
            Time.timeScale = 0;
            StartCoroutine(KillMotion());
            ClearRewardManager.Instance.ClearReward(false);
        }
        private static IEnumerator KillMotion()
        {
           yield return DOTween.KillAll(true);
        }
        public IEnumerator WaitForPanelToClose()
        {
            if (commonRewardPanel.activeSelf)
            {
                while (commonRewardPanel.activeSelf)
                {
                    yield return null;
                }
            }

            if (!expRewardPanel.activeSelf) yield break;
            while (expRewardPanel.activeSelf)
            {
                yield return null;
            }
        }
        public void GameSpeedSelect()
        {
            if (spawnManager.isTutorial) return;
            if (speedUp == false)
            {
                speedUp = true;
                speedUpImage.sprite = doubleSpeedImage;
                GameSpeed();
            }
            else
            {
                speedUp = false;
                speedUpImage.sprite = normalSpeedImage;
                GameSpeed();
            }
        }
        public void GameSpeed()
        {
            if (speedUp)
            {
                Time.timeScale = IsBattle ? 2 : 1;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        public static void ReturnRobby()
        {
            PlayerPrefs.DeleteKey("unitState");
            PlayerPrefs.DeleteKey("EnforceData");
            PlayerPrefs.SetInt("GridHeight", 6);
            PlayerPrefs.SetInt($"{StageManager.Instance.latestStage}Stage_ProgressWave",1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("SelectScene");
        }
    }
}