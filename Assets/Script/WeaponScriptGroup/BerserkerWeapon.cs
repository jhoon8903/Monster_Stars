using System.Collections;
using System.Collections.Generic;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class BerserkerWeapon : WeaponBase
    {
        private float _distance;
        private Vector3 _enemyTransform;
        private List<GameObject> _enemyTransforms = new List<GameObject>();
        private int _bounceCount;
        private Rigidbody2D _rigidBody2D;
        private new void Awake()
        {
            _rigidBody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            _enemyTransforms = CharacterBase.GetComponent<Berserker>().DetectEnemies();

            if (CharacterBase.GetComponent<Berserker>().atkCount % 3 == 0)
            {
                CharacterBase.GetComponent<Berserker>().atkCount = 0;
                Damage *= 2f;
                var useTime = Distance / Speed;
                _rigidBody2D.velocity = Vector2.up;
                yield return new WaitForSeconds(useTime);
                StopUseWeapon(gameObject);
            }
            
            foreach (var enemy in _enemyTransforms)
            {
                _enemyTransform = enemy.transform.position;
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
