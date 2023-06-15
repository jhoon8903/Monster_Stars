using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.CharacterGroupScript;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.UIManager;
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
            _enemyTransforms = CharacterBase.GetComponent<Unit_F>().DetectEnemies();
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
            var enemy = collision.gameObject.GetComponent<EnemyBase>();

            if (PoisonInstantKill && enemy.healthPoint < (enemy.maxHealthPoint * 0.15f))
            {
                if (Random.Next(100) < 15)
                {
                    WeaponsPool.ReturnToPool(gameObject);
                    FindObjectOfType<WaveManager>().EnemyDestroyInvoke();
                    if (ExpManager.Instance != null)
                    {
                        const EnemyBase.KillReasons reason = EnemyBase.KillReasons.ByPlayer;
                        ExpManager.Instance.HandleEnemyKilled(reason);
                    }
                }
            }

            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                if (enemy.IsRestraint && PoisonRestraintAdditionalDamage)
                {
                    enemy.ReceiveDamage(Damage * 2f, UnitProperty);
                }
                else
                {
                    enemy.ReceiveDamage(Damage, UnitProperty);
                }
                
                var hitColliders = Physics2D.OverlapCircleAll(collision.transform.position, 1f);
                foreach (var hitCollider in hitColliders)
                {
                    var hitEnemy = hitCollider.gameObject.GetComponent<EnemyBase>();
                    if (hitEnemy == null || !hitEnemy.gameObject.activeInHierarchy || hitEnemy == enemy) continue;
                    if (enemy.IsRestraint && PoisonRestraintAdditionalDamage)
                    {
                        enemy.ReceiveDamage(Damage * 2f, UnitProperty);
                    }
                    else
                    {
                        enemy.ReceiveDamage(Damage, UnitProperty);
                    }
                    AtkEffect(hitEnemy);
                }
            }
            StopUseWeapon(gameObject);
        }
        public IEnumerator PoisonEffect(EnemyBase hitEnemy)
        {
            if (hitEnemy.RegistryType == EnemyBase.RegistryTypes.Poison) yield break;
            var venomDuration = 2f;
            if (PoisonIncreaseTime)
            {
               venomDuration = 4f; // duration of the poison effect
            }

            var poisonColor = new Color(0.18f, 1f, 0.1f);

            if (poisonDotDamage == 0) yield break;
    
            hitEnemy.GetComponent<SpriteRenderer>().DOColor(poisonColor, 0.2f);
    
            var elapsedTime = 0f;

            if (hitEnemy.IsRestraint && PoisonRestraintAdditionalDamage)
            {
                poisonDotDamage *= 2f;
            }

            while (elapsedTime < venomDuration)
            {
                hitEnemy.ReceiveDamage(poisonDotDamage, CharacterBase.UnitProperties.Poison);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
    
            hitEnemy.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.2f); // Reset the color
            hitEnemy.IsPoison = false; // Set IsPoison to false to stop the poison effect
        }
    }
}