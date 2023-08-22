using System.Collections;
using System.Collections.Generic;
using Script.AdsScript;
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
       public SpecialOffer specialOffer;

       public void SpecialBtnSet()
       {
           coinAdsBtn.GetComponent<Button>().onClick.AddListener(CoinAds);
           staminaAdsBtn.GetComponent<Button>().onClick.AddListener(StaminaAds);
           gemAdsBtn.GetComponent<Button>().onClick.AddListener(GemAds);
       }

       public void SpecialBtnRemove()
       {
           coinAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
           gemAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
           staminaAdsBtn.GetComponent<Button>().onClick.RemoveAllListeners();
       }

       public void InstanceSpecialOffer()
       {
           specialOffer = Instantiate(this, StoreMenu.Instance.specialOfferLayer);
           specialOffer.transform.SetSiblingIndex(specialOffer.transform.GetSiblingIndex() + 1);
           specialOffer.SpecialBtnSet();
       }

       private static void CoinAds()
       {
           AdsManager.Instance.ShowRewardedAd();
           AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Coin;
       }
       private static void StaminaAds()
       {
           AdsManager.Instance.ShowRewardedAd();
           AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Stamina;
       }
       public static void GemAds()
       {
           StoreMenu.Instance.ErrorClose();
           AdsManager.Instance.ShowRewardedAd();
           AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.Gem;
       }

       public Sprite RewardSprite(StoreMenu.BoxGrade grade)
       {
           var rewardSprite = coinSprite;
           switch (grade)
           {
               case StoreMenu.BoxGrade.Coin :
                    rewardSprite = coinSprite;
                    break;
               case StoreMenu.BoxGrade.Stamina :
                   rewardSprite = staminaSprite;
                   break;
               case StoreMenu.BoxGrade.Gem:
                   rewardSprite = gemSprite;
                   break;
           }
           return rewardSprite;
       }
    }
}

