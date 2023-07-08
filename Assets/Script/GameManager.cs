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

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            swipeManager.isBusy = true;
            StageManager.Instance.UpdateWaveText();
            gridManager.GenerateInitialGrid();
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
        public IEnumerator ContinueOrLose()
        {
            IsBattle = false;
            AtkManager.Instance.ClearWeapons();
            if (castleManager.HpPoint > 0)
            {          
                IsClear = true;
                ClearRewardManager.Instance.GetCoin(StageManager.Instance.currentStage, StageManager.Instance.currentWave);
                StageManager.Instance.currentWave++;
                PlayerPrefs.SetInt(StageManager.Instance.clearedWaveKey, StageManager.Instance.clearWave);
                PlayerPrefs.SetInt(StageManager.Instance.currentWaveKey, StageManager.Instance.currentWave);
                StageManager.Instance.UpdateWaveText();
                yield return StartCoroutine(NextWave());
                if (StageManager.Instance.currentWave > StageManager.Instance.clearWave )
                {
                    StageManager.Instance.clearWave++;
                }
                if (StageManager.Instance.currentWave % 10 == 0)
                {
                    StageManager.Instance.ClearBoss = true;
                    yield return StartCoroutine(commonRewardManager.WaveReward());
                    yield return StartCoroutine(spawnManager.BossStageClearRule());
                    StageManager.Instance.ClearBoss = false;
                }
                if (levelUpRewardManager.HasUnitInGroup(CharacterBase.UnitGroups.D) && 
                    EnforceManager.Instance.physicIncreaseDamage)  
                    FindObjectOfType<UnitD>().ResetDamage();
            }
            else
            {
                IsClear = false;
                LoseGame();
            }

            AtkManager.Instance.weaponsList.Clear();
        }
        private IEnumerator NextWave()
        {
            spawnManager.SaveUnitState();
            expManager.SaveExp();
            castleManager.SaveCastleHp();
            EnforceManager.Instance.SaveEnforceData();
            Time.timeScale = 1;
            yield return StartCoroutine(KillMotion());
            yield return new WaitForSecondsRealtime(0.5f);
            _bossSpawnArea = new Vector3Int(Random.Range(1,5), 10, 0);
            if (StageManager.Instance.currentWave % 10 == 0)
            {
                gridManager.ApplyBossSpawnColor(_bossSpawnArea);
            }
            if (EnforceManager.Instance.recoveryCastle)
            {
                castleManager.RecoverCastleHp();
            }
            castleManager.TookDamageLastWave = false;
            moveCount = 7 + EnforceManager.Instance.rewardMoveCount;
            PlayerPrefs.SetInt("moveCount",moveCount);
            countManager.Initialize(PlayerPrefs.GetInt("moveCount"));
            yield return StartCoroutine(backgroundManager.ChangePuzzleSize());
            yield return StartCoroutine(cameraManager.CameraPuzzleSizeChange());
            enemyPool.ClearList();

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