using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.AdsScript;
using Script.CharacterManagerScript;
using Script.QuestGroup;
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
        [SerializeField] private GameObject contents;
        [SerializeField] private GameObject boxRewardPanel;
        [SerializeField] private GameObject boxRewardContents;
        [SerializeField] private GameObject closeBtn;
        [SerializeField] private GameObject adsRewardBtn;
        [SerializeField] private Goods rewardItem;
        [SerializeField] private TreasureChest chestItem;
        [SerializeField] private DailyOffer dailyOffer;
        [SerializeField] private List<CharacterBase> unitList = new List<CharacterBase>();
        [SerializeField] private Sprite errorGemSprite;
        [SerializeField] private Sprite errorCoinSprite;
        [SerializeField] private GameObject chestRewardPanel;
        [SerializeField] private Image chestBackLightImage;
        [SerializeField] private Image chestParticleImage;
        [SerializeField] private GameObject chestGrade;
        [SerializeField] private GameObject chestOpenBtn;
        [SerializeField] private GameObject chestErrorPanel;
        [SerializeField] private GameObject chestErrorCloseBtn;
        [SerializeField] private GameObject getBackToStoreBtn;
        [SerializeField] private GameObject errorContentsImage;
        
        public static StoreMenu Instance { get; private set; }
        public SpecialOffer specialOffer;
        private const string ResetKey = "ResetKey";
        private DateTime _lastDayCheck;
        private const string LastDayKey = "LastDayKey";
        private int _bronzeOpenCount;
        private const string BronzeOpenCountKey = "BronzeOpenCount";
        private const int BronzeOpenMaxCount = 20;
        public int SilverAdsOpen { get; private set; }
        private const string SilverOpenCountKey = "SilverOpenCount";
        private DateTime _silverOpenTime;
        private const string SilverOpenTimeKey = "SilverOpenTime";
        private TimeSpan _silverPassed;
        private const int SilverOpenMaxCount = 7;
        private const int SilverRewardCoolTime = 5;
        public int GoldAdsOpen { get; private set; }
        private const string GoldOpenCountKey = "GoldOpenCount";
        private DateTime _goldOpenTime;
        private const string GoldOpenTimeKey = "GoldOpenTimeKey";
        private TimeSpan _goldPassed;
        private const int GoldOpenMaxCount = 5;
        private const int GoldRewardCoolTime = 30;
        
        private int _coinReward;
        private int _unitPieceReward;
        private readonly Dictionary<CharacterBase, Tuple<int, Goods>> _unitPieceDict = new Dictionary<CharacterBase, Tuple<int, Goods>>();
        private Goods _coinObject;
        private Goods _unitPieceObject;

        public enum BoxGrade { Bronze, Silver, Gold, Coin, Stamina, Gem, BronzeGem, SilverGem, GoldGem }
        public bool isReset;

        public enum ButtonType { BronzeAds, SilverAds, GoldAds, BronzeGem, SilverGem, GoldGem }

        private Vector3 _originalPosition;
        private Vector3 _newPosition;
        private TreasureChest _chestObject;
        private SpecialOffer _specialOffer;
        private DailyOffer _dailyOffer;

     
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
            if (PlayerPrefs.HasKey(BronzeOpenCountKey))
            {
                _bronzeOpenCount = PlayerPrefs.GetInt(BronzeOpenCountKey);
            }
            else
            {
                _bronzeOpenCount = 0;
                PlayerPrefs.SetInt(BronzeOpenCountKey, _bronzeOpenCount);
            }

            // Blue Count Check
            if (PlayerPrefs.HasKey(SilverOpenCountKey))
            {
                SilverAdsOpen = PlayerPrefs.GetInt(SilverOpenCountKey);
            }
            else
            {
                SilverAdsOpen = 0;
                PlayerPrefs.SetInt(SilverOpenCountKey, SilverAdsOpen);
            }

            // Blue last Open Time Check
            if (PlayerPrefs.HasKey(SilverOpenTimeKey))
            {
                _silverOpenTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(SilverOpenTimeKey)));
            }
            else
            {
                _silverOpenTime = DateTime.Now.AddMinutes(-SilverRewardCoolTime);
                PlayerPrefs.SetString(SilverOpenTimeKey, _silverOpenTime.ToBinary().ToString());
                PlayerPrefs.Save();
            }


            // Purple Count Check
            if (PlayerPrefs.HasKey(GoldOpenCountKey))
            {
                GoldAdsOpen = PlayerPrefs.GetInt(GoldOpenCountKey);
            }
            else
            {
                GoldAdsOpen = 0;
                PlayerPrefs.SetInt(GoldOpenCountKey, GoldAdsOpen);
            }
            // Purple Last Open Check
            if (PlayerPrefs.HasKey(GoldOpenTimeKey))
            {
                _goldOpenTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(GoldOpenTimeKey)));
            }
            else
            {
                _goldOpenTime = DateTime.Now.AddMinutes(-GoldRewardCoolTime);
                PlayerPrefs.SetString(GoldOpenTimeKey, _goldOpenTime.ToBinary().ToString());
                PlayerPrefs.Save();
            }
            _chestObject = Instantiate(chestItem, contents.transform);
            _specialOffer = Instantiate(specialOffer, contents.transform);
            _dailyOffer = Instantiate(dailyOffer, contents.transform);
            _specialOffer.transform.SetSiblingIndex(_chestObject.transform.GetSiblingIndex() + 1);
            _dailyOffer.transform.SetSiblingIndex(_specialOffer.transform.GetSiblingIndex() + 1);
        }
        private void Start()
        {
            _chestObject.ChestBtnSet();
            _specialOffer.SpecialBtnSet();
           
            closeBtn.GetComponent<Button>().onClick.AddListener(ReceiveReward);
            // adsRewardBtn.GetComponent<Button>().onClick.AddListener(AdsReceiveReward);
            gameObject.SetActive(false);
            
            _originalPosition = chestGrade.transform.position;
            _newPosition = _originalPosition;
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
                QuestManager.Instance.ResetQuest();
                _lastDayCheck = DateTime.Today;
                PlayerPrefs.SetString(LastDayKey, _lastDayCheck.ToBinary().ToString());
                PlayerPrefs.SetInt(ResetKey, 1); // 리셋 상태를 1로 설정하여 리셋이 발생했음을 저장합니다.
                PlayerPrefs.Save();
            }
        }
        private void ResetButtonCounts()
        {
            isReset = true;
            _bronzeOpenCount = 0;

            SilverAdsOpen = 0;
            _silverOpenTime = DateTime.Now.AddMinutes(-SilverRewardCoolTime);
            PlayerPrefs.SetString(SilverOpenTimeKey, _silverOpenTime.ToBinary().ToString());
            GoldAdsOpen = 0;
            _goldOpenTime = DateTime.Now.AddMinutes(-GoldRewardCoolTime);
            PlayerPrefs.SetString(GoldOpenTimeKey, _goldOpenTime.ToBinary().ToString());
            PlayerPrefs.SetInt(BronzeOpenCountKey, _bronzeOpenCount);
            PlayerPrefs.SetInt(SilverOpenCountKey, SilverAdsOpen);
            PlayerPrefs.SetInt(GoldOpenCountKey, GoldAdsOpen);
            PlayerPrefs.Save();
        }
        private void UpdateButtonState()
        {
            UpdateBronzeButton();
            UpdateSilverButton();
            UpdateGoldButton();
            _chestObject.UpdateBtnColor();
        }
        private void UpdateBronzeButton()
        {
            if (isReset || _bronzeOpenCount < BronzeOpenMaxCount)
            {
                _chestObject.bronzeAdsBtn.GetComponent<Button>().interactable = true;
                _chestObject.bronzeAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{_bronzeOpenCount} / {BronzeOpenMaxCount}";
            }
            else
            {
                var resetTime = _lastDayCheck.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
                _chestObject.bronzeAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"Reset: {resetTime}";
            }
            _chestObject.bronzeGemBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = "150";
        }
        private void UpdateSilverButton()
        {
            _silverPassed = DateTime.Now - _silverOpenTime;
            if (isReset || (SilverAdsOpen < SilverOpenMaxCount && _silverPassed.TotalMinutes >= SilverRewardCoolTime))
            {
                _chestObject.silverAdsBtn.GetComponent<Button>().interactable = true;
                _chestObject.silverAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{SilverAdsOpen} / {SilverOpenMaxCount}";
            }
            else
            {
                _chestObject.silverAdsBtn.GetComponent<Button>().interactable = false;
                var remainingTime = TimeSpan.FromMinutes(SilverRewardCoolTime) - _silverPassed;
                if (SilverAdsOpen == SilverOpenMaxCount)
                {
                    var resetTime = _lastDayCheck.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
                    _chestObject.silverAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"Reset: {resetTime}";
                }
                else if (remainingTime > TimeSpan.Zero)
                {
                    var remainingTimeText = remainingTime.ToString(@"mm\:ss");
                    _chestObject.silverAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = remainingTimeText;
                }
            }
            _chestObject.silverGemBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = "450";
        }
        private void UpdateGoldButton()
        {
            _goldPassed = DateTime.Now - _goldOpenTime;
            if (isReset || (GoldAdsOpen < GoldOpenMaxCount && _goldPassed.TotalMinutes >= GoldRewardCoolTime))
            {
                _chestObject.goldAdsBtn.GetComponent<Button>().interactable = true;
                _chestObject.goldAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{GoldAdsOpen} / {GoldOpenMaxCount}";
            }
            else
            {
                _chestObject.goldAdsBtn.GetComponent<Button>().interactable = false;
                var remainingTime = TimeSpan.FromMinutes(GoldRewardCoolTime) - _goldPassed;
                if (GoldAdsOpen == GoldOpenMaxCount)
                {
                    var resetTime = _lastDayCheck.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
                    _chestObject.goldAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"Reset: {resetTime}";
                }
                else if (remainingTime > TimeSpan.Zero)
                {
                    var remainingTimeText = remainingTime.ToString(@"mm\:ss");
                    _chestObject.goldAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = remainingTimeText;
                }
            }
            _chestObject.goldGemBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = "900";
        }
        public void Reward(BoxGrade boxTypes)
        {
            closeBtn.SetActive(true);
            Debug.Log("boxTypes :"+boxTypes );
            const int count = 0;
            switch (boxTypes)
            {
                case BoxGrade.Bronze:
                    _bronzeOpenCount = PlayerPrefs.GetInt(BronzeOpenCountKey, 0);
                    CalculateCoinReward(boxTypes, _bronzeOpenCount);
                    CalculateUnitPieceReward(boxTypes, _bronzeOpenCount);
                    if (_bronzeOpenCount == BronzeOpenMaxCount) break;
                    _bronzeOpenCount++;
                    PlayerPrefs.SetInt(BronzeOpenCountKey, _bronzeOpenCount);
                    QuestManager.Instance.OpenBoxQuest();
                    break;
                case BoxGrade.Silver:
                    SilverAdsOpen = PlayerPrefs.GetInt(SilverOpenCountKey, 0);
                    CalculateCoinReward(boxTypes, SilverAdsOpen);
                    CalculateUnitPieceReward(boxTypes, SilverAdsOpen);
                    if (SilverAdsOpen == SilverOpenMaxCount) break;
                    SilverAdsOpen++;
                    _silverOpenTime = DateTime.Now;
                    PlayerPrefs.SetInt(SilverOpenCountKey, SilverAdsOpen);
                    PlayerPrefs.SetString(SilverOpenTimeKey, _silverOpenTime.ToBinary().ToString());
                    QuestManager.Instance.OpenBoxQuest();
                    break;
                case BoxGrade.Gold:
                    GoldAdsOpen = PlayerPrefs.GetInt(GoldOpenCountKey, 0);
                    CalculateCoinReward(boxTypes, GoldAdsOpen);
                    CalculateUnitPieceReward(boxTypes, GoldAdsOpen);
                    if (GoldAdsOpen == GoldOpenMaxCount) break; 
                    GoldAdsOpen++;
                    _goldOpenTime = DateTime.Now;
                    PlayerPrefs.SetInt(GoldOpenCountKey, GoldAdsOpen);
                    PlayerPrefs.SetString(GoldOpenTimeKey, _goldOpenTime.ToBinary().ToString());
                    QuestManager.Instance.OpenBoxQuest();
                    break;
                case BoxGrade.Coin:
                    CalculateCoinReward(boxTypes, count);
                    break;
                case BoxGrade.Stamina:
                    CalculateCoinReward(boxTypes, count);
                    break;
                case BoxGrade.Gem:
                    CalculateCoinReward(boxTypes, count);
                    break;
                case BoxGrade.BronzeGem:
                    CalculateCoinReward(boxTypes, count);
                    CalculateUnitPieceReward(boxTypes, count);
                    QuestManager.Instance.OpenBoxQuest();
                    break;
                case BoxGrade.SilverGem:
                    CalculateCoinReward(boxTypes, count);
                    CalculateUnitPieceReward(boxTypes, count);
                    QuestManager.Instance.OpenBoxQuest();
                    break;
                case BoxGrade.GoldGem:
                    CalculateCoinReward(boxTypes, count);
                    CalculateUnitPieceReward(boxTypes, count);
                    QuestManager.Instance.OpenBoxQuest();
                    break;
            }
            isReset = false;
            PlayerPrefs.Save();
        }
        private void ReceiveReward()
        {
            ChestCheck.Instance.CloseChestCheck();
            boxRewardPanel.SetActive(false);
            foreach (var unitReward in _unitPieceDict)
            {
                unitReward.Key.CharacterPeaceCount += unitReward.Value.Item1;
                HoldCharacterList.Instance.UpdateRewardPiece(unitReward.Key);
                Destroy(unitReward.Value.Item2.gameObject);
            }
            Destroy(_coinObject.gameObject);
            _unitPieceDict.Clear();
        }
        private static void BronzeAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_greenbox");
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.BronzeAds;
        }
        private static void SilverAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_bluebox");
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.SilverAds;
        }
        private static void GoldAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_goldbox");
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.GoldAds;
        }
        public void CoinAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Coin;
        }
        public void StaminaAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Stamina;
        }
        public void GemAds()
        {
            ErrorClose();
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Gem;
        }
        private void CalculateCoinReward(BoxGrade boxTypes, int openCount)
        {
            if (_coinObject != null)
            {
                Destroy(_coinObject.gameObject);
            }
            _coinReward = boxTypes switch
            {
                BoxGrade.Bronze => 500,
                BoxGrade.Silver => openCount switch
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
                BoxGrade.Gold => openCount switch
                {
                    0 => 5000,
                    1 => 15000,
                    2 => 30000,
                    3 => 45000,
                    4 => 60000,
                    _ => _coinReward
                },
                BoxGrade.Coin => 1000,
                BoxGrade.Stamina => 10,
                BoxGrade.Gem => 200,
                BoxGrade.BronzeGem => 500,
                BoxGrade.SilverGem => 25000,
                BoxGrade.GoldGem => 60000,
                _ => _coinReward
            };
            if (_coinReward == 0) return;
            _coinObject = Instantiate(rewardItem, boxRewardContents.transform);
            _coinObject.goodsBack.GetComponent<Image>().color = Color.white;
            _coinObject.goodsSprite.GetComponent<Image>().sprite = boxTypes switch
            {
                BoxGrade.Coin => _specialOffer.coinSprite,
                BoxGrade.Stamina => _specialOffer.staminaSprite,
                BoxGrade.Gem => _specialOffer.gemSprite,
                _ => _specialOffer.coinSprite
            };
            _coinObject.goodsSprite.GetComponent<RectTransform>().localScale = boxTypes switch
            {
                BoxGrade.Coin => new Vector3(1, 0.8f, 0),
                BoxGrade.Stamina => new Vector3(0.8f, 0.9f, 0),
                BoxGrade.Gem => new Vector3(1, 0.8f, 0),
                _ => new Vector3(0.8f, 0.8f, 0)
            };
            _coinObject.goodsValue.text = $"{_coinReward}";
            _coinObject.goodsValue.GetComponent<RectTransform>().localScale = boxTypes switch
            {
                BoxGrade.Coin =>  new Vector3(1, 1, 0),
                BoxGrade.Stamina =>  new Vector3(1, 1, 0),
                BoxGrade.Gem =>  new Vector3(1, 1, 0),
                _ =>   new Vector3(1, 1, 0)
            };
            CoinsScript.Instance.Coin += _coinReward;
            CoinsScript.Instance.UpdateCoin();
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
                    unitList[index].UnitGrade == grade && unitList[index].unitPeaceLevel < 14).ToList();
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
                if (unit.unitPeaceLevel >= 14) continue;
                unit.Initialize();
                _unitPieceReward = pieceCountPerUnit[index];
                if (_unitPieceReward == 0) continue;
                _unitPieceObject = Instantiate(rewardItem, boxRewardContents.transform);
                _unitPieceObject.goodsSprite.GetComponent<Image>().sprite = unit.GetSpriteForLevel(unit.unitPeaceLevel);
                _unitPieceObject.goodsSprite.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
                _unitPieceObject.goodsValue.text = $"{_unitPieceReward}";
                _unitPieceObject.goodsValue.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
                _unitPieceDict[unit] = new Tuple<int, Goods>(_unitPieceReward, _unitPieceObject);
            }
            var totalUnitPieces = pieceCountPerUnit.Values.Sum();
            Debug.Log($"Total unit pieces: {totalUnitPieces}");
            QuestManager.Instance.GetPieceQuest(totalUnitPieces);
        }
        private static int GetUnitPieceReward(CharacterBase.UnitGrades unitGrade, BoxGrade boxGrade, int openCount)
                 {
                     int greenValue ;
                     int blueValue;
                     int purpleValue;
                     switch (boxGrade)
                     {
                         case BoxGrade.Bronze:
                             greenValue = 24;
                             blueValue = 0;
                             purpleValue = 0;
                             break;
                         case BoxGrade.Silver:
                             greenValue = 180;
                             blueValue = 24 + 15 * (openCount - 1);
                             purpleValue = openCount > 2 ? openCount - 2 : 0;
                             break;
                         case BoxGrade.Gold:
                             greenValue = 480;
                             blueValue = 165 + 40 * (openCount - 1);
                             purpleValue = openCount > 2 ? 6 + (openCount - 1) * 2 : 6;
                             break;
                         case BoxGrade.BronzeGem:
                             greenValue = 24;
                             blueValue = 0;
                             purpleValue = 0;
                             break;
                         case BoxGrade.SilverGem:
                             greenValue = 180;
                             blueValue = 114;
                             purpleValue = 5;
                             break;
                         case BoxGrade.GoldGem:
                             greenValue = 480;
                             blueValue = 285;
                             purpleValue = 14;
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
        public void ChestCheckClick(ButtonType buttonType)
        {
            ChestCheck.Instance.OpenPanel();

            switch (buttonType)
            {
                case ButtonType.BronzeAds:
                    ChestCheck.Instance.chestCheckTopText.text = "Bronze Chest";
                    ChestCheck.Instance.chestImage.GetComponent<Image>().sprite = _chestObject.bronzeSprite;
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Image>().sprite = _chestObject.chestAdsBtnSprite;
                    ChestCheck.Instance.chestCheckBtnText.text = $"{_bronzeOpenCount} / {BronzeOpenMaxCount}";
                    
                    ChestReward.Instance.SetStart(ButtonType.BronzeAds);
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Button>().onClick.AddListener(BronzeAds);
                    break;
                case ButtonType.SilverAds:
                    ChestCheck.Instance.chestCheckTopText.text = "Silver Chest";
                    ChestCheck.Instance.chestImage.GetComponent<Image>().sprite = _chestObject.silverSprite;
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Image>().sprite = _chestObject.chestAdsBtnSprite;
                    ChestCheck.Instance.chestCheckBtnText.text = $"{SilverAdsOpen} / {SilverOpenMaxCount}";

                    ChestReward.Instance.SetStart(ButtonType.SilverAds);
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Button>().onClick.AddListener(SilverAds);
                    break;
                case ButtonType.GoldAds:
                    ChestCheck.Instance.chestCheckTopText.text = "Gold Chest";
                    ChestCheck.Instance.chestImage.GetComponent<Image>().sprite = _chestObject.goldSprite;
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Image>().sprite = _chestObject.chestAdsBtnSprite;
                    ChestCheck.Instance.chestCheckBtnText.text = $"{GoldAdsOpen} / {GoldOpenMaxCount}";
                    
                    ChestReward.Instance.SetStart(ButtonType.GoldAds);
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Button>().onClick.AddListener(GoldAds);
                    break;
                case ButtonType.BronzeGem:
                    ChestCheck.Instance.chestCheckTopText.text = "Bronze Chest";
                    ChestCheck.Instance.chestImage.GetComponent<Image>().sprite = _chestObject.bronzeSprite;
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Image>().sprite = _chestObject.chestGemBtnSprite;
                    ChestCheck.Instance.chestCheckBtnText.text = "150";
                    ChestReward.Instance.SetStart(ButtonType.BronzeGem);
                    
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Button>().onClick.AddListener(() => CheckAndSummonChest(ButtonType.BronzeGem));
                    break;
                case ButtonType.SilverGem:
                    ChestCheck.Instance.chestCheckTopText.text = "Silver Chest";
                    ChestCheck.Instance.chestImage.GetComponent<Image>().sprite = _chestObject.silverSprite;
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Image>().sprite = _chestObject.chestGemBtnSprite;
                    ChestCheck.Instance.chestCheckBtnText.text = "450";
                    ChestReward.Instance.SetStart(ButtonType.SilverGem);
                   
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Button>().onClick.AddListener(() => CheckAndSummonChest(ButtonType.SilverGem));
                    break;
                case ButtonType.GoldGem:
                    ChestCheck.Instance.chestCheckTopText.text = "Gold Chest";
                    ChestCheck.Instance.chestImage.GetComponent<Image>().sprite = _chestObject.goldSprite;
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Image>().sprite = _chestObject.chestGemBtnSprite;
                    ChestCheck.Instance.chestCheckBtnText.text = "900";
                    ChestReward.Instance.SetStart(ButtonType.GoldGem);
                  
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Button>().onClick.AddListener(() => CheckAndSummonChest(ButtonType.GoldGem));
                    break;
                default:
                    Debug.Log("Unknown button is clicked!");
                    break;
            }
        }
        private void CheckAndSummonChest(ButtonType chestType)
        {
            if (GemScript.Instance.Gem >= int.Parse(ChestCheck.Instance.chestCheckBtnText.text))
            {
                GemScript.Instance.Gem -= int.Parse(ChestCheck.Instance.chestCheckBtnText.text);
                GemScript.Instance.UpdateGem();
                SummonChest(chestType);
            }
            else
            {
                chestErrorPanel.SetActive(true);
                chestErrorCloseBtn.GetComponent<Button>().onClick.AddListener(ErrorClose);
                getBackToStoreBtn.GetComponent<Button>().onClick.AddListener(GemAds);
                errorContentsImage.GetComponent<Image>().sprite = errorGemSprite;
                errorContentsImage.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 1f, 0);
            }
        }
        public void SummonChest(ButtonType chestType)
        {
            chestRewardPanel.SetActive(true);
            chestOpenBtn.GetComponent<Button>().onClick.AddListener(() => OpenChest(chestType));
            StartCoroutine(IncreaseBackLightAlpha(chestType));
        }
        private IEnumerator IncreaseBackLightAlpha(ButtonType chestType)
        {
            chestGrade.gameObject.SetActive(true);
            chestGrade.transform.position = _newPosition;
            Sprite chestSprite = null;
            switch (chestType)
            {
                case ButtonType.BronzeAds:
                    chestSprite = _chestObject.bronzeSprite;
                    break;
                case ButtonType.SilverAds:
                    chestSprite = _chestObject.silverSprite;
                    break;
                case ButtonType.GoldAds:
                    chestSprite = _chestObject.goldSprite;
                    break;
                case ButtonType.BronzeGem:
                    chestSprite = _chestObject.bronzeSprite;
                    break;
                case ButtonType.SilverGem:
                    chestSprite = _chestObject.silverSprite;
                    break;
                case ButtonType.GoldGem:
                    chestSprite = _chestObject.goldSprite;
                    break;
            }
            if (chestSprite != null)
            {
                chestGrade.GetComponentInChildren<Image>().sprite = chestSprite;
            }
            chestParticleImage.color = new Color(1f, 1f, 1f, 0f);
            chestBackLightImage.color = new Color(1f, 1f, 1f, 0f); // 초기 알파 값을 0으로 설정

            // 알파 값을 0에서 1로 서서히 증가시키는 애니메이션
            var lightOn = chestBackLightImage.DOFade(1f, 1.5f);
            
            // goldGem 인 경우에만 파티클을 활성화
            if (chestType == ButtonType.GoldGem)
            {
                // 파티클을 활성화하는 코드 추가 (예: particleObject.SetActive(true);)
                var particleOn = chestParticleImage.DOFade(1f, 1.5f);
            }

            var moveDuration = 1.0f; // 이동 애니메이션의 지속 시간
            var targetY = -0.5f; // 목표 Y 좌표
            var initialScale = new Vector3(0f, 0f, 0f); // 초기 스케일 값
            var targetScale = new Vector3(1f, 1f, 1f); // 목표 스케일 값

            chestGrade.transform.localScale = initialScale; // 초기 스케일 값을 설정

            // chestGrade 오브젝트를 현재 위치에서 (targetY, 현재 z 좌표)로 내려오는 애니메이션을 생성
            var position = chestGrade.transform.position;
            var moveAnimation = chestGrade.transform.DOMove(new Vector3(position.x, targetY, position.z), moveDuration);

            // chestGrade 오브젝트의 스케일을 목표 스케일로 변경하는 애니메이션을 생성
            var scaleAnimation = chestGrade.transform.DOScale(targetScale, moveDuration);

            // 두 개의 애니메이션이 모두 완료될 때까지 대기
            yield return moveAnimation.WaitForCompletion();
            yield return scaleAnimation.WaitForCompletion();
            
            // 애니메이션 완료 후 중지
            moveAnimation.Kill();
            scaleAnimation.Kill();
            
            // 흔들리는 애니메이션을 실행
            var shakeAnimation = chestGrade.transform.DOShakeScale(2.0f, 0.5f, 4);
    
            // 흔들리는 애니메이션이 끝날 때까지 대기
            yield return shakeAnimation.WaitForCompletion();
            shakeAnimation.Kill();
        }
        private void OpenChest(ButtonType chestType)
        {
            ChestCheck.Instance.chestCheckPanel.SetActive(false);
            chestRewardPanel.SetActive(false);
            boxRewardPanel.SetActive(true);
            chestGrade.gameObject.SetActive(false);
            BoxGrade boxGrade;
            switch (chestType)
            {
                case ButtonType.BronzeAds:
                    boxGrade = BoxGrade.Bronze;
                    Reward(boxGrade);
                    break;
                case ButtonType.SilverAds:
                    boxGrade = BoxGrade.Silver;
                    Reward(boxGrade);
                    break;
                case ButtonType.GoldAds:
                    boxGrade = BoxGrade.Gold;
                    Reward(boxGrade);
                    break;
                case ButtonType.BronzeGem:
                    boxGrade = BoxGrade.BronzeGem;
                    Reward(boxGrade);
                    break;
                case ButtonType.SilverGem:
                    boxGrade = BoxGrade.SilverGem;
                    Reward(boxGrade);
                    break;
                case ButtonType.GoldGem:
                    boxGrade = BoxGrade.GoldGem;
                    Reward(boxGrade);
                    break;
            }
        }
        public void OpenAds(BoxGrade adsType)
        {
            // 상자 열기 동작 수행
            Debug.Log("adsType :"+adsType);
            boxRewardPanel.SetActive(true);
            BoxGrade boxGrade;
            switch (adsType)
            {
                case BoxGrade.Coin:
                    boxGrade = BoxGrade.Coin;
                    Reward(boxGrade);
                    break;
                case BoxGrade.Stamina:
                    boxGrade = BoxGrade.Stamina;
                    Reward(boxGrade);
                    break;
                case BoxGrade.Gem:
                    boxGrade = BoxGrade.Gem;
                    Reward(boxGrade);
                    break;
            }
        }
        private void ErrorClose()
        {
            chestErrorPanel.SetActive(false);
        }
        public void DeleteEvent()
        { 
            _specialOffer.SpecialBtnRemove();
            _chestObject.ChestBtnRemove();
            chestOpenBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            closeBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            adsRewardBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            chestOpenBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            chestErrorCloseBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            getBackToStoreBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            ChestCheck.Instance.chestCheckBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            ChestReward.Instance.ClearChests();
            ReBtnSet();
        }
        private void ReBtnSet()
        {
            _chestObject.ChestBtnSet();
            _specialOffer.SpecialBtnSet();
            closeBtn.GetComponent<Button>().onClick.AddListener(ReceiveReward);
        }
    }
}