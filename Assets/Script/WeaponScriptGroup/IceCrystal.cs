using System.Collections;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class IceCrystal : WeaponBase
    {
        private Rigidbody2D _rigidbody2D;
        
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, Speed);
            var useTime = Distance / Speed;
            yield return new WaitForSeconds(useTime);
            StopUseWeapon(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy);
            enemy.ReceiveDamage(enemy,damage);
            StopUseWeapon(gameObject);
        }
    }
}
