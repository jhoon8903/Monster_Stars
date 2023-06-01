using System.Collections;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class Spear : WeaponBase
    {
        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            Damage = CharacterBase.defaultDamage;
            FireRate = CharacterBase.defaultAtkRate;
            Speed = CharacterBase.projectileSpeed;
            var MaxDistanceY = CharacterBase.defaultAtkDistance;
            transform.DOMoveY(MaxDistanceY, Speed).SetEase(Ease.Linear).OnComplete(() => StopUseWeapon(this.gameObject));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            if(enemy != null)
            {
                enemy.ReceiveDamage(Damage);
            }

            StopUseWeapon(this.gameObject);
        }
    }
}
