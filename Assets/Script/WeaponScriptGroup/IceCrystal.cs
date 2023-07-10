using System.Collections;
using System.Collections.Generic;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class IceCrystal : WeaponBase
    {
        private Rigidbody2D _rigidbody2D;
        private Vector3 _enemyTransformC;
        private List<GameObject> _enemyTransformsC = new List<GameObject>();
        private List<GameObject> _enemyTransformsB = new List<GameObject>();
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            var useTime = Distance / Speed;
            if (EnforceManager.Instance.water2BackAttack)
            {
                _enemyTransformsC = CharacterBase.GetComponent<UnitC>().DetectEnemies();
                foreach (var enemy in _enemyTransformsC)
                {
                    _enemyTransformC = enemy.transform.position;
                }
                var position = transform.position;
                Distance = Vector3.Distance(position, _enemyTransformC);
                var velocityDirection = (_enemyTransformC.y > position.y) ? 1 : -1;
                _rigidbody2D.velocity = new Vector2(0, Speed * velocityDirection);
                transform.rotation = Quaternion.Euler(0, 0, _enemyTransformC.y > transform.position.y ? 0 : 180);
            }
            _rigidbody2D.velocity = Direction * Speed;
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
