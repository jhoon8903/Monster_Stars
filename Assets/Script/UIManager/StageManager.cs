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
        [SerializeField] private CastleManager castleManager;
        public static StageManager Instance;
        
        public int maxWaveCount;
        public int maxStageCount;
        public int latestStage;
        public int currentWave;
        public bool isStageClear;
        public bool isBossClear;
        public int selectStage;
        private void Awake()
        {
            Instance = this;
            SelectedStages();
            latestStage = selectStage;
            currentWave = PlayerPrefs.GetInt($"{latestStage}Stage_ProgressWave",1);
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
            selectStage = MainPanel.Instance.SelectStage;
        }
        public IEnumerator WaveController()
        {
            var (group1,group1Zone, group2, group2Zone,group3,group3Zone) = GetSpawnCountForWave(selectStage, currentWave);
            const int sets = 2;
            if (currentWave % 10 == 0)
            {
                yield return StartCoroutine(enemySpawnManager.SpawnBoss(currentWave));
            }
            else
            {
                for (var i = 0; i < sets; i++)
                {
                    StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.Group1, group1, group1Zone));
                    StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.Group2, group2, group2Zone));
                    StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.Group3, group3, group3Zone));
                    yield return new WaitForSeconds(2f);
                }
            }
        }
        private static Dictionary<string, Dictionary<int, (int group1, List<EnemyBase.SpawnZones> group1Zone, int group2, List<EnemyBase.SpawnZones> group2Zone, int group3, List<EnemyBase.SpawnZones> group3Zone)>> LoadCsvData(string filename)
        {
            var data = new Dictionary<string, Dictionary<int, (int group1, List<EnemyBase.SpawnZones> group1Zone, int group2, List<EnemyBase.SpawnZones> group2Zone, int group3, List<EnemyBase.SpawnZones> group3Zone)>>();

            var csvFile = Resources.Load<TextAsset>(filename);
            var lines = csvFile.text.Split('\n');

            for (var i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (var j = 0; j < values.Length; j++)
                {
                    values[j] = values[j].Trim('"');
                }
                var stage = values[0];
                var wave = int.Parse(values[1]);
                var group1 = int.Parse(values[2]);
                var group1Zone = ParseSpawnZones(values[3]);
                var group2 = int.Parse(values[4]);
                var group2Zone = ParseSpawnZones(values[5]);
                var group3 = int.Parse(values[6]);
                var group3Zone = ParseSpawnZones(values[7]);

                if (stage.Contains("~"))
                {
                    var rangeParts = stage.Split('~');
                    var rangeStart = int.Parse(rangeParts[0]);
                    var rangeEnd = int.Parse(rangeParts[1]);

                    for (var s = rangeStart; s <= rangeEnd; s++)
                    {
                        if (!data.ContainsKey(s.ToString()))
                        {
                            data[s.ToString()] = new Dictionary<int, (int group1, List<EnemyBase.SpawnZones> group1Zone, int group2, List<EnemyBase.SpawnZones> group2Zone, int group3, List<EnemyBase.SpawnZones> group3Zone)>();
                        }
                        data[s.ToString()][wave] = (group1,group1Zone, group2, group2Zone,group3,group3Zone);
                    }
                }
                else
                {
                    if (!data.ContainsKey(stage))
                    {
                        data[stage] = new Dictionary<int, (int group1, List<EnemyBase.SpawnZones> group1Zone, int group2, List<EnemyBase.SpawnZones> group2Zone, int group3, List<EnemyBase.SpawnZones> group3Zone)>();
                    }
                    data[stage][wave] = (group1,group1Zone, group2, group2Zone,group3,group3Zone);
                }
            }
            return data;
        }
        private static List<EnemyBase.SpawnZones> ParseSpawnZones(string zoneString)
        {
            var zones = new List<EnemyBase.SpawnZones>();
            var cleanedZoneString = zoneString.Replace("\"", "");
            var zoneStrings = cleanedZoneString.Split(' '); // 공백으로 구분
            foreach (var zone in zoneStrings)
            {
                if (Enum.TryParse(typeof(EnemyBase.SpawnZones), zone, out var parsedZone))
                {
                    zones.Add((EnemyBase.SpawnZones)parsedZone);
                }
            }
            return zones;
        }
        private static (int group1, List<EnemyBase.SpawnZones> group1Zone, int group2, List<EnemyBase.SpawnZones> group2Zone, int group3, List<EnemyBase.SpawnZones> group3Zone) GetSpawnCountForWave(int stage, int wave)
        {
            var data = LoadCsvData("stageData");

            return data[stage.ToString()][wave];
        }
        public void EnemyDestroyEvent(EnemyBase enemyBase)
        {
            enemyPool.enemyBases.Remove(enemyBase);
            if (enemyPool.enemyBases.Count != 0 ) return;
           
            if (castleManager.HpPoint > 0)
            {
                if (enemyBase.EnemyType == EnemyBase.EnemyTypes.Boss)
                {
                    isBossClear = true;
                }
            }

            if (currentWave == MaxWave() && isBossClear )
            {
                PlayerPrefs.SetInt($"{latestStage}Stage_ClearWave", MaxWave());
                StageClear();
            }
            else
            {
                StartCoroutine(GameManager.Instance.ContinueOrLose());
            }
        }
        private void StageClear()
        {
            isStageClear = true;
            CharacterPool.theFirst = false;
            ClearRewardManager.Instance.ClearReward(latestStage);
            EnforceManager.Instance.addGold = false;
            EnforceManager.Instance.addGoldCount = 0;
            PlayerPrefs.SetInt($"{latestStage}Stage_ProgressWave", 1);
            PlayerPrefs.SetInt($"{latestStage}Stage_ClearWave", MaxWave());
            latestStage++;

            if (latestStage > maxStageCount)
            {
                GameClear();
            }

            PlayerPrefs.SetInt("LatestStage",latestStage);
            PlayerPrefs.SetInt($"{latestStage}Stage_MaxWave", MaxWave());
            PlayerPrefs.Save();
            continueBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.ReturnRobby);
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
            PlayerPrefs.SetInt($"{latestStage}Stage_ClearWave", currentWave);
            currentWave++;
            PlayerPrefs.SetInt($"{latestStage}Stage_ProgressWave", currentWave);
            PlayerPrefs.Save();
            UpdateWaveText();
        }
        private int MaxWave()
        {
            maxWaveCount = latestStage switch
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
