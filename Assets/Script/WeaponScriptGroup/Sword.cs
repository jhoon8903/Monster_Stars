using System.Collections;
using DG.Tweening;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class Sword : WeaponBase
    {
        public GameObject pivotPoint;
        public GameObject secondSword;

        private Tween _pivotTween; // Add this line to store the Tween

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();

            Speed = CharacterBase.swingSpeed;

            if (_pivotTween == null || _pivotTween.IsComplete())
            {
                _pivotTween = pivotPoint.transform.DORotate(new Vector3(0, 0, 360), Speed, RotateMode.FastBeyond360);
                _pivotTween.OnComplete(() => {
                    StopUseWeapon(pivotPoint);
                    _pivotTween = null; // Reset the tween so it can be recreated next time
                }).SetAutoKill(false); // Do not kill the tween when it's complete
            }
            if (!_pivotTween.IsActive() || _pivotTween.IsComplete())
            {
                _pivotTween.Restart();
            }

            if (!EnforceManager.Instance.physicAdditionalWeapon) yield break;

            secondSword.SetActive(true);
            secondSword.GetComponent<WeaponBase>().InitializeWeapon(CharacterBase);

            if (secondSword.activeInHierarchy) 
                StartCoroutine(secondSword.GetComponent<WeaponBase>().UseWeapon());

            var weaponPool = FindObjectOfType<WeaponsPool>();
            weaponPool.SetSprite(WeaponsPool.WeaponType.Sword, CharacterBase.unitPuzzleLevel, secondSword);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;

            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            AtkEffect(enemy);

            var damage = DamageCalculator(Damage, enemy);
            enemy.ReceiveDamage(enemy, damage);
        }
    }
}