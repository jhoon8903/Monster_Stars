using System.Collections;
using System.Collections.Generic;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;
namespace Script.WeaponScriptGroup
{
    public class Spear : WeaponBase
    {
        private float _distance;
        private Vector3 _enemyTransform;
        private List<GameObject> _enemyTransforms = new List<GameObject>();
        private Rigidbody2D _rigidbody2D;
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            _enemyTransforms = CharacterBase.GetComponent<UnitA>().DetectEnemies();
            foreach (var enemy in _enemyTransforms)
            {
                _enemyTransform = enemy.transform.position;
            }

            var position = transform.position;
            _distance = Vector3.Distance(position, _enemyTransform);
            var velocityDirection = (_enemyTransform.y > position.y) ? 1 : -1;
            _rigidbody2D.velocity = new Vector2(0, Speed * velocityDirection);

            transform.rotation = Quaternion.Euler(0, 0, _enemyTransform.y > transform.position.y ? 0 : 180);

            yield return new WaitForSeconds(_distance / Speed);
            StopUseWeapon(gameObject);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy == null || HitEnemy.Contains(enemy)) return; // Skip if it's already hit
            HitEnemy.Add(enemy);
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy);
            enemy.ReceiveDamage(enemy,damage);
            switch (EnforceManager.Instance.divinePenetrate)
            {
                case true when HitEnemy.Count == 2:
                    StopUseWeapon(gameObject);
                    HitEnemy.Clear(); // Only clear when the weapon stops
                    break;
                case false:
                    StopUseWeapon(gameObject);
                    HitEnemy.Clear();
                    break;
            }
        }
    }
}