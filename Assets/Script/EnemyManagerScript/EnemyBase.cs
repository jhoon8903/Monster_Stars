using System.Collections;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.UIManager;
using Script.WeaponScriptGroup;
using UnityEngine;
using UnityEngine.UI;

namespace Script.EnemyManagerScript
{
    public class EnemyBase : MonoBehaviour
    {
        private CharacterBase _characterBase;
        private EnemyPool _enemyPool;
        private Slider _hpSlider;
        private bool _updateSlider;
        protected internal float CrushDamage; 
        protected internal float MoveSpeed;
        public enum EnemyTypes { Boss, BasicA, BasicD, Slow, Fast }
        protected internal EnemyTypes EnemyType;
        public enum RegistryTypes { Physics, Divine, Poison, Burn, Water, Darkness,  None }
        protected internal RegistryTypes RegistryType; 
        protected internal SpawnZones SpawnZone;
        protected internal bool isDead;
        protected internal int CurrentPoisonStacks { get; set; }
        protected internal int BurningStack { get; set; }
        protected internal int BleedingStack { get; set; }
        private static readonly object Lock = new object();
        public enum KillReasons { ByPlayer }
        public int number;
        public float healthPoint; // 적 오브젝트의 체력
        public float maxHealthPoint;
        public float currentHealth;
        public float lastIncreaseHealthPoint;
        public enum SpawnZones { A, B, C, D, E }
        public bool isRestraint;
        public bool isSlow;



        public bool isPoison;
        public bool IsPoison
        {
            get => isPoison;
            set
            {
                isPoison = value;
                if (!isPoison) return;
                var venomSac = FindObjectOfType<VenomSac>();
                StartCoroutine(venomSac.PoisonEffect(this));
            }
        }

        public bool isBurn;
        public bool IsBurn
        {
            get => isBurn;
            set
            {
                isBurn = value;
                if (!isBurn) return;
               var fireBall = FindObjectOfType<FireBall>();
                StartCoroutine(fireBall.BurningEffect(this));
            }
        }

        public bool isBleed;
        public bool IsBleed
        {
            get => isBleed;
            set
            {
                isBleed = value;
                if(!isBleed) return;
               var dart = FindObjectOfType<Dart>();
                StartCoroutine(dart.BleedEffect(this));

            }
        }
        public void Awake()
        {
            EnforceManager.Instance.OnAddRow += ResetEnemyHealthPoint;
        }

        public virtual void Initialize()
        {
            _hpSlider = GetComponentInChildren<Slider>(true);
            var stage = StageManager.Instance.currentStage;
            var wave = StageManager.Instance.currentWave;
            if (EnemyType != EnemyTypes.Boss)
            {
                if (wave % 10 == 1 && wave != 1)
                {
                    var previousWave = wave - 1;
                    var previousWaveIncrease = healthPoint * Mathf.Pow(1.1f, previousWave - 1) + (stage - 1) * 10;
                    maxHealthPoint = previousWaveIncrease * 0.6f;
                }
                else
                {
                    maxHealthPoint = healthPoint * Mathf.Pow(1.1f, wave - 1) + (stage - 1) * 10;
                }
            }
            healthPoint = maxHealthPoint;
            currentHealth = maxHealthPoint;
            _hpSlider.maxValue = maxHealthPoint;
            _hpSlider.value = currentHealth;
            StartCoroutine(UpdateHpSlider());
        }

        public void ReceiveDamage(EnemyBase detectEnemy, float damage, KillReasons reason = KillReasons.ByPlayer)
        {
            lock (Lock)
            {
                if (isDead) return;
                currentHealth -= damage;
                if (gameObject == null || !gameObject.activeInHierarchy) return;
                _updateSlider = true;
                if (currentHealth > 0f || isDead) return;
                isDead = true;
                ExpManager.Instance.HandleEnemyKilled(reason);
                if (EnforceManager.Instance.physicIncreaseDamage) EnforceManager.Instance.PhysicIncreaseDamage();
                EnemyKilledEvents(detectEnemy);
            }
        }

        public void EnemyKilledEvents(EnemyBase detectedEnemy)
        {
            var characterBase = FindObjectOfType<CharacterBase>();
            var enemyPool = FindObjectOfType<EnemyPool>();
            characterBase.DetectEnemies().Remove(detectedEnemy.gameObject);
            StageManager.Instance.EnemyDestroyEvent(detectedEnemy);
            enemyPool.ReturnToPool(detectedEnemy);
        }

        private IEnumerator UpdateHpSlider()
        {
            while (true)
            {
                if (_updateSlider)
                {
                    yield return _hpSlider.DOValue(currentHealth, 0.1f);
                    _updateSlider = false;
                }
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void ResetEnemyHealthPoint()
        { 
            lastIncreaseHealthPoint *= 0.4f;
            healthPoint = lastIncreaseHealthPoint;
            maxHealthPoint = lastIncreaseHealthPoint;
           currentHealth = maxHealthPoint;
           _hpSlider.value = currentHealth;
           _hpSlider.maxValue = maxHealthPoint;
        }

        public void Start()
        {
            StartCoroutine(UpdateHpSlider());
        }

        private void OnDestroy()
        {
            EnforceManager.Instance.OnAddRow -= ResetEnemyHealthPoint;
        }
    }
}