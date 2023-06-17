using System;
using DG.Tweening;
using Script.CharacterGroupScript;
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
        public enum KillReasons { ByPlayer }
        public int number = 0;
        public float healthPoint; // 적 오브젝트의 체력
        public float maxHealthPoint;
        protected internal int CrushDamage; // 충돌시 데미지
        protected internal float MoveSpeed; // 적 오브젝트의 이동속도, 1f 는 1초에 1Grid를 가는 속도 숫자가 커질수록 느려져야 함
        public enum EnemyTypes { Boss, Basic, Slow, Fast }
        protected internal EnemyTypes EnemyType; // 적 타입 빠른적, 느린적, 보통적, 보스
        public enum RegistryTypes { Physics, Divine, Poison, None }
        protected internal RegistryTypes RegistryType; // 저항타입, 만약 공격하는 적이 해당 타입과 일치하면 20%의 데미지를 덜 입게 됨
        public enum SpawnZones { A, B, C, D, E }
        protected internal SpawnZones SpawnZone;
        private static readonly object Lock = new object();
        private float _currentHealth;
        public delegate void EnemyKilledEventHandler(object source, EventArgs args);
        public event EnemyKilledEventHandler EnemyKilled;

        public bool IsRestraint { get; set; } = false;
        public bool IsSlow { get; set; } = false;
        private Coroutine _poisonEffectCoroutine;
        private bool _isPoison;
        public bool IsPoison
        {
            get => _isPoison;
            set
            {
                _isPoison = value;
                if (_isPoison)
                {
                    var venomSac = FindObjectOfType<VenomSac>();
                    if (venomSac != null && gameObject.activeInHierarchy)
                    {
                        _poisonEffectCoroutine = StartCoroutine(venomSac.PoisonEffect(this));
                    }
                }
                else
                {
                    if (_poisonEffectCoroutine == null || !(healthPoint <= 0)) return;
                    StopCoroutine(_poisonEffectCoroutine);
                    gameObject.GetComponent<SpriteRenderer>().DOColor(new Color(1f, 1f, 1f), 0.2f);
                    _poisonEffectCoroutine = null;
                }
            }
        }
        private EnforceManager _enforceManager;
        protected internal int CurrentPoisonStacks { get; set; }

        public void Initialize()
        {
            _enforceManager = FindObjectOfType<EnforceManager>();
            var wave = FindObjectOfType<GameManager>().wave;
            _hpSlider = GetComponentInChildren<Slider>(true);

            if (EnemyType != EnemyTypes.Boss)
            {
                healthPoint *= Mathf.Pow(1.3f, wave - 1);
            }
            maxHealthPoint = healthPoint;
            _currentHealth = healthPoint;
            _hpSlider.maxValue = maxHealthPoint;
            _hpSlider.value = _currentHealth;
            UpdateHpSlider();
        }
        protected internal virtual void EnemyProperty()
        {
        }
        public void ReceiveDamage(float damage,KillReasons reason = KillReasons.ByPlayer)
        {
            lock (Lock)
            {
                _currentHealth -= damage;
                UpdateHpSlider();
                if (_currentHealth >= 0) return;
                if (_enforceManager.physicIncreaseDamage)
                {
                    _enforceManager.PhysicIncreaseDamage();
                }
                EnemyKilled?.Invoke(this, EventArgs.Empty);
                FindObjectOfType<EnemyPool>().ReturnToPool(gameObject);
                FindObjectOfType<WaveManager>().EnemyDestroyInvoke();
                if (ExpManager.Instance != null)
                {
                    ExpManager.Instance.HandleEnemyKilled(reason);
                }
            }
        }
        private void UpdateHpSlider()
        {
            _hpSlider.DOValue(_currentHealth, 0.5f);
        }
    }
}