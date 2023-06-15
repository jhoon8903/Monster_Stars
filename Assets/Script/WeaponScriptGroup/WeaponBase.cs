using System;
using System.Collections;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.WeaponScriptGroup
{
    public class WeaponBase : MonoBehaviour
    {
        public bool IsInUse { get; protected set; }
        protected float Speed { get; set; }
        protected float Damage { get; private set; }
        protected float Distance { get; private set; }
        protected CharacterBase.UnitProperties UnitProperty { get; private set; }
        private CharacterBase.UnitEffects UnitEffect { get; set; }
        protected Vector3 StartingPosition;
        protected CharacterBase CharacterBase;
        protected readonly System.Random Random = new System.Random();
        private EnemyBase _poisonedEnemy;
        public bool DivinePenetrate { get; set; } = false;
        public int hitCount;
        public bool DivinePoisonAdditionalDamage { get; set; } = false;
        public bool PhysicIncreaseWeaponScale { get; set; } = false;
        public bool PhysicSlowAdditionalDamage { get; set; } = false;
        public bool PoisonRestraintAdditionalDamage { get; set; } = false;
        public bool PoisonIncreaseTime { get; set; } = false;
        public bool PoisonInstantKill { get; set; } = false;
        public bool WaterRestraintKnockBack { get; set; } = false;

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
            WeaponsPool.ReturnToPool(weapon);
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
                case CharacterBase.UnitEffects.None:
                    break;
                default:
                    return;
            }
        }
        // 속박속성공격
        private void RestraintEffect(EnemyBase enemyStatus)
        {
            if (Random.Next(100) < 20)
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
        }
    }
}

