using System.Collections;
using System.Collections.Generic;
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
        private readonly List<GameObject> _damagePopupList = new List<GameObject>();
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
        private static readonly object Lock = new object();
        public enum KillReasons { ByPlayer }
        public int number;
        public float healthPoint;
        public float maxHealthPoint;
        public float currentHealth;
        public enum SpawnZones { A, B, C, D, E, F }
        private GameObject _damagePopup;
        public bool isDead;
        public bool isBind;
        public bool isSlowC;
        public bool isSlowE;
        public bool isPoison;
        public bool isKnockBack;
        public bool isFreeze;
        public bool isFreezeE;
        private bool _pooling;
    
        public bool IsPoison
        {
            get => isPoison;
            set
            {
                isPoison = value;
                if (!isPoison) return;
                var venomSac = FindObjectOfType<F>();
                var poisonColor = new Color(0.18f, 1f, 0.1f);
                var originColor = gameObject.GetComponent<SpriteRenderer>();
                if (venomSac == null) return;
                if (!isPoison || !gameObject.activeInHierarchy) return;
                StartCoroutine(venomSac.PoisonEffect(this));
                StartCoroutine(FlickerEffect(originColor, poisonColor));
            }
        }
        public bool isBurnG;
        public bool IsBurnG
        {
            get => isBurnG;
            set
            {
                isBurnG = value;
                var g = FindObjectOfType<G>();
                var burningColor = new Color(1f,0,0.4f);
                var originColor = gameObject.GetComponent<SpriteRenderer>();
                if (g == null) return;
                if (!isBurnG || !gameObject.activeInHierarchy) return;
                StartCoroutine(g.BurningGEffect(this));
                StartCoroutine(FlickerEffect(originColor, burningColor));
            }
        }
        public bool isBurnH;
        public bool IsBurnH
        {
            get => isBurnH;
            set
            {
                isBurnH = value;
                var h = FindObjectOfType<H>();
                var burningColor = new Color(1f,0,0.4f);
                var originColor = gameObject.GetComponent<SpriteRenderer>();
                if (h == null) return;
                if (!isBurnH || !gameObject.activeInHierarchy) return;
                StartCoroutine(h.BurningHEffect(this));
                StartCoroutine(FlickerEffect(originColor, burningColor));
            }
        }

        public bool isBleed;
        public bool IsBleed
        {
            get => isBleed;
            set
            {
                isBleed = value;
                var sword = FindObjectOfType<D>();
                var bleedingColor = new Color(1f,0.2f,0.3f);
                var originColor = gameObject.GetComponent<SpriteRenderer>();
                if (!isBleed || !gameObject.activeInHierarchy) return;
                StartCoroutine(sword.BleedEffect(this));
                StartCoroutine(FlickerEffect(originColor, bleedingColor));
            }
        }
        public virtual void Initialize()
        {
            _hpSlider = GetComponentInChildren<Slider>(true);
            if (!_pooling)
            {
                for (var i = 0; i < 10; i++)
                {
                    _damagePopup = Instantiate(damageText, gameObject.transform, false);
                    _damagePopupList.Add(_damagePopup);
                    _damagePopup.SetActive(false);
                    _pooling = true;
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

            foreach (var popup in _damagePopupList)
            {
                if (popup.activeInHierarchy) continue;
                var pos = gameObject.transform.position;
                popup.transform.position = new Vector3(pos.x,pos.y + 0.5f,0f);
                if (EnemyType == EnemyTypes.Boss)
                {
                    popup.transform.position = new Vector3(pos.x,pos.y + 1.7f,0f);
                    popup.transform.localScale = new Vector3(3,3,0);
                }
                Vector2 startPosition = popup.transform.position;
                var endPosition = new Vector2(startPosition.x, startPosition.y + 0.2f);
                if (damage == 0) continue;
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
        public void ReceiveDamage(EnemyBase detectEnemy, float damage, CharacterBase atkUnit, KillReasons reason = KillReasons.ByPlayer)
        {
            lock (Lock)
            {
                var receiveDamage = (int)damage;
                if (isDead) return;
                currentHealth -= receiveDamage;
                if (!gameObject.activeInHierarchy) return;
                StartCoroutine(DamageTextPopup(receiveDamage));
                _updateSlider = true;

                if (currentHealth > 0f || isDead) return;
                StopCoroutine(DamageTextPopup(receiveDamage));
                isDead = true;
                if (EnforceManager.Instance.divineShackledExplosion && atkUnit.unitGroup == CharacterBase.UnitGroups.A)
                {
                    ExplosionDamage(detectEnemy, damage, atkUnit);
                }
                else if (EnforceManager.Instance.fireBurnedEnemyExplosion && atkUnit.unitGroup == CharacterBase.UnitGroups.H)
                {
                    ExplosionDamage(detectEnemy, damage, atkUnit);
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
            foreach (var popup in detectedEnemy._damagePopupList)
            {
                popup.SetActive(false);
            }
            var characterBase = FindObjectOfType<CharacterBase>();
            var enemyPool = FindObjectOfType<EnemyPool>();
            characterBase.DetectEnemies().Remove(detectedEnemy.gameObject);
            StageManager.Instance.EnemyDestroyEvent(detectedEnemy);
            detectedEnemy.IsPoison = false;
            detectedEnemy.isPoison = false;
            detectedEnemy.isBind = false;
            detectedEnemy.IsBurnG = false;
            detectedEnemy.isBurnG = false;
            detectedEnemy.IsBleed = false;
            detectedEnemy.isBleed = false;
            detectedEnemy.isDead = false;
            detectedEnemy.isSlowC = false;
            detectedEnemy.isSlowE = false;
            EnemyPatternManager.Instance.AlreadyRestrain.Remove(detectedEnemy);
            EnemyPatternManager.Instance.AlreadySlow.Remove(detectedEnemy);
            EnemyPatternManager.Instance.AlreadyKnockBack.Remove(detectedEnemy);
            EnemyPatternManager.Instance.AlreadyStatusSlow.Remove(detectedEnemy);
            EnemyPatternManager.Instance.AlreadyFreeze.Remove(detectedEnemy);
            detectedEnemy.transform.localScale = Vector3.one;
            detectedEnemy.GetComponent<SpriteRenderer>().color = Color.white;
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
        private IEnumerator FlickerEffect(SpriteRenderer render, Color targetColor)
        {
            var originalColor = render.color;
            var elapsedTime = 0f;
            while ( isBurnG || isPoison || isBleed )
            {
                var value = Mathf.Abs(Mathf.Sin(elapsedTime / 0.5f * Mathf.PI));
                render.color = Color.Lerp(originalColor, targetColor, value);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            render.color = originalColor;
        }
    }
}