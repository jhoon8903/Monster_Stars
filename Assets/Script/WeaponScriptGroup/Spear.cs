using System.Collections;
using DG.Tweening;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class Spear : WeaponBase
    {
        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            var duration = Distance / Speed;
            var endPosition = StartingPosition.y + (EnforceManager.divineAtkRange? -Distance : Distance);
            transform.rotation = Quaternion.Euler(0,0,EnforceManager.divineAtkRange? 180: 0);
            transform.DOMoveY(endPosition, duration).SetEase(Ease.Linear).OnComplete(() => { StopUseWeapon(gameObject);});
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
            
            if (!EnforceManager.divinePenetrate )
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
