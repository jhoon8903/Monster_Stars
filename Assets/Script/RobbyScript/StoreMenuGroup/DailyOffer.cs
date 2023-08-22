using UnityEngine;

namespace Script.RobbyScript.StoreMenuGroup
{
    public class DailyOffer : MonoBehaviour
    {
        public DailyOffer dailyOffer;

        public void InstanceDailyOffer()
        {
            dailyOffer = Instantiate(this, StoreMenu.Instance.dailyOfferLayer);
            dailyOffer.transform.SetSiblingIndex(dailyOffer.transform.GetSiblingIndex() + 1);
        }
    }
}

