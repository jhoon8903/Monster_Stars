using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.StoreMenuGroup
{
    public class ChestCheck : MonoBehaviour
    {
        [SerializeField] private GameObject chestCheckPanel;
        [SerializeField] private GameObject chestCheckCloseBtn;
        [SerializeField] private GameObject chestCheckContents;
        
        // [SerializeField] private Goods rewardItem;
        // [SerializeField] private List<CharacterBase> unitList = new List<CharacterBase>();
        
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

        public void CloseChestCheck()
        {
            chestCheckPanel.SetActive(false);
        }
    }
}