using System.Collections;
using System.Collections.Generic;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class CobraWeapon : WeaponBase
    {
        private float _distance;
        private Vector3 _enemyTransform;
        private List<GameObject> _enemyTransforms = new List<GameObject>();
        private Rigidbody2D _rigidbody2D;
        private int _bounceCount; 

        private new void Awake() 
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();

            while (isInUse)
            {
                _enemyTransforms = CharacterBase.GetComponent<Cobra>().DetectEnemies();
                if (_enemyTransforms.Count == 0 || !_enemyTransforms[0].activeInHierarchy) 
                {
                    StopUseWeapon(gameObject);
                    break;
                }
                var currentEnemy = _enemyTransforms[0].GetComponent<EnemyBase>();
                var upwards = (currentEnemy.transform.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(Vector3.forward, upwards);
                _rigidbody2D.velocity = upwards * Speed;
                yield return null;
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (HasHit) return;
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