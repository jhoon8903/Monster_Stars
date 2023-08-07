using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.StoreMenuGroup
{
    public class ChestReward : MonoBehaviour
    {
        [SerializeField] private GameObject chestRewardList;
        [SerializeField] private Transform chestCheckContents;

        public GameObject chestReward;
        public TextMeshProUGUI chestRewardText;

        public static ChestReward Instance { get; private set; }

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
        }

        public void Start()
        {
            // 4개의 ChestRewardList 프리팹을 생성하여 각각의 이미지와 텍스트를 변경
            for (int i = 0; i < 4; i++)
            {
                GameObject newChestRewardList = Instantiate(chestRewardList, chestCheckContents.position, Quaternion.identity);
                newChestRewardList.transform.SetParent(chestCheckContents);
            }
        }
    }
}

