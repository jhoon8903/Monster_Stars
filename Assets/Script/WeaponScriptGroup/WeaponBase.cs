using System.Collections;
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
        protected float Damage { get; set; }
        protected float FireRate { get; set; }
        protected Vector3 StartingPosition;
        protected CharacterBase CharacterBase;

        public void InitializeWeapon(CharacterBase characterBase)
        {
            CharacterBase = characterBase;
        }

        public virtual IEnumerator UseWeapon()
        {
            IsInUse = true;
            StartingPosition = transform.position;
            Damage = CharacterBase.defaultDamage;
            FireRate = CharacterBase.defaultAtkRate;
            Speed = CharacterBase.projectileSpeed;
            yield return null;
        }

        protected void StopUseWeapon(GameObject weapon)
        {
            IsInUse = false;
            WeaponsPool.ReturnToPool(weapon);
        }
    }
}

