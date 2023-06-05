using System.Collections;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
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
        [SerializeField] private EnemySpawnManager enemySpawnManager;
        [SerializeField] private EnemyPatternManager enemyPatternManager;
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private GameObject gamePanel;
        [SerializeField] private GameObject commonRewardPanel;
        [SerializeField] private GameObject expRewardPanel;
        [SerializeField] private AtkManager atkManager;
        [SerializeField] private TextMeshProUGUI speedUpText;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private CommonRewardManager commonRewardManager;
        private readonly WaitForSecondsRealtime _waitOneSecRealtime = new WaitForSecondsRealtime(1f);
        private readonly WaitForSecondsRealtime _waitTwoSecRealtime = new WaitForSecondsRealtime(2f);
        private bool _speedUp = false;
        public int wave = 1;
        private Vector3Int _bossSpawnArea;
        private bool _isBattle = false;
        public bool RecoveryCastle { get; set; } = false;


        private void Start()
        {
            swipeManager.isBusy = true;
            countManager.Initialize(moveCount); // 이동 횟수 초기화
            gridManager.GenerateInitialGrid(); // 초기 그리드 생성
            _speedUp = true;
            waveText.text = $"{wave}";
            GameSpeedSelect();
            StartCoroutine(spawnManager.PositionUpCharacterObject()); // 매치 시작 후 확인
            swipeManager.isBusy = false;
        }
        public IEnumerator Count0Call()
        {
            _isBattle = true;
            yield return _waitOneSecRealtime;
            cameraManager.CameraBattleSizeChange();
            backgroundManager.ChangeBattleSize();
            yield return _waitTwoSecRealtime;
            StartCoroutine(waveManager.StartWave(wave));
            while (enemySpawnManager.fieldList.Count > 0)
            {
                yield return StartCoroutine(WaitForPanelToClose());
                GameSpeed();
                atkManager.CheckForAttack();
                StartCoroutine(enemyPatternManager.Zone_Move());
                yield return _waitOneSecRealtime;
                if (enemySpawnManager.fieldList.Count != 0) continue;
                StartCoroutine(ContinueOrLose());
            }
            _isBattle = false;
        }
        public IEnumerator WaitForPanelToClose()
        {
            if (commonRewardPanel.activeSelf)
            {
                while (commonRewardPanel.activeSelf)
                {
                    yield return null; // Wait until the next frame
                }
            }

            if (expRewardPanel.activeSelf)
            {
                while (expRewardPanel.activeSelf)
                {
                    yield return null; // Wait until the next frame
                }
            }
        }
        public IEnumerator ContinueOrLose()
        {
            DOTween.KillAll(true);
            if (castleManager.hpPoint != 0)
            {
                wave++;
                waveText.text = $"{wave}";
                if (wave == 11)
                {
                    yield return StartCoroutine(commonRewardManager.WaveReward());
                    yield return StartCoroutine(spawnManager.Wave10Spawn());
                }
                NextStage();
            }
            else
            {
                LoseGame();
            }
        }
        private void LoseGame()
        {
            Time.timeScale = 0;
            gamePanel.SetActive(true);
        }
        private void NextStage()
        {
            Time.timeScale = 1;
            _bossSpawnArea = new Vector3Int(Random.Range(2,5), 10, 0);
            var previousWave = wave - 1;
            if (wave % 10 == 0)
            {
                gridManager.ApplyBossSpawnColor(_bossSpawnArea);
            }
            if (previousWave % 10 == 0)
            {
                gridManager.ResetBossSpawnColor();
            }

            if (RecoveryCastle && !castleManager.Damaged)
            {
                castleManager.hpPoint += 200;
                if (castleManager.hpPoint > castleManager.maxHpPoint)
                {
                    castleManager.hpPoint = castleManager.maxHpPoint;
                }
            }

            // Update previous HP
            castleManager.UpdatePreviousHp();
            moveCount = 7;
            countManager.Initialize(moveCount);
            backgroundManager.ChangePuzzleSize();
            cameraManager.CameraPuzzleSizeChange();
        }
        public void RetryGame()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        public void GameSpeedSelect()
        {
            if (_speedUp == false)
            {
                _speedUp = true;
                speedUpText.text = "x2";
            }
            else
            {
                _speedUp = false;
                speedUpText.text = "x1";
            }
        }
        public void GameSpeed()
        {
            if (countManager.baseMoveCount == 0 && _speedUp)
            {
                Time.timeScale = _isBattle ? 2 : 1;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
}