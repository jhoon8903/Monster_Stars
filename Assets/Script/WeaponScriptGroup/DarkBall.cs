using System.Collections;
using System.Collections.Generic;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class DarkBall : WeaponBase
    {
        private float _distance;
        private Vector3 _enemyTransform;
        private List<GameObject> _enemyTransforms = new List<GameObject>();
        private Rigidbody2D _rigidbody2D;
        private int _bounceCount; 

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();

            while (isInUse)
            {
                if (_bounceCount == 0)
                {
                    _enemyTransforms = CharacterBase.GetComponent<UnitB>().DetectEnemies();
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
                    Vector3.Distance(transform.position, enemy1.transform.position)
                        .CompareTo(Vector3.Distance(transform.position, enemy2.transform.position)));

                if (_enemyTransforms.Count == 0)
                {
                    StopUseWeapon(gameObject);
                    break;
                }
                var currentEnemy = _enemyTransforms[0].GetComponent<EnemyBase>();
                var direction = (currentEnemy.transform.position - transform.position).normalized;
                _rigidbody2D.velocity = direction * Speed;
                yield return null;
            }
            StopUseWeapon(gameObject);
        }



        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy == null || HitEnemy.Contains(enemy)) return;

            HitEnemy.Add(enemy);
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy);
            enemy.ReceiveDamage(enemy, damage);

            if (EnforceManager.Instance.darkProjectileBounce &&
                _bounceCount <= EnforceManager.Instance.darkProjectileBounceCount)
            {
                _bounceCount++;
                StopCoroutine(UseWeapon());
                StartCoroutine(UseWeapon());
            }
            else
            {
                StopUseWeapon(gameObject);
            }
        }
    }
}
