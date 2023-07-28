using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class G : WeaponBase
    {
        public GameObject pivotPoint;
        public GameObject secondSword;
        private Tween _pivotTween;
        private int _maxStack; 
        private readonly Dictionary<EnemyBase, int> burnStacks = new Dictionary<EnemyBase, int>();

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
            if (HasHit) return;
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            HasHit = true;
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy, CharacterBase.UnitGroups.G); 
            enemy.ReceiveDamage(enemy,damage, CharacterBase);
        }

        public IEnumerator BurningGEffect(EnemyBase hitEnemy)
        {
            _maxStack = 1;
            if (!burnStacks.ContainsKey(hitEnemy))
            {
                burnStacks[hitEnemy] = 1;
            }
            else
            {
                burnStacks[hitEnemy] = Math.Min(burnStacks[hitEnemy] + 1, _maxStack);
            }

            var burnDotDamage = DamageCalculator(Damage, hitEnemy, CharacterBase.UnitGroups.G) * 0.2f * burnStacks[hitEnemy];
            var burningDuration = EnforceManager.Instance.fire2BurnDurationBoost ? 5:3;

            for (var i = 0; i < burningDuration; i++)
            {
                hitEnemy.ReceiveDamage(hitEnemy, (int)burnDotDamage , CharacterBase);
                yield return new WaitForSeconds(1f);
            }

            burnStacks[hitEnemy]--;

            if (burnStacks[hitEnemy] > 0) yield break;
            burnStacks.Remove(hitEnemy);
            hitEnemy.isBurnG = false;
            hitEnemy.IsBurnG = false;
        }
    }
}