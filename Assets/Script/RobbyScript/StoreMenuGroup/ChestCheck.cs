using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.StoreMenuGroup
{
    public class ChestCheck : MonoBehaviour
    {
        [SerializeField] private GameObject chestCheckCloseBtn;
      
        public GameObject chestCheckPanel;
        public TextMeshProUGUI chestCheckTopText;
        public TextMeshProUGUI chestCheckBtnText;
        public GameObject chestImage;
        public GameObject chestCheckBtn;
        public static ChestCheck Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            chestCheckCloseBtn.GetComponent<Button>().onClick.AddListener(CloseChestCheck);
        }
        
        public void OpenPanel()
        {
            chestCheckPanel.SetActive(true);
        }

        private void CloseChestCheck()
        {
            // ChestReward.Instance.ClearChests();
            chestCheckPanel.SetActive(false);
            StoreMenu.Instance.DeleteEvent();
        }
    }
}