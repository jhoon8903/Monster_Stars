using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class FireBall : WeaponBase
    {
        private float _distance;
        private Vector3 _enemyTransform;
        private List<GameObject> _enemyTransforms = new List<GameObject>();
        public float burnDotDamage;
        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            _enemyTransforms = CharacterBase.GetComponent<UnitG>().DetectEnemies();
            foreach (var enemy in _enemyTransforms)
            {
                _enemyTransform = enemy.transform.position;
            }

            _distance = Vector3.Distance(transform.position, _enemyTransform);
            var adjustedSpeed = Speed * _distance;
            var timeToMove = _distance / adjustedSpeed;
            transform.DOMove(_enemyTransform, timeToMove).SetEase(Ease.Linear).OnComplete(() => StopUseWeapon(gameObject));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy); 
            enemy.ReceiveDamage(enemy,damage);
            StopUseWeapon(gameObject);
        }

        public IEnumerator BurningEffect(EnemyBase hitEnemy)
        {
            burnDotDamage = Damage * 0.1f;
            var burningColor = new Color(1f,0,0.4f);
            hitEnemy.BurningStack++;
            if (hitEnemy.BurningStack > 1) yield break;
            const float burningDuration = 2f;
            var elapsedTime = 0f;
            StartCoroutine(FlickerEffect(hitEnemy.GetComponent<SpriteRenderer>(), burningColor, burningDuration));
            while (elapsedTime < burningDuration)
            {
                hitEnemy.ReceiveDamage(hitEnemy, burnDotDamage);
                yield return new WaitForSeconds(1f);
                elapsedTime += Time.deltaTime;
            }
            hitEnemy.BurningStack--;
            hitEnemy.isBurn = false;
        }
    }
}