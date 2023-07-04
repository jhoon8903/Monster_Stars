using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
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
        [SerializeField] private GameObject stageClearPanel;
        [SerializeField] private GameObject rewardBox;
        [SerializeField] private GameObject continueBtn;
        [SerializeField] private Goods goods;
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private TextMeshProUGUI waveText;



        public static StageManager Instance { get; private set; }
        public int currentStage;
        public int currentWave;
        public int maxWaveCount = 30;
        public int maxStageCount = 50;
        public bool ClearBoss { get; set; }
        private const string ClearedStageKey = "ClearedStage";
        private const string ClearedWaveKey = "ClearedWave";
        public List<CharacterBase> rewardUnitList = new List<CharacterBase>();
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

        public void StartWave()
        {
            StartCoroutine(waveManager.WaveController(currentWave));
            StartCoroutine(AtkManager.Instance.CheckForAttack());
        }

        public IEnumerator WaveClear()
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

            GetCoin(currentStage, currentWave);
            currentWave++;
            PlayerPrefs.SetInt(ClearedWaveKey, currentWave); // currentWave 값을 저장하는 코드 추가
            UpdateWaveText();
        }

        private void StageClear()
        {
            isStageClear = true;
            var clearStage = currentStage;
            ClearReward(clearStage);
            currentStage++;
            currentWave = 1; // Stage가 clear 되면 wave는 다시 1로 초기화
            PlayerPrefs.SetInt(ClearedStageKey, currentStage);
            PlayerPrefs.SetInt(ClearedWaveKey, currentWave); // currentWave 값을 저장하는 코드 추가
            PlayerPrefs.Save();
            continueBtn.GetComponent<Button>().onClick.AddListener(LoadRobby);
        }

        public void LoadRobby()
        {
            SceneManager.LoadScene("SelectScene");
        }

        public void UpdateWaveText()
        {
            waveText.text = $"{currentWave}";
        }

        private void ClearReward(int stage)
        {
            stageClearPanel.SetActive(true);
            RewardUnitPiece(stage);
            GetCoin(stage, 30);
            isStageClear = false;
        }

        private void GetCoin(int stage, int wave)
        {
            var coin = stage switch
            {
                >= 1 and <= 10 => 7 * wave,
                <= 20 => 10 + 7 * wave,
                <= 30 => 20 + 7 * wave,
                <= 40 => 30 + 7 * wave,
                _ => 40 + 7 * wave
            };
            CoinsScript.Instance.Coin += coin;
            CoinsScript.Instance.UpdateCoin();
            if (!isStageClear) return;
            Instantiate(goods, rewardBox.transform);
            goods.goodsBack.GetComponent<Image>().color = Color.cyan;
            goods.goodsValue.text = $"{coin}";
        }

        private void RewardUnitPiece(int stage)
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
                { CharacterBase.UnitGrades.Green, GetUnitPieceReward(stage, CharacterBase.UnitGrades.Green) },
                { CharacterBase.UnitGrades.Blue, GetUnitPieceReward(stage, CharacterBase.UnitGrades.Blue) },
                { CharacterBase.UnitGrades.Purple, GetUnitPieceReward(stage, CharacterBase.UnitGrades.Purple) }
            };

            foreach (var grade in totalPiecesPerGrade.Keys)
            {
                var unitsOfThisGrade = selectedUnitIndices.Where(index => rewardUnitList[index].UnitGrade == grade).ToList();
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
                        Debug.Log("Assigning remaining pieces: " + remainingPieces);
                    }
                }
            }

            foreach (var index in selectedUnitIndices)
            {
                var unit = rewardUnitList[index];
                unit.Initialize();
                var unitPieceReward = pieceCountPerUnit[index];
                if (unitPieceReward == 0)
                {
                    continue;
                }

                var goodies = Instantiate(goods, rewardBox.transform);
                goodies.goodsBack.GetComponent<Image>().color = unit.UnitGrade switch
                {
                    CharacterBase.UnitGrades.Green => Color.green,
                    CharacterBase.UnitGrades.Blue => Color.blue,
                    CharacterBase.UnitGrades.Purple => Color.magenta,
                    _ => Color.gray
                };
                goodies.goodsSprite.GetComponent<Image>().sprite = unit.GetSpriteForLevel(1);
    
                unit.CharacterPieceCount += unitPieceReward;
                goodies.goodsValue.text = $"{unitPieceReward}";
            }
        }
        private static int GetUnitPieceReward(int stage, CharacterBase.UnitGrades unitGrade)
        {
            var greenReward = 0;
            var blueReward = 0;
            var purpleReward = 0;
            switch (stage)
            {
                case >= 1 and <= 10:
                    greenReward = 24;
                    blueReward = 1;
                    purpleReward = 0;
                    break;
                case <= 20:
                    greenReward = 26;
                    blueReward = 1;
                    purpleReward = 0;
                    break;
                case <= 30:
                    greenReward = 28;
                    blueReward = 2;
                    purpleReward = 0;
                    break;
                case <= 40:
                    greenReward = 30;
                    blueReward = 2;
                    purpleReward = 0;
                    break;
                case <= 50:
                    greenReward = 32;
                    blueReward = 2;
                    purpleReward = 1;
                    break;
            }
            return unitGrade switch
            {
                CharacterBase.UnitGrades.Green => greenReward,
                CharacterBase.UnitGrades.Blue => blueReward,
                CharacterBase.UnitGrades.Purple => purpleReward,
                _ => 0
            };
        }
    }
}
