using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.UIManager;
using UnityEngine;
using UnityEngine.UI;

namespace Script.EnemyManagerScript
{
    public class EnemyBase : MonoBehaviour
    {
        private Slider _hpSlider;
        public enum KillReasons { ByPlayer, ByCastle }
        public event Action<KillReasons> OnEnemyKilled;
        public int number = 0;
        public float HealthPoint; // 적 오브젝트의 체력
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
        private readonly Dictionary<CharacterBase.UnitEffects, Coroutine> _activeEffects = new Dictionary<CharacterBase.UnitEffects, Coroutine>();
        public bool canMove = true;

        public void Initialize()
        {
            _hpSlider = GetComponentInChildren<Slider>(true);
            _maxHealthPoint = HealthPoint;
            _currentHealth = HealthPoint;
            _hpSlider.maxValue = _maxHealthPoint;
            _hpSlider.value = _currentHealth;
            UpdateHpSlider();
        }
        protected internal virtual void EnemyProperty()
        {
        }
        public void ReceiveDamage(float damage, CharacterBase.UnitProperties unitProperty, CharacterBase.UnitEffects unitEffect)
        {
            RegistryDamageFunction(damage, unitProperty);
        }
        public void RegistryDamageFunction(float damage,CharacterBase.UnitProperties unitProperty, KillReasons reason = KillReasons.ByPlayer)
        {
            if (unitProperty.ToString() == RegistryType.ToString())
            { 
                damage *= 0.8f;
            }
            lock (Lock)
            {
                _currentHealth -= damage;
                UpdateHpSlider();
                if (!(_currentHealth <= 0)) return;
                FindObjectOfType<EnemyPool>().ReturnToPool(gameObject);
                OnEnemyKilled?.Invoke(reason);
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