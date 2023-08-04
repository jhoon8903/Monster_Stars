using System.Collections.Generic;
using Script.CharacterManagerScript;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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



        public List<Sprite> NormalBackSprite;
        public List<Sprite> InfoBackSprite;
        public List<Sprite> FrameSprite;
        public List<Sprite> UnitPropertiesSprite;
    }
}