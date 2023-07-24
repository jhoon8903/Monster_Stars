using System.Collections;
using System.Collections.Generic;
using Script.CharacterGroupScript;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class B : WeaponBase
    {
        private float _distance;
        private Vector3 _enemyTransform;
        private List<GameObject> _enemyTransforms = new List<GameObject>();
        private int _bounceCount;

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            _enemyTransforms = CharacterBase.GetComponent<UnitB>().DetectEnemies();
            foreach (var enemy in _enemyTransforms)
            {
                _enemyTransform = enemy.transform.position;
            }

            if (EnforceManager.Instance.darkTenthAttackDamageBoost && CharacterBase.GetComponent<UnitB>().atkCount == 10)
            {
                Damage *= 3f;
                transform.GetComponent<SpriteRenderer>().color = Color.black;
            }

            while (Vector3.Distance(transform.position, _enemyTransform) > 0.1f)
            {
                var position = transform.position;
                var direction = (_enemyTransform - position).normalized;
                transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
                position = Vector3.MoveTowards(position, _enemyTransform, Speed * Time.deltaTime);
                transform.position = position;
                yield return null;
            }

            StopUseWeapon(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy == null || HitEnemy.Contains(enemy)) return;
            HitEnemy.Add(enemy);
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy, CharacterBase.UnitGroups.B); 
            enemy.ReceiveDamage(enemy, (int)damage, CharacterBase);
            StopUseWeapon(gameObject);
            HitEnemy.Clear();
        }
    }
}
