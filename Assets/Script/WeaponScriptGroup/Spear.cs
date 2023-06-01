using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class Spear : WeaponBase
    {
        private const float MaxDistanceY = 9f;

        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            // DoTween을 이용하여 무기를 이동시키는 코드를 작성합니다.
            // 예를 들어, Y 방향으로 무기를 이동시킬 수 있습니다.
            Speed = 5f;
            transform.DOMoveY(MaxDistanceY, Speed).SetEase(Ease.Linear).OnComplete(() => StopUseWeapon(this.gameObject));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                StopUseWeapon(this.gameObject);
            }
        }
    }

}
