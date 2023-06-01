using System.Collections;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class VenomSac : WeaponBase
    {
        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            Damage = CharacterBase.defaultDamage;
            Speed = CharacterBase.projectileSpeed;
            FireRate = CharacterBase.defaultAtkRate;
            var enemyList = CharacterBase.DetectEnemies();
            var enemyTransform  = new UnityEngine.Vector3();
            foreach (var enemy in enemyList)
            {
                enemyTransform = enemy.transform.position;
                yield return enemyTransform;
            }
            transform.DOMove(enemyTransform,Speed).SetEase(Ease.Linear).OnComplete(() => StopUseWeapon(this.gameObject));
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
