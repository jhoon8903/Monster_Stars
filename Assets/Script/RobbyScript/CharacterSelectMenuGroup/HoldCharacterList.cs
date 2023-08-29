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
        [SerializeField] private GameObject selectBackPanel;
        [SerializeField] private Image pointer1;
        [SerializeField] private Image pointer2;
        [SerializeField] private Image pointer3;
        [SerializeField] private Image pointer4;
        
        private GameObject _activeStatusPanel;
        private GameObject _activeNormalBack;
        private static readonly Dictionary<UnitIcon, UnitIcon> UnitIconMapping = new Dictionary<UnitIcon, UnitIcon>();
        private InformationPanel _informationPanel;
        public static HoldCharacterList Instance { get; private set; }
        private bool _blueUnlock;
        public UnitIcon selectedToSwap;
        public UnitIcon secondSwap;
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

            if (Input.GetMouseButtonDown(0))
            {
                var clickedObject = EventSystem.current.currentSelectedGameObject;
                if (clickedObject != null && IsDescendantOrSelf(_activeStatusPanel, clickedObject))
                {
                    return;
                }
                if (_activeStatusPanel != null)
                {
                    _activeStatusPanel.SetActive(false);
                    if (_activeNormalBack != null)  // null 체크 추가
                    {
                        _activeNormalBack.SetActive(true);  // 활성화
                    }
                    _activeStatusPanel = null;
                    _activeNormalBack = null;  // 초기화
                }
            }
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

        public void SwapUnitInstances(UnitIcon first, CharacterBase firstBase, UnitIcon second, CharacterBase secondBase)
        {
            Debug.Log($"First: {first.name} / {firstBase.unitGroup}");
            firstBase.selected = true;
            SelectedUnitHolder.Instance.selectedUnit.Add(firstBase);
            PlayerPrefs.SetInt(firstBase.unitGroup.ToString(), 1);
            PlayerPrefs.Save();
            first.transform.SetParent(selectedContent.transform);
            first.transform.SetAsLastSibling();
            first.normalBack.SetActive(true);
            first.infoBack.SetActive(false);
            var firstCanvas = first.unitCanvas;
            if (firstCanvas != null)
            {
                firstCanvas.sortingLayerName = "TopMenu";
            }

            Debug.Log($"Second: {second.name} / {secondBase.unitGroup}");
            secondBase.selected = false;
            SelectedUnitHolder.Instance.selectedUnit.Remove(secondBase);
            PlayerPrefs.DeleteKey(secondBase.unitGroup.ToString());
            second.transform.SetParent(activateUnitContent.transform);
            second.transform.SetAsLastSibling();
            second.normalBack.SetActive(true);
            second.infoBack.SetActive(false);
            var secondCanvas = first.unitCanvas;
            if (secondCanvas != null)
            {
                secondCanvas.sortingLayerName = "Unit";
            }

            UpdateMainUnitContent();
            SortChildrenBySortingLayer(activateUnitContent.transform);
            AdjustRectTransform(activateUnitContent.transform);
        }
        public void InstanceUnit()
        {
            var sortingLayerOder = 11;
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
                if (_informationPanel == null)
                {
                    _informationPanel = Instantiate(informationPanelPrefab, gamePanel.transform).GetComponent<InformationPanel>();
                }
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
                SoundManager.Instance.PlaySound(SoundManager.Instance.unitSelect);
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
                    selectBackPanel.SetActive(true);
                    StartCoroutine(MovePointer());
                    var canvas = unitInstance.unitCanvas;
                    if (canvas == null) return;
                    canvas.sortingLayerName = "TopMenu";
                    if (selectedToSwap != null) return;
                    selectedToSwap = unitInstance;
                    Debug.Log(selectedToSwap.CharacterBase.unitGroup);
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
       private IEnumerator MovePointer()
        {
            pointer1.rectTransform.localRotation =  Quaternion.Euler(0, 0, 0);
            pointer2.rectTransform.localRotation =  Quaternion.Euler(0, 0, 0);
            pointer3.rectTransform.localRotation =  Quaternion.Euler(0, 0, 0);
            pointer4.rectTransform.localRotation =  Quaternion.Euler(0, 0, 0);
            const float duration = 0.5f;

            while (true)
            {
                pointer1.rectTransform.DOLocalRotate(new Vector3(0, 0, 30), duration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(duration);
                pointer1.rectTransform.DOLocalRotate(new Vector3(0, 0, 0), duration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(duration);
                pointer2.rectTransform.DOLocalRotate(new Vector3(0, 0, 30), duration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(duration);
                pointer2.rectTransform.DOLocalRotate(new Vector3(0, 0, 0), duration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(duration);
                pointer3.rectTransform.DOLocalRotate(new Vector3(0, 0, 30), duration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(duration);
                pointer3.rectTransform.DOLocalRotate(new Vector3(0, 0, 0), duration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(duration);
                pointer4.rectTransform.DOLocalRotate(new Vector3(0, 0, 30), duration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(duration);
                pointer4.rectTransform.DOLocalRotate(new Vector3(0, 0, 0), duration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(duration);
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
                    UpdateUnit(unitIcon, unitIcon.CharacterBase);
                    SetupUnitIcon(unitIcon, unitIcon.CharacterBase);
                    unitIcon.transform.SetParent(activateUnitContent.transform, false);
                    AdjustRectTransform(activateUnitContent.transform);
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


    }
}
