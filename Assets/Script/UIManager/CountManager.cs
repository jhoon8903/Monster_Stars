using System;
using Script.PuzzleManagerGroup;
using Script.RewardScript;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Script.UIManager
{
    public class CountManager : MonoBehaviour
    {
        [SerializeField] private SwipeManager swipeManager;
        private int _comboCount;
        private int _baseMoveCount;
        private int _rewardMoveCount;
        protected internal int TotalMoveCount;
        public bool IsSwapOccurred { get; set; }
        public TextMeshProUGUI moveCountText;

        public void Initialize(int initialMoveCount)
        {
            _baseMoveCount = initialMoveCount;
            _rewardMoveCount = EnforceManager.Instance.permanentIncreaseMovementCount;
            TotalMoveCount = _baseMoveCount + _rewardMoveCount;
            _comboCount = 0;
            UpdateMoveCountText();
            EnforceManager.Instance.rewardMoveCount = 0;
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
            TotalMoveCount += comboCount;
            UpdateMoveCountText();
        }

        public void IncrementComboCount()
        {
            var tutorial = bool.Parse(PlayerPrefs.GetString("TutorialKey", "true"));
            if (tutorial) return;
            _comboCount++;
            IncreaseMoveCount(_comboCount);
            _comboCount = 0;
        }
    }
}
