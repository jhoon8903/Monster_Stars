using System.Collections;
using System.Collections.Generic;
using Script.CharacterManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public abstract class WeaponBase : MonoBehaviour
    {
        public bool IsInUse { get; protected set; }

        // 무기의 속도, 데미지, 생성 속도 등의 속성값을 저장할 변수들을 선언합니다.
        // 예를 들어, 아래와 같이 선언할 수 있습니다.
        protected float Speed { get; set; }
        public int Damage { get; set; }
        private float FireRate { get; set; }

        protected Vector3 startingPosition;

        public virtual IEnumerator UseWeapon()
        {
            FireRate = 0.7f;
            IsInUse = true;
            startingPosition = transform.position;
            yield return new WaitForSecondsRealtime(FireRate);
        }

        protected void StopUseWeapon(GameObject weapon)
        {

            IsInUse = false;
            WeaponsPool.ReturnToPool(weapon);
        }
    }

}

