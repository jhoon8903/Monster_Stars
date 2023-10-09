using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.RobbyScript.TopMenuGroup;
using Script.UIManager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using Image = UnityEngine.UI.Image;

namespace Script.RobbyScript.CharacterSelectMenuGroup
{
    public class HoldCharacterList : MonoBehaviour
    {                    
        [SerializeField] private List<CharacterBase> characterList;
        [SerializeField] private GameObject selectedContent;
        [SerializeField] private GameObject mainUnitContent;
        [SerializeField] private GameObject activateUnitContent;
        [SerializeField] private GameObject inActivateUnitContent;
        [SerializeField] private UnitIcon unitIconPrefab;
        [SerializeField] private GameObject gamePanel;
        [SerializeField] private GameObject informationPanelPrefab;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Sprite lockImage;
        [SerializeField] private Sprite lockBack;
        [SerializeField] private Sprite lockFrame;

        private GameObject _activeStatusPanel;
        private GameObject _activeNormalBack;
        private static readonly Dictionary<UnitIcon, UnitIcon> UnitIconMapping = new Dictionary<UnitIcon, UnitIcon>();
        private InformationPanel _informationPanel;
        public static HoldCharacterList Instance { get; private set; }
        private bool _blueUnlock;
        public UnitIcon selectedToSwap;
        public UnitIcon secondSwap;
        public List<UnitIcon> unitList = new List<UnitIcon>();
        public List<UnitIcon> topList = new List<UnitIcon>();
        public int sortingNumber = 11;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(Instance.gameObject);
            }
            gameObject.SetActive(false);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                PlayerPrefs.DeleteAll();
            }

            if (!Input.GetMouseButtonDown(0)) return;
            var clickedObject = EventSystem.current.currentSelectedGameObject;
            
            if (selectedToSwap != null)
            {
                secondSwap = clickedObject.GetComponentInParent<UnitIcon>();
                var secondSwapBase = secondSwap.CharacterBase;
                SwapUnitInstances(selectedToSwap, selectedToSwap.CharacterBase, secondSwap, secondSwapBase);
            }
            else
            {
                if (clickedObject != null && IsDescendantOrSelf(_activeStatusPanel, clickedObject))
                {
                    return;
                }
                    
                if (_activeStatusPanel == null) return;
                _activeStatusPanel.SetActive(false);
                if (_activeNormalBack != null)
                {
                    _activeNormalBack.SetActive(true);
                }

                _activeStatusPanel = null;
                _activeNormalBack = null;  
            }
        }

        private IEnumerator ViveUnitCard()
        {
            while (selectedToSwap != null)
            { 
                if (secondSwap) StartCoroutine(ViveUnitCard());
                foreach (var topInstance in topList)
                {
                    topInstance.transform.DOScale(new Vector3(0.95f, 0.95f, 0), 0.5f).SetLoops(2, LoopType.Yoyo);
                }
                yield return new WaitForSeconds(1f);  // 원하는 대기 시간을 설정합니다.
            }
        }

        public void SwapUnitInstances(UnitIcon first, CharacterBase firstBase, UnitIcon second, CharacterBase secondBase)
        {
            firstBase.selected = true;
            SelectedUnitHolder.Instance.selectedUnit.Add(firstBase);
            topList.Add(first);
            PlayerPrefs.SetString($"{firstBase.unitGroup}{CharacterBase.SelectKey}","true");
            PlayerPrefs.Save();
            first.transform.SetParent(selectedContent.transform);
            first.transform.SetAsLastSibling();
            first.normalBack.SetActive(true);
            first.infoBack.SetActive(false);
            var firstCanvas = first.unitCanvas;
            var secondCanvas = second.unitCanvas;
            if (firstCanvas != null)
            {
                firstCanvas.sortingOrder = secondCanvas.sortingOrder;
                firstCanvas.sortingLayerName = secondCanvas.sortingLayerName;
            }
            secondBase.selected = false;
            unitList.Add(second);
            topList.Remove(second);
            SelectedUnitHolder.Instance.selectedUnit.Remove(secondBase);
            PlayerPrefs.SetString($"{secondBase.unitGroup}{CharacterBase.SelectKey}","false");
            second.transform.SetParent(activateUnitContent.transform);
            second.transform.SetAsLastSibling();
            second.normalBack.SetActive(true);
            second.infoBack.SetActive(false);
            if (secondCanvas != null)
            {
                secondCanvas.sortingOrder = firstCanvas.sortingOrder;
                secondCanvas.sortingLayerName = firstCanvas.sortingLayerName;
            }
            foreach (var unitObjet in unitList)
            {
                unitObjet.unitCanvas.sortingLayerName = "Unit";
            }
            UpdateMainUnitContent();
            SortChildrenBySortingLayer(activateUnitContent.transform);
            AdjustRectTransform(activateUnitContent.transform);
            selectedToSwap = null;
            secondSwap = null;
        }
        private static bool IsDescendantOrSelf(Object obj, GameObject toCheck)
        {
            if (obj == null || toCheck == null) return false;
            var current = toCheck.transform;
            while (current != null)
            {
                if (current.gameObject == obj) return true;
                current = current.parent;
            }
            return false;
        }
        
        public void InstanceUnit()
        {
            var sortingLayerOder = 11;
            foreach (var character in characterList)
            {
                character.Initialize();
                character.unLock = bool.Parse(PlayerPrefs.GetString($"{character.unitGroup}{CharacterBase.UnLockKey}", "false"));
                character.selected = bool.Parse(PlayerPrefs.GetString($"{character.unitGroup}{CharacterBase.SelectKey}", "false"));
                if (character.unLock)
                {
                    if (character.selected)
                    {
                        var selectedUnitInstance = Instantiate(unitIconPrefab, selectedContent.transform, false);
                        SetupUnitIcon(selectedUnitInstance, character);
                        UpdateMainUnitContent();

                        var canvas = selectedUnitInstance.unitCanvas;
                        if (canvas != null && sortingLayerOder > 0)
                        {
                            canvas.sortingOrder = sortingLayerOder;
                            canvas.sortingLayerName = "TopMenu"; // Use "TopMenu" for selected units
                        }
                        AdjustRectTransform(activateUnitContent.transform);
                        SelectedUnitHolder.Instance.selectedUnit.Add(character);
                        topList.Add(selectedUnitInstance);
                     
                    }
                    else
                    {
                        var activeUnitInstance = Instantiate(unitIconPrefab, selectedContent.transform, false);
                        activeUnitInstance.transform.SetParent(activateUnitContent.transform, false);
                        SetupUnitIcon(activeUnitInstance, character);
                        unitList.Add(activeUnitInstance);

                        var canvas = activeUnitInstance.unitCanvas;
                        if (canvas != null && sortingLayerOder > 0)
                        {
                            canvas.sortingOrder = sortingLayerOder;
                            canvas.sortingLayerName = "Unit"; // Use "Unit" for active but not selected units
                        }
                    }
                    sortingLayerOder--;
                    AdjustRectTransform(activateUnitContent.transform);
                  
                }
                else
                {
                    var unitInstance = Instantiate(unitIconPrefab, inActivateUnitContent.transform, false);
                    SetupInActiveUnitIcon(unitInstance, character);
                    AdjustRectTransform(inActivateUnitContent.transform);
                    unitList.Add(unitInstance);
                }
                UpdateRewardPiece(character);
            }
        }
        private void SetupUnitIcon(UnitIcon unitInstance, CharacterBase character)
        {
            unitInstance.CharacterBase = character;
            UpdateUnit(unitInstance, character);
            unitInstance.unitBtn.onClick.AddListener(() => {SwapBackGround(unitInstance, character);});
            unitInstance.infoBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.popupOpen);
                unitInstance.normalBack.SetActive(true);
                unitInstance.infoBack.SetActive(false);
                _activeStatusPanel = null;
                if (_informationPanel != null) return;
                _informationPanel = Instantiate(informationPanelPrefab, gamePanel.transform).GetComponent<InformationPanel>();
                _informationPanel.OpenInfoPanel(unitInstance, character);
                _informationPanel.transform.localScale = Vector3.zero;
                _informationPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);

            });

            unitInstance.levelUpBtn.onClick.AddListener(() =>
            {
                unitInstance.normalBack.SetActive(true);
                unitInstance.infoBack.SetActive(false);
                _activeStatusPanel = null;
                if (_informationPanel != null) return;
                _informationPanel = Instantiate(informationPanelPrefab, gamePanel.transform).GetComponent<InformationPanel>();
                _informationPanel.OpenInfoPanel(unitInstance, character);
                _informationPanel.transform.localScale = Vector3.zero;
                _informationPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
            });

            unitInstance.removeBtn.onClick.AddListener(() =>
            {
                character.selected = false;
                PlayerPrefs.SetString($"{character.unitGroup}{CharacterBase.SelectKey}","false");
                PlayerPrefs.Save();
                SelectedUnitHolder.Instance.selectedUnit.Remove(character);
                unitInstance.transform.SetParent(activateUnitContent.transform);
                unitInstance.transform.SetAsLastSibling(); 
                unitInstance.normalBack.SetActive(true);
                unitInstance.infoBack.SetActive(false);
                unitList.Add(unitInstance);
                topList.Remove(unitInstance);
                UpdateMainUnitContent();
                var canvas = unitInstance.unitCanvas;
                if (canvas == null) return;
                canvas.sortingLayerName = "Unit";
                SortChildrenBySortingLayer(activateUnitContent.transform);
                AdjustRectTransform(activateUnitContent.transform);

            });

            unitInstance.useBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.unitSelect);
                if (SelectedUnitHolder.Instance.selectedUnit.Count < 4)
                {
                    character.selected = true;
                    PlayerPrefs.SetString($"{character.unitGroup}{CharacterBase.SelectKey}","true");
                    PlayerPrefs.Save();
                    SelectedUnitHolder.Instance.selectedUnit.Add(character);
                    unitInstance.transform.SetParent(selectedContent.transform);
                    unitInstance.transform.SetAsLastSibling(); 
                    unitInstance.normalBack.SetActive(true);
                    unitInstance.infoBack.SetActive(false);
                    unitList.Remove(unitInstance);
                    topList.Add(unitInstance);
                    UpdateMainUnitContent();
                    AdjustRectTransform(activateUnitContent.transform);
                    var canvas = unitInstance.unitCanvas;
                    if (canvas == null) return;
                    canvas.sortingLayerName = "TopMenu";
                }
                else
                {
                    if (selectedToSwap != null) return;
                    selectedToSwap = unitInstance;
                    StartCoroutine(ViveUnitCard());
                    selectedToSwap.infoBack.SetActive(false);
                    selectedToSwap.normalBack.SetActive(true);
                    unitList.Remove(selectedToSwap);
                    foreach (var unitObject in unitList.Where(unitObject => selectedToSwap != unitObject))
                    {
                        var anotherCanvas = unitObject.unitCanvas;
                        anotherCanvas.sortingLayerName = "Default";
                    }
                }
            });
        }
        private static void SortChildrenBySortingLayer(Transform parent)
        {
            var children = new List<Transform>();
            for (var i = 0; i < parent.childCount; i++)
            {
                children.Add(parent.GetChild(i));
            }
    
            children.Sort((a, b) =>
            {
                var canvasA = a.GetComponentInChildren<Canvas>();
                var canvasB = b.GetComponentInChildren<Canvas>();
        
                if (canvasA != null && canvasB != null)
                {
                    return canvasB.sortingOrder.CompareTo(canvasA.sortingOrder);
                }
                return 0;
            });
            for (var i = 0; i < children.Count; i++)
            {
                children[i].SetSiblingIndex(i);
            }
        }
        public static void UpdateUnit(UnitIcon unitInstance, CharacterBase character)
        {
            unitInstance.normalBack.GetComponent<Image>().sprite = character.UnitGrade switch
            {
              CharacterBase.UnitGrades.G => unitInstance.normalBackSprite[0],
              CharacterBase.UnitGrades.B => unitInstance.normalBackSprite[1],
              CharacterBase.UnitGrades.P => unitInstance.normalBackSprite[2],
              _=> unitInstance.normalBackSprite[3]
            };
            unitInstance.infoBack.GetComponent<Image>().sprite = character.UnitGrade switch
            {
                CharacterBase.UnitGrades.G => unitInstance.infoBackSprite[0],
                CharacterBase.UnitGrades.B => unitInstance.infoBackSprite[1],
                CharacterBase.UnitGrades.P => unitInstance.infoBackSprite[2],
            };
            unitInstance.unitFrame.GetComponent<Image>().sprite = character.UnitGrade switch
            {
                CharacterBase.UnitGrades.G => unitInstance.frameSprite[0],
                CharacterBase.UnitGrades.B => unitInstance.frameSprite[1],
                CharacterBase.UnitGrades.P => unitInstance.frameSprite[2],
                _=> unitInstance.frameSprite[3]
            };
            unitInstance.unitImage.GetComponent<Image>().sprite = character.GetSpriteForLevel(character.unitPieceLevel);
            unitInstance.unitImage.GetComponent<Image>().color = Color.white;
            SetUpUnitProperty(unitInstance, character);
            SetUpUnitLevelProgress(unitInstance);
        }
        private void SwapBackGround(UnitIcon unitInstance, CharacterBase characterBase)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.unitSelect);
            if (_activeStatusPanel == null)
            {
                unitInstance.infoBack.SetActive(true);
                unitInstance.normalBack.SetActive(false);  // 명시적으로 비활성화
                _activeStatusPanel = unitInstance.infoBack;
                _activeNormalBack = unitInstance.normalBack;  // 할당
            }
            else if (_activeStatusPanel != null && _activeStatusPanel != unitInstance.infoBack)
            {
                _activeStatusPanel.SetActive(false);
                if (_activeNormalBack != null)  // null 체크 추가
                {
                    _activeNormalBack.SetActive(true);  // 활성화
                }
                unitInstance.infoBack.SetActive(true);
                unitInstance.normalBack.SetActive(false);  // 명시적으로 비활성화
                _activeStatusPanel = unitInstance.infoBack;
                _activeNormalBack = unitInstance.normalBack;  // 할당
            }
         
            switch (characterBase.unLock)
            {
                case true when characterBase.selected:
                {
                    unitInstance.infoBtn.gameObject.SetActive(true);
                    if (characterBase.UnitPieceCount >= characterBase.UnitPieceMaxPiece && CoinsScript.Instance.Coin >= characterBase.CharacterLevelUpCoin)
                    {
                        unitInstance.levelUpBtn.gameObject.SetActive(true);
                        unitInstance.removeBtn.gameObject.SetActive(false);
                        unitInstance.useBtn.gameObject.SetActive(false); 
                    }
                    else
                    {
                        unitInstance.levelUpBtn.gameObject.SetActive(false);
                        unitInstance.removeBtn.gameObject.SetActive(true);
                        unitInstance.useBtn.gameObject.SetActive(false); 
                    }
                    break;
                }
                case true when !characterBase.selected:
                {
                    unitInstance.infoBtn.gameObject.SetActive(true);
                    if (characterBase.UnitPieceCount >= characterBase.UnitPieceMaxPiece && CoinsScript.Instance.Coin >= characterBase.CharacterLevelUpCoin)
                    {
                        unitInstance.levelUpBtn.gameObject.SetActive(true);
                        unitInstance.removeBtn.gameObject.SetActive(false);
                        unitInstance.useBtn.gameObject.SetActive(false);
                    }
                    else
                    {
                        unitInstance.levelUpBtn.gameObject.SetActive(false);
                        unitInstance.removeBtn.gameObject.SetActive(false);
                        unitInstance.useBtn.gameObject.SetActive(true);
                    }
                    break;
                }
                case false:
                    _informationPanel.OpenInfoPanel(unitInstance, characterBase);
                    break;
            }
        }
        private void SetupInActiveUnitIcon(UnitIcon unitInstance, CharacterBase character)
        {
            unitInstance.CharacterBase = character;
            unitInstance.unitImage.GetComponent<Image>().sprite = character.GetSpriteForLevel(character.unitPieceLevel);
            unitInstance.unitImage.GetComponent<Image>().color = Color.black;
            unitInstance.normalBack.GetComponent<Image>().sprite = lockBack;
            unitInstance.unitFrame.GetComponent<Image>().sprite = lockFrame;
            SetUpUnitProperty(unitInstance, character);
            SetUpUnitLevelProgress(unitInstance);
        }
        private static void SetUpUnitProperty(UnitIcon unitInstance, CharacterBase character)
        {
            unitInstance.unitProperty.GetComponent<Image>().sprite = character.UnitProperty switch
            { 
                CharacterBase.UnitProperties.Darkness => unitInstance.unitPropertiesSprite[0],
                CharacterBase.UnitProperties.Fire => unitInstance.unitPropertiesSprite[1],
                CharacterBase.UnitProperties.Physics => unitInstance.unitPropertiesSprite[2],
                CharacterBase.UnitProperties.Poison => unitInstance.unitPropertiesSprite[3],
                CharacterBase.UnitProperties.Water => unitInstance.unitPropertiesSprite[4],
            };
        }
        private static void SetUpUnitLevelProgress(UnitIcon unitInstance)
        {
            unitInstance.unitLevelText.text = unitInstance.CharacterBase.unLock ? $"Lv. {unitInstance.CharacterBase.unitPieceLevel}" : "Lock" ;
            unitInstance.unitPieceSlider.maxValue = unitInstance.CharacterBase.UnitPieceMaxPiece;
            unitInstance.unitPieceSlider.value = unitInstance.CharacterBase.UnitPieceCount;
            unitInstance.unitPieceText.text = $"{unitInstance.CharacterBase.UnitPieceCount}/{unitInstance.CharacterBase.UnitPieceMaxPiece}";
        }
        public void UpdateRewardPiece(CharacterBase characterBase)
        {
            var allUnitIconInstances = new List<UnitIcon>();
            allUnitIconInstances.AddRange(selectedContent.GetComponentsInChildren<UnitIcon>());
            allUnitIconInstances.AddRange(mainUnitContent.GetComponentsInChildren<UnitIcon>());
            allUnitIconInstances.AddRange(activateUnitContent.GetComponentsInChildren<UnitIcon>());
            allUnitIconInstances.AddRange(inActivateUnitContent.GetComponentsInChildren<UnitIcon>());
            var matchingUnitIcons = allUnitIconInstances.Where(unitIcon => unitIcon.CharacterBase == characterBase).ToList();
            foreach (var unitIcon in matchingUnitIcons)
            {
                unitIcon.CharacterBase = characterBase;
                unitIcon.unitPieceSlider.maxValue = characterBase.UnitPieceMaxPiece;
                unitIcon.unitPieceSlider.value = characterBase.UnitPieceCount;
                unitIcon.unitPieceText.text = $"{characterBase.UnitPieceCount}/{unitIcon.unitPieceSlider.maxValue}";

                if (unitIcon.CharacterBase.UnitPieceCount >= unitIcon.CharacterBase.UnitPieceMaxPiece && !unitIcon.CharacterBase.unLock)
                {
                    
                    if (unitIcon.CharacterBase.UnitGrade == CharacterBase.UnitGrades.B && !_blueUnlock)
                    {
                        _blueUnlock = true;
                        Firebase.Analytics.FirebaseAnalytics.LogEvent("blue_unlocked");
                    }
                    switch (unitIcon.CharacterBase.unitGroup)
                    {
                        case CharacterBase.UnitGroups.DarkElf:
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("darkelf_unlocked");
                            break;
                        case CharacterBase.UnitGroups.DeathChiller:
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("chiller_unlocked");
                            break;
                    }
                    unitIcon.CharacterBase.unLock = true;
                    PlayerPrefs.SetString($"{unitIcon.CharacterBase.unitGroup}{CharacterBase.UnLockKey}", "true");
                    UpdateUnit(unitIcon, unitIcon.CharacterBase);
                    SetupUnitIcon(unitIcon, unitIcon.CharacterBase);
                    unitIcon.transform.SetParent(activateUnitContent.transform, false);
                    AdjustRectTransform(activateUnitContent.transform);
                    unitIcon.unitCanvas.sortingOrder = sortingNumber;
                    sortingNumber--;
                }
            }
        }
        private static void AdjustRectTransform(Transform parent)
        {
            var numberOfChildren = parent.childCount;
            var numberOfRows = Mathf.CeilToInt((float)numberOfChildren / 4);
            var newHeight = numberOfRows * 1100f + (numberOfRows - 1) * 65f;
            var rectTransform = parent.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
            }
        }
        private void UpdateMainUnitContent()
        {
            UnitIconMapping.Clear();
            foreach (Transform child in mainUnitContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in selectedContent.transform)
            {
                var newUnitInstance = Instantiate(child.gameObject, mainUnitContent.transform);
                var newUnit = newUnitInstance.GetComponent<UnitIcon>();
                var originalUnit = child.GetComponent<UnitIcon>();
                UnitIconMapping[originalUnit] = newUnit;
                newUnit.CharacterBase = originalUnit.CharacterBase;
                var newUnitBase = newUnit.CharacterBase;
                newUnit.unitBtn.onClick.AddListener(() =>
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.popupOpen);
                    newUnit.infoBack.SetActive(false);
                    if (_informationPanel == null)
                    {
                        _informationPanel = Instantiate(informationPanelPrefab, gamePanel.transform).GetComponent<InformationPanel>();
                        _informationPanel.OpenInfoPanel(newUnit, newUnitBase);
                        _informationPanel.transform.localScale = Vector3.zero;
                        _informationPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
                    }
                    SyncWithSelected(newUnit, newUnitBase);
                });
            }
        }
        public static void SyncWithSelected(UnitIcon unitIcon, CharacterBase unitBase)
        {
            var correspondingUnit 
                = (from pair in UnitIconMapping where pair.Key 
                    == unitIcon || pair.Value == unitIcon select (pair.Key == unitIcon) 
                    ? pair.Value 
                    : pair.Key).FirstOrDefault();
            if (correspondingUnit == null) return;
            correspondingUnit.CharacterBase = unitBase;
            correspondingUnit.infoBack.SetActive(unitIcon.infoBack.activeSelf);
            UpdateUnit(unitIcon, unitBase);
            UpdateUnit(correspondingUnit, unitBase);
        }
    }
}
