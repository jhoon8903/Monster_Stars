using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Script.EnemyManagerScript;
using Script.RewardScript;

namespace Script.UIManager
{
    public class ExpManager : MonoBehaviour
    {
        [SerializeField] private float expPoint = 0;
        [SerializeField] private int levelUpPoint = 5;
        [SerializeField] private Slider expBar;
        [SerializeField] private TextMeshProUGUI expText;
        [SerializeField] private LevelUpRewardManager levelUpRewardManager;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private EnforceManager enforceManager;
        public int level = 0;
        public static ExpManager Instance { get; private set; }

        private void Start()
        {
            expBar.maxValue = levelUpPoint;
            expBar.value = expPoint;
            UpdateExpText();
            UpdateLevelText(level);
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public IEnumerator HandleEnemyKilled(EnemyBase.KillReasons reason)
        {
            if (reason != EnemyBase.KillReasons.ByPlayer) yield break;
            var additionalExp = expPoint * (enforceManager.expPercentage / 100.0f);
            expPoint += 1 + additionalExp; 
            if (expPoint >= levelUpPoint)
            {
                level++;
                UpdateLevelText(level);
                expPoint = 0;
                if (level <= 14)
                {
                    levelUpPoint += 5;
                    expBar.maxValue = levelUpPoint;
                }
                yield return StartCoroutine(levelUpRewardManager.LevelUpReward());
            }
            expBar.value = expPoint;
            expBar.DOValue(expPoint, 0.5f);
            UpdateExpText();
        }

        private void UpdateExpText()
        {
            expText.text = $"{expPoint} / {levelUpPoint}";
        }

        private void UpdateLevelText(int text)
        {
            levelText.text = $"LV {text}";
        }
    }
}