using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class Sword : WeaponBase
    {
        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            // DoTween을 이용하여 무기를 회전시키는 코드를 작성합니다.
            // 예를 들어, 360도 돌아가게 할 수 있습니다.
            Speed = 5f;
            transform.DORotate(new Vector3(0, 0, 360), Speed, RotateMode.FastBeyond360).OnComplete(() => StopUseWeapon(this.gameObject));
        }
    }

}
