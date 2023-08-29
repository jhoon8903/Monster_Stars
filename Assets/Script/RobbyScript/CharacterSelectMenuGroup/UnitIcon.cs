using System.Collections.Generic;
using Script.CharacterManagerScript;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script.RobbyScript.CharacterSelectMenuGroup
{

    public class UnitIcon : MonoBehaviour, IPointerClickHandler
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

        private void Start() 
        {
            var trigger = gameObject.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entry.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });
            trigger.triggers.Add(entry);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (SelectedUnitHolder.Instance.selectedUnit.Count < 4) return;
            if (HoldCharacterList.Instance.selectedToSwap == null)
            {
                return; 
            }
            HoldCharacterList.Instance.secondSwap = this;
            HoldCharacterList.Instance.SwapUnitInstances(
                HoldCharacterList.Instance.selectedToSwap,
                HoldCharacterList.Instance.selectedToSwap.CharacterBase,
                HoldCharacterList.Instance.secondSwap,
                HoldCharacterList.Instance.secondSwap.CharacterBase
            );
            Debug.Log("Second: " + HoldCharacterList.Instance.selectedToSwap);
            HoldCharacterList.Instance.selectedToSwap = null;
            HoldCharacterList.Instance.secondSwap = null;
        }
    }
}