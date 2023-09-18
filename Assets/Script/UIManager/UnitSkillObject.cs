using System.Collections.Generic;
using Script.RewardScript;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Script.UIManager
{
    public class UnitSkillObject : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] public List<Sprite> background;
        [SerializeField] public Image skillBackground;
        [SerializeField] public Image skillIcon; 
        [SerializeField] public TextMeshProUGUI skillType;
        [SerializeField] private GameObject descLeft;
        [SerializeField] private GameObject descCenter;
        [SerializeField] private GameObject descRight;
        [SerializeField] public TextMeshProUGUI leftDesc;
        [SerializeField] public TextMeshProUGUI centerDesc;
        [SerializeField] public TextMeshProUGUI rightDesc;
        public GameObject activeSkillInfoPanel;
        public int skillLevel;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (activeSkillInfoPanel != null &&
                activeSkillInfoPanel != descLeft && 
                activeSkillInfoPanel != descCenter && 
                activeSkillInfoPanel != descRight)
            {
                activeSkillInfoPanel.SetActive(false);
            }

            var isChildOfThisObject = eventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(transform);
            if (!isChildOfThisObject) return;
            switch (skillLevel)
            {
                case <= 3:
                case >= 11:
                    TogglePanel(descRight);
                    break;
                case 5:
                    TogglePanel(descCenter);
                    break;
                default:
                    TogglePanel(descLeft);
                    break;
            }
        }

        private void TogglePanel(GameObject panel)
        {
            panel.SetActive(!panel.activeSelf);
            activeSkillInfoPanel = panel.activeSelf ? panel : null;
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0) || activeSkillInfoPanel == null ||
                !activeSkillInfoPanel.activeSelf) return;
            var clickedObject = EventSystem.current.currentSelectedGameObject;

            if (clickedObject == null || !IsDescendantOrSelf(activeSkillInfoPanel, clickedObject))
            {
                activeSkillInfoPanel.SetActive(false);
            }
        }

        private static bool IsDescendantOrSelf(GameObject parent, GameObject toCheck)
        {
            if (parent == null || toCheck == null) return false;

            var current = toCheck.transform;
            while (current != null)
            {
                if (current == parent.transform) return true;
                current = current.parent;
            }
            return false;
        }
    }
}
