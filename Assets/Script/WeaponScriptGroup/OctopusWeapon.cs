using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;
namespace Script.WeaponScriptGroup
{
    public class OctopusWeapon : WeaponBase
    {
        private float _distance;
        private Rigidbody2D _rigidBody2D;
        private List<GameObject> _enemyTransforms = new List<GameObject>();
        private Vector3 _enemyTransform;
        private new void Awake()
        {
            _rigidBody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            _enemyTransforms = CharacterBase.GetComponent<Octopus>().DetectEnemies();
            foreach (var enemy in _enemyTransforms)
            {
                _enemyTransform = enemy.transform.position;
            }
            if (CharacterBase.GetComponent<Octopus>().atkCount == 5)
            {
                Sprite = GetComponent<SpriteRenderer>().color = Color.yellow;
                transform.localScale = new Vector3(1.5f, 1.5f, 0);
                Damage *= 2f;
                CharacterBase.GetComponent<Octopus>().atkCount = 0;                
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