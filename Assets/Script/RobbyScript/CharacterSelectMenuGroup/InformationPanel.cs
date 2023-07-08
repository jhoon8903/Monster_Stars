using System;
using System.Collections;
using Script.CharacterManagerScript;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.CharacterSelectMenuGroup
{
    public class InformationPanel : MonoBehaviour
    {
        [SerializeField] private HoldCharacterList holdCharacterList;
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private GameObject unit;
        [SerializeField] private TextMeshProUGUI unitLevel;
        [SerializeField] private TextMeshProUGUI unitProperty;
        [SerializeField] private TextMeshProUGUI unitNoticeText;
        [SerializeField] private GameObject unitInformation;
        [SerializeField] private GameObject unitSkillList;
        [SerializeField] private GameObject levelUpBtn;
        [SerializeField] private TextMeshProUGUI levelUpCoinText;
        private Sprite _skillSprite;
        private TextMeshProUGUI _skillNoticeText;

        public void OpenInfoPanel(UnitIcon unitInstance, CharacterBase characterBase)
        {
            infoPanel.SetActive(true);
            StartCoroutine(CheckForLevelUp(unitInstance, characterBase));
        }

        private void Information(Component unitInstance, CharacterBase characterBase)
        {
            nameText.text = characterBase.name;
            unit.GetComponent<Image>().sprite = characterBase.GetSpriteForLevel(characterBase.UnitPieceLevel);
            unitLevel.text = $"LV.{characterBase.UnitPieceLevel}";
            unitProperty.text = UnitPropertyText(characterBase);
            // // unitNoticeText.text = "UnitNotice.csv";
            // // foreach (var unitInfo in unitInformationNotice)
            // // {
            // //     unitInformation.transform = unitInfo;
            // //     skillSprite = unitInfo;
            // //     skillNoticeText.text = unitInfo;
            // // }
            // // foreach (var skill in unitSkillListCsv)
            // // {
            // //     unitSkillList = skill;
            // // }
            var unitLevelUpPrice = characterBase.UnitPieceLevel * 500;
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

        private IEnumerator CheckForLevelUp(UnitIcon unitInstance, CharacterBase characterBase)
        {
            levelUpBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            if (characterBase.CharacterPieceCount >= characterBase.CharacterMaxPiece)
            {
                levelUpBtn.GetComponent<Button>().interactable = true;

                levelUpBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    StartCoroutine(characterBase.UnitLevelUp());
                    StartCoroutine(CheckForLevelUp(unitInstance, characterBase)); 
                    HoldCharacterList.UpdateUnit(unitInstance, characterBase);
                    holdCharacterList.SyncWithSelected(unitInstance, characterBase);
                    Information(unitInstance, characterBase);
                });
            }
            else
            {
                levelUpBtn.GetComponent<Button>().interactable = false;
                levelUpCoinText.color = Color.gray;
            }
            Information(unitInstance, characterBase);
            yield return null;
        }
    }
}
