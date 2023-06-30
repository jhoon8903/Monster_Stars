using System;
using System.Collections.Generic;
using Script.CharacterManagerScript;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.CharacterSelectMenuGroup
{
    [Serializable] 
    public class UnitScrollPosition
    {
        public Image unitBackGround;
        public GameObject unit; // 프리팹
        public Slider unitLevelSlider;
        public TextMeshProUGUI unitLevelText;
        public Slider unitPieceSlider;
        public TextMeshProUGUI unitPieceText;
    }

    public class UnitInformation
    {
        public CharacterBase unit;
        public Color unitBackColor;
        public Sprite unitSprite;
        public int unitLevelValue;
        public int unitLevelMaxValue;
        public int unitPieceValue;
        public int unitPieceMaxvalue;
    }
    public class HoldCharacterList : MonoBehaviour
    {
        [SerializeField] private SelectedCharacter selectedCharacter;
        [SerializeField] private List<CharacterBase> characterList = new List<CharacterBase>();
        [SerializeField] private List<UnitScrollPosition> unitScrollPosition;
        [SerializeField] private Sprite defaultLocker;
        public UnitInformation unitInformation;

        private void Start()
        {
            HoldList();
        }

        private void HoldList()
        {
            for (var i = 0; i < unitScrollPosition.Count; i++)
            {
                if (i < characterList.Count)
                {
                    var character = characterList[i];
                    var characterBase = character.GetComponent<CharacterBase>();
                    characterBase.Initialize();

                    var unitProperties = characterBase.UnitProperty;
                    unitScrollPosition[i].unitBackGround.GetComponent<Image>().color = unitProperties switch
                    {
                        CharacterBase.UnitProperties.Divine => new Color(0.9725f, 1f, 0f, 1f),
                        CharacterBase.UnitProperties.Darkness => new Color(0.2402f, 0f, 1f, 1f),
                        CharacterBase.UnitProperties.Fire => new Color(1f, 0.0319f, 0f, 1f),
                        CharacterBase.UnitProperties.Physics => new Color(0.4433f, 0.4433f, 0.4433f, 1f),
                        CharacterBase.UnitProperties.Poison => new Color(0f, 1f, 0.2585f, 1f),
                        CharacterBase.UnitProperties.Water => new Color(0f, 0.6099f, 1f, 1f),
                        CharacterBase.UnitProperties.None => new Color(1f, 1f, 1f, 1f),
                        _ => throw new ArgumentOutOfRangeException(nameof(unitProperties))
                    };
                    var unitColor = unitScrollPosition[i].unitBackGround.GetComponent<Image>().color;

                    var spriteForLevel = character.GetSpriteForLevel(character.CharacterObjectLevel);
                    var unitLevel = characterBase.CharacterObjectLevel;
                    unitScrollPosition[i].unit.GetComponent<Image>().sprite = spriteForLevel;
                    var unitSprite = unitScrollPosition[i].unit.GetComponent<Image>().sprite;
                    
                    unitScrollPosition[i].unitLevelSlider.maxValue = 30;
                    unitScrollPosition[i].unitLevelSlider.value = unitLevel;
                    unitScrollPosition[i].unitLevelText.text = $"{unitScrollPosition[i].unitLevelSlider.value}/30";
                    var levelMaxValue = (int)unitScrollPosition[i].unitLevelSlider.maxValue;
                    var levelValue = (int)unitScrollPosition[i].unitLevelSlider.value;

                    
                    var requiredMax = (unitLevel * 5);
                    var pieceCount = characterBase.CharacterPieceCount;
                    unitScrollPosition[i].unitPieceSlider.maxValue = requiredMax;
                    unitScrollPosition[i].unitPieceSlider.value = pieceCount;
                    unitScrollPosition[i].unitPieceText.text = $"{unitScrollPosition[i].unitPieceSlider.value}/{unitScrollPosition[i].unitPieceSlider.maxValue}";
                    var pieceMaxValue = (int)unitScrollPosition[i].unitPieceSlider.maxValue;
                    var pieceValue = (int)unitScrollPosition[i].unitPieceSlider.value;
                    CharacterInformation(characterBase, unitColor, unitSprite, levelMaxValue, levelValue, pieceMaxValue,
                        pieceValue);
                }
                else
                {
                    unitScrollPosition[i].unitBackGround.color = new Color(0.6415f, 0.6415f, 0.6415f, 1f);
                    unitScrollPosition[i].unit.GetComponent<Image>().sprite = defaultLocker;
                    unitScrollPosition[i].unitLevelSlider.maxValue = 0;
                    unitScrollPosition[i].unitLevelSlider.value = 0;
                    unitScrollPosition[i].unitLevelText.text = "0/0";
                    unitScrollPosition[i].unitPieceSlider.maxValue = 0;
                    unitScrollPosition[i].unitPieceSlider.value = 0;
                    unitScrollPosition[i].unitPieceText.text = "0/0";
                }
            }
        }

        public void CharacterInformation(CharacterBase characterBase, Color unitColor, Sprite unitSprite,
            int levelMaxValue, int levelValue, int pieceMaxValue, int pieceValue)
        {
            unitInformation = new UnitInformation
            {
                unit = characterBase,
                unitBackColor = unitColor,
                unitSprite = unitSprite,
                unitLevelMaxValue = levelMaxValue,
                unitLevelValue = levelValue,
                unitPieceMaxvalue = pieceMaxValue,
                unitPieceValue = pieceValue
            };
        }
    }
}