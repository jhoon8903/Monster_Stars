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
        public static GameManager Instance { get; private set; }
        private readonly WaitForSecondsRealtime _waitOneSecRealtime = new WaitForSecondsRealtime(1f);
        private readonly WaitForSecondsRealtime _waitTwoSecRealtime = new WaitForSecondsRealtime(2f);
        public bool speedUp;
        private Vector3Int _bossSpawnArea;
        public bool IsBattle { get; private set; }
        private bool IsClear { get; set; }

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
        private void Start()
        {
            swipeManager.isBusy = true;
            gridManager.GenerateInitialGrid(PlayerPrefs.GetInt("GridHeight", 6));
            if (PlayerPrefs.HasKey("unitState"))
            {
                EnforceManager.Instance.LoadEnforceData();
                countManager.Initialize(PlayerPrefs.GetInt("moveCount"));
                expManager.LoadExp();
                castleManager.LoadCastleHp();
                StartCoroutine(spawnManager.LoadGameState());
                if (StageManager.Instance.currentWave % 10 == 0)
                {
                    _bossSpawnArea = new Vector3Int(Random.Range(1,5), 10, 0);
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
            swipeManager.isBusy = false;
        }

        public IEnumerator Count0Call()
        {
            IsBattle = true;
            yield return _waitOneSecRealtime;
            StartCoroutine(cameraManager.CameraBattleSizeChange());
            StartCoroutine(backgroundManager.ChangeBattleSize());
            yield return _waitTwoSecRealtime;
            StageManager.Instance.StartWave();
            GameSpeed();
        }

        public IEnumerator ContinueOrLose()
        {
            IsBattle = false;
            AtkManager.Instance.ClearWeapons();
            if (castleManager.HpPoint > 0)
            {          
                IsClear = true;
                ClearRewardManager.Instance.GetCoin(StageManager.Instance.currentStage, StageManager.Instance.currentWave);
                StageManager.Instance.clearWave = StageManager.Instance.currentWave;
                StageManager.Instance.SaveClearWave();

                if (StageManager.Instance.currentWave == StageManager.Instance.MaxWave())
                {
                    StageManager.Instance.StageClear();
                }
                else if (StageManager.Instance.isBossClear)
                {
                    moveCount = 15 + EnforceManager.Instance.rewardMoveCount;
                    yield return StartCoroutine(commonRewardManager.WaveRewardChance());
                    yield return StartCoroutine(spawnManager.BossStageClearRule());
                    yield return StartCoroutine(gridManager.ResetBossSpawnColor());
                    yield return StartCoroutine(InitializeWave());
                }
                else
                {
                    moveCount = 7 + EnforceManager.Instance.rewardMoveCount;
                    yield return StartCoroutine(InitializeWave());
                }
            }
            else
            {
                IsClear = false;
                LoseGame();
            }
            AtkManager.Instance.weaponsList.Clear();
        }
        private IEnumerator InitializeWave()
        {
            spawnManager.SaveUnitState();
            expManager.SaveExp();
            castleManager.SaveCastleHp();
            EnforceManager.Instance.SaveEnforceData();
            PlayerPrefs.SetInt("moveCount",moveCount);
            countManager.Initialize(PlayerPrefs.GetInt("moveCount"));
            
            Time.timeScale = 1;

            if (EnforceManager.Instance.recoveryCastle)
            {
                castleManager.RecoverCastleHp();
            }
            castleManager.TookDamageLastWave = false;

            if (StageManager.Instance.currentWave % 10 == 0)
            {
                _bossSpawnArea = new Vector3Int(Random.Range(1,5), 10, 0);
                gridManager.ApplyBossSpawnColor(_bossSpawnArea);
            }

            yield return StartCoroutine(backgroundManager.ChangePuzzleSize());
            yield return StartCoroutine(cameraManager.CameraPuzzleSizeChange());
            
            if (levelUpRewardManager.HasUnitInGroup(CharacterBase.UnitGroups.D) && 
                EnforceManager.Instance.physicIncreaseDamage)  
                FindObjectOfType<UnitD>().ResetDamage();
            enemyPool.ClearList();
            StageManager.Instance.isBossClear = false;
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
        public void RetryGame()
        {
            gamePanel.SetActive(false);
            SceneManager.LoadScene("StageScene");
        }
        public void ReturnRobby()
        {
            PlayerPrefs.DeleteKey("unitState");
            PlayerPrefs.DeleteKey("EnforceData");
            PlayerPrefs.SetInt(StageManager.Instance.currentWaveKey,1);
            SceneManager.LoadScene("SelectScene");
        }
    }
}