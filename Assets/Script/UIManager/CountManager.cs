using TMPro;
using UnityEngine;

namespace Script.UIManager
{
    public class CountManager : MonoBehaviour
    {
        private int _comboCount;
        protected internal int BaseMoveCount;
        protected internal int RewardMoveCount;
        protected internal int TotalMoveCount;
        public bool IsSwapOccurred { get; set; } = false;
        public TextMeshProUGUI moveCountText;

        public void Initialize(int initialMoveCount)
        {
            BaseMoveCount = initialMoveCount;
            RewardMoveCount = 0;
            TotalMoveCount = BaseMoveCount;
            _comboCount = 0;
            UpdateMoveCountText();
        }

        protected internal void UpdateMoveCountText()
        {
            moveCountText.text = $"{TotalMoveCount}";
        }

        public bool CanMove()
        {
            return TotalMoveCount > 0;
        }
        
        public void DecreaseMoveCount()
        {
            if (BaseMoveCount <= 0) return;
            BaseMoveCount--;
            TotalMoveCount = BaseMoveCount;
            UpdateMoveCountText();
        }

        private void IncreaseMoveCount(int comboCount)
        {
            BaseMoveCount += comboCount;
            TotalMoveCount = BaseMoveCount;
            UpdateMoveCountText();
        }

        public void IncrementComboCount()
        {
            _comboCount++;
            IncreaseMoveCount(_comboCount);
            _comboCount = 0;
        }

        public void IncreaseRewardMoveCount(int increaseAmount)
        {
            BaseMoveCount += increaseAmount;
            TotalMoveCount = BaseMoveCount;
            UpdateMoveCountText();
        }
    }
}
