using System.Collections;
using DG.Tweening;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class IceCrystal : WeaponBase
    {
        private EnemyBase _cachedEnemy;
        private float _lastHitTime;
        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            var MaxDistanceY = CharacterBase.defaultAtkDistance;
            var distance = Mathf.Abs(transform.position.y - MaxDistanceY);
            var adjustedSpeed = Speed * distance;
            var timeToMove = distance / adjustedSpeed;

            transform.DOMoveY(transform.position.y + MaxDistanceY, timeToMove).SetEase(Ease.Linear).OnComplete(() => StopUseWeapon(this.gameObject));

            yield return new WaitForSecondsRealtime(FireRate);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                enemy.ReceiveDamage(Damage, unitProperty, unitEffect);
            }
            StopUseWeapon(gameObject);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy == null || !enemy.gameObject.activeInHierarchy || !(Time.time > _lastHitTime + FireRate)) return;
            enemy.ReceiveDamage(Damage, unitProperty, unitEffect);
            _lastHitTime = Time.time;
        }

    }
}
