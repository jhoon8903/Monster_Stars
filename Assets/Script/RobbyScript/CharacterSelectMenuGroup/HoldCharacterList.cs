using System;
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
        private GameObject _activeStatusPanel;
        private static readonly Dictionary<UnitIcon, UnitIcon> UnitIconMapping = new Dictionary<UnitIcon, UnitIcon>();
        private InformationPanel _informationPanel;
        public static HoldCharacterList Instance { get; private set; }
        public bool update;

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
            update = true;
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
            foreach (var character in characterList)
            {           
                character.Initialize();
                if (character.UnLock)
                {
                    if (PlayerPrefs.HasKey(character.unitGroup.ToString()))
                    {
                        character.Selected = true;
                        SelectedUnitHolder.Instance.selectedUnit.Add(character);
                    }
                    if (character.Selected)
                    {
                        var selectedUnitInstance = Instantiate(unitIconPrefab, selectedContent.transform, false);
                        SetupUnitIcon(selectedUnitInstance, character);
                        UpdateMainUnitContent();
                    }
                    else
                    {
                        var activeUnitInstance = Instantiate(unitIconPrefab, selectedContent.transform, false);
                        activeUnitInstance.transform.SetParent(activateUnitContent.transform, false);
                        SetupUnitIcon(activeUnitInstance,character);
                    }
                }
                else
                {
                    var unitInstance = Instantiate(unitIconPrefab, inActivateUnitContent.transform, false);
                    SetupInActiveUnitIcon(unitInstance, character);
                }
            }
        }

        private void SetupUnitIcon(UnitIcon unitInstance, CharacterBase character)
        {
            unitInstance.CharacterBase = character;
            UpdateUnit(unitInstance, character);
            unitInstance.GetComponent<Button>().onClick.AddListener(() =>
            {
                OpenStatusPanel(unitInstance, character);
            });
            unitInstance.infoBtn.onClick.AddListener(() =>
            {
                unitInstance.statusPanel.SetActive(false);
                _activeStatusPanel = null;
                if (_informationPanel == null)
                {
                    _informationPanel = Instantiate(informationPanelPrefab, gamePanel.transform).GetComponent<InformationPanel>();
                }
                _informationPanel.gameObject.SetActive(true);
                _informationPanel.OpenInfoPanel(unitInstance, character);
            });
            unitInstance.levelUpBtn.onClick.AddListener(() =>
            {
                unitInstance.statusPanel.SetActive(false);
                _activeStatusPanel = null;
                if (_informationPanel == null)
                {
                    _informationPanel = Instantiate(informationPanelPrefab, gamePanel.transform).GetComponent<InformationPanel>();
                }
                _informationPanel.gameObject.SetActive(true);
                _informationPanel.OpenInfoPanel(unitInstance, character);
            });
            unitInstance.removeBtn.onClick.AddListener(() =>
            {
                character.Selected = false;
                SelectedUnitHolder.Instance.selectedUnit.Remove(character);
                PlayerPrefs.DeleteKey(character.unitGroup.ToString());
                unitInstance.transform.SetParent(activateUnitContent.transform);
                unitInstance.statusPanel.SetActive(false);
                UpdateMainUnitContent();
            });
            unitInstance.useBtn.onClick.AddListener(() =>
            {
                if (SelectedUnitHolder.Instance.selectedUnit.Count < 4)
                {
                    character.Selected = true;
                    SelectedUnitHolder.Instance.selectedUnit.Add(character);
                    PlayerPrefs.SetInt(character.unitGroup.ToString(), 1);
                    PlayerPrefs.Save();
                    unitInstance.transform.SetParent(selectedContent.transform);
                    unitInstance.statusPanel.SetActive(false);
                    UpdateMainUnitContent();
                }
                else
                { 
                    warningPanel.SetActive(true);
                    messageText.text = "더이상 배치할 수 없습니다.";
                }
            });
        }

        public static void UpdateUnit(UnitIcon unitInstance, CharacterBase character)
        {
            unitInstance.unitBackGround.GetComponent<Image>().color = character.UnitGrade switch
            {
                CharacterBase.UnitGrades.Green => Color.green,
                CharacterBase.UnitGrades.Blue => Color.blue,
                CharacterBase.UnitGrades.Purple => Color.magenta,
                _ => throw new ArgumentOutOfRangeException(nameof(character.UnitGrade))
            };
            
            unitInstance.unit.GetComponent<Image>().sprite = character.GetSpriteForLevel(character.unitPieceLevel);
            unitInstance.levelBack.color = character.UnitGrade switch
            {
                CharacterBase.UnitGrades.Blue => new Color(0.1489f, 0f, 0.6226f, 1f),
                CharacterBase.UnitGrades.Purple => new Color(0.5188f, 0.06162f, 0f, 1f),
                CharacterBase.UnitGrades.Green => new Color(0f, 0.5566f, 0.1430f, 1f),
                _ => throw new ArgumentOutOfRangeException(nameof(character.UnitProperty))
            };
            unitInstance.unitLevelText.text = $"레벨 {character.unitPieceLevel}";
            unitInstance.unitPieceSlider.maxValue = character.CharacterMaxPiece;
            unitInstance.unitPieceSlider.value = character.CharacterPieceCount;
            unitInstance.unitPieceText.text = $"{character.CharacterPieceCount}/{unitInstance.unitPieceSlider.maxValue}";
        }
        private void SetupInActiveUnitIcon(UnitIcon unitInstance, CharacterBase character)
        {
            unitInstance.CharacterBase = character;
            unitInstance.unitBackGround.GetComponent<Image>().color = Color.gray;
            unitInstance.unit.GetComponent<Image>().sprite = character.GetSpriteForLevel(1);
            unitInstance.unit.GetComponent<Image>().color = Color.grey;
            unitInstance.levelBack.color = new Color(0.4433f, 0.4433f, 0.4433f, 1f);
            unitInstance.unitLevelText.text = "비활성화";
            unitInstance.unitPieceSlider.maxValue = 5;
            unitInstance.unitPieceSlider.value = 0;
            unitInstance.unitPieceText.text = $"{unitInstance.unitPieceSlider.value}/{unitInstance.unitPieceSlider.maxValue}";
            unitInstance.GetComponent<Button>().onClick.AddListener(() =>
            {
                OpenStatusPanel(unitInstance, character);
            });

        }
        private void OpenStatusPanel(UnitIcon unitInstance, CharacterBase characterBase)
        {
            if (_activeStatusPanel == null)
            {
                unitInstance.statusPanel.SetActive(true);
                _activeStatusPanel = unitInstance.statusPanel;
            }
            else if (_activeStatusPanel != null && _activeStatusPanel != unitInstance.statusPanel)
            {
                _activeStatusPanel.SetActive(false);
                unitInstance.statusPanel.SetActive(true);
                _activeStatusPanel = unitInstance.statusPanel;
            }
         
            switch (characterBase.UnLock)
            {
                case true when characterBase.Selected:
                {
                    unitInstance.infoBtn.gameObject.SetActive(true);
                    if (characterBase.CharacterPieceCount >= characterBase.CharacterMaxPiece && CoinsScript.Instance.Coin >= characterBase.unitPieceLevel * 500)
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
                case true when !characterBase.Selected:
                {
                    unitInstance.infoBtn.gameObject.SetActive(true);
                    if (characterBase.CharacterPieceCount >= characterBase.CharacterMaxPiece && CoinsScript.Instance.Coin >= characterBase.unitPieceLevel * 500)
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

        public void UpdateMainUnitContent()
        {
            UnitIconMapping.Clear();
            foreach (Transform child in mainUnitContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in selectedContent.transform)
            {
                var newUnitInstance = Instantiate(child.gameObject, mainUnitContent.transform, false);
                var newUnit = newUnitInstance.GetComponent<UnitIcon>();
                var originalUnit = child.GetComponent<UnitIcon>();
                UnitIconMapping[originalUnit] = newUnit;
                newUnit.CharacterBase = originalUnit.CharacterBase;
                var newUnitBase = newUnit.CharacterBase;
                newUnit.GetComponent<Button>().onClick.AddListener(() =>
                {
                    newUnit.statusPanel.SetActive(false);
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
            correspondingUnit.statusPanel.SetActive(unitIcon.statusPanel.activeSelf);
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
                unitIcon.unitPieceSlider.maxValue = characterBase.CharacterMaxPiece;
                unitIcon.unitPieceSlider.value = characterBase.CharacterPieceCount;
                unitIcon.unitPieceText.text = $"{characterBase.CharacterPieceCount}/{unitIcon.unitPieceSlider.maxValue}";
            }
        }

    }
}
