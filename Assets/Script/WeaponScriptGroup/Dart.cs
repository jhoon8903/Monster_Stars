using System.Collections;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;
namespace Script.WeaponScriptGroup
{
    public class Dart : WeaponBase
    {
        private float _distance;
        private Vector3 _enemyTransform;
        private Rigidbody2D _rigidbody2D;
        public float bleedDotDamage;
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, Speed);
            var useTime = Distance / Speed;
            yield return new WaitForSeconds(useTime);
            StopUseWeapon(gameObject);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy);
            enemy.ReceiveDamage(enemy,damage);
            switch (EnforceManager.Instance.physics2ProjectilePenetration)
            {
                case true when HitEnemy.Count == 2:
                    StopUseWeapon(gameObject);
                    HitEnemy.Clear();
                    break;
                case false:
                    StopUseWeapon(gameObject);
                    HitEnemy.Clear();
                    break;
            }
        }

        public IEnumerator BleedEffect(EnemyBase hitEnemy)
        {
            bleedDotDamage = Damage * 0.1f;
            var bleedingColor = new Color(1f,0.127f,0.207f);
            var elapsedTime = 0f;
            const float bleedDuration = 2f;
            hitEnemy.BleedingStack++;
            if (hitEnemy.BleedingStack > EnforceManager.Instance.physics2AdditionalBleedingLayer) yield break;
            StartCoroutine(FlickerEffect(hitEnemy.GetComponent<SpriteRenderer>(), bleedingColor, bleedDuration));
            while (elapsedTime < bleedDuration)
            {
                hitEnemy.ReceiveDamage(hitEnemy, bleedDotDamage);
                yield return new WaitForSeconds(1f);
                elapsedTime += Time.deltaTime;
            }
            hitEnemy.BleedingStack--;
            hitEnemy.isBleed = false;
        }
    }
}