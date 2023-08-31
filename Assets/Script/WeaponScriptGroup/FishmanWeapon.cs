using System.Collections;
using System.Linq;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class FishmanWeapon : WeaponBase
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
            var enemyTransforms = CharacterBase.GetComponent<Fishman>().DetectEnemies();
            foreach (var enemy in enemyTransforms)
            {
                Debug.Log(enemy.transform.position);
            }

            direction = Vector2.up;
            foreach (var unused in enemyTransforms.Where(enemy => enemy.transform.position.y < CharacterBase.transform.position.y))
            {
                transform.rotation = Quaternion.Euler(0, 0, 180);
                direction = Vector2.down;
            }
            _rigidBody2D.velocity = direction == Vector2.down ? new Vector2(0, -Speed) : new Vector2(0, Speed);
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
