using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.RobbyScript.TopMenuGroup;
using Script.UIManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.CharacterSelectMenuGroup
{
    public class InformationPanel : MonoBehaviour
    {
        // Panel
        [SerializeField] private Image panel;
        // unit Name Tag
        [SerializeField] private TextMeshProUGUI nameText;
        // UnitBack
        [SerializeField] private List<Sprite> panelGrade;
        [SerializeField] private Image unitBackGround;
        [SerializeField] private List<Sprite> unitBackGroundSpriteList;
        [SerializeField] private Image unitFrame;
        [SerializeField] private List<Sprite> unitFrameSpriteList;
        [SerializeField] private Image unitImage;
        [SerializeField] private TextMeshProUGUI unitLevelText;
        [SerializeField] private Slider unitPeaceSlider;
        [SerializeField] private TextMeshProUGUI unitPieceText;
        [SerializeField] private Image frameProperty;
        // UnitNotice
        [SerializeField] private Image unitPropertyImage;
        [SerializeField] private TextMeshProUGUI unitPropertyText;
        [SerializeField] private TextMeshProUGUI unitNoticeText;
        [SerializeField] private Image basicShape;
        // unit InformationGrid
        [SerializeField] private GameObject unitInformationGrid;
        [SerializeField] private InfoObject infoObject;
        // unit Skill Grid
        [SerializeField] private GameObject unitSkillGrid;
        [SerializeField] private UnitSkillObject skillObject;
        [SerializeField] internal List<Sprite> skillGradeSpriteList;
        // unit LevelUpBtn
        [SerializeField] private GameObject levelUpBtn;
        [SerializeField] internal List<Sprite> levelUpBtnSprite;
        [SerializeField] private TextMeshProUGUI levelUpText;
        private Sprite _skillSprite;
        private TextMeshProUGUI _skillNoticeText;

        public void OpenInfoPanel(UnitIcon unitInstance, CharacterBase characterBase)
        {
            StartCoroutine(CheckForLevelUp(unitInstance, characterBase));
        }
        private IEnumerator CheckForLevelUp(UnitIcon unitInstance, CharacterBase characterBase)
        {
            levelUpBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            
            if (characterBase.UnitPieceCount >= characterBase.UnitPieceMaxPiece && CoinsScript.Instance.Coin >= characterBase.CharacterLevelUpCoin)
            {
                levelUpBtn.GetComponent<Button>().interactable = true;
                levelUpBtn.GetComponent<Image>().sprite = levelUpBtnSprite[0];
                levelUpBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    StartCoroutine(characterBase.UnitLevelUp(characterBase.unitGroup));
                    StartCoroutine(CheckForLevelUp(unitInstance, characterBase));
                    HoldCharacterList.SyncWithSelected(unitInstance, characterBase);
                    HoldCharacterList.UpdateUnit(unitInstance, characterBase);
                    HoldCharacterList.Instance.UpdateRewardPiece(characterBase);
                    Information(unitInstance, characterBase);
                });
            }
            else if (characterBase.unitPieceLevel >= 14)
            {
                levelUpBtn.GetComponent<Button>().interactable = false;
                levelUpBtn.GetComponent<Image>().sprite = levelUpBtnSprite[1];
            }
            else
            {
                levelUpBtn.GetComponent<Button>().interactable = false;
                levelUpBtn.GetComponent<Image>().sprite = levelUpBtnSprite[1];
            }
            Information(unitInstance,characterBase);
            yield return null;
        }
        private void Information(UnitIcon unitInstance, CharacterBase characterBase)
        {
            unitInstance.CharacterBase = characterBase;
            nameText.text = unitInstance.CharacterBase.name;
            switch (unitInstance.CharacterBase.unLock)
            {
                 case true:
                     switch (unitInstance.CharacterBase.UnitGrade)
                     {
                         case CharacterBase.UnitGrades.G:
                             panel.sprite = panelGrade[0];
                             unitBackGround.sprite = unitBackGroundSpriteList[0];
                             unitFrame.sprite = unitFrameSpriteList[0];
                             break;
                         case CharacterBase.UnitGrades.B:
                             panel.sprite = panelGrade[1];
                             unitBackGround.sprite = unitBackGroundSpriteList[1];
                             unitFrame.sprite = unitFrameSpriteList[1];
                             break;
                         case CharacterBase.UnitGrades.P:
                             panel.sprite = panelGrade[2];
                             unitBackGround.sprite = unitBackGroundSpriteList[2];
                             unitFrame.sprite = unitFrameSpriteList[2];
                             break;
                     }
                     break;
                 case false:
                     panel.sprite = panelGrade[3];
                     unitBackGround.sprite = unitBackGroundSpriteList[3];
                     unitFrame.sprite = unitFrameSpriteList[3];
                     break;
            }

            unitImage.sprite = unitInstance.CharacterBase.GetSpriteForLevel(unitInstance.CharacterBase.unitPieceLevel);
            unitLevelText.text = $"LV.{unitInstance.CharacterBase.unitPieceLevel}";
            unitPeaceSlider.value = unitInstance.CharacterBase.UnitPieceCount;
            unitPeaceSlider.maxValue = unitInstance.CharacterBase.UnitPieceMaxPiece;
            unitPieceText.text = $"{unitInstance.CharacterBase.UnitPieceCount}/{unitInstance.CharacterBase.UnitPieceMaxPiece}";
            switch (unitInstance.CharacterBase.UnitProperty)
            {
                case CharacterBase.UnitProperties.Darkness:
                    unitInstance.unitProperty.GetComponent<Image>().sprite = unitInstance.unitPropertiesSprite[0];
                    frameProperty.sprite = unitInstance.unitPropertiesSprite[0];
                    unitPropertyImage.sprite = unitInstance.unitPropertiesSprite[0];
                    break;
                case CharacterBase.UnitProperties.Fire:
                    unitInstance.unitProperty.GetComponent<Image>().sprite = unitInstance.unitPropertiesSprite[1];
                    frameProperty.sprite = unitInstance.unitPropertiesSprite[1];
                    unitPropertyImage.sprite = unitInstance.unitPropertiesSprite[1];
                    break;
                case CharacterBase.UnitProperties.Physics:
                    unitInstance.unitProperty.GetComponent<Image>().sprite = unitInstance.unitPropertiesSprite[2];
                    frameProperty.sprite = unitInstance.unitPropertiesSprite[2];
                    unitPropertyImage.sprite = unitInstance.unitPropertiesSprite[2];
                    break;
                case CharacterBase.UnitProperties.Poison:
                    unitInstance.unitProperty.GetComponent<Image>().sprite = unitInstance.unitPropertiesSprite[3];
                    frameProperty.sprite = unitInstance.unitPropertiesSprite[3];
                    unitPropertyImage.sprite = unitInstance.unitPropertiesSprite[3];
                    break;
                case CharacterBase.UnitProperties.Water:
                    unitInstance.unitProperty.GetComponent<Image>().sprite = unitInstance.unitPropertiesSprite[4];
                    frameProperty.sprite = unitInstance.unitPropertiesSprite[4];
                    unitPropertyImage.sprite = unitInstance.unitPropertiesSprite[4];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitInstance.CharacterBase.UnitProperty));
            }
            basicShape.sprite = unitInstance.CharacterBase.GetBasicSprite();
            unitPropertyText.text = UnitPropertyText(unitInstance.CharacterBase);
            unitNoticeText.text = unitInstance.CharacterBase.UnitDesc;
            var unitDataList = UnitData(unitInstance.CharacterBase);
            PopulateUnitInfoObject(unitDataList, unitInstance.CharacterBase);
            var unitSkillLists = UnitSkills(unitInstance.CharacterBase);
            PopulateUnitSkillObject(unitSkillLists, unitInstance.CharacterBase);
            var unitLevelUpPrice = unitInstance.CharacterBase.CharacterLevelUpCoin;
            levelUpText.text = unitInstance.CharacterBase.unitPieceLevel < 14 ? unitLevelUpPrice.ToString() : "Max Level";
       
        }
        private static string UnitPropertyText(CharacterBase characterBase)
        {
            var unitProperties = characterBase.UnitProperty;
            var unit = unitProperties switch
            {
                CharacterBase.UnitProperties.Darkness => "Darkness",
                CharacterBase.UnitProperties.Physics => "Physical",
                CharacterBase.UnitProperties.Water => "Water",
                CharacterBase.UnitProperties.Poison => "Poison",
                CharacterBase.UnitProperties.Fire => "Fire",
            };
            return unit;
        }
        private static List<Dictionary<string, object>> UnitData(CharacterBase characterBase)
        {
            var result = new List<Dictionary<string, object>>();
            var unitInfoFile = Resources.Load<TextAsset>("UnitInformationData");
            var unitInfoData = unitInfoFile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            for (var i = 1; i < unitInfoData.Length; i++)
            {
                var data = unitInfoData[i].Split(',');
                var unitGroup = (CharacterBase.UnitGroups)Enum.Parse(typeof(CharacterBase.UnitGroups), data[0], true);
                if (unitGroup != characterBase.unitGroup) continue;
                var unitDataDict = new Dictionary<string, object>
                {
                    {"AttackRange", data[1]},
                    {"Damage", data[2]},
                    {"AttackSpeed", data[3]},
                    {"Type", data[4]},
                    {"Duration", data[5]},
                    {"Intensity", data[6]},
                    {"Bounce", data[7]}
                };
                result.Add(unitDataDict);
            }
            return result;
        }
        private void PopulateUnitInfoObject(List<Dictionary<string, object>> unitDataList, CharacterBase characterBase)
        {
            foreach (Transform child in unitInformationGrid.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var unitDataDict in unitDataList)
            {
                string currentType = null;

                foreach (var unitData in unitDataDict.Where(unitData => unitData.Value.ToString() != "Null"))
                {
                    if (unitData.Key == "Type")
                    {
                        currentType = unitData.Value.ToString();
                        continue; 
                    }
                    var instance = Instantiate(infoObject, unitInformationGrid.transform);

                    switch (unitData.Key)
                    {
                        case "AttackRange":
                            instance.infoIcon.sprite = instance.attackRangeSprite;
                            instance.infoTitle.text = "Attack Range";
                            instance.infoDesc.text = unitData.Value.ToString();
                            break;
                        case "Damage":
                            instance.infoIcon.sprite = instance.damageSprite;
                            instance.infoTitle.text = "Damage";
                            instance.infoDesc.text = characterBase.unitPieceLevel == 1 ? $"{unitData.Value}" : $"{unitData.Value}(+{characterBase.UnitLevelDamage})";
                            break;
                        case "AttackSpeed":
                            instance.infoIcon.sprite = instance.attackSpeedSprite;
                            instance.infoTitle.text = "Attack Speed (Per Second)";
                            instance.infoDesc.text = unitData.Value.ToString();
                            break;
                        case "Type":
                            currentType = unitData.Value.ToString();
                            break;
                        case "Duration":
                            switch (currentType)
                            {
                                case "Slow":
                                    instance.infoIcon.sprite = instance.durationSprites[0];
                                    instance.infoTitle.text = "SlowDown Duration";
                                    break;
                                case "Burn":
                                    instance.infoIcon.sprite = instance.durationSprites[1];
                                    instance.infoTitle.text = "Burning Duration";
                                    break;
                                case "Poison":
                                    instance.infoIcon.sprite = instance.durationSprites[2];
                                    instance.infoTitle.text = "Poison Duration";
                                    break;
                                case "Bleed":
                                    instance.infoIcon.sprite = instance.durationSprites[3];
                                    instance.infoTitle.text = "Bleed Duration";
                                    break;
                            }
                            instance.infoDesc.text = unitData.Value.ToString();
                            break;
                        case "Intensity":
                            switch (currentType)
                            {
                                case "Slow":
                                    instance.infoIcon.sprite = instance.intensitySprites[0];
                                    instance.infoTitle.text = "SlowDown Intensity";
                                    break;
                                case "Burn":
                                    instance.infoIcon.sprite = instance.intensitySprites[1];
                                    instance.infoTitle.text = "Burning Damage";
                                    break;
                                case "Poison":
                                    instance.infoIcon.sprite = instance.intensitySprites[2];
                                    instance.infoTitle.text = "Poison Damage";
                                    break;
                                case "Bleed":
                                    instance.infoIcon.sprite = instance.intensitySprites[3];
                                    instance.infoTitle.text = "Bleed Damage";
                                    break;
                            }
                            instance.infoDesc.text = unitData.Value.ToString();
                            break;
                        case "Bounce":
                            instance.infoIcon.sprite = instance.bounceSprite;
                            instance.infoTitle.text = "Bounces";
                            instance.infoDesc.text = unitData.Value.ToString();
                            break;
                    }
                }
            }
        }
        private static List<Dictionary<string, object>> UnitSkills(CharacterBase characterBase)
        {
            var unitSkillFile = Resources.Load<TextAsset>("SkillData");
            var unitSkillData = unitSkillFile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var skillList = new List<Dictionary<string, object>>();
            for (var i = 1; i < unitSkillData.Length; i++)
            {
                var data = unitSkillData[i].Split(",");
                var unitGroup = (CharacterBase.UnitGroups)Enum.Parse(typeof(CharacterBase.UnitGroups), data[0], true);
                if (unitGroup != characterBase.unitGroup) continue;
                var unitSkillDict = new Dictionary<string, object>
                {
                    { "Grade", data[4] },
                    { "Level", int.Parse(data[1]) },
                    { "Type", data[3] },
                    { "EngDesc", data[6] },
                    { "PopupDesc", data[7]}
                };
                skillList.Add(unitSkillDict);
            }
            skillList.Sort((a, b) => {
                var levelComparison = ((int)a["Level"]).CompareTo((int)b["Level"]);
                return levelComparison == 0 ? string.Compare((string)a["Grade"], (string)b["Grade"], StringComparison.Ordinal) : levelComparison;
            });
            return skillList;
        }
        private void PopulateUnitSkillObject(List<Dictionary<string, object>> skills, CharacterBase characterBase)
        {
            foreach (Transform child in unitSkillGrid.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var skill in skills)
            {
                var instance = Instantiate(skillObject, unitSkillGrid.transform);
                if (characterBase.unitPieceLevel < (int)skill["Level"])
                {
                    instance.skillBackground.sprite = instance.background[3];
                }
                else
                {
                    switch ((string)skill["Grade"])
                    {
                        case "G":
                            instance.skillBackground.sprite = instance.background[0];
                            break;
                        case "B":
                            instance.skillBackground.sprite = instance.background[1];
                            break;
                        case "P":
                            instance.skillBackground.sprite = instance.background[2];
                            break;
                    }
                }
                var skillLevel = (int)skill["Level"];
                instance.skillLevel = skillLevel;
                if (characterBase.UnitSkillDict.TryGetValue(skillLevel, out var skillSpriteDict))
                {
                    instance.skillIcon.sprite = characterBase.unitPieceLevel < skillLevel 
                        ? skillSpriteDict.Values.First() :
                        skillSpriteDict.Keys.First();
                }
                switch (skillLevel)
                {
                    case <=3:
                    case >=11:
                        instance.rightDesc.text =  (string)skill["PopupDesc"];
                        break;
                    case 5:
                        instance.centerDesc.text =  (string)skill["PopupDesc"];
                        break;
                    default:
                        instance.leftDesc.text = (string)skill["PopupDesc"];
                        break;
                }
              
                // instance.skillType.text = $"{skill["Type"]} / {skill["Level"]}";
            }
        }
        public void ClosePanel()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.popupClose);
            Destroy(gameObject);
        }
    }
}
