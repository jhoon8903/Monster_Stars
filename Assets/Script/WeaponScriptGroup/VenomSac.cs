using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.CharacterGroupScript;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class VenomSac : WeaponBase
    {
        private float _distance;
        private Vector3 _enemyTransform = new Vector3();
        private List<GameObject> _enemyTransforms = new List<GameObject>();

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
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                enemy.ReceiveDamage(Damage, UnitProperty, UnitEffect);
                var hitColliders = Physics2D.OverlapCircleAll(collision.transform.position, 1f);
                foreach (var hitCollider in hitColliders)
                {
                    var hitEnemy = hitCollider.gameObject.GetComponent<EnemyBase>();
                    if (hitEnemy == null || !hitEnemy.gameObject.activeInHierarchy || hitEnemy == enemy) continue;
                    hitEnemy.ReceiveDamage(Damage / 2, UnitProperty, UnitEffect);
                    AtkEffect(hitEnemy);
                    StartCoroutine(PoisonEffect(hitEnemy)); 
                }
            }
            StopUseWeapon(gameObject);
        }

        public static IEnumerator PoisonEffect(EnemyBase hitEnemy)
        {
            const float duration = 2f; // duration of the poison effect
            for (float time = 0; time < duration; time += 0.5f)
            {
                hitEnemy.ReceiveDamage(10, CharacterBase.UnitProperties.Poison, CharacterBase.UnitEffects.Poison);
                yield return new WaitForSeconds(0.5f);
            }
            hitEnemy.IsPoison = false;  // end the poison effect after the duration
        }
    }
}