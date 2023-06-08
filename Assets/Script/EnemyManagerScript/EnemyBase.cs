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
        [SerializeField] private EnemySpawnManager enemySpawnManager;
        [SerializeField] private ExpManager expManager;
        private Slider _hpSlider;
        public enum KillReasons { ByPlayer, ByCastle }
        public event Action<KillReasons> OnEnemyKilled;
        public int number = 0;
        protected internal float HealthPoint; // 적 오브젝트의 체력
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

        public void HpBarActive()
        {
            _hpSlider = GetComponentInChildren<Slider>(true);
            _maxHealthPoint = HealthPoint;
            _hpSlider.maxValue = _maxHealthPoint;
            _hpSlider.value = HealthPoint;
            UpdateHpSlider();
        }
        protected internal virtual void EnemyProperty()
        {
        }
        public void ReceiveDamage(float damage, CharacterBase.UnitProperties unitProperty, CharacterBase.UnitEffects unitEffect)
        {
            UnitEffectFunction(unitEffect);
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
                HealthPoint -= damage;
                UpdateHpSlider();
                if (!(HealthPoint <= 0)) return;
                FindObjectOfType<EnemyPool>().ReturnToPool(gameObject);
                OnEnemyKilled?.Invoke(reason);
                if (ExpManager.Instance != null)
                {
                    ExpManager.Instance.HandleEnemyKilled(reason);
                }
            }
        }
        private void UnitEffectFunction(CharacterBase.UnitEffects unitEffect)
        {
            // Cancel previous effect if it is still running
            if (_activeEffects.ContainsKey(unitEffect) && _activeEffects[unitEffect] != null)
            {
                StopCoroutine(_activeEffects[unitEffect]);
            }
            switch (unitEffect)
            {
                case CharacterBase.UnitEffects.Bleed:
                    _activeEffects[unitEffect] = StartCoroutine(BleedEffect(10, 2));
                    break;
                case CharacterBase.UnitEffects.Slow:
                    _activeEffects[unitEffect] = StartCoroutine(SlowEffect(0.6f, 1));
                    break;
                case CharacterBase.UnitEffects.Poison:
                    _activeEffects[unitEffect] = StartCoroutine(PoisonEffect(10, 2));
                    break;
                case CharacterBase.UnitEffects.Burn:
                    _activeEffects[unitEffect] = StartCoroutine(BurnEffect(10, 3));
                    break;
                case CharacterBase.UnitEffects.Stun:
                    _activeEffects[unitEffect] = StartCoroutine(StunEffect(1, 2));
                    break;
                case CharacterBase.UnitEffects.Strike:
                    _activeEffects[unitEffect] = StartCoroutine(StrikeEffect(1, 1));
                    break;
                case CharacterBase.UnitEffects.Restraint:
                    _activeEffects[unitEffect] = StartCoroutine(RestraintEffect(1, 2));
                    break;
                case CharacterBase.UnitEffects.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitEffect), unitEffect, null);
            }
        }
        private IEnumerator BleedEffect(float damage, float duration)
        {
            float timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                ReceiveDamage(damage, CharacterBase.UnitProperties.None, CharacterBase.UnitEffects.None); // No specific properties or effects associated with bleed
                yield return new WaitForSeconds(1); // Damage every second
            }
            _activeEffects[CharacterBase.UnitEffects.Bleed] = null;
        }
        private IEnumerator SlowEffect(float slowAmount, float duration)
        {
            var originalSpeed = MoveSpeed;
            MoveSpeed *= slowAmount;
            yield return new WaitForSeconds(duration);
            MoveSpeed = originalSpeed;
            _activeEffects[CharacterBase.UnitEffects.Slow] = null;
        }
        private IEnumerator PoisonEffect(float damage, float duration)
        {
            float timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                ReceiveDamage(damage, CharacterBase.UnitProperties.None, CharacterBase.UnitEffects.None);
                yield return new WaitForSeconds(1);
            }
            _activeEffects[CharacterBase.UnitEffects.Poison] = null;
        }
        private IEnumerator BurnEffect(float damage, float duration)
        {
            float timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                ReceiveDamage(damage, CharacterBase.UnitProperties.None, CharacterBase.UnitEffects.None);
                yield return new WaitForSeconds(1);
            }
            _activeEffects[CharacterBase.UnitEffects.Burn] = null;
        }
        private IEnumerator StrikeEffect(float knockBackDistance, float effectCooldown)
        {
            var position = transform.position;
            Vector2 originalPosition = position;
            yield return transform.DOMoveY(position.y + knockBackDistance, effectCooldown);
            _activeEffects[CharacterBase.UnitEffects.Strike] = null;
        }
        private IEnumerator StunEffect(float stunDuration, float effectCooldown)
        {
            var originalCanMove = canMove;
            canMove = false;
            yield return new WaitForSeconds(stunDuration);
            canMove = originalCanMove;
            yield return new WaitForSeconds(effectCooldown);
            _activeEffects[CharacterBase.UnitEffects.Stun] = null;
        }
        private IEnumerator RestraintEffect(float restraintDuration, float effectCooldown)
        {
            var originalCanMove = canMove;
            canMove = false;
            yield return new WaitForSeconds(restraintDuration);
            canMove = originalCanMove;
            yield return new WaitForSeconds(effectCooldown);
            _activeEffects[CharacterBase.UnitEffects.Restraint] = null;
        }
        public void DecreaseMoveSpeed(int decreaseAmount)
        {
            var percentageIncrease = (float)decreaseAmount / 100;
            MoveSpeed *= decreaseAmount * percentageIncrease;
        }
        private void UpdateHpSlider()
        {
            _hpSlider.DOValue(HealthPoint, 1.0f);
        }
    }
}