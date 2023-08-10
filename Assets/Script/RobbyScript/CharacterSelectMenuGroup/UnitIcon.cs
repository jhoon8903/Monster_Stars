using System.Collections.Generic;
using Script.CharacterManagerScript;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.CharacterSelectMenuGroup
{

    public class UnitIcon : MonoBehaviour
    {
        public CharacterBase CharacterBase { get; set; }
        public Canvas unitCanvas;
        public Button unitBtn;
        public GameObject normalBack;
        public Slider unitPieceSlider;
        public TextMeshProUGUI unitPieceText;
        public GameObject infoBack;
        public Button infoBtn;
        public Button levelUpBtn;
        public Button removeBtn;
        public Button useBtn;
        public GameObject unitFrame;
        public GameObject unitProperty;
        public GameObject unitImage;
        public TextMeshProUGUI unitLevelText; 
        public List<Sprite> normalBackSprite;
        public List<Sprite> infoBackSprite;
        public List<Sprite> frameSprite;
        public List<Sprite> unitPropertiesSprite;
    }
}