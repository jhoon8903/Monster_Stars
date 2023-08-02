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
using UnityEngine.PlayerLoop;
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

        private const string ResetKey = "ResetKey";

        private DateTime _lastDayCheck;
        private const string LastDayKey = "LastDayKey";

        private int _greenOpenCount;
        private const string GreenOpenCountKey = "GreenOpenCount";
        private const int GreenOpenMaxCount = 20;

        private int _blueOpenCount;
        private const string BlueOpenCountKey = "BlueOpenCount";
        private DateTime _blueOpenTime;
        private const string BlueOpenTimeKey = "BlueOpenTime";
        private TimeSpan _bluePassed;
        private const int BlueOpenMaxCount = 7;
        private const int BlueRewardCoolTime = 5;

        private int _purpleOpenCount;
        private const string PurpleOpenCountKey = "PurpleOpenCount";
        private DateTime _purpleOpenTime;
        private const string PurpleOpenTimeKey = "PurpleOpenTimeKey";
        private TimeSpan _purplePassed;
        private const int PurpleOpenMaxCount = 5;
        private const int PurpleRewardCoolTime = 30;

        private int _coinReward;
        private int _unitPieceReward;
        private readonly Dictionary<CharacterBase, Tuple<int, Goods>> _unitPieceDict = new Dictionary<CharacterBase, Tuple<int, Goods>>();
        private Goods _coinObject;
        private Goods _unitPieceObject;

        public enum BoxGrade { Green, Blue, Purple }
        public bool isReset;

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
            // Day Check
            if (!PlayerPrefs.HasKey(LastDayKey))
            {
                _lastDayCheck = DateTime.Today;
                PlayerPrefs.SetString(LastDayKey, _lastDayCheck.ToBinary().ToString());
                PlayerPrefs.SetInt(ResetKey, 0); // 처음 실행하는 경우 리셋 상태를 0으로 설정합니다.
                PlayerPrefs.Save();
            }
            // Green Count Check
            if (PlayerPrefs.HasKey(GreenOpenCountKey))
            {
                _greenOpenCount = PlayerPrefs.GetInt(GreenOpenCountKey);
            }
            else
            {
                _greenOpenCount = 0;
                PlayerPrefs.SetInt(GreenOpenCountKey, _greenOpenCount);
            }

            // Blue Count Check
            if (PlayerPrefs.HasKey(BlueOpenCountKey))
            {
                _blueOpenCount = PlayerPrefs.GetInt(BlueOpenCountKey);
            }
            else
            {
                _blueOpenCount = 0;
                PlayerPrefs.SetInt(BlueOpenCountKey, _blueOpenCount);
            }

            // Blue last Open Time Check
            if (PlayerPrefs.HasKey(BlueOpenTimeKey))
            {
                _blueOpenTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(BlueOpenTimeKey)));
            }
            else
            {
                _blueOpenTime = DateTime.Now.AddMinutes(-BlueRewardCoolTime);
                PlayerPrefs.SetString(BlueOpenTimeKey, _blueOpenTime.ToBinary().ToString());
                PlayerPrefs.Save();
            }


            // Purple Count Check
            if (PlayerPrefs.HasKey(PurpleOpenCountKey))
            {
                _purpleOpenCount = PlayerPrefs.GetInt(PurpleOpenCountKey);
            }
            else
            {
                _purpleOpenCount = 0;
                PlayerPrefs.SetInt(PurpleOpenCountKey, _purpleOpenCount);
            }
            // Purple Last Open Check
            if (PlayerPrefs.HasKey(PurpleOpenTimeKey))
            {
                _purpleOpenTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(PurpleOpenTimeKey)));
            }
            else
            {
                _purpleOpenTime = DateTime.Now.AddMinutes(-PurpleRewardCoolTime);
                PlayerPrefs.SetString(PurpleOpenTimeKey, _purpleOpenTime.ToBinary().ToString());
                PlayerPrefs.Save();
            }

            greenBtn.GetComponent<Button>().onClick.AddListener(GreenAds);
            blueBtn.GetComponent<Button>().onClick.AddListener(BlueAds);
            purpleBtn.GetComponent<Button>().onClick.AddListener(PurpleAds);
            closeBtn.GetComponent<Button>().onClick.AddListener(ReceiveReward);
            gameObject.SetActive(false);
        }
        private void Update()
        {
            Reset();
            UpdateButtonState();
        }
        private void Reset()
        {
            if (DateTime.Today > _lastDayCheck.Date && PlayerPrefs.GetInt(ResetKey) == 0)
            {
                Debug.Log("리셋?");
                ResetButtonCounts();
                _lastDayCheck = DateTime.Today;
                PlayerPrefs.SetString(LastDayKey, _lastDayCheck.ToBinary().ToString());
                PlayerPrefs.SetInt(ResetKey, 1); // 리셋 상태를 1로 설정하여 리셋이 발생했음을 저장합니다.
                PlayerPrefs.Save();
            }
        }
        private void ResetButtonCounts()
        {
            isReset = true;
            _greenOpenCount = 0;

            _blueOpenCount = 0;
            _blueOpenTime = DateTime.Now.AddMinutes(-BlueRewardCoolTime);
            PlayerPrefs.SetString(BlueOpenTimeKey, _blueOpenTime.ToBinary().ToString());
            _purpleOpenCount = 0;
            _purpleOpenTime = DateTime.Now.AddMinutes(-PurpleRewardCoolTime);
            PlayerPrefs.SetString(PurpleOpenTimeKey, _purpleOpenTime.ToBinary().ToString());
            
            PlayerPrefs.SetInt(GreenOpenCountKey, _greenOpenCount);
            PlayerPrefs.SetInt(BlueOpenCountKey, _blueOpenCount);
            PlayerPrefs.SetInt(PurpleOpenCountKey, _purpleOpenCount);
            PlayerPrefs.Save();
        }

        private void UpdateButtonState()
        {
            UpdateGreenButton();
            UpdateBlueButton();
            UpdatePurpleButton();
        }
        private void UpdateGreenButton()
        {
            if (isReset || _greenOpenCount < GreenOpenMaxCount)
            {
                greenBtn.GetComponent<Button>().interactable = true;
                greenBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"광고보고 \n 보상 열기 \n{_greenOpenCount} / {GreenOpenMaxCount}";
            }
            else
            {
                var resetTime = _lastDayCheck.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
                greenBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"Reset: {resetTime}";
            }
        }

        private void UpdateBlueButton()
        {
            _bluePassed = DateTime.Now - _blueOpenTime;
            if (isReset || (_blueOpenCount < BlueOpenMaxCount && _bluePassed.TotalMinutes >= BlueRewardCoolTime))
            {
                blueBtn.GetComponent<Button>().interactable = true;
                blueBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"광고보고 \n 보상 열기 \n{_blueOpenCount} / {BlueOpenMaxCount}";
            }
            else
            {
                blueBtn.GetComponent<Button>().interactable = false;
                var remainingTime = TimeSpan.FromMinutes(BlueRewardCoolTime) - _bluePassed;
                if (_blueOpenCount == BlueOpenMaxCount)
                {
                    var resetTime = _lastDayCheck.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
                    blueBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"Reset: {resetTime}";
                }
                else if (remainingTime > TimeSpan.Zero)
                {
                    var remainingTimeText = remainingTime.ToString(@"mm\:ss");
                    blueBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = remainingTimeText;
                }
            }
        }

        private void UpdatePurpleButton()
        {
            _purplePassed = DateTime.Now - _purpleOpenTime;
            if (isReset || (_purpleOpenCount < PurpleOpenMaxCount && _purplePassed.TotalMinutes >= PurpleRewardCoolTime))
            {
                purpleBtn.GetComponent<Button>().interactable = true;
                purpleBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"광고보고 \n 보상 열기 \n{_purpleOpenCount} / {PurpleOpenMaxCount}";
            }
            else
            {
                purpleBtn.GetComponent<Button>().interactable = false;
                var remainingTime = TimeSpan.FromMinutes(PurpleRewardCoolTime) - _purplePassed;
                if (_purpleOpenCount == PurpleOpenMaxCount)
                {
                    var resetTime = _lastDayCheck.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
                    purpleBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"Reset: {resetTime}";
                }
                else if (remainingTime > TimeSpan.Zero)
                {
                    var remainingTimeText = remainingTime.ToString(@"mm\:ss");
                    purpleBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = remainingTimeText;
                }
            }
        }

        public void Reward(BoxGrade boxTypes)
        {
            boxRewardPanel.SetActive(true);
            switch (boxTypes)
            {
                case BoxGrade.Green:
                    _greenOpenCount = PlayerPrefs.GetInt(GreenOpenCountKey, 0);
                    CalculateCoinReward(boxTypes, _greenOpenCount);
                    CalculateUnitPieceReward(boxTypes, _greenOpenCount);
                    if (_greenOpenCount == GreenOpenMaxCount) break;
                    _greenOpenCount++;
                    PlayerPrefs.SetInt(GreenOpenCountKey, _greenOpenCount);
                    break;
                case BoxGrade.Blue:
                    _blueOpenCount = PlayerPrefs.GetInt(BlueOpenCountKey, 0);
                    CalculateCoinReward(boxTypes, _blueOpenCount);
                    CalculateUnitPieceReward(boxTypes, _blueOpenCount);
                    if (_blueOpenCount == BlueOpenMaxCount) break;
                    _blueOpenCount++;
                    _blueOpenTime = DateTime.Now;
                    PlayerPrefs.SetInt(BlueOpenCountKey, _blueOpenCount);
                    PlayerPrefs.SetString(BlueOpenTimeKey, _blueOpenTime.ToBinary().ToString());
                    break;
                case BoxGrade.Purple:
                    _purpleOpenCount = PlayerPrefs.GetInt(PurpleOpenCountKey, 0);
                    CalculateCoinReward(boxTypes, _purpleOpenCount);
                    CalculateUnitPieceReward(boxTypes, _purpleOpenCount);
                    if (_purpleOpenCount == PurpleOpenMaxCount) break; 
                    _purpleOpenCount++;
                    _purpleOpenTime = DateTime.Now;
                    PlayerPrefs.SetInt(PurpleOpenCountKey, _purpleOpenCount);
                    PlayerPrefs.SetString(PurpleOpenTimeKey, _purpleOpenTime.ToBinary().ToString());
                    break;
            }
            isReset = false;
            PlayerPrefs.Save(); 
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
            Destroy(_coinObject.gameObject);
            _unitPieceDict.Clear();
            boxRewardPanel.SetActive(false);
        }
        private static void GreenAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Green;
        }
        private static void BlueAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Blue;
        }
        private static void PurpleAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Purple;
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
                var assignedUnits = new List<int>();
                var remainingPieces = totalPiecesPerGrade[grade];

                while (remainingPieces > 0 && unitsOfThisGrade.Count > 0)
                {
                    var randomIndex = Random.Range(0, unitsOfThisGrade.Count);
                    var maxPiecesPerUnit = Math.Max(1, (int)(remainingPieces / (float)unitsOfThisGrade.Count));
                    var piecesForThisUnit = Random.Range(1, maxPiecesPerUnit + 1);

                    pieceCountPerUnit[unitsOfThisGrade[randomIndex]] += piecesForThisUnit;
                    remainingPieces -= piecesForThisUnit;
                    assignedUnits.Add(unitsOfThisGrade[randomIndex]);
                    unitsOfThisGrade.RemoveAt(randomIndex);
                }

                while (remainingPieces > 0 && assignedUnits.Count > 0)
                {
                    var randomIndex = Random.Range(0, assignedUnits.Count);
                    pieceCountPerUnit[assignedUnits[randomIndex]] += 1;
                    remainingPieces -= 1;
                }
            }

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
