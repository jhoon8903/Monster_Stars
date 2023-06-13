using System.Collections;
using DG.Tweening;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class Sword : WeaponBase
    {
        public GameObject pivotPoint;
        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            Speed = CharacterBase.swingSpeed;
            pivotPoint.transform.DORotate(new Vector3(0, 0, 360), Speed, RotateMode.FastBeyond360).OnComplete(() => StopUseWeapon(pivotPoint));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                enemy.ReceiveDamage(Damage, UnitProperty, UnitEffect);
                AtkEffect(enemy);
            }
        }
    }
}
