using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;
namespace Script.WeaponScriptGroup
{
    public class Spear : WeaponBase
    {
        private Vector3 _enemyTransform;
        private List<GameObject> _enemyTransforms = new List<GameObject>();

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            _enemyTransforms = CharacterBase.GetComponent<UnitA>().DetectEnemies();
            foreach (var enemy in _enemyTransforms)
            {
                _enemyTransform = enemy.transform.position;
            }

            var position = transform.position;
            var timeToMove = Speed * 1.5f;

            if (EnforceManager.Instance.divineAtkRange)
            {
                transform.rotation = Quaternion.Euler(0, 0, _enemyTransform.y > position.y ? 0 : 180);
            }
            transform.DOMove(_enemyTransform, timeToMove).SetEase(Ease.Linear).OnComplete(() => StopUseWeapon(gameObject));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy == null || HitEnemy.Contains(enemy)) return; // Skip if it's already hit
            HitEnemy.Add(enemy);
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy);
            enemy.ReceiveDamage(enemy,damage);
            switch (EnforceManager.Instance.divinePenetrate)
            {
                case true when HitEnemy.Count == 2:
                    StopUseWeapon(gameObject);
                    HitEnemy.Clear(); // Only clear when the weapon stops
                    break;
                case false:
                    StopUseWeapon(gameObject);
                    HitEnemy.Clear();
                    break;
            }
        }
    }
}