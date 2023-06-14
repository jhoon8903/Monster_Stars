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
        public float poisonDotDamage = 10f;

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
                enemy.ReceiveDamage(Damage, UnitProperty);
                var hitColliders = Physics2D.OverlapCircleAll(collision.transform.position, 1f);
                foreach (var hitCollider in hitColliders)
                {
                    var hitEnemy = hitCollider.gameObject.GetComponent<EnemyBase>();
                    if (hitEnemy == null || !hitEnemy.gameObject.activeInHierarchy || hitEnemy == enemy) continue;
                    hitEnemy.ReceiveDamage(Damage, UnitProperty);
                    AtkEffect(hitEnemy);
                }
            }
            StopUseWeapon(gameObject);
        }
        public IEnumerator PoisonEffect(EnemyBase hitEnemy)
        {
            const float duration = 2f; // duration of the poison effect
            var poisonColor = new Color(0.18f, 1f, 0.1f);

            if (poisonDotDamage == 0) yield break;
            hitEnemy.GetComponent<SpriteRenderer>().DOColor(poisonColor, 0.2f);
            for (float time = 0; time < duration; time += 0.5f)
            {
                hitEnemy.ReceiveDamage(poisonDotDamage, CharacterBase.UnitProperties.Poison);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}