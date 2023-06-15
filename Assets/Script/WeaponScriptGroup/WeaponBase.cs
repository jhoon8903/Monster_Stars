using System.Collections;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class WeaponBase : MonoBehaviour
    {
        [SerializeField] private WeaponsPool weaponsPool;
        public bool IsInUse { get; protected set; }
        protected float Speed { get; set; }
        protected float Damage { get; private set; }
        protected float Distance { get; private set; }
        protected CharacterBase.UnitProperties UnitProperty { get; private set; }
        private CharacterBase.UnitEffects UnitEffect { get; set; }
        protected Vector3 StartingPosition;
        protected CharacterBase CharacterBase;
        private readonly System.Random _random = new System.Random();
        private EnemyBase _poisonedEnemy;
        protected internal bool DivinePenetrate { get; set; } = false;
        protected int HitCount;
        protected internal bool DivinePoisonAdditionalDamage { get; set; } = false;
        protected internal bool PhysicIncreaseWeaponScale { get; set; } = false;
        protected internal bool PhysicSlowAdditionalDamage { get; set; } = false;

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
            }
        }

        // 속박속성공격
        private void RestraintEffect(EnemyBase enemyStatus)
        {
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
        }
    }
}

