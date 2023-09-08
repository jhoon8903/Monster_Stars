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
using Script.UIManager;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script.RobbyScript.StoreMenuGroup
{
    public class StoreMenu : MonoBehaviour
    {
        [SerializeField] private List<CharacterBase> unitList = new List<CharacterBase>();

        // Treasure Chest
        [SerializeField] public Transform treasureLayer;
        [SerializeField] private TreasureChest treasureChest;
       
        // Special Offer
        [SerializeField] public Transform specialOfferLayer;
        [SerializeField] public SpecialOffer specialOffer;

        // Daily Offer
        [SerializeField] public Transform dailyOfferLayer;
        [SerializeField] public DailyOffer dailyOffer;

        // ChestItem
        [SerializeField] public ChestItem chestItem;

        [SerializeField] public GameObject boxRewardPanel;
        [SerializeField] public GameObject boxRewardContents;
        [SerializeField] private GameObject closeBtn;
        [SerializeField] private GameObject adsRewardBtn;
        [SerializeField] private Goods rewardItem;
        [SerializeField] private Sprite errorGemSprite;
        [SerializeField] private Sprite errorCoinSprite;
        [SerializeField] private GameObject chestRewardPanel;
        [SerializeField] private Image chestBackLightImage;
        [SerializeField] private Image chestParticleImage;
        [SerializeField] private GameObject chestGrade;
        [SerializeField] public GameObject chestOpenBtn;
        [SerializeField] private GameObject chestErrorPanel;
        [SerializeField] private GameObject chestErrorCloseBtn;
        [SerializeField] private GameObject getBackToStoreBtn;
        [SerializeField] private GameObject errorContentsImage;
        [SerializeField] private QuestManager questManager;
        
        [SerializeField] public Sprite gGradeSprite; 
        [SerializeField] public Sprite bGradeSprite; 
        [SerializeField] public Sprite pGradeSprite;
        
        public static StoreMenu Instance { get; private set; }
        private int _coinReward;
        private int _unitPieceReward;
        private readonly Dictionary<CharacterBase, Tuple<int, Goods>> _unitPieceDict = new Dictionary<CharacterBase, Tuple<int, Goods>>();
        private Goods _coinObject;
        private Goods _unitPieceObject;
        public enum BoxGrade { Bronze, Silver, Gold, Coin, Stamina, Gem, BronzeGem, SilverGem, GoldGem}
        public bool isReset;
        public enum ButtonType { BronzeAds, SilverAds, GoldAds, BronzeGem, SilverGem, GoldGem }
        private Vector3 _originalPosition;
        private Vector3 _newPosition;

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
            treasureChest.InstanceTreasureChest();
            specialOffer.InstanceSpecialOffer();
            dailyOffer.InstanceDailyOffer();
            closeBtn.GetComponent<Button>().onClick.AddListener(ReceiveReward);
        }
        private void Start()
        {
            // adsRewardBtn.GetComponent<Button>().onClick.AddListener(AdsReceiveReward);
            _originalPosition = chestGrade.transform.position;
            _newPosition = _originalPosition;
            gameObject.SetActive(false);
        }
        private void Update()
        {
            treasureChest.UpdateButtonState();
        }
        public void ResetButtonCounts()
        {
            isReset = true;
            treasureChest.ResetTreasureChest();
        }
        private void Reward(BoxGrade boxTypes)
        {
            closeBtn.SetActive(true);
            const int count = 0;
            switch (boxTypes)
            {
                case BoxGrade.Bronze:
                    treasureChest.BronzeOpenCount = PlayerPrefs.GetInt(TreasureChest.BronzeOpenCountKey, 0);
                    CalculateCoinReward(boxTypes, treasureChest.BronzeOpenCount);
                    CalculateUnitPieceReward(boxTypes, treasureChest.BronzeOpenCount);
                    if (treasureChest.BronzeOpenCount == TreasureChest.BronzeOpenMaxCount) break;
                    treasureChest.BronzeOpenCount++;
                    PlayerPrefs.SetInt(TreasureChest.BronzeOpenCountKey, treasureChest.BronzeOpenCount);
                    break;
                case BoxGrade.Silver:
                    treasureChest.SilverOpenCount = PlayerPrefs.GetInt(TreasureChest.SilverOpenCountKey, 0);
                    CalculateCoinReward(boxTypes, treasureChest.SilverOpenCount);
                    CalculateUnitPieceReward(boxTypes, treasureChest.SilverOpenCount);
                    if (treasureChest.SilverOpenCount == TreasureChest.SilverOpenMaxCount) break;
                    treasureChest.SilverOpenCount++;
                    treasureChest.SilverOpenTime = DateTime.Now;
                    PlayerPrefs.SetInt(TreasureChest.SilverOpenCountKey, treasureChest.SilverOpenCount);
                    PlayerPrefs.SetString(TreasureChest.SilverOpenTimeKey, treasureChest.SilverOpenTime.ToBinary().ToString());
                    break;
                case BoxGrade.Gold:
                    treasureChest.GoldOpenCount = PlayerPrefs.GetInt(TreasureChest.GoldOpenCountKey, 0);
                    CalculateCoinReward(boxTypes, treasureChest.GoldOpenCount);
                    CalculateUnitPieceReward(boxTypes, treasureChest.GoldOpenCount);
                    if (treasureChest.GoldOpenCount == TreasureChest.GoldOpenMaxCount) break; 
                    treasureChest.GoldOpenCount++;
                    treasureChest.GoldOpenTime = DateTime.Now;
                    PlayerPrefs.SetInt(TreasureChest.GoldOpenCountKey,treasureChest.GoldOpenCount);
                    PlayerPrefs.SetString(TreasureChest.GoldOpenTimeKey, treasureChest.GoldOpenTime.ToBinary().ToString());
                    break;
                case BoxGrade.Coin:
                case BoxGrade.Stamina:
                case BoxGrade.Gem:
                    CalculateCoinReward(boxTypes, count);
                    break;
                case BoxGrade.BronzeGem:
                case BoxGrade.SilverGem:
                case BoxGrade.GoldGem:
                    CalculateCoinReward(boxTypes, count);
                    CalculateUnitPieceReward(boxTypes, count);
                    break;
            }

            if (boxTypes is not (BoxGrade.Coin and BoxGrade.Stamina and BoxGrade.Gem ))
            {
                var boxOpenCount = PlayerPrefs.GetInt($"{QuestManager.QuestTypes.OpenBox}Value", 0);
                boxOpenCount++;
                PlayerPrefs.SetInt($"{QuestManager.QuestTypes.OpenBox}Value", boxOpenCount);
            }
            isReset = false;
            PlayerPrefs.Save();
        }
        private void ReceiveReward()
        {
            boxRewardPanel.transform.DOScale(0.1f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    boxRewardPanel.SetActive(false);
                    ChestCheck.Instance.CloseChestCheck();
                    foreach (var unitReward in _unitPieceDict)
                    {
                        unitReward.Key.UnitPieceCount += unitReward.Value.Item1;
                        PlayerPrefs.SetInt($"{unitReward.Key.unitGroup}{CharacterBase.PieceKey}", unitReward.Key.UnitPieceCount);
                        HoldCharacterList.Instance.UpdateRewardPiece(unitReward.Key);
                        Destroy(unitReward.Value.Item2.gameObject);
                    }
                    if (_coinObject != null)
                    {
                        Destroy(_coinObject.gameObject);
                    }
                    _unitPieceDict.Clear();
                });
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
            _coinObject = chestItem.CoinInstance(rewardItem, boxRewardContents.transform, boxTypes, _coinReward);
            var getCoin = PlayerPrefs.GetInt($"{QuestManager.QuestTypes.GetCoin}Value",0);
            var questGetCoin = getCoin + _coinReward;
            PlayerPrefs.SetInt($"{QuestManager.QuestTypes.GetCoin}Value", questGetCoin);
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
                { CharacterBase.UnitGrades.G, GetUnitPieceReward(CharacterBase.UnitGrades.G, boxTypes, openCount) },
                { CharacterBase.UnitGrades.B, GetUnitPieceReward(CharacterBase.UnitGrades.B, boxTypes, openCount) },
                { CharacterBase.UnitGrades.P, GetUnitPieceReward(CharacterBase.UnitGrades.P, boxTypes, openCount) }
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
                _unitPieceObject = chestItem.PieceInstance(unit, rewardItem, _unitPieceReward, boxRewardContents.transform);
                _unitPieceObject.goodsBack.GetComponent<Image>().color = Color.white;
                _unitPieceObject.goodsBack.GetComponent<Image>().sprite = unit.UnitGrade switch
                {
                    CharacterBase.UnitGrades.G => gGradeSprite,
                    CharacterBase.UnitGrades.B => bGradeSprite,
                    CharacterBase.UnitGrades.P => pGradeSprite,
                };
                _unitPieceDict[unit] = new Tuple<int, Goods>(_unitPieceReward, _unitPieceObject);
            }
            var totalUnitPieces = pieceCountPerUnit.Values.Sum();
            var questGetPiece = PlayerPrefs.GetInt($"{QuestManager.QuestTypes.GetPiece}Value",0);
            PlayerPrefs.SetInt($"{QuestManager.QuestTypes.GetPiece}Value", totalUnitPieces + questGetPiece);
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
               CharacterBase.UnitGrades.G =>  greenValue,
               CharacterBase.UnitGrades.B => blueValue,
               CharacterBase.UnitGrades.P => purpleValue,
               _ => throw new ArgumentOutOfRangeException(nameof(unitGrade), unitGrade, null)
             };
         }
        public void CheckAndSummonChest(ButtonType chestType)
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
                getBackToStoreBtn.GetComponent<Button>().onClick.AddListener(SpecialOffer.GemAds);
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
        public IEnumerator IncreaseBackLightAlpha(ButtonType chestType)
        {
            chestGrade.gameObject.SetActive(true);
            chestGrade.transform.position = _newPosition;
            Sprite chestSprite = null;
            switch (chestType)
            {
                case ButtonType.BronzeAds:
                    chestSprite = treasureChest.bronzeSprite;
                    break;
                case ButtonType.SilverAds:
                    chestSprite = treasureChest.silverSprite;
                    break;
                case ButtonType.GoldAds:
                    chestSprite = treasureChest.goldSprite;
                    break;
                case ButtonType.BronzeGem:
                    chestSprite = treasureChest.bronzeSprite;
                    break;
                case ButtonType.SilverGem:
                    chestSprite = treasureChest.silverSprite;
                    break;
                case ButtonType.GoldGem:
                    chestSprite = treasureChest.goldSprite;
                    break;
            }
            if (chestSprite != null)
            {
                chestGrade.GetComponentInChildren<Image>().sprite = chestSprite;
            }
            chestParticleImage.color = new Color(1f, 1f, 1f, 0f);
            chestBackLightImage.color = new Color(1f, 1f, 1f, 0f);
            chestBackLightImage.DOFade(1f, 1.5f);
            if (chestType == ButtonType.GoldGem)
            {
                chestParticleImage.DOFade(1f, 1.5f);
            }

            const float moveDuration = 1.0f;
            const float targetY = -0.5f;
            var initialScale = new Vector3(0f, 0f, 0f);
            var targetScale = new Vector3(1f, 1f, 1f);
            chestGrade.transform.localScale = initialScale;
            var position = chestGrade.transform.position;
            var moveAnimation = chestGrade.transform.DOMove(new Vector3(position.x, targetY, position.z), moveDuration);
            var scaleAnimation = chestGrade.transform.DOScale(targetScale, moveDuration);
            yield return moveAnimation.WaitForCompletion();
            yield return scaleAnimation.WaitForCompletion();
            moveAnimation.Kill();
            scaleAnimation.Kill();
            var shakeAnimation = chestGrade.transform.DOShakeScale(2.0f, 0.5f, 4);
            yield return shakeAnimation.WaitForCompletion();
            shakeAnimation.Kill();
        }
        private void OpenChest(ButtonType chestType)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.reward);
            ChestCheck.Instance.chestCheckPanel.SetActive(false);
            chestRewardPanel.SetActive(false);
            chestGrade.gameObject.SetActive(false);
            boxRewardPanel.SetActive(true);
            boxRewardPanel.transform.localScale = Vector3.zero;
            boxRewardPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                var boxGrade = chestType switch
                {
                    ButtonType.BronzeAds => BoxGrade.Bronze,
                    ButtonType.SilverAds => BoxGrade.Silver,
                    ButtonType.GoldAds => BoxGrade.Gold,
                    ButtonType.BronzeGem => BoxGrade.BronzeGem,
                    ButtonType.SilverGem => BoxGrade.SilverGem,
                    ButtonType.GoldGem => BoxGrade.GoldGem,
                    _ => BoxGrade.Bronze
                };
                Reward(boxGrade);
            });
        }
        public void OpenAds(BoxGrade adsType)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.reward);
            boxRewardPanel.SetActive(true);
            boxRewardPanel.transform.localScale = Vector3.zero;
            boxRewardPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                var boxGrade = adsType switch
                {
                    BoxGrade.Coin => BoxGrade.Coin,
                    BoxGrade.Stamina => BoxGrade.Stamina,
                    BoxGrade.Gem => BoxGrade.Gem,
                    _ => BoxGrade.Bronze
                };
                Reward(boxGrade);
            });
        }
        public void ErrorClose()
        {
            chestErrorPanel.SetActive(false);
        }
        public void DeleteEvent()
        { 
            specialOffer.SpecialBtnRemove();
            treasureChest.ChestBtnRemove();
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
            treasureChest.ChestBtnSet();
            specialOffer.SpecialBtnSet();
            closeBtn.GetComponent<Button>().onClick.AddListener(ReceiveReward);
        }
    }
}