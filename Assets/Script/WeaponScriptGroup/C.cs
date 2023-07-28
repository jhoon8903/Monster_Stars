using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterGroupScript;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class C : WeaponBase
    {
        private Rigidbody2D _rigidbody2D;
        private Vector3 _enemyTransformC;
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            var useTime = Distance / Speed;
            var enemyPosition = CharacterBase.GetComponent<UnitC>().DetectEnemies();
            foreach (var enemy in enemyPosition.Where(enemy => enemy.transform.position.y < CharacterBase.transform.position.y))
            {
                Speed = -Speed;
                transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            _rigidbody2D.velocity = new Vector2(0, Speed);
            yield return new WaitForSeconds(useTime);
            StopUseWeapon(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (HasHit) return;
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            HasHit = true;
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy, CharacterBase.UnitGroups.C); 
            enemy.ReceiveDamage(enemy,(int)damage,CharacterBase);
            StopUseWeapon(gameObject);
        }
    }
}
