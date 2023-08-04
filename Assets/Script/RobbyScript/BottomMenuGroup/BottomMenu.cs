using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Script.RobbyScript.BottomMenuGroup
{
    public class BottomMenu : MonoBehaviour
    {
        [SerializeField] private GameObject battlePanel;
        [SerializeField] private GameObject battleUnClick;
        [SerializeField] private GameObject battleUnClickNew;
        [SerializeField] private GameObject battleClick;
        [SerializeField] private GameObject battleClickNew;
        [SerializeField] private GameObject cardPanel;
        [SerializeField] private GameObject cardUnClick;
        [SerializeField] private GameObject cardUnClickNew;
        [SerializeField] private GameObject cardClick;
        [SerializeField] private GameObject cardClickNew;
        [SerializeField] private GameObject storePanel;
        [SerializeField] private GameObject storeUnClick;
        [SerializeField] private GameObject storeUnClickNew;
        [SerializeField] private GameObject storeClick;
        [SerializeField] private GameObject storeClickNew;

        private void Awake()
        {
            battlePanel.SetActive(true);
            battleClick.SetActive(true);
            battleUnClick.SetActive(false);
            storeUnClick.SetActive(true);
            cardUnClick.SetActive(true);

            battleUnClick.GetComponent<Button>().onClick.AddListener(BattleClick);
            storeUnClick.GetComponent<Button>().onClick.AddListener(StoreClick);
            cardUnClick.GetComponent<Button>().onClick.AddListener(CardClick);
        }

        private void BattleClick()
        {
            
            battlePanel.SetActive(true);
            battleClick.SetActive(true);
            battleUnClick.SetActive(false);
            
            storePanel.SetActive(false);
            storeClick.SetActive(false);
            storeUnClick.SetActive(true);
            
            cardPanel.SetActive(false);
            cardClick.SetActive(false);
            cardUnClick.SetActive(true);
        }

        private void StoreClick()
        {
            storePanel.SetActive(true);
            storeClick.SetActive(true);
            storeUnClick.SetActive(false);
            
            battlePanel.SetActive(false);
            battleClick.SetActive(false);
            battleUnClick.SetActive(true);
            
            cardPanel.SetActive(false);
            cardClick.SetActive(false);
            cardUnClick.SetActive(true);
        }

        private void CardClick()
        {
            cardPanel.SetActive(true);
            cardClick.SetActive(true);
            cardUnClick.SetActive(false);
            
            battlePanel.SetActive(false);
            battleClick.SetActive(false);
            battleUnClick.SetActive(true);
            
            storePanel.SetActive(false);
            storeClick.SetActive(false);
            storeUnClick.SetActive(true);
        }

    }
}
