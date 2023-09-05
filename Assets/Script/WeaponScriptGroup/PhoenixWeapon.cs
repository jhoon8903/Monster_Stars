using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class PhoenixWeapon : WeaponBase
    {
        private float _distance;
        private Vector3 _enemyTransform;
        private List<GameObject> _enemyTransforms = new List<GameObject>();
        private int _bounceCount;

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            _enemyTransforms = CharacterBase.GetComponent<Phoenix>().DetectEnemies();
            foreach (var enemy in _enemyTransforms)
            {
                if (enemy.activeInHierarchy)
                {
                    _enemyTransform = enemy.transform.position;
                }
                else
                {
                    StopUseWeapon(gameObject);
                    yield break;
                }
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
            AtkEffect(enemy, CharacterBase);
            var damage = DamageCalculator(Damage, enemy, CharacterBase); 
            enemy.ReceiveDamage(enemy, (int)damage, CharacterBase);
            StopUseWeapon(gameObject);
        }
    }
}