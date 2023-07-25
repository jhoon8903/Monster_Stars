using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.UIManager
{
    public class UnitSkillObject : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] public TextMeshProUGUI skillType;
        [SerializeField] private GameObject skillInfoPanel;
        [SerializeField] public TextMeshProUGUI skillDesc;

        private static GameObject _activeSkillInfoPanel;

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Clicked on: " + eventData.pointerCurrentRaycast.gameObject.name);
            if (_activeSkillInfoPanel != null && _activeSkillInfoPanel != skillInfoPanel)
            {
                _activeSkillInfoPanel.SetActive(false);
            }
            var isChildOfThisObject = eventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(transform);
            if (!isChildOfThisObject) return;
            skillInfoPanel.SetActive(!skillInfoPanel.activeSelf);
            _activeSkillInfoPanel = skillInfoPanel.activeSelf ? skillInfoPanel : null;
        }
    }
}
