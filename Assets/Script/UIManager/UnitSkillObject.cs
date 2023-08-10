using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script.UIManager
{
    public class UnitSkillObject : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] public Image skillIcon; 
        [SerializeField] public TextMeshProUGUI skillType;
        [SerializeField] private GameObject skillInfoPanel;
        [SerializeField] public TextMeshProUGUI skillDesc;
        private static GameObject _activeSkillInfoPanel;
        public void OnPointerClick(PointerEventData eventData)
        {
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
