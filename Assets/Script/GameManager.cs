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
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private GameObject gamePanel;
        [SerializeField] private GameObject commonRewardPanel;
        [SerializeField] private GameObject expRewardPanel;
        [SerializeField] private TextMeshProUGUI speedUpText;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private CommonRewardManager commonRewardManager;
        [SerializeField] private AtkManager atkManager;
        [SerializeField] private EnemyPool enemyPool;
        private readonly WaitForSecondsRealtime _waitOneSecRealtime = new WaitForSecondsRealtime(1f);
        private readonly WaitForSecondsRealtime _waitTwoSecRealtime = new WaitForSecondsRealtime(2f);
        public bool speedUp;
        public int wave = 1;
        private Vector3Int _bossSpawnArea;
        public bool IsBattle { get; private set; }

        private void Start()
        {
            swipeManager.isBusy = true;
            countManager.Initialize(moveCount);
            gridManager.GenerateInitialGrid(); 
            speedUp = true;
            waveText.text = $"{wave}";
            GameSpeedSelect();
            StartCoroutine(spawnManager.PositionUpCharacterObject());
            swipeManager.isBusy = false;
            DOTween.SetTweensCapacity(200000, 500);
        }
        public IEnumerator Count0Call()
        {
            IsBattle = true;
            yield return _waitOneSecRealtime;
            StartCoroutine(cameraManager.CameraBattleSizeChange());
            StartCoroutine(backgroundManager.ChangeBattleSize());
            yield return _waitTwoSecRealtime;
            StartCoroutine(waveManager.WaveController(wave));
            StartCoroutine(atkManager.CheckForAttack());
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
            if (castleManager.hpPoint != 0)
            {
                wave++;
                waveText.text = $"{wave}";
                if (wave == 11)
                {
                    yield return StartCoroutine(commonRewardManager.WaveReward());
                    moveCount = 7;
                    countManager.Initialize(moveCount);
                    yield return StartCoroutine(spawnManager.BossStageSpawnRule());
                }
                yield return StartCoroutine(NextStage());
                FindObjectOfType<UnitD>().ResetDamage();
            }
            else
            {
                LoseGame();
            }
        }
        private void LoseGame()
        {
            StartCoroutine(KillMotion());
            Time.timeScale = 0;
            gamePanel.SetActive(true);
        }
        private IEnumerator NextStage()
        {
            Time.timeScale = 1;
            _bossSpawnArea = new Vector3Int(Random.Range(2,5), 10, 0);
            if (wave % 10 == 0)
            {
                gridManager.ApplyBossSpawnColor(_bossSpawnArea);
            }

            if (EnforceManager.Instance.recoveryCastle)
            {
                castleManager.RecoveryCastle();
            }
            castleManager.UpdatePreviousHp();
            if (wave != 11)
            {
                moveCount = 7;
                countManager.Initialize(moveCount);
            }
            yield return StartCoroutine(backgroundManager.ChangePuzzleSize());
            yield return StartCoroutine(cameraManager.CameraPuzzleSizeChange());
            enemyPool.ClearList();
        }

        private static IEnumerator KillMotion()
        {
            DOTween.KillAll(true);
            yield return null;
        }

        public void RetryGame()
        {
            Time.timeScale = 1;
            StartCoroutine(KillMotion());
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
    }
}