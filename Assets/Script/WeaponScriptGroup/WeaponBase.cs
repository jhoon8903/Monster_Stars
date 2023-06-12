using System.Collections;
using Script.CharacterManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public abstract class WeaponBase : MonoBehaviour
    {
        public bool IsInUse { get; protected set; }
        protected float Speed { get; set; }
        protected float Damage { get; set; }
        protected float Distance { get; set; }
        protected CharacterBase.UnitProperties UnitProperty { get; set; }
        protected CharacterBase.UnitEffects UnitEffect { get; set; }
        protected Vector3 StartingPosition;
        protected CharacterBase CharacterBase;

        public void InitializeWeapon(CharacterBase characterBase)
        {
            CharacterBase = characterBase;
        }

        public virtual IEnumerator UseWeapon()
        {
            IsInUse = true;
            UnitProperty = CharacterBase.UnitProperty;
            UnitEffect = CharacterBase.UnitEffect;
            StartingPosition = transform.position;
            Distance = CharacterBase.defaultAtkDistance;
            Damage = CharacterBase.defaultDamage;
            Speed = CharacterBase.projectileSpeed;
            yield return null;
        }

        protected void StopUseWeapon(GameObject weapon)
        {
            IsInUse = false;
            FindObjectOfType<WeaponsPool>().ReturnToPool(weapon);
        }
    }
}

