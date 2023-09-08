using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.QuestGroup
{
    public class Quest : MonoBehaviour
    {
        [SerializeField] private QuestManager questManager;
        public static Quest Instance;
        private void Awake()
        {
            Instance = this;
        }

        // View Ads Quest (Fix)
        public void AdsViewQuest()
        {                             
            if (SceneManager.GetActiveScene().name != "SelectScene") return;
            StartCoroutine(questManager.SpecialQuestUpdate(QuestManager.QuestTypes.ViewAds, 1));
        }
        // All Clear Quest (Fix)
        public void AllClearQuest(QuestManager.QuestTypes questType)
        {
            if (questType == QuestManager.QuestTypes.AllClear) return;
            StartCoroutine(questManager.SpecialQuestUpdate(QuestManager.QuestTypes.AllClear, 1));
        }
        // Use Coin Quest (Fix)
        // Open Box (Store) Quest (Rotation)
        // Get Coin Quest (Fix)
        // Victory Quest (Rotation)
        // Get Piece Quest (Rotation)
        // Kill Boss Quest (Rotation)
        // Kill Enemy Quest (Rotation)
        // Match Coin Quest (Rotation)
    }
}