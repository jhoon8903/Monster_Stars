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
        [SerializeField] private Button blankBtn1;
        [SerializeField] private Button blankBtn2;

        private void OnEnable()
        {
            mainBtn.onClick.AddListener(()=>
            {
                mainPanel.SetActive(true);
                unitPanel.SetActive(false);
                storePanel.SetActive(false);
            });
            unitSelectBtn.onClick.AddListener(()=>
            {
                mainPanel.SetActive(false);
                unitPanel.SetActive(true);
                storePanel.SetActive(false);
            });
            storeBtn.onClick.AddListener(() =>
            {
                mainPanel.SetActive(false);
                unitPanel.SetActive(false);
                storePanel.SetActive(true);
            });
        }
    }
}
