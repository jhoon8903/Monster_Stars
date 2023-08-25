using System.Linq;
using UnityEngine;

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
            var adsViewQuest = questManager.FixQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.ViewAds);
            // questManager.UpdateQuest(adsViewQuest, 1);
        }
        // All Clear Quest (Fix)
        public void AllClearQuest()
        {
            var allClearQuest = questManager.FixQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.AllClear);
            // questManager.UpdateQuest(allClearQuest,1);
        }
        // Use Coin Quest (Fix)
        public void UseCoinQuest(int useCoin)
        {
            var fixQuest = questManager.FixQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.UseCoin);
            // questManager.UpdateQuest(fixQuest, useCoin);
        }
        // Get Coin Quest (Fix)
        public void GetCoinQuest(int getCoin)
        {
            var getCoinQuest = questManager.FixQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.GetCoin);
            // questManager.UpdateQuest(getCoinQuest, getCoin);
        }
        // Kill Enemy Quest (Rotation)
        public void KillEnemyQuest()
        {
            var killEnemyQuest = questManager.RotationQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.KillEnemy);
            // questManager.UpdateQuest(killEnemyQuest, 1);
        }
        // Open Box (Store) Quest (Rotation)
        public void OpenBoxQuest()
        {
            var openBoxQuest = questManager.RotationQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.OpenBox);
            // questManager.UpdateQuest(openBoxQuest, 1);
        }
        // Kill Boss Quest (Rotation)
        public void KillBossQuest()
        {
            var killBossQuest = questManager.RotationQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.KillBoss);
            // questManager.UpdateQuest(killBossQuest, 1);
        }
        // Match Coin Quest (Rotation)
        public void MatchCoinQuest()
        {
            var matchCoinQuest = questManager.RotationQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.MatchCoin);
            // questManager.UpdateQuest(matchCoinQuest, 1);
        }
        // Victory Quest (Rotation)
        public void VictoryQuest()
        {
            var victoryQuest = questManager.RotationQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.Victory);
            // questManager.UpdateQuest(victoryQuest, 1);
        }
        // Get Piece Quest (Rotation)
        public void GetPieceQuest(int getPiece)
        {
            var getPieceQuest = questManager.RotationQuestList.FirstOrDefault(q => q.QuestType == QuestManager.QuestTypes.GetPiece);
            // questManager.UpdateQuest(getPieceQuest, getPiece);
        }


    }
}