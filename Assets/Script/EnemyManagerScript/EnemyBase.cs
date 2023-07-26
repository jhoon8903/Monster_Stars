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
        public float moveSpeed;
        public float originSpeed;
        public enum EnemyTypes { Boss, Group1, Group2, Group3 }
        protected internal EnemyTypes EnemyType;
        public enum RegistryTypes { Physics, Divine, Poison, Burn, Water, Darkness, None }
        protected internal RegistryTypes RegistryType; 
        protected internal SpawnZones SpawnZone;
        protected internal bool IsDead;
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
        public enum SpawnZones { A, B, C, D, E, F }
        public bool isBind;
        public bool isSlow;
        public bool isReceiveDamageDebuff;
        public bool isSlowC;
        public bool isSlowE;
        public bool isSlowStun;
        public bool isSlowBleedStun;
        public bool isBurningPoison;
        public bool isPoison;
        public bool isKnockBack;
        public bool IsPoison
        {
            get => isPoison;
            set
            {
                isPoison = value;
                if (!isPoison) return;
                var venomSac = FindObjectOfType<F>();
                if (venomSac == null) return;
                if (isPoison && gameObject.activeInHierarchy)
                {
                    StartCoroutine(venomSac.PoisonEffect(this));
                }
            }
        }
        public bool isBurn;
        public bool IsBurn
        {
            get => isBurn;
            set
            {
                isBurn = value;
                var fireBall = FindObjectOfType<G>();
                if (fireBall == null) return;
                if (isBurn && gameObject.activeInHierarchy)
                {
                    StartCoroutine(fireBall.BurningEffect(this));
                }
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
                var d= FindObjectOfType<D>();
                StartCoroutine(d.BleedEffect(this));
            }
        }
        public void Awake()
        {
            EnforceManager.Instance.OnAddRow += ResetEnemyHealthPoint;
        }

        public virtual void Initialize()
        {
            _hpSlider = GetComponentInChildren<Slider>(true);
            LoadEnemyHealthData();
            maxHealthPoint = healthPoint;
            currentHealth = maxHealthPoint;
            _hpSlider.maxValue = maxHealthPoint;
            _hpSlider.value = currentHealth;
            StartCoroutine(UpdateHpSlider());
            moveSpeed = originSpeed;
        }

        private void LoadEnemyHealthData()
        {
            var enemyHPs = Resources.Load<TextAsset>("EnemyHpData");
            var data = enemyHPs.text.Split(new[] { '\n' });
            var stageData = data[StageManager.Instance.latestStage].Split(",");
            var healthPointData = stageData[1].Split(" ");
            var baseHealthPoint = int.Parse(healthPointData[StageManager.Instance.currentWave - 1]);
            healthPoint = baseHealthPoint;
        }

        public void ReceiveDamage(EnemyBase detectEnemy, float damage, CharacterBase atkUnit, KillReasons reason = KillReasons.ByPlayer)
        {
            lock (Lock)
            {
                if (IsDead) return;
                if (detectEnemy.isReceiveDamageDebuff) damage *= 1.15f;
                currentHealth -= (int)damage;
                if (gameObject == null || !gameObject.activeInHierarchy) return;
                _updateSlider = true;
                if (currentHealth > 0f || IsDead) return;
                IsDead = true;
                ExpManager.Instance.HandleEnemyKilled(reason);
                EnemyKilledEvents(detectEnemy);
                if (EnforceManager.Instance.divineShackledExplosion && atkUnit.unitGroup == CharacterBase.UnitGroups.A)
                {
                   ExplosionDamage(detectEnemy, damage, atkUnit);
                }
                if (EnforceManager.Instance.fireBurnedEnemyExplosion && atkUnit.unitGroup == CharacterBase.UnitGroups.H)
                {
                    ExplosionDamage(detectEnemy, damage * 2, atkUnit);
                }
                if (EnforceManager.Instance.water2IceSpikeProjectile)
                {
                    if (atkUnit.unitGroup == CharacterBase.UnitGroups.E)
                    {
                       StartCoroutine( AtkManager.Instance.SplitAttack(new AttackData(atkUnit.gameObject,WeaponsPool.WeaponType.E),detectEnemy.transform.position));
                    }
                }
            }
        }

        private void ExplosionDamage(EnemyBase detectEnemy, float damage, CharacterBase atkUnit)
        {
            var enemyPosition = detectEnemy.transform.position;
            var explosionSize = new Vector2(3, 3);
            var colliders = Physics2D.OverlapBoxAll(enemyPosition, explosionSize, 0f);
            foreach (var enemyObject in colliders)
            {
                if (!enemyObject.gameObject.CompareTag("Enemy") || !enemyObject.gameObject.activeInHierarchy) continue;
                var nearEnemy = enemyObject.GetComponent<EnemyBase>();
                var explosionDamage = GetComponent<WeaponBase>().DamageCalculator(damage, detectEnemy, atkUnit.unitGroup);
                ReceiveDamage(nearEnemy, (int)explosionDamage, atkUnit);
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