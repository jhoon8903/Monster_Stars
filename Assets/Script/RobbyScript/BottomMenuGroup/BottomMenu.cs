using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.BottomMenuGroup
{
    public class BottomMenu : MonoBehaviour
    {
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject unitPanel;
        [SerializeField] private GameObject storePanel;
        [SerializeField] private Button mainBtn;
        [SerializeField] private Button unitSelectBtn;
        [SerializeField] private Button storeBtn;
        [SerializeField] private HorizontalLayoutGroup layoutGroup;

        private const float DefaultWidth = 820;
        private const float SelectedWidth = 1300;

        private readonly Color _selectedColor = new Color(0, 1, 0.2594f, 1);
        private readonly Color _defaultColor = new Color(0.6603f, 0.6603f, 0.6603f, 1);

        private void OnEnable()
        {
            mainBtn.onClick.AddListener(()=>
            {
                SetActivePanel(mainPanel);
                SetSelectedButton(mainBtn);
            });
            unitSelectBtn.onClick.AddListener(()=>
            {
                SetActivePanel(unitPanel);
                SetSelectedButton(unitSelectBtn);
            });
            storeBtn.onClick.AddListener(() =>
            {
                SetActivePanel(storePanel);
                SetSelectedButton(storeBtn);
            });
        }

        private void Start()
        {
            SetSelectedButton(mainBtn);
        }

        private void SetActivePanel(GameObject panel)
        {
            mainPanel.SetActive(false);
            unitPanel.SetActive(false);
            storePanel.SetActive(false);
            panel.SetActive(true);
        }

        private void SetSelectedButton(Component selectedButton)
        {
            mainBtn.GetComponent<LayoutElement>().preferredWidth = DefaultWidth;
            unitSelectBtn.GetComponent<LayoutElement>().preferredWidth = DefaultWidth;
            storeBtn.GetComponent<LayoutElement>().preferredWidth = DefaultWidth;
            mainBtn.GetComponent<Image>().color = _defaultColor;
            unitSelectBtn.GetComponent<Image>().color = _defaultColor;
            storeBtn.GetComponent<Image>().color = _defaultColor;
            selectedButton.GetComponent<LayoutElement>().preferredWidth = SelectedWidth;
            selectedButton.GetComponent<Image>().color = _selectedColor;
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }
    }
}
