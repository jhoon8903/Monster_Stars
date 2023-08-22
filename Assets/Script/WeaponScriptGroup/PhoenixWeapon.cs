using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class PhoenixWeapon : WeaponBase
    {
        public GameObject pivotPoint;
        public GameObject secondSword;
        private Tween _pivotTween;

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();

            Speed = CharacterBase.swingSpeed;

            if (_pivotTween == null || _pivotTween.IsComplete())
            {
                if (EnforceManager.Instance.orcSwordAddition)
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
            AtkEffect(enemy, CharacterBase);
            var damage = DamageCalculator(Damage, enemy, CharacterBase); 
            enemy.ReceiveDamage(enemy,damage, CharacterBase);
        }
    }
}