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
            if (enemy == null || HitEnemy.Contains(enemy)) return; // Skip if it's already hit
            HitEnemy.Add(enemy);
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy);
            enemy.ReceiveDamage(enemy,damage);
            switch (EnforceManager.Instance.divinePenetrate)
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
