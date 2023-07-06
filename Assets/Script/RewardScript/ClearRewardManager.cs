using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.RobbyScript.TopMenuGroup;
using Script.UIManager;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script.RewardScript
{
    public class ClearRewardManager : MonoBehaviour
    {
        [SerializeField] private GameObject stageClearPanel;
        [SerializeField] private Goods goods;
        [SerializeField] private GameObject rewardBox;
        [SerializeField] private List<CharacterBase> rewardUnitList = new List<CharacterBase>();
        public static ClearRewardManager Instance;
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
        }
        public void ClearReward(int stage)
        {
            stageClearPanel.SetActive(true);
            RewardUnitPiece(stage);
            GetCoin(stage, 30);
        }
        public void GetCoin(int stage, int wave)
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
            goods.goodsValue.text = $"{coin}";
            if (!StageManager.Instance.isStageClear) return;
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