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
            var (normalCount, slowCount, fastCount) = GetSpawnCountForWave(currentStage, currentWaves);
            Debug.Log($"{normalCount} / {slowCount} / {fastCount}");

            const int sets = 2;
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
        private static Dictionary<string, Dictionary<int, (int normal, int slow, int fast)>> LoadCsvData(string filename)
        {
            var data = new Dictionary<string, Dictionary<int, (int normal, int slow, int fast)>>();

            var csvFile = Resources.Load<TextAsset>(filename);
            var lines = csvFile.text.Split('\n');

            for (var i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                var stage = values[0];
                var wave = int.Parse(values[1]);
                var normal = int.Parse(values[2]);
                var slow = int.Parse(values[3]);
                var fast = int.Parse(values[4]);

                if (stage.Contains("~"))
                {
                    var rangeParts = stage.Split('~');
                    var rangeStart = int.Parse(rangeParts[0]);
                    var rangeEnd = int.Parse(rangeParts[1]);

                    for (var s = rangeStart; s <= rangeEnd; s++)
                    {
                        if (!data.ContainsKey(s.ToString()))
                        {
                            data[s.ToString()] = new Dictionary<int, (int normal, int slow, int fast)>();
                        }
                        data[s.ToString()][wave] = (normal, slow, fast);
                    }
                }
                else
                {
                    if (!data.ContainsKey(stage))
                    {
                        data[stage] = new Dictionary<int, (int normal, int slow, int fast)>();
                    }
                    data[stage][wave] = (normal, slow, fast);
                }
            }
            return data;
        }
        private static (int normal, int slow, int fast) GetSpawnCountForWave(int stage, int wave)
        {
            var data = LoadCsvData("stageData");

            return data[stage.ToString()][wave];
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
