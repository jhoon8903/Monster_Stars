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

            Damage = CharacterBase.defaultDamage;
            FireRate = CharacterBase.defaultAtkRate;
            var MaxDistanceY = CharacterBase.defaultAtkDistance;
            var distance = Mathf.Abs(transform.position.y - MaxDistanceY);
            var adjustedSpeed = Speed * distance;
            var timeToMove = distance / adjustedSpeed;

            transform.DOMoveY(MaxDistanceY, timeToMove).SetEase(Ease.Linear).OnComplete(() => StopUseWeapon(this.gameObject));

            yield return new WaitForSecondsRealtime(FireRate);
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
