using System.Collections;
using DG.Tweening;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class VenomSac : WeaponBase
    {
        // 캐시된 적의 참조를 보관합니다.
        private EnemyBase _cachedEnemy;

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();

            var enemyList = CharacterBase.DetectEnemies();
            var enemyTransform  = new UnityEngine.Vector3();
            foreach (var enemy in enemyList)
            {
                enemyTransform = enemy.transform.position;
                yield return enemyTransform;
            }
            var distance = Vector3.Distance(transform.position, enemyTransform);
            var adjustedSpeed = Speed * distance;
            var timeToMove = distance / adjustedSpeed;

            transform.DOMove(enemyTransform, timeToMove)
                .SetEase(Ease.Linear)
                .OnComplete(() => StopUseWeapon(this.gameObject));

            yield return new WaitForSecondsRealtime(FireRate);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;

            // 캐시된 적의 참조를 사용합니다.
            if(_cachedEnemy != null)
            {
                _cachedEnemy.ReceiveDamage(Damage, unitProperty, unitEffect);
            }
            StopUseWeapon(this.gameObject);
        }
    }
}