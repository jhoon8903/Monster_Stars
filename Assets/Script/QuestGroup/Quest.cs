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
        public List<QuestManager.QuestTypes> questList = new List<QuestManager.QuestTypes>();
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
        public void AllClearQuest()
        {
            StartCoroutine(questManager.SpecialQuestUpdate(QuestManager.QuestTypes.AllClear, 1));
        }
        // Use Coin Quest (Fix)
        public void UseCoinQuest(int useCoin)
        {
            var fixQuest = questManager.FixQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.UseCoin);
            StartCoroutine(questManager.UpdateQuest(fixQuest, useCoin));
        }
        // Get Coin Quest (Fix)
        public void GetCoinQuest(int getCoin)
        {
            var getCoinQuest = questManager.FixQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.GetCoin);
            StartCoroutine(questManager.UpdateQuest(getCoinQuest, getCoin));
        }
        // Kill Enemy Quest (Rotation)
        public void KillEnemyQuest()
        {
            var killEnemyQuest = questManager.RotationQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.KillEnemy);
            StartCoroutine(questManager.UpdateQuest(killEnemyQuest, 1));
        }
        // Open Box (Store) Quest (Rotation)
        public void OpenBoxQuest()
        {
            var openBoxQuest = questManager.RotationQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.OpenBox);
            StartCoroutine(questManager.UpdateQuest(openBoxQuest, 1));
        }
        // Kill Boss Quest (Rotation)
        public void KillBossQuest()
        {
            var killBossQuest = questManager.RotationQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.KillBoss);
            StartCoroutine(questManager.UpdateQuest(killBossQuest, 1));
        }
        // Match Coin Quest (Rotation)
        public void MergeBoxQuest()
        {
            var mergeBoxQuest = questManager.RotationQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.MergeBox);
            StartCoroutine(questManager.UpdateQuest(mergeBoxQuest, 1));
        }
        // Victory Quest (Rotation)
        public void VictoryQuest()
        {
            var victoryQuest = questManager.RotationQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.Victory);
            StartCoroutine(questManager.UpdateQuest(victoryQuest, 1));
        }
        // Get Piece Quest (Rotation)
        public void GetPieceQuest(int getPiece)
        {
            var getPieceQuest = questManager.RotationQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.GetPiece);
            StartCoroutine(questManager.UpdateQuest(getPieceQuest, getPiece));
        }
    }
}