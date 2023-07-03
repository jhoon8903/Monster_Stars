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

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            EnemyBase currentEnemy = null;
            while (isInUse)
            {
                _enemyTransforms = CharacterBase.GetComponent<UnitB>().DetectEnemies();
                if (_enemyTransforms.Count > 0)
                {
                    _enemyTransforms.Sort((enemy1, enemy2) => Vector3.Distance(transform.position, enemy1.transform.position)
                        .CompareTo(Vector3.Distance(transform.position, enemy2.transform.position)));
                    if (currentEnemy == null || !currentEnemy.gameObject.activeInHierarchy || 
                        Vector3.Distance(transform.position, currentEnemy.transform.position) > 
                        Vector3.Distance(transform.position, _enemyTransforms[0].transform.position))
                    {
                        currentEnemy = _enemyTransforms[0].GetComponent<EnemyBase>();
                    }
                }
                else
                {
                    StopUseWeapon(gameObject);
                    yield break;
                }
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
            if (enemy == null || HitEnemy.Contains(enemy)) return; // Skip if it's already hit
            HitEnemy.Add(enemy);
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy);
            enemy.ReceiveDamage(enemy,damage);
            switch (EnforceManager.Instance.darkProjectilePenetration)
            {
                case true when HitEnemy.Count == 2:
                    StopUseWeapon(gameObject);
                    HitEnemy.Clear(); // Only clear when the weapon stops
                    break;
                case false:
                    StopUseWeapon(gameObject);
                    HitEnemy.Clear();
                    break;
            }
        }
    }
}