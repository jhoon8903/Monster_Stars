using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.UIManager
{
    public class CountManager : MonoBehaviour
    {
        private int _comboCount;
        public int baseMoveCount;
        private int _rewardMoveCount;
        private int _totalMoveCount;
        public bool IsSwapOccurred { get; set; } = false;
        public TextMeshProUGUI moveCountText;
        [SerializeField] private GameManager gameManager;

        /** 
         * Game MoveCount Initialize
         * Called GameManager as public "baseMoveCount"
         * And This Methods is called UpdateMoveCountText();
         */
        public void Initialize(int initialMoveCount)
        {
            baseMoveCount = initialMoveCount;
            _rewardMoveCount = 0;
            _totalMoveCount = baseMoveCount;
            _comboCount = 0;
            UpdateMoveCountText();
        }
        
        /**
         *  UpdateMoveCountText() is Update Count Text UI
         */
        private void UpdateMoveCountText()
        {
            moveCountText.text = $"{baseMoveCount}";
            if (baseMoveCount <= 0)
            {   
                StartCoroutine(gameManager.Count0Call());
            }
        }
        
        /**
         * CanMove is Can Use MoveCount Check
         * if Count under 'int 0' Blocked Swipe
         */
        public bool CanMove()
        {
            return baseMoveCount > 0;
        }
        
        /**
         * DecreaseMoveCount()
         * This Method is if you move or Swipe
         * Decrease Count And Update Text UI
         */
        public void DecreaseMoveCount()
        {
            if (baseMoveCount <= 0) return;
            baseMoveCount--;
            _comboCount = 0;
            UpdateMoveCountText();
        }

        /**
         * IncreaseMoveCount()
         * If will Checking 'Combo' and PowerUp_Property Count
         */
        private void IncreaseMoveCount(int comboCount)
        {
            baseMoveCount += comboCount;
            UpdateMoveCountText();
        }

        public void IncrementComboCount()
        {
            _comboCount++;
            IncreaseMoveCount(_comboCount);
        }

        public void IncreaseRewardMoveCount(int increaseAmount)
        {
            baseMoveCount += increaseAmount;
            UpdateMoveCountText();
        }

        public void PermanentIncreaseMoveCount(int increaseAmount)
        {
            _rewardMoveCount += increaseAmount;
            _totalMoveCount = baseMoveCount + _rewardMoveCount;
            UpdateMoveCountText();
        }
    }
}
