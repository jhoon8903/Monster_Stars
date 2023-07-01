using System;
using Script.CharacterManagerScript;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.CharacterSelectMenuGroup
{
    public class InformationPanel : MonoBehaviour
    {
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private GameObject unit;
        [SerializeField] private TextMeshProUGUI unitProperty;
        [SerializeField] private TextMeshProUGUI unitNoticeText;
        [SerializeField] private GameObject unitInformation;
        [SerializeField] private GameObject unitSkillList;
        [SerializeField] private TextMeshProUGUI levelUpCoinText;
        private Sprite _skillSprite;
        private TextMeshProUGUI _skillNoticeText;

        public void OpenInfoPanel(UnitIcon unitInstance, CharacterBase characterBase)
        {
            infoPanel.SetActive(true);
            Information(unitInstance, characterBase);
        }

        private void Information(Component unitInstance, CharacterBase characterBase)
        {
            nameText.text = characterBase.name;
            unit.GetComponent<Image>().sprite = characterBase.GetComponent<Image>().sprite;
            unitProperty.text = UnitPropertyText(characterBase);
            // unitNoticeText.text = "UnitNotice.csv";
            // foreach (var unitInfo in unitInformationNotice)
            // {
            //     unitInformation.transform = unitInfo;
            //     skillSprite = unitInfo;
            //     skillNoticeText.text = unitInfo;
            // }
            // foreach (var skill in unitSkillListCsv)
            // {
            //     unitSkillList = skill;
            // }
            var unitLevelUpPrice = characterBase.CharacterObjectLevel * 500;
            levelUpCoinText.text = unitLevelUpPrice.ToString();
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
    }
}
