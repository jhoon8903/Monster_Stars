using System.Collections;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class D : WeaponBase
    {
        public GameObject pivotPoint;
        public GameObject secondSword;
        public float bleedDotDamage;
        private Tween _pivotTween;

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();

            Speed = CharacterBase.swingSpeed;

            if (_pivotTween == null || _pivotTween.IsComplete())
            {
                if (EnforceManager.Instance.physicalSwordAddition)
                {
                    secondSword.SetActive(true);
                    secondSword.GetComponent<WeaponBase>().InitializeWeapon(CharacterBase);
                }
                _pivotTween = pivotPoint.transform.DORotate(new Vector3(0, 0, 360), Speed, RotateMode.FastBeyond360);
                _pivotTween.OnComplete(() => {
                    StopUseWeapon(pivotPoint);
                    _pivotTween = null; 
                }).SetAutoKill(false); 
            }
            if (!_pivotTween.IsActive() || _pivotTween.IsComplete())
            {
                _pivotTween.Restart();
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy, CharacterBase.UnitGroups.G); 
            enemy.ReceiveDamage(enemy,damage, CharacterBase);
        }

        public IEnumerator BleedEffect(EnemyBase hitEnemy)
        {
            bleedDotDamage = DamageCalculator(Damage, hitEnemy, CharacterBase.UnitGroups.D) * 0.1f;
            const float bleedDuration = 2f;
            for(var i = 0; i < bleedDuration; i++)
            {
                hitEnemy.ReceiveDamage(hitEnemy, (int)bleedDotDamage, CharacterBase);
                yield return new WaitForSeconds(1f);
            }
            hitEnemy.isBleed = false;
            hitEnemy.IsBleed = false;
        }
    }
}