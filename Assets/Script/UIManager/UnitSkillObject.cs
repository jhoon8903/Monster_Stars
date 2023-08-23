using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private static GameObject _activeSkillInfoPanel;
        public int skillLevel;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_activeSkillInfoPanel != null &&
                _activeSkillInfoPanel != descLeft && 
                _activeSkillInfoPanel != descCenter && 
                _activeSkillInfoPanel != descRight)
            {
                _activeSkillInfoPanel.SetActive(false);
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

        private static void TogglePanel(GameObject panel)
        {
            panel.SetActive(!panel.activeSelf);
            _activeSkillInfoPanel = panel.activeSelf ? panel : null;
        }
    }
}
