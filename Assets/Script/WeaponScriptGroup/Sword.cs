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
            if (PhysicIncreaseWeaponScale)
            {
                pivotPoint.transform.localScale = new Vector3(2f,1.7f,0);
            }
            pivotPoint.transform.DORotate(new Vector3(0, 0, 360), Speed, RotateMode.FastBeyond360).OnComplete(() => StopUseWeapon(pivotPoint));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                if (enemy.IsSlow && PhysicSlowAdditionalDamage)
                {
                    enemy.ReceiveDamage(Damage * 2f, UnitProperty);
                }
                else
                {
                    enemy.ReceiveDamage(Damage, UnitProperty);
                }
                AtkEffect(enemy);
            }
        }
    }
}
