using System.Collections;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class Spear : WeaponBase
    {
        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
             
            hitCount = 0;
            var duration = Distance / Speed;
            var endPosition = StartingPosition.y + (CharacterBase.DivineAtkRange? -Distance : Distance);
            transform.rotation = Quaternion.Euler(0,0,CharacterBase.DivineAtkRange? 180: 0);
            transform.DOMoveY(endPosition, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => {
                    // 버프 조건 확인 요망
                    if (DivinePenetrate && hitCount==2 && UnitProperty == CharacterBase.UnitProperties.Divine)
                    {
                        StopUseWeapon(gameObject);
                    }
                    else
                    {
                        StopUseWeapon(gameObject);
                    }
                });
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            var enemy = collision.gameObject.GetComponent<EnemyBase>();
            // 버프 조건 확인 요망
            hitCount++;
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                if (enemy.IsPoison && DivinePoisonAdditionalDamage)
                {
                    enemy.ReceiveDamage(Damage * 1.5f, UnitProperty);
                }
                else
                {
                    enemy.ReceiveDamage(Damage, UnitProperty);
                }
                AtkEffect(enemy);
            }
            
            // 버프 조건 확인 요망  
            if (!DivinePenetrate || UnitProperty != CharacterBase.UnitProperties.Divine || hitCount >=2)
            {
                StopUseWeapon(gameObject);
            }
        }
    }
}
