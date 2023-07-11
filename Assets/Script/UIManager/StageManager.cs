using System;
using System.Collections;
using System.Collections.Generic;
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
        public bool ClearBoss { get; set; }
        public int currentStage;
        public string currentStageKey = "CurrentStage";
        public int clearStage;
        public string clearStageKey = "ClearStage";
        public int currentWave;
        public string currentWaveKey = "CurrentWave";
        public int clearWave;
        public string clearedWaveKey = "ClearWave";
        public string maxWaveCountKey = "MaxWaves";
        public bool isStageClear;
        private int SelectedStage { get; set; }

        private void Awake()
        {
            Instance = this;
            currentStage = PlayerPrefs.GetInt(currentStageKey, 1);
            clearStage = PlayerPrefs.GetInt(clearStageKey, 1);
            currentWave = PlayerPrefs.GetInt(currentWaveKey, 1);
            clearWave = PlayerPrefs.GetInt(clearedWaveKey, 1);
            SelectedStage = MainPanel.Instance.Stage;
        }

        public int MaxWave()
        {
            maxWaveCount = currentStage switch
            {
                >= 1 and < 5 => 10,
                >= 5 and < 10 => 20,
                _ => 30
            };
            PlayerPrefs.SetInt(maxWaveCountKey, maxWaveCount);
            return maxWaveCount;
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
            StartCoroutine(GameManager.Instance.ContinueOrLose());
        }
        public void StageClear()
        {
            isStageClear = true;
            var listString = PlayerPrefs.GetString("ClearStageList", "");
            var clearStageList = new List<int>();
            if (!string.IsNullOrEmpty(listString))
            {
                clearStageList = new List<int>(Array.ConvertAll(listString.Split(','), int.Parse));
            }
            clearStageList.Add(currentStage);
            PlayerPrefs.SetString("ClearStageList", string.Join(",", clearStageList));
            ClearRewardManager.Instance.ClearReward(currentStage, maxWaveCount);
            clearStage = currentStage;
            PlayerPrefs.SetInt(clearStageKey, clearStage);
            currentStage++;
            if (currentStage > maxStageCount)
            {
                GameClear();
            }
            else if (currentStage > clearStage)
            {
                PlayerPrefs.SetInt(currentStageKey, currentStage);
                MaxWave();
                currentWave = 1;
                clearWave = currentWave;
                PlayerPrefs.SetInt(currentWaveKey, currentWave);
                PlayerPrefs.SetInt(clearedWaveKey, clearWave);
                PlayerPrefs.Save();
            }
            EnforceManager.Instance.addGold = false;
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
            PlayerPrefs.SetInt(clearedWaveKey, clearWave);
            currentWave++;
            PlayerPrefs.SetInt(currentWaveKey, currentWave);
            UpdateWaveText();
        }
    }
}
