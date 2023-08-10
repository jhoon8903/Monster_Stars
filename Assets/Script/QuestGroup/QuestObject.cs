using System;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.RobbyScript.CharacterSelectMenuGroup;
using Script.RobbyScript.TopMenuGroup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script.QuestGroup
{
    public class QuestObject : MonoBehaviour
    {
        [SerializeField] private List<CharacterBase> unitList = new List<CharacterBase>();
        [SerializeField] private TextMeshProUGUI questDesc;
        [SerializeField] private GameObject rewardItem1;
        [SerializeField] private GameObject rewardItem2;
        [SerializeField] private Slider rewardProgress;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private GameObject rewardBtn;
        [SerializeField] private GameObject rewardPanel;
        [SerializeField] private Goods rewardItem;

        public enum QuestConditions { Fix, Rotation }
        public QuestConditions questCondition;
        public enum QuestTypes { AllClear, UseCoin, GetCoin, OpenBox, GetPiece, KillEnemy, KillBoss, ViewAds, MatchCoin, Victory }
        public QuestTypes questType;
        public int questGoal;
        public int rewardCoin;
        public int rewardGreenPiece;
        public int rewardBluePiece;
        public int rewardPurplePiece;
        public string questDescKor;
        public string questDescEng;
        private Goods _unitPieceObject;
        private Goods _coinObject;
        private int _coinReward;
        private int _unitPieceReward;
        public Language.LanguageType selectedLanguage;
        private readonly Dictionary<CharacterBase, Tuple<int, Goods>> _unitPieceDict = new Dictionary<CharacterBase, Tuple<int, Goods>>();


        private void Start()
        {
            UpdateDesc();
            UpdateProgress(0);
            ShowItems();
        }

        public void UpdateDesc()
        {
            if (PlayerPrefs.HasKey("Language"))
            {
                var savedLanguage = PlayerPrefs.GetString("Language", "KOR");
                selectedLanguage = savedLanguage switch
                {
                    "KOR" => Language.LanguageType.Korean,
                    "ENG" => Language.LanguageType.English,
                    _ => selectedLanguage
                };
            }
            questDesc.text = selectedLanguage == Language.LanguageType.Korean ? questDescKor : questDescEng;
        }
        public void UpdateProgress(int progress)
        {
            rewardProgress.value = progress;
            rewardProgress.maxValue = questGoal;
        }
        public void ShowItems()
        {
            rewardItem1.GetComponentInChildren<TextMeshProUGUI>().text = rewardCoin.ToString();
            rewardItem2.GetComponentInChildren<TextMeshProUGUI>().text =
                (rewardGreenPiece + rewardBluePiece + rewardPurplePiece).ToString();
        }
        private void ShowCoin()
        {
            _coinReward = rewardCoin;
            if (_coinReward == 0) return;
            _coinObject = Instantiate(rewardItem, rewardPanel.transform);
            _coinObject.goodsBack.GetComponent<Image>().color = Color.gray;
            _coinObject.goodsValue.text = $"{_coinReward}";
        }
        private void ShowPiece()
        {
            var possibleIndices = Enumerable.Range(0, unitList.Count).ToList();
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

            var totalPiecesPerGrade = new Dictionary<CharacterBase.UnitGrades, int>
            {
                { CharacterBase.UnitGrades.Green, GetUnitPieceReward(CharacterBase.UnitGrades.Green, rewardGreenPiece) },
                { CharacterBase.UnitGrades.Blue, GetUnitPieceReward(CharacterBase.UnitGrades.Blue, rewardBluePiece) },
                { CharacterBase.UnitGrades.Purple, GetUnitPieceReward(CharacterBase.UnitGrades.Purple, rewardPurplePiece)}
            };

            foreach (var grade in totalPiecesPerGrade.Keys)
            {
                var unitsOfThisGrade = selectedUnitIndices.Where(index =>
                    unitList[index].UnitGrade == grade && unitList[index].unitPeaceLevel < 14).ToList();
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
                var unit = unitList[index];
                if (unit.unitPeaceLevel >= 14) continue;
                unit.Initialize();
                _unitPieceReward = pieceCountPerUnit[index];
                if (_unitPieceReward == 0) continue;
                _unitPieceObject = Instantiate(rewardItem, rewardPanel.transform);
                _unitPieceObject.goodsBack.GetComponent<Image>().color = unit.UnitGrade switch
                {
                    CharacterBase.UnitGrades.Green => Color.green,
                    CharacterBase.UnitGrades.Blue => Color.blue,
                    CharacterBase.UnitGrades.Purple => Color.magenta
                };
                _unitPieceObject.goodsSprite.GetComponent<Image>().sprite = unit.GetSpriteForLevel(unit.unitPeaceLevel);
                _unitPieceObject.goodsSprite.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 0);
                _unitPieceObject.goodsValue.text = $"{_unitPieceReward}";
                _unitPieceObject.goodsValue.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 0);
                _unitPieceDict[unit] = new Tuple<int, Goods>(_unitPieceReward, _unitPieceObject);
            }
        }
        private static int GetUnitPieceReward(CharacterBase.UnitGrades unitGrade, int rewardPiece)
        {

            return unitGrade switch
            {
                CharacterBase.UnitGrades.Green => rewardPiece,
                CharacterBase.UnitGrades.Blue => rewardPiece,
                CharacterBase.UnitGrades.Purple => rewardPiece,
            };
        }
        private void ReceiveReward()
        {
            if (_coinObject != null)
            {
                Destroy(_coinObject.gameObject);
            }
            CoinsScript.Instance.Coin += _coinReward;
            CoinsScript.Instance.UpdateCoin();
            foreach (var unitPiece in _unitPieceDict)
            {
                unitPiece.Key.CharacterPeaceCount += unitPiece.Value.Item1;
                HoldCharacterList.Instance.UpdateRewardPiece(unitPiece.Key);
                Destroy(unitPiece.Value.Item2.gameObject);
            }
            Destroy(_coinObject.gameObject);
            _unitPieceDict.Clear();
        }
    }
}
