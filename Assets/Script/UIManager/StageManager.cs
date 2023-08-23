#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.QuestGroup;
using Script.RewardScript;
using Script.RobbyScript.MainMenuGroup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EnemyBase = Script.EnemyManagerScript.EnemyBase;

namespace Script.UIManager
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private GameObject continueBtn;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private EnemySpawnManager enemySpawnManager;
        [SerializeField] private EnemyPool enemyPool;
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private CharacterPool characterPool;
        public static StageManager? Instance;
        private int _setCount;
        public int maxWaveCount;
        public int maxStageCount;
        public int latestStage;
        public int currentWave;
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

        private static Dictionary<int, Dictionary<int, (int? wave1SpawnValue, EnemyBase.EnemyClasses? wave1EnemyClass, List<EnemyBase.SpawnZones>? wave1SpawnZone, int? wave2SpawnValue, EnemyBase.EnemyClasses? wave2EnemyClass, List<EnemyBase.SpawnZones>? wave2SpawnZone, int? wave3SpawnValue, EnemyBase.EnemyClasses? wave3EnemyClass, List<EnemyBase.SpawnZones>? wave3SpawnZone, EnemyBase.EnemyClasses? bossClass)>> LoadCsvData(string filename)
        {
            var data = new Dictionary<int, Dictionary<int, (int? wave1SpawnValue, EnemyBase.EnemyClasses? wave1EnemyClass, List<EnemyBase.SpawnZones>? wave1SpawnZone, int? wave2SpawnValue, EnemyBase.EnemyClasses? wave2EnemyClass, List<EnemyBase.SpawnZones>? wave2SpawnZone, int? wave3SpawnValue, EnemyBase.EnemyClasses? wave3EnemyClass, List<EnemyBase.SpawnZones>? wave3SpawnZone, EnemyBase.EnemyClasses? bossClass)>>();
            var csvFile = Resources.Load<TextAsset>(filename);
            var lines = csvFile.text.Split('\n');
            for (var i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(new [] { ',' }, StringSplitOptions.None);
                for (var j = 0; j < values.Length; j++)
                {
                    values[j] = values[j].Trim('"');
                }
                var stage = int.Parse(values[0]);
                var wave = int.Parse(values[1]);
                var wave1SpawnValue = !string.IsNullOrWhiteSpace(values[2]) 
                    ? int.Parse(values[2]) : 0;
                
                var wave1EnemyClass = !string.IsNullOrWhiteSpace(values[3]) 
                    ? ParseEnemyClass(values[3]) 
                    : (EnemyBase.EnemyClasses?)null;
                
                var wave1SpawnZone = !string.IsNullOrWhiteSpace(values[4]) 
                    ? ParseSpawnZones(values[4]) 
                    : null; 
                
                var wave2SpawnValue = !string.IsNullOrWhiteSpace(values[5]) 
                    ? int.Parse(values[5]) 
                    : (int?)null;
                
                var wave2EnemyClass = !string.IsNullOrWhiteSpace(values[6]) 
                    ? ParseEnemyClass(values[6]) 
                    : (EnemyBase.EnemyClasses?)null;
                
                var wave2SpawnZone = !string.IsNullOrWhiteSpace(values[7]) 
                    ? ParseSpawnZones(values[7]) 
                    : null;
                
                var wave3SpawnValue = !string.IsNullOrWhiteSpace(values[8]) 
                    ? int.Parse(values[8]) 
                    : (int?)null;
                
                var wave3EnemyClass = !string.IsNullOrWhiteSpace(values[9]) 
                    ? ParseEnemyClass(values[9]) 
                    : (EnemyBase.EnemyClasses?)null;
                
                var wave3SpawnZone = !string.IsNullOrWhiteSpace(values[10]) 
                    ? ParseSpawnZones(values[10]) 
                    : null;
                
                var bossClass = !string.IsNullOrWhiteSpace(values[11]) 
                    ? ParseEnemyClass(values[11]) 
                    : (EnemyBase.EnemyClasses?)null;
                
                if (!data.ContainsKey(stage))
                {
                    data[stage] = new Dictionary<int, (int? wave1SpawnValue, EnemyBase.EnemyClasses? wave1EnemyClass, List<EnemyBase.SpawnZones>? wave1SpawnZone, int? wave2SpawnValue, EnemyBase.EnemyClasses? wave2EnemyClass, List<EnemyBase.SpawnZones>? wave2SpawnZone, int? wave3SpawnValue, EnemyBase.EnemyClasses? wave3EnemyClass, List<EnemyBase.SpawnZones>? wave3SpawnZone, EnemyBase.EnemyClasses? bossClass)>();
                }

                data[stage][wave] = (wave1SpawnValue, wave1EnemyClass, wave1SpawnZone, wave2SpawnValue, wave2EnemyClass,
                    wave2SpawnZone, wave3SpawnValue, wave3EnemyClass, wave3SpawnZone, bossClass);
            }
            return data;
        }

        private static List<EnemyBase.SpawnZones> ParseSpawnZones(string zoneString)
        {
            var zones = new List<EnemyBase.SpawnZones>();
            var cleanedZoneString = zoneString.Replace("\"", "");
            var zoneStrings = cleanedZoneString.Split(' ');
            foreach (var zone in zoneStrings)
            {
                if (Enum.TryParse(typeof(EnemyBase.SpawnZones), zone, out var parsedZone))
                {
                    zones.Add((EnemyBase.SpawnZones)parsedZone);
                }
            }
            return zones;
        }

        private static EnemyBase.EnemyClasses ParseEnemyClass(string enemyClassString)
        {
            return Enum.TryParse(enemyClassString, out EnemyBase.EnemyClasses enemyClass) ? enemyClass : EnemyBase.EnemyClasses.Farmer;
        }

        private static (int? wave1SpawnValue, EnemyBase.EnemyClasses? wave1EnemyClass, List<EnemyBase.SpawnZones>? wave1SpawnZone, int? wave2SpawnValue, EnemyBase.EnemyClasses? wave2EnemyClass, List<EnemyBase.SpawnZones>? wave2SpawnZone, int? wave3SpawnValue, EnemyBase.EnemyClasses? wave3EnemyClass, List<EnemyBase.SpawnZones>? wave3SpawnZone, EnemyBase.EnemyClasses? bossClass) GetSpawnCountForWave(int stage, int wave)
        {
            var data = LoadCsvData("StageData");

            return data[stage][wave];
        }

        public IEnumerator WaveController()
        {
            var (set1SpawnValue,set1EnemyClass, set1SpawnZone, 
                set2SpawnValue,set2EnemyClass,set2SpawnZone,
                set3SpawnValue, set3EnemyClass, set3SpawnZone
                , bossClass) = GetSpawnCountForWave(selectStage, currentWave);
            const int sets = 2;
            if (currentWave % 10 == 0)
            {
                yield return StartCoroutine(enemySpawnManager.SpawnBoss(bossClass));
            }
            else
            {
                for (var i = 0; i < sets; i++)
                {
                    StartCoroutine(enemySpawnManager.SpawnEnemies(set1SpawnValue, set1EnemyClass, set1SpawnZone));
                    StartCoroutine(enemySpawnManager.SpawnEnemies(set2SpawnValue, set2EnemyClass, set2SpawnZone));
                    StartCoroutine(enemySpawnManager.SpawnEnemies(set3SpawnValue, set3EnemyClass, set3SpawnZone));
                    _setCount++;
                    yield return new WaitForSeconds(3f);
                }
            }
        }

        public void EnemyDestroyEvent(EnemyBase enemyBase)
        {
            enemyPool.enemyBases.Remove(enemyBase);
            if (enemyPool.enemyBases.Count != 0) return;
            if (enemyBase.EnemyType != EnemyBase.EnemyTypes.Boss)
            {
                if (_setCount != 2) return;
                _setCount = 0;
            }
            if (castleManager.HpPoint > 0)
            {
                if (enemyBase.EnemyType == EnemyBase.EnemyTypes.Boss)
                {
                    isBossClear = true;
                    Quest.Instance.KillBossQuest();
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
            EnforceManager.Instance.addRow = false;
            characterPool.theFirst = false;
            ClearRewardManager.Instance.ClearReward(true);
            EnforceManager.Instance.addGoldCount = 0;
            PlayerPrefs.SetInt($"{latestStage}Stage_ProgressWave", 1);
            PlayerPrefs.SetInt($"{latestStage}Stage_ClearWave", MaxWave());
            Firebase.Analytics.FirebaseAnalytics.LogEvent("stage_success", "success", latestStage );
            latestStage++;
            if (latestStage > maxStageCount)
            {
                GameClear();
            }
            PlayerPrefs.SetInt("LatestStage",latestStage);
            PlayerPrefs.SetInt($"{latestStage}Stage_MaxWave", MaxWave());
            PlayerPrefs.DeleteKey("unitState");
            PlayerPrefs.DeleteKey("EnforceData");
            PlayerPrefs.SetInt("GridHeight", 6);
            PlayerPrefs.Save();
            continueBtn.GetComponent<Button>().onClick.AddListener(GameManager.ReturnRobby);
        }
        public void UpdateWaveText()
        {
            waveText.text = $"{currentWave}/{MaxWave()}";
        }
        private static void GameClear()
        {
            
        }
        public void SaveClearWave()
        {
            PlayerPrefs.SetInt($"{latestStage}Stage_ClearWave", currentWave);
            ClearRewardManager.Instance.GetCoin(currentWave);
            ClearRewardManager.Instance.RewardUnitPiece(latestStage, currentWave);
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
