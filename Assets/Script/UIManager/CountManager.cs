using Script.RewardScript;
using TMPro;
using UnityEngine;

namespace Script.UIManager
{
    public class CountManager : MonoBehaviour
    {
        private int _comboCount;
        private int _baseMoveCount;
        private int _rewardMoveCount;
        private int _stepRewardCount;
        protected internal int TotalMoveCount;
        public bool IsSwapOccurred { get; set; } = false;
        public TextMeshProUGUI moveCountText;

        public void Initialize(int initialMoveCount)
        {
            _baseMoveCount = initialMoveCount;
            _rewardMoveCount = EnforceManager.Instance.permanentIncreaseMovementCount;
            TotalMoveCount = _baseMoveCount + _rewardMoveCount;
            _comboCount = 0;
            _stepRewardCount = 0;
            UpdateMoveCountText();
        }

        private void UpdateMoveCountText()
        {
            moveCountText.text = $"{TotalMoveCount}";
        }

        public bool CanMove()
        {
            return TotalMoveCount > 0;
        }
        
        public void DecreaseMoveCount()
        {
            if (_baseMoveCount <= 0) return;
            TotalMoveCount--;
            UpdateMoveCountText();
        }

        public void IncreaseMoveCount(int comboCount)
        {
            _stepRewardCount += comboCount;
            TotalMoveCount += _stepRewardCount;
            UpdateMoveCountText();
            _stepRewardCount = 0;
        }

        public void IncrementComboCount()
        {
            _comboCount++;
            IncreaseMoveCount(_comboCount);
            _comboCount = 0;
        }
    }
}
