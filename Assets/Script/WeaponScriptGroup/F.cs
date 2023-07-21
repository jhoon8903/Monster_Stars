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
                _rigidbody2D.velocity = direction * Speed;
                yield return null;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (EnforceManager.Instance.poisonEnemyInstantKill && enemy.healthPoint < enemy.maxHealthPoint * 0.07f && enemy.isPoison)
            {
                InstantKill(enemy);
            }
            else
            {
                AtkEffect(enemy);
                var damage = DamageCalculator(Damage, enemy, CharacterBase.UnitGroups.F); 
                enemy.ReceiveDamage(enemy,(int)damage,CharacterBase);
            }
            StopUseWeapon(gameObject);
        }


        public IEnumerator PoisonEffect(EnemyBase hitEnemy)
        {
            if (!EnforceManager.Instance.poisonPerHitEffect) yield break;
            poisonDotDamage = Damage * (0.1f + EnforceManager.Instance.poisonDamageAttackPowerIncrease / 10f);
            var damage = DamageCalculator(poisonDotDamage, hitEnemy, CharacterBase.UnitGroups.F); 
            var poisonColor = new Color(0.18f, 1f, 0.1f);
            hitEnemy.GetComponent<SpriteRenderer>().DOColor(poisonColor, 0.2f);
            hitEnemy.CurrentPoisonStacks++;
            if (hitEnemy.CurrentPoisonStacks >= EnforceManager.Instance.poisonMaxStackIncrease)
            {
                hitEnemy.CurrentPoisonStacks = EnforceManager.Instance.poisonMaxStackIncrease;
            }
            const float venomDuration = 4f;
            var elapsedTime = 0f;
            for (var i = 0; i < hitEnemy.CurrentPoisonStacks; i++)
            {
                while (elapsedTime < venomDuration)
                {

                    hitEnemy.ReceiveDamage(hitEnemy,(int)damage, CharacterBase);
                    yield return new WaitForSeconds(1f);
                    elapsedTime += Time.deltaTime;
                }
            }
            hitEnemy.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.2f);
            hitEnemy.CurrentPoisonStacks--;
            hitEnemy.isPoison = false;
        }
    }
}