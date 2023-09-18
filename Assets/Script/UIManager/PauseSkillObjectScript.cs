using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script.UIManager
{
    public class PauseSkillObjectScript : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] internal Image skillIcon;
        [SerializeField] internal GameObject descPanelLeft;
        [SerializeField] internal TextMeshProUGUI leftDesc;
        [SerializeField] internal GameObject descPanelCenter;
        [SerializeField] internal TextMeshProUGUI centerDesc;
        [SerializeField] internal GameObject descPanelRight;
        [SerializeField] internal TextMeshProUGUI rightDesc;
        [SerializeField] internal TextMeshProUGUI value;
        public int instanceCount;
        public GameObject activeSkillInfoPanel;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (activeSkillInfoPanel != null &&
                activeSkillInfoPanel != descPanelLeft&& 
                activeSkillInfoPanel != descPanelCenter && 
                activeSkillInfoPanel != descPanelRight)
            {
                activeSkillInfoPanel.SetActive(false);
            }

            var isChildOfThisObject = eventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(transform);
            if (!isChildOfThisObject) return;

            if (new[] { 1, 2, 7, 8, 13, 14, 19, 20 }.Contains(instanceCount))
            {
                TogglePanel(descPanelLeft);
            }
            else if (new[] { 3, 4, 9, 10, 15, 16, 21, 22 }.Contains(instanceCount))
            {
                TogglePanel(descPanelCenter);
            }
            else if (new[] { 5, 6, 11, 12, 17, 18, 23, 24 }.Contains(instanceCount))
            {
                TogglePanel(descPanelRight);
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
