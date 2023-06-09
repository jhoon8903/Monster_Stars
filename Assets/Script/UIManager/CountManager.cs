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


        public void Initialize(int initialMoveCount)
        {
            baseMoveCount = initialMoveCount;
            _rewardMoveCount = 0;
            _totalMoveCount = baseMoveCount;
            _comboCount = 0;
            UpdateMoveCountText();
        }

        private void UpdateMoveCountText()
        {
            moveCountText.text = $"{baseMoveCount}";
            if (baseMoveCount <= 0)
            {   
                StartCoroutine(gameManager.Count0Call());
            }
        }

        public bool CanMove()
        {
            return baseMoveCount > 0;
        }
        

        public void DecreaseMoveCount()
        {
            if (baseMoveCount <= 0) return;
            baseMoveCount--;
            _comboCount = 0;
            UpdateMoveCountText();
        }

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
