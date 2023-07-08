using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using Unity.VisualScripting;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class VenomSac : WeaponBase
    {
        private float _distance;
        private Vector3 _enemyTransform;
        private List<GameObject> _enemyTransforms = new List<GameObject>();
        public float poisonDotDamage;

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            _enemyTransforms = CharacterBase.GetComponent<UnitF>().DetectEnemies();
            foreach (var enemy in _enemyTransforms)
            {
                _enemyTransform = enemy.transform.position;
            }
            _distance = Vector3.Distance(transform.position, _enemyTransform);
            var adjustedSpeed = Speed * _distance;
            var timeToMove = _distance / adjustedSpeed;
            if (_enemyTransform == Vector3.zero) yield return null;
            transform.DOMove(_enemyTransform, timeToMove).SetEase(Ease.Linear).OnComplete(() => StopUseWeapon(gameObject));
        }

        // VenomSac.cs
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (EnforceManager.Instance.poisonInstantKill && (enemy.healthPoint < enemy.maxHealthPoint * 0.15f))
            {
                var instantKillChance = Random.Range(0, 100);
                if (instantKillChance < 15)
                {
                    InstantKill(enemy);
                }
            }
            else
            {
                AtkEffect(enemy);
                var damage = DamageCalculator(Damage, enemy);
                enemy.ReceiveDamage(enemy,damage);
            }
            StopUseWeapon(gameObject);
        }


        public IEnumerator PoisonEffect(EnemyBase hitEnemy)
        {
            // If the enemy has max stacks of poison, we don't apply the poison again.
           
            if (hitEnemy.RegistryType == EnemyBase.RegistryTypes.Poison) yield break;
            if (!EnforceManager.Instance.activatePoison) yield break;
            poisonDotDamage = Damage * 0.1f;
            var poisonColor = new Color(0.18f, 1f, 0.1f);
            hitEnemy.GetComponent<SpriteRenderer>().DOColor(poisonColor, 0.2f);
            hitEnemy.CurrentPoisonStacks++;
            if (hitEnemy.CurrentPoisonStacks >= EnforceManager.Instance.poisonOverlapping) // Increment the poison count
            {
                hitEnemy.CurrentPoisonStacks = EnforceManager.Instance.poisonOverlapping;
            }

            const float venomDuration = 2f;
            var elapsedTime = 0f;

            for (var i = 0; i < hitEnemy.CurrentPoisonStacks; i++)
            {
                while (elapsedTime < venomDuration)
                {
    
                    hitEnemy.ReceiveDamage(hitEnemy,poisonDotDamage);
                    Debug.Log($"중첩{i}도트데미지: {poisonDotDamage}");
                    yield return new WaitForSeconds(1f);
                    elapsedTime += Time.deltaTime;
                }
            }
            hitEnemy.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.2f); // Reset the color
            hitEnemy.CurrentPoisonStacks--; // Decrement the poison count
            hitEnemy.isPoison = false;
        }
    }
}