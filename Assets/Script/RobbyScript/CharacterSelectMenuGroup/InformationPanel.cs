using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private GameObject infoPanel;
        // unit Name Tag
        [SerializeField] private TextMeshProUGUI nameText;
        // unit Image
        [SerializeField] private Image unitBackGround;
        [SerializeField] private List<Sprite> unitBackGroundSpriteList;
        [SerializeField] private Image unitFrame;
        [SerializeField] private List<Sprite> unitFrameSpriteList;
        [SerializeField] private Image unitImage;
        [SerializeField] private TextMeshProUGUI unitLevelText;
        [SerializeField] private Slider unitPeaceSlider;
        // unit Desc && Properties
        [SerializeField] private Image unitPropertyImage;
        [SerializeField] private TextMeshProUGUI unitPropertyText;
        [SerializeField] private TextMeshProUGUI unitNoticeText;
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
        private string _selectLang;

        private void Awake()
        {
            _selectLang = PlayerPrefs.GetString("Language","ENG");
        }
        public void OpenInfoPanel(UnitIcon unitInstance, CharacterBase characterBase)
        {
            infoPanel.SetActive(true);
            StartCoroutine(CheckForLevelUp(unitInstance, characterBase));
        }
        private IEnumerator CheckForLevelUp(UnitIcon unitInstance, CharacterBase characterBase)
        {
            levelUpBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            
            if (characterBase.CharacterPeaceCount >= characterBase.CharacterMaxPeace && CoinsScript.Instance.Coin >= characterBase.CharacterLevelUpCoin)
            {
                levelUpBtn.GetComponent<Button>().interactable = true;
                levelUpBtn.GetComponent<Image>().sprite = levelUpBtnSprite[0];
                levelUpBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    StartCoroutine(characterBase.UnitLevelUp());
                    StartCoroutine(CheckForLevelUp(unitInstance, characterBase));
                    HoldCharacterList.SyncWithSelected(unitInstance, characterBase);
                    HoldCharacterList.UpdateUnit(unitInstance, characterBase);
                    HoldCharacterList.Instance.UpdateRewardPiece(characterBase);
                    Information(unitInstance, characterBase);
                });
            }
            else if (characterBase.unitPeaceLevel >= 14)
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
            // unit Name
            nameText.text = characterBase.name;
            // unit Area-Unit Image
            switch (characterBase.UnitGrade)
            {
                case CharacterBase.UnitGrades.Green:
                    unitBackGround.GetComponent<Image>().sprite = unitBackGroundSpriteList[0];
                    unitFrame.GetComponent<Image>().sprite = unitFrameSpriteList[0];
                    break;
                case CharacterBase.UnitGrades.Blue:
                    unitBackGround.GetComponent<Image>().sprite = unitBackGroundSpriteList[1];
                    unitFrame.GetComponent<Image>().sprite = unitFrameSpriteList[1];
                    break;
                case CharacterBase.UnitGrades.Purple:
                    unitBackGround.GetComponent<Image>().sprite = unitBackGroundSpriteList[2];
                    unitFrame.GetComponent<Image>().sprite = unitFrameSpriteList[2];
                    break;
                default:
                    unitBackGround.GetComponent<Image>().sprite = unitBackGroundSpriteList[3];
                    unitFrame.GetComponent<Image>().sprite = unitFrameSpriteList[3];
                    break;
            }
            unitImage.GetComponent<Image>().sprite = characterBase.GetSpriteForLevel(characterBase.unitPeaceLevel);
            unitLevelText.text = $"LV.{characterBase.unitPeaceLevel}";
            unitPeaceSlider.value = characterBase.CharacterPeaceCount;
            unitPeaceSlider.maxValue = characterBase.CharacterMaxPeace;
            // unit Area-Desc
            unitPropertyImage.GetComponent<Image>().sprite = characterBase.UnitProperty switch
            {
                CharacterBase.UnitProperties.Divine => unitInstance.unitPropertiesSprite[0],
                CharacterBase.UnitProperties.Darkness => unitInstance.unitPropertiesSprite[1],
                CharacterBase.UnitProperties.Physics => unitInstance.unitPropertiesSprite[2],
                CharacterBase.UnitProperties.Water => unitInstance.unitPropertiesSprite[3],
                CharacterBase.UnitProperties.Poison => unitInstance.unitPropertiesSprite[4],
                CharacterBase.UnitProperties.Fire => unitInstance.unitPropertiesSprite[5],
            };
            unitPropertyText.text = UnitPropertyText(characterBase);
            unitNoticeText.text = characterBase.UnitDesc;
            // unit Info Grid
            var unitDataList = UnitData(characterBase);
            PopulateUnitInfoObject(unitDataList, characterBase);
            // unit Skill Grid
            var unitSkillLists = UnitSkills(characterBase);
            PopulateUnitSkillObject(unitSkillLists, characterBase);
            // unit LevelUp Btn
            var unitLevelUpPrice = characterBase.CharacterLevelUpCoin;
            levelUpText.text = characterBase.unitPeaceLevel < 14 ? unitLevelUpPrice.ToString() : "Max Level";
       
        }
        private static string UnitPropertyText(CharacterBase characterBase)
        {
            var unitProperties = characterBase.UnitProperty;
            var unit = unitProperties switch
            {
                CharacterBase.UnitProperties.Divine => "Divine",
                CharacterBase.UnitProperties.Darkness => "Darkness",
                CharacterBase.UnitProperties.Physics => "Physical",
                CharacterBase.UnitProperties.Water => "Water",
                CharacterBase.UnitProperties.Poison => "Poison",
                CharacterBase.UnitProperties.Fire => "Fire",
            };
            return unit;
        }
        private List<Dictionary<string, object>> UnitData(CharacterBase characterBase)
        {
            var result = new List<Dictionary<string, object>>();
            var unitInfoFile = Resources.Load<TextAsset>("UnitInformationData");
            var unitInfoData = unitInfoFile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            for (var i = 1; i < unitInfoData.Length; i++)
            {
                var data = unitInfoData[i].Split(',');
                var unitGroup = (CharacterBase.UnitGroups)Enum.Parse(typeof(CharacterBase.UnitGroups), data[1], true);
                if (unitGroup != characterBase.unitGroup) continue;
                var language = data[0];
                if (language != _selectLang) continue;
                var unitDataDict = new Dictionary<string, object>
                {
                    {"Damage", data[2]},
                    {"Range", data[3]},
                    {"Rate", data[4]},
                    {"Form", data[5]},
                    {"Effect", data[6]},
                    {"Time", data[7]},
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
                foreach (var unitData in unitDataDict)
                {
                    if (unitData.Value as string == "Null" && unitData.Key is "Effect" or "Time") continue;
                    var instance = Instantiate(infoObject, unitInformationGrid.transform);
                    switch (unitData.Key)
                    {
                        case "Damage":
                            instance.infoTitle.text = _selectLang == "KOR" ? "공격력" : "Damage";
                            instance.infoDesc.text = characterBase.unitPeaceLevel == 1 ? $"{unitData.Value}" : $"{unitData.Value} + {characterBase.UnitLevelDamage}";
                            break;
                        case "Range":
                            instance.infoTitle.text = _selectLang == "KOR" ? "공격범위" : "Range";
                            instance.infoDesc.text = unitData.Value as string;
                            break;
                        case "Rate":
                            instance.infoTitle.text = _selectLang == "KOR" ? "공격속도" : "Rate";
                            instance.infoDesc.text = unitData.Value as string;
                            break;
                        case "Form":
                            instance.infoTitle.text = _selectLang == "KOR" ? "공격형태" : "Form";
                            instance.infoDesc.text = unitData.Value as string;
                            break;
                        case "Effect":
                            instance.infoTitle.text = _selectLang == "KOR" ? "특수효과" : "Effect";
                            instance.infoDesc.text = unitData.Value as string;
                            break;
                        case "Time":
                            instance.infoTitle.text = _selectLang == "KOR" ? "지속시간" : "Time";
                            instance.infoDesc.text = unitData.Value as string;
                            break;
                    }
                }
            }
        }

        private static List<Dictionary<string, object>> UnitSkills(CharacterBase characterBase)
        {
            var unitSkillFile = Resources.Load<TextAsset>("UnitSkillData");
            var unitSkillData = unitSkillFile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var skillList = new List<Dictionary<string, object>>();
            for (var i = 1; i < unitSkillData.Length; i++)
            {
                var data = unitSkillData[i].Split(",");
                var unitGroup = (CharacterBase.UnitGroups)Enum.Parse(typeof(CharacterBase.UnitGroups), data[0], true);
                if (unitGroup != characterBase.unitGroup) continue;
                var unitSkillDict = new Dictionary<string, object>
                {
                    { "Grade", data[1] },
                    { "Level", int.Parse(data[2]) },
                    { "Type", data[3] },
                    { "KorDesc", data[4] },
                    { "EngDesc", data[5] }
                };
                skillList.Add(unitSkillDict);
            }
            skillList.Sort((a, b) => {
                var levelComparison = ((int)a["Level"]).CompareTo((int)b["Level"]);
                return levelComparison == 0 ? string.Compare(((string)a["Grade"]), (string)b["Grade"], StringComparison.Ordinal) : levelComparison;
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
                if (characterBase.unitPeaceLevel < (int)skill["Level"])
                {
                    instance.GetComponent<Image>().sprite = skillGradeSpriteList[3];
                }
                else
                {
                    switch ((string)skill["Grade"])
                    {
                        case "G":
                            instance.GetComponent<Image>().sprite = skillGradeSpriteList[0];
                            break;
                        case "B":
                            instance.GetComponent<Image>().sprite = skillGradeSpriteList[1];
                            break;
                        case "P":
                            instance.GetComponent<Image>().sprite = skillGradeSpriteList[2];
                            break;
                        default:
                            Debug.LogError("Unexpected grade: " + skill["Grade"]);
                            break;
                    }
                }
                var skillLevel = (int)skill["Level"];
                if (characterBase.UnitSkillDict.TryGetValue(skillLevel, out var skillSprite))
                {
                    // instance.skillIcon.sprite = null;
                    instance.skillIcon.sprite = skillSprite;
                }
                instance.skillDesc.text = _selectLang == "KOR" ? (string)skill["KorDesc"] : (string)skill["EngDesc"];
                // instance.skillType.text = $"{skill["Type"]} / {skill["Level"]}";
            }
        }
        public void ClosePanel()
        {
            gameObject.SetActive(false);
        }
    }
}
