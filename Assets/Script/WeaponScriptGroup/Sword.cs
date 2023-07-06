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
        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            Speed = CharacterBase.swingSpeed;
            pivotPoint.transform.DORotate(new Vector3(0, 0, 360), Speed, RotateMode.FastBeyond360).OnComplete(() => StopUseWeapon(pivotPoint));
            if (!EnforceManager.Instance.physicAdditionalWeapon) yield break;
            secondSword.SetActive(true);
            secondSword.GetComponent<WeaponBase>().InitializeWeapon(CharacterBase);
            if(secondSword.activeInHierarchy) StartCoroutine(secondSword.GetComponent<WeaponBase>().UseWeapon());
            var weaponPool = FindObjectOfType<WeaponsPool>();
            weaponPool.SetSprite(WeaponsPool.WeaponType.Sword, CharacterBase.UnitInGameLevel, secondSword);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            AtkEffect(enemy);
            var damage = DamageCalculator(Damage, enemy);
            enemy.ReceiveDamage(enemy,damage);
        }
    }
}
