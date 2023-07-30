using System.Collections;
using System.Linq;
using Script.CharacterGroupScript;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;
namespace Script.WeaponScriptGroup
{
    public class A : WeaponBase
    {
        private float _distance;
        private Rigidbody2D _rigidBody2D;
        private void Awake()
        {
            _rigidBody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();

            if (CharacterBase.GetComponent<UnitA>().atkCount == 5)
            {
                Sprite = GetComponent<SpriteRenderer>().color = Color.yellow;
                transform.localScale = new Vector3(1.5f, 1.5f, 0);
                Damage *= 2f;
                CharacterBase.GetComponent<UnitA>().atkCount = 0;                
            }
            var useTime = Distance / Speed;
            if (!EnforceManager.Instance.divineDualAttack)
            {
                var enemyTransforms = CharacterBase.GetComponent<UnitA>().DetectEnemies();
                foreach (var unused in enemyTransforms.Where(enemy => enemy.transform.position.y < CharacterBase.transform.position.y))
                {
                    Speed = -Speed;
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
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy, CharacterBase.UnitGroups.A); 
            enemy.ReceiveDamage(enemy,(int)damage,CharacterBase);
            StopUseWeapon(gameObject);
        }
    }
}