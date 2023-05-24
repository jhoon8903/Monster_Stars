using TMPro;
using UnityEngine;

namespace Script
{
    public class CountManager : MonoBehaviour
    {
        public int _moveCount;
        private int _comboCount;
        public bool IsSwapOccurred { get; set; } = false;
        public TextMeshProUGUI moveCountText;
        [SerializeField] private GameManager gameManager;



        /** 
         * Game MoveCount Initialize
         * Called GameManager as public "moveCount"
         * And This Methods is called UpdateMoveCountText();
         */
        public void Initialize(int initialMoveCount)
        {
            _moveCount = initialMoveCount;
            _comboCount = 0;
            UpdateMoveCountText();
        }
        
        /**
         *  UpdateMoveCountText() is Update Count Text UI
         */
        private void UpdateMoveCountText()
        {
            moveCountText.text = $"{_moveCount}";
            if (_moveCount <= 0)
            {
                gameManager.Count0Call();
            }
        }
        
        /**
         * CanMove is Can Use MoveCount Check
         * if Count under 'int 0' Blocked Swipe
         */
        public bool CanMove()
        {
            return _moveCount > 0;
        }
        
        /**
         * DecreaseMoveCount()
         * This Method is if you move or Swipe
         * Decrease Count And Update Text UI
         */
        public void DecreaseMoveCount()
        {
            if (_moveCount <= 0) return;
            _moveCount--;
            _comboCount = 0;
            UpdateMoveCountText();
        }

        /**
         * IncreaseMoveCount()
         * If will Checking 'Combo' and Increase Count
         */
        private void IncreaseMoveCount(int _comboCount)
        {
            _moveCount += _comboCount;
            Debug.Log("카운트 업!");
            UpdateMoveCountText();
        }

        public void IncrementComboCount()
        {
            _comboCount++;
            IncreaseMoveCount(_comboCount);
        }
    }
}
