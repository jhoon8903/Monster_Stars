using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.RobbyScript.TopMenuGroup;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace Script.RobbyScript.CharacterSelectMenuGroup
{
    public class HoldCharacterList : MonoBehaviour, IPointerClickHandler
    {                    
        [SerializeField] private List<CharacterBase> characterList;
        [SerializeField] private GameObject selectedContent;
        [SerializeField] private GameObject mainUnitContent;
        [SerializeField] private GameObject activateUnitContent;
        [SerializeField] private GameObject inActivateUnitContent;
        [SerializeField] private UnitIcon unitIconPrefab;
        [SerializeField] private GameObject gamePanel;
        [SerializeField] private GameObject informationPanelPrefab;
        [SerializeField] private GameObject warningPanel;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Sprite lockImage;
        [SerializeField] private Sprite lockBack;
        [SerializeField] private Sprite lockFrame;
        
        private GameObject _activeStatusPanel;
        private static readonly Dictionary<UnitIcon, UnitIcon> UnitIconMapping = new Dictionary<UnitIcon, UnitIcon>();
        private InformationPanel _informationPanel;
        public static HoldCharacterList Instance { get; private set; }
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
        }

        public void InstanceUnit()
        {
            var sortingLayerOder = 10;
            foreach (var character in characterList)
            {           
                character.Initialize();
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
                    }
                    else
                    {
                        var activeUnitInstance = Instantiate(unitIconPrefab, selectedContent.transform, false);
                        activeUnitInstance.transform.SetParent(activateUnitContent.transform, false);
                        SetupUnitIcon(activeUnitInstance, character);

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

        private void SetupUnitIcon(UnitIcon unitInstance, CharacterBase character)
        {
            unitInstance.CharacterBase = character;
            UpdateUnit(unitInstance, character);
            unitInstance.unitBtn.onClick.AddListener(() => {SwapBackGround(unitInstance, character);});
            unitInstance.infoBtn.onClick.AddListener(() =>
            {
                unitInstance.normalBack.SetActive(true);
                unitInstance.infoBack.SetActive(false);
                _activeStatusPanel = null;
                if (_informationPanel == null)
                {
                    _informationPanel = Instantiate(informationPanelPrefab, gamePanel.transform).GetComponent<InformationPanel>();
                }
                // _informationPanel.gameObject.SetActive(true);
                _informationPanel.OpenInfoPanel(unitInstance, character);
            });

            unitInstance.levelUpBtn.onClick.AddListener(() =>
            {
                unitInstance.normalBack.SetActive(true);
                unitInstance.infoBack.SetActive(false);
                _activeStatusPanel = null;
                if (_informationPanel == null)
                {
                    _informationPanel = Instantiate(informationPanelPrefab, gamePanel.transform).GetComponent<InformationPanel>();
                }
                // _informationPanel.gameObject.SetActive(true);
                _informationPanel.OpenInfoPanel(unitInstance, character);
            });

            unitInstance.removeBtn.onClick.AddListener(() =>
            {
                character.selected = false;
                SelectedUnitHolder.Instance.selectedUnit.Remove(character);
                PlayerPrefs.DeleteKey(character.unitGroup.ToString());
                unitInstance.transform.SetParent(activateUnitContent.transform);
                unitInstance.transform.SetAsLastSibling(); 
                unitInstance.normalBack.SetActive(true);
                unitInstance.infoBack.SetActive(false);
                UpdateMainUnitContent();
                var canvas = unitInstance.unitCanvas;
                if (canvas == null) return;
                canvas.sortingLayerName = "Unit";
                SortChildrenBySortingLayer(activateUnitContent.transform);
                AdjustRectTransform(activateUnitContent.transform);

            });

            unitInstance.useBtn.onClick.AddListener(() =>
            {
                if (SelectedUnitHolder.Instance.selectedUnit.Count < 4)
                {
                    character.selected = true;
                    SelectedUnitHolder.Instance.selectedUnit.Add(character);
                    PlayerPrefs.SetInt(character.unitGroup.ToString(), 1);
                    PlayerPrefs.Save();
                    unitInstance.transform.SetParent(selectedContent.transform);
                    unitInstance.transform.SetAsLastSibling(); 
                    unitInstance.normalBack.SetActive(true);
                    unitInstance.infoBack.SetActive(false);
                    UpdateMainUnitContent();
                    AdjustRectTransform(activateUnitContent.transform);
                    var canvas = unitInstance.unitCanvas;
                    if (canvas == null) return;
                    canvas.sortingLayerName = "TopMenu";
                }
                else
                { 
                    warningPanel.SetActive(true);
                    messageText.text = "No more can be placed.";
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
              CharacterBase.UnitGrades.Green => unitInstance.normalBackSprite[0],
              CharacterBase.UnitGrades.Blue => unitInstance.normalBackSprite[1],
              CharacterBase.UnitGrades.Purple => unitInstance.normalBackSprite[2],
              _=> unitInstance.normalBackSprite[3]
            };
            unitInstance.infoBack.GetComponent<Image>().sprite = character.UnitGrade switch
            {
                CharacterBase.UnitGrades.Green => unitInstance.infoBackSprite[0],
                CharacterBase.UnitGrades.Blue => unitInstance.infoBackSprite[1],
                CharacterBase.UnitGrades.Purple => unitInstance.infoBackSprite[2],
            };
            unitInstance.unitFrame.GetComponent<Image>().sprite = character.UnitGrade switch
            {
                CharacterBase.UnitGrades.Green => unitInstance.frameSprite[0],
                CharacterBase.UnitGrades.Blue => unitInstance.frameSprite[1],
                CharacterBase.UnitGrades.Purple => unitInstance.frameSprite[2],
                _=> unitInstance.frameSprite[3]
            };
            unitInstance.unitProperty.GetComponent<Image>().sprite = character.UnitProperty switch
            {
              CharacterBase.UnitProperties.Divine => unitInstance.unitPropertiesSprite[0],
              CharacterBase.UnitProperties.Darkness => unitInstance.unitPropertiesSprite[1],
              CharacterBase.UnitProperties.Physics => unitInstance.unitPropertiesSprite[2],
              CharacterBase.UnitProperties.Water => unitInstance.unitPropertiesSprite[3],
              CharacterBase.UnitProperties.Poison => unitInstance.unitPropertiesSprite[4],
              CharacterBase.UnitProperties.Fire => unitInstance.unitPropertiesSprite[5],
            };

            unitInstance.unitImage.GetComponent<Image>().sprite = character.GetSpriteForLevel(character.unitPeaceLevel);
            unitInstance.unitLevelText.text = $"Lv. {character.unitPeaceLevel}";
            unitInstance.unitPieceSlider.maxValue = character.CharacterMaxPeace;
            unitInstance.unitPieceSlider.value = character.CharacterPeaceCount;
            unitInstance.unitPieceText.text = $"{character.CharacterPeaceCount}/{unitInstance.unitPieceSlider.maxValue}";
        }

        private void SetupInActiveUnitIcon(UnitIcon unitInstance, CharacterBase character)
        {
            unitInstance.CharacterBase = character;
            unitInstance.unitImage.GetComponent<Image>().sprite = lockImage;
            unitInstance.normalBack.GetComponent<Image>().sprite = lockBack;
            unitInstance.unitFrame.GetComponent<Image>().sprite = lockFrame;
            unitInstance.unitLevelText.text = "InActivate";
            unitInstance.unitPieceSlider.maxValue = 0;
            unitInstance.unitPieceSlider.value = 0;
            unitInstance.unitPieceText.text = $"{unitInstance.unitPieceSlider.value}/{unitInstance.unitPieceSlider.maxValue}";

        }

        private void SwapBackGround(UnitIcon unitInstance, CharacterBase characterBase)
        {
            if (_activeStatusPanel == null)
            {
                unitInstance.infoBack.SetActive(true);
                _activeStatusPanel = unitInstance.infoBack;
            }
            else if (_activeStatusPanel != null && _activeStatusPanel != unitInstance.infoBack)
            {
                _activeStatusPanel.SetActive(false);
                unitInstance.infoBack.SetActive(true);
                _activeStatusPanel = unitInstance.infoBack;
            }
         
            switch (characterBase.unLock)
            {
                case true when characterBase.selected:
                {
                    unitInstance.infoBtn.gameObject.SetActive(true);
                    if (characterBase.CharacterPeaceCount >= characterBase.CharacterMaxPeace && CoinsScript.Instance.Coin >= characterBase.CharacterLevelUpCoin)
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
                    if (characterBase.CharacterPeaceCount >= characterBase.CharacterMaxPeace && CoinsScript.Instance.Coin >= characterBase.CharacterLevelUpCoin)
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
                    newUnit.infoBack.SetActive(false);
                    if (_informationPanel == null)
                    {
                        _informationPanel = Instantiate(informationPanelPrefab, gamePanel.transform).GetComponent<InformationPanel>();
                    }
                    _informationPanel.OpenInfoPanel(newUnit, newUnitBase);
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

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_activeStatusPanel == null ||
                _activeStatusPanel.transform == eventData.pointerCurrentRaycast.gameObject.transform) return;
            _activeStatusPanel.SetActive(false);
            _activeStatusPanel = null;
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
                unitIcon.unitPieceSlider.maxValue = characterBase.CharacterMaxPeace;
                unitIcon.unitPieceSlider.value = characterBase.CharacterPeaceCount;
                unitIcon.unitPieceText.text = $"{characterBase.CharacterPeaceCount}/{unitIcon.unitPieceSlider.maxValue}";
            }
        }
    }
}
