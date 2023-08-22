using System.Collections;
using System.Collections.Generic;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class BeholderWeapon : WeaponBase
    {
        private float _distance;
        private Vector3 _enemyTransform;
        private List<GameObject> _enemyTransforms = new List<GameObject>();
        private Rigidbody2D _rigidbody2D; 
        private int _bounceCount;
        private new void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            if (!EnforceManager.Instance.beholderProjectileSpeedIncrease)
            {
                while (isInUse)
                {
                    _enemyTransforms = CharacterBase.GetComponent<Beholder>().DetectEnemies();
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
                        _enemyTransforms = CharacterBase.GetComponent<Beholder>().DetectEnemies();
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
            AtkEffect(enemy, CharacterBase);
            var damage = DamageCalculator(Damage, enemy, CharacterBase); 
            enemy.ReceiveDamage(enemy,damage, CharacterBase);
            var maxBounceCount = EnforceManager.Instance.beholderProjectileBounceIncrease ? 2 : 1;
            if (EnforceManager.Instance.beholderProjectileBounceDamage && _bounceCount <= maxBounceCount)
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
    }
}