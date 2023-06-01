using System.Collections;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class VenomSac : WeaponBase
    {
        public override IEnumerator UseWeapon()
        {
            yield return base.UseWeapon();
            // VenomSac는 케릭터 기준 1.5f 의 원형공간이 공격범위인데 이를 어떻게 구현할지는 추가 정보가 필요합니다.
            // 일단 여기서는 VenomSac가 Enemy 태그를 가진 오브젝트와 충돌하면 WeaponPool에 반환되도록 하였습니다.
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
