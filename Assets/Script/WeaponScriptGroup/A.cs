using System.Collections;
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
        private Rigidbody2D _rigidbody2D;
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            var useTime = Distance / Speed;
            _rigidbody2D.velocity = new Vector2(0, Speed);
            if (EnforceManager.Instance.divineFifthAttackBoost && CharacterBase.GetComponent<UnitA>().atkCount == 5)
            {
                transform.GetComponent<SpriteRenderer>().color = Color.cyan;
                Damage *= 2f;
                CharacterBase.GetComponent<UnitA>().atkCount = 0;
            }
            yield return new WaitForSeconds(useTime);
            StopUseWeapon(gameObject);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy == null || HitEnemy.Contains(enemy)) return;
            HitEnemy.Add(enemy);
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy, CharacterBase.UnitGroups.A); 
            enemy.ReceiveDamage(enemy,(int)damage,CharacterBase);
            switch (EnforceManager.Instance.divineProjectilePierce)
            {
                case true when HitEnemy.Count == 2:
                    StopUseWeapon(gameObject);
                    HitEnemy.Clear();
                    break;
                case false:
                    StopUseWeapon(gameObject);
                    HitEnemy.Clear();
                    break;
            }
        }
    }
}