using System.Collections;
using System.Collections.Generic;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class C : WeaponBase
    {
        private Rigidbody2D _rigidbody2D;
        private Vector3 _enemyTransformC;
        private List<GameObject> _enemyTransformsC = new List<GameObject>();
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            var useTime = Distance / Speed;
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
