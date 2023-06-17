using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class VenomSac : WeaponBase
    {
        private float _distance;
        private Vector3 _enemyTransform = new Vector3();
        private List<GameObject> _enemyTransforms = new List<GameObject>();
        public float poisonDotDamage = 5f;

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
            transform.DOMove(_enemyTransform, timeToMove).SetEase(Ease.Linear).OnComplete(() => StopUseWeapon(gameObject));
        }

        // VenomSac.cs
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var hitColliders = Physics2D.OverlapCircleAll(collision.transform.position, 0.5f);
            foreach (var enemyObject in hitColliders)
            {
                var enemy = enemyObject.gameObject.GetComponent<EnemyBase>();
                HitEnemy.Add(enemy);
                foreach (var targetObject in HitEnemy)
                {
                    if (EnforceManager.poisonInstantKill && 
                        (targetObject.healthPoint < (targetObject.maxHealthPoint * 0.15f))
                        && Random.Next(100) >= 15)
                    {
                        InstantKill(targetObject);
                    }
                    else
                    {
                        AtkEffect(targetObject);
                        var damage = DamageCalculator(Damage, targetObject);
                        if (targetObject == null || !targetObject.gameObject.activeInHierarchy || targetObject == enemy) continue;
                        targetObject.ReceiveDamage(damage);
                    }
                }
                StopUseWeapon(gameObject);
            }
            HitEnemy.Clear();
        }

        public IEnumerator PoisonEffect(EnemyBase hitEnemy)
        {
            // If the enemy has max stacks of poison, we don't apply the poison again.
            if (hitEnemy.CurrentPoisonStacks >= EnforceManager.poisonOverlapping) yield break;
    
            if (hitEnemy.RegistryType == EnemyBase.RegistryTypes.Poison) yield break;
            if (!EnforceManager.activatePoison) yield break;

            const float venomDuration = 2f;
            var poisonColor = new Color(0.18f, 1f, 0.1f);
    
            if (poisonDotDamage == 0) yield break;
    
            hitEnemy.GetComponent<SpriteRenderer>().DOColor(poisonColor, 0.2f);
    
            hitEnemy.CurrentPoisonStacks++; // Increment the poison count
    
            var elapsedTime = 0f;

            while (elapsedTime < venomDuration)
            {
                hitEnemy.ReceiveDamage(poisonDotDamage);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
    
            hitEnemy.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.2f); // Reset the color
            hitEnemy.CurrentPoisonStacks--; // Decrement the poison count
        }
    }
}