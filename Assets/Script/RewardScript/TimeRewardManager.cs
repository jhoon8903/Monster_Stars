using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.RobbyScript.CharacterSelectMenuGroup;
using Script.RobbyScript.StoreMenuGroup;
using Script.RobbyScript.TopMenuGroup;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
#pragma warning disable CS8509

namespace Script.RewardScript
{
    public class TimeRewardManager : MonoBehaviour
    {
        [SerializeField] private GameObject timeRewardPanel;
        [SerializeField] private GameObject timeRewardTop;
        [SerializeField] private GameObject timeRewardLevel;
        [SerializeField] private GameObject timeRewardTime;
        [SerializeField] private GameObject timeRewardContents;
        [SerializeField] private GameObject timeRewardBtn;
        [SerializeField] private GameObject timeRewardCloseBtn;
        [SerializeField] private Goods rewardItem;
        [SerializeField] private List<CharacterBase> unitList = new List<CharacterBase>();
        public static TimeRewardManager Instance { get; private set; }
        private TextMeshProUGUI _rewardLevelText;
        private TextMeshProUGUI _rewardTimeText;
        private int _hour;
        private int _min;
        private int _sec;
        private const string LatestOpenTimeKey = "LatestOpenTime";
        private DateTime _latestOpenTime;
        private TimeSpan _timePassed;
        private int _coinReward;
        private int _unitPieceReward;
        private readonly Dictionary<CharacterBase, Tuple<int, Goods>> _unitPieceDict = new Dictionary<CharacterBase, Tuple<int, Goods>>();
        private Goods _coinObject;
        private Goods _unitPieceObject;
        private const int MaxHours = 16;
        private const int CoinRewardTime = 10;
        private const int GreenPieceRewardTime = 30;
        private const int BluePieceRewardTime = 1;

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
            _rewardLevelText = timeRewardLevel.GetComponentInChildren<TextMeshProUGUI>();
            _rewardTimeText = timeRewardTime.GetComponentInChildren<TextMeshProUGUI>();
            timeRewardCloseBtn.GetComponent<Button>().onClick.AddListener(CloseTimeReward);
            var latestReceiveRewardTime = PlayerPrefs.GetString(LatestOpenTimeKey, null);
            if (!string.IsNullOrEmpty(latestReceiveRewardTime))
            {
                _latestOpenTime = DateTime.FromBinary(Convert.ToInt64(latestReceiveRewardTime));
            }
            else
            {
                _latestOpenTime = DateTime.Now;
                PlayerPrefs.SetString(LatestOpenTimeKey, _latestOpenTime.ToBinary().ToString());
            }
            timeRewardBtn.GetComponent<Button>().onClick.AddListener(ReceiveTimeReward);
        }
        public void OpenPanel()
        {
            timeRewardPanel.SetActive(true);
            ShowTimeReward();
            StartCoroutine(UpdateReward());
            timeRewardBtn.GetComponent<Button>().interactable = timeRewardContents.transform.childCount > 0;
        }
        private void ShowTimeReward()
        {
            var latestStage = PlayerPrefs.GetInt("LatestStage", 1);
            _timePassed = DateTime.Now - _latestOpenTime;
            _timePassed = TimeSpan.FromHours(Math.Min(_timePassed.TotalHours, MaxHours));
            CalculateCoinReward(_timePassed, latestStage);

            // 기존 딕셔너리에 있는 보상 정보 업데이트
            foreach (var unitPiece in _unitPieceDict)
            {
                unitPiece.Key.CharacterPeaceCount += unitPiece.Value.Item1;
            }

            // 보상 정보 불러오기 및 업데이트
            foreach (var unit in unitList)
            {
                int savedRewardCount = PlayerPrefs.GetInt($"UnitReward_{unit.unitGroup}", 0);
                Debug.Log($"PlayerPrefs key: UnitReward_{unit.unitGroup}, savedRewardCount: {savedRewardCount}");

                if (savedRewardCount > 0)
                {
                    if (_unitPieceDict.ContainsKey(unit))
                    {
                        _unitPieceDict[unit] = new Tuple<int, Goods>(savedRewardCount, null); 
                    }
                    else
                    {
                        _unitPieceDict.Add(unit, new Tuple<int, Goods>(savedRewardCount, null));
                    }
                }
            }
            CalculateUnitPieceReward(_timePassed, latestStage);
        }
        private void CalculateCoinReward(TimeSpan timePassed, int stage)
        {
            if (_coinObject != null)
            {
                Destroy(_coinObject.gameObject);
            }
            var coinValue = stage * 5 + 20;
            _coinReward = coinValue * (int)(timePassed.TotalMinutes / CoinRewardTime);
            if (_coinReward == 0) return;
            _coinObject = Instantiate(rewardItem, timeRewardContents.transform);
            _coinObject.goodsBack.GetComponent<Image>().color = Color.white;
            _coinObject.goodsSprite.GetComponent<Image>().sprite = StoreMenu.Instance.specialOffer.coinSprite;
            _coinObject.goodsValue.text = $"{_coinReward}";
        }
        private void CalculateUnitPieceReward(TimeSpan timePassed, int stage)
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
                { CharacterBase.UnitGrades.Green, GetUnitPieceReward(stage, CharacterBase.UnitGrades.Green, timePassed) },
                { CharacterBase.UnitGrades.Blue, GetUnitPieceReward(stage, CharacterBase.UnitGrades.Blue, timePassed) }
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
                _unitPieceObject = Instantiate(rewardItem, timeRewardContents.transform);
                _unitPieceObject.goodsSprite.GetComponent<Image>().sprite = unit.GetSpriteForLevel(unit.unitPeaceLevel);
                _unitPieceObject.goodsSprite.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
                _unitPieceObject.goodsValue.text = $"{_unitPieceReward}";
                _unitPieceObject.goodsValue.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
                _unitPieceDict[unit] = new Tuple<int, Goods>(_unitPieceReward, _unitPieceObject);
            }
        }
        private static int GetUnitPieceReward(int stage, CharacterBase.UnitGrades unitGrade, TimeSpan timePassed)
        {
            var blueReward = 0;
            if (stage >= 10)
            {
                blueReward = stage switch
                {
                    10 or 11 => 1,
                    12 or 13 => 2,
                    14 or 15 => 3,
                    16 or 17 => 4,
                    18 or 20 => 5,
                };
            }

            return unitGrade switch
            {
              CharacterBase.UnitGrades.Green => stage * (int)(timePassed.TotalMinutes / GreenPieceRewardTime),
              CharacterBase.UnitGrades.Blue => blueReward * (int)(timePassed.TotalHours / BluePieceRewardTime),
            };
        }
        private IEnumerator UpdateReward()
        {
            while (timeRewardPanel.activeInHierarchy)
            {
                var timePassed = DateTime.Now - _latestOpenTime;
                timePassed = TimeSpan.FromHours(Math.Min(timePassed.TotalHours, MaxHours));
                _hour = timePassed.Hours;
                _min = timePassed.Minutes;
                _sec = timePassed.Seconds;
                _rewardLevelText.text = $"보상레벨 [{PlayerPrefs.GetInt("LatestStage", 1)} Lv]";
                _rewardTimeText.text = $"보상누적 [{_hour:D2}:{_min:D2}:{_sec:D2}]";
                yield return new WaitForSecondsRealtime(1f);
            }
        }
        private void ReceiveTimeReward()
        {
            CoinsScript.Instance.Coin += _coinReward;
            CoinsScript.Instance.UpdateCoin();
            _latestOpenTime = DateTime.Now;
            PlayerPrefs.SetString(LatestOpenTimeKey, _latestOpenTime.ToBinary().ToString());
            foreach (var unitPiece in _unitPieceDict)
            {
                unitPiece.Key.CharacterPeaceCount += unitPiece.Value.Item1;
                HoldCharacterList.Instance.UpdateRewardPiece(unitPiece.Key);
                Destroy(unitPiece.Value.Item2.gameObject);
            }
            Destroy(_coinObject.gameObject);
            _unitPieceDict.Clear();
            timeRewardPanel.SetActive(false);
        }
        private void CloseTimeReward()
        {
            // 저장된 유닛 보상 정보 제거
            foreach (var unit in unitList)
            {
                // 보상 정보 저장
                if (_unitPieceDict.ContainsKey(unit))
                {
                    PlayerPrefs.SetInt($"UnitReward_{unit.unitGroup}", _unitPieceDict[unit]?.Item1 ?? 0);
                }
            }

            // 보상 객체들 삭제
            foreach (var unitPiece in _unitPieceDict)
            {
                if (unitPiece.Value.Item2 != null)
                {
                    Destroy(unitPiece.Value.Item2.gameObject);
                }
            }

            // 유닛 보상 딕셔너리 초기화
            _unitPieceDict.Clear();
            timeRewardBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            timeRewardPanel.SetActive(false);
        }
    }   
}