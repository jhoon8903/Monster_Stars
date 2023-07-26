using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.UIManager;
using Script.WeaponScriptGroup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.EnemyManagerScript
{
    public class EnemyBase : MonoBehaviour
    {
        [SerializeField] private GameObject damageText;
        private readonly List<GameObject> damagePopupList = new List<GameObject>();
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
        public float healthPoint;
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
        private GameObject damagePopup;
        private bool pooling;
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

        public virtual void Initialize()
        {
            _hpSlider = GetComponentInChildren<Slider>(true);
            if (!pooling)
            {
                for (var i = 0; i < 10; i++)
                {
                    damagePopup = Instantiate(damageText, gameObject.transform, false);
                    damagePopupList.Add(damagePopup);
                    damagePopup.SetActive(false);
                    pooling = true;
                }
            }
            if (EnemyType != EnemyTypes.Boss)
            {
                _hpSlider.gameObject.SetActive(false);
            }
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
            StartCoroutine(UpdateHpSlider());
        }

        private IEnumerator DamageTextPopup(int damage)
        {
            if (!gameObject.activeInHierarchy) yield break;

            foreach (var popup in damagePopupList)
            {
                if (!popup.activeInHierarchy)
                {
                    var pos = gameObject.transform.position;
                    popup.transform.position = new Vector3(pos.x,pos.y + 0.5f,0f);
                    if (EnemyType == EnemyTypes.Boss)
                    {
                        popup.transform.position = new Vector3(pos.x,pos.y + 1.7f,0f);
                    }

                    Vector2 startPosition = popup.transform.position;
                    var endPosition = new Vector2(startPosition.x, startPosition.y + 0.2f);

                    if (damage != 0)
                    {
                        popup.SetActive(true);
                        popup.GetComponent<TextMeshPro>().text = damage.ToString();

                        float t = 0;
                        const float speed = 1f;
                        while (t < 1)
                        {
                            t += Time.deltaTime * speed;
                            popup.transform.position = Vector2.Lerp(startPosition, endPosition, t);
                            yield return null;
                        }
                        yield return new WaitForSeconds(0.1f); // Adjust this time as needed
                        popup.SetActive(false);
                    }
                }
            }
        }


        public void ReceiveDamage(EnemyBase detectEnemy, float damage, CharacterBase atkUnit, KillReasons reason = KillReasons.ByPlayer)
        {
            lock (Lock)
            {
                var receiveDamage = (int)damage;
                if (IsDead) return;
                if (detectEnemy.isReceiveDamageDebuff) damage *= 1.15f;
                currentHealth -= receiveDamage;
                StartCoroutine(DamageTextPopup(receiveDamage));
                if (!gameObject.activeInHierarchy) return;
                _updateSlider = true;
             
                if (currentHealth > 0f || IsDead) return;
                IsDead = true;
                StopCoroutine(DamageTextPopup(receiveDamage));
                foreach (var popup in damagePopupList.Where(popup => popup.activeInHierarchy))
                {
                    popup.SetActive(false);
                }
                if (EnforceManager.Instance.divineShackledExplosion && atkUnit.unitGroup == CharacterBase.UnitGroups.A)
                {
                    ExplosionDamage(detectEnemy, damage, atkUnit);
                }
                else if (EnforceManager.Instance.fireBurnedEnemyExplosion && atkUnit.unitGroup == CharacterBase.UnitGroups.H)
                {
                    ExplosionDamage(detectEnemy, damage, atkUnit);
                }
                if (EnforceManager.Instance.water2IceSpikeProjectile)
                {
                    if (atkUnit.unitGroup == CharacterBase.UnitGroups.E)
                    {
                        StartCoroutine( AtkManager.Instance.SplitAttack(new AttackData(atkUnit.gameObject,WeaponsPool.WeaponType.E),detectEnemy.transform.position));
                    }
                }
                ExpManager.Instance.HandleEnemyKilled(reason);
                EnemyKilledEvents(detectEnemy);
            }
        }

        private void ExplosionDamage(EnemyBase detectEnemy, float damage, CharacterBase atkUnit)
        {
            if (atkUnit.unitGroup == CharacterBase.UnitGroups.H)
            {
                damage *= 2f;
            }
            var enemyPosition = detectEnemy.transform.position;
            const float explosionSize = 1.5f;
            var colliders = Physics2D.OverlapCircleAll(enemyPosition, explosionSize);
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
    }
}