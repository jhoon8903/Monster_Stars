
using System.Collections;
using System.Linq;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class DarkElfWeapon : WeaponBase
    {
        private float _distance;
        private Rigidbody2D _rigidBody2D;
        private new void Awake()
        {
            _rigidBody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            var useTime = Distance / Speed;
            if (!EnforceManager.Instance.dark2DualAttack)
            {
                var enemyTransforms = CharacterBase.GetComponent<DarkElf>().DetectEnemies();
                foreach (var unused in enemyTransforms.Where(enemy => enemy.transform.position.y <= CharacterBase.transform.position.y))
                {
                    Speed = -Speed;
                    Damage *= 1.3f; 
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                }
            }
            _rigidBody2D.velocity = direction == Vector2.down ? new Vector2(0, -Speed) : new Vector2(0, Speed);
            yield return new WaitForSeconds(useTime);
            StopUseWeapon(gameObject);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (HasHit)return;
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            HasHit = true;
            AtkEffect(enemy, CharacterBase);
            var damage = DamageCalculator(Damage, enemy, CharacterBase); 
            enemy.ReceiveDamage(enemy,(int)damage,CharacterBase);
            StopUseWeapon(gameObject);
        }
    }
}
