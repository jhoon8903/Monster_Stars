using System.Collections;
using DG.Tweening;
using Script.CharacterGroupScript;
using Script.CharacterManagerScript;
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
        [SerializeField] private EnforceManager enforceManager;
        private readonly WaitForSecondsRealtime _waitOneSecRealtime = new WaitForSecondsRealtime(1f);
        private readonly WaitForSecondsRealtime _waitTwoSecRealtime = new WaitForSecondsRealtime(2f);
        public bool speedUp;
        public int wave = 1;
        private Vector3Int _bossSpawnArea;
        public bool isBattle;

        private void Start()
        {
            swipeManager.isBusy = true;
            countManager.Initialize(moveCount); // 이동 횟수 초기화
            gridManager.GenerateInitialGrid(); // 초기 그리드 생성
            speedUp = true;
            waveText.text = $"{wave}";
            GameSpeedSelect();
            StartCoroutine(spawnManager.PositionUpCharacterObject()); // 매치 시작 후 확인
            swipeManager.isBusy = false;
            DOTween.SetTweensCapacity(200000, 500);
        }
        public IEnumerator Count0Call()
        {
            isBattle = true;
            yield return _waitOneSecRealtime;
            cameraManager.CameraBattleSizeChange();
            backgroundManager.ChangeBattleSize();
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
            isBattle = false;
            if (castleManager.hpPoint != 0)
            {
                wave++;
                waveText.text = $"{wave}";
                if (wave == 11)
                {
                    yield return StartCoroutine(commonRewardManager.WaveReward());
                    yield return StartCoroutine(spawnManager.BossStageSpawnRule());
                }
                NextStage();
                FindObjectOfType<UnitD>().ResetDamage();
            }
            else
            {
                LoseGame();
            }
        }
        private void LoseGame()
        {
            DOTween.KillAll(true);
            Time.timeScale = 0;
            gamePanel.SetActive(true);
        }
        private void NextStage()
        {
            DOTween.KillAll(true);
            Time.timeScale = 1;
            _bossSpawnArea = new Vector3Int(Random.Range(2,5), 10, 0);
            if (wave % 10 == 0)
            {
                gridManager.ApplyBossSpawnColor(_bossSpawnArea);
            }

            if (enforceManager.recoveryCastle)
            {
                castleManager.RecoveryCastle();
            }
            moveCount = 7;
            countManager.Initialize(moveCount);
            backgroundManager.ChangePuzzleSize();
            cameraManager.CameraPuzzleSizeChange();
        }
        public void RetryGame()
        {
            DOTween.KillAll(true);
            Time.timeScale = 1;
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
                Time.timeScale = isBattle ? 2 : 1;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
}