using System.Collections.Generic;
using Script.CharacterManagerScript;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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


        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("OnPointerClick called");
            if (SelectedUnitHolder.Instance.selectedUnit.Count >= 4)
            {
                if (HoldCharacterList.Instance.selectedToSwap == null)
                {
                    HoldCharacterList.Instance.selectedToSwap = this;
                }
                else
                {
                    HoldCharacterList.Instance.secondSwap = this;
                    HoldCharacterList.Instance.SwapUnitInstances(
                        HoldCharacterList.Instance.selectedToSwap,
                        HoldCharacterList.Instance.selectedToSwap.CharacterBase,
                        HoldCharacterList.Instance.secondSwap,
                        HoldCharacterList.Instance.secondSwap.CharacterBase
                    );
                    HoldCharacterList.Instance.selectedToSwap = null;
                    HoldCharacterList.Instance.secondSwap = null;
                }
            }
        }
    }
}