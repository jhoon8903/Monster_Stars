using System;
using System.Collections;
using DG.Tweening;
using Script.CharacterGroupScript;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.PuzzleManagerGroup;
using Script.RewardScript;
using Script.UIManager;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private CountManager countManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private int moveCount;
        [SerializeField] private SwipeManager swipeManager;
        [SerializeField] private CameraManager cameraManager;
        [SerializeField] private BackGroundManager backgroundManager;
        [SerializeField] private GameObject gamePanel;
        [SerializeField] private GameObject commonRewardPanel;
        [SerializeField] private GameObject expRewardPanel;
        [SerializeField] private TextMeshProUGUI speedUpText;
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private EnemyPool enemyPool;
        [SerializeField] private LevelUpRewardManager levelUpRewardManager;
        [SerializeField] private ExpManager expManager;
        [SerializeField] private CommonRewardManager commonRewardManager;
        [SerializeField] private TextMeshProUGUI stageText;
        public static GameManager Instance { get; private set; }
        private readonly WaitForSecondsRealtime _waitOneSecRealtime = new WaitForSecondsRealtime(1f);
        private readonly WaitForSecondsRealtime _waitTwoSecRealtime = new WaitForSecondsRealtime(2f);
        public bool speedUp;
        private Vector3Int _bossSpawnArea;
        public bool IsBattle { get; private set; }
        public bool globalSlow;

        private void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 300;

        }

        private void Start()
        {
            StartCoroutine(LoadGame());
        }

        private IEnumerator LoadGame()
        {
            swipeManager.isBusy = true; 
            gridManager.GenerateInitialGrid(PlayerPrefs.GetInt("GridHeight", 6));
            if (PlayerPrefs.HasKey("unitState"))
            {
                StartCoroutine(spawnManager.LoadGameState());
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
                countManager.Initialize(moveCount);
                StartCoroutine(spawnManager.PositionUpCharacterObject());
            }
            StageManager.Instance.SelectedStages();
            StageManager.Instance.UpdateWaveText();
            speedUp = true;
            GameSpeedSelect();
            stageText.text = $"{StageManager.Instance.selectStage} STAGE";
            swipeManager.isBusy = false;
            yield return null;
        }
        public IEnumerator Count0Call()
        {
            IsBattle = true;
            yield return _waitOneSecRealtime;
            StartCoroutine(cameraManager.CameraBattleSizeChange());
            StartCoroutine(backgroundManager.ChangeBattleSize());
            yield return _waitTwoSecRealtime;
            StartCoroutine(AtkManager.Instance.CheckForAttack());
            StartCoroutine(StageManager.Instance.WaveController());
            // var allUnits = FindObjectsOfType<CharacterBase>();
            // foreach (var unit in allUnits)
            // {
            //     if (unit is UnitE unitE)
            //     {
            //         unitE.ApplyAttackSpeedBuffToAllies();
            //     }
            // }
            GameSpeed();
        }
        public IEnumerator ContinueOrLose()
        {
            IsBattle = false;
            AtkManager.Instance.ClearWeapons();
            // var allUnits = FindObjectsOfType<CharacterBase>();
            // foreach (var unit in allUnits)
            // {
            //     if (unit is not UnitE { HasAttackSpeedBuff: true } || !EnforceManager.Instance.water2AttackSpeedBuffToAdjacentAllies) continue;
            //     unit.defaultAtkRate /= 0.9f;
            //     unit.HasAttackSpeedBuff = false;
            // }
            if (castleManager.HpPoint > 0)
            {
                ClearRewardManager.Instance.GetCoin();
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
            spawnManager.SaveUnitState();
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
            if (levelUpRewardManager.HasUnitInGroup(CharacterBase.UnitGroups.D))
            {
                AtkManager.Instance.groupDAtkCount = 0;
                FindObjectOfType<UnitD>().groupDAtkRate = 0f;
            } else if (levelUpRewardManager.HasUnitInGroup(CharacterBase.UnitGroups.F))
            {
                AtkManager.Instance.groupFCount = 0;
                FindObjectOfType<UnitF>().groupFDamage = 0f;
            }
        }
        private void LoseGame()
        {
            Time.timeScale = 0;
            StartCoroutine(KillMotion());
            gamePanel.SetActive(true);
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
            if (speedUp == false)
            {
                speedUp = true;
                speedUpText.text = "x2";
                GameSpeed();
            }
            else
            {
                speedUp = false;
                speedUpText.text = "x1";
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
            StageManager.Instance.isStageClear = false;
            PlayerPrefs.DeleteKey("unitState");
            PlayerPrefs.DeleteKey("EnforceData");
            PlayerPrefs.SetInt($"{StageManager.Instance.latestStage}Stage_ProgressWave",1);
            PlayerPrefs.SetInt("GridHeight", 6);
            PlayerPrefs.Save();
            SceneManager.LoadScene("SelectScene");
        }
    }
}