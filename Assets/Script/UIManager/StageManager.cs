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
            var (group1, group2, group3) = GetSpawnCountForWave(currentStage, currentWaves);
            const int sets = 2;
            if (currentWaves % 10 == 0)
            {
                yield return StartCoroutine(enemySpawnManager.SpawnBoss(currentWaves));
            }
            else
            {
                for (var i = 0; i < sets; i++)
                {
                    yield return StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.Group1, group1));
                    yield return new WaitForSeconds(0.2f);
                    yield return StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.Group2, group2));
                    yield return new WaitForSeconds(0.2f);
                    yield return StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.Group3, group3));
                    yield return new WaitForSeconds(0.2f);
                    yield return new WaitForSeconds(3f);
                }
            }
        }
        private static Dictionary<string, Dictionary<int, (int group1, int group2, int group3)>> LoadCsvData(string filename)
        {
            var data = new Dictionary<string, Dictionary<int, (int group1, int group2, int group3)>>();

            var csvFile = Resources.Load<TextAsset>(filename);
            var lines = csvFile.text.Split('\n');

            for (var i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                var stage = values[0];
                var wave = int.Parse(values[1]);
                var group1 = int.Parse(values[2]);
                var group2 = int.Parse(values[3]);
                var group3 = int.Parse(values[4]);

                if (stage.Contains("~"))
                {
                    var rangeParts = stage.Split('~');
                    var rangeStart = int.Parse(rangeParts[0]);
                    var rangeEnd = int.Parse(rangeParts[1]);

                    for (var s = rangeStart; s <= rangeEnd; s++)
                    {
                        if (!data.ContainsKey(s.ToString()))
                        {
                            data[s.ToString()] = new Dictionary<int, (int group1, int group2, int group3)>();
                        }
                        data[s.ToString()][wave] = (group1, group2, group3);
                    }
                }
                else
                {
                    if (!data.ContainsKey(stage))
                    {
                        data[stage] = new Dictionary<int, (int group1, int group2, int group3)>();
                    }
                    data[stage][wave] = (group1, group2, group3);
                }
            }
            return data;
        }
        private static (int group1, int group2, int group3) GetSpawnCountForWave(int stage, int wave)
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
