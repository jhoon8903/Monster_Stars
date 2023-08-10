using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script.UIManager
{
    public class PauseSkillObjectScript : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] internal Image skillBackGround;
        [SerializeField] internal Image skillIcon;
        [SerializeField] internal GameObject descPanel;
        [SerializeField] internal TextMeshProUGUI desc;
        [SerializeField] internal TextMeshProUGUI value;
        private static GameObject _activeSkillInfoPanel;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_activeSkillInfoPanel != null && _activeSkillInfoPanel != descPanel)
            {
                _activeSkillInfoPanel.SetActive(false);
            }
            var isChildOfThisObject = eventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(transform);
            if (!isChildOfThisObject) return;
            descPanel.SetActive(!descPanel.activeSelf);
            _activeSkillInfoPanel = descPanel.activeSelf ? descPanel : null;
        }
    }
}
