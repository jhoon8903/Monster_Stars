using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.CharacterGroupScript;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class F : WeaponBase
    {
        private float _distance;
        private Vector3 _enemyTransform;
        private List<GameObject> _enemyTransforms = new List<GameObject>();
        public float poisonDotDamage;
        private Rigidbody2D _rigidbody2D;
        private int _bounceCount; 

        private void Start() 
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();

            while (isInUse)
            {
                _enemyTransforms = CharacterBase.GetComponent<UnitF>().DetectEnemies();
                if (_enemyTransforms.Count == 0)                             
                {
                    StopUseWeapon(gameObject);
                    break;
                }
                var currentEnemy = _enemyTransforms[0].GetComponent<EnemyBase>();
                var direction = (currentEnemy.transform.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
                _rigidbody2D.velocity = direction * Speed;
                yield return null;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (HasHit) return;
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            HasHit = true;
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy, CharacterBase.UnitGroups.F); 
            enemy.ReceiveDamage(enemy,(int)damage,CharacterBase);
            StopUseWeapon(gameObject);
        }


        public IEnumerator PoisonEffect(EnemyBase hitEnemy)
        {
            var increaseDotDamage = EnforceManager.Instance.poisonDotDamageBoost ? 0.3f : 0.2f;
            poisonDotDamage = DamageCalculator(Damage, hitEnemy, CharacterBase.UnitGroups.F) * increaseDotDamage;
            var venomDuration = EnforceManager.Instance.poisonDurationBoost ? 5 : 3;
            for (var i = 0; i < venomDuration; i++)
            {
                hitEnemy.ReceiveDamage(hitEnemy,(int)poisonDotDamage , CharacterBase);
                yield return new WaitForSeconds(1f);
            }
            hitEnemy.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.2f);
            hitEnemy.isPoison = false;
            hitEnemy.IsPoison = false;
        }
    }
}