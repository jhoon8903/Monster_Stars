using System.Collections;
using System.Collections.Generic;
using Script.CharacterGroupScript;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
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
            if (CharacterBase.GetComponent<UnitB>().atkCount == 5)
            {
                Damage *= 2f;
                CharacterBase.GetComponent<UnitB>().atkCount = 0;
            }
            while (Vector3.Distance(transform.position, _enemyTransform) > 0.1f)
            {
                var position = transform.position;
                var upwards = (_enemyTransform - position).normalized;
                transform.rotation = Quaternion.LookRotation(Vector3.forward, upwards);
                position = Vector3.MoveTowards(position, _enemyTransform, Speed * Time.deltaTime);
                transform.position = position;
                yield return null;
            }
            StopUseWeapon(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (HasHit) return;
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            HasHit = true;
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy, CharacterBase.UnitGroups.B); 
            enemy.ReceiveDamage(enemy, (int)damage, CharacterBase);
            StopUseWeapon(gameObject);
        }
    }
}
