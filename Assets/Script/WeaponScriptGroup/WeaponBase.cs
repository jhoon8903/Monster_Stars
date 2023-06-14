using System.Collections;
using System.Collections.Generic;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public abstract class WeaponBase : MonoBehaviour
    {
        public bool IsInUse { get; protected set; }
        protected float Speed { get; set; }
        protected float Damage { get; private set; }
        protected float Distance { get; private set; }
        protected CharacterBase.UnitProperties UnitProperty { get; private set; }
        protected CharacterBase.UnitEffects UnitEffect { get; private set; }
        protected Vector3 StartingPosition;
        protected CharacterBase CharacterBase;
        private readonly System.Random _random = new System.Random();
        

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

        protected void AtkEffect(EnemyBase enemyObject)
        {
            switch (UnitEffect)
            {
                case CharacterBase.UnitEffects.Restraint:
                    RestraintEffect(enemyObject);
                    break;
                case CharacterBase.UnitEffects.Poison:
                    PoisonEffect(enemyObject);
                    break;
                case CharacterBase.UnitEffects.Slow:
                    SlowEffect(enemyObject);
                    break;
            }
        }

        // 속박속성공격
        private void RestraintEffect(EnemyBase enemyStatus)
        {
            // 20% 확률로 이동 불가능 효과 적용
            if (_random.Next(100) < 20)
            {
                enemyStatus.IsRestraint = true;
            }

        }

        // 중독공격
        private static void PoisonEffect(EnemyBase enemyStatus)
        {
           enemyStatus.IsPoison = true;
        }

        // 감속효과
        private static void SlowEffect(EnemyBase enemyStatus)
        {
            enemyStatus.IsSlow = true;
            // if (_random.Next(100) < 20)
            // {
            //     
            // }
        }
    }
}

