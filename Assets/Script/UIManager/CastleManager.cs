using Script.EnemyManagerScript;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Script.RewardScript;

namespace Script.UIManager
{
    public class CastleManager : MonoBehaviour
    {
        [SerializeField] protected internal Slider hpBar;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private GameManager gameManager;
        public float HpPoint { get; private set; }
        private float MaxHpPoint { get; set; }
        public float baseCastleHp = 1000;
        private float _increaseHp;                   
        public bool TookDamageLastWave { get; set; }

        private void Awake()
        {
            _increaseHp = EnforceManager.Instance.castleMaxHp;
            HpPoint = baseCastleHp;
            MaxHpPoint = baseCastleHp;
            hpBar.maxValue = MaxHpPoint;
            hpBar.value = HpPoint;
            UpdateHpText();
        }

        private void UpdateHpText()
        {
            hpText.text = $"{HpPoint} / {MaxHpPoint}";
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemyBase = collision.gameObject.GetComponent<EnemyBase>();
            if (enemyBase == null) return;
            HpPoint -= enemyBase.CrushDamage;
            TookDamageLastWave = true;
            hpBar.DOValue(HpPoint, 1.0f);
            UpdateHpText();
            FindObjectOfType<EnemyBase>().EnemyKilledEvents(enemyBase);
            if (HpPoint > 0) return;
            HpPoint = 0;
            hpBar.value = HpPoint;
            UpdateHpText();
            StartCoroutine(gameManager.ContinueOrLose());
        }

        public void IncreaseMaxHp()
        {
            _increaseHp = EnforceManager.Instance.castleMaxHp;
            MaxHpPoint = baseCastleHp + _increaseHp; 
            HpPoint += 200f;
            hpBar.maxValue = MaxHpPoint;
            hpBar.value = HpPoint;
            UpdateHpText();
        }


        public void RecoverCastleHp()
        {
            if (TookDamageLastWave) return;
            HpPoint += 200;
            if (HpPoint > MaxHpPoint)
            {
                HpPoint = MaxHpPoint;
            }
        }

        public void SaveCastleHp()
        {
             PlayerPrefs.SetFloat("castleHP", HpPoint);
        }

        public void LoadCastleHp()
        {
            HpPoint = PlayerPrefs.GetFloat("castleHP");
            _increaseHp = EnforceManager.Instance.castleMaxHp;
            MaxHpPoint = baseCastleHp + _increaseHp;
            hpBar.maxValue = MaxHpPoint;
            hpBar.value = HpPoint;
            UpdateHpText();
        }
    }
}
