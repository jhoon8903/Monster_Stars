using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.QuestGroup;
using Script.RobbyScript.CharacterSelectMenuGroup;
using Script.RobbyScript.TopMenuGroup;
using Script.UIManager;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script.RewardScript
{
    [System.Serializable]
    public class Serialization<TKey, TValue>
    {
        [SerializeField]
        private List<TKey> keys;
        [SerializeField]
        private List<TValue> values;

        public Serialization(Dictionary<TKey, TValue> dictionary)
        {
            keys = new List<TKey>(dictionary.Keys);
            values = new List<TValue>(dictionary.Values);
        }

        public Dictionary<TKey, TValue> ToDictionary()
        {
            return keys.Zip(values, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
        }
    }

    [System.Serializable]
    public class UnitPieceData
    {
        public CharacterBase.UnitGroups unitId;
        public int pieceCount;
        public UnitPieceData(CharacterBase.UnitGroups unitId, int pieceCount)
        {
            this.unitId = unitId;
            this.pieceCount = pieceCount;
        }
    }

    [System.Serializable]
    public class SerializationWrapper<T>
    {
        public List<T> list;

        public SerializationWrapper(List<T> list)
        {
            this.list = list;
        }
    }

    public class ClearRewardManager : MonoBehaviour
    {
        [SerializeField] private Image title;
        [SerializeField] private Sprite winTitle;
        [SerializeField] private Sprite loseTitle;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private GameObject stageClearPanel;
        [SerializeField] private GameObject continueBtn;
        [SerializeField] private List<CharacterBase> rewardUnitList;
        [SerializeField] private Transform rewardGrid;
        [SerializeField] private Transform unitDpsGrid;
        [SerializeField] private TextMeshProUGUI totalDps;
        [SerializeField] private Dps dps;
        [SerializeField] private ItemObject itemObject;
        public static ClearRewardManager Instance;
        public int cumulativeCoin;
        public readonly Dictionary<CharacterBase, int> CumulativeUnitPieces = new Dictionary<CharacterBase, int>();
        private const string CumulativeCoinKey = "CumulativeCoin";
        private const string CumulativeUnitPiecesKey = "CumulativeUnitPieces";
        public bool alreadyPrintPanel;
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
            continueBtn.GetComponent<Button>().onClick.AddListener(ResetCumulativeData);
        }
        public void ClearReward(bool gameResult)
        {
            stageClearPanel.SetActive(true);
            if (gameResult)
            {
                Win();
            }
            else
            {
                Lose();
            }

            LoadCumulativeData();
            if (alreadyPrintPanel) return;
            alreadyPrintPanel = true;
            dps.UnitDps(totalDps,unitDpsGrid);
            itemObject.CoinObject(rewardGrid);
            itemObject.InstantiateCumulativeUnitPieces(rewardGrid);
        }
        private void Win()
        {
            title.sprite = winTitle;
            waveText.text = StageManager.Instance.currentWave.ToString();
            SoundManager.Instance.ClearSoundEffect(SoundManager.Instance.stageClearSound);
        }
        private void Lose()
        {
            title.sprite = loseTitle;
            waveText.text = StageManager.Instance.currentWave.ToString();
            SoundManager.Instance.ClearSoundEffect(SoundManager.Instance.stageFailSound);
        }
        public void GetCoin(int wave)
        {
            var rewardCoin = wave * 50;
            var getCoin = rewardCoin + EnforceManager.Instance.addGoldCount;
            Quest.Instance.GetCoinQuest(getCoin);
            CoinsScript.Instance.Coin += getCoin;
            cumulativeCoin += getCoin;
            CoinsScript.Instance.UpdateCoin();
            EnforceManager.Instance.addGoldCount = 0;
            SaveCumulativeData();
        }
        public void RewardUnitPiece(int stage, int wave)
        {
            var possibleIndices = Enumerable.Range(0, rewardUnitList.Count).ToList();
            var selectedUnitIndices = new List<int>();
            var pieceCountPerUnit = new Dictionary<int, int>();
            foreach (var index in possibleIndices)
            {
                pieceCountPerUnit.TryAdd(index, 0);
            }

            while (possibleIndices.Count > 0)
            {
                var randomIndex = Random.Range(0, possibleIndices.Count);
                selectedUnitIndices.Add(possibleIndices[randomIndex]);
                possibleIndices.RemoveAt(randomIndex);
            }

            var totalPiecesPerGrade = new Dictionary<CharacterBase.UnitGrades, int>()
            {
                { CharacterBase.UnitGrades.G, GetUnitPieceReward(stage,wave, CharacterBase.UnitGrades.G) },
                { CharacterBase.UnitGrades.B, GetUnitPieceReward(stage,wave, CharacterBase.UnitGrades.B) }
            };

            foreach (var grade in totalPiecesPerGrade.Keys)
            {
                var unitsOfThisGrade = selectedUnitIndices.Where(index =>
                    rewardUnitList[index].UnitGrade == grade && rewardUnitList[index].unitPieceLevel < 14).ToList();
                var remainingPieces = totalPiecesPerGrade[grade];
                foreach (var index in unitsOfThisGrade)
                {
                    pieceCountPerUnit.TryAdd(index, 0);
                    if (remainingPieces > 1)
                    {
                        var piecesForThisUnit = Random.Range(1, remainingPieces);
                        pieceCountPerUnit[index] = piecesForThisUnit;
                        remainingPieces -= piecesForThisUnit;
                    }
                    else
                    {
                        pieceCountPerUnit[index] = remainingPieces;
                        remainingPieces = 0;
                        break;
                    }
                }

                while (remainingPieces > 0 && unitsOfThisGrade.Count > 0)
                {
                    for (var i = 0; i < unitsOfThisGrade.Count && remainingPieces > 0; i++)
                    {
                        var index = unitsOfThisGrade[i];
                        pieceCountPerUnit[index]++;
                        remainingPieces--;
                    }
                }
            }

            foreach (var index in selectedUnitIndices)
            {
                var unit = rewardUnitList[index];
                if (unit.unitPieceLevel >= 14) continue;
                var unitPieceReward = pieceCountPerUnit[index];
                if (unitPieceReward == 0) continue;
                CumulativeUnitPieces.TryAdd(unit, 0);
                CumulativeUnitPieces[unit] += unitPieceReward;
                unit.UnitPieceCount += unitPieceReward;
            }
            var totalUnitPieces = pieceCountPerUnit.Values.Sum();
            Quest.Instance.GetPieceQuest(totalUnitPieces);
            SaveCumulativeData();
        }
        private static int GetUnitPieceReward(int stage, int wave, CharacterBase.UnitGrades unitGrade)
        {
            var greenReward = 0;
            var blueReward = 0;

            switch (stage)
            {
                case >= 1 and <= 10:
                    switch (wave)
                    {
                        case 1:
                        case 2: 
                        case 3:
                        case 4:
                        case 5:
                            break;
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                            greenReward = 2;
                            blueReward = 0;
                            break;
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15:
                            greenReward = 5;
                            blueReward = 2;
                            break;
                        case 16:
                        case 17:
                        case 18:
                        case 19:
                        case 20:
                            greenReward = 6;
                            blueReward = 2;
                            break;
                        case 21:
                        case 22:
                        case 23:
                        case 24:
                        case 25:
                            greenReward = 9;
                            blueReward = 4;
                            break;
                        case 26:
                        case 27:
                        case 28:
                        case 29:
                            greenReward = 10;
                            blueReward = 4;
                            break;
                        case 30:
                            greenReward = 15;
                            blueReward = 5;
                            break;
                    }
                    break;
                case <= 20:
                    switch (wave)
                    {
                        case 1:
                        case 2: 
                        case 3:
                        case 4:
                        case 5:
                            break;
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                            greenReward = 5;
                            blueReward = 0;
                            break;
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15:
                            greenReward = 8;
                            blueReward = 3;
                            break;
                        case 16:
                        case 17:
                        case 18:
                        case 19:
                        case 20:
                            greenReward = 9;
                            blueReward = 3;
                            break;
                        case 21:
                        case 22:
                        case 23:
                        case 24:
                        case 25:
                            greenReward = 12;
                            blueReward = 6;
                            break;
                        case 26:
                        case 27:
                        case 28:
                        case 29:
                            greenReward = 13;
                            blueReward = 6;
                            break;
                        case 30:
                            greenReward = 18;
                            blueReward = 10;
                            break;
                    }
                    break;
            }

            return unitGrade switch
            {
                CharacterBase.UnitGrades.G => greenReward,
                CharacterBase.UnitGrades.B => blueReward,
                _ => 0
            };
        }
        private void SaveCumulativeData()
        {
            PlayerPrefs.SetInt(CumulativeCoinKey, cumulativeCoin);
            var unitPieceDataList = CumulativeUnitPieces.Select(pair => new UnitPieceData(pair.Key.unitGroup, pair.Value)).ToList();
            var jsonUnitPieces = JsonUtility.ToJson(new SerializationWrapper<UnitPieceData>(unitPieceDataList));
            PlayerPrefs.SetString(CumulativeUnitPiecesKey, jsonUnitPieces);
            PlayerPrefs.Save();
        }
        private void LoadCumulativeData()
        {
            cumulativeCoin = PlayerPrefs.GetInt(CumulativeCoinKey, 0);
            var jsonUnitPieces = PlayerPrefs.GetString(CumulativeUnitPiecesKey, "");
            var unitPieceDataList = JsonUtility.FromJson<SerializationWrapper<UnitPieceData>>(jsonUnitPieces).list;
            // CumulativeUnitPieces.Clear();

            foreach (var data in unitPieceDataList)
            {
                var unitBase = rewardUnitList.FirstOrDefault(characterBase => characterBase.unitGroup == data.unitId);
                if (unitBase != null)
                {
                    CumulativeUnitPieces[unitBase] = data.pieceCount;
                }
            }
        }
        private void ResetCumulativeData()
        {
            alreadyPrintPanel = false;
            PlayerPrefs.DeleteKey(CumulativeCoinKey);
            PlayerPrefs.DeleteKey(CumulativeUnitPiecesKey);
            cumulativeCoin = 0;
            CumulativeUnitPieces.Clear();
            GameManager.ReturnRobby();
        }
    }
}