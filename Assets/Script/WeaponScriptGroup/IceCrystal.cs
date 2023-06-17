using System.Collections;
using DG.Tweening;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class IceCrystal : WeaponBase
    {
        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            var duration = Distance / Speed;
            var endPosition = StartingPosition.y + Distance;
            transform.DOMoveY(endPosition, duration).SetEase(Ease.Linear).OnComplete(() => StopUseWeapon(this.gameObject));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            HitEnemy.Add(enemy);
            foreach (var enemyObject in HitEnemy)
            {
                AtkEffect(enemyObject);
                if (EnforceManager.waterRestraintKnockBack)
                {
                    KnockBackEffect(enemyObject);
                }
                var damage = DamageCalculator(Damage, enemyObject);
                enemyObject.ReceiveDamage(damage);
                StopUseWeapon(gameObject);
            }
            HitEnemy.Clear();
        }
        private static bool KnockBackEffect(EnemyBase enemyObject)
        {
            var knockBack = false;
            if (enemyObject.IsRestraint)
            {
                enemyObject.transform.DOMoveY(enemyObject.transform.position.y + 0.5f, 0.2f)
                    .OnComplete(() => knockBack = true);
            }
            return knockBack;
        }
    }
}
