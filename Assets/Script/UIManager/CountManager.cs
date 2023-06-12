using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.UIManager
{
    public class CountManager : MonoBehaviour
    {
        private int _comboCount;
        private int _baseMoveCount;
        private int _rewardMoveCount;
        public int totalMoveCount;
        public bool IsSwapOccurred { get; set; } = false;
        public TextMeshProUGUI moveCountText;
        [SerializeField] private GameManager gameManager;


        public void Initialize(int initialMoveCount)
        {
            _baseMoveCount = initialMoveCount;
            _rewardMoveCount = 0;
            totalMoveCount = _baseMoveCount;
            _comboCount = 0;
            UpdateMoveCountText();
        }

        private void UpdateMoveCountText()
        {
            moveCountText.text = $"{totalMoveCount}";
        }

        public bool CanMove()
        {
            return totalMoveCount > 0;
        }
        
        public void DecreaseMoveCount()
        {
            if (_baseMoveCount <= 0) return;
            _baseMoveCount--;
            totalMoveCount = _baseMoveCount;
            UpdateMoveCountText();
        }

        private void IncreaseMoveCount(int comboCount)
        {
            _baseMoveCount += comboCount;
            totalMoveCount = _baseMoveCount;
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
            _baseMoveCount += increaseAmount;
            totalMoveCount = _baseMoveCount;
            UpdateMoveCountText();
        }

        public void PermanentIncreaseMoveCount(int increaseAmount)
        {
            _rewardMoveCount += increaseAmount;
            totalMoveCount = _baseMoveCount + _rewardMoveCount;
            UpdateMoveCountText();
        }
    }
}
