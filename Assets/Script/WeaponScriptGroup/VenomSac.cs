using System.Collections;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class VenomSac : WeaponBase
    {
        private EnemyBase _cachedEnemy;
        private float _lastHitTime;
        public float attackRange = 5.0f;  // You should set this to an appropriate value

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            var enemyList = FindObjectOfType<CharacterBase>().DetectEnemies();
            var enemyTransform  = new Vector3();
            foreach (var enemy in enemyList)
            {
                enemyTransform = enemy.transform.position;
            }
            var distance = Vector3.Distance(transform.position, enemyTransform);
            var adjustedSpeed = Speed * distance;
            var timeToMove = distance / adjustedSpeed;
            transform.DOMove(enemyTransform, timeToMove).SetEase(Ease.Linear).OnComplete(() => StopUseWeapon(gameObject));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                _cachedEnemy = enemy;
                enemy.ReceiveDamage(Damage, UnitProperty, UnitEffect);
            }
            StopUseWeapon(gameObject);
        }

        private void Update()
        {
            if (_cachedEnemy == null || !_cachedEnemy.gameObject.activeInHierarchy)
            {
                StopUseWeapon(gameObject);
                return;
            }

            var distance = Vector3.Distance(transform.position, _cachedEnemy.transform.position);
            if (distance > attackRange)
            {
                StopUseWeapon(gameObject);
            }
        }
    }
}