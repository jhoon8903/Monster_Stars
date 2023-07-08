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
        [SerializeField] private float expPoint ;
        [SerializeField] private int levelUpPoint = 5;
        [SerializeField] private Slider expBar;
        [SerializeField] private TextMeshProUGUI expText;
        [SerializeField] private LevelUpRewardManager levelUpRewardManager;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private EnforceManager enforceManager;
        private static readonly object Lock = new object();
        public int level;
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

        public void HandleEnemyKilled(EnemyBase.KillReasons reason)
        {
            lock (Lock)
            {
                if (reason != EnemyBase.KillReasons.ByPlayer) return;
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
                    StartCoroutine(levelUpRewardManager.LevelUpReward());
                }
                expBar.value = expPoint;
                expBar.DOValue(expPoint, 0.5f);
                UpdateExpText();
            }
        }

        private void UpdateExpText()
        {
            expText.text = $"{expPoint} / {levelUpPoint}";
        }

        private void UpdateLevelText(int inGameLevel)
        {
            levelText.text = $"LV {inGameLevel}";
        }

        public void SaveExp()
        {
            PlayerPrefs.SetInt("inGameLevel", level);
            PlayerPrefs.SetInt("levelUpPoint", levelUpPoint);
            PlayerPrefs.SetFloat("expPoint", expPoint);
        }

        public void LoadExp()
        {
            level = PlayerPrefs.GetInt("inGameLevel");
            levelUpPoint = PlayerPrefs.GetInt("levelUpPoint");
            expPoint = PlayerPrefs.GetFloat("expPoint");
            UpdateExpText();
            UpdateLevelText(level);
        }
    }
}