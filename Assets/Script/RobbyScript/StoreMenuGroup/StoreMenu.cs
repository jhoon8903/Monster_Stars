using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.AdsScript;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.RobbyScript.CharacterSelectMenuGroup;
using Script.RobbyScript.TopMenuGroup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script.RobbyScript.StoreMenuGroup
{
    public class StoreMenu : MonoBehaviour
    {
        [SerializeField] private GameObject greenBtn;
        [SerializeField] private GameObject blueBtn;
        [SerializeField] private GameObject purpleBtn;
        [SerializeField] private GameObject boxRewardPanel;
        [SerializeField] private GameObject boxRewardContents;
        [SerializeField] private GameObject closeBtn;
        [SerializeField] private Goods rewardItem;
        [SerializeField] private List<CharacterBase> unitList = new List<CharacterBase>();

        public static StoreMenu Instance { get; private set; }
        private const string GreenOpenKey = "GreenOpen";
        private DateTime _lastDayCheck = DateTime.Now;
        private DateTime _greenTime;
        private TimeSpan _greenPassed;
        private const string BlueOpenCountKey = "BlueOpenCount";
        private const string BlueOpenTimeKey = "BlueOpenTime";
        private DateTime _blueTime = DateTime.MinValue;
        private TimeSpan _bluePassed;
        private const string PurpleOpenCountKey = "PurpleOpenCount";
        private const string PurpleOpenTimeKey = "PurpleOpenTimeKey";
        private DateTime _purpleTime = DateTime.MinValue;
        private TimeSpan _purplePassed;
        private BoxGrade _selectBox;
        private int _coinReward;
        private int _unitPieceReward;
        private Dictionary<CharacterBase, Tuple<int, Goods>> _unitPieceDict = new Dictionary<CharacterBase, Tuple<int, Goods>>();
        private Goods _coinObject;
        private Goods _unitPieceObject;
        private const int GreenLimit = 20;
        private int _greenOpenCount;
        private bool _canGreenOpen;
        private const int BlueRewardTime = 5;
        private const int BlueLimit = 7;
        private int _blueOpenCount;
        private bool _canBlueOpen;
        private const int PurpleRewardTime = 30;
        private const int PurpleLimit = 5;
        private int _purpleOpenCount;
        private bool _canPurpleOpen;
        public enum BoxGrade { Green, Blue, Purple }

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
            CheckOpen();
            StartCoroutine(CheckBox());
            closeBtn.GetComponent<Button>().onClick.AddListener(ReceiveReward);
            if (PlayerPrefs.HasKey(BlueOpenTimeKey))
            {
                var temp = Convert.ToInt64(PlayerPrefs.GetString(BlueOpenTimeKey));
                _blueTime = DateTime.FromBinary(temp);
            }
            if (PlayerPrefs.HasKey(PurpleOpenTimeKey))
            {
                var temp = Convert.ToInt64(PlayerPrefs.GetString(PurpleOpenTimeKey));
                _purpleTime = DateTime.FromBinary(temp);
            }
            gameObject.SetActive(false);
        }

        private void ResetButtonCounts()
        {
            _greenOpenCount = 0;
            _blueOpenCount = 0;
            _purpleOpenCount = 0;
            PlayerPrefs.SetInt(GreenOpenKey, _greenOpenCount);
            PlayerPrefs.SetInt(BlueOpenCountKey, _blueOpenCount);
            PlayerPrefs.SetInt(PurpleOpenCountKey, _purpleOpenCount);
        }

        private void CheckOpen()
        {
            _greenOpenCount = PlayerPrefs.GetInt(GreenOpenKey, 0);
            if (_greenOpenCount < GreenLimit)
            {
                _canGreenOpen = true;
            }
            _blueOpenCount = PlayerPrefs.GetInt(BlueOpenCountKey, 0);
            _bluePassed = DateTime.UtcNow - _blueTime;
            if (_blueOpenCount < BlueLimit && _bluePassed.TotalMinutes >= BlueRewardTime)
            {
                _canBlueOpen = true;
            }
            _purpleOpenCount = PlayerPrefs.GetInt(PurpleOpenCountKey, 0);
            _purplePassed = DateTime.UtcNow - _purpleTime;
            if (_purpleOpenCount < PurpleLimit && _purplePassed.TotalMinutes >= PurpleRewardTime)
            {
                _canPurpleOpen = true;
            }
        }

        private IEnumerator CheckBox()
        {
            var buttonDict = new Dictionary<Button, Tuple<bool, Action, int, TimeSpan, int>>
            {
                { greenBtn.GetComponent<Button>(), new Tuple<bool, Action, int, TimeSpan, int>(_canGreenOpen, GreenAds, _greenOpenCount, TimeSpan.Zero, GreenLimit) },
                { blueBtn.GetComponent<Button>(), new Tuple<bool, Action, int, TimeSpan, int>(_canBlueOpen, BlueAds, _blueOpenCount, _bluePassed, BlueLimit) },
                { purpleBtn.GetComponent<Button>(), new Tuple<bool, Action, int, TimeSpan, int>(_canPurpleOpen, PurpleAds, _purpleOpenCount, _purplePassed, PurpleLimit) }
            };

            foreach (var (button, (openTypes, actionTypes, leftCount, timePassed, limit)) in buttonDict)
            {
                if (openTypes)
                {
                    button.interactable = true;
                    button.GetComponentInChildren<TextMeshProUGUI>().text = $"광고보고 \n 보상 열기 \n{leftCount} / {limit}";
                    button.onClick.AddListener(actionTypes.Invoke);
                }
                else
                {
                    button.interactable = false;
                    var remainingTime = TimeSpan.FromMinutes(limit) - timePassed;
                    if (remainingTime.TotalMinutes > 0)
                    {
                        button.GetComponentInChildren<TextMeshProUGUI>().text = remainingTime.ToString(@"mm\:ss");
                    }
                    else
                    {
                        var resetTime = DateTime.Today.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
                        button.GetComponentInChildren<TextMeshProUGUI>().text = $"Reset: {resetTime}";
                    }
                }
            }
            yield return null;
        }

        private void GreenAds()
        {
            AppLovinScript.ShowRewardedAd();
            RewardManager.Instance.RewardButtonClicked("Green");
            _selectBox = BoxGrade.Green;
        }

        private void BlueAds()
        {
            AppLovinScript.ShowRewardedAd();
            RewardManager.Instance.RewardButtonClicked("Blue");
            _selectBox = BoxGrade.Blue;
        }

        private void PurpleAds()
        {
            AppLovinScript.ShowRewardedAd();
            RewardManager.Instance.RewardButtonClicked("Purple");
            _selectBox = BoxGrade.Purple;
        }

        public void Reward(BoxGrade boxTypes)
        {
            boxRewardPanel.SetActive(true);
            switch (boxTypes)
            {
                case BoxGrade.Green:
                    _greenOpenCount = PlayerPrefs.GetInt(GreenOpenKey, 0);
                    CalculateCoinReward(boxTypes, _greenOpenCount);
                    CalculateUnitPieceReward(boxTypes, _greenOpenCount);
                    if (_greenOpenCount >= GreenLimit)
                    {
                        _canGreenOpen = false;
                        PlayerPrefs.SetInt(GreenOpenKey, 0);
                    }
                    else
                    {
                        _canGreenOpen = true;
                        _greenOpenCount++;
                        PlayerPrefs.SetInt(GreenOpenKey, _greenOpenCount);
                    }
                    break;
                case BoxGrade.Blue:
                    _blueOpenCount = PlayerPrefs.GetInt(BlueOpenCountKey, 0);
                    CalculateCoinReward(boxTypes, _blueOpenCount);
                    CalculateUnitPieceReward(boxTypes, _blueOpenCount);
                    if (_blueOpenCount >= BlueLimit)
                    {
                        _canBlueOpen = false;
                        PlayerPrefs.SetInt(BlueOpenCountKey, 0);
                    }
                    else
                    {
                        _canBlueOpen = true;
                        _blueOpenCount++;
                        PlayerPrefs.SetInt(BlueOpenCountKey, _blueOpenCount);
                    }
                    break;
                case BoxGrade.Purple:
                    _purpleOpenCount = PlayerPrefs.GetInt(PurpleOpenCountKey, 0);
                    CalculateCoinReward(boxTypes, _purpleOpenCount);
                    CalculateUnitPieceReward(boxTypes, _purpleOpenCount);
                    if (_purpleOpenCount >= PurpleLimit)
                    {
                        _canPurpleOpen = false;
                        PlayerPrefs.SetInt(PurpleOpenCountKey, 0);
                    }
                    else
                    {
                        _canPurpleOpen = true;
                        _purpleOpenCount++;
                        PlayerPrefs.SetInt(PurpleOpenCountKey, _purpleOpenCount);
                    }
                    break;
            }
        }
        
        private void ReceiveReward()
        { 
            CoinsScript.Instance.Coin += _coinReward;
            CoinsScript.Instance.UpdateCoin();
            foreach (var unitReward in _unitPieceDict)
            {
                unitReward.Key.CharacterPieceCount += unitReward.Value.Item1;
                HoldCharacterList.Instance.UpdateRewardPiece(unitReward.Key);
                Destroy(unitReward.Value.Item2.gameObject);
            }
            switch (_selectBox)
            {
                case BoxGrade.Blue:
                    _blueTime = DateTime.UtcNow;
                    PlayerPrefs.SetString(BlueOpenTimeKey, _blueTime.ToBinary().ToString());
                    break;
                case BoxGrade.Purple:
                   _purpleTime = DateTime.UtcNow;
                   PlayerPrefs.SetString(PurpleOpenTimeKey, _purpleTime.ToBinary().ToString());
                   break;
            }
            CheckOpen();
            StartCoroutine(CheckBox());
            Destroy(_coinObject.gameObject);
            _unitPieceDict.Clear();
            boxRewardPanel.SetActive(false);
        }

        private void CalculateCoinReward(BoxGrade boxTypes, int openCount)
        {
            if (_coinObject != null)
            {
                Destroy(_coinObject.gameObject);
            }
            _coinReward = boxTypes switch
            {
                BoxGrade.Green => 500,
                BoxGrade.Blue => openCount switch
                {
                    0 => 2000,
                    1 => 3000,
                    2 => 5000,
                    3 => 7000,
                    4 => 10000,
                    5 => 15000,
                    6 => 25000,
                    _ => _coinReward
                },
                BoxGrade.Purple => openCount switch
                {
                    0 => 5000,
                    1 => 15000,
                    2 => 30000,
                    3 => 45000,
                    4 => 60000,
                    _ => _coinReward
                },
                _ => _coinReward
            };
            if (_coinReward == 0) return;
            _coinObject = Instantiate(rewardItem, boxRewardContents.transform);
            _coinObject.goodsBack.GetComponent<Image>().color = Color.clear;
            _coinObject.goodsValue.text = $"{_coinReward}";
        }
        private void CalculateUnitPieceReward(BoxGrade boxTypes, int openCount)
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
                { CharacterBase.UnitGrades.Green, GetUnitPieceReward(CharacterBase.UnitGrades.Green, boxTypes, openCount) },
                { CharacterBase.UnitGrades.Blue, GetUnitPieceReward(CharacterBase.UnitGrades.Blue, boxTypes, openCount) },
                { CharacterBase.UnitGrades.Purple, GetUnitPieceReward(CharacterBase.UnitGrades.Purple, boxTypes, openCount) }
            };

            foreach (var grade in totalPiecesPerGrade.Keys)
            {
                var unitsOfThisGrade = selectedUnitIndices.Where(index =>
                    unitList[index].UnitGrade == grade && unitList[index].unitPieceLevel < 14).ToList();
                var remainingPieces = totalPiecesPerGrade[grade];
                var piecesPerUnitGroup = remainingPieces / unitsOfThisGrade.Count;
                foreach (var index in unitsOfThisGrade)
                {
                    pieceCountPerUnit[index] = piecesPerUnitGroup;
                }
                remainingPieces -= piecesPerUnitGroup * unitsOfThisGrade.Count;
                if (remainingPieces <= 0) continue;
                var randomIndex = Random.Range(0, unitsOfThisGrade.Count);
                pieceCountPerUnit[unitsOfThisGrade[randomIndex]] += remainingPieces;
            }
            Debug.Log(selectedUnitIndices.Count);
            foreach (var index in selectedUnitIndices)
            {
                var unit = unitList[index];
                if (unit.unitPieceLevel >= 14) continue;
                unit.Initialize();
                _unitPieceReward = pieceCountPerUnit[index];
                if (_unitPieceReward == 0) continue;
                _unitPieceObject = Instantiate(rewardItem, boxRewardContents.transform);
                _unitPieceObject.goodsBack.GetComponent<Image>().color = unit.UnitGrade switch
                {
                    CharacterBase.UnitGrades.Green => Color.green,
                    CharacterBase.UnitGrades.Blue => Color.blue,
                    CharacterBase.UnitGrades.Purple => Color.magenta,
                    _ => throw new ArgumentOutOfRangeException()
                };
                _unitPieceObject.goodsSprite.GetComponent<Image>().sprite = unit.GetSpriteForLevel(unit.unitPieceLevel);
                _unitPieceObject.goodsSprite.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 0);
                _unitPieceObject.goodsValue.text = $"{_unitPieceReward}";
                _unitPieceObject.goodsValue.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 0);
                _unitPieceDict[unit] = new Tuple<int, Goods>(_unitPieceReward, _unitPieceObject);
            } 
            totalPiecesPerGrade.Clear();
            pieceCountPerUnit.Clear();
            selectedUnitIndices.Clear();
            possibleIndices.Clear();
        }
        private static int GetUnitPieceReward(CharacterBase.UnitGrades unitGrade, BoxGrade boxGrade, int openCount)
        {
            int greenValue;
            int blueValue;
            int purpleValue;
            switch (boxGrade)
            {
                case BoxGrade.Green:
                    greenValue = 24;
                    blueValue = 0;
                    purpleValue = 0;
                    break;
                case BoxGrade.Blue:
                    greenValue = 180;
                    blueValue = 24 + 15 * (openCount - 1);
                    purpleValue = openCount > 2 ? openCount - 2 : 0;
                    break;
                case BoxGrade.Purple:
                    greenValue = 480;
                    blueValue = 125 + 40 * (openCount - 1);
                    purpleValue = openCount > 2 ? 6 + (openCount - 1) * 2 : 6;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(boxGrade), boxGrade, null);
            }

            return unitGrade switch
            {
              CharacterBase.UnitGrades.Green =>  greenValue,
              CharacterBase.UnitGrades.Blue => blueValue,
              CharacterBase.UnitGrades.Purple => purpleValue,
              _ => throw new ArgumentOutOfRangeException(nameof(unitGrade), unitGrade, null)
            };
        }
    }
}
