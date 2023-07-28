using System.Collections;
using Script.CharacterGroupScript;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using UnityEngine;
namespace Script.WeaponScriptGroup
{
    public class A : WeaponBase
    {
        private float _distance;
        private Rigidbody2D _rigidbody2D;
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();

            if (CharacterBase.GetComponent<UnitA>().atkCount == 5)
            {
                Sprite = Color.yellow;
                Damage *= 2f;
                CharacterBase.GetComponent<UnitA>().atkCount = 0;                
            }
            var useTime = Distance / Speed;
            _rigidbody2D.velocity = new Vector2(0, Speed);
            yield return new WaitForSeconds(useTime);
            StopUseWeapon(gameObject);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (HasHit)return;
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            HasHit = true;
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy, CharacterBase.UnitGroups.A); 
            enemy.ReceiveDamage(enemy,(int)damage,CharacterBase);
            StopUseWeapon(gameObject);
        }
    }
}