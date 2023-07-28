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
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private GameObject unit;
        [SerializeField] private TextMeshProUGUI unitLevel;
        [SerializeField] private TextMeshProUGUI unitProperty;
        [SerializeField] private TextMeshProUGUI unitNoticeText;
        [SerializeField] private GameObject unitInformationList;
        [SerializeField] private UnitInfoObject unitInfoObject;
        [SerializeField] private GameObject unitSkillList;
        [SerializeField] private UnitSkillObject unitSkillObject;
        [SerializeField] private GameObject levelUpBtn;
        [SerializeField] private TextMeshProUGUI levelUpCoinText;
        private Sprite _skillSprite;
        private TextMeshProUGUI _skillNoticeText;
        private string _selectLang;

        private void Awake()
        {
            _selectLang = PlayerPrefs.GetString("Language","KOR");
        }

        public void OpenInfoPanel(UnitIcon unitInstance, CharacterBase characterBase)
        {
            infoPanel.SetActive(true);
            StartCoroutine(CheckForLevelUp(unitInstance, characterBase));
        }

        private void Information(Component unitInstance, CharacterBase characterBase)
        {
            nameText.text = characterBase.name;
            unit.GetComponent<Image>().sprite = characterBase.GetSpriteForLevel(characterBase.unitPieceLevel);
            unitLevel.text = $"LV.{characterBase.unitPieceLevel}";
            unitProperty.text = UnitPropertyText(characterBase);
            unitNoticeText.text = characterBase.UnitDesc;
            var unitDataList = UnitData(characterBase);
            PopulateUnitInfoObject(unitDataList, characterBase);
            var unitSkillLists = UnitSkills(characterBase);
            PopulateUnitSkillObject(unitSkillLists, characterBase);
            var unitLevelUpPrice = characterBase.CharacterLevelUpCoin;
            levelUpCoinText.text = unitLevelUpPrice.ToString();
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

        private void PopulateUnitInfoObject(List<Dictionary<string, object>> unitDataList, CharacterBase characterBase)
        {
            foreach (Transform child in unitInformationList.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var unitDataDict in unitDataList)
            {
                foreach (var unitData in unitDataDict)
                {
                    if (unitData.Value as string == "Null" && unitData.Key is "Effect" or "Time") continue;
                    var instance = Instantiate(unitInfoObject, unitInformationList.transform);
                    switch (unitData.Key)
                    {
                        case "Damage":
                            instance.infoTitle.text = _selectLang == "KOR" ? "공격력" : "Damage";
                            instance.infoDesc.text = characterBase.unitPieceLevel == 1 ? $"{unitData.Value}" : $"{unitData.Value} + {characterBase.UnitLevelDamage}";
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
        private void PopulateUnitSkillObject(List<Dictionary<string, object>> skills, CharacterBase characterBase)
        {

            foreach (Transform child in unitSkillList.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var skill in skills)
            {
                var instance = Instantiate(unitSkillObject, unitSkillList.transform);
                if (characterBase.unitPieceLevel < (int)skill["Level"])
                {
                    instance.GetComponent<Image>().color = Color.gray;
                }
                else
                {
                    switch ((string)skill["Grade"])
                    {
                        case "G":
                            instance.GetComponent<Image>().color = Color.green;
                            break;
                        case "B":
                            instance.GetComponent<Image>().color = Color.blue;
                            break;
                        case "P":
                            instance.GetComponent<Image>().color = Color.magenta;
                            break;
                        default:
                            Debug.LogError("Unexpected grade: " + skill["Grade"]);
                            break;
                    }
                }
                instance.skillDesc.text = _selectLang == "KOR" ? (string)skill["KorDesc"] : (string)skill["EngDesc"];
                instance.skillType.text = $"{skill["Type"]} / {skill["Level"]}";
            }
        }

        private static string UnitPropertyText(CharacterBase characterBase)
        {
            var unitProperties = characterBase.UnitProperty;
            var unit = unitProperties switch
            {
                CharacterBase.UnitProperties.Divine => "신성",
                CharacterBase.UnitProperties.Darkness => "어둠",
                CharacterBase.UnitProperties.Physics => "물리",
                CharacterBase.UnitProperties.Water => "물",
                CharacterBase.UnitProperties.Poison => "독",
                CharacterBase.UnitProperties.Fire => "불",
                CharacterBase.UnitProperties.None => "무속성",
                _ => throw new ArgumentOutOfRangeException()
            };
            return unit;
        }

        private IEnumerator CheckForLevelUp(UnitIcon unitInstance, CharacterBase characterBase)
        {
            levelUpBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            if (characterBase.CharacterPieceCount >= characterBase.CharacterMaxPiece && CoinsScript.Instance.Coin >= characterBase.CharacterLevelUpCoin)
            {
                levelUpBtn.GetComponent<Button>().interactable = true;
                levelUpBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    StartCoroutine(characterBase.UnitLevelUp());
                    StartCoroutine(CheckForLevelUp(unitInstance, characterBase)); 
                    HoldCharacterList.UpdateUnit(unitInstance, characterBase);
                    HoldCharacterList.SyncWithSelected(unitInstance, characterBase);
                    Information(unitInstance, characterBase);
                });
            }
            else if (characterBase.unitPieceLevel >= 14)
            {
                levelUpBtn.GetComponent<Button>().interactable = false;
                levelUpCoinText.color = Color.gray;
            }
            else
            {
                levelUpBtn.GetComponent<Button>().interactable = false;
                levelUpCoinText.color = Color.gray;
            }
            Information(unitInstance, characterBase);
            yield return null;
        }

        public void ClosePanel()
        {
            gameObject.SetActive(false);
        }
    }
}
