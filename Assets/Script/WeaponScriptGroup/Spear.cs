using System.Collections;
using DG.Tweening;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class Spear : WeaponBase
    {
        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();

            // Detect enemies in both directions
            if (EnforceManager.Instance.divineAtkRange)
            {
                FireProjectile(-Distance); // Fire in -y direction
                FireProjectile(Distance); // Fire in +y direction
            }
            else
            {
                FireProjectile(Distance); // Fire in +y direction
            }
        }

// This method handles the actual firing of the projectile.
        private void FireProjectile(float distance)
        {
            var duration = Mathf.Abs(distance) / Speed;
            float endPosition;

            if (distance < 0)
            {
                // If we're moving in the negative direction, we need to rotate the projectile.
                transform.rotation = Quaternion.Euler(0, 0, 180);
                endPosition = StartingPosition.y - distance;
                transform.DOMoveY(-endPosition, duration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => StopUseWeapon(gameObject));
            }
            else
            {
                endPosition = StartingPosition.y + distance;
                transform.DOMoveY(endPosition, duration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => StopUseWeapon(gameObject));
            }

        }



        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            HitEnemy.Add(enemy);
            foreach (var enemyObject in HitEnemy)
            {
                AtkEffect(enemyObject);
                var damage = DamageCalculator(Damage, enemyObject);
                enemy.ReceiveDamage(damage);
            }
            
            if (!EnforceManager.Instance.divinePenetrate )
            {
                StopUseWeapon(gameObject);
            }
            else
            {
                if (HitEnemy.Count == 2)
                {
                    StopUseWeapon(gameObject);
                }
            }
            HitEnemy.Clear();
        }
    }
}
