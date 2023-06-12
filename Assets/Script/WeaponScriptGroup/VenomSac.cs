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
        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            var enemyList = FindObjectOfType<CharacterBase>().DetectEnemies();
            var enemyTransform  = new Vector3();
            foreach (var enemy in enemyList)
            {
                enemyTransform = enemy.transform.position;
                yield return enemyTransform;
            }
            var distance = Vector3.Distance(transform.position, enemyTransform);
            var adjustedSpeed = Speed * distance;
            var timeToMove = distance / adjustedSpeed;
            transform.DOMove(enemyTransform, timeToMove).SetEase(Ease.Linear).OnComplete(() => StopUseWeapon(gameObject));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            if(_cachedEnemy != null)
            {
                _cachedEnemy.ReceiveDamage(Damage, UnitProperty, UnitEffect);
            }
            StopUseWeapon(this.gameObject);
        }
    }
}