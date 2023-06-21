using System.Collections;
using System.Collections.Generic;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using Script.UIManager;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class WeaponBase : MonoBehaviour
    {
        public bool isInUse;
        protected float Speed { get; set; }
        protected float Damage { get; private set; }
        protected float Distance { get; private set; }
        private CharacterBase.UnitProperties UnitProperty { get; set; }
        private CharacterBase.UnitEffects UnitEffect { get; set; }
        protected Vector3 StartingPosition;
        protected CharacterBase CharacterBase;
        private readonly System.Random _random = new System.Random();
        private EnemyBase _poisonedEnemy;
        protected readonly List<EnemyBase> HitEnemy = new List<EnemyBase>();

        public void InitializeWeapon(CharacterBase characterBase)
        {
            CharacterBase = characterBase;
        }
        public virtual IEnumerator UseWeapon()
        {
            isInUse = true;
            UnitProperty = CharacterBase.UnitProperty;
            UnitEffect = CharacterBase.UnitEffect;
            StartingPosition = transform.position;
            Distance = CharacterBase.defaultAtkDistance;
            Damage = CharacterBase.DefaultDamage;
            Speed = CharacterBase.projectileSpeed * 2f;
            yield return null;
        }
        protected void StopUseWeapon(GameObject weapon)
        {
            isInUse = false;
            HitEnemy.Clear();
            WeaponsPool.ReturnToPool(weapon);
        }
        protected void AtkEffect(EnemyBase enemyObject)
        {
            if (enemyObject == null) return;
                switch (UnitEffect)
            {
                case CharacterBase.UnitEffects.Restraint:
                    RestraintAttribution(enemyObject);
                    break;
                case CharacterBase.UnitEffects.Poison:
                    PoisonAttribution(enemyObject);
                    break;
                case CharacterBase.UnitEffects.Slow:
                    SlowAttribution(enemyObject);
                    break;
                case CharacterBase.UnitEffects.None:
                    PhysicsAttribution(enemyObject);
                    break;
                default:
                    Debug.Log("UnKnown AtkEffect");
                    return;
            }
        }
        private void RestraintAttribution(EnemyBase enemyStatus)
        {
            switch (enemyStatus.RegistryType)
            {
                case EnemyBase.RegistryTypes.Physics:
                    IsRestraint(enemyStatus);
                    break;
                case EnemyBase.RegistryTypes.Poison:
                    IsRestraint(enemyStatus);
                    break;
                case EnemyBase.RegistryTypes.None:
                    IsRestraint(enemyStatus);
                    break;
                case EnemyBase.RegistryTypes.Divine:
                    break;
                default:
                    Debug.Log("UnKnown Registries");
                    break;
            }
        }
        private static void PoisonAttribution(EnemyBase enemyStatus)
        {
            switch (enemyStatus.RegistryType)
            {
                case EnemyBase.RegistryTypes.Divine:
                    IsPoison(enemyStatus);
                    break;
                case EnemyBase.RegistryTypes.Physics:
                    IsPoison(enemyStatus);
                    break;
                case EnemyBase.RegistryTypes.None:
                    IsPoison(enemyStatus);
                    break;
                case EnemyBase.RegistryTypes.Poison:
                    break;
                default:
                    Debug.Log("UnKnown Registries");
                    break;
            }
        }
        private static void SlowAttribution(EnemyBase enemyStatus)
        {
            switch (enemyStatus.RegistryType)
            {
                case EnemyBase.RegistryTypes.Divine:
                    IsSlow(enemyStatus);
                    break;
                case EnemyBase.RegistryTypes.Physics:
                    IsSlow(enemyStatus);
                    break;
                case EnemyBase.RegistryTypes.Poison:
                    IsSlow(enemyStatus);
                    break;
                case EnemyBase.RegistryTypes.None:
                    IsSlow(enemyStatus);
                    break;
                default:
                    Debug.Log("UnKnown Registries");
                    break;
            }
        }
        private static void PhysicsAttribution(EnemyBase enemyStatus)
        {
            switch (enemyStatus.RegistryType)
            {
                case EnemyBase.RegistryTypes.Divine:
                case EnemyBase.RegistryTypes.Physics:
                case EnemyBase.RegistryTypes.Poison:
                case EnemyBase.RegistryTypes.None:
                    break;
                default:
                    Debug.Log("UnKnown Registries");
                    break;
            }
        }
        private void IsRestraint(EnemyBase enemyStatus)
        {
            if (!EnforceManager.Instance.activeRestraint) return;
            if (_random.Next(100) < 20)
            {
                enemyStatus.isRestraint = true;
            }
        }
        private static void IsSlow(EnemyBase enemyStatus)
        {
            enemyStatus.isSlow = true;
        }
        private static void IsPoison(EnemyBase enemyStatus)
        {
            enemyStatus.IsPoison = true;
        }
        protected float DamageCalculator(float damage,EnemyBase enemyBase)
        {
            switch (UnitProperty)
            {
                case CharacterBase.UnitProperties.Divine:
                    if (enemyBase.RegistryType != EnemyBase.RegistryTypes.Divine)
                    {
                        if (EnforceManager.Instance.divinePoisonAdditionalDamage && enemyBase.IsPoison)
                        {
                            var increaseDamage = 1f + (0.3f * EnforceManager.Instance.divinePoisonAdditionalDamageCount );
                            damage *= increaseDamage;
                        }
                        return damage;
                    } 
                    
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Divine)
                    {
                        damage *= 0.8f;
                    }
                    return damage;

                case CharacterBase.UnitProperties.Physics:
                    if (enemyBase.RegistryType != EnemyBase.RegistryTypes.Physics)
                    {
                        if (EnforceManager.Instance.physicSlowAdditionalDamage && enemyBase.isSlow)
                        {
                            damage *= 2.0f;
                        }

                        if (EnforceManager.Instance.physicIncreaseDamage)
                        {
                            damage *= damage * (1f + EnforceManager.Instance.increasePhysicsDamage);
                        }
                        return damage;
                    }

                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Physics)
                    {
                        damage *= 0.8f;
                    }
                    return damage;

                case CharacterBase.UnitProperties.Poison:
                    if (enemyBase.RegistryType != EnemyBase.RegistryTypes.Poison)
                    {
                        if (EnforceManager.Instance.poisonRestraintAdditionalDamage && enemyBase.isRestraint)
                        {
                            damage *= 2.0f;
                        }
                        return damage;
                    }

                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Poison)
                    {
                        damage *= 0.8f;
                    }
                    return damage;

                case CharacterBase.UnitProperties.Water:

                        damage += damage * EnforceManager.Instance.IncreaseWaterDamage;
                        return damage;
            }
           
            return damage;
        }
        protected static void InstantKill(EnemyBase target)
        {
            FindObjectOfType<EnemyBase>().EnemyKilledEvents(target);
            if (ExpManager.Instance == null) return;
            const EnemyBase.KillReasons reason = EnemyBase.KillReasons.ByPlayer;
            ExpManager.Instance.HandleEnemyKilled(reason);
        }
    }
}

