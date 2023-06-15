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
            if (enemy.IsRestraint && WaterRestraintKnockBack)
            {
                enemy.transform.DOMoveY(enemy.transform.position.y + 0.5f, 0.1f);
            }
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                enemy.ReceiveDamage(Damage, UnitProperty);
                AtkEffect(enemy);
            }
            StopUseWeapon(gameObject);
        }
    }
}
