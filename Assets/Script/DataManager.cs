using System.Collections.Generic;
using Script.CharacterManagerScript;
using Script.RewardScript;
using UnityEngine;
using File = System.IO.File;

namespace Script
{
    public class UnitData
    {
        public CharacterBase unitGroup;
        public CharacterBase unitLevel;
    }

    public class PowerData
    {
        public ExpData expData;
        public CommonData commonData;
    }
    public class PlayerData
    {
        public List<(UnitData, Vector3Int)> selectUnit = new List<(UnitData, Vector3Int)>();
        public List<PowerData> powerData = new List<PowerData>();
        public int stage;
        public int wave;
        public int coin;
        public int stamina;
        public int gem;
    }

    public class DataManager  : MonoBehaviour
    {
        public static DataManager Instance;
        public PlayerData playerData = new PlayerData();
        private string _path;
        private const string FileName = "save";

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
            _path = Application.persistentDataPath + "/";
        }

        private void SaveData()
        {
            var data = JsonUtility.ToJson(playerData);
            File.WriteAllText(_path + FileName, data);
        }

        private void LoadData()
        {
            var data = File.ReadAllText(_path + FileName);
            playerData = JsonUtility.FromJson<PlayerData>(data);
        }

        // public List<(UnitData, Vector3Int)> SaveUnit()
        // {
        //
        // }

        private void StageAndWave(int currentStage, int currentWave)
        {
            playerData.stage = currentStage;
            playerData.wave = currentWave;
        }
    }
}