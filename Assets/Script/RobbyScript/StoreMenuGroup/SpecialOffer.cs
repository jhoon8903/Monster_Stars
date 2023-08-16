using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.StoreMenuGroup
{
    public class SpecialOffer : MonoBehaviour
    {
       public GameObject coinAdsBtn;
       public GameObject staminaAdsBtn;
       public GameObject gemAdsBtn;
       public Sprite coinSprite;
       public Sprite staminaSprite;
       public Sprite gemSprite;

       public void SpecialBtnSet()
       {
           coinAdsBtn.GetComponent<Button>().onClick.AddListener(StoreMenu.Instance.CoinAds);
           staminaAdsBtn.GetComponent<Button>().onClick.AddListener(StoreMenu.Instance.StaminaAds);
           gemAdsBtn.GetComponent<Button>().onClick.AddListener(StoreMenu.Instance.GemAds);
       }

       public void SpecialBtnRemove()
       {
           coinAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
           gemAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
           staminaAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
       }
    }
}

