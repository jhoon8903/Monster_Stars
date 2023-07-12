using System.Collections;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using Script.RobbyScript.MainMenuGroup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.UIManager
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private GameObject continueBtn;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private EnemySpawnManager enemySpawnManager;
        [SerializeField] private EnemyPool enemyPool;
        public static StageManager Instance;
        public int maxWaveCount;
        public int maxStageCount;
        public int currentStage;
        public string currentStageKey = "CurrentStage";
        public int currentWave;
        public bool isStageClear;
        private int SelectedStage { get; set; }
        public bool isBossClear;

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

        private void Awake()
        {
            Instance = this;
            currentStage = PlayerPrefs.GetInt(currentStageKey, 1);
            var currentWaveKey = "CurrentWave" + currentStage;
            currentWave = PlayerPrefs.GetInt(currentWaveKey, 1);
            SelectedStage = MainPanel.Instance.Stage;
        }

        public void SelectedStages()
        {
            currentStage = SelectedStage;
        }
        public void StartWave()
        {
            StartCoroutine(WaveController(currentWave));
            StartCoroutine(AtkManager.Instance.CheckForAttack());
        }
        private IEnumerator WaveController(int currentWaves)
        {
            var (normalCount, slowCount, fastCount, sets) = GetSpawnCountForWave(currentWaves);

            if (currentWaves % 10 == 0)
            {
                yield return StartCoroutine(enemySpawnManager.SpawnBoss(currentWaves));
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
        private static (int normal, int slow, int fast, int sets) GetSpawnCountForWave(int currentWaves)
        {
            if (currentWaves is 10 or 20) 
            {
                return (0, 0, 0, 0);
            }
            var baseCount = currentWaves;
            if (currentWaves > 10)
            {
                baseCount = currentWaves - 10;
            }
            var normalCount = baseCount + 3;
            var slowCount = baseCount - 1;
            var fastCount = baseCount - 1;
            var sets = currentWaves <= 10 ? 2 : 3;
            return (normalCount, slowCount, fastCount, sets);
        }
        public void EnemyDestroyEvent(EnemyBase enemyBase)
        {
            enemyPool.enemyBases.Remove(enemyBase);
            if (enemyPool.enemyBases.Count != 0 ) return;
            isBossClear = enemyBase.EnemyType == EnemyBase.EnemyTypes.Boss;
            StartCoroutine(GameManager.Instance.ContinueOrLose());
        }
        public void StageClear()
        {
            isStageClear = true;
            ClearRewardManager.Instance.ClearReward(currentStage, maxWaveCount);
            PlayerPrefs.SetInt("ClearWave"+ currentStage, MaxWave());
            currentStage++;
            PlayerPrefs.SetInt(currentStageKey, currentStage);
            PlayerPrefs.SetInt("MaxWave"+currentStage,MaxWave());
            PlayerPrefs.Save();
            if (currentStage > maxStageCount)
            {
                GameClear();
            }
            EnforceManager.Instance.addGold = false;
            EnforceManager.Instance.addGoldCount = 0;
            continueBtn.GetComponent<Button>().onClick.AddListener(PauseManager.ReturnRobby);
        }
        public void UpdateWaveText()
        {
            waveText.text = $"{currentWave}";
        }

        private void GameClear()
        {
            
        }

        public void SaveClearWave()
        {      
            currentWave++;
            PlayerPrefs.SetInt("ClearWave" + currentStage, currentWave);
            var currentWaveKey = "CurrentWave" + currentStage;
            PlayerPrefs.SetInt(currentWaveKey, currentWave);
            PlayerPrefs.Save();
            UpdateWaveText();
        }

        public int MaxWave()
        {
            maxWaveCount = currentStage switch
            {
                1 => 10,
                >= 2 and <= 9 => 20,
                >= 9 and <= 20 => 30,
                _ => maxWaveCount
            };
            return maxWaveCount;
        }
    }
}
