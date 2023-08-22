using UnityEngine;

namespace Script.RobbyScript.StoreMenuGroup
{
    public class DailyOffer : MonoBehaviour
    {

        public void InstanceDailyOffer()
        {
            transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
        }
    }
}

