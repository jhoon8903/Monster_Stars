using System;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
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
        [SerializeField] private InformationPanel informationPanel;
        [SerializeField] private GameObject warningPanel;
        [SerializeField] private TextMeshProUGUI messageText;
        private GameObject _activeStatusPanel;
        private readonly Dictionary<UnitIcon, UnitIcon> _unitIconMapping = new Dictionary<UnitIcon, UnitIcon>();

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
                informationPanel.OpenInfoPanel(unitInstance, character);
            });
            unitInstance.levelUpBtn.onClick.AddListener(() =>
            {
                unitInstance.statusPanel.SetActive(false);
                _activeStatusPanel = null;
                informationPanel.OpenInfoPanel(unitInstance, character);
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
            unitInstance.unitBackGround.GetComponent<Image>().color = character.UnitProperty switch
            {
                CharacterBase.UnitProperties.Divine => new Color(0.9725f, 1f, 0f, 1f),
                CharacterBase.UnitProperties.Darkness => new Color(0.2402f, 0f, 1f, 1f),
                CharacterBase.UnitProperties.Fire => new Color(1f, 0.0319f, 0f, 1f),
                CharacterBase.UnitProperties.Physics => new Color(0.6509f, 0.6509f, 0.6509f, 1f),
                CharacterBase.UnitProperties.Poison => new Color(0f, 1f, 0.2585f, 1f),
                CharacterBase.UnitProperties.Water => new Color(0f, 0.6099f, 1f, 1f),
                CharacterBase.UnitProperties.None => new Color(1f, 1f, 1f, 1f),
                _ => throw new ArgumentOutOfRangeException(nameof(character.UnitProperty))
            };
            
            unitInstance.unit.GetComponent<Image>().sprite = character.GetSpriteForLevel(character.UnitGameLevel);
            
            unitInstance.levelBack.color = character.UnitProperty switch
            {
                CharacterBase.UnitProperties.Divine => new Color(0.7064f, 0.7264f, 0f, 1f),
                CharacterBase.UnitProperties.Darkness => new Color(0.1489f, 0f, 0.6226f, 1f),
                CharacterBase.UnitProperties.Fire => new Color(0.5188f, 0.06162f, 0f, 1f),
                CharacterBase.UnitProperties.Physics => new Color(0.4433f, 0.4433f, 0.4433f, 1f),
                CharacterBase.UnitProperties.Poison => new Color(0f, 0.5566f, 0.1430f, 1f),
                CharacterBase.UnitProperties.Water => new Color(0f, 0.3231f, 0.5183f, 1f),
                CharacterBase.UnitProperties.None => new Color(1f, 1f, 1f, 1f),
                _ => throw new ArgumentOutOfRangeException(nameof(character.UnitProperty))
            };
            unitInstance.unitLevelText.text = $"레벨 {character.UnitGameLevel}";
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
                    if (characterBase.CharacterPieceCount >= characterBase.CharacterMaxPiece)
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
                    if (characterBase.CharacterPieceCount >= characterBase.CharacterMaxPiece)
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
                    informationPanel.OpenInfoPanel(unitInstance, characterBase);
                    break;
            }
        }
        private void UpdateMainUnitContent()
        {
            _unitIconMapping.Clear();

            foreach (Transform child in mainUnitContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in selectedContent.transform)
            {
                var newUnitInstance = Instantiate(child.gameObject, mainUnitContent.transform, false);
                var newUnit = newUnitInstance.GetComponent<UnitIcon>();
                var originalUnit = child.GetComponent<UnitIcon>();
                _unitIconMapping[originalUnit] = newUnit;
                newUnit.CharacterBase = originalUnit.CharacterBase;
                var newUnitBase = newUnit.CharacterBase;
                newUnit.GetComponent<Button>().onClick.AddListener(() =>
                {
                    newUnit.statusPanel.SetActive(false);
                    informationPanel.OpenInfoPanel(newUnit, newUnitBase);
                    SyncWithSelected(newUnit, newUnitBase);
                });
            }
        }
        public void SyncWithSelected(UnitIcon unitIcon, CharacterBase unitBase)
        {
            var correspondingUnit 
                = (from pair in _unitIconMapping where pair.Key 
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
    }
}
