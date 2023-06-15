using System;
using DG.Tweening;
using Script.CharacterManagerScript;
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
        private float _maxHealthPoint;
        protected internal int CrushDamage; // 충돌시 데미지
        protected internal float MoveSpeed; // 적 오브젝트의 이동속도, 1f 는 1초에 1Grid를 가는 속도 숫자가 커질수록 느려져야 함
        public enum EnemyTypes { Boss, Basic, Slow, Fast }
        protected internal EnemyTypes EnemyType; // 적 타입 빠른적, 느린적, 보통적, 보스
        protected enum RegistryTypes { Physics, Divine, Poison, None }
        protected RegistryTypes RegistryType; // 저항타입, 만약 공격하는 적이 해당 타입과 일치하면 20%의 데미지를 덜 입게 됨
        public enum SpawnZones { A, B, C, D, E }
        protected internal SpawnZones SpawnZone;
        private static readonly object Lock = new object();
        private float _currentHealth;
        public delegate void EnemyKilledEventHandler(object source, EventArgs args);
        public event EnemyKilledEventHandler EnemyKilled;

        // 상태이상로직
        public int groupSlowCount;
        public bool IsRestraint { get; set; } = false;

        public GameObject attackChar;

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

        public void Initialize()
        {
            _hpSlider = GetComponentInChildren<Slider>(true);
            _maxHealthPoint = healthPoint;
            _currentHealth = healthPoint;
            _hpSlider.maxValue = _maxHealthPoint;
            _hpSlider.value = _currentHealth;
            UpdateHpSlider();
            groupSlowCount = 0;
        }
        protected internal virtual void EnemyProperty()
        {
        }
        public void ReceiveDamage(float damage, CharacterBase.UnitProperties unitProperty, KillReasons reason = KillReasons.ByPlayer)
        {
            lock (Lock)
            {
                switch (unitProperty)
                {
                    case CharacterBase.UnitProperties.Physics:
                        if (RegistryType == RegistryTypes.Physics)
                        {
                            damage *= 0.8f;
                        }

                        break;
                    case CharacterBase.UnitProperties.Divine:
                        if (RegistryType == RegistryTypes.Divine)
                        {
                            damage *= 0.8f;
                        }
                        break;
                    case CharacterBase.UnitProperties.Poison:
                        var venomSac = FindObjectOfType<VenomSac>();
                        if (venomSac != null && RegistryType == RegistryTypes.Poison)
                        {
                            damage *= 0.8f;
                            venomSac.poisonDotDamage = 0;
                        }
                        break;
                    default:
                        damage *= 1f;
                        break;
                }
                _currentHealth -= damage;
                UpdateHpSlider();
                if (_currentHealth >= 0) return;
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

        public void DecreasedMoveSpeed()
        {
            if (groupSlowCount > 4) return;
            MoveSpeed *= 1.45f;
            groupSlowCount += 1;
        }
    }
}