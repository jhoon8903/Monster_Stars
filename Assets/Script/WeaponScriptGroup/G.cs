using System.Collections;
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
        public float burnDotDamage;
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

        public IEnumerator BurningEffect(EnemyBase hitEnemy)
        {
            burnDotDamage = CharacterBase.unitGroup switch
            {
                CharacterBase.UnitGroups.G => Damage * (0.1f + EnforceManager.Instance.fire2BurningDamageBoost / 10f),
                CharacterBase.UnitGroups.H => Damage * 0.1f,
            };

            var burningColor = new Color(1f,0,0.4f);
            var burningDuration = CharacterBase.unitGroup switch
            {
               CharacterBase.UnitGroups.G => 2f,
               CharacterBase.UnitGroups.H => EnforceManager.Instance.fireBurnPerAttackEffect ? 5f : 0f,
            };
            var maxBurningStack = CharacterBase.unitGroup switch
            {
                CharacterBase.UnitGroups.G => EnforceManager.Instance.fire2BurnStackIncrease ? 1 : 4,
                CharacterBase.UnitGroups.H => EnforceManager.Instance.fireImageOverlapIncrease
            };
            hitEnemy.BurningStack++;
            if (hitEnemy.BurningStack > maxBurningStack) yield break;
            var elapsedTime = 0f;
            StartCoroutine(FlickerEffect(hitEnemy.GetComponent<SpriteRenderer>(), burningColor, burningDuration));
            while (elapsedTime < burningDuration)
            {
                var damage = DamageCalculator(burnDotDamage, hitEnemy, CharacterBase.UnitGroups.G); 
                hitEnemy.ReceiveDamage(hitEnemy, (int)damage, CharacterBase);
                yield return new WaitForSeconds(1f);
                elapsedTime += Time.deltaTime;
            }
            hitEnemy.BurningStack--;
            hitEnemy.isBurn = false;
        }
    }
}