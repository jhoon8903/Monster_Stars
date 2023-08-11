using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.AdsScript;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.RobbyScript.CharacterSelectMenuGroup;
using Script.RobbyScript.TopMenuGroup;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script.RobbyScript.StoreMenuGroup
{
    public class StoreMenu : MonoBehaviour
    {
        [SerializeField] private GameObject bronzeGemBtn;
        [SerializeField] private GameObject silverGemBtn;
        [SerializeField] private GameObject goldGemBtn;
        [SerializeField] private GameObject bronzeAdsBtn;
        [SerializeField] private GameObject silverAdsBtn;
        [SerializeField] private GameObject goldAdsBtn;
        [SerializeField] private GameObject coinAdsBtn;
        [SerializeField] private GameObject staminaAdsBtn;
        [SerializeField] private GameObject gemAdsBtn;
        [SerializeField] private GameObject boxRewardPanel;
        [SerializeField] private GameObject boxRewardContents;
        [SerializeField] private GameObject closeBtn;
        [SerializeField] private GameObject adsRewardBtn;
        [SerializeField] private Goods rewardItem;
        [SerializeField] private List<CharacterBase> unitList = new List<CharacterBase>();
        
        [SerializeField] private Sprite bronzeSprite;
        [SerializeField] private Sprite silverSprite;
        [SerializeField] private Sprite goldSprite;
        [SerializeField] private Sprite chestAdsBtnSprite;
        [SerializeField] private Sprite chestGemBtnSprite;
        [SerializeField] private Sprite coinSprite;
        [SerializeField] private Sprite staminaSprite;
        [SerializeField] private Sprite gemSprite;
        [SerializeField] private Sprite errorGemSprite;
        [SerializeField] private Sprite errorCoinSprite;
        
        [SerializeField] private TextMeshProUGUI BronzeGemBtnText;
        [SerializeField] private TextMeshProUGUI SilverGemBtnText;
        [SerializeField] private TextMeshProUGUI GoldGemBtnText;
        
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
        
        private const string ResetKey = "ResetKey";

        private DateTime _lastDayCheck;
        private const string LastDayKey = "LastDayKey";

        private int _bronzeOpenCount;
        private const string BronzeOpenCountKey = "BronzeOpenCount";
        private const int BronzeOpenMaxCount = 20;

        private int _silverOpenCount;

        public int SilverAdsOpen
        {
            get { return _silverOpenCount; }
            set { _silverOpenCount = value; }
        }
        private const string SilverOpenCountKey = "SilverOpenCount";
        private DateTime _silverOpenTime;
        private const string SilverOpenTimeKey = "SilverOpenTime";
        private TimeSpan _silverPassed;
        private const int SilverOpenMaxCount = 7;
        private const int SilverRewardCoolTime = 5;

        private int _goldOpenCount;
        
        public int GoldAdsOpen
        {
            get { return _goldOpenCount; }
            set { _goldOpenCount = value; }
        }
        private const string GoldOpenCountKey = "GoldOpenCount";
        private DateTime _goldOpenTime;
        private const string GoldOpenTimeKey = "GoldOpenTimeKey";
        private TimeSpan _goldPassed;
        private const int GoldOpenMaxCount = 5;
        private const int GoldRewardCoolTime = 30;
        
        //
        // private int _coinOpenCount;
        // private const string CoinOpenCountKey = "CoinOpenCount";
        // private DateTime _coinOpenTime;
        // private const string CoinOpenTimeKey = "CoinOpenTimeKey";
        // private TimeSpan _coinPassed;
        // private const int CoinOpenMaxCount = 10;
        // private const int CoinRewardCoolTime = 1;
        //
        // private int _staminaOpenCount;
        // private const string StaminaOpenCountKey = "StaminaOpenCount";
        // private DateTime _staminaOpenTime;
        // private const string StaminaOpenTimeKey = "StaminaOpenTimeKey";
        // private TimeSpan _staminaPassed;
        // private const int StaminaOpenMaxCount = 10;
        // private const int StaminaRewardCoolTime = 1;
        //
        // private int _gemOpenCount;
        // private const string GemOpenCountKey = "GemOpenCount";
        // private DateTime _gemOpenTime;
        // private const string GemOpenTimeKey = "GemOpenTimeKey";
        // private TimeSpan _gemPassed;
        // private const int GemOpenMaxCount = 10;
        // private const int GemRewardCoolTime = 1;
        //
        
        private int _coinReward;
        private int _unitPieceReward;
        private readonly Dictionary<CharacterBase, Tuple<int, Goods>> _unitPieceDict = new Dictionary<CharacterBase, Tuple<int, Goods>>();
        private Goods _coinObject;
        private Goods _unitPieceObject;

        public enum BoxGrade { Bronze, Silver, Gold, Coin, Stamina, Gem, BronzeGem, SilverGem, GoldGem }
        public bool isReset;

        public enum ButtonType { BronzeAds, SilverAds, GoldAds, BronzeGem, SilverGem, GoldGem }

        private Vector3 originalPosition = new Vector3();
        private Vector3 newPosition = new Vector3();
        
        private const int BronzeGemText = 150;
        private const int SilverGemText = 450;
        private const int GoldGemText = 900;
        
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
                _silverOpenCount = PlayerPrefs.GetInt(SilverOpenCountKey);
            }
            else
            {
                _silverOpenCount = 0;
                PlayerPrefs.SetInt(SilverOpenCountKey, _silverOpenCount);
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
                _goldOpenCount = PlayerPrefs.GetInt(GoldOpenCountKey);
            }
            else
            {
                _goldOpenCount = 0;
                PlayerPrefs.SetInt(GoldOpenCountKey, _goldOpenCount);
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
            
            // Coin Count Check
            // if (PlayerPrefs.HasKey(CoinOpenCountKey))
            // {
            //     _coinOpenCount = PlayerPrefs.GetInt(CoinOpenCountKey);
            // }
            // else
            // {
            //     _coinOpenCount = 0;
            //     PlayerPrefs.SetInt(CoinOpenCountKey, _coinOpenCount);
            // }
            // // Coin Last Open Check
            // if (PlayerPrefs.HasKey(CoinOpenTimeKey))
            // {
            //     _coinOpenTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(CoinOpenTimeKey)));
            // }
            // else
            // {
            //     _coinOpenTime = DateTime.Now.AddMinutes(-CoinRewardCoolTime);
            //     PlayerPrefs.SetString(CoinOpenTimeKey, _coinOpenTime.ToBinary().ToString());
            //     PlayerPrefs.Save();
            // }
            //
            //
            // // Stamina Count Check
            // if (PlayerPrefs.HasKey(StaminaOpenCountKey))
            // {
            //     _staminaOpenCount = PlayerPrefs.GetInt(StaminaOpenCountKey);
            // }
            // else
            // {
            //     _staminaOpenCount = 0;
            //     PlayerPrefs.SetInt(StaminaOpenCountKey, _staminaOpenCount);
            // }
            // // Stamina Last Open Check
            // if (PlayerPrefs.HasKey(StaminaOpenTimeKey))
            // {
            //     _staminaOpenTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(StaminaOpenTimeKey)));
            // }
            // else
            // {
            //     _staminaOpenTime = DateTime.Now.AddMinutes(-StaminaRewardCoolTime);
            //     PlayerPrefs.SetString(StaminaOpenTimeKey, _staminaOpenTime.ToBinary().ToString());
            //     PlayerPrefs.Save();
            // }
            //
            //
            // // Gem Count Check
            // if (PlayerPrefs.HasKey(GemOpenCountKey))
            // {
            //     _gemOpenCount = PlayerPrefs.GetInt(GemOpenCountKey);
            // }
            // else
            // {
            //     _gemOpenCount = 0;
            //     PlayerPrefs.SetInt(GemOpenCountKey, _gemOpenCount);
            // }
            // // Gem Last Open Check
            // if (PlayerPrefs.HasKey(GemOpenTimeKey))
            // {
            //     _gemOpenTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(GemOpenTimeKey)));
            // }
            // else
            // {
            //     _gemOpenTime = DateTime.Now.AddMinutes(-GemRewardCoolTime);
            //     PlayerPrefs.SetString(GemOpenTimeKey, _gemOpenTime.ToBinary().ToString());
            //     PlayerPrefs.Save();
            // }
            
            bronzeAdsBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheckClick(ButtonType.BronzeAds));
            silverAdsBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheckClick(ButtonType.SilverAds));
            goldAdsBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheckClick(ButtonType.GoldAds));
            bronzeGemBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheckClick(ButtonType.BronzeGem));
            silverGemBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheckClick(ButtonType.SilverGem));
            goldGemBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheckClick(ButtonType.GoldGem));
            coinAdsBtn.GetComponent<Button>().onClick.AddListener(CoinAds);
            staminaAdsBtn.GetComponent<Button>().onClick.AddListener(StaminaAds);
            gemAdsBtn.GetComponent<Button>().onClick.AddListener(GemAds);
            closeBtn.GetComponent<Button>().onClick.AddListener(ReceiveReward);
            adsRewardBtn.GetComponent<Button>().onClick.AddListener(AdsReceiveReward);
            gameObject.SetActive(false);
            originalPosition = chestGrade.transform.position;
            newPosition = originalPosition;
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
            _bronzeOpenCount = 0;

            _silverOpenCount = 0;
            _silverOpenTime = DateTime.Now.AddMinutes(-SilverRewardCoolTime);
            PlayerPrefs.SetString(SilverOpenTimeKey, _silverOpenTime.ToBinary().ToString());
            _goldOpenCount = 0;
            _goldOpenTime = DateTime.Now.AddMinutes(-GoldRewardCoolTime);
            PlayerPrefs.SetString(GoldOpenTimeKey, _goldOpenTime.ToBinary().ToString());
            
            // _coinOpenCount = 0;
            // _coinOpenTime = DateTime.Now.AddMinutes(-CoinRewardCoolTime);
            // PlayerPrefs.SetString(CoinOpenTimeKey, _coinOpenTime.ToBinary().ToString());
            // _staminaOpenCount = 0;
            // _staminaOpenTime = DateTime.Now.AddMinutes(-StaminaRewardCoolTime);
            // PlayerPrefs.SetString(StaminaOpenTimeKey, _staminaOpenTime.ToBinary().ToString());
            // _gemOpenCount = 0;
            // _gemOpenTime = DateTime.Now.AddMinutes(-GemRewardCoolTime);
            // PlayerPrefs.SetString(GemOpenTimeKey, _gemOpenTime.ToBinary().ToString());
            
            PlayerPrefs.SetInt(BronzeOpenCountKey, _bronzeOpenCount);
            PlayerPrefs.SetInt(SilverOpenCountKey, _silverOpenCount);
            PlayerPrefs.SetInt(GoldOpenCountKey, _goldOpenCount);
            // PlayerPrefs.SetInt(CoinOpenCountKey, _coinOpenCount);
            // PlayerPrefs.SetInt(StaminaOpenCountKey, _staminaOpenCount);
            // PlayerPrefs.SetInt(GemOpenCountKey, _gemOpenCount);
            PlayerPrefs.Save();
        }

        private void UpdateButtonState()
        {
            UpdateBronzeButton();
            UpdateSilverButton();
            UpdateGoldButton();
            // UpdateCoinButton();
            // UpdateStaminaButton();
            // UpdateGemButton();
            UpdateGemButtonText();
        }
        
        private void UpdateBronzeButton()
        {
            if (isReset || _bronzeOpenCount < BronzeOpenMaxCount)
            {
                bronzeAdsBtn.GetComponent<Button>().interactable = true;
                bronzeAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{_bronzeOpenCount} / {BronzeOpenMaxCount}";
            }
            else
            {
                var resetTime = _lastDayCheck.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
                bronzeAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"Reset: {resetTime}";
            }
            bronzeGemBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = "150";
        }

        private void UpdateSilverButton()
        {
            _silverPassed = DateTime.Now - _silverOpenTime;
            if (isReset || (_silverOpenCount < SilverOpenMaxCount && _silverPassed.TotalMinutes >= SilverRewardCoolTime))
            {
                silverAdsBtn.GetComponent<Button>().interactable = true;
                silverAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{_silverOpenCount} / {SilverOpenMaxCount}";
            }
            else
            {
                silverAdsBtn.GetComponent<Button>().interactable = false;
                var remainingTime = TimeSpan.FromMinutes(SilverRewardCoolTime) - _silverPassed;
                if (_silverOpenCount == SilverOpenMaxCount)
                {
                    var resetTime = _lastDayCheck.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
                    silverAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"Reset: {resetTime}";
                }
                else if (remainingTime > TimeSpan.Zero)
                {
                    var remainingTimeText = remainingTime.ToString(@"mm\:ss");
                    silverAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = remainingTimeText;
                }
            }
            silverGemBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = "450";
        }

        private void UpdateGoldButton()
        {
            _goldPassed = DateTime.Now - _goldOpenTime;
            if (isReset || (_goldOpenCount < GoldOpenMaxCount && _goldPassed.TotalMinutes >= GoldRewardCoolTime))
            {
                goldAdsBtn.GetComponent<Button>().interactable = true;
                goldAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{_goldOpenCount} / {GoldOpenMaxCount}";
            }
            else
            {
                goldAdsBtn.GetComponent<Button>().interactable = false;
                var remainingTime = TimeSpan.FromMinutes(GoldRewardCoolTime) - _goldPassed;
                if (_goldOpenCount == GoldOpenMaxCount)
                {
                    var resetTime = _lastDayCheck.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
                    goldAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"Reset: {resetTime}";
                }
                else if (remainingTime > TimeSpan.Zero)
                {
                    var remainingTimeText = remainingTime.ToString(@"mm\:ss");
                    goldAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = remainingTimeText;
                }
            }
            goldGemBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = "900";
        }
        
        // private void UpdateCoinButton()
        // {
        //     _coinPassed = DateTime.Now - _coinOpenTime;
        //     if (isReset || (_coinOpenCount < CoinOpenMaxCount && _coinPassed.TotalMinutes >= CoinRewardCoolTime))
        //     {
        //         coinAdsBtn.GetComponent<Button>().interactable = true;
        //         coinAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{_coinOpenCount} / {CoinOpenMaxCount}";
        //     }
        //     else
        //     {
        //         coinAdsBtn.GetComponent<Button>().interactable = false;
        //         var remainingTime = TimeSpan.FromMinutes(CoinRewardCoolTime) - _coinPassed;
        //         if (_coinOpenCount == CoinOpenMaxCount)
        //         {
        //             var resetTime = _lastDayCheck.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
        //             coinAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{resetTime}";
        //             // $"Reset: {resetTime}";   
        //         }
        //         else if (remainingTime > TimeSpan.Zero)
        //         {
        //             var remainingTimeText = remainingTime.ToString(@"mm\:ss");
        //             coinAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = remainingTimeText;
        //         }
        //     }
        // }
        //
        // private void UpdateStaminaButton()
        // {
        //     _staminaPassed = DateTime.Now - _staminaOpenTime;
        //     if (isReset || (_staminaOpenCount < StaminaOpenMaxCount && _staminaPassed.TotalMinutes >= StaminaRewardCoolTime))
        //     {
        //         staminaAdsBtn.GetComponent<Button>().interactable = true;
        //         staminaAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{_staminaOpenCount} / {StaminaOpenMaxCount}";
        //     }
        //     else
        //     {
        //         staminaAdsBtn.GetComponent<Button>().interactable = false;
        //         var remainingTime = TimeSpan.FromMinutes(StaminaRewardCoolTime) - _staminaPassed;
        //         if (_staminaOpenCount == StaminaOpenMaxCount)
        //         {
        //             var resetTime = _lastDayCheck.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
        //             staminaAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{resetTime}";
        //         }
        //         else if (remainingTime > TimeSpan.Zero)
        //         {
        //             var remainingTimeText = remainingTime.ToString(@"mm\:ss");
        //             staminaAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = remainingTimeText;
        //         }
        //     }
        // }
        //
        // private void UpdateGemButton()
        // {
        //     _gemPassed = DateTime.Now - _gemOpenTime;
        //     if (isReset || (_gemOpenCount < GemOpenMaxCount && _gemPassed.TotalMinutes >= GemRewardCoolTime))
        //     {
        //         gemAdsBtn.GetComponent<Button>().interactable = true;
        //         gemAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{_gemOpenCount} / {GemOpenMaxCount}";
        //     }
        //     else
        //     {
        //         gemAdsBtn.GetComponent<Button>().interactable = false;
        //         var remainingTime = TimeSpan.FromMinutes(GemRewardCoolTime) - _gemPassed;
        //         if (_gemOpenCount == GemOpenMaxCount)
        //         {
        //             var resetTime = _lastDayCheck.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
        //             gemAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"{resetTime}";
        //         }
        //         else if (remainingTime > TimeSpan.Zero)
        //         {
        //             var remainingTimeText = remainingTime.ToString(@"mm\:ss");
        //             gemAdsBtn.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = remainingTimeText;
        //         }
        //     }
        // }

        private void UpdateGemButtonText()
        {
            int currentGemCount = GemScript.Instance.Gem;

            if (BronzeGemText > currentGemCount)
            {
                BronzeGemBtnText.color = Color.red;
            }
            else
            {
                BronzeGemBtnText.color = Color.white;
            }

            if (SilverGemText > currentGemCount)
            {
                SilverGemBtnText.color = Color.red;
            }
            else
            {
                SilverGemBtnText.color = Color.white;
            }

            if (GoldGemText > currentGemCount)
            {
                GoldGemBtnText.color = Color.red;
            }
            else
            {
                GoldGemBtnText.color = Color.white;
            }
        }


        //!!!!!!!!!!!!!!!!!!!!!
        public void Reward(BoxGrade boxTypes)
        {
            adsRewardBtn.SetActive(true);
            closeBtn.SetActive(true);
            Debug.Log("boxTypes :"+boxTypes );
            switch (boxTypes)
            {
                case BoxGrade.Bronze:
                    _bronzeOpenCount = PlayerPrefs.GetInt(BronzeOpenCountKey, 0);
                    CalculateCoinReward(boxTypes, _bronzeOpenCount);
                    CalculateUnitPieceReward(boxTypes, _bronzeOpenCount);
                    if (_bronzeOpenCount == BronzeOpenMaxCount) break;
                    _bronzeOpenCount++;
                    PlayerPrefs.SetInt(BronzeOpenCountKey, _bronzeOpenCount);
                    adsRewardBtn.SetActive(false);
                    break;
                case BoxGrade.Silver:
                    _silverOpenCount = PlayerPrefs.GetInt(SilverOpenCountKey, 0);
                    CalculateCoinReward(boxTypes, _silverOpenCount);
                    CalculateUnitPieceReward(boxTypes, _silverOpenCount);
                    if (_silverOpenCount == SilverOpenMaxCount) break;
                    _silverOpenCount++;
                    _silverOpenTime = DateTime.Now;
                    PlayerPrefs.SetInt(SilverOpenCountKey, _silverOpenCount);
                    PlayerPrefs.SetString(SilverOpenTimeKey, _silverOpenTime.ToBinary().ToString());
                    adsRewardBtn.SetActive(false);
                    break;
                case BoxGrade.Gold:
                    _goldOpenCount = PlayerPrefs.GetInt(GoldOpenCountKey, 0);
                    CalculateCoinReward(boxTypes, _goldOpenCount);
                    CalculateUnitPieceReward(boxTypes, _goldOpenCount);
                    if (_goldOpenCount == GoldOpenMaxCount) break; 
                    _goldOpenCount++;
                    _goldOpenTime = DateTime.Now;
                    PlayerPrefs.SetInt(GoldOpenCountKey, _goldOpenCount);
                    PlayerPrefs.SetString(GoldOpenTimeKey, _goldOpenTime.ToBinary().ToString());
                    adsRewardBtn.SetActive(false);
                    break;
                case BoxGrade.Coin:
                    // _coinOpenCount = PlayerPrefs.GetInt(CoinOpenCountKey, 0);
                    // if (_coinOpenCount == CoinOpenMaxCount) break; 
                    // _coinOpenCount++;
                    // _coinOpenTime = DateTime.Now;
                    // PlayerPrefs.SetInt(CoinOpenCountKey, _coinOpenCount);
                    // PlayerPrefs.SetString(CoinOpenTimeKey, _coinOpenTime.ToBinary().ToString());
                    CalculateAdsReward(boxTypes);
                    closeBtn.SetActive(false);
                    break;
                case BoxGrade.Stamina:
                    // _staminaOpenCount = PlayerPrefs.GetInt(StaminaOpenCountKey, 0);
                    // if (_staminaOpenCount == StaminaOpenMaxCount) break; 
                    // _staminaOpenCount++;
                    // _staminaOpenTime = DateTime.Now;
                    // PlayerPrefs.SetInt(StaminaOpenCountKey, _staminaOpenCount);
                    // PlayerPrefs.SetString(StaminaOpenTimeKey, _staminaOpenTime.ToBinary().ToString());
                    CalculateAdsReward(boxTypes);
                    closeBtn.SetActive(false);
                    break;
                case BoxGrade.Gem:
                    // _gemOpenCount = PlayerPrefs.GetInt(GemOpenCountKey, 0);
                    // if (_gemOpenCount == GemOpenMaxCount) break; 
                    // _gemOpenCount++;
                    // _gemOpenTime = DateTime.Now;
                    // PlayerPrefs.SetInt(GemOpenCountKey, _gemOpenCount);
                    // PlayerPrefs.SetString(GemOpenTimeKey, _gemOpenTime.ToBinary().ToString());
                    CalculateAdsReward(boxTypes);
                    closeBtn.SetActive(false);
                    break;
                case BoxGrade.BronzeGem:
                    OpenChestWithGemForCoin(boxTypes);
                    OpenChestWithGemForUnitPieceReward(boxTypes);
                    adsRewardBtn.SetActive(false);
                    break;
                case BoxGrade.SilverGem:
                    OpenChestWithGemForCoin(boxTypes);
                    OpenChestWithGemForUnitPieceReward(boxTypes);
                    adsRewardBtn.SetActive(false);
                    break;
                case BoxGrade.GoldGem:
                    OpenChestWithGemForCoin(boxTypes);
                    OpenChestWithGemForUnitPieceReward(boxTypes);
                    adsRewardBtn.SetActive(false);
                    break;
            }
            isReset = false;
            PlayerPrefs.Save();
        }
        
        private void ReceiveReward()
        {
            DeleteEvent();
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

        private void AdsReceiveReward()
        {
            DeleteExceptionEvent();
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
        
        private void BronzeAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.BronzeAds;
        }
        private static void SilverAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.SilverAds;
        }
        private static void GoldAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.GoldAds;
        }
        private static void CoinAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Coin;
        }
        private static void StaminaAds()
        {
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Stamina;
        }
        private void GemAds()
        {
            AllPanelClose();
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Gem;
        }
        
        private void CalculateAdsReward(BoxGrade boxTypes)
        {
            if (_coinObject != null)
            {
                Destroy(_coinObject.gameObject);
            }
            _coinReward = boxTypes switch
            {
                BoxGrade.Coin => 1000,
                BoxGrade.Stamina => 10,
                BoxGrade.Gem => 200,
                _ => _coinReward
            };
            if (_coinReward == 0) return;
            _coinObject = Instantiate(rewardItem, boxRewardContents.transform);
            _coinObject.goodsSprite.GetComponent<Image>().sprite = boxTypes switch
            {
                BoxGrade.Coin => coinSprite,
                BoxGrade.Stamina =>  staminaSprite,
                BoxGrade.Gem => gemSprite
            };
            _coinObject.goodsSprite.GetComponent<RectTransform>().localScale = boxTypes switch
            {
                BoxGrade.Coin => new Vector3(1, 0.8f, 0),
                BoxGrade.Stamina => new Vector3(0.8f, 0.9f, 0),
                BoxGrade.Gem => new Vector3(1, 0.8f, 0),
            };
            _coinObject.goodsValue.text = $"{_coinReward}";
            _coinObject.goodsValue.GetComponent<RectTransform>().localScale = boxTypes switch
            {
                BoxGrade.Coin =>  new Vector3(1, 1, 0),
                BoxGrade.Stamina =>  new Vector3(1, 1, 0),
                BoxGrade.Gem =>  new Vector3(1, 1, 0),
            };
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
                _ => _coinReward
            };
            if (_coinReward == 0) return;
            _coinObject = Instantiate(rewardItem, boxRewardContents.transform);
            // _coinObject.goodsBack.GetComponent<Image>().color = Color.clear;
            _coinObject.goodsValue.text = $"{_coinReward}";
            CoinsScript.Instance.Coin += _coinReward;
            CoinsScript.Instance.UpdateCoin();
        }
        
        private void OpenChestWithGemForCoin(BoxGrade boxTypes)
        {
            if (_coinObject != null)
            {
                Destroy(_coinObject.gameObject);
            }
            _coinReward = boxTypes switch
            {
                BoxGrade.BronzeGem => 500,
                BoxGrade.SilverGem => 25000,
                BoxGrade.GoldGem => 60000,
            };
            if (_coinReward == 0) return;
            _coinObject = Instantiate(rewardItem, boxRewardContents.transform);
            // _coinObject.goodsBack.GetComponent<Image>().color = Color.clear;
            _coinObject.goodsValue.text = $"{_coinReward}";
            CoinsScript.Instance.Coin += _coinReward;
            CoinsScript.Instance.UpdateCoin();
        }
        
        private void OpenChestWithGemForUnitPieceReward(BoxGrade boxTypes)
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
                { CharacterBase.UnitGrades.Green, GemUnitPieceReward(CharacterBase.UnitGrades.Green, boxTypes) },
                { CharacterBase.UnitGrades.Blue, GemUnitPieceReward(CharacterBase.UnitGrades.Blue, boxTypes) },
                { CharacterBase.UnitGrades.Purple, GemUnitPieceReward(CharacterBase.UnitGrades.Purple, boxTypes) }
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
                _unitPieceObject.goodsValue.text = $"{_unitPieceReward}";
                _unitPieceDict[unit] = new Tuple<int, Goods>(_unitPieceReward, _unitPieceObject);
            }
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
                // _unitPieceObject.goodsBack.GetComponent<Image>().color = unit.UnitGrade switch
                // {
                //     CharacterBase.UnitGrades.Green => Color.green,
                //     CharacterBase.UnitGrades.Blue => Color.blue,
                //     CharacterBase.UnitGrades.Purple => Color.magenta,
                //     _ => throw new ArgumentOutOfRangeException()
                // };
                _unitPieceObject.goodsSprite.GetComponent<Image>().sprite = unit.GetSpriteForLevel(unit.unitPeaceLevel);
                _unitPieceObject.goodsSprite.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
                _unitPieceObject.goodsValue.text = $"{_unitPieceReward}";
                _unitPieceObject.goodsValue.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
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
        
        private static int GemUnitPieceReward(CharacterBase.UnitGrades unitGrade, BoxGrade boxGrade)
        {
            int greenValue;
            int blueValue;
            int purpleValue;
            switch (boxGrade)
            {
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

        private void ChestCheckClick(ButtonType buttonType)
        {
            ChestCheck.Instance.OpenPanel();

            switch (buttonType)
            {
                case ButtonType.BronzeAds:
                    ChestCheck.Instance.chestCheckTopText.text = "Bronze Chest";
                    ChestCheck.Instance.chestImage.GetComponent<Image>().sprite = bronzeSprite;
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Image>().sprite = chestAdsBtnSprite;
                    ChestCheck.Instance.chestCheckBtnText.text = $"{_bronzeOpenCount} / {BronzeOpenMaxCount}";
                    
                    ChestReward.Instance.SetStart(ButtonType.BronzeAds);
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Button>().onClick.AddListener(BronzeAds);
                    break;
                case ButtonType.SilverAds:
                    ChestCheck.Instance.chestCheckTopText.text = "Silver Chest";
                    ChestCheck.Instance.chestImage.GetComponent<Image>().sprite = silverSprite;
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Image>().sprite = chestAdsBtnSprite;
                    ChestCheck.Instance.chestCheckBtnText.text = $"{_silverOpenCount} / {SilverOpenMaxCount}";

                    ChestReward.Instance.SetStart(ButtonType.SilverAds);
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Button>().onClick.AddListener(SilverAds);
                    break;
                case ButtonType.GoldAds:
                    ChestCheck.Instance.chestCheckTopText.text = "Gold Chest";
                    ChestCheck.Instance.chestImage.GetComponent<Image>().sprite = goldSprite;
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Image>().sprite = chestAdsBtnSprite;
                    ChestCheck.Instance.chestCheckBtnText.text = $"{_goldOpenCount} / {GoldOpenMaxCount}";
                    
                    ChestReward.Instance.SetStart(ButtonType.GoldAds);
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Button>().onClick.AddListener(GoldAds);
                    break;
                case ButtonType.BronzeGem:
                    ChestCheck.Instance.chestCheckTopText.text = "Bronze Chest";
                    ChestCheck.Instance.chestImage.GetComponent<Image>().sprite = bronzeSprite;
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Image>().sprite = chestGemBtnSprite;
                    ChestCheck.Instance.chestCheckBtnText.text = "150";
                    ChestReward.Instance.SetStart(ButtonType.BronzeGem);
                    
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Button>().onClick.AddListener(() => CheckAndSummonChest(StoreMenu.ButtonType.BronzeGem));
                    break;
                case ButtonType.SilverGem:
                    ChestCheck.Instance.chestCheckTopText.text = "Silver Chest";
                    ChestCheck.Instance.chestImage.GetComponent<Image>().sprite = silverSprite;
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Image>().sprite = chestGemBtnSprite;
                    ChestCheck.Instance.chestCheckBtnText.text = "450";
                    ChestReward.Instance.SetStart(ButtonType.SilverGem);
                   
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Button>().onClick.AddListener(() => CheckAndSummonChest(StoreMenu.ButtonType.SilverGem));
                    break;
                case ButtonType.GoldGem:
                    ChestCheck.Instance.chestCheckTopText.text = "Gold Chest";
                    ChestCheck.Instance.chestImage.GetComponent<Image>().sprite = goldSprite;
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Image>().sprite = chestGemBtnSprite;
                    ChestCheck.Instance.chestCheckBtnText.text = "900";
                    ChestReward.Instance.SetStart(ButtonType.GoldGem);
                  
                    ChestCheck.Instance.chestCheckBtn.GetComponent<Button>().onClick.AddListener(() => CheckAndSummonChest(StoreMenu.ButtonType.GoldGem));
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
                SommonChest(chestType);
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
        
        public void SommonChest(ButtonType chestType)
        {
            chestRewardPanel.SetActive(true);
            chestOpenBtn.GetComponent<Button>().onClick.AddListener(() => OpenChest(chestType));
            StartCoroutine(IncreaseBackLightAlpha(chestType));
        }
        private IEnumerator IncreaseBackLightAlpha(ButtonType chestType)
        {
            chestGrade.transform.position = newPosition;
            Sprite chestSprite = null;
            switch (chestType)
            {
                case ButtonType.BronzeAds:
                    chestSprite = bronzeSprite;
                    break;
                case ButtonType.SilverAds:
                    chestSprite = silverSprite;
                    break;
                case ButtonType.GoldAds:
                    chestSprite = goldSprite;
                    break;
                case ButtonType.BronzeGem:
                    chestSprite = bronzeSprite;
                    break;
                case ButtonType.SilverGem:
                    chestSprite = silverSprite;
                    break;
                case ButtonType.GoldGem:
                    chestSprite = goldSprite;
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
            var moveAnimation = chestGrade.transform.DOMove(new Vector3(chestGrade.transform.position.x, targetY, chestGrade.transform.position.z), moveDuration);

            // chestGrade 오브젝트의 스케일을 목표 스케일로 변경하는 애니메이션을 생성
            var scaleAnimation = chestGrade.transform.DOScale(targetScale, moveDuration);

            // 두 개의 애니메이션이 모두 완료될 때까지 대기
            yield return moveAnimation.WaitForCompletion();
            yield return scaleAnimation.WaitForCompletion();

            // 흔들리는 애니메이션을 실행
            chestGrade.transform.DOShakeScale(2.0f, 0.5f, 4);
        }

        private void OpenChest(ButtonType chestType)
        {
            DeleteEvent();
            // 상자 열기 동작 수행
            Debug.Log("chestType :"+chestType);
            ChestCheck.Instance.chestCheckPanel.SetActive(false);
            chestRewardPanel.SetActive(false);
            boxRewardPanel.SetActive(true);
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

        private void AllPanelClose()
        {
            chestErrorPanel.SetActive(false);
            ChestCheck.Instance.chestCheckPanel.SetActive(false);
            DeleteEvent();
        }

        public void DeleteEvent()
        { 
            chestOpenBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            closeBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            adsRewardBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            bronzeAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            bronzeGemBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            chestOpenBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            coinAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            silverAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            gemAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            goldAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            goldGemBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            silverGemBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            staminaAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            chestErrorCloseBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            getBackToStoreBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            ChestCheck.Instance.chestCheckBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            ChestReward.Instance.ClearChests();
            Reallocation();
        }

        public void DeleteExceptionEvent()
        {
            chestOpenBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            closeBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            adsRewardBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            bronzeAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            bronzeGemBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            chestOpenBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            coinAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            silverAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            gemAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            goldAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            goldGemBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            silverGemBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            staminaAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            chestErrorCloseBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            getBackToStoreBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            ChestCheck.Instance.chestCheckBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            Reallocation();
        }

        private void Reallocation()
        {
            bronzeAdsBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheckClick(ButtonType.BronzeAds));
            silverAdsBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheckClick(ButtonType.SilverAds));
            goldAdsBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheckClick(ButtonType.GoldAds));
            bronzeGemBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheckClick(ButtonType.BronzeGem));
            silverGemBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheckClick(ButtonType.SilverGem));
            goldGemBtn.GetComponent<Button>().onClick.AddListener(() => ChestCheckClick(ButtonType.GoldGem));
            coinAdsBtn.GetComponent<Button>().onClick.AddListener(CoinAds);
            staminaAdsBtn.GetComponent<Button>().onClick.AddListener(StaminaAds);
            gemAdsBtn.GetComponent<Button>().onClick.AddListener(GemAds);
            closeBtn.GetComponent<Button>().onClick.AddListener(ReceiveReward);
            adsRewardBtn.GetComponent<Button>().onClick.AddListener(ReceiveReward);
        }
    }
}