using System.Collections;
using System.Linq;
using Script.CharacterGroupScript;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class DeathChillerWeapon : WeaponBase
    {
        private Rigidbody2D _rigidBody2D;
        private Vector3 _enemyTransformC;
        private new void Awake()
        {
            _rigidBody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            var useTime = Distance / Speed;
            var enemyPosition = CharacterBase.GetComponent<DeathChiller>().DetectEnemies();
            foreach (var unused in enemyPosition.Where(enemy => enemy.transform.position.y <= CharacterBase.transform.position.y))
            {
                Speed = -Speed;
                transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            _rigidBody2D.velocity = new Vector2(0, Speed);
            yield return new WaitForSeconds(useTime);
            StopUseWeapon(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (HasHit) return;
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            HasHit = true;
            AtkEffect(enemy, CharacterBase);
            var damage = DamageCalculator(Damage, enemy, CharacterBase); 
            enemy.ReceiveDamage(enemy,(int)damage, CharacterBase);
            StopUseWeapon(gameObject);
        }
    }
}