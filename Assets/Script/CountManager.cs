using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    public class CountManager : MonoBehaviour
    {
        private int _moveCount;
        public TextMeshProUGUI moveCountText;

        // 카운트 초기화
        public void Initialize(int initialMoveCount)
        {
            _moveCount = initialMoveCount;
            UpdateMoveCountText();
        }

        // 이동 가능한 횟수 확인
        public bool CanMove()
        {
            return _moveCount > 0;
        }

        // 이동시 이동 Count -1
        public void DecreaseMoveCount()
        {
            if (_moveCount <= 0) return;
            _moveCount--;
            UpdateMoveCountText();
        }

        public void IncreaseMoveCount()
        {
            _moveCount++;
            UpdateMoveCountText();
        }
        
        private void UpdateMoveCountText()
        {
            moveCountText.text = $"Count: {_moveCount}";
        }
    }
}
