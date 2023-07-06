using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.PuzzleManagerGroup;
using Script.RewardScript;
using Script.RobbyScript.TopMenuGroup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Script.UIManager
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private CommonRewardManager commonRewardManager;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private GameObject continueBtn;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private EnemySpawnManager enemySpawnManager;
        [SerializeField] private EnemyPool enemyPool;
        public static StageManager Instance;
        public int currentStage;
        public int currentWave;
        public int maxWaveCount = 30;
        public int maxStageCount = 50;
        public bool ClearBoss { get; set; }
        private const string ClearedStageKey = "ClearedStage";
        private const string ClearedWaveKey = "ClearedWave";
        public bool isStageClear;

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
            currentStage = PlayerPrefs.GetInt(ClearedStageKey, 1);
            currentWave = PlayerPrefs.GetInt(ClearedWaveKey, 1);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                StageClear();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                PlayerPrefs.DeleteAll();
            }
        }
        private static (int normal, int slow, int fast, int sets) GetSpawnCountForWave(int wave)
        {
            if (wave is 10 or 20) 
            {
                return (0, 0, 0, 0);
            }
            var baseCount = wave;
            if (wave > 10)
            {
                baseCount = wave - 10;
            }
            var normalCount = baseCount + 3;
            var slowCount = baseCount - 1;
            var fastCount = baseCount - 1;
            var sets = wave <= 10 ? 2 : 3;
            return (normalCount, slowCount, fastCount, sets);
        }
        private IEnumerator WaveController(int wave)
        {
            var (normalCount, slowCount, fastCount, sets) = GetSpawnCountForWave(wave);

            if (wave % 10 == 0)
            {
                yield return StartCoroutine(enemySpawnManager.SpawnBoss(wave));
            }
            else
            {
                for (var i = 0; i < sets; i++)
                {
                    var normalCountA = normalCount / 2;
                    var normalCountB = normalCount / 2;

                    if (normalCount % 2 == 1)
                    {
                        normalCountA += 1; 
                    }
                    yield return StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.BasicA, normalCountA));
                    yield return StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.BasicD, normalCountB));
                    yield return StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.Slow, slowCount));
                    yield return StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.Fast, fastCount)); 
                    yield return new WaitForSeconds(3f);
                }
            }
        }
        public void EnemyDestroyEvent(EnemyBase enemyBase)
        {
            enemyPool.enemyBases.Remove(enemyBase);
            if (enemyPool.enemyBases.Count != 0) return;
            StartCoroutine(StageManager.Instance.WaveClear());
        }
        public void StartWave()
        {
            StartCoroutine(WaveController(currentWave));
            StartCoroutine(AtkManager.Instance.CheckForAttack());
        }
        private IEnumerator WaveClear()
        {
            if (currentWave % 10 == 0)
            {
                ClearBoss = true;
                yield return StartCoroutine(commonRewardManager.WaveReward());
                yield return StartCoroutine(spawnManager.BossStageClearRule());
            }
            else if (currentWave == maxWaveCount)
            {
                StageClear();
            }
            else
            {
                yield return StartCoroutine(GameManager.Instance.ContinueOrLose());
            }

            ClearRewardManager.Instance.GetCoin(currentStage, currentWave);
            currentWave++;
            PlayerPrefs.SetInt(ClearedWaveKey, currentWave);
            UpdateWaveText();
        }
        private void StageClear()
        {
            isStageClear = true;
            var clearStage = currentStage;
            ClearRewardManager.Instance.ClearReward(clearStage);
            currentStage++;
            currentWave = 1;
            PlayerPrefs.SetInt(ClearedStageKey, currentStage);
            PlayerPrefs.SetInt(ClearedWaveKey, currentWave);
            PlayerPrefs.Save();
            continueBtn.GetComponent<Button>().onClick.AddListener(PauseManager.ReturnRobby);
        }
        public void UpdateWaveText()
        {
            waveText.text = $"{currentWave}";
        }
    }
}
