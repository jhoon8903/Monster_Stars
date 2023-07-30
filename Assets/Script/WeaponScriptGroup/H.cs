using System;
using System.Collections;
using System.Collections.Generic;
using Script.CharacterGroupScript;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;
namespace Script.WeaponScriptGroup
{
    public class H : WeaponBase
    {
        private float _distance;
        private Vector3 _enemyTransform;
        private List<GameObject> _enemyTransforms = new List<GameObject>();
        private Rigidbody2D _rigidbody2D; 
        private int _bounceCount; 
        private int _maxStack; 
        private readonly Dictionary<EnemyBase, int> _burnStacks = new Dictionary<EnemyBase, int>();
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            if (!EnforceManager.Instance.fireProjectileSpeedIncrease)
            {
                while (isInUse)
                {
                    _enemyTransforms = CharacterBase.GetComponent<UnitH>().DetectEnemies();
                    if (_enemyTransforms.Count == 0)
                    {
                        StopUseWeapon(gameObject);
                        break;
                    }
                    var currentEnemy = _enemyTransforms[0].GetComponent<EnemyBase>();
                    var upwards = (currentEnemy.transform.position - transform.position).normalized;

                    transform.rotation = Quaternion.LookRotation(Vector3.forward, upwards);
                    _rigidbody2D.velocity = upwards * Speed;
                    var useTime = Distance / Speed;
                    yield return new WaitForSeconds(useTime);
                    StopUseWeapon(gameObject);
                }
            }
            else
            {
                while (isInUse)
                {
                    if (_bounceCount == 0)
                    {
                        _enemyTransforms = CharacterBase.GetComponent<UnitH>().DetectEnemies();
                    }
                    else
                    {
                        var nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, 1f);
                        _enemyTransforms = new List<GameObject>();
                        foreach (var enemyCollider in nearbyEnemies)
                        {
                            var enemy = enemyCollider.gameObject;
                            if (enemy.CompareTag("Enemy") && !HitEnemy.Contains(enemy.GetComponent<EnemyBase>()))
                            {
                                _enemyTransforms.Add(enemy);
                            }
                        }
                    }
                    _enemyTransforms.Sort((enemy1, enemy2) =>
                    {
                        var position = transform.position;
                        return Vector3.Distance(position, enemy1.transform.position)
                            .CompareTo(Vector3.Distance(position, enemy2.transform.position));
                    });
                    if (_enemyTransforms.Count == 0)
                    {
                        StopUseWeapon(gameObject);
                        break;
                    }
                    var currentEnemy = _enemyTransforms[0].GetComponent<EnemyBase>();
                    var upwards = (currentEnemy.transform.position - transform.position).normalized;
                    transform.rotation = Quaternion.LookRotation(Vector3.forward, upwards);
                    _rigidbody2D.velocity = upwards * Speed;
                    yield return null;
                }
                StopUseWeapon(gameObject);
            }

        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (HasHit) return;
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            HasHit = true;
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy, CharacterBase.UnitGroups.H); 
            enemy.ReceiveDamage(enemy,damage, CharacterBase);
            var maxBounceCount = EnforceManager.Instance.fireProjectileBounceIncrease ? 2 : 1;
            if (EnforceManager.Instance.fireProjectileBounceDamage && _bounceCount <= maxBounceCount)
            {
                _bounceCount++;
                StopCoroutine(UseWeapon());
                HasHit = false;
                StartCoroutine(UseWeapon());
            }
            else
            {
                StopUseWeapon(gameObject);
            }
        }
        public IEnumerator BurningHEffect(EnemyBase hitEnemy)
        {
            _maxStack = EnforceManager.Instance.fireStackOverlap ? 4 : 1;
            if (!_burnStacks.ContainsKey(hitEnemy))
            {
                _burnStacks[hitEnemy] = 1;
            }
            else
            {
                _burnStacks[hitEnemy] = Math.Min(_burnStacks[hitEnemy] + 1, _maxStack);
            }
            var burnDotDamage = DamageCalculator(Damage, hitEnemy, CharacterBase.UnitGroups.H) * 0.1f * _burnStacks[hitEnemy];
            const int burningDuration = 5;
            for (var i = 0; i < burningDuration; i++)
            {
                hitEnemy.ReceiveDamage(hitEnemy, (int)burnDotDamage , CharacterBase);
                yield return new WaitForSeconds(1f);
            }
            _burnStacks[hitEnemy]--;
            if (_burnStacks[hitEnemy] > 0) yield break;
            _burnStacks.Remove(hitEnemy);
            hitEnemy.isBurnH = false;
            hitEnemy.IsBurnH = false;
        }
    }
}