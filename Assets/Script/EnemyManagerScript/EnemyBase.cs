using System;
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
        private Slider _hpSlider;

        protected internal float CrushDamage; // 충돌시 데미지
        protected internal float MoveSpeed; // 적 오브젝트의 이동속도, 1f 는 1초에 1Grid를 가는 속도 숫자가 커질수록 느려져야 함
        public enum EnemyTypes { Boss, BasicA, BasicD,Slow, Fast }
        protected internal EnemyTypes EnemyType; // 적 타입 빠른적, 느린적, 보통적, 보스
        public enum RegistryTypes { Physics, Divine, Poison, None }
        protected internal RegistryTypes RegistryType; // 저항타입, 만약 공격하는 적이 해당 타입과 일치하면 20%의 데미지를 덜 입게 됨
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
                if (venomSac != null && gameObject.activeInHierarchy)
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
                if (!isBurn) return;
                var fireBall = FindObjectOfType<FireBall>();
                if (fireBall != null && gameObject.activeInHierarchy)
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
                var dart = FindObjectOfType<Dart>();
                if (dart != null && gameObject.activeInHierarchy)
                {
                    StartCoroutine(dart.BleedEffect(this));
                }

            }
        }

        public void Awake()
        {
            EnforceManager.Instance.OnAddRow += ResetEnemyHealthPoint;
        }

        public virtual void Initialize()
        {
            var wave = FindObjectOfType<GameManager>().wave;
            _hpSlider = GetComponentInChildren<Slider>(true);
            if (EnemyType != EnemyTypes.Boss)
            {
                lastIncreaseHealthPoint = healthPoint  * Mathf.Pow(1.3f, wave - 1);
            }
            maxHealthPoint = lastIncreaseHealthPoint != 0 ? lastIncreaseHealthPoint : healthPoint;
            healthPoint = maxHealthPoint;
            currentHealth = maxHealthPoint;
            _hpSlider.maxValue = maxHealthPoint;
            _hpSlider.value = currentHealth;
            StartCoroutine(UpdateHpSlider());
        }
        public void ReceiveDamage(EnemyBase detectEnemy, float damage, KillReasons reason = KillReasons.ByPlayer)
        {
            // var stopPattern = FindObjectOfType<EnemyPatternManager>().Zone_Move(detectEnemy);
            lock (Lock)
            {
                if (isDead)
                {
                    return;
                }
                currentHealth -= damage;
                if (gameObject == null || !gameObject.activeInHierarchy) return;
                StartCoroutine(UpdateHpSlider());
                if (currentHealth > 0f ||  isDead) return;
                isDead = true;
                // StopCoroutine(UpdateHpSlider());
                ExpManager.Instance.HandleEnemyKilled(reason);
                if (EnforceManager.Instance.physicIncreaseDamage)
                {
                    EnforceManager.Instance.PhysicIncreaseDamage();
                }
                EnemyKilledEvents(detectEnemy);
            }
        }

        public void EnemyKilledEvents(EnemyBase detectedEnemy)
        {
            var waveManager = FindObjectOfType<WaveManager>();
            var characterBase = FindObjectOfType<CharacterBase>();
            var enemyPool = FindObjectOfType<EnemyPool>();
            characterBase.DetectEnemies().Remove(detectedEnemy.gameObject);
            waveManager.EnemyDestroyEvent(detectedEnemy);
            enemyPool.ReturnToPool(detectedEnemy);
        }

        private IEnumerator UpdateHpSlider()
        {
            yield return _hpSlider.DOValue(currentHealth, 0.1f);
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

        private void OnDestroy()
        {
            EnforceManager.Instance.OnAddRow -= ResetEnemyHealthPoint;
        }
    }
}